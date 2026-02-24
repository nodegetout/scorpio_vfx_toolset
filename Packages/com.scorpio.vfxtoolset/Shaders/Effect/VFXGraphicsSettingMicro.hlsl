#ifndef _VFX_GRAPHICS_SETTING_MICRO_
#define _VFX_GRAPHICS_SETTING_MICRO_

#if defined(_FRESNEL_ON)
    #define VERTEX_REQUIRE_NORMAL
    #define FRAGMENT_REQUIRE_NORMAL
    #define REQUIRE_POSITIONWS
#endif

#if defined(_ENABLE_PLANAR_SOFT_PARTICLE)
    #define REQUIRE_POSITIONWS
#endif

#if defined(_ENABLE_VERTEX_OFFSET)
    #define VERTEX_REQUIRE_NORMAL
#endif

#endif