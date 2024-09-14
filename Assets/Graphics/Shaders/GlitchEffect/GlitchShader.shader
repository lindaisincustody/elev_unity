Shader "Custom/GlitchShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _GlitchAmount ("Glitch Amount", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _GlitchAmount;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : COLOR
            {
                half4 col = tex2D(_MainTex, i.uv);

                // Generate a glitch effect using UV coordinates
                float glitch = abs(sin(i.uv.x * 10.0 + _Time.y * 5.0)) * _GlitchAmount;
                if (glitch > 0.5)
                {
                    // Apply glitch by offsetting UVs
                    col.rgb = tex2D(_MainTex, i.uv + float2(0.1, 0)).rgb;
                }

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
