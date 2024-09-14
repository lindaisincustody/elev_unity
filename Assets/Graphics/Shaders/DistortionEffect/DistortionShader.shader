Shader "Custom/Distortion"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Distortion ("Distortion", Range(0, 1)) = 0.0
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
            float _Distortion;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex); // Use Unity's built-in function
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : COLOR
            {
                float2 offset = (sin(i.uv * 10.0) * _Distortion);
                half4 col = tex2D(_MainTex, i.uv + offset);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
