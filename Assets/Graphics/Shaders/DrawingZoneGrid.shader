Shader "Custom/DrawingZoneGrid"
{
    Properties
    {
        _LineColor        ("Line Color",          Color)           = (0.15, 0.9,  1.0, 1.0)
        _GlowColor        ("Glow Color",          Color)           = (0.0,  0.45, 0.95,1.0)
        _BgColor          ("Background",          Color)           = (0.01, 0.02, 0.08,0.0)

        _GridSize         ("Grid Size",           Float)           = 7.0
        _LineWidth        ("Line Width",          Range(0.01,0.12))= 0.045
        _GlowFalloff      ("Glow Falloff",        Range(1,8))      = 3.5
        _GlowStr          ("Glow Strength",       Range(0,1))      = 0.5

        // Controlled at runtime by DrawingZoneController
        _RevealAmount     ("Reveal Amount",       Range(0,1))      = 0.0
        _PulseDiag        ("Pulse Position",      Float)           = 0.0
        _PulseWidth       ("Pulse Band Width",    Range(0.02,0.6)) = 0.18
        _IdlePulseAlpha   ("Idle Pulse Alpha",    Range(0,1))      = 0.28
        _FingerUV         ("Finger UV",           Vector)          = (0.5,0.5,0,0)
        _FingerActive     ("Finger Active",       Range(0,1))      = 0.0
        _DistortRadius    ("Distort Radius",      Range(0.01,0.5)) = 0.10
        _DistortStrength  ("Distort Strength",    Range(0,0.06))   = 0.018

        _EdgeFade         ("Left Edge Fade",      Range(0,0.15))   = 0.035
    }

    SubShader
    {
        Tags
        {
            "Queue"          = "Transparent"
            "RenderType"     = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector"= "True"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Lighting Off

        Pass
        {
            Name "DrawingZoneGrid"
            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _LineColor;
                float4 _GlowColor;
                float4 _BgColor;
                float  _GridSize;
                float  _LineWidth;
                float  _GlowFalloff;
                float  _GlowStr;
                float  _RevealAmount;
                float  _PulseDiag;
                float  _PulseWidth;
                float  _IdlePulseAlpha;
                float4 _FingerUV;
                float  _FingerActive;
                float  _DistortRadius;
                float  _DistortStrength;
                float  _EdgeFade;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float  t  = _Time.y;

                // ── Finger ripple distortion ──────────────────────────────────
                float2 fingerDelta = uv - _FingerUV.xy;
                float  fingerDist  = length(fingerDelta);
                float2 warpDir     = fingerDist > 0.001 ? normalize(fingerDelta) : float2(0.0, 1.0);
                float  warpEnv     = exp(-fingerDist / max(_DistortRadius, 0.001)) * _FingerActive;
                float  ripple      = sin(fingerDist * 24.0 - t * 10.0) * warpEnv * _DistortStrength;
                float2 distUV      = uv + warpDir * ripple;

                // ── Aspect-corrected grid on distorted UVs ────────────────────
                float  aspect  = (_ScreenParams.x * 0.5) / max(_ScreenParams.y, 1.0);
                float2 g       = distUV * float2(aspect, 1.0) * _GridSize;
                float2 gf      = frac(g) - 0.5;
                float  dist    = min(abs(gf.x), abs(gf.y));
                float  hw      = _LineWidth * 0.5;

                float gridLine = 1.0 - smoothstep(hw * 0.5, hw, dist);
                float gridGlow = pow(saturate(1.0 - dist / max(hw * 6.0, 0.001)), _GlowFalloff)
                                 * _GlowStr;

                // Intersection dots
                float gridDot  = (1.0 - smoothstep(hw * 0.6, hw * 1.4, length(gf))) * 0.65;

                // Faint sub-grid at half frequency
                float2 gf2    = frac(g * 0.5) - 0.5;
                float  subLine = (1.0 - smoothstep(hw * 0.25, hw * 0.55,
                                  min(abs(gf2.x), abs(gf2.y)))) * 0.22;

                float gridMask = saturate(gridLine + gridDot + subLine);

                // ── Diagonal pulse (top-left → bottom-right) ──────────────────
                // diagCoord: 0 at (0,0), 2 at (1,1)
                float diagCoord = uv.x + uv.y;
                float diagDist  = abs(diagCoord - _PulseDiag);
                float pulseVal  = 1.0 - smoothstep(0.0, _PulseWidth, diagDist);

                // ── Reveal blend ──────────────────────────────────────────────
                // Idle  (reveal≈0): only the thin pulse band lights up the grid
                // Active(reveal≈1): full grid visible, pulse adds a bright sweep on top
                float baseGridA  = gridMask * _RevealAmount;
                float pulseGridA = pulseVal * _IdlePulseAlpha * gridMask;
                // When fully revealed the pulse is also stronger
                pulseGridA      += pulseVal * gridMask * _RevealAmount * 0.45;

                float combinedGrid = saturate(baseGridA + pulseGridA);
                float combinedGlow = gridGlow * saturate(_RevealAmount + pulseVal * 0.55);

                // ── Color assembly ────────────────────────────────────────────
                float3 lineRGB = lerp(_GlowColor.rgb, _LineColor.rgb,
                                      saturate(gridLine + gridDot));
                // Pulse sweeps a brighter / whiter tint along the band
                lineRGB = lerp(lineRGB, _LineColor.rgb * 1.4, pulseVal * 0.55);

                float3 rgb = _BgColor.rgb   * _RevealAmount
                           + lineRGB        * combinedGrid
                           + _GlowColor.rgb * combinedGlow * (1.0 - combinedGrid);

                float a = saturate(
                      _BgColor.a    * _RevealAmount
                    + combinedGrid  * 0.95
                    + combinedGlow  * 0.28
                );

                // Soft left-edge seam fade
                a *= smoothstep(0.0, _EdgeFade + 0.001, uv.x);

                // Fingertip corona glow (visible even outside the grid lines)
                float corona  = exp(-fingerDist * 9.0) * _FingerActive * 0.55;
                rgb          += _LineColor.rgb * corona;
                a             = saturate(a + corona * 0.45);

                return half4(saturate(rgb), a);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/InternalErrorShader"
}
