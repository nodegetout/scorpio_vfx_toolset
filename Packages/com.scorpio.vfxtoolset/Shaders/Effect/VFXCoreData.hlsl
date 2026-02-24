#ifndef _VFX_CORE_DATA_
#define _VFX_CORE_DATA_

struct Attributes
{
    float4 positionOS  : POSITION;
    half2  uv0         : TEXCOORD0;
    #if defined(VERTEX_REQUIRE_VERTEXCOLOR)
    half4  vertexColor : COLOR;
    #endif
    #if defined(VERTEX_REQUIRE_UV1)
    half4 uv1         : TEXCOORD1;
    #endif
    #if defined(VERTEX_REQUIRE_UV2)
    half4 uv2         : TEXCOORD2;
    #endif
    #if defined(VERTEX_REQUIRE_NORMAL)
    half3  normalOS    : NORMAL;
    #endif
};

struct Varyings
{
    float4 positionHCS : SV_POSITION;
    half4  uv0         : TEXCOORD0;
    
    #if defined(FRAGMENT_REQUIRE_UV1)
    half4  uv1         : TEXCOORD1;
    #endif
    
    #if defined(FRAGMENT_REQUIRE_UV2)
    half4  uv2         : TEXCOORD2;
    #endif
    
    #if defined(FRAGMENT_REQUIRE_VERTEXCOLOR)
    half4  vertexColor : TEXCOORD3;
    #endif
    
    #if defined(FRAGMENT_REQUIRE_NORMAL)
    half4  normalWS    : TEXCOORD4;
    #endif
    
    #if defined(REQUIRE_POSITIONWS)
    float4 positionWS : TEXCOORD5;
    #endif
    
    #if defined(_ENABLE_SCREEN_UV)
    float4 screenPos : TEXCOORD6;
    #endif
};

float2 GetOneHourCycledTime()
{
    return fmod(_Time.yy, 3600.0f);
}

#endif