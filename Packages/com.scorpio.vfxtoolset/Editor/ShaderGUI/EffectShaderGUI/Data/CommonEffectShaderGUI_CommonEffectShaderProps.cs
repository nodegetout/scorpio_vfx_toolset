using UnityEngine;

namespace HeroShowRenderingGUI.VFX.CommonEffect
{
    public static class ShaderPropIDs
    {
        // base map
        public static readonly string k_BaseMapPropId               = "_BaseMap";
        public static readonly string k_BaseColorPropId             = "_BaseColor";
        public static readonly string k_BaseFrontColorPropId        = "_BaseFrontColor";
        public static readonly string k_BaseMapUVParamsPropId       = "_BaseUVParams";
        public static readonly string k_BaseMapIntensityPropId      = "_FrontIntensity";
        public static readonly string k_DouYinEffectParamsPropId    = "_DouYinEffectParams";
        public static readonly string k_BaseMapTogglesPropId        = "_BaseMapToggles";

        // mask map
        public static readonly string k_MaskMapPropId              = "_Mask";
        public static readonly string k_MaskMapUVParamsPropId      = "_MaskUVParams";
        public static readonly string k_MaskMapParamsPropId        = "_MaskMapParams";
        public static readonly string k_MaskMapSwitchUV1PropId     = "_MaskMapSwitchUV1";
        public static readonly string k_EnableMaskMapPolarUVPropId = "_EnableMaskMapPolarUV";
        public static readonly string k_MaskMapTogglesPropId       = "_MaskMapToggles";
        
        // mix map
        public static readonly string k_MixBaseOnPropId = "_MixBaseOn";
        public static readonly string k_MixBaseMapPropId = "_MixDiffuse";
        public static readonly string k_MixTintColorPropId = "_MixTintColor";
        public static readonly string k_MixBasePolarUVOnPropId = "_MixBasePolarUVOn";
        public static readonly string k_MixMapParamsPropId = "_MixMapParams";
        public static readonly string k_MixBaseMapUVParamsPropId = "_MixBaseMapUVParams";
        
        // double side
        public static readonly string k_DoubleSideOnPropId          = "_DoubleSideOn";
        public static readonly string k_BackIntensityPropId         = "_BackIntensity";
        public static readonly string k_DiffuseBackColorPropId      = "_DiffuseBackColor";

        // fresnel
        public static readonly string k_FresnelOnPropId = "_FresnelOn";
        public static readonly string k_FresnelMapPropId = "_FresnelMap";
        public static readonly string k_FresnelMapUse2UPropId = "_FresnelMapUse2U";
        public static readonly string k_FresnelParamsPropId = "_FresnelParams";
        public static readonly string k_FresnelColorPropId = "_FresnelColor";
        
        // dissolve 
        public static readonly string k_DissolveOnPropId = "_DissolveOn";
        public static readonly string k_DissolveTexPropId = "_DissolveTex";
        public static readonly string k_DissolveParamsPropId = "_DissolveParams";
        public static readonly string k_DissolveEdgeParamsPropId = "_DissolveEdgeParams";
        public static readonly string k_DissolveColorPropId = "_DissolveColor";
        
        // flow map
        public static readonly string k_FlowMapOnPropId = "_FlowMapOn";
        public static readonly string k_FlowMapPropId = "_FlowMap";
        public static readonly string k_FlowMapParamsPropId = "_FlowMapParams";
        
        // noise
        public static readonly string k_NoiseOnPropId = "_NoiseOn";
        public static readonly string k_NoiseTex1PropId = "_NoiseTex1";
        public static readonly string k_NoiseUnEffectDiffPropId = "_NoiseUnEffectDiff";
        public static readonly string k_DualDirectionNoisePropId = "_DualDirectionNoise";
        public static readonly string k_NoiseTex2PropId = "_NoiseTex2";
        public static readonly string k_NoiseStrengthPropId = "_NoiseStrength";
        public static readonly string k_GChannelPropId = "_GChannel";
        
        // vertex offset
        public static readonly string k_EnableVertexOffsetPropId = "_EnableVertexOffset";
        public static readonly string k_VertexOffsetNoiseMapPropId = "_VertexOffsetNoiseMap";
        public static readonly string k_MotionDirPropId = "_MotionDir";
        public static readonly string k_VertexDirPropId = "_VertexDir";
        public static readonly string k_VertexOffsetParamsPropId = "_VertexOffsetParams";
        public static readonly string k_VertexMotionSpeedPropId = "_VertexMotionSpeed";

        // visibility testing
        public static readonly string k_DstBlendPropId = "_DstBlend";
        public static readonly string k_CullModePropId = "_CullMode";
        public static readonly string k_ZWritePropId   = "_ZWrite";
        public static readonly string k_ZTestPropId    = "_ZTest";
        
        // gradient
        public static readonly string k_GradientOnPropId = "_GradientOn";
        public static readonly string k_GradientSameDiffOnPropId = "_GradientSameDiffOn";
        public static readonly string k_LeftColorPropId = "_LeftColor";
        public static readonly string k_RightColorPropId = "_RightColor";
        public static readonly string k_GradientParamsPropId = "_GradientParams";
        
        // color grading 
        public static readonly string k_ColourOnPropId = "_ColourOn";
        public static readonly string k_ColorGradingParamsPropId = "_ColorGradingParams";
        public static readonly string k_SaturationRightColorPropId = "_SaturationRightColor";
        public static readonly string k_SaturationLeftColorPropId = "_SaturationLeftColor";
        public static readonly string k_SaturationRightColorWeightsPropId = "_SaturationRightColorWeights";
        public static readonly string k_SaturationLeftColorWeightsPropId = "_SaturationLeftColorWeights";
        
        // soft particle
        public static readonly string k_EnablePlanarSoftParticlePropId = "_EnablePlanarSoftParticle";
        public static readonly string k_ContactRangePropId             = "_ContactRange";
        public static readonly string k_HorizontalPlaneYPropId         = "_HorizontalPlaneY";
        
        // stencil
        public static readonly string k_StencilCompPropId = "_StencilComp";
        public static readonly string k_StencilPassPropId = "_StencilPass";
        public static readonly string k_StencilFailPropId = "_StencilFail";
        public static readonly string k_StencilRefPropId = "_StencilRef";
        // public static readonly string k_StencilZFailPropId = "_StencilZFail";
        // public static readonly string k_StencilReadMaskPropId = "_StencilReadMask";
        // public static readonly string k_StencilWriteMaskPropId = "_StencilWriteMask";
        
        // keywords
        public static readonly string k_MixMapModueKeyword              = "_MIX_BASE_ON";
        public static readonly string k_DissolveModueKeyword            = "_DISSOLVE_ON";
        public static readonly string k_VertexOffsetModueKeyword        = "_ENABLE_VERTEX_OFFSET";
        public static readonly string k_GradientModueKeyword            = "_GRADIENT_ON";
        public static readonly string k_PlannarSoftParticleModueKeyword = "_ENABLE_PLANAR_SOFT_PARTICLE";
        public static readonly string k_ColourModueKeyword              = "_COLOUR_ON";
        public static readonly string k_FresnelModueKeyword             = "_FRESNEL_ON";
        public static readonly string k_FlowMapModueKeyword             = "_FLOW_MAP_ON";
        public static readonly string k_NoiseModueKeyword               = "_NOISE_ON";
    }
}