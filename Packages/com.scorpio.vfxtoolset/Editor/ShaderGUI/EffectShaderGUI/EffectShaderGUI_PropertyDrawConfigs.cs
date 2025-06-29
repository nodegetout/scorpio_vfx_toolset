using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.scorpio.vfxtoolset.Editor
{
    public static class PropertyDrawConfigs
    {
        public delegate void DrawMaterialPropertyFunc(MaterialEditor materialEditor, MaterialProperty property, string label);

        private static readonly Dictionary<PropertyType, DrawMaterialPropertyFunc> _drawMaterialPropertyFuncMap =
            new Dictionary<PropertyType, DrawMaterialPropertyFunc>()
            {
                {
                    PropertyType.TextureScaleOffsetProperty,
                    (editor, property, label) => editor.ShaderProperty(property, label)
                },
                {
                    PropertyType.ShaderProperty,
                    (editor, property, label) => editor.ShaderProperty(property, label)
                }
            };

        public static void AddPropertyDrawConfig(PropertyType type, DrawMaterialPropertyFunc drawMaterialPropertyFunc)
        {
            if (!_drawMaterialPropertyFuncMap.ContainsKey(type))
            {
                _drawMaterialPropertyFuncMap.Add(type, drawMaterialPropertyFunc);
            }
        }

        public static DrawMaterialPropertyFunc GetPropertyDrawFunc(PropertyType type)
        {
            if (_drawMaterialPropertyFuncMap.TryGetValue(type, out var func))
            {
                return func;
            }

            Debug.LogWarning($"{type.ToString()} doesn't have a DrawMaterialPropertyFunc any!");
            return null;
        }
    }
}