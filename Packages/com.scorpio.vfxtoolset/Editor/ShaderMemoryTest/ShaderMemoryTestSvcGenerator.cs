using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ScorpioEditor.ShaderMemoryTest
{
    public static class ShaderMemoryTestSvcGenerator
    {
        /// <summary>
        /// 生成 Shader Variant Collection 资产，包含所有合法关键字组合对应的变体。
        /// </summary>
        public static ShaderVariantCollection GenerateSvc(ShaderMemoryTestConfig config, out string error)
        {
            error = null;
            var shader = Shader.Find(config.ShaderName);
            if (shader == null)
            {
                error = $"Shader not found: {config.ShaderName}";
                return null;
            }

            var combinations = KeywordCombinationGenerator.EnumerateAll(
                config.MaxLocalFeatureCount,
                config.MaxMaterialCount,
                config.UseRandomSample,
                config.RandomSeed);

            var svc = new ShaderVariantCollection();
            PassType[] passTypesToTry = { PassType.Normal, PassType.ScriptableRenderPipelineDefaultUnlit, PassType.ForwardBase };
            int addedCount = 0;

            foreach (var combo in combinations)
            {
                string[] keywords = MaterialKeywordApplier.GetEnabledKeywords(combo);
                foreach (var passType in passTypesToTry)
                {
                    try
                    {
                        var variant = new ShaderVariantCollection.ShaderVariant(shader, passType, keywords);
                        if (svc.Add(variant))
                            addedCount++;
                        break;
                    }
                    catch (System.ArgumentException)
                    {
                        continue;
                    }
                }
            }

            if (addedCount == 0)
            {
                addedCount = TryAddFromMaterials(config, shader, svc, passTypesToTry);
                if (addedCount == 0 && error == null)
                    error = "未能添加任何变体。请先执行「1. 生成材质资产」，或检查 shader 的 Pass/LightMode 与 PassType 是否匹配。";
            }

            if (string.IsNullOrWhiteSpace(config.SvcOutputPath))
            {
                error = "SvcOutputPath 未配置。";
                return svc;
            }

            string path = config.SvcOutputPath.Trim();
            if (!path.StartsWith("Assets/"))
            {
                error = "SvcOutputPath 须为本地工程 Assets 下路径。";
                return svc;
            }

            EnsureDirectoryExists(System.IO.Path.GetDirectoryName(path).Replace('\\', '/'));
            AssetDatabase.CreateAsset(svc, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return svc;
        }

        /// <summary>
        /// 从已生成的材质收集变体（材质中的 shaderKeywords 与运行时一致，可避免 PassType 不匹配）。
        /// </summary>
        private static int TryAddFromMaterials(ShaderMemoryTestConfig config, Shader shader, ShaderVariantCollection svc, PassType[] passTypesToTry)
        {
            var materials = ShaderMemoryTestSceneGenerator.LoadMaterialsFromOutputDir(config.MaterialsOutputDir);
            if (materials == null || materials.Count == 0)
                return 0;

            int added = 0;
            foreach (var mat in materials)
            {
                if (mat == null || mat.shader != shader) continue;
                string[] keywords = mat.shaderKeywords;
                foreach (var passType in passTypesToTry)
                {
                    try
                    {
                        var variant = new ShaderVariantCollection.ShaderVariant(shader, passType, keywords);
                        if (svc.Add(variant))
                            added++;
                        break;
                    }
                    catch (System.ArgumentException)
                    {
                        continue;
                    }
                }
            }
            return added;
        }

        private static void EnsureDirectoryExists(string dir) =>
            ShaderMemoryTestPathUtils.EnsureDirectoryExists(dir);
    }
}
