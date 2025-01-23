Shader "Custom/PsychedelicBackground"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Speed", Float) = 1.0
        _DistortionStrength ("Distortion Strength", Float) = 0.1
    }

    SubShader
    {
        Tags { "Queue"="Background" }
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _Speed;
            float _DistortionStrength;
            sampler2D _MainTex;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 distortion = sin(uv * 10 + _Time.y * _Speed) * _DistortionStrength;
                uv += distortion;

                fixed4 col = tex2D(_MainTex, uv);
                
                // Apply color shifting or additional effects here

                return col;
            }
            ENDCG
        }
    }
}
