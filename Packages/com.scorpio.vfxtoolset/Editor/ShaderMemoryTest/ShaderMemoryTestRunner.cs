using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScorpioEditor.ShaderMemoryTest
{
    /// <summary>
    /// 材质生成协程的结果与状态，供 GenerateMaterialsCoroutine 填充。
    /// </summary>
    public class MaterialGenerationState
    {
        public string Error;
        public List<Material> Materials = new List<Material>();
    }

    public static class ShaderMemoryTestRunner
    {
        private const int MaterialsPerYield = 20;

        private struct PrepareResult
        {
            public Shader Shader;
            public List<KeywordCombination> Combinations;
            public string Dir;
        }

        /// <summary>
        /// 校验配置、查找 Shader、枚举组合、创建输出目录，供同步/协程版本共用。
        /// </summary>
        private static bool ValidateAndPrepare(ShaderMemoryTestConfig config, out PrepareResult result, out string error)
        {
            result = default;
            error = null;

            var shader = Shader.Find(config.ShaderName);
            if (shader == null)
            {
                error = $"Shader not found: {config.ShaderName}";
                return false;
            }

            string dir = config.MaterialsOutputDir;
            if (string.IsNullOrWhiteSpace(dir))
            {
                error = "MaterialsOutputDir is empty.";
                return false;
            }
            dir = dir.Trim();
            if (!dir.StartsWith("Assets/"))
            {
                error = "MaterialsOutputDir 须为本地工程 Assets 下路径（如 Assets/ShaderMemoryTest/Materials）。";
                return false;
            }
            if (!AssetDatabase.IsValidFolder("Assets"))
            {
                error = "当前项目无 Assets 目录。";
                return false;
            }

            ShaderMemoryTestPathUtils.EnsureDirectoryExists(dir);

            var combinations = KeywordCombinationGenerator.EnumerateAll(
                config.MaxLocalFeatureCount,
                config.MaxMaterialCount,
                config.UseRandomSample,
                config.RandomSeed);

            result = new PrepareResult { Shader = shader, Combinations = combinations, Dir = dir };
            return true;
        }

        private static Material CreateAndSaveMaterial(Shader shader, in KeywordCombination combo, string dir)
        {
            var mat = new Material(shader);
            MaterialKeywordApplier.Apply(mat, combo);
            string name = MaterialKeywordApplier.GetCombinationName(combo);
            string path = $"{dir}/Mat_{name}.mat";
            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        /// <summary>
        /// 协程版本：分批生成材质，每批后 yield，便于显示进度与取消。
        /// onProgress(current, total) 返回 true 表示用户取消。
        /// </summary>
        public static IEnumerator GenerateMaterialsCoroutine(
            ShaderMemoryTestConfig config,
            MaterialGenerationState state,
            Func<int, int, bool> onProgress)
        {
            state.Error = null;
            state.Materials.Clear();

            if (!ValidateAndPrepare(config, out var prep, out string err))
            {
                state.Error = err;
                yield break;
            }

            int total = prep.Combinations.Count;
            for (int i = 0; i < total; i++)
            {
                state.Materials.Add(CreateAndSaveMaterial(prep.Shader, prep.Combinations[i], prep.Dir));

                if ((i + 1) % MaterialsPerYield == 0 || i == total - 1)
                {
                    if (onProgress != null && onProgress(i + 1, total))
                    {
                        state.Error = "用户取消";
                        yield break;
                    }
                    yield return null;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static List<Material> GenerateMaterials(ShaderMemoryTestConfig config, out string error)
        {
            if (!ValidateAndPrepare(config, out var prep, out error))
                return new List<Material>();

            var materials = new List<Material>(prep.Combinations.Count);
            foreach (var combo in prep.Combinations)
                materials.Add(CreateAndSaveMaterial(prep.Shader, combo, prep.Dir));

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return materials;
        }
    }
}
