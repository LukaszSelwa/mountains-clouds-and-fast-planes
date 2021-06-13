Shader "Custom/MountainsShader"
{
    Properties
    {
        _RockColor  ("Rock Color",  Color) = (1,1,1,1)
        _GrassColor ("Grass Color", Color) = (1,1,1,1)
        _RockTex  ("Rock Texture",  2D) = "white" {}
        _GrassTex ("Grass Texture", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _GrassDirection ("Grass Direction", Vector) = (0, 1, 0, 0)
        _Threshold ("Threshold", Range(0,1)) = 0
        _Blend ("Blend", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _RockTex;
        sampler2D _GrassTex;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
            float2 uv_RockTex;
            float2 uv_GrassTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _RockColor;
        fixed4 _GrassColor;
        float3 _GrassDirection;
        float _Threshold;
        float _Blend;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float slope = saturate(dot(IN.worldNormal, _GrassDirection));
            float grassFactor = saturate((slope - _Threshold) / (_Blend * (1-_Threshold)));


            // Albedo comes from a texture tinted by color
            fixed4 rock = tex2D (_RockTex, IN.uv_RockTex) * _RockColor;
            fixed4 grass = tex2D (_GrassTex, IN.uv_GrassTex) * _GrassColor;
            fixed4 c = lerp(rock, grass, grassFactor);
            o.Albedo = c.rgb;
            
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
