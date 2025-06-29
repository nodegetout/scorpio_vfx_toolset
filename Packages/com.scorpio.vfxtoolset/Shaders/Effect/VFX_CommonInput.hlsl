#ifndef _VFX_COMMON_INPUT_
#define _VFX_COMMON_INPUT_

sampler2D _BaseMap;
half4     _BaseMap_ST;
half4     _BaseColor;
half      _FrontIntensity;
half4     _BaseUVParams;
#define _DiffXSpeed   _BaseUVParams.x
#define _DiffYSpeed   _BaseUVParams.y
#define _MainTexScale _BaseUVParams.z
#define _MainTexAngle _BaseUVParams.w
half _UnMult;
half _BaseMapSwitchUV1;
half _BaseMultiplyLuminance;
half _EnableBaseMapPolarUV;

sampler2D _Mask;
half4 _Mask_ST;
half4 _MaskUVParams;
half  _MaskMapSwitchUV1;
half  _EnableMaskMapPolarUV;
half  _MaskNotEffectDiff;
half  _MaskedDissolve;
half  _EffectByMask;
#define  _MaskXSpeed _MaskUVParams.x
#define  _MaskYSpeed _MaskUVParams.y
#define  _MaskScale  _MaskUVParams.z
#define  _MaskAngle  _MaskUVParams.w
            
#if defined(_DOUBLE_SIDE_ON)
half4 _DiffuseBackColor;
half  _BackIntensity;
#endif

#if defined(_MIX_BASE_ON)
sampler2D _MixDiffuse;
half4 _MixDiffuse_ST;

half _MixDiffusePower;
half _MixMultiplyLuminance;
half _EnableRampMode;
half _MixRampUVOffset;
half4 _MixBaseMapUVParams;
#define _MixDiffuseXSpeed _MixBaseMapUVParams.x
#define _MixDiffuseYSpeed _MixBaseMapUVParams.y
#define _MixDiffuseScale  _MixBaseMapUVParams.z
#define _MixDiffuseAngle  _MixBaseMapUVParams.w
#endif

#if defined(_GRADIENT_ON)
half4 _LeftColor;
half4 _RightColor;
half _UVWeights;
half _LeftWeights;
half _RightWeights;
half _Gradient;
half _GradientSameDiffOn;
#endif

#if defined(_DISSOLVE_NOISE_ON)
sampler2D _DissolveTex;
half4 _DissolveTex_ST;
half4 _NoiseTex2_ST;
half4 _DissolveColor;
half  _DissolveStep; 
half  _DissolveColorPW;
half  _DissolveAngle;
half  _DissolveOutlineSoft;
half  _DissolveOutlineOn;
half  _DissolveOutlineWidth;
half  _SoftSize;
half  _NoiseStrength;
float4 _GChannel;
half _NoiseUnEffectDiff;

#if defined(_DUAL_LAYER_NOISE)
sampler2D _NoiseTex2;
half _NoiseStrength1;
#endif
# endif

#if defined(_COLOUR_ON)
half _EffectHue;
half _EffectSaturation;
half _EffectContrast;
half4 _SaturationLeftColor;
half4 _SaturationRightColor;
half  _SaturationLeftColorWeights;
half  _SaturationRightColorWeights;
#endif

#if defined(_FRESNEL_ON)
half  _FresnelPower;
half  _FresnelScale;
half4 _FresnelColor;
half _TransparentFresnelPart;
#endif

#if defined(_ENABLE_VERTEX_OFFSET)
sampler2D _VertexOffsetNoiseMap;
half4 _VONoiseTiling;
half4 _VOSpeedParams;
half3 _VOScale;
#endif

#if defined(_ENABLE_PLANAR_SOFT_PARTICLE)
half _ContactRange;
#endif

uniform float _HorizontalPlaneY;

#endif