Shader "Custom/TrippyChain"
{
    Properties
    {
        _MainTex("Chain Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _Scroll("Scroll Direction", Vector) = (1,0,0,0)
        _Speed("Scroll Speed", Float) = 0.5
        _WaveAmp("Wave Amplitude", Float) = 0.1
        _WaveFreq("Wave Frequency", Float) = 6
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off Lighting Off ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4   _MainTex_ST;
            fixed4   _Color;
            float2   _Scroll;
            float    _Speed;
            float    _WaveAmp;
            float    _WaveFreq;
            // **no float _Time;**

            struct appdata_t {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color    : COLOR;
            };
            struct v2f {
                float4 vertex   : SV_POSITION;
                float2 uv       : TEXCOORD0;
                fixed4 color    : COLOR;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv     = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color  = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // use built‐in _Time.y (time in seconds)
                float t = _Time.y;

                // scroll UV
                float2 uv = i.uv + _Scroll * (t * _Speed);

                // sinusoidal distortion
                uv.y += sin((uv.x + t) * _WaveFreq) * _WaveAmp;

                fixed4 c = tex2D(_MainTex, uv) * i.color;
                return c;
            }
            ENDCG
        }
    }
}
