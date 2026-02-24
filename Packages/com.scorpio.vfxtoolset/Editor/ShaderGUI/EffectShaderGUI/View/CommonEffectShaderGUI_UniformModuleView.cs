using UnityEditor;
using UnityEngine;

namespace HeroShowRenderingGUI.VFX
{
    public class UniformModuleView : ModuleViewBase
    {
        private readonly string _togglePropertyName;
        
        public UniformModuleView(string moduleName, string togglePropertyName, PropertyInfo[] propertyInfoArray,
            bool foldoutFlag = false, bool moduleEnabled = false)
            : base(moduleName, propertyInfoArray, foldoutFlag, moduleEnabled)
        {
            this._togglePropertyName = togglePropertyName;
        }

        protected override void CheckModuleEnabledState(Material material)
        {
            if (!string.IsNullOrEmpty(_togglePropertyName) && material.HasProperty(_togglePropertyName))
            {
                moduleEnabled = material.GetFloat(_togglePropertyName) > 0.5f;
            }
        }

        protected override void OnCheckboxClicked(ref Material material)
        {
            material.SetFloat(_togglePropertyName, moduleEnabled ? 1.0f : 0.0f);
        }
    }
}