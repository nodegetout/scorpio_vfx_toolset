using UnityEngine;

namespace Scorpio.VFXToolset.Editor.Data
{
    [System.Serializable]
    public class ShaderInfo
    {
        public Shader shader;
        public string shaderName;
        public string shaderPath;
        public bool isUsed;
            
        public ShaderInfo(Shader shader, string name, string path)
        {
            this.shader = shader;
            shaderName = name;
            shaderPath = path;
            isUsed = false; // 默认认为未被使用
        }
    }
}