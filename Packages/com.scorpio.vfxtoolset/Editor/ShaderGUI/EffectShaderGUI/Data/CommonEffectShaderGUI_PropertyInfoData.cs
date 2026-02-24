using HeroShowRenderingGUI.VFX.CommonEffect;

namespace HeroShowRenderingGUI.VFX
{
    public static class PropertyInfoData
    {
        public static readonly PropertyInfo[] k_MeshMainModulePropInfo = 
        {
            new PropertyInfo(ShaderPropIDs.k_BaseMapPropId, "主贴图"),
            new PropertyInfo(ShaderPropIDs.k_BaseMapTogglesPropId, string.Empty, PropertyType.MeshBaseMapTogglesProperty),
            new PropertyInfo(ShaderPropIDs.k_BaseColorPropId, "整体叠色"),
            new PropertyInfo(ShaderPropIDs.k_BaseFrontColorPropId, "前面叠色"),
            new PropertyInfo(ShaderPropIDs.k_BaseMapIntensityPropId, "颜色强度"),
            new PropertyInfo(ShaderPropIDs.k_DouYinEffectParamsPropId, string.Empty, PropertyType.DouYinEffectParamsProperty),
            new PropertyInfo(ShaderPropIDs.k_BaseMapUVParamsPropId, string.Empty, PropertyType.UVParamsProperty),
        };
        
        public static readonly PropertyInfo[] k_PsMainModulePropInfo = 
        {
            new PropertyInfo(ShaderPropIDs.k_BaseMapPropId, "主贴图"),
            new PropertyInfo(ShaderPropIDs.k_BaseMapTogglesPropId, string.Empty, PropertyType.PsBaseMapTogglesProperty),
            new PropertyInfo(ShaderPropIDs.k_BaseColorPropId, "整体叠色"),
            new PropertyInfo(ShaderPropIDs.k_BaseFrontColorPropId, "前面叠色"),
            new PropertyInfo(ShaderPropIDs.k_BaseMapIntensityPropId, "颜色强度"),
            new PropertyInfo(ShaderPropIDs.k_DouYinEffectParamsPropId, string.Empty, PropertyType.DouYinEffectParamsProperty),
            new PropertyInfo(ShaderPropIDs.k_BaseMapUVParamsPropId, string.Empty, PropertyType.UVParamsProperty),
        };
        
        public static readonly PropertyInfo[] k_MaskModulePropInfo =
        {
            new PropertyInfo(ShaderPropIDs.k_MaskMapPropId, "Mask贴图"),
            new PropertyInfo(ShaderPropIDs.k_MaskMapUVParamsPropId, string.Empty, PropertyType.UVParamsProperty),
            new PropertyInfo(ShaderPropIDs.k_MaskMapParamsPropId, string.Empty, PropertyType.MaskMapParamsProperty),
            new PropertyInfo(ShaderPropIDs.k_MaskMapSwitchUV1PropId, "切换2U"),
            new PropertyInfo(ShaderPropIDs.k_EnableMaskMapPolarUVPropId, "开启极坐标(禁动画中K开关)"),
            new PropertyInfo(ShaderPropIDs.k_MaskMapTogglesPropId, string.Empty, PropertyType.MaskMapTogglesProperty),
        };
        
        public static readonly PropertyInfo[] k_UVModulePropInfo =
        {
            new PropertyInfo("_RequireCustomData", "开启CustomData"),
            new PropertyInfo("_EnableScreenUV", "启用屏幕UV")
        };
        
        public static readonly PropertyInfo[] k_DoubleSideModulePropInfo =
        {
            new PropertyInfo(ShaderPropIDs.k_DoubleSideOnPropId, "开启双面材质(禁动画中K开关)"),
            new PropertyInfo(ShaderPropIDs.k_DiffuseBackColorPropId, "背面颜色"),
            new PropertyInfo(ShaderPropIDs.k_BackIntensityPropId, "背面颜色强度"),
        };
        
        public static readonly PropertyInfo[] k_StencilModulePropInfo =
        {
            new PropertyInfo(ShaderPropIDs.k_StencilRefPropId,"模板参考值"),
            new PropertyInfo(ShaderPropIDs.k_StencilCompPropId,"比较方式"),
            new PropertyInfo(ShaderPropIDs.k_StencilPassPropId,"通过运算"),
            new PropertyInfo(ShaderPropIDs.k_StencilFailPropId,"失败运算"),
            // new PropertyInfo(StencilModuleProps.k_StencilZFailPropId,"StencilZFail"),
            // new PropertyInfo(StencilModuleProps.k_StencilReadMaskPropId,"StencilReadMask"),
            // new PropertyInfo(StencilModuleProps.k_StencilWriteMaskPropId,"StencilWriteMask"),
        };
        
        public static readonly PropertyInfo[] k_FresnelModulePropInfo =
        {
            new PropertyInfo(ShaderPropIDs.k_FresnelOnPropId, "开启Fresnel(禁动画中K开关)"),
            new PropertyInfo(ShaderPropIDs.k_FresnelMapPropId, "叠乘贴图" ),
            new PropertyInfo(ShaderPropIDs.k_FresnelMapUse2UPropId, "使用2U" ),
            new PropertyInfo(ShaderPropIDs.k_FresnelColorPropId, "颜色" ),
            new PropertyInfo(ShaderPropIDs.k_FresnelParamsPropId, string.Empty, PropertyType.FresnelParamsProperty),
        };
        
        public static readonly PropertyInfo[] k_SoftParticleModulePropInfo =
        {
            new PropertyInfo(ShaderPropIDs.k_EnablePlanarSoftParticlePropId, "开启软粒子(禁动画中K开关)"),
            new PropertyInfo(ShaderPropIDs.k_ContactRangePropId, "软化范围" ),
            new PropertyInfo(ShaderPropIDs.k_HorizontalPlaneYPropId, "水平面Y值"),
        };
        
        public static readonly PropertyInfo[] k_MergeStageModulePropInfo =
        {
            new PropertyInfo(ShaderPropIDs.k_DstBlendPropId, "混合模式"),
            new PropertyInfo(ShaderPropIDs.k_CullModePropId, "剔除模式"),
            new PropertyInfo(ShaderPropIDs.k_ZWritePropId, "深度测试"),
            new PropertyInfo(ShaderPropIDs.k_ZTestPropId, "总是最前"),
        };

        public static readonly PropertyInfo[] k_MixBaseMapModulePropInfo =
        {
            new PropertyInfo(ShaderPropIDs.k_MixBaseOnPropId, "开启混合贴图模块(禁动画中K开关)"),
            new PropertyInfo(ShaderPropIDs.k_MixBaseMapPropId, "混合贴图"),
            new PropertyInfo(ShaderPropIDs.k_MixTintColorPropId, "混合贴图叠色"),
            new PropertyInfo(ShaderPropIDs.k_MixBasePolarUVOnPropId, "开启极坐标"),
            new PropertyInfo(ShaderPropIDs.k_MixMapParamsPropId, string.Empty, PropertyType.MixMapParamsProperty),
            new PropertyInfo(ShaderPropIDs.k_MixBaseMapUVParamsPropId, string.Empty, PropertyType.UVParamsProperty),
        };

        public static readonly PropertyInfo[] k_DissolveModulePropInfo =
        {
            // dissolve part
            new PropertyInfo(ShaderPropIDs.k_DissolveOnPropId, "开启溶解扰动(禁动画中K开关)"),
            new PropertyInfo(ShaderPropIDs.k_DissolveTexPropId, "溶解扰动贴图"),
            new PropertyInfo(ShaderPropIDs.k_DissolveParamsPropId, string.Empty, PropertyType.DissolveParamsProperty),
            new PropertyInfo(ShaderPropIDs.k_DissolveEdgeParamsPropId, string.Empty, PropertyType.DissolveEdgeParamsProperty),
            new PropertyInfo(ShaderPropIDs.k_DissolveColorPropId, "边缘颜色"),
        };
        
        public static readonly PropertyInfo[] k_FlowMapModulePropInfo =
        {
            new PropertyInfo(ShaderPropIDs.k_FlowMapOnPropId, "开启FlowMap(禁动画中K开关)"),
            new PropertyInfo(ShaderPropIDs.k_FlowMapPropId, "FlowMap贴图"),
            new PropertyInfo(ShaderPropIDs.k_FlowMapParamsPropId, string.Empty, PropertyType.FlowMapParamsProperty),
        };
        
        public static readonly PropertyInfo[] k_NoiseModulePropInfo =
        {
            // noise part
            new PropertyInfo(ShaderPropIDs.k_NoiseOnPropId, "开启溶解扰动(禁动画中K开关)"),
            new PropertyInfo(ShaderPropIDs.k_NoiseTex1PropId, "扰动贴图"),
            new PropertyInfo(ShaderPropIDs.k_NoiseUnEffectDiffPropId, "Noise不影响主贴图(禁动画中K开关)"),
            new PropertyInfo(ShaderPropIDs.k_DualDirectionNoisePropId, "开启双层Noise效果(禁动画中K开关)"),
            new PropertyInfo(ShaderPropIDs.k_NoiseTex2PropId, "第二层noise图"),
            new PropertyInfo(ShaderPropIDs.k_GChannelPropId, "UV控制(xy第一层noise速度 zw第二层noise速度)"),
            new PropertyInfo(ShaderPropIDs.k_NoiseStrengthPropId, string.Empty, PropertyType.NoiseStrengthParamsProperty),
        };

        public static readonly PropertyInfo[] k_VertexOffsetModulePropInfo =
        {
            new PropertyInfo(ShaderPropIDs.k_EnableVertexOffsetPropId, "开启顶点偏移(禁动画中K开关)"),
            new PropertyInfo(ShaderPropIDs.k_VertexOffsetNoiseMapPropId, "定点偏移噪声图(R通道)"),
            new PropertyInfo(ShaderPropIDs.k_MotionDirPropId, "运动方向"),
            new PropertyInfo(ShaderPropIDs.k_VertexDirPropId, "顶点方向"),
            new PropertyInfo(ShaderPropIDs.k_VertexOffsetParamsPropId, string.Empty, PropertyType.VertexOffsetParamsProperty),
            new PropertyInfo(ShaderPropIDs.k_VertexMotionSpeedPropId, "VertexMotionSpeed"),
        };

        public static readonly PropertyInfo[] k_GradientModulePropInfo =
        {
            new PropertyInfo(ShaderPropIDs.k_GradientOnPropId, "左右渐变颜色开关(禁动画中K开关)"),
            new PropertyInfo(ShaderPropIDs.k_GradientSameDiffOnPropId, "左右渐变开启Diff相同UV(禁动画中K开关)"),
            new PropertyInfo(ShaderPropIDs.k_LeftColorPropId, "左侧渐变色"),
            new PropertyInfo(ShaderPropIDs.k_RightColorPropId, "右侧渐变色"),
            new PropertyInfo(ShaderPropIDs.k_GradientParamsPropId, string.Empty, PropertyType.GradientParamsProperty)
        };

        public static readonly PropertyInfo[] k_ColorGradingModulePropInfo =
        {
            new PropertyInfo(ShaderPropIDs.k_ColourOnPropId, "色彩开关(禁动画中K开关)"),
            new PropertyInfo(ShaderPropIDs.k_ColorGradingParamsPropId, string.Empty, PropertyType.ColorGradingParamsProperty),
            new PropertyInfo(ShaderPropIDs.k_SaturationRightColorPropId, "灰度渐变亮色"),
            new PropertyInfo(ShaderPropIDs.k_SaturationLeftColorPropId, "灰度渐变暗色"),
            new PropertyInfo(ShaderPropIDs.k_SaturationRightColorWeightsPropId, "灰度渐变亮色权重"),
            new PropertyInfo(ShaderPropIDs.k_SaturationLeftColorWeightsPropId, "灰度渐变暗色权重"),
        };
    }
}