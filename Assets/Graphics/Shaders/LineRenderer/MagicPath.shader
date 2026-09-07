Shader "Custom/MagicPath"
{
    Properties
    {
        [HDR] _ParticleColor("Particle Color", Color) = (1, 0.97, 0.9, 1)
        [HDR] _HazeColor("Haze Color", Color) = (0.35, 0.68, 1, 1)
        _HazeStrength("Haze Strength", Range(0, 1)) = 0.15

        _Spacing("Particle Spacing", Float) = 0.18
        _ParticleSize("Particle Size", Float) = 0.02
        _HaloStrength("Halo Strength", Range(0, 1)) = 0.3

        _Falloff("Width Falloff", Range(0, 1)) = 0.35
        _EndFade("End Fade", Range(0, 0.5)) = 0.06

        _FlowSpeed("Flow Speed", Float) = 0.35
        _Bob("Bob Amount", Float) = 0.05
        _TwinkleSpeed("Twinkle Speed", Float) = 0.35

        _PixelsPerUnit("Pixels Per Unit", Float) = 100


        _StencilRef("Stencil Ref", Integer) = 1
        _StencilReadMask("Stencil Read Mask", Integer) = 3
        _StencilWriteMask("Stencil Write Mask", Integer) = 3

        _LineLength("Line Length", Float) = 1
        _LineWidth("Line Width", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            Name "MagicPath"

            Stencil
            {
                Ref [_StencilRef]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp Equal
                Pass IncrSat
                Fail Keep
                ZFail Keep
            }

            Blend One OneMinusSrcAlpha
            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _ParticleColor;
                half4 _HazeColor;
                float _HazeStrength;
                float _Spacing;
                float _ParticleSize;
                float _HaloStrength;
                float _Falloff;
                float _EndFade;
                float _FlowSpeed;
                float _Bob;
                float _TwinkleSpeed;
                float _PixelsPerUnit;
                float _LineLength;
                float _LineWidth;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color;

                return output;
            }

            float2 Hash2(float2 c)
            {
                float3 p = frac(float3(c.xyx) * float3(0.1031, 0.1030, 0.0973));
                p += dot(p, p.yzx + 33.33);

                return frac((p.xx + p.yz) * p.zy);
            }

            float ParticleLayer(float2 q, float spacing, float seed, float scroll, float bob)
            {
                float2 p = q / spacing;
                p.x -= _Time.y * scroll / spacing;

                float2 cell = floor(p);
                float2 f = p - cell;

                float radius = _ParticleSize / spacing;
                float acc = 0;

                [unroll]
                for (int y = -1; y <= 1; y++)
                {
                    [unroll]
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 offset = float2(x, y);
                        float2 hash = Hash2(cell + offset + seed);

                        float2 center = offset + hash;
                        center.y += sin(_Time.y * (0.5 + hash.x) + hash.y * 6.2831) * bob / spacing;

                        float life = frac(_Time.y * _TwinkleSpeed * (0.6 + hash.y) + hash.x);
                        float twinkle = sin(life * 3.14159);

                        float distance = length(f - center);
                        float core = step(distance, radius);
                        float halo = step(distance, radius * 2) * _HaloStrength;

                        acc = max(acc, max(core, halo) * twinkle);
                    }
                }

                return acc;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float t = saturate(input.uv.x);
                float across = input.uv.y * 2 - 1;

                float2 q = float2(t * _LineLength, across * _LineWidth * 0.5);
                q = floor(q * _PixelsPerUnit) / _PixelsPerUnit;

                float envelope = 1 - smoothstep(_Falloff, 1, abs(across));
                float ends = smoothstep(0, _EndFade, t) * smoothstep(0, _EndFade, 1 - t);

                float near = ParticleLayer(q, _Spacing, 0, _FlowSpeed, _Bob);
                float far = ParticleLayer(q, _Spacing * 1.7, 37.2, _FlowSpeed * 0.55, _Bob * 1.6) * 0.6;
                float particles = max(near, far) * envelope * ends;

                float haze = envelope * envelope * _HazeStrength * ends;

                half3 rgb = _ParticleColor.rgb * particles + _HazeColor.rgb * haze;
                half alpha = saturate(particles * _ParticleColor.a + haze * _HazeColor.a);

                return half4(rgb * input.color.rgb * input.color.a, alpha * input.color.a);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
