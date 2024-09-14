Shader "Custom/ExcludeFromPostProcessing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}  // Player sprite texture
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" }  // Renders the sprite on top
        Pass
        {
            ZWrite Off
            Cull Off
            Lighting Off
            Stencil
            {
                Ref 1          // Reference value used for stencil operations
                Comp Always    // Always pass the stencil comparison
                Pass Replace   // Replace stencil buffer value with reference value
            }
            
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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"  // Fallback for older rendering pipelines
}
