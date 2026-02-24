using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HeroShowRenderingGUI.VFX
{
    public static class PropertyDrawConfigs
    {
        public delegate void DrawMaterialPropertyFunc(MaterialEditor materialEditor, MaterialProperty property, string label);

        private static Dictionary<PropertyType, DrawMaterialPropertyFunc> s_drawMaterialPropertyFuncMap =
            new Dictionary<PropertyType, DrawMaterialPropertyFunc>
            {
                {
                    PropertyType.TextureScaleOffsetProperty,
                    (editor, property, label) => editor.ShaderProperty(property, label)
                },
                {
                    PropertyType.ShaderProperty,
                    (editor, property, label) => editor.ShaderProperty(property, label)
                },
                {
                    PropertyType.UVParamsProperty,
                    VectorParamsDrawerConfigs.k_UVParamsDrawer.DrawVectorParamsProperty
                }
            };

        public static void AddPropertyDrawConfig(PropertyType type, DrawMaterialPropertyFunc drawMaterialPropertyFunc)
        {
            if (!s_drawMaterialPropertyFuncMap.ContainsKey(type))
            {
                s_drawMaterialPropertyFuncMap.Add(type, drawMaterialPropertyFunc);
            }
        }

        public static DrawMaterialPropertyFunc GetPropertyDrawFunc(PropertyType type)
        {
            if (s_drawMaterialPropertyFuncMap.TryGetValue(type, out var func))
            {
                return func;
            }

            Debug.LogWarning($"{type.ToString()} doesn't have a DrawMaterialPropertyFunc any!");
            return null;
        }
    }
}