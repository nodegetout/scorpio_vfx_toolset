#ifndef _VFX_GRAPHICS_SETTING_MICRO_
#define _VFX_GRAPHICS_SETTING_MICRO_

#if defined(_REQUIRE_CUSTOMDATA)
    #define VERTEX_REQUIRE_UV1
    #define VERTEX_REQUIRE_UV2
    #define FRAGMENT_REQUIRE_UV1
    #define FRAGMENT_REQUIRE_UV2
#else
    #ifndef _ENABLE_SCREEN_UV
        #define VERTEX_REQUIRE_UV1
    #endif
#endif

#if defined(_FRESNEL_ON)
    #define REQUIRE_NORMAL
    #define REQUIRE_POSITIONWS
#endif

#if defined(_ENABLE_PLANAR_SOFT_PARTICLE)
    #define REQUIRE_POSITIONWS
#endif

#endif