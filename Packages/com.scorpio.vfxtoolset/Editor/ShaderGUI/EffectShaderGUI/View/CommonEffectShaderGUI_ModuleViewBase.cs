using UnityEditor;
using UnityEngine;

namespace HeroShowRenderingGUI.VFX
{
    public class ModuleViewBase
    {
        protected bool foldoutFlag = false;
        protected bool moduleEnabled = true;
        
        private string _moduleName;
        private PropertyInfo[] _propertyInfoArray;
        private Rect _checkboxRect      = Rect.zero;
        private Rect _moduleTitleRect  = Rect.zero;
        
        public delegate MaterialProperty FindProperty(string propertyName, MaterialProperty[] properties);
        
        protected ModuleViewBase(string moduleName, PropertyInfo[] propertyInfoArray, bool foldoutFlag = false, 
            bool moduleEnabled = false)
        {
            this._propertyInfoArray = propertyInfoArray;
            this._moduleName = moduleName;
            this.moduleEnabled = moduleEnabled;
            this.foldoutFlag = foldoutFlag;
        }

        public void DrawModuleDataGUI(MaterialEditor materialEditor, MaterialProperty[] properties,ref Material material, 
            FindProperty findPropertyFunc)
        {
            EditorGUILayout.Space(); 
            DrawModuleFoldout(ref material);
            
            if (foldoutFlag)
            {
                DrawModuleProperties(materialEditor, properties, material, findPropertyFunc);
            }
        }
        
        protected virtual void DrawModuleFoldout(ref Material material)
        {
            CheckModuleEnabledState(material);
            
            DrawModuleTitle();
            
            var e = Event.current;
            if (e.type == EventType.Repaint)
            {
                DrawModuleCheckout(_moduleTitleRect);
                DrawFoldoutArrow(_moduleTitleRect);
            }

            if (e.type == EventType.MouseDown && _moduleTitleRect.Contains(e.mousePosition))
            {
                if (_checkboxRect.Contains(e.mousePosition))
                {
                    moduleEnabled = !moduleEnabled;
                    OnCheckboxClicked(ref material);
                    EditorUtility.SetDirty(material);
                    e.Use();
                }
                else if (_moduleTitleRect.Contains(e.mousePosition))
                {
                    foldoutFlag = !foldoutFlag;
                    e.Use();
                }
            }
        }
        
        private void DrawModuleProperties(MaterialEditor materialEditor, MaterialProperty[] properties, Material material, FindProperty findPropertyFunc)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space(2);
                
            EditorGUI.indentLevel++;
            int length = _propertyInfoArray.Length;
            for (int i = 0; i < length; i++)
            {
                if (material.HasProperty(_propertyInfoArray[i].propertyName))
                {
                    var materialProp = findPropertyFunc.Invoke(_propertyInfoArray[i].propertyName, properties);
                    if (materialProp != null)
                    {
                        var drawFunc = PropertyDrawConfigs.GetPropertyDrawFunc(_propertyInfoArray[i].propertyType);
                        drawFunc.Invoke(materialEditor, materialProp, _propertyInfoArray[i].propertyLabel);
                    }
                }
            }
            EditorGUI.indentLevel--;
                
            EditorGUILayout.Space(2);
            EditorGUILayout.EndVertical();
        }

        protected void DrawModuleTitle(float titleOffsetX = 36f)
        {
            var style  = CreateModuleTitleStyle(titleOffsetX);
            _moduleTitleRect = GUILayoutUtility.GetRect(16f, 22f, style);
            GUI.Box(_moduleTitleRect, _moduleName, style);
        }
        
        private GUIStyle CreateModuleTitleStyle(float titleOffsetX = 36f)
        {
            var style = new GUIStyle("ShurikenModuleTitle");

            style.font = EditorStyles.boldLabel.font;
            style.fontSize = 11;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleLeft;

            style.border = new RectOffset(15, 7, 4, 4);
            style.fixedHeight = 22f;
            style.contentOffset = new Vector2(titleOffsetX, -2f);

            style.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            style.hover.textColor = Color.white;
            style.active.textColor = Color.white;
            style.focused.textColor = Color.white;

            return style;
        }
        
        private void DrawModuleCheckout(Rect rect)
        {
            _checkboxRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            GUIStyle checkboxStyle = new GUIStyle("ShurikenToggle");
            checkboxStyle.Draw(_checkboxRect, false, false, moduleEnabled, false);
        }
        
        private void DrawFoldoutArrow(Rect rect)
        {
            var toggleRect = new Rect(rect.x + 22f, rect.y + 2f, 13f, 13f);
            
            var foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.normal.textColor = Color.white;
            foldoutStyle.onNormal.textColor = Color.white;
            foldoutStyle.focused.textColor = Color.white;
            foldoutStyle.onFocused.textColor = Color.white;
            foldoutStyle.active.textColor = Color.white;
            foldoutStyle.onActive.textColor = Color.white;
            foldoutStyle.hover.textColor = Color.white;
            foldoutStyle.onHover.textColor = Color.white;
                    
            foldoutStyle.Draw(toggleRect, false, false, foldoutFlag, false);
        }
        
        protected virtual void CheckModuleEnabledState(Material material)
        {
            // To be overridden in derived classes if needed
        }
        
        protected virtual void OnCheckboxClicked(ref Material material)
        {
            // To be overridden in derived classes if needed
        }
    }
}