using System;
using UnityEditor;

namespace com.scorpio.vfxtoolset.Editor
{
    public class ModuleData
    {
        public bool moduleFlag = false;
        public string moduleName = String.Empty;
        public PropertyInfo[] propertyInfoArray;

        public ModuleData(string moduleName, PropertyInfo[] propertyInfoArray)
        {
            this.moduleName        = moduleName;
            this.propertyInfoArray = propertyInfoArray;
        }
    }

    public struct PropertyInfo
    {
        public string propertyName;
        public PropertyType propertyType;
        public MaterialProperty materialProperty;
        public string propertyLabel;

        public PropertyInfo(string propertyName, string propertyLabel, PropertyType propertyType = PropertyType.ShaderProperty)
        {
            this.propertyName  = propertyName;
            this.propertyType  = propertyType;
            this.propertyLabel = propertyLabel;
            materialProperty = null;
        }
    }
}