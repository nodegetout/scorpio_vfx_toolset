using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.scorpio.vfxtoolset.Editor
{
    public class SimpleEffectShaderGUI : EffectShaderGUIBase
    {
        private static Dictionary<string, ModuleData> s_SimpleEffectModuleDataMap = new Dictionary<string, ModuleData>()
        {
            {"【混合模式设置】", new ModuleData("【混合模式设置】", new[] { new PropertyInfo("_DstBlend", "混合模式") })},
            {"【主贴图设置】", new ModuleData("【主贴图设置】", EffectModuleConfigs.k_MainModulePropConfig)},
            {"【测试设置】", new ModuleData("【测试设置】", EffectModuleConfigs.k_TestConfigModulePropConfig)},
            {"【Stencil设置】", new ModuleData("【Stencil设置】", EffectModuleConfigs.k_StencilModulePropConfig)}
        };
        
        protected override void AppendModuleData()
        {
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
            if (!_customPropertyFuncMap.ContainsKey(PropertyType.UVParamsProperty))
            {
                _customPropertyFuncMap.Add(PropertyType.UVParamsProperty, DrawUVParamsProperty);
            }
        }
        
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Material material = materialEditor.target as Material;
            RemoveCustomDataModuleForMeshEffect(material);
            base.OnGUI(materialEditor, properties);
        }

        protected override void DrawModules(MaterialEditor materialEditor, Material material, MaterialProperty[] properties)
        {
            foreach (var moduleData in s_SimpleEffectModuleDataMap)
            {
                DrawModuleWithData(materialEditor,  moduleData.Value, material, properties);
            }
        }

        private void RemoveCustomDataModuleForMeshEffect(Material material)
        {
        }
    }
}