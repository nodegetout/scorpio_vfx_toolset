#ifndef _SCORPIO_PIPELINE_COMMON_INCLUDED
#define _SCORPIO_PIPELINE_COMMON_INCLUDED

float4 ComputeScreenPos(float4 positionCS)
{
    float4 o = positionCS * 0.5f;
    o.xy = float2(o.x, o.y * _ProjectionParams.x) + o.w;
    o.zw = positionCS.zw;
    return o;
}

#endif