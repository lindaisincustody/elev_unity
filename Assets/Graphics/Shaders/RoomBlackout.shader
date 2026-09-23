Shader "Custom/RoomBlackout"
{
    Properties
    {
        _Color("Color", Color) = (0, 0, 0, 1)
        _Fade("Fade", Range(0, 1)) = 1
        _EdgeFeather("Edge Feather", Float) = 0.02
        _Dither("Dither", Range(0, 1)) = 1
        _PixelsPerUnit("Pixels Per Unit", Float) = 100

        _RippleColor("Ripple Color", Color) = (0.30, 0.42, 0.52, 1)
        _Radius("Reach Radius", Float) = 4.5
        _Intensity("Intensity", Range(0, 2)) = 0.7
        _RippleFrequency("Ripple Frequency", Float) = 5
        _RippleSpeed("Ripple Speed", Float) = 1.6
        _Falloff("Falloff", Range(0.5, 6)) = 2.5
        _Breath("Idle Breath", Range(0, 1)) = 0.15
        _BreathSpeed("Breath Speed", Float) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
        }

        Pass
        {
            Name "RoomBlackout"

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _EdgePoints[32];
            int _EdgeCount;

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _RippleColor;
                float _Fade;
                float _EdgeFeather;
                float _Dither;
                float _PixelsPerUnit;
                float _Radius;
                float _Intensity;
                float _RippleFrequency;
                float _RippleSpeed;
                float _Falloff;
                float _Breath;
                float _BreathSpeed;
            CBUFFER_END

            float4 _PlayerWorldPos;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 positionWS : TEXCOORD1;
                float2 positionOS : TEXCOORD2;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);

                output.positionCS = TransformWorldToHClip(positionWS);
                output.positionWS = positionWS.xy;
                output.positionOS = input.positionOS.xy;
                output.uv = input.uv;

                return output;
            }

            float Bayer2(float2 a)
            {
                a = floor(a);
                return frac(a.x * a.y * 0.5 + a.x * 0.5 + 0.25);
            }

            float PolygonDistance(float2 p, float2 uv)
            {
                if (_EdgeCount < 3)
                    return min(min(uv.x, 1 - uv.x), min(uv.y, 1 - uv.y));

                float closest = 1e6;

                [loop]
                for (int i = 0; i < _EdgeCount; i++)
                {
                    float2 a = _EdgePoints[i].xy;
                    float2 b = _EdgePoints[(i + 1) % _EdgeCount].xy;

                    float2 pa = p - a;
                    float2 ba = b - a;
                    float h = saturate(dot(pa, ba) / max(dot(ba, ba), 1e-6));

                    closest = min(closest, length(pa - ba * h));
                }

                return closest;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float edge = PolygonDistance(input.positionOS, input.uv);

                float2 pixel = floor(input.positionWS * _PixelsPerUnit);
                float dither = (Bayer2(pixel * 0.5) * 0.25 + Bayer2(pixel) - 0.5) * _Dither;

                float toPlayer = length(input.positionWS - _PlayerWorldPos.xy);
                float reach = pow(saturate(1 - toPlayer / max(_Radius, 1e-4)), _Falloff);

                float wave = sin(toPlayer * _RippleFrequency - _Time.y * _RippleSpeed);
                wave = wave * 0.5 + 0.5;

                float breath = 1 + _Breath * sin(_Time.y * _BreathSpeed + edge * 2);

                float disturbance = reach * wave * _Intensity * breath;
                disturbance = saturate(disturbance + dither * 0.25);

                half3 color = _Color.rgb + _RippleColor.rgb * disturbance;

                float alpha = _Color.a * _Fade;
                alpha *= saturate(edge / max(_EdgeFeather, 1e-4));

                return half4(color, saturate(alpha + dither * 0.03));
            }
            ENDHLSL
        }
    }

    Fallback Off
}
