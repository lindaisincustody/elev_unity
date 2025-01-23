Shader "Custom/BalatroBackground"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "black" {}
        _Speed ("Speed", Float) = 0.5
        _DistortionStrength ("Distortion Strength", Float) = 0.1
        _TimeMultiplier ("Time Multiplier", Float) = 1.0
        _BrightnessFactor ("Brightness Factor", Float) = 0.5
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

            // Shader parameters
            float _Speed;
            float _DistortionStrength;
            float _TimeMultiplier;
            float _BrightnessFactor;
            sampler2D _MainTex;

            // Noise function
            float Noise(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            // Vertex Shader
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Fragment Shader
            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float time = _Time.y * _TimeMultiplier;

                // Add wave distortion
                float waveX = sin(uv.y * 10.0 + time * _Speed) * _DistortionStrength;
                float waveY = cos(uv.x * 10.0 + time * _Speed) * _DistortionStrength;
                uv.x += waveX;
                uv.y += waveY;

                // Apply noise for further distortion
                uv += Noise(uv * 10.0 + time) * _DistortionStrength;

                // Fetch the color from the main texture
                fixed4 col = tex2D(_MainTex, uv);

                // Color shifting effect (sinusoidal shifting)
                //col.rgb += sin(time * 0.5 + uv.x * 5.0) * 0.05;  // Reduced color shift
               // col.rgb += sin(time * 0.2 + uv.y * 7.0) * 0.05;  // Reduced color shift

                // Darken the color by multiplying with a brightness factor
                col.rgb *= _BrightnessFactor;

                // Further adjustment to prevent the colors from going too bright
                col.rgb = max(col.rgb, 0.0);  // Clamp to prevent going below black

                return col;
            }

            ENDCG
        }
    }

    Fallback "Diffuse"
}
