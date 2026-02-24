using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace HeroShowRenderingGUI.VFX
{
    public abstract class EffectShaderGUIBase : ShaderGUI
    {
        bool _bInited = false;

        protected Dictionary<PropertyType, VectorParamsDrawer> _customPropertyFuncMap =
            new Dictionary<PropertyType, VectorParamsDrawer>();

        protected virtual void AppendPropertyDrawerFunc(MaterialEditor materialEditor)
        {
        }

        protected virtual void ChangeModuleByEffectType(MaterialEditor materialEditor)
        {
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (!_bInited)
            {
                Initialize(materialEditor);
                _bInited = true;
            }

            EditorGUIUtility.fieldWidth = 0;
            Material material = materialEditor.target as Material;
            Undo.RecordObject(material, "修改材质属性");

            DrawWarningGUI();
            DrawModules(materialEditor, material, properties);

            EditorGUILayout.Space();
            if (SupportedRenderingFeatures.active.editableMaterialRenderQueue)
                materialEditor.RenderQueueField();
        }

        void Initialize(MaterialEditor materialEditor)
        {
            AppendPropertyDrawerFunc(materialEditor);
            ChangeModuleByEffectType(materialEditor);
            foreach (var config in _customPropertyFuncMap)
            {
                PropertyDrawConfigs.AddPropertyDrawConfig(config.Key, config.Value.DrawVectorParamsProperty);
            }
        }

        protected virtual void DrawWarningGUI()
        {
            var warningContent = "本Shader为Theseus展台特效，目前暂未开放使用";
            EditorGUILayout.HelpBox(warningContent, MessageType.Error, true);
        }

        protected abstract void DrawModules(MaterialEditor materialEditor, Material material,
            MaterialProperty[] properties);

        protected void DrawModuleWithData(MaterialEditor materialEditor, ModuleViewBase moduleView, Material material,
            MaterialProperty[] properties)
        {
            moduleView.DrawModuleDataGUI(materialEditor, properties, ref material, FindProperty);
        }
    }
}