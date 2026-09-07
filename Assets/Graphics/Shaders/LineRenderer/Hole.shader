Shader "Custom/Hole"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [KeywordEnum(Void, Pit, Abyss)] _Style("Style", Float) = 1

        _Color("Tint", Color) = (1, 1, 1, 1)
        _RimColor("Rim", Color) = (0.42, 0.39, 0.5, 1)
        _MidColor("Wall", Color) = (0.13, 0.11, 0.18, 1)
        _DeepColor("Deep", Color) = (0.02, 0.015, 0.04, 1)

        _RimWidth("Rim Width", Float) = 0.05
        _WallDepth("Wall Depth", Float) = 0.6
        _DepthBias("Vertical Bias", Range(-1, 1)) = 0.35

        _Bands("Color Bands", Range(2, 8)) = 4
        _Dither("Dither", Range(0, 1)) = 1
        _PixelsPerUnit("Pixels Per Unit", Float) = 100

        _MoteColor("Mote Color", Color) = (1, 1, 1, 1)
        _MoteColumns("Mote Columns", Range(1, 40)) = 14
        _MoteDensity("Mote Density", Range(0, 1)) = 0.5
        _MoteSpread("Mote Spread", Range(0.2, 3)) = 1.4
        _MoteSize("Mote Size", Range(1, 4)) = 1
        _MoteFall("Mote Fall Speed", Float) = 0.35
        _MoteTop("Mote Top", Range(0, 1)) = 0.85
        _MoteBottom("Mote Bottom", Range(0, 1)) = 0.1
        _MoteFadeStart("Mote Fade Start", Range(0, 1)) = 0.45

        _Cutoff("Alpha Cutoff", Range(0, 1)) = 0.1
        _StencilRef("Stencil Ref", Integer) = 1
        [Enum(Invisible, 0, Visible, 15)] _ColorMask("Draw Sprite", Integer) = 15
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
            Name "HoleStencil"

            Stencil
            {
                Ref [_StencilRef]
                Comp Always
                Pass Replace
            }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest Always
            Cull Off
            ColorMask [_ColorMask]

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0
            #pragma shader_feature_local _STYLE_VOID _STYLE_PIT _STYLE_ABYSS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                half4 _RimColor;
                half4 _MidColor;
                half4 _DeepColor;
                half4 _MoteColor;
                float _RimWidth;
                float _WallDepth;
                float _DepthBias;
                float _Bands;
                float _Dither;
                float _PixelsPerUnit;
                float _MoteColumns;
                float _MoteDensity;
                float _MoteSpread;
                float _MoteSize;
                float _MoteFall;
                float _MoteTop;
                float _MoteBottom;
                float _MoteFadeStart;
                float _Cutoff;
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
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.color;

                return output;
            }

            float Hash(float n)
            {
                return frac(sin(n * 127.1) * 43758.5453);
            }

            float Bayer2(float2 a)
            {
                a = floor(a);
                return frac(a.x * a.y * 0.5 + a.x * 0.5 + 0.25);
            }

            float Dither(float2 positionWS)
            {
                float2 pixel = floor(positionWS * _PixelsPerUnit);
                return Bayer2(pixel * 0.5) * 0.25 + Bayer2(pixel) - 0.5;
            }

            float EdgeDistance(float2 uv, float2 uvPerWorld)
            {
                float2 toEdge = min(uv, 1 - uv) / uvPerWorld;
                float inward = min(toEdge.x, toEdge.y);

                return floor(inward * _PixelsPerUnit) / _PixelsPerUnit;
            }

            float Motes(float2 uv, float2 positionWS, float2 uvPerWorld)
            {
                float columns = floor(_MoteColumns);
                float column = floor(uv.x * columns);

                float center = (column + 0.5 + (Hash(column + 5.1) - 0.5) * 0.8) / columns;
                float weight = saturate(1 - abs(center - 0.5) * 2 * _MoteSpread);
                float active = step(Hash(column + 3.7), _MoteDensity * weight);

                float speed = lerp(0.4, 1, Hash(column + 41.3));
                float life = frac(_Time.y * speed * _MoteFall + Hash(column + 11.9));

                float2 moteUV = float2(center, lerp(_MoteTop, _MoteBottom, life));
                float2 moteWS = positionWS + (moteUV - uv) / uvPerWorld;

                float2 offset = floor(moteWS * _PixelsPerUnit) - floor(positionWS * _PixelsPerUnit);
                float hit = step(max(abs(offset.x), abs(offset.y)), floor(_MoteSize) - 1);

                float fade = saturate(life * 6) * (1 - smoothstep(_MoteFadeStart, 1, life));

                return active * hit * fade;
            }

            half3 Ramp(float t)
            {
                return t < 0.5
                    ? lerp(_RimColor.rgb, _MidColor.rgb, t * 2)
                    : lerp(_MidColor.rgb, _DeepColor.rgb, (t - 0.5) * 2);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half alpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).a * _Color.a * input.color.a;
                clip(alpha - _Cutoff);

                float2 uvPerWorld = max(float2(
                    fwidth(input.uv.x) / max(fwidth(input.positionWS.x), 1e-6),
                    fwidth(input.uv.y) / max(fwidth(input.positionWS.y), 1e-6)), 1e-6);

                float edge = EdgeDistance(input.uv, uvPerWorld);
                float dither = Dither(input.positionWS);

                float depth = saturate(edge / max(_WallDepth, 1e-4));
                depth = saturate(depth + (0.5 - input.uv.y) * _DepthBias);

                float banded = saturate(floor(depth * _Bands + dither * _Dither) / max(_Bands - 1, 1));

                #if defined(_STYLE_VOID)
                    half3 color = _DeepColor.rgb;
                #elif defined(_STYLE_ABYSS)
                    half3 color = lerp(_MidColor.rgb, _DeepColor.rgb, banded);
                #else
                    half3 color = Ramp(banded);
                #endif

                float mote = Motes(input.uv, input.positionWS, uvPerWorld) * banded;
                color = lerp(color, _MoteColor.rgb, mote * _MoteColor.a);

                color = edge < _RimWidth ? _RimColor.rgb : color;

                return half4(color * _Color.rgb, alpha);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
