Shader "Custom/RippleEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RippleStrength ("Ripple Strength", Float) = 0.05
        _RippleSpeed ("Ripple Speed", Float) = 1.0
        _RippleCenter ("Ripple Center", Vector) = (0.5, 0.5, 0.0, 0.0)
        _Opacity ("Opacity", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma alpha_test

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
            float _RippleStrength;
            float _RippleSpeed;
            float4 _RippleCenter;
            float _Opacity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 RippleEffect(float2 uv, float2 center, float strength, float time)
            {
                float dist = distance(uv, center);
                float ripple = sin((dist - time * _RippleSpeed) * 10.0) * _RippleStrength / (dist + 0.01);
                uv += normalize(uv - center) * ripple;
                return uv;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 rippleUV = RippleEffect(i.uv, _RippleCenter.xy, _RippleStrength, _Time.y);
                fixed4 color = tex2D(_MainTex, rippleUV);
                color.a *= _Opacity; // Apply opacity
                return color;
            }
            ENDCG

            // Blending settings moved inside Pass
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
        }
    }
    FallBack "Diffuse"
}
