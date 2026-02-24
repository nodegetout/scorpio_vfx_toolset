using UnityEngine;

namespace HeroShowRenderingGUI.VFX
{
    public class KeywordModuleView : ModuleViewBase    
    {
        private readonly string _keyword;
        
        public KeywordModuleView(string moduleName, string keyword, PropertyInfo[] propertyInfoArray, 
            bool foldoutFlag = false, bool moduleEnabled = false)
            : base(moduleName, propertyInfoArray, foldoutFlag, moduleEnabled)
        {
            this._keyword = keyword;
        }

        protected override void CheckModuleEnabledState(Material material)
        {
            if (!string.IsNullOrEmpty(_keyword))
            {
                moduleEnabled = material.IsKeywordEnabled(_keyword);
            }
        }
        
        protected override void OnCheckboxClicked(ref Material material)
        {
            if (moduleEnabled)
            {
                material.EnableKeyword(_keyword);
            }
            else
            {
                material.DisableKeyword(_keyword);
            }
        }
    }
}