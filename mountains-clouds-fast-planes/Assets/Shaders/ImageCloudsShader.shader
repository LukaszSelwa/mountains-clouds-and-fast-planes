Shader "Hidden/ImageCloudsShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            float rayYPlaneDst(float planeY, float3 rayOrigin, float3 rayDir) {
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
            float BottomPlane;
            float TopPlane;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 rayOrigin = _WorldSpaceCameraPos;
                float viewLength = length(i.viewVector);
                float3 rayDir = i.viewVector / viewLength;

                // Depth towards other object in the scene
                float nonlin_depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float depth = LinearEyeDepth(nonlin_depth) * viewLength;
                
                float dstToTopPlane = rayYPlaneDst(TopPlane, rayOrigin, rayDir);
                float dstToBottomPlane = rayYPlaneDst(BottomPlane, rayOrigin, rayDir);

                float dstFront = min(dstToTopPlane, dstToBottomPlane);
                float dstBack = min(max(dstToTopPlane, dstToBottomPlane), depth);

                if (dstBack > 0 && dstFront < depth) {
                    float density = dstBack - max(0, dstFront);
                    float fog = 1 / (0.2 * density + 1);
                    col = 1 - fog + col * fog;
                }
                return col;
            }
            ENDCG
        }
    }
}
