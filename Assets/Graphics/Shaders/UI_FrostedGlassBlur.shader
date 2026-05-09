Shader "UI/FrostedGlassBlur"
{
    Properties
    {
        // Set automatically by BackgroundBlurCapture.cs — do not assign manually.
        _MainTex       ("Captured Background", 2D)           = "black" {}

        _BlurSize      ("Blur Amount",         Range(0, 10)) = 3.0
        _TintColor     ("Tint Color",          Color)        = (0.04, 0.07, 0.18, 0.45)
        _Brightness    ("Brightness",          Range(0, 1.5))= 0.72
        _Saturation    ("Saturation",          Range(0, 2))  = 0.55
        _ChromaShift   ("Chromatic Shift",     Range(0, 3))  = 0.8
        _VignetteStr   ("Vignette Strength",   Range(0, 1))  = 0.25
        _NoiseStr      ("Grain Strength",      Range(0,0.06))= 0.018
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

        Pass
        {
            ZWrite Off
            ZTest [unity_GUIZTestMode]
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_TexelSize;   // auto-filled: (1/w, 1/h, w, h)

            float  _BlurSize;
            float4 _TintColor;
            float  _Brightness;
            float  _Saturation;
            float  _ChromaShift;
            float  _VignetteStr;
            float  _NoiseStr;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv     = v.uv;
                o.color  = v.color;
                return o;
            }

            // Gaussian weights (sigma ≈ 1.3, normalised 1-D, 5-tap)
            static const float GW[3] = { 0.3829, 0.2417, 0.0538 };

            float4 GaussianBlur(float2 uv, float2 ts, float size)
            {
                // Centre sample
                float4 col = tex2D(_MainTex, uv) * (GW[0] * GW[0]);

                // Axis-aligned taps
                [unroll]
                for (int r = 1; r <= 2; r++)
                {
                    float  w  = GW[0] * GW[r];
                    float2 ox = float2(r * size * ts.x, 0);
                    float2 oy = float2(0, r * size * ts.y);
                    col += tex2D(_MainTex, uv + ox) * w;
                    col += tex2D(_MainTex, uv - ox) * w;
                    col += tex2D(_MainTex, uv + oy) * w;
                    col += tex2D(_MainTex, uv - oy) * w;
                }

                // Diagonal taps
                [unroll]
                for (int x = 1; x <= 2; x++)
                {
                    [unroll]
                    for (int y = 1; y <= 2; y++)
                    {
                        float  w = GW[x] * GW[y];
                        float2 d = float2(x * size * ts.x, y * size * ts.y);
                        col += tex2D(_MainTex, uv + d)                   * w;
                        col += tex2D(_MainTex, uv - d)                   * w;
                        col += tex2D(_MainTex, uv + float2( d.x, -d.y)) * w;
                        col += tex2D(_MainTex, uv + float2(-d.x,  d.y)) * w;
                    }
                }

                return col;
            }

            // Cheap animating film grain (no texture needed)
            float Hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 ts = _MainTex_TexelSize.xy;

                // Chromatic aberration: R and B shifted in opposite directions
                float  ca   = _ChromaShift * ts.x * 2.0;
                float4 colR = GaussianBlur(i.uv + float2( ca, 0), ts, _BlurSize);
                float4 colG = GaussianBlur(i.uv,                  ts, _BlurSize);
                float4 colB = GaussianBlur(i.uv + float2(-ca, 0), ts, _BlurSize);

                float4 col;
                col.r = colR.r;
                col.g = colG.g;
                col.b = colB.b;
                col.a = 1.0;

                // Desaturate
                float lum = dot(col.rgb, float3(0.299, 0.587, 0.114));
                col.rgb   = lerp(float3(lum, lum, lum), col.rgb, _Saturation);

                // Brightness + tint
                col.rgb *= _Brightness;
                col.rgb  = lerp(col.rgb, _TintColor.rgb, _TintColor.a);

                // Edge vignette (squared distance avoids sqrt)
                float2 c       = i.uv * 2.0 - 1.0;
                float  vd      = dot(c, c);
                float  vignette= 1.0 - smoothstep(0.4, 1.6, vd * _VignetteStr * 3.0);
                col.rgb       *= lerp(1.0, vignette, _VignetteStr);

                // Animated film grain
                float grain = Hash(i.uv + frac(_Time.y * 0.07)) * 2.0 - 1.0;
                col.rgb    += grain * _NoiseStr;

                col.a = i.color.a;
                return col;
            }
            ENDCG
        }
    }

    FallBack "UI/Default"
}
