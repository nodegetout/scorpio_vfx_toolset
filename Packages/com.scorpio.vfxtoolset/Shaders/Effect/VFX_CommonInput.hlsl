#ifndef _VFX_COMMON_INPUT_
#define _VFX_COMMON_INPUT_

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

sampler2D _BaseMap;
half4     _BaseMap_ST;
half4     _BaseColor;
half4     _BaseFrontColor;

half4     _DouYinEffectParams;
half4     _BaseUVParams;
#define _DiffXSpeed   _BaseUVParams.x
#define _DiffYSpeed   _BaseUVParams.y
#define _MainTexScale _BaseUVParams.z
#define _MainTexAngle _BaseUVParams.w

half4 _BaseMapToggles;
half4 _MaskMapParams;
#define _UnMult _BaseMapToggles.x
#define _BaseMultiplyLuminance _BaseMapToggles.y
#define _EnableBaseMapPolarUV _BaseMapToggles.z
#define _BaseMapSwitchUV1 _BaseMapToggles.w

half    _FrontIntensity;

sampler2D _Mask;
half4 _Mask_ST;

half4 _MaskUVParams;
#define  _MaskXSpeed _MaskUVParams.x
#define  _MaskYSpeed _MaskUVParams.y
#define  _MaskScale  _MaskUVParams.z
#define  _MaskAngle  _MaskUVParams.w
half4 _MaskMapToggles;
#define _MaskNotEffectDiff _MaskMapToggles.x
#define _MaskNotEffectMixMap _MaskMapToggles.y
#define _MaskedDissolve _MaskMapToggles.z
#define _EffectByMask _MaskMapToggles.w

half  _MaskMapSwitchUV1;
half  _EnableMaskMapPolarUV;


half4 _DiffuseBackColor;
half  _BackIntensity;
half  _DoubleSideOn;

#if defined(_MIX_BASE_ON)
sampler2D _MixDiffuse;
half4 _MixDiffuse_ST;
half4 _MixTintColor;

half  _MixBasePolarUVOn;
half4 _MixMapParams;
#define _MixMultiplyLuminance _MixMapParams.x
#define _EnableNoiseEffect _MixMapParams.y
#define _EnableRampMode _MixMapParams.z
#define _MixDiffusePower _MixMapParams.w

half4 _MixBaseMapUVParams;
#define _MixDiffuseXSpeed _MixBaseMapUVParams.x
#define _MixDiffuseYSpeed _MixBaseMapUVParams.y
#define _MixDiffuseScale  _MixBaseMapUVParams.z
#define _MixDiffuseAngle  _MixBaseMapUVParams.w
#endif

#if defined(_GRADIENT_ON)
half _GradientOn;
half _GradientSameDiffOn;
half4 _LeftColor;
half4 _RightColor;
half4 _GradientParams;
#define _UVWeights _GradientParams.x
#define _LeftWeights _GradientParams.y
#define _RightWeights _GradientParams.z
#define _Gradient _GradientParams.w
#endif

#if defined(_DISSOLVE_ON)
sampler2D _DissolveTex;
half4 _DissolveTex_ST;
half4 _DissolveColor;

half4 _DissolveParams;
#define _DissolveAngle _DissolveParams.x
#define _DissolveStep _DissolveParams.y
#define _SoftSize _DissolveParams.z
#define _EdgeColorBlendMode _DissolveParams.w

half4 _DissolveEdgeParams;
// #define _DissolveOutlineOn _DissolveEdgeParams.x
// #define _DissolveOutlineWidth _DissolveEdgeParams.y
// #define _DissolveOutlineSoft _DissolveEdgeParams.z
// #define _DissolveColorPW _DissolveEdgeParams.w
#endif

#if defined(_FALLOFF_DISSOLVE_ON)
sampler2D _DissolveTex;
int _DissolveDir;
half4 _DissolveNoiseParam;
half4 _DissolveControlParams;
half4 _DissolveColor;
#define _DissolveThreshold       _DissolveControlParams.x 
#define _DissolveFallOff         _DissolveControlParams.y 
#define _DissolveColorThreshold  _DissolveControlParams.z 
#define _DissolveColorFallOff    _DissolveControlParams.w 
#endif


#if defined(_NOISE_ON)
sampler2D _NoiseTex1;
sampler2D _NoiseTex2;
half4 _NoiseTex1_ST;
half4 _NoiseTex2_ST;
half  _NoiseUnEffectDiff;
half4  _NoiseStrength;
half4 _GChannel;
half  _DualDirectionNoise;
# endif

#if defined(_COLOUR_ON)
half  _ColourOn;
half4 _ColorGradingParams;
#define _EffectHue _ColorGradingParams.x
#define _EffectSaturation _ColorGradingParams.y
#define _EffectContrast _ColorGradingParams.z
half4 _SaturationLeftColor;
half4 _SaturationRightColor;
half  _SaturationLeftColorWeights;
half  _SaturationRightColorWeights;
#endif

#if defined(_FRESNEL_ON)
half _FresnelOn;
sampler2D _FresnelMap;
half4 _FresnelMap_ST;
half4 _FresnelParams;
#define _TransparentFresnelPart _FresnelParams.x
#define _FresnelPower _FresnelParams.y
#define _FresnelScale _FresnelParams.z
half4 _FresnelColor;
half _FresnelMapUse2U;
#endif

#if defined(_ENABLE_VERTEX_OFFSET)
sampler2D _VertexOffsetNoiseMap;
half4 _VertexOffsetNoiseMap_ST;
half4 _VertexDir;
half4 _VertexOffsetParams;
#define _VertexScale _VertexOffsetParams.x
#define _VertexPower _VertexOffsetParams.y
// #define _VertexScaleHeight _VertexOffsetParams.z
// #define _VertexScaleHeight _VertexOffsetParams.w
float2 _VertexMotionSpeed;
float _MotionDir;
float _ToggleVertexOffset;
#endif

#if defined(_FLOW_MAP_ON)
sampler2D _FlowMap;
half4 _FlowMapParams;
#endif

#if defined(_ENABLE_SCREEN_UV)
half _EnableScreenUV;
#endif

#if defined(_ENABLE_PLANAR_SOFT_PARTICLE)
half _ContactRange;
half _EnablePlanarSoftParticle;
uniform float _HorizontalPlaneY;
#endif

#endif