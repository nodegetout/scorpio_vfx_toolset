using System.Text.RegularExpressions;
using UnityEngine;

namespace ScorpioEditor.ShaderMemoryTest
{
    [System.Serializable]
    public class ShaderMemoryTestEntry
    {
        [Tooltip("Shader 路径，如 Hidden/Theseus/VFX/ParticleEffect_CommonSF_Modular")]
        public string ShaderName = "Hidden/Theseus/VFX/ParticleEffect_CommonSF_Modular";

        [Tooltip("材质输出目录，留空则使用默认：Assets/ShaderMemoryTest/Materials/{Shader简短名}")]
        public string MaterialsOutputDir = "";

        /// <summary>
        /// 获取有效的材质输出目录。若 MaterialsOutputDir 为空，则使用默认路径。
        /// </summary>
        public string GetEffectiveMaterialsOutputDir()
        {
            if (!string.IsNullOrWhiteSpace(MaterialsOutputDir))
                return MaterialsOutputDir.Trim();
            return GetDefaultMaterialsOutputDir(ShaderName);
        }

        /// <summary>
        /// 根据 Shader 名生成默认材质输出目录。
        /// 从路径取最后一段，替换非法字符为下划线。
        /// </summary>
        public static string GetDefaultMaterialsOutputDir(string shaderName)
        {
            if (string.IsNullOrWhiteSpace(shaderName))
                return "Assets/ShaderMemoryTest/Materials/Unknown";
            string shortName = shaderName;
            int lastSlash = shaderName.LastIndexOf('/');
            if (lastSlash >= 0 && lastSlash < shaderName.Length - 1)
                shortName = shaderName.Substring(lastSlash + 1);
            shortName = Regex.Replace(shortName, @"[^\w\-\.]", "_");
            if (string.IsNullOrEmpty(shortName))
                shortName = "Unknown";
            return $"Assets/ShaderMemoryTest/Materials/{shortName}";
        }

        /// <summary>
        /// 获取用于场景中空物体命名的简短名。
        /// </summary>
        public string GetShortNameForScene()
        {
            if (string.IsNullOrWhiteSpace(ShaderName))
                return "Unknown";
            int lastSlash = ShaderName.LastIndexOf('/');
            if (lastSlash >= 0 && lastSlash < ShaderName.Length - 1)
                return ShaderName.Substring(lastSlash + 1);
            return ShaderName;
        }
    }
}
