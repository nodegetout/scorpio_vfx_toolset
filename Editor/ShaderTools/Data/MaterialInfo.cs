using UnityEditor;
using UnityEngine;

namespace Scorpio.VFXToolset.Editor.Data
{
    [System.Serializable]
    public class MaterialInfo
    {
        public Material material;
        public string shaderName;
        public string shaderPath;
        public string materialPath;
        public bool isUsed;
            
        public MaterialInfo(Material mat, string shader, string path)
        {
            material = mat;
            shaderName = shader;
            shaderPath = path;
            materialPath = AssetDatabase.GetAssetPath(mat);
            isUsed = true; // 默认认为被引用就是被使用
        }
    }
}