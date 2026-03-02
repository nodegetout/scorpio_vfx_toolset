using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ScorpioEditor.ShaderMemoryTest
{
    public static class ShaderMemoryTestSceneGenerator
    {
        private const int QuadsPerYield = 50;
        private const float QuadSpacing = 1.5f;
        private const string RootName = "ShaderMemoryTestRoot";

        public static List<Material> LoadMaterialsFromOutputDir(string materialsOutputDir)
        {
            var list = new List<Material>();
            if (string.IsNullOrWhiteSpace(materialsOutputDir)) return list;
            string[] guids = AssetDatabase.FindAssets("t:Material", new[] { materialsOutputDir });
            foreach (string guid in guids)
            {
                string p = AssetDatabase.GUIDToAssetPath(guid);
                var mat = AssetDatabase.LoadAssetAtPath<Material>(p);
                if (mat != null) list.Add(mat);
            }
            return list;
        }

        public static bool GenerateOrUpdateScene(ShaderMemoryTestConfig config, List<Material> materials, out string error)
        {
            error = null;
            if (materials == null || materials.Count == 0)
            {
                error = "No materials to reference.";
                return false;
            }

            if (!OpenOrCreateScene(config.TestScenePath, out var scene, out var root, out error))
                return false;

            CreateQuadsUnderParent(root, materials);

            bool saved = EditorSceneManager.SaveScene(scene, config.TestScenePath);
            if (!saved)
                error = "Failed to save scene to " + config.TestScenePath;
            return saved;
        }

        /// <summary>
        /// 多 Shader 场景生成：根节点下每个 Shader 一个空物体，其下挂该 Shader 的 Quad。
        /// </summary>
        public static bool GenerateOrUpdateSceneMultiShader(
            string testScenePath,
            IReadOnlyList<(ShaderMemoryTestEntry entry, List<Material> materials)> perShaderMaterials,
            out string error)
        {
            error = null;
            if (perShaderMaterials == null || perShaderMaterials.Count == 0)
            {
                error = "No shader entries or materials.";
                return false;
            }

            if (!OpenOrCreateScene(testScenePath, out var scene, out var root, out error))
                return false;

            foreach (var (entry, materials) in perShaderMaterials)
            {
                if (materials == null || materials.Count == 0) continue;

                GameObject shaderRoot = new GameObject(entry.GetShortNameForScene());
                shaderRoot.transform.SetParent(root, false);
                CreateQuadsUnderParent(shaderRoot.transform, materials);
            }

            bool saved = EditorSceneManager.SaveScene(scene, testScenePath);
            if (!saved)
                error = "Failed to save scene to " + testScenePath;
            return saved;
        }

        /// <summary>
        /// 协程版本：分批创建 Quad，每批后 yield，便于显示进度与取消。
        /// onProgress(current, total, shaderIndex, shaderCount) 返回 true 表示用户取消。
        /// </summary>
        public static IEnumerator GenerateOrUpdateSceneMultiShaderCoroutine(
            string testScenePath,
            IReadOnlyList<(ShaderMemoryTestEntry entry, List<Material> materials)> perShaderMaterials,
            Func<int, int, int, int, bool> onProgress,
            Action<string> setError)
        {
            setError?.Invoke(null);
            if (perShaderMaterials == null || perShaderMaterials.Count == 0)
            {
                setError?.Invoke("No shader entries or materials.");
                yield break;
            }

            if (!OpenOrCreateScene(testScenePath, out var scene, out var root, out string openErr))
            {
                setError?.Invoke(openErr);
                yield break;
            }

            int shaderCount = perShaderMaterials.Count;
            int totalQuads = 0;
            foreach (var (_, mats) in perShaderMaterials)
                totalQuads += mats?.Count ?? 0;

            int quadsCreated = 0;
            for (int s = 0; s < perShaderMaterials.Count; s++)
            {
                var (entry, materials) = perShaderMaterials[s];
                if (materials == null || materials.Count == 0) continue;

                GameObject shaderRoot = new GameObject(entry.GetShortNameForScene());
                shaderRoot.transform.SetParent(root, false);

                int count = materials.Count;
                int cols = Mathf.Max(1, (int)Mathf.Sqrt(count));
                for (int i = 0; i < count; i++)
                {
                    CreateSingleQuad(shaderRoot.transform, materials[i], i, cols);
                    quadsCreated++;

                    if (i % QuadsPerYield == QuadsPerYield - 1 || i == count - 1)
                    {
                        if (onProgress != null && onProgress(quadsCreated, totalQuads, s + 1, shaderCount))
                        {
                            setError?.Invoke("用户取消");
                            yield break;
                        }
                        yield return null;
                    }
                }
            }

            bool saved = EditorSceneManager.SaveScene(scene, testScenePath);
            if (!saved)
                setError?.Invoke("Failed to save scene to " + testScenePath);
        }

        // ─── 私有辅助方法 ───

        /// <summary>
        /// 打开或创建场景，并准备根节点（销毁旧的、创建新的）。
        /// </summary>
        private static bool OpenOrCreateScene(string scenePath, out Scene scene, out Transform root, out string error)
        {
            scene = default;
            root = null;
            error = null;

            if (string.IsNullOrWhiteSpace(scenePath) || !scenePath.EndsWith(".unity"))
            {
                error = "TestScenePath must be a .unity path (e.g. Assets/ShaderMemoryTest/ShaderMemoryTestScene.unity).";
                return false;
            }

            ShaderMemoryTestPathUtils.EnsureDirectoryExists(
                System.IO.Path.GetDirectoryName(scenePath).Replace('\\', '/'));

            scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) != null
                ? EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single)
                : EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            Transform existing = GameObject.Find(RootName)?.transform;
            if (existing != null)
                UnityEngine.Object.DestroyImmediate(existing.gameObject);

            root = new GameObject(RootName).transform;
            return true;
        }

        /// <summary>
        /// 在 parent 下为每个材质创建一个 Quad 并网格排列。
        /// </summary>
        private static void CreateQuadsUnderParent(Transform parent, List<Material> materials)
        {
            int count = materials.Count;
            int cols = Mathf.Max(1, (int)Mathf.Sqrt(count));
            for (int i = 0; i < count; i++)
                CreateSingleQuad(parent, materials[i], i, cols);
        }

        private static void CreateSingleQuad(Transform parent, Material material, int index, int cols)
        {
            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = $"Quad_{index}";
            quad.transform.SetParent(parent, false);
            int row = index / cols;
            int col = index % cols;
            quad.transform.localPosition = new Vector3(col * QuadSpacing, row * QuadSpacing, 0f);

            var renderer = quad.GetComponent<MeshRenderer>();
            if (renderer != null)
                renderer.sharedMaterial = material;
        }
    }
}
