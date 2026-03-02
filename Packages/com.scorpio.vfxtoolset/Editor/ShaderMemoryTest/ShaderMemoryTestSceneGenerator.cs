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

            string path = config.TestScenePath;
            if (string.IsNullOrWhiteSpace(path) || !path.EndsWith(".unity"))
            {
                error = "TestScenePath must be a .unity path (e.g. Assets/ShaderMemoryTest/ShaderMemoryTestScene.unity).";
                return false;
            }

            EnsureDirectoryExists(System.IO.Path.GetDirectoryName(path).Replace('\\', '/'));

            Scene scene;
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path) != null)
                scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            else
                scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            const string rootName = "ShaderMemoryTestRoot";
            Transform root = GameObject.Find(rootName)?.transform;
            if (root != null)
            {
                UnityEngine.Object.DestroyImmediate(root.gameObject);
            }

            GameObject rootGo = new GameObject(rootName);
            root = rootGo.transform;

            int count = materials.Count;
            int cols = Mathf.Max(1, (int)Mathf.Sqrt(count));
            float spacing = 1.5f;
            for (int i = 0; i < count; i++)
            {
                var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.name = $"Quad_{i}";
                quad.transform.SetParent(root, false);
                int row = i / cols;
                int col = i % cols;
                quad.transform.localPosition = new Vector3(col * spacing, row * spacing, 0f);

                var renderer = quad.GetComponent<MeshRenderer>();
                if (renderer != null && i < materials.Count)
                    renderer.sharedMaterial = materials[i];
            }

            bool saved = EditorSceneManager.SaveScene(scene, path);
            if (!saved)
                error = "Failed to save scene to " + path;
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

            if (string.IsNullOrWhiteSpace(testScenePath) || !testScenePath.EndsWith(".unity"))
            {
                error = "TestScenePath must be a .unity path.";
                return false;
            }

            EnsureDirectoryExists(System.IO.Path.GetDirectoryName(testScenePath).Replace('\\', '/'));

            Scene scene;
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(testScenePath) != null)
                scene = EditorSceneManager.OpenScene(testScenePath, OpenSceneMode.Single);
            else
                scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            const string rootName = "ShaderMemoryTestRoot";
            Transform root = GameObject.Find(rootName)?.transform;
            if (root != null)
                UnityEngine.Object.DestroyImmediate(root.gameObject);

            GameObject rootGo = new GameObject(rootName);
            root = rootGo.transform;

            foreach (var (entry, materials) in perShaderMaterials)
            {
                if (materials == null || materials.Count == 0) continue;

                string shortName = entry.GetShortNameForScene();
                GameObject shaderRoot = new GameObject(shortName);
                shaderRoot.transform.SetParent(root, false);

                int count = materials.Count;
                int cols = Mathf.Max(1, (int)Mathf.Sqrt(count));
                float spacing = 1.5f;
                for (int i = 0; i < count; i++)
                {
                    var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    quad.name = $"Quad_{i}";
                    quad.transform.SetParent(shaderRoot.transform, false);
                    int row = i / cols;
                    int col = i % cols;
                    quad.transform.localPosition = new Vector3(col * spacing, row * spacing, 0f);

                    var renderer = quad.GetComponent<MeshRenderer>();
                    if (renderer != null && i < materials.Count)
                        renderer.sharedMaterial = materials[i];
                }
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

            if (string.IsNullOrWhiteSpace(testScenePath) || !testScenePath.EndsWith(".unity"))
            {
                setError?.Invoke("TestScenePath must be a .unity path.");
                yield break;
            }

            EnsureDirectoryExists(System.IO.Path.GetDirectoryName(testScenePath).Replace('\\', '/'));

            Scene scene;
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(testScenePath) != null)
                scene = EditorSceneManager.OpenScene(testScenePath, OpenSceneMode.Single);
            else
                scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            const string rootName = "ShaderMemoryTestRoot";
            Transform root = GameObject.Find(rootName)?.transform;
            if (root != null)
                UnityEngine.Object.DestroyImmediate(root.gameObject);

            GameObject rootGo = new GameObject(rootName);
            root = rootGo.transform;

            int shaderCount = perShaderMaterials.Count;
            int totalQuads = 0;
            foreach (var (_, mats) in perShaderMaterials)
                totalQuads += mats?.Count ?? 0;

            int quadsCreated = 0;
            for (int s = 0; s < perShaderMaterials.Count; s++)
            {
                var (entry, materials) = perShaderMaterials[s];
                if (materials == null || materials.Count == 0) continue;

                string shortName = entry.GetShortNameForScene();
                GameObject shaderRoot = new GameObject(shortName);
                shaderRoot.transform.SetParent(root, false);

                int count = materials.Count;
                int cols = Mathf.Max(1, (int)Mathf.Sqrt(count));
                float spacing = 1.5f;
                for (int i = 0; i < count; i++)
                {
                    var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    quad.name = $"Quad_{i}";
                    quad.transform.SetParent(shaderRoot.transform, false);
                    int row = i / cols;
                    int col = i % cols;
                    quad.transform.localPosition = new Vector3(col * spacing, row * spacing, 0f);

                    var renderer = quad.GetComponent<MeshRenderer>();
                    if (renderer != null && i < materials.Count)
                        renderer.sharedMaterial = materials[i];

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

        private static void EnsureDirectoryExists(string dir)
        {
            if (string.IsNullOrEmpty(dir) || !dir.StartsWith("Assets/")) return;
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
