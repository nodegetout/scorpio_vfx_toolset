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

        public string GetEffectiveMaterialsOutputDir()
        {
            if (!string.IsNullOrWhiteSpace(MaterialsOutputDir))
                return MaterialsOutputDir.Trim();
            return GetDefaultMaterialsOutputDir(ShaderName);
        }

        public static string GetDefaultMaterialsOutputDir(string shaderName)
        {
            string shortName = GetShortNameFromShaderPath(shaderName);
            shortName = Regex.Replace(shortName, @"[^\w\-\.]", "_");
            if (string.IsNullOrEmpty(shortName))
                shortName = "Unknown";
            return $"Assets/ShaderMemoryTest/Materials/{shortName}";
        }

        public string GetShortNameForScene() => GetShortNameFromShaderPath(ShaderName);

        private static string GetShortNameFromShaderPath(string shaderName)
        {
            if (string.IsNullOrWhiteSpace(shaderName))
                return "Unknown";
            int lastSlash = shaderName.LastIndexOf('/');
            if (lastSlash >= 0 && lastSlash < shaderName.Length - 1)
                return shaderName.Substring(lastSlash + 1);
            return shaderName;
        }
    }
}
