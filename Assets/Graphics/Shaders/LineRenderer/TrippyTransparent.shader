Shader "Custom/TrippyTransparent"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,0.5)
        _Transparency ("Transparency", Range(0, 1)) = 0.5
        _TimeSpeed ("Time Speed", Float) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float _Transparency;
            float4 _Color;
            float _TimeSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex.xy * 0.1 + _Time.y * _TimeSpeed;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float intensity = abs(sin(i.uv.x * 3.14159) + sin(i.uv.y * 3.14159));
                return float4(_Color.rgb * intensity, _Transparency);
            }
            ENDCG
        }
    }
}
