using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HeroShowRenderingGUI.VFX
{
    public class SimpleEffectShaderGUI : EffectShaderGUIBase
    {
        private static List<ModuleViewBase> s_SimpleEffectModuleDataMap = new List<ModuleViewBase>()
        {
            new PersistentModuleView("【合并阶段设置】", PropertyInfoData.k_MergeStageModulePropInfo),
            new PersistentModuleView("【主贴图设置】", PropertyInfoData.k_MeshMainModulePropInfo),
            new PersistentModuleView("【Stencil设置】", PropertyInfoData.k_StencilModulePropInfo)
        };

        protected override void DrawModules(MaterialEditor materialEditor, Material material, MaterialProperty[] properties)
        {
            foreach (var moduleDataRecord in s_SimpleEffectModuleDataMap)
            {
                DrawModuleWithData(materialEditor, moduleDataRecord, material, properties);
            }
        }
    }
}