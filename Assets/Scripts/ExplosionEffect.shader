Shader "Unlit/ExplosionEffect"
{
    Properties
    {
        _Progress("Progress", Range(0, 1)) = 0
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _Progress;

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

            fixed4 frag(v2f i) : SV_Target
            {
                // Simple circle explosion effect
                float dist = distance(i.uv, float2(0.5, 0.5)) - _Progress;
                float alpha = 1.0 - smoothstep(0.0, 0.1, abs(dist));
                return fixed4(1, 0, 0, alpha); // Red color
            }
            ENDCG
        }
    }
}
