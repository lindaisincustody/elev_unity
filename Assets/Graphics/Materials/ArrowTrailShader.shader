Shader "Custom/ArrowTrailShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1,1,1,1)
        _NoiseIntensity("Noise Intensity", Range(0,1)) = 0.2
        _TimeFactor("Time Factor", Range(0,5)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float4 _TintColor;
            float _NoiseIntensity;
            float _TimeFactor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _TintColor;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Create a dynamic offset for a trippy effect.
                float2 noiseOffset = float2(sin(_Time.y * _TimeFactor), cos(_Time.y * _TimeFactor)) * _NoiseIntensity;
                float2 uv = i.uv + noiseOffset;
                
                fixed4 texColor = tex2D(_MainTex, uv);
                fixed4 col = texColor * i.color;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
