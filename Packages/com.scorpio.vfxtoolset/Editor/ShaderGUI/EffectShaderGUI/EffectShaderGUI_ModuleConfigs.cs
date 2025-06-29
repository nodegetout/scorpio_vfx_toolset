namespace com.scorpio.vfxtoolset.Editor
{
    public static class EffectModuleConfigs
    {
        public static readonly PropertyInfo[] k_MainModulePropConfig = 
        {
            new PropertyInfo(BaseMapModuleProps.k_BaseMapPropId, "主贴图"),
            new PropertyInfo(BaseMapModuleProps.k_BaseMapMultiplyAlphaPropId, "开启预乘Alpha"),
            new PropertyInfo(BaseMapModuleProps.k_BaseMultiplyLuminancePropId, "去黑底"),
            new PropertyInfo(BaseMapModuleProps.k_EnableBaseMapPolarUVPropId, "开启极坐标(禁动画中K开关)"),
            new PropertyInfo(BaseMapModuleProps.k_BaseMapSwitchUV1PropId, "切换为2U"),
            new PropertyInfo(BaseMapModuleProps.k_BaseColorPropId, "主贴图叠色"),
            new PropertyInfo(BaseMapModuleProps.k_BaseMapIntensityPropId, "颜色强度"),
            new PropertyInfo(BaseMapModuleProps.k_BaseMapUVParamsPropId, string.Empty, PropertyType.UVParamsProperty),
        };
        
        public static readonly PropertyInfo[] k_MaskModulePropConfig =
        {
            new PropertyInfo(MaskModuleProps.k_MaskMapPropId, "Mask贴图"),
            new PropertyInfo(MaskModuleProps.k_MaskMapUVParamsPropId, string.Empty, PropertyType.UVParamsProperty),
            new PropertyInfo(MaskModuleProps.k_MaskMapSwitchUV1PropId, "切换2U"),
            new PropertyInfo(MaskModuleProps.k_EnableMaskMapPolarUVPropId, "开启极坐标(禁动画中K开关)"),
            new PropertyInfo(MaskModuleProps.k_MaskNotEffectDiffPropId, "R通道不影响主贴图Alpha(禁动画中K开关)"),
            new PropertyInfo(MaskModuleProps.k_MaskedDissolvePropId, "G通道影响溶解(禁动画中K开关)"),
            new PropertyInfo(MaskModuleProps.k_EffectByMaskPropId, "B通道影响扰动(禁动画中K开关)"),
        };
        
        public static readonly PropertyInfo[] k_UVModulePropConfig =
        {
            new PropertyInfo("_RequireCustomData", "开启CustomData"),
            new PropertyInfo("_EnableScreenUV", "启用屏幕UV")
        };
        
        public static readonly PropertyInfo[] k_DoubleSideModulePropConfig =
        {
            new PropertyInfo(DoubleSideModuleProps.k_DoubleSideOnPropId, "开启双面材质(禁动画中K开关)"),
            new PropertyInfo(DoubleSideModuleProps.k_DiffuseBackColorPropId, "背面颜色"),
            new PropertyInfo(DoubleSideModuleProps.k_BackIntensityPropId, "背面颜色强度"),
        };
        
        public static readonly PropertyInfo[] k_TestConfigModulePropConfig =
        {
            new PropertyInfo(VisibilityTestingModuleProps.k_CullModePropId, "剔除模式"),
            new PropertyInfo(VisibilityTestingModuleProps.k_ZWritePropId, "深度测试" ),
            new PropertyInfo(VisibilityTestingModuleProps.k_ZTestPropId, "总是最前"), 
        };
        
        public static readonly PropertyInfo[] k_StencilModulePropConfig =
        {
            new PropertyInfo(StencilModuleProps.k_StencilRefPropId,"模板参考值"),
            new PropertyInfo(StencilModuleProps.k_StencilCompPropId,"比较方式"),
            new PropertyInfo(StencilModuleProps.k_StencilPassPropId,"通过运算"),
            new PropertyInfo(StencilModuleProps.k_StencilFailPropId,"失败运算"),
            // new PropertyInfo(StencilModuleProps.k_StencilZFailPropId,"StencilZFail"),
            // new PropertyInfo(StencilModuleProps.k_StencilReadMaskPropId,"StencilReadMask"),
            // new PropertyInfo(StencilModuleProps.k_StencilWriteMaskPropId,"StencilWriteMask"),
        };
        
        public static readonly PropertyInfo[] k_FresnelModulePropConfig =
        {
            new PropertyInfo(FresnelModuleProps.k_FresnelOnPropId, "开启Fresnel(禁动画中K开关)"),
            new PropertyInfo(FresnelModuleProps.k_FresnelColorPropId, "颜色" ),
            new PropertyInfo(FresnelModuleProps.k_TransparentFresnelPartPropId, "反向Fresnel Alpha"),
            new PropertyInfo(FresnelModuleProps.k_FresnelPowerPropId, "强度"),
            new PropertyInfo(FresnelModuleProps.k_FresnelScalePropId, "范围"),
        };
        
        public static readonly PropertyInfo[] k_SoftParticleModulePropConfig =
        {
            new PropertyInfo(SoftParticleModuleProps.k_EnablePlanarSoftParticlePropId, "开启软粒子(禁动画中K开关)"),
            new PropertyInfo(SoftParticleModuleProps.k_ContactRangePropId, "软化范围" ),
            new PropertyInfo(SoftParticleModuleProps.k_HorizontalPlaneYPropId, "水平面Y值"),
        };
    }
    
    public static class BaseMapModuleProps
    {
        public static readonly string k_BaseMapPropId               = "_BaseMap";
        public static readonly string k_BaseColorPropId             = "_BaseColor";
        public static readonly string k_BaseMapUVParamsPropId       = "_BaseUVParams";
        public static readonly string k_BaseMapIntensityPropId      = "_FrontIntensity";
        public static readonly string k_BaseMapMultiplyAlphaPropId  = "_UnMult";
        public static readonly string k_BaseMultiplyLuminancePropId = "_BaseMultiplyLuminance";
        public static readonly string k_BaseMapSwitchUV1PropId      = "_BaseMapSwitchUV1";
        public static readonly string k_EnableBaseMapPolarUVPropId  = "_EnableBaseMapPolarUV";
    }

    public static class MaskModuleProps
    {
        public static readonly string k_MaskMapPropId              = "_Mask";
        public static readonly string k_MaskMapUVParamsPropId      = "_MaskUVParams";
        public static readonly string k_MaskMapSwitchUV1PropId     = "_MaskMapSwitchUV1";
        public static readonly string k_MaskNotEffectDiffPropId    = "_MaskNotEffectDiff";
        public static readonly string k_MaskedDissolvePropId       = "_MaskedDissolve";
        public static readonly string k_EffectByMaskPropId         = "_EffectByMask";
        public static readonly string k_EnableMaskMapPolarUVPropId = "_EnableMaskMapPolarUV";
    }

    public static class DoubleSideModuleProps
    {
        public static readonly string k_DoubleSideOnPropId          = "_DoubleSideOn";
        public static readonly string k_MixMultiplyLuminancePropId  = "_MixMultiplyLuminance";
        public static readonly string k_BackIntensityPropId         = "_BackIntensity";
        public static readonly string k_DiffuseBackColorPropId      = "_DiffuseBackColor";
    }
    
    public static class FresnelModuleProps
    {
        public static readonly string k_FresnelOnPropId = "_FresnelOn";
        public static readonly string k_TransparentFresnelPartPropId = "_TransparentFresnelPart";
        public static readonly string k_FresnelPowerPropId = "_FresnelPower";
        public static readonly string k_FresnelScalePropId = "_FresnelScale";
        public static readonly string k_FresnelColorPropId = "_FresnelColor";
    }
    
    public static class VisibilityTestingModuleProps
    {
        public static readonly string k_CullModePropId = "_CullMode";
        public static readonly string k_ZWritePropId   = "_ZWrite";
        public static readonly string k_ZTestPropId    = "_ZTest";
    }
    public static class SoftParticleModuleProps
    {
        public static readonly string k_EnablePlanarSoftParticlePropId = "_EnablePlanarSoftParticle";
        public static readonly string k_ContactRangePropId             = "_ContactRange";
        public static readonly string k_HorizontalPlaneYPropId         = "_HorizontalPlaneY";
    }

    public static class StencilModuleProps
    {
        public static readonly string k_StencilCompPropId = "_StencilComp";
        public static readonly string k_StencilPassPropId = "_StencilPass";
        public static readonly string k_StencilFailPropId = "_StencilFail";
        public static readonly string k_StencilZFailPropId = "_StencilZFail";
        public static readonly string k_StencilRefPropId = "_StencilRef";
        public static readonly string k_StencilReadMaskPropId = "_StencilReadMask";
        public static readonly string k_StencilWriteMaskPropId = "_StencilWriteMask";
    }
}