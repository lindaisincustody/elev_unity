Shader "Custom/TrippyShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Distortion("Distortion", Float) = 0.1
        _Speed("Speed", Float) = 1.0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

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
                float _Distortion;
                float _Speed;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                float4 frag(v2f i) : SV_Target
                {
                    float2 uv = i.uv;
                    uv += _Distortion * sin(_Time.y * _Speed + uv.x * uv.y);
                    float4 color = tex2D(_MainTex, uv);
                    return color;
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}