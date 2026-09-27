#ifndef WORLD_SPRITE_EFFECTS
#define WORLD_SPRITE_EFFECTS

float4 _PlayerPosition;

float WorldHash(float2 p)
{
    return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
}

float ValueNoise(float2 p)
{
    float2 cell = floor(p);
    float2 f = frac(p);
    float2 u = f * f * (3.0 - 2.0 * f);

    float a = WorldHash(cell);
    float b = WorldHash(cell + float2(1, 0));
    float c = WorldHash(cell + float2(0, 1));
    float d = WorldHash(cell + float2(1, 1));

    return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
}

void ApplyWorldEffects(inout half4 color, float3 positionWS, float2 objectOrigin)
{
    float blotches = ValueNoise(positionWS.xy / _SpreadScale);
    float threshold = WorldHash(objectOrigin) * _Unevenness + (blotches - 0.5) * _Spread;
    threshold = clamp(threshold, 0.0, 1.0 - _Softness);
    float drain = smoothstep(threshold, threshold + _Softness, _Insanity);

    float radius = lerp(_PocketRadius, _PocketMinRadius, _Insanity);
    float distanceToPlayer = length(positionWS.xy - _PlayerPosition.xy);
    float outsidePocket = smoothstep(radius * 0.6, radius, distanceToPlayer);

    half3 grey = dot(color.rgb, half3(0.2126, 0.7152, 0.0722));
    color.rgb = lerp(color.rgb, grey, drain * outsidePocket);
}

#endif
