Shader "Unlit/WavyGhostEffect"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Tint Color", Color) = (1,1,1,1)
        _Amplitude("Wave Amplitude", Float) = 0.5
        _Frequency("Wave Frequency", Float) = 3.0
        _Speed("Wave Speed", Float) = 2.0
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            LOD 100

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

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
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float _Amplitude;
                float _Frequency;
                float _Speed;
                fixed4 _Color;

                v2f vert(appdata v)
                {
                    v2f o;
                    float wave = sin(_Frequency * (v.vertex.y + _Time.y * _Speed)) * _Amplitude;
                    v.vertex.x += wave;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                    return col;
                }
                ENDCG
            }
        }
}
