using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.scorpio.vfxtoolset.Editor
{
    public abstract class EffectShaderGUIBase : ShaderGUI
    {
        protected static Dictionary<string, ModuleData> s_moduleDataMap = new Dictionary<string, ModuleData>();
        
        bool bInited = false;
        protected Dictionary<PropertyType, PropertyDrawConfigs.DrawMaterialPropertyFunc> _customPropertyFuncMap=new Dictionary<PropertyType, PropertyDrawConfigs.DrawMaterialPropertyFunc>();
        
        protected abstract void AppendModuleData();
        protected abstract void AppendPropertyDrawerFunc();

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (!bInited)
            {
                Initialize();
                bInited = true;
            }
            
            EditorGUIUtility.fieldWidth = 0;
            Material material = materialEditor.target as Material;
            Undo.RecordObject(material, "修改材质属性");

            DrawModules(materialEditor, material, properties);
        }
        void Initialize()
        {
            AppendModuleData();
            AppendPropertyDrawerFunc();
            foreach (var config in _customPropertyFuncMap)
            {
                PropertyDrawConfigs.AddPropertyDrawConfig(config.Key, config.Value);
            }
        }
        
        void DrawModules(MaterialEditor materialEditor, Material material, MaterialProperty[] properties)
        {
            foreach (var moduleData in s_moduleDataMap)
            {
                DrawModuleWithData(materialEditor,  moduleData.Value, material, properties);
            }
        }
        
        void DrawModuleWithData(MaterialEditor materialEditor, ModuleData moduleData, Material material, MaterialProperty[] properties)
        {
            EditorGUILayout.Space();
            moduleData.moduleFlag = Foldout(moduleData.moduleFlag, moduleData.moduleName);
            if(moduleData.moduleFlag)
            {
                EditorGUI.indentLevel++;
                int length = moduleData.propertyInfoArray.Length;
                for (int i = 0; i < length; i++)
                {
                    if (material.HasProperty(moduleData.propertyInfoArray[i].propertyName))
                    {
                        var materialProp = moduleData.propertyInfoArray[i].materialProperty;
                        materialProp = FindProperty(moduleData.propertyInfoArray[i].propertyName, properties);
                        if (materialProp != null)
                        {
                            var drawFunc = PropertyDrawConfigs.GetPropertyDrawFunc(moduleData.propertyInfoArray[i].propertyType);
                            drawFunc.Invoke(materialEditor, materialProp, moduleData.propertyInfoArray[i].propertyLabel);
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        static bool Foldout(bool display, string title)
        {
            var style = new GUIStyle("ShurikenModuleTitle");
            style.font = new GUIStyle(EditorStyles.boldLabel).font;
            style.border = new RectOffset(15, 7, 4, 4);
            style.fixedHeight = 22;
            style.contentOffset = new Vector2(20f, -2f);
            
            var rect = GUILayoutUtility.GetRect(16f, 22f, style);
            GUI.Box(rect, title, style);
            
            var e = Event.current;
            var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if (e.type == EventType.Repaint)
            {
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
            }

            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                display = !display;
                e.Use();
            }

            return display;
        }
    }
}