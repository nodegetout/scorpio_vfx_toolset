using UnityEditor;

namespace HeroShowRenderingGUI.VFX
{
    public struct PropertyInfo
    {
        public string propertyName;
        public PropertyType propertyType;
        public string propertyLabel;

        public PropertyInfo(string propertyName, string propertyLabel, PropertyType propertyType = PropertyType.ShaderProperty)
        {
            this.propertyName  = propertyName;
            this.propertyType  = propertyType;
            this.propertyLabel = propertyLabel;
        }
    }
}