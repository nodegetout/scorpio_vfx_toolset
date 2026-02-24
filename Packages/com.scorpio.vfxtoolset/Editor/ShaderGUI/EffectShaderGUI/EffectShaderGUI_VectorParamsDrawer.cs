using UnityEditor;
using UnityEngine;

namespace HeroShowRenderingGUI.VFX
{
    public class VectorParamsDrawer
    {
        VectorComponent[] _vectorComponents;
        
        public VectorParamsDrawer(VectorComponent c1, VectorComponent c2, VectorComponent c3, VectorComponent c4)
        {
            _vectorComponents = new[] { c1, c2, c3, c4 };
        }
        
        public void DrawVectorParamsProperty(MaterialEditor materialEditor, MaterialProperty property, string label)
        {
            if (property == null || property.type != MaterialProperty.PropType.Vector)
            {
                return;
            }

            for (int i = 0; i < _vectorComponents.Length; i++)
            {
                if (_vectorComponents[i] != null && !string.IsNullOrEmpty(_vectorComponents[i]?.label))
                {
                    _vectorComponents[i].DrawComponent(property.vectorValue[i]);
                }
            }
            property.vectorValue = new Vector4(
                _vectorComponents[0].value, _vectorComponents[1].value, 
                _vectorComponents[2].value, _vectorComponents[3].value
            );
        }
    }
}