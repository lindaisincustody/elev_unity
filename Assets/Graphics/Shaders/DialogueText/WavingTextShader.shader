Shader "Custom/WavingTextShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _WaveAmplitude("Wave Amplitude", Float) = 0.5
        _WaveFrequency("Wave Frequency", Float) = 1.0
        _WaveSpeed("Wave Speed", Float) = 1.0
    }

        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            LOD 100

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

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
                float _WaveAmplitude;
                float _WaveFrequency;
                float _WaveSpeed;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);

                    float wave = sin(_WaveFrequency * v.vertex.x + _Time.y * _WaveSpeed) * _WaveAmplitude;

                    o.vertex.y += wave;
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    return col;
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}
