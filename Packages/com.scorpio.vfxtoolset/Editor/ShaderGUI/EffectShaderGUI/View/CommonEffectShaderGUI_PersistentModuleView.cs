using UnityEditor;
using UnityEngine;

namespace HeroShowRenderingGUI.VFX
{
    public class PersistentModuleView : ModuleViewBase
    {
        public PersistentModuleView(string moduleName, PropertyInfo[] propertyInfoArray, bool foldoutFlag = false, 
            bool moduleEnabled = true)
            : base(moduleName, propertyInfoArray, foldoutFlag, moduleEnabled)
        {
        }
        
        protected override void DrawModuleFoldout(ref Material material)
        {
            foldoutFlag = true;
            DrawModuleTitle(20f);
        }
    }
}