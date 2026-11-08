Shader "Custom/GuidePath"
{
    Properties
    {
        [PerRendererData] _MainTex("Field", 2D) = "black" {}

        _HazeColor("Haze Color", Color) = (1, 0.68, 0.28, 1)
        _HazeAlpha("Haze Alpha", Range(0, 1)) = 0.6
        _HazeWidth("Haze Width", Float) = 0.3
        _HazeBlock("Haze Block Pixels", Float) = 4
        _HazeNoise("Haze Noise", Range(0, 1)) = 0.45
        _HazeNoiseScale("Haze Noise Scale", Float) = 2.2
        _HazeDrift("Haze Drift", Float) = 0.35

        _ShadeColor("Shade Color", Color) = (0.12, 0.06, 0.02, 1)
        _ShadeAlpha("Shade Alpha", Range(0, 1)) = 0.12
        _ShadeWidth("Shade Width", Float) = 0.42

        _StrandColor("Strand Color", Color) = (1, 0.84, 0.48, 1)
        _StrandAlpha("Strand Alpha", Range(0, 1)) = 0.8
        _StrandCount("Strand Count", Range(1, 3)) = 3
        _StrandAmplitude("Strand Amplitude", Float) = 0.17
        _StrandWavelength("Strand Wavelength", Float) = 2.2
        _StrandSpeed("Strand Speed", Float) = 0.25
        _StrandThickness("Strand Thickness Pixels", Float) = 1.7
        _StrandTaper("Strand Taper", Float) = 0.9

        _StartFade("Start Fade", Float) = 1.8
        _EndFade("End Fade", Float) = 0.15
        _EndGlow("End Glow", Range(0, 1)) = 0.3
        _EndGlowRadius("End Glow Radius", Float) = 0.22

        _Length("Length", Float) = 1
        _PixelsPerUnit("Pixels Per Unit", Float) = 36
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
            Name "GuidePath"

            Blend One OneMinusSrcAlpha
            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;

            CBUFFER_START(UnityPerMaterial)
                half4 _HazeColor;
                float _HazeAlpha;
                float _HazeWidth;
                float _HazeBlock;
                float _HazeNoise;
                float _HazeNoiseScale;
                float _HazeDrift;
                half4 _ShadeColor;
                float _ShadeAlpha;
                float _ShadeWidth;
                half4 _StrandColor;
                float _StrandAlpha;
                float _StrandCount;
                float _StrandAmplitude;
                float _StrandWavelength;
                float _StrandSpeed;
                float _StrandThickness;
                float _StrandTaper;
                float _StartFade;
                float _EndFade;
                float _EndGlow;
                float _EndGlowRadius;
                float _Length;
                float _PixelsPerUnit;
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
                float2 positionWS : TEXCOORD1;
                half4 color : COLOR;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(positionWS);
                output.positionWS = positionWS.xy;
                output.uv = input.uv;
                output.color = input.color;

                return output;
            }

            float Hash(float2 p)
            {
                float3 q = frac(float3(p.xyx) * 0.1031);
                q += dot(q, q.yzx + 33.33);

                return frac((q.x + q.y) * q.z);
            }

            float Noise(float2 p)
            {
                float2 cell = floor(p);
                float2 f = p - cell;
                f = f * f * (3 - 2 * f);

                float a = Hash(cell);
                float b = Hash(cell + float2(1, 0));
                float c = Hash(cell + float2(0, 1));
                float d = Hash(cell + float2(1, 1));

                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            float2 Field(float2 uv)
            {
                return SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_MainTex, uv, 0).rg;
            }

            float Ends(float along)
            {
                return smoothstep(0, _StartFade, along) * smoothstep(0, _EndFade, _Length - along);
            }

            float Strand(float along, float across, float index)
            {
                float taper = smoothstep(0, _StrandTaper, along) * smoothstep(0, _StrandTaper, _Length - along);
                float scale = 1 - index * 0.22;
                float frequency = 6.2831853 / (_StrandWavelength * (1 + index * 0.31));
                float direction = index == 1 ? -1 : 1;
                float phase = along * frequency - _Time.y * _StrandSpeed * 6.2831853 * direction + index * 2.094;

                float amplitude = _StrandAmplitude * scale * taper;
                float centre = amplitude * sin(phase);
                float slope = amplitude * frequency * cos(phase);
                float distance = abs(across - centre) / sqrt(1 + slope * slope);

                float shimmer = 0.55 + 0.45 * sin(along * 1.7 - _Time.y * 1.3 + index * 1.9);

                return step(distance * _PixelsPerUnit, _StrandThickness * 0.5) * shimmer * (1 - index * 0.18);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 texels = _MainTex_TexelSize.zw;
                float2 pixel = input.positionWS * _PixelsPerUnit;
                float2 origin = pixel - input.uv * texels;
                float2 block = (floor(pixel / _HazeBlock) + 0.5) * _HazeBlock - 0.25;

                float2 coarse = Field((block - origin) / texels);
                float2 fine = Field(input.uv);

                float hazeEnds = Ends(coarse.x);
                float spread = 1 + 0.6 * (1 - smoothstep(0, _StartFade, coarse.x));
                float glow = exp(-pow(coarse.y / (_HazeWidth * spread), 2));
                float noise = Noise(float2(coarse.x * _HazeNoiseScale - _Time.y * _HazeDrift, coarse.y * _HazeNoiseScale * 1.7));
                float remaining = _Length - coarse.x;
                float endGlow = _EndGlow * exp(-(remaining * remaining + coarse.y * coarse.y) / (_EndGlowRadius * _EndGlowRadius));
                float haze = saturate(max(_HazeAlpha * glow * lerp(1, noise * 1.6, _HazeNoise) * hazeEnds, endGlow));
                float shade = _ShadeAlpha * exp(-pow(coarse.y / (_ShadeWidth * spread), 2)) * hazeEnds;

                float strands = 0;
                [unroll]
                for (int i = 0; i < 3; i++)
                    strands = max(strands, i < _StrandCount ? Strand(fine.x, fine.y, i) : 0);
                strands *= _StrandAlpha * Ends(fine.x);

                half3 rgb = _ShadeColor.rgb * shade;
                half alpha = shade;

                rgb = _HazeColor.rgb * haze + rgb * (1 - haze);
                alpha = haze + alpha * (1 - haze);

                rgb = _StrandColor.rgb * strands + rgb * (1 - strands);
                alpha = strands + alpha * (1 - strands);

                return half4(rgb, alpha) * input.color.a;
            }
            ENDHLSL
        }
    }

    Fallback Off
}
