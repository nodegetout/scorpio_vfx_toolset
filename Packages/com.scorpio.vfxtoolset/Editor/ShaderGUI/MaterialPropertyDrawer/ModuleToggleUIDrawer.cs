using System;
using UnityEditor;
using UnityEngine;

namespace com.scorpio.vfxtoolset.Editor
{
    public class ModuleToggleUIDrawer : MaterialPropertyDrawer
    {
        public ModuleToggleUIDrawer()
        {
        }

        public ModuleToggleUIDrawer(string keyword)
        {
        }

        protected virtual void SetKeyword(MaterialProperty prop, bool on)
        {
        }

        private static bool IsPropertyTypeSuitable(MaterialProperty prop)
        {
            return prop.type == MaterialProperty.PropType.Vector;
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return !IsPropertyTypeSuitable(prop) ? 45f : base.GetPropertyHeight(prop, label, editor);
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (!IsPropertyTypeSuitable(prop))
            {
                GUIContent label1 = EditorGUIUtility.IconContent("Toggle used on a non-float property: " + prop.name);
                EditorGUI.LabelField(position, label1, EditorStyles.helpBox);
            }
            else
            {
                MaterialEditor.BeginProperty(position, prop);
                if (prop.type != MaterialProperty.PropType.Int)
                {
                    EditorGUI.BeginChangeCheck();
                    bool flag = (double)Math.Abs(prop.floatValue) > 1.0 / 1000.0;
                    EditorGUI.showMixedValue = prop.hasMixedValue;
                    bool on = EditorGUI.Toggle(position, label, flag);
                    EditorGUI.showMixedValue = false;
                    if (EditorGUI.EndChangeCheck())
                    {
                        prop.floatValue = on ? 1f : 0.0f;
                        this.SetKeyword(prop, on);
                    }
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    bool flag = prop.intValue != 0;
                    EditorGUI.showMixedValue = prop.hasMixedValue;
                    bool on = EditorGUI.Toggle(position, label, flag);
                    EditorGUI.showMixedValue = false;
                    if (EditorGUI.EndChangeCheck())
                    {
                        prop.intValue = on ? 1 : 0;
                        this.SetKeyword(prop, on);
                    }
                }

                MaterialEditor.EndProperty();
            }
        }

        public override void Apply(MaterialProperty prop)
        {
            base.Apply(prop);
            if (!IsPropertyTypeSuitable(prop) || prop.hasMixedValue)
                return;
            if (prop.type != MaterialProperty.PropType.Int)
                this.SetKeyword(prop, (double)Math.Abs(prop.floatValue) > 1.0 / 1000.0);
            else
                this.SetKeyword(prop, prop.intValue != 0);
        }
    }
}