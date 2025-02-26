Shader "Custom/CRTEffectTrippy"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScanlineIntensity ("Scanline Intensity", Range(0,1)) = 0.5
        _NoiseIntensity ("Noise Intensity", Range(0,1)) = 0.3
        _Curvature ("Screen Curvature", Range(0,1)) = 0.2
        _Distortion ("Distortion", Range(0,1)) = 0.2
        _ColorShift ("Color Shift", Range(0,1)) = 0.5
        _Speed ("Effect Speed", Range(0,5)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _ScanlineIntensity;
            float _NoiseIntensity;
            float _Curvature;
            float _Distortion;
            float _ColorShift;
            float _Speed;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Use _Time.y multiplied by speed for dynamic effects.
                float time = _Time.y * _Speed;

                // Convert UVs to range [-1, 1] for distortion.
                float2 uv = i.uv * 2.0 - 1.0;

                // Apply curvature distortion.
                uv += uv * _Curvature * (uv.yx);

                // Add dynamic distortion using sine and cosine functions.
                uv.x += sin(uv.y * 10.0 + time) * _Distortion;
                uv.y += cos(uv.x * 10.0 + time) * _Distortion;

                // Convert UVs back to [0,1].
                uv = uv * 0.5 + 0.5;

                // Sample the base texture.
                half4 col = tex2D(_MainTex, uv);

                // Add scanline effect (with dynamic offset).
                float scan = sin(i.uv.y * 800.0 + time * 20.0) * _ScanlineIntensity;
                col.rgb -= scan;

                // Add noise effect.
                float2 noiseUV = i.uv * 100.0 + float2(time, time);
                float noise = frac(sin(dot(noiseUV, float2(12.9898, 78.233))) * 43758.5453);
                col.rgb += noise * _NoiseIntensity;

                // Add a color shift: sample slightly shifted UVs for R and B channels.
                float shift = sin(time) * _ColorShift * 0.02;
                float4 colR = tex2D(_MainTex, uv + float2(shift, 0.0));
                float4 colG = tex2D(_MainTex, uv);
                float4 colB = tex2D(_MainTex, uv - float2(shift, 0.0));
                // Blend original with the shifted colors.
                col.rgb = lerp(col.rgb, float3(colR.r, colG.g, colB.b), _ColorShift);

                return col;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
