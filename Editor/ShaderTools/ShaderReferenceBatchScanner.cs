using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Scorpio.VFXToolset.Editor.Data;
using ShaderInfo = Scorpio.VFXToolset.Editor.Data.ShaderInfo;

namespace Scorpio.VFXToolset.Editor
{
    public class ShaderReferenceScanner : EditorWindow
    {
        // Separate scroll positions for used materials and unused shaders
        private Vector2 scrollUsed;
        private Vector2 scrollUnused;
        private List<MaterialInfo> foundMaterials = new List<MaterialInfo>();
        private List<ShaderInfo> unusedShaders = new List<ShaderInfo>();
        // Foldout states for each found material record
        private List<bool> materialFoldouts = new List<bool>();
        private string searchPath = "Assets/Shaders/ShaderTheseus/Effect/FromOldEffectShader";
        private bool showOnlyUsed = true;
        private bool scanInProgress = false;

        // UI layout constants
        private const int maxDisplayCount = 5;
        private const float itemHeight = 60f; // approximate height per record
        private readonly float scrollViewHeight = maxDisplayCount * itemHeight;

        [MenuItem("ScorpioVFXToolset/Batch Scanner/Shader Reference")]
        public static void ShowWindow()
        {
            GetWindow<ShaderReferenceScanner>("Shader Reference Scanner");
        }

        private void OnGUI()
        {
            GUILayout.Label("Shader Reference Scanner", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // 搜索路径设置
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("搜索路径:", GUILayout.Width(80));
            searchPath = EditorGUILayout.TextField(searchPath);
            if (GUILayout.Button("浏览", GUILayout.Width(60)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("选择Shader目录", "Assets", "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // 转换为相对路径
                    if (selectedPath.StartsWith(Application.dataPath))
                    {
                        searchPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            // 控制选项
            EditorGUILayout.BeginHorizontal();
            showOnlyUsed = EditorGUILayout.Toggle("只显示被引用的材质", showOnlyUsed);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            // 扫描按钮
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = !scanInProgress;
            if (GUILayout.Button("开始扫描", GUILayout.Height(30)))
            {
                ScanForShaderReferences();
            }
            GUI.enabled = true;
            
            if (scanInProgress)
            {
                GUILayout.Label("扫描中...", EditorStyles.helpBox);
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            // 保存按钮
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = foundMaterials.Count > 0 && !scanInProgress;
            if (GUILayout.Button("保存结果到TXT", GUILayout.Height(25)))
            {
                SaveResultsToTxt();
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            // 结果显示
            if (foundMaterials.Count > 0 || unusedShaders.Count > 0)
            {
                GUILayout.Label($"找到 {foundMaterials.Count} 个材质球引用，{unusedShaders.Count} 个未使用的Shader:", EditorStyles.boldLabel);

                // --- 被引用的材质球（独立ScrollView，带折叠） ---
                GUILayout.Label("被引用的材质球:", EditorStyles.boldLabel);
                scrollUsed = EditorGUILayout.BeginScrollView(scrollUsed, GUILayout.Height(scrollViewHeight));

                // Ensure foldout list size
                if (materialFoldouts == null) materialFoldouts = new List<bool>();

                for (int i = 0; i < foundMaterials.Count; i++)
                {
                    var materialInfo = foundMaterials[i];
                    if (showOnlyUsed && !materialInfo.isUsed) continue;

                    // Ensure index exists
                    while (materialFoldouts.Count <= i) materialFoldouts.Add(false);

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    // Foldout header with object field and locate button
                    EditorGUILayout.BeginHorizontal();
                    materialFoldouts[i] = EditorGUILayout.Foldout(materialFoldouts[i], materialInfo.material != null ? materialInfo.material.name : materialInfo.shaderName);
                    EditorGUILayout.ObjectField(materialInfo.material, typeof(Material), false, GUILayout.Width(200));
                    if (GUILayout.Button("定位", GUILayout.Width(50)))
                    {
                        if (materialInfo.material != null)
                        {
                            EditorGUIUtility.PingObject(materialInfo.material);
                            Selection.activeObject = materialInfo.material;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    // Expanded details
                    if (materialFoldouts[i])
                    {
                        EditorGUILayout.LabelField("材质路径:", materialInfo.materialPath);
                        EditorGUILayout.LabelField("Shader名称:", materialInfo.shaderName);
                        EditorGUILayout.LabelField("Shader路径:", materialInfo.shaderPath);
                    }

                    EditorGUILayout.EndVertical();
                    GUILayout.Space(5);
                }

                EditorGUILayout.EndScrollView();

                GUILayout.Space(8);

                // --- 未使用的Shader（独立ScrollView） ---
                GUILayout.Label("未使用的Shader:", EditorStyles.boldLabel);
                scrollUnused = EditorGUILayout.BeginScrollView(scrollUnused, GUILayout.Height(scrollViewHeight));

                for (int i = 0; i < unusedShaders.Count; i++)
                {
                    var shaderInfo = unusedShaders[i];
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(shaderInfo.shader, typeof(Shader), false, GUILayout.Width(200));
                    if (GUILayout.Button("定位", GUILayout.Width(50)))
                    {
                        if (shaderInfo.shader != null)
                        {
                            EditorGUIUtility.PingObject(shaderInfo.shader);
                            Selection.activeObject = shaderInfo.shader;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("Shader名称:", shaderInfo.shaderName);
                    EditorGUILayout.LabelField("Shader路径:", shaderInfo.shaderPath);
                    EditorGUILayout.LabelField("状态:", "未使用", EditorStyles.miniLabel);

                    EditorGUILayout.EndVertical();
                    GUILayout.Space(5);
                }

                EditorGUILayout.EndScrollView();
            }
            else if (!scanInProgress)
            {
                EditorGUILayout.HelpBox("点击'开始扫描'来查找引用指定目录下Shader的材质球", MessageType.Info);
            }
        }

        private void ScanForShaderReferences()
        {
            scanInProgress = true;
            foundMaterials.Clear();
            unusedShaders.Clear();
            if (materialFoldouts != null) materialFoldouts.Clear();
            
            try
            {
                // 获取指定目录下的所有Shader文件
                string[] shaderGuids = AssetDatabase.FindAssets("t:Shader", new[] { searchPath });
                HashSet<string> shaderPaths = new HashSet<string>();
                Dictionary<string, ShaderInfo> allShaders = new Dictionary<string, ShaderInfo>();
                
                foreach (string guid in shaderGuids)
                {
                    string shaderPath = AssetDatabase.GUIDToAssetPath(guid);
                    Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);
                    if (shader != null)
                    {
                        shaderPaths.Add(shaderPath);
                        allShaders[shaderPath] = new ShaderInfo(shader, shader.name, shaderPath);
                    }
                }
                
                Debug.Log($"在 {searchPath} 目录下找到 {shaderPaths.Count} 个Shader文件");
                
                // 获取所有材质球
                string[] materialGuids = AssetDatabase.FindAssets("t:Material");
                int processedCount = 0;
                
                foreach (string guid in materialGuids)
                {
                    string materialPath = AssetDatabase.GUIDToAssetPath(guid);
                    Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                    
                    if (material != null && material.shader != null)
                    {
                        string shaderPath = AssetDatabase.GetAssetPath(material.shader);
                        
                        // 检查是否引用了目标目录下的Shader
                        if (shaderPaths.Contains(shaderPath))
                        {
                            MaterialInfo materialInfo = new MaterialInfo(material, material.shader.name, shaderPath);
                            // mark as used on the material record where appropriate
                            materialInfo.isUsed = true;
                            foundMaterials.Add(materialInfo);
                            
                            // 标记Shader为已使用
                            if (allShaders.ContainsKey(shaderPath))
                            {
                                allShaders[shaderPath].isUsed = true;
                            }
                        }
                    }
                    
                    processedCount++;
                    if (processedCount % 100 == 0)
                    {
                        EditorUtility.DisplayProgressBar("扫描进度", $"正在扫描材质球... {processedCount}/{materialGuids.Length}", 
                            (float)processedCount / materialGuids.Length);
                    }
                }
                
                // 收集未使用的Shader
                foreach (var kvp in allShaders)
                {
                    if (!kvp.Value.isUsed)
                    {
                        unusedShaders.Add(kvp.Value);
                    }
                }
                
                EditorUtility.ClearProgressBar();
                Debug.Log($"扫描完成! 找到 {foundMaterials.Count} 个材质球引用了目标目录下的Shader，{unusedShaders.Count} 个Shader未被使用");
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError($"扫描过程中出现错误: {e.Message}");
            }
            finally
            {
                scanInProgress = false;
            }
        }

        private void SaveResultsToTxt()
        {
            try
            {
                // 选择保存路径
                string fileName = $"ShaderReferenceScan_{System.DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string savePath = EditorUtility.SaveFilePanel("保存扫描结果", "", fileName, "txt");
                 
                 if (string.IsNullOrEmpty(savePath))
                     return;

                 // 创建报告内容
                 System.Text.StringBuilder report = new System.Text.StringBuilder();
                 report.AppendLine("==========================================");
                 report.AppendLine("Shader Reference Scanner Report");
                 report.AppendLine("==========================================");
                 report.AppendLine($"扫描时间: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                 report.AppendLine($"搜索路径: {searchPath}");
                 report.AppendLine($"找到材质球数量: {foundMaterials.Count}");
                 report.AppendLine($"未使用Shader数量: {unusedShaders.Count}");
                 report.AppendLine("==========================================");
                 report.AppendLine();

                // 按Shader分组统计
                Dictionary<string, List<MaterialInfo>> shaderGroups = new Dictionary<string, List<MaterialInfo>>();
                foreach (var materialInfo in foundMaterials)
                {
                    if (!shaderGroups.ContainsKey(materialInfo.shaderName))
                        shaderGroups[materialInfo.shaderName] = new List<MaterialInfo>();
                    shaderGroups[materialInfo.shaderName].Add(materialInfo);
                }

                // 写入统计信息
                report.AppendLine("Shader使用统计:");
                report.AppendLine("------------------------------------------");
                foreach (var kvp in shaderGroups)
                {
                    report.AppendLine($"{kvp.Key}: {kvp.Value.Count} 个材质球");
                }
                report.AppendLine();

                // 写入未使用Shader统计
                if (unusedShaders.Count > 0)
                {
                    report.AppendLine("未使用的Shader:");
                    report.AppendLine("------------------------------------------");
                    foreach (var shaderInfo in unusedShaders)
                    {
                        report.AppendLine($"{shaderInfo.shaderName}");
                        report.AppendLine($"  Shader路径: {shaderInfo.shaderPath}");
                        report.AppendLine();
                    }
                    report.AppendLine();
                }

                // 写入详细信息
                report.AppendLine("详细信息:");
                report.AppendLine("------------------------------------------");
                
                int index = 1;
                foreach (var kvp in shaderGroups)
                {
                    report.AppendLine($"\n【{kvp.Key}】");
                    report.AppendLine($"Shader路径: {kvp.Value[0].shaderPath}");
                    report.AppendLine($"引用材质球数量: {kvp.Value.Count}");
                    report.AppendLine();
                    
                    foreach (var materialInfo in kvp.Value)
                    {
                        report.AppendLine($"{index}. {materialInfo.material.name}");
                        report.AppendLine($"   材质路径: {materialInfo.materialPath}");
                        report.AppendLine();
                        index++;
                    }
                }

                // 写入文件
                File.WriteAllText(savePath, report.ToString(), System.Text.Encoding.UTF8);
                
                // 显示成功消息
                EditorUtility.DisplayDialog("保存成功", 
                    $"扫描结果已保存到:\n{savePath}\n\n共保存 {foundMaterials.Count} 个材质球的引用信息和 {unusedShaders.Count} 个未使用的Shader", "确定");
                
                Debug.Log($"扫描结果已保存到: {savePath}");
                
                // 在资源管理器中显示文件
                EditorUtility.RevealInFinder(savePath);
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("保存失败", $"保存文件时出现错误:\n{e.Message}", "确定");
                Debug.LogError($"保存文件时出现错误: {e.Message}");
            }
        }

        private void OnDestroy()
        {
            EditorUtility.ClearProgressBar();
        }
    }
}
