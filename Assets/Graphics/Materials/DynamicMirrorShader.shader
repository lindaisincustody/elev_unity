Shader "Custom/DynamicMirrorShader" {
    Properties {
        _MainTex ("Base Texture", 2D) = "white" {}
        _ReflectionTex ("Reflection Texture", 2D) = "black" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _ReflectionTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Flip the UVs for the reflection texture
                float2 reflUV = float2(1.0 - i.uv.x, i.uv.y);
                fixed4 reflection = tex2D(_ReflectionTex, reflUV);
                // You can blend the base texture and reflection as needed
                fixed4 baseColor = tex2D(_MainTex, i.uv);
                return lerp(baseColor, reflection, 0.5);
            }
            ENDCG
        }
    }
}
