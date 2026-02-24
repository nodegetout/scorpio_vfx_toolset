using System.Collections.Generic;
using HeroShowRenderingGUI.VFX.CommonEffect;

namespace HeroShowRenderingGUI.VFX
{
    public static class CommonEffectModuleConfigs
    {
        // shader feature modules config data
        public static List<ModuleViewBase> sfShaderModulesConfigData = new List<ModuleViewBase>()
        {
            // persistent modules
            new PersistentModuleView("【合并阶段设置】", PropertyInfoData.k_MergeStageModulePropInfo),
            new PersistentModuleView("【UV模式设置】",PropertyInfoData.k_UVModulePropInfo),
            new PersistentModuleView("【模板缓存设置】",PropertyInfoData.k_StencilModulePropInfo),
            
            new PersistentModuleView("【遮罩设置】",PropertyInfoData.k_MaskModulePropInfo),
            
            // uniform modules
            new UniformModuleView("【双面设置】", ShaderPropIDs.k_DoubleSideOnPropId, PropertyInfoData.k_DoubleSideModulePropInfo),
            
            // keyword modules
            new KeywordModuleView("【混合贴图设置】", ShaderPropIDs.k_MixMapModueKeyword, PropertyInfoData.k_MixBaseMapModulePropInfo),
            new KeywordModuleView("【溶解设置】", ShaderPropIDs.k_DissolveModueKeyword, PropertyInfoData.k_DissolveModulePropInfo),
            new KeywordModuleView("【FlowMap设置】", ShaderPropIDs.k_FlowMapModueKeyword, PropertyInfoData.k_FlowMapModulePropInfo),
            new KeywordModuleView("【扰动设置】", ShaderPropIDs.k_NoiseModueKeyword, PropertyInfoData.k_NoiseModulePropInfo),
            new KeywordModuleView("【Fresnel设置】", ShaderPropIDs.k_FresnelModueKeyword, PropertyInfoData.k_FresnelModulePropInfo),
            new KeywordModuleView("【顶点偏移设置】", ShaderPropIDs.k_VertexOffsetModueKeyword, PropertyInfoData.k_VertexOffsetModulePropInfo),
            new KeywordModuleView("【渐变设置】", ShaderPropIDs.k_GradientModueKeyword, PropertyInfoData.k_GradientModulePropInfo),
            new KeywordModuleView("【软粒子设置】", ShaderPropIDs.k_PlannarSoftParticleModueKeyword, PropertyInfoData.k_SoftParticleModulePropInfo),
            new KeywordModuleView("【调色设置】", ShaderPropIDs.k_ColourModueKeyword, PropertyInfoData.k_ColorGradingModulePropInfo),
        };
        
        public static List<ModuleViewBase> obsoleteShaderModulesConfigData = new List<ModuleViewBase>()
        {
            // persistent modules
            new PersistentModuleView("【合并阶段设置】", PropertyInfoData.k_MergeStageModulePropInfo),
            new PersistentModuleView("【UV模式设置】",PropertyInfoData.k_UVModulePropInfo),
            new PersistentModuleView("【模板缓存设置】",PropertyInfoData.k_StencilModulePropInfo),
            new PersistentModuleView("【遮罩设置】",PropertyInfoData.k_MaskModulePropInfo),
            
            // uniform modules
            new UniformModuleView("【双面设置】", ShaderPropIDs.k_DoubleSideOnPropId, PropertyInfoData.k_DoubleSideModulePropInfo),
            
            // keyword modules
            new KeywordModuleView("【混合贴图设置】", ShaderPropIDs.k_MixMapModueKeyword, PropertyInfoData.k_MixBaseMapModulePropInfo),
            new KeywordModuleView("【溶解设置】", ShaderPropIDs.k_DissolveModueKeyword, PropertyInfoData.k_DissolveModulePropInfo),
            // new KeywordModuleView("【FlowMap设置】", ShaderPropIDs.k_FlowMapModueKeyword, PropertyInfoData.k_FlowMapModulePropInfo),
            // new KeywordModuleView("【扰动设置】", ShaderPropIDs.k_NoiseModueKeyword, PropertyInfoData.k_NoiseModulePropInfo),
            new KeywordModuleView("【顶点偏移设置】", ShaderPropIDs.k_VertexOffsetModueKeyword, PropertyInfoData.k_VertexOffsetModulePropInfo),
            
            // uniform modules
            new UniformModuleView("【Fresnel设置】", ShaderPropIDs.k_FresnelModueKeyword, PropertyInfoData.k_FresnelModulePropInfo),
            new UniformModuleView("【渐变设置】", ShaderPropIDs.k_GradientModueKeyword, PropertyInfoData.k_GradientModulePropInfo),
            new UniformModuleView("【软粒子设置】", ShaderPropIDs.k_EnablePlanarSoftParticlePropId, PropertyInfoData.k_SoftParticleModulePropInfo),
            new UniformModuleView("【调色设置】", ShaderPropIDs.k_ColourModueKeyword, PropertyInfoData.k_ColorGradingModulePropInfo),
        };
    }
}