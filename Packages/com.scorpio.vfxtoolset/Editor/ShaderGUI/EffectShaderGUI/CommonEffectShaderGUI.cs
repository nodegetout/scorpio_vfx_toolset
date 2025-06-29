using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.scorpio.vfxtoolset.Editor
{
    public sealed class CommonEffectShaderGUI : EffectShaderGUIBase
    {
        #region Shader Property Names
        private static class MixBaseModuleProps
        {
            public static readonly string k_MixBaseOnPropId = "_MixBaseOn";
            public static readonly string k_MixBaseMapPropId = "_MixDiffuse";
            public static readonly string k_MixBaseMapIntensityPropId = "_MixDiffusePower";
            public static readonly string k_EnableRampModePropId = "_EnableRampMode";
            public static readonly string k_MixRampUVOffsetPropId = "_MixRampUVOffset";
            public static readonly string k_MixBaseMapUVParamsPropId = "_MixBaseMapUVParams";
        }
        private static class DissolveNoiseModuleProps
        {
            public static readonly string k_DissolveNoiseOnPropId = "_DissolveNoiseOn";
            public static readonly string k_DissolveTexPropId = "_DissolveTex";
            public static readonly string k_DissolveAnglePropId = "_DissolveAngle";
            public static readonly string k_DissolveStepPropId = "_DissolveStep";
            public static readonly string k_SoftSizePropId = "_SoftSize";
            public static readonly string k_DissolveOutlineOnPropId = "_DissolveOutlineOn";
            public static readonly string k_DissolveOutlineWidthPropId = "_DissolveOutlineWidth";
            public static readonly string k_DissolveOutlineSoftPropId = "_DissolveOutlineSoft";
            public static readonly string k_DissolveColorPWPropId = "_DissolveColorPW";
            public static readonly string k_DissolveColorPropId = "_DissolveColor";
            public static readonly string k_NoiseUnEffectDiffPropId = "_NoiseUnEffectDiff";
            public static readonly string k_DualDirectionNoisePropId = "_DualDirectionNoise";
            public static readonly string k_NoiseTex2PropId = "_NoiseTex2";
            public static readonly string k_NoiseStrengthPropId = "_NoiseStrength";
            public static readonly string k_NoiseStrength1PropId = "_NoiseStrength1";
            public static readonly string k_GChannelPropId = "_GChannel";
        }
        public static class GradientModuleProps
        {
            public static readonly string k_GradientOnPropId = "_GradientOn";
            public static readonly string k_GradientSameDiffOnPropId = "_GradientSameDiffOn";
            public static readonly string k_LeftColorPropId = "_LeftColor";
            public static readonly string k_RightColorPropId = "_RightColor";
            public static readonly string k_UVWeightsPropId = "_UVWeights";
            public static readonly string k_LeftWeightsPropId = "_LeftWeights";
            public static readonly string k_RightWeightsPropId = "_RightWeights";
            public static readonly string k_GradientPropId = "_Gradient";
        }
        public static class ColorCorrectionModuleProps
        {
            public static readonly string k_ColourOnPropId = "_ColourOn";
            public static readonly string k_EffectHuePropId = "_EffectHue";
            public static readonly string k_EffectSaturationPropId = "_EffectSaturation";
            public static readonly string k_EffectContrastPropId = "_EffectContrast";
            public static readonly string k_SaturationRightColorPropId = "_SaturationRightColor";
            public static readonly string k_SaturationLeftColorPropId = "_SaturationLeftColor";
            public static readonly string k_SaturationRightColorWeightsPropId = "_SaturationRightColorWeights";
            public static readonly string k_SaturationLeftColorWeightsPropId = "_SaturationLeftColorWeights";
        }
        
        public static class VertexOffsetModuleProps
        {
            public static readonly string k_ToggleVertexOffsetPropId   = "_ToggleVertexOffset";
            public static readonly string k_VertexOffsetNoiseMapPropId = "_VertexOffsetNoiseMap";
            public static readonly string k_VONoiseTilingPropId        = "_VONoiseTiling";
            public static readonly string k_VOSpeedParamsPropId        = "_VOSpeedParams";
            public static readonly string k_VOScalePropId              = "_VOScale";
        }
        #endregion
        
        #region Module Configuration Data
        
        private static readonly PropertyInfo[] k_MixBaseMapModulePropConfig =
        {
            new PropertyInfo(MixBaseModuleProps.k_MixBaseOnPropId, "开启混合贴图模块(禁动画中K开关)"),
            new PropertyInfo(DoubleSideModuleProps.k_MixMultiplyLuminancePropId, "去黑底"),
            new PropertyInfo(MixBaseModuleProps.k_MixBaseMapPropId, "混合贴图"),
            new PropertyInfo(MixBaseModuleProps.k_MixBaseMapIntensityPropId, "混合贴图强度"),
            new PropertyInfo(MixBaseModuleProps.k_EnableRampModePropId, "开启Ramp混色模式"),
            new PropertyInfo(MixBaseModuleProps.k_MixRampUVOffsetPropId, "Ramp混合UV偏移"),
            new PropertyInfo(MixBaseModuleProps.k_MixBaseMapUVParamsPropId, string.Empty, PropertyType.UVParamsProperty),
        };
        
        private static readonly PropertyInfo[] k_DissolveNoiseModulePropConfig =
        {
            new PropertyInfo(DissolveNoiseModuleProps.k_DissolveNoiseOnPropId, "开启溶解扰动(禁动画中K开关)"),
            new PropertyInfo(DissolveNoiseModuleProps.k_DissolveTexPropId, "溶解扰动贴图" ),
            new PropertyInfo(DissolveNoiseModuleProps.k_DissolveAnglePropId, "贴图旋转"),
            new PropertyInfo(DissolveNoiseModuleProps.k_DissolveStepPropId, "溶解强度"),
            new PropertyInfo(DissolveNoiseModuleProps.k_SoftSizePropId, "溶解软硬"),
            new PropertyInfo(DissolveNoiseModuleProps.k_DissolveOutlineOnPropId, "开启溶解边缘叠色(禁动画中K开关)"),
            new PropertyInfo(DissolveNoiseModuleProps.k_DissolveOutlineWidthPropId, "边缘宽度"),
            new PropertyInfo(DissolveNoiseModuleProps.k_DissolveOutlineSoftPropId, "边缘软硬"),
            new PropertyInfo(DissolveNoiseModuleProps.k_DissolveColorPWPropId, "边缘强度"),
            new PropertyInfo(DissolveNoiseModuleProps.k_DissolveColorPropId, "边缘颜色"),
            new PropertyInfo(DissolveNoiseModuleProps.k_NoiseUnEffectDiffPropId, "Noise不影响主贴图(禁动画中K开关)"),
            new PropertyInfo(DissolveNoiseModuleProps.k_NoiseStrengthPropId, "扭曲强度"),
            new PropertyInfo(DissolveNoiseModuleProps.k_DualDirectionNoisePropId, "开启双层Noise效果(禁动画中K开关)"),
            new PropertyInfo(DissolveNoiseModuleProps.k_NoiseTex2PropId, "第二层noise图"),
            new PropertyInfo(DissolveNoiseModuleProps.k_NoiseStrength1PropId, "第二层扭曲强度"),
            new PropertyInfo(DissolveNoiseModuleProps.k_GChannelPropId, "UV控制(xy第一层noise速度 zw第二层noise速度)"),
        };

        private static readonly PropertyInfo[] k_VertexOffsetModulePropConfig =
        {
            new PropertyInfo(VertexOffsetModuleProps.k_ToggleVertexOffsetPropId, "开启顶点偏移(禁动画中K开关)"),
            new PropertyInfo(VertexOffsetModuleProps.k_VertexOffsetNoiseMapPropId, "定点偏移噪声图"),
            new PropertyInfo(VertexOffsetModuleProps.k_VONoiseTilingPropId, "偏移噪声Tiling"),
            new PropertyInfo(VertexOffsetModuleProps.k_VOSpeedParamsPropId, "偏移速度设置"),
            new PropertyInfo(VertexOffsetModuleProps.k_VOScalePropId, "偏移振幅缩放"),
        };
        private static readonly PropertyInfo[] k_GradientModulePropConfig =
        {
            new PropertyInfo(GradientModuleProps.k_GradientOnPropId, "左右渐变颜色开关(禁动画中K开关)"),
            new PropertyInfo(GradientModuleProps.k_GradientSameDiffOnPropId, "左右渐变开启Diff相同UV(禁动画中K开关)" ),
            new PropertyInfo(GradientModuleProps.k_LeftColorPropId, "左侧渐变色"),
            new PropertyInfo(GradientModuleProps.k_RightColorPropId, "右侧渐变色"),
            new PropertyInfo(GradientModuleProps.k_UVWeightsPropId, "UV权重"),
            new PropertyInfo(GradientModuleProps.k_LeftWeightsPropId, "左侧渐变色权重"),
            new PropertyInfo(GradientModuleProps.k_RightWeightsPropId, "右侧渐变色权重"),
            new PropertyInfo(GradientModuleProps.k_GradientPropId, "渐变色权重偏移"),
        };
        
        private static readonly PropertyInfo[] k_ColorGradingModulePropConfig = 
        {
            new PropertyInfo(ColorCorrectionModuleProps.k_ColourOnPropId, "色彩开关(禁动画中K开关)"),
            new PropertyInfo(ColorCorrectionModuleProps.k_EffectHuePropId, "色相" ),
            new PropertyInfo(ColorCorrectionModuleProps.k_EffectSaturationPropId, "饱和度"),
            new PropertyInfo(ColorCorrectionModuleProps.k_EffectContrastPropId, "对比度"),
            new PropertyInfo(ColorCorrectionModuleProps.k_SaturationRightColorPropId, "灰度渐变亮色"),
            new PropertyInfo(ColorCorrectionModuleProps.k_SaturationLeftColorPropId, "灰度渐变暗色"),
            new PropertyInfo(ColorCorrectionModuleProps.k_SaturationRightColorWeightsPropId, "灰度渐变亮色权重"),
            new PropertyInfo(ColorCorrectionModuleProps.k_SaturationLeftColorWeightsPropId, "灰度渐变暗色权重"),
        };
        
        private List<ModuleData> _moduleConfigData = new List<ModuleData>
        {
            new ModuleData("【合并阶段设置】", new[]
            {
                new PropertyInfo("_DstBlend", "混合模式"),
                new PropertyInfo(VisibilityTestingModuleProps.k_CullModePropId, "剔除模式"),
                new PropertyInfo(VisibilityTestingModuleProps.k_ZWritePropId, "深度测试" ),
                new PropertyInfo(VisibilityTestingModuleProps.k_ZTestPropId, "总是最前"), 
            }),
            new ModuleData("【UV模式设置】", EffectModuleConfigs.k_UVModulePropConfig),
            new ModuleData("【主贴图设置】", EffectModuleConfigs.k_MainModulePropConfig),
            new ModuleData("【遮罩设置】", EffectModuleConfigs.k_MaskModulePropConfig),
            new ModuleData("【双面设置】", EffectModuleConfigs.k_DoubleSideModulePropConfig),
            new ModuleData("【混合贴图设置】", k_MixBaseMapModulePropConfig),
            new ModuleData("【溶解与扰动设置】", k_DissolveNoiseModulePropConfig),
            new ModuleData("【Fresnel设置】", EffectModuleConfigs.k_FresnelModulePropConfig),
            new ModuleData("【顶点偏移设置】", k_VertexOffsetModulePropConfig),
            new ModuleData("【渐变设置】", k_GradientModulePropConfig),
            new ModuleData("【软粒子设置】", EffectModuleConfigs.k_SoftParticleModulePropConfig),
            new ModuleData("【调色设置】", k_ColorGradingModulePropConfig),
            new ModuleData("【模板缓存设置】", EffectModuleConfigs.k_StencilModulePropConfig),
        };
        #endregion
        

        #region Property DrawerFunctions
        private void DrawUVParamsProperty(MaterialEditor materialEditor, MaterialProperty property, string label)
        {
            float diffXSpeed   = EditorGUILayout.FloatField("贴图U方向流速", property.vectorValue.x);
            float diffYSpeed   = EditorGUILayout.FloatField("贴图V方向流速", property.vectorValue.y);
            float mainTexScale = EditorGUILayout.Slider("贴图缩放", property.vectorValue.z, 0.0f, 10.0f);
            float mainTexAngle = EditorGUILayout.Slider("贴图旋转", property.vectorValue.w, 0.0f, 720.0f);
            property.vectorValue = new Vector4(diffXSpeed, diffYSpeed, mainTexScale, mainTexAngle);
        }
        #endregion

        protected override void AppendModuleData()
        {
            foreach (var configData in _moduleConfigData)
            {
                if (!s_moduleDataMap.ContainsKey(configData.moduleName))
                {
                    s_moduleDataMap.Add(configData.moduleName, configData);
                }
            }
        }
        
        protected override void AppendPropertyDrawerFunc()
        {
            _customPropertyFuncMap.Add(PropertyType.UVParamsProperty, DrawUVParamsProperty);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Material material = materialEditor.target as Material;
            RemoveCustomDataModuleForMeshEffect(material);
            base.OnGUI(materialEditor, properties);
        }

        private void RemoveCustomDataModuleForMeshEffect(Material material)
        {
            if (material != null && material.shader.name.Contains("MeshEffect"))
            {
                // Debug.Log($"{material.shader.name}");
                var moduleData = _moduleConfigData.Find(x => x.moduleName.Equals("【CustomData设置】"));
                if (moduleData != null)
                {
                    _moduleConfigData.Remove(moduleData);
                }
            }
        }

    }
}