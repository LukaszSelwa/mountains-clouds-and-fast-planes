Shader "Hidden/ImageCloudsShader"
{
    Properties
    {
        _MainTex  ("Texture", 2D) = "white" {}
        _CloudDarkColor ("Cloud Dark Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define MAX_STEP_COUNT 10
            #define LIGHT_SAMPLES 3

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewVector : TEXCOORD1;
            };

            // Distance from ray origin to plane (0*X + planeY * Y + 0*Z = 0) looking towards ray direction.
            // Vector rayDir should be normalized
            inline float rayYPlaneDst(float planeY, float3 rayOrigin, float3 rayDir) {
                float t = (planeY - rayOrigin.y) / rayDir.y;
                // float3 dstVec = t * rayDir;
                return t;
            }

            v2f vert (appdata v)
            {
                v2f o; 
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                // Camera space matches OpenGL convention where cam forward is -z. In unity forward is positive z.
                // (https://docs.unity3d.com/ScriptReference/Camera-cameraToWorldMatrix.html)
                float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
                o.viewVector = mul(unity_CameraToWorld, float4(viewVector,0));
                return o;
            }

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            
            Texture3D<float4> _NoiseTex;
            Texture2D<float4> _FluidTex;

            // samplers
            SamplerState my_linear_repeat_sampler;
            SamplerState fluid_linear_repeat_sampler;
            
            // Planes constants
            float _BottomPlane;
            float _TopPlane;
            float _TopDeclinePlane;
            float _BottomDeclinePlane;


            float _ScaleLargeWorley;
            float _ScaleSmallWorley;
            float _FogFactor;
            float _MinLightScatter;
            float _MaxLightScatter;
            float _LightFactor;
            float _CloudThreshold;
            float _SmallNoiseFactor;

            fixed4 _DistFogColor;
            fixed4 _CloudDarkColor;

            float _DistortOffset;

            float _DistanceFogFactor;

            float sampleCloudNoise(float3 pos) {
                float large = _NoiseTex.Sample(my_linear_repeat_sampler, pos * _ScaleLargeWorley).r;
                float small = _NoiseTex.Sample(my_linear_repeat_sampler, pos * _ScaleSmallWorley).g;
                float sample = lerp(large, small, _SmallNoiseFactor);

                if (pos.y > _TopDeclinePlane) {
                    sample = sample * (_TopPlane - pos.y) / (_TopPlane - _TopDeclinePlane);
                }
                if (pos.y < _BottomDeclinePlane) {
                    sample = sample * (pos.y - _BottomPlane) / (_BottomDeclinePlane - _BottomPlane);
                }
                sample = max(0, sample - _CloudThreshold) / (1 - _CloudThreshold);
                return sample;
            }

            float sampleLight(float3 pos, float3 lightDir) {
                float dstToTopPlane = rayYPlaneDst(_TopPlane, pos, -lightDir);
                float density = 0.0;
                float step = dstToTopPlane / LIGHT_SAMPLES;
                for (int i = 0; i < LIGHT_SAMPLES; ++i) {
                    density += sampleCloudNoise(pos);
                    pos += -step * lightDir;
                }
                return step * density;
            }

            float3 sampleDistortNoise(uint2 uv) {
                float2 pos = float2(uv.x*_ScreenParams.x, uv.y*_ScreenParams.y);
                pos = pos / 1000;
                return _FluidTex.Sample(fluid_linear_repeat_sampler, pos);
            } 

            float2 sampleDensity(float3 pos, float3 rayDir, float distance, float3 lightDir) {
                float density = 0.0;
                float sample;
                float step = distance / MAX_STEP_COUNT;
                float scatteredlight = 0.0;
                float transmittance = 1.0;
                float lightScatterCoef = lerp(_MinLightScatter, _MaxLightScatter, length(cross(lightDir, -rayDir)));
                for (int i = 0; i <= MAX_STEP_COUNT; ++i) {
                    sample = sampleCloudNoise(pos);
                    density += step * sample.r;
                    if (sample > 0) {
                        transmittance *= exp(-step * sample * _FogFactor);
                        scatteredlight += exp(-lightScatterCoef * (sampleLight(pos, lightDir))) * density * step * transmittance;
                    }
                    pos += step * rayDir;
                } 
                return float2(transmittance, step * scatteredlight);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 rayOrigin = _WorldSpaceCameraPos;
                float viewLength = length(i.viewVector);
                float3 rayDir = i.viewVector / viewLength;

                float3 lightDirection = -_WorldSpaceLightPos0.xyz;

                // Depth towards other object in the scene
                float nonlin_depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float depthLinear = max(LinearEyeDepth(nonlin_depth) * viewLength, 0);
                // Ignore the value of depth if it reaches its maximum 
                bool infiniteDepth = Linear01Depth(nonlin_depth) >= 1.0;
                
                float dstToTopPlane = rayYPlaneDst(_TopPlane, rayOrigin, rayDir);
                float dstToBottomPlane = rayYPlaneDst(_BottomPlane, rayOrigin, rayDir);

                float dstFront = max(min(dstToTopPlane, dstToBottomPlane), 0);
                float dstBack = max(dstToTopPlane, dstToBottomPlane);

                // Case when object is inside the cloud
                if (!infiniteDepth)
                    dstBack = min(dstBack, depthLinear);

                // Draw if ray intersects with cloud
                if (dstBack >= 0 && (dstFront <= depthLinear || infiniteDepth)) {                    
                    float cloudRayLen = dstBack - dstFront;

                    float3 pos = rayOrigin + rayDir * dstFront;
                    pos += _DistortOffset * sampleDistortNoise(i.uv);

                    float2 march = sampleDensity(pos, rayDir, cloudRayLen, lightDirection);
                    fixed4 lightCol = lerp(1, _CloudDarkColor, 1/(_LightFactor * march.y + 1));
                    col = saturate(col * march.x + lightCol * (1 - march.x));
                    
                    // add distance fog for far clouds
                    float distFog = exp(-_DistanceFogFactor * cloudRayLen * 0.001);
                    col = lerp(_DistFogColor, col, distFog);
                }
                return col;
            }
            ENDCG
        }
    }
}
