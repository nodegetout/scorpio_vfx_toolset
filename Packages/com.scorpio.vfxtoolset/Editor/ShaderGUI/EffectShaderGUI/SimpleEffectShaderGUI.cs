using UnityEditor;
using UnityEngine;

namespace com.scorpio.vfxtoolset.Editor
{
    public class SimpleEffectShaderGUI : EffectShaderGUIBase
    {
        protected override void AppendModuleData()
        {
            var blendModeModule  = new ModuleData("【混合模式设置】", new[]{new PropertyInfo("_DstBlend", "混合模式")});
            var mainModule       = new ModuleData("【主贴图设置】", EffectModuleConfigs.k_MainModulePropConfig);
            // var customDataModule = new ModuleData("【CustomData设置】", EffectModuleConfigs.k_SpecialModulePropConfig);
            var visibilityModule = new ModuleData("【测试设置】", EffectModuleConfigs.k_TestConfigModulePropConfig);
            var stencilModule    = new ModuleData("【Stencil设置】", EffectModuleConfigs.k_StencilModulePropConfig);
            
            s_moduleDataMap.Add(blendModeModule.moduleName, blendModeModule);
            // _moduleDataMap.Add(customDataModule.moduleName, customDataModule);
            s_moduleDataMap.Add(mainModule.moduleName, mainModule);
            s_moduleDataMap.Add(visibilityModule.moduleName, visibilityModule);
            s_moduleDataMap.Add(stencilModule.moduleName, stencilModule);
        }
        
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
            // if (material != null && material.shader.name.Contains("MeshEffect"))
            // {
                // Debug.Log($"{material.shader.name}");
                // if (_moduleDataList.ContainsKey("【CustomData设置】"))
                // {
                //     _moduleDataMap.Remove("【CustomData设置】");
                // }
            // }
        }
    }
}