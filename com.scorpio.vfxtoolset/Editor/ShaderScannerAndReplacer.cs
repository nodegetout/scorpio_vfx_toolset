using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace com.scorpio.vfxtoolset.Editor
{
    public class ShaderScannerAndReplacer : EditorWindow
    {
        private static readonly string k_ReplaceShaderPrefix = "Scorpio/";

        public List<string> shaderNameList = new List<string>()
        {
            "Legacy Shaders/Particles/Additive", "Legacy Shaders/Particles/Additive (Soft)",
            "Legacy Shaders/Particles/Alpha Blended", "Legacy Shaders/Particles/Alpha Blended Premultiply",
            "Legacy Shaders/Diffuse", "Legacy Shaders/VertexLit", "Legacy Shaders/Transparent/Diffuse",
            "Legacy Shaders/Transparent/VertexLit", "Particles/Standard Unlit", "Sprites/Default", "Sprites/Mask",
            "Standard (Specular setup)", "Standard", "Mobile/Particles/Additive", "Mobile/Diffuse", "UI/Unlit/Text",
            "UI/Default Font", "Unlit/Transparent Cutout", "Unlit/Texture",
        };

        public string directoryPath = "Assets";
        private List<string> _result = new List<string>();
        private string _saveFilePath = "Assets/ShaderScanResults.txt";

        private EditorCoroutine _scanCoroutine;

        [MenuItem("Scorpio/VFXToolset/Shader Scanner & Replacer")]
        public static void ShowWindow()
        {
            GetWindow<ShaderScannerAndReplacer>("Shader Scanner & Replacer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Directory to Scan", EditorStyles.boldLabel);
            directoryPath = EditorGUILayout.TextField("Directory Path", directoryPath);
            GUILayout.Label("Shaders to Check", EditorStyles.boldLabel);
            for (int i = 0; i < shaderNameList.Count; i++)
            {
                shaderNameList[i] = EditorGUILayout.TextField("Shader Name " + (i + 1), shaderNameList[i]);
            }

            if (GUILayout.Button("Add Shader Name"))
            {
                shaderNameList.Add(string.Empty);
            }

            if (GUILayout.Button("Scan Materials"))
            {
                StartScan();
            }

            if (GUILayout.Button("Cancel Scan & Replacer"))
            {
                EditorUtility.ClearProgressBar();
                if (_scanCoroutine != null)
                {
                    EditorCoroutineUtility.StopCoroutine(_scanCoroutine);
                    _scanCoroutine = null;
                }
            }

            GUILayout.Label("Save Results to File", EditorStyles.boldLabel);
            _saveFilePath = EditorGUILayout.TextField("Save File Path", _saveFilePath);
            if (GUILayout.Button("Save Results"))
            {
                SaveResultsToFile();
            }
        }

        private void StartScan()
        {
            _result.Clear();
            _scanCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(ScanMaterialsStep());
        }

        private IEnumerator ScanMaterialsStep()
        {
            var materialPaths = Directory.GetFiles(directoryPath, "*.mat", SearchOption.AllDirectories);
            int curMaterialIndex = 0;
            int materialCount = materialPaths.Length;
            foreach (var materialPath in materialPaths)
            {
                Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                if (material != null)
                {
                    // 遍历 Shader 名单，检查材质使用的 Shader 是否匹配
                    foreach (var shaderName in shaderNameList)
                    {
                        if (!string.IsNullOrEmpty(shaderName))
                        {
                            Shader shader = Shader.Find(shaderName);
                            if (shader != null && material.shader == shader)
                            {
                                string newShaderName = RecomposeShaderName(shaderName);
                                Shader newShader = Shader.Find(newShaderName);
                                if (newShader != null)
                                {
                                    material.shader = newShader;
                                }

                                _result.Add($"Material: {material.name} uses Shader: {shader.name}");
                            }
                        }
                    }
                }

                yield return null;
                // 更新进度条
                curMaterialIndex++;
                float replaceProgress = (float)curMaterialIndex / materialCount;
                EditorUtility.DisplayProgressBar("Replacing Shaders",
                    $"Replacing shader {curMaterialIndex} of {materialCount}", replaceProgress);
            }
        }

        private string RecomposeShaderName(string shaderName)
        {
            return k_ReplaceShaderPrefix + shaderName;
        }

        private void SaveResultsToFile()
        {
            // 检查文件路径是否有效
            if (string.IsNullOrEmpty(_saveFilePath))
            {
                EditorUtility.DisplayDialog("Invalid Path", "Please specify a valid path to save the results.", "OK");
                return;
            }

            // 确保文件夹存在
            string directory = Path.GetDirectoryName(_saveFilePath);
            if (!Directory.Exists(directory))
            {
                if (directory != null) Directory.CreateDirectory(directory);
            }

            using (StreamWriter writer = new StreamWriter(_saveFilePath, false))
            {
                writer.WriteLine("Shader Scan Results");
                writer.WriteLine("-------------------");
                foreach (var line in _result)
                {
                    writer.WriteLine(line);
                }

                writer.WriteLine("-------------------");
                writer.WriteLine("End of Results");
            }

            EditorUtility.DisplayDialog("Success", $"Results have been saved to {_saveFilePath}", "OK");
        }
    }
}