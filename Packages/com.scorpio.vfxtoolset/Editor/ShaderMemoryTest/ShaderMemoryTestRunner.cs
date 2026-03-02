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

            var shader = Shader.Find(config.ShaderName);
            if (shader == null)
            {
                state.Error = $"Shader not found: {config.ShaderName}";
                yield break;
            }

            var combinations = KeywordCombinationGenerator.EnumerateAll(
                config.MaxLocalFeatureCount,
                config.MaxMaterialCount,
                config.UseRandomSample,
                config.RandomSeed);

            string dir = config.MaterialsOutputDir;
            if (string.IsNullOrWhiteSpace(dir))
            {
                state.Error = "MaterialsOutputDir is empty.";
                yield break;
            }
            dir = dir.Trim();
            if (!dir.StartsWith("Assets/"))
            {
                state.Error = "MaterialsOutputDir 须为本地工程 Assets 下路径（如 Assets/ShaderMemoryTest/Materials）。";
                yield break;
            }
            if (!AssetDatabase.IsValidFolder("Assets"))
            {
                state.Error = "当前项目无 Assets 目录。";
                yield break;
            }

            EnsureDirectoryExists(dir);

            int total = combinations.Count;
            for (int i = 0; i < total; i++)
            {
                var combo = combinations[i];
                var mat = new Material(shader);
                MaterialKeywordApplier.Apply(mat, combo);
                string name = MaterialKeywordApplier.GetCombinationName(combo);
                string path = $"{dir}/Mat_{name}.mat";
                AssetDatabase.CreateAsset(mat, path);
                state.Materials.Add(mat);

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
            error = null;
            var shader = Shader.Find(config.ShaderName);
            if (shader == null)
            {
                error = $"Shader not found: {config.ShaderName}";
                return new List<Material>();
            }

            var combinations = KeywordCombinationGenerator.EnumerateAll(
                config.MaxLocalFeatureCount,
                config.MaxMaterialCount,
                config.UseRandomSample,
                config.RandomSeed);

            string dir = config.MaterialsOutputDir;
            if (string.IsNullOrWhiteSpace(dir))
            {
                error = "MaterialsOutputDir is empty.";
                return new List<Material>();
            }
            dir = dir.Trim();
            if (!dir.StartsWith("Assets/"))
            {
                error = "MaterialsOutputDir 须为本地工程 Assets 下路径（如 Assets/ShaderMemoryTest/Materials）。";
                return new List<Material>();
            }
            if (!AssetDatabase.IsValidFolder("Assets"))
            {
                error = "当前项目无 Assets 目录。";
                return new List<Material>();
            }

            // 结果写入本地工程（当前 Unity 项目）的 Assets，不写入包内
            EnsureDirectoryExists(dir);

            var materials = new List<Material>(combinations.Count);
            for (int i = 0; i < combinations.Count; i++)
            {
                var combo = combinations[i];
                var mat = new Material(shader);
                MaterialKeywordApplier.Apply(mat, combo);
                string name = MaterialKeywordApplier.GetCombinationName(combo);
                string path = $"{dir}/Mat_{name}.mat";
                AssetDatabase.CreateAsset(mat, path);
                materials.Add(mat);
            }

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return materials;
        }

        private static void EnsureDirectoryExists(string dir)
        {
            string[] parts = dir.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
