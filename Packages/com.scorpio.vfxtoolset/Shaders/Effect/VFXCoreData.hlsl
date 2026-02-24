#ifndef _VFX_CORE_DATA_
#define _VFX_CORE_DATA_

float2 GetOneHourCycledTime()
{
    return fmod(_Time.yy, 3600.0f);
}

#endif