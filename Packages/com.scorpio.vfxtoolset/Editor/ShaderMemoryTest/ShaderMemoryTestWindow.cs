using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScorpioEditor.ShaderMemoryTest
{
    public class ShaderMemoryTestWindow : EditorWindow
    {
        [SerializeField] private List<ShaderMemoryTestEntry> _shaderEntries;
        [SerializeField] private int _maxLocalFeatureCount = 7;
        [SerializeField] private string _testScenePath = "Assets/ShaderMemoryTest/ShaderMemoryTestScene.unity";
        [SerializeField] private string _svcOutputBasePath = "Assets/ShaderMemoryTest/SVC";
        [SerializeField] private int _maxMaterialCount = 0;
        [SerializeField] private bool _useRandomSample = false;
        [SerializeField] private int _randomSeed = 42;

        private SerializedObject _so;
        private Vector2 _scroll;
        private string _lastError;
        private string _lastMessage;

        [MenuItem("Window/Scorpio VFX/Shader Memory Test (ParticleEffect Modular)")]
        public static void Open()
        {
            var w = GetWindow<ShaderMemoryTestWindow>(false, "Shader Memory Test", true);
            w.minSize = new Vector2(400, 400);
        }

        private void OnEnable()
        {
            _so = new SerializedObject(this);
            if (_shaderEntries == null || _shaderEntries.Count == 0)
            {
                _shaderEntries = new List<ShaderMemoryTestEntry>
                {
                    new ShaderMemoryTestEntry()
                };
            }
        }

        private IEnumerator RunFullPipelineRoutine()
        {
            var perShaderMaterials = new List<(ShaderMemoryTestEntry entry, List<Material> materials)>();

            // Step 1: 生成材质
            for (int e = 0; e < _shaderEntries.Count; e++)
            {
                var entry = _shaderEntries[e];
                var config = GetConfigForEntry(entry);
                var state = new MaterialGenerationState();

                var matRoutine = ShaderMemoryTestRunner.GenerateMaterialsCoroutine(config, state, (cur, tot) =>
                {
                    return EditorUtility.DisplayCancelableProgressBar(
                        "Step 1/4 — 生成材质",
                        $"[{e + 1}/{_shaderEntries.Count}] {entry.GetShortNameForScene()} {cur}/{tot}",
                        (float)cur / Math.Max(1, tot));
                });

                while (matRoutine.MoveNext())
                    yield return null;

                if (!string.IsNullOrEmpty(state.Error))
                {
                    _lastError = state.Error;
                    EditorUtility.ClearProgressBar();
                    yield break;
                }

                perShaderMaterials.Add((entry, state.Materials));
            }

            EditorUtility.ClearProgressBar();

            // Step 2: 生成场景
            string sceneError = null;
            var sceneRoutine = ShaderMemoryTestSceneGenerator.GenerateOrUpdateSceneMultiShaderCoroutine(
                _testScenePath,
                perShaderMaterials,
                (cur, tot, sIdx, sCount) =>
                {
                    return EditorUtility.DisplayCancelableProgressBar(
                        "Step 2/4 — 生成场景",
                        $"创建 Quad {cur}/{tot} (Shader {sIdx}/{sCount})",
                        (float)cur / Math.Max(1, tot));
                },
                err => sceneError = err);

            while (sceneRoutine.MoveNext())
                yield return null;

            EditorUtility.ClearProgressBar();

            if (!string.IsNullOrEmpty(sceneError))
            {
                _lastError = sceneError;
                yield break;
            }

            // Step 3: 生成 SVC
            int totalVariants = 0;
            for (int e = 0; e < _shaderEntries.Count; e++)
            {
                var entry = _shaderEntries[e];
                EditorUtility.DisplayProgressBar(
                    "Step 3/4 — 生成 SVC",
                    $"[{e + 1}/{_shaderEntries.Count}] {entry.GetShortNameForScene()}",
                    (float)(e + 1) / _shaderEntries.Count);

                var config = GetConfigForEntry(entry);
                var svc = ShaderMemoryTestSvcGenerator.GenerateSvc(config, out string svcErr);
                if (svcErr != null)
                {
                    _lastError = svcErr;
                    EditorUtility.ClearProgressBar();
                    yield break;
                }
                if (svc != null) totalVariants += svc.variantCount;
                yield return null;
            }

            EditorUtility.ClearProgressBar();

            // Step 4: 加入 Preloaded Shaders
            int addedSvc = 0;
            for (int e = 0; e < _shaderEntries.Count; e++)
            {
                var entry = _shaderEntries[e];
                EditorUtility.DisplayProgressBar(
                    "Step 4/4 — 加入 Preloaded Shaders",
                    $"[{e + 1}/{_shaderEntries.Count}] {entry.GetShortNameForScene()}",
                    (float)(e + 1) / _shaderEntries.Count);

                var path = $"{_svcOutputBasePath.TrimEnd('/')}/{entry.GetShortNameForScene()}.shadervariants";
                if (ShaderMemoryTestPreloadedShaders.AddSvcToPreloadedShadersByPath(path, out string preloadErr))
                    addedSvc++;
                else if (preloadErr != null)
                {
                    _lastError = preloadErr;
                    EditorUtility.ClearProgressBar();
                    yield break;
                }
                yield return null;
            }

            EditorUtility.ClearProgressBar();

            int totalMats = 0;
            foreach (var (_, mats) in perShaderMaterials)
                totalMats += mats.Count;
            _lastMessage = $"全部完成：{perShaderMaterials.Count} 个 Shader，{totalMats} 个材质，{totalVariants} 个变体，{addedSvc} 个 SVC 已加入 Preloaded Shaders，场景已保存到 {_testScenePath}。";
        }

        private ShaderMemoryTestConfig GetConfigForEntry(ShaderMemoryTestEntry entry)
        {
            var c = CreateInstance<ShaderMemoryTestConfig>();
            c.ShaderName = entry.ShaderName;
            c.MaxLocalFeatureCount = _maxLocalFeatureCount;
            c.MaterialsOutputDir = entry.GetEffectiveMaterialsOutputDir();
            c.TestScenePath = _testScenePath;
            c.SvcOutputPath = $"{_svcOutputBasePath.TrimEnd('/')}/{entry.GetShortNameForScene()}.shadervariants";
            c.MaxMaterialCount = _maxMaterialCount;
            c.UseRandomSample = _useRandomSample;
            c.RandomSeed = _randomSeed;
            return c;
        }

        private void OnGUI()
        {
            if (_so == null)
                OnEnable();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Theseus ParticleEffect Modular — Android 内存测试", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "按配置枚举合法关键字组合，生成材质资产并（可选）生成测试场景，便于打 Android 包后用 Memory Profiler 查看 Shader/材质内存。\n支持多个 Shader，每个 Shader 单独文件夹与场景子节点。",
                MessageType.Info);
            EditorGUILayout.Space(4);

            EditorGUILayout.LabelField("Shader 列表", EditorStyles.boldLabel);
            for (int i = 0; i < _shaderEntries.Count; i++)
            {
                var entry = _shaderEntries[i];
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                entry.ShaderName = EditorGUILayout.TextField("Shader Name", entry.ShaderName);
                entry.MaterialsOutputDir = EditorGUILayout.TextField("Materials Output Dir（留空用默认）", entry.MaterialsOutputDir);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("删除", GUILayout.Width(50)) && _shaderEntries.Count > 1)
                {
                    _shaderEntries.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            if (GUILayout.Button("添加 Shader"))
                _shaderEntries.Add(new ShaderMemoryTestEntry());

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("全局配置", EditorStyles.boldLabel);
            _so.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField(_so.FindProperty("_maxLocalFeatureCount"), new GUIContent("Max Local Feature Count"));
            EditorGUILayout.PropertyField(_so.FindProperty("_testScenePath"), new GUIContent("Test Scene Path"));
            EditorGUILayout.PropertyField(_so.FindProperty("_svcOutputBasePath"), new GUIContent("SVC Output Base Path"));
            EditorGUILayout.PropertyField(_so.FindProperty("_maxMaterialCount"), new GUIContent("Max Material Count"));
            EditorGUILayout.PropertyField(_so.FindProperty("_useRandomSample"), new GUIContent("Use Random Sample"));
            if (_useRandomSample)
                EditorGUILayout.PropertyField(_so.FindProperty("_randomSeed"), new GUIContent("Random Seed"));
            _so.ApplyModifiedPropertiesWithoutUndo();

            EditorGUILayout.Space(8);

            if (!string.IsNullOrEmpty(_lastError))
                EditorGUILayout.HelpBox(_lastError, MessageType.Error);
            if (!string.IsNullOrEmpty(_lastMessage))
                EditorGUILayout.HelpBox(_lastMessage, MessageType.None);

            EditorGUILayout.Space(4);

            bool hasEntries = _shaderEntries != null && _shaderEntries.Count > 0;
            bool coroutineRunning = ShaderMemoryTestCoroutineUtility.IsRunning;
            EditorGUI.BeginDisabledGroup(!hasEntries || coroutineRunning);

            if (GUILayout.Button("一键生成（材质 → 场景 → SVC → Preloaded Shaders）", GUILayout.Height(36)))
            {
                _lastError = null;
                _lastMessage = null;
                ShaderMemoryTestCoroutineUtility.Start(RunFullPipelineRoutine());
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField("说明与流程", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "• 生成材质：根据 MaxLocalFeatureCount 等枚举合法组合，为每种组合创建材质并序列化为 .mat 资产。\n" +
                "• 生成场景：在场景中为每个材质创建一个 Quad 并引用该材质，保存场景以便打包时包含。\n" +
                "• 生成 SVC：创建 Shader Variant Collection，显式包含所有变体，避免构建时被 strip。\n" +
                "• 加入 Preloaded Shaders：将 SVC 注册到 Graphics Settings，确保变体打入 build。\n" +
                "• 打包：Build Settings → Android，将本测试场景加入 Scenes In Build，打 APK。\n" +
                "• 分析：在设备上运行，用 Memory Profiler 抓取快照，查看该 Shader 与相关材质的 GPU/内存占用。",
                MessageType.None);

            EditorGUILayout.EndScrollView();
        }
    }

    /// <summary>
    /// 轻量 Editor 协程驱动，用 EditorApplication.update 驱动 IEnumerator，
    /// 避免大量创建材质/物体时 Editor 无响应。
    /// 命名避免与 com.unity.editorcoroutines 的 EditorCoroutineUtility 冲突。
    /// </summary>
    internal static class ShaderMemoryTestCoroutineUtility
    {
        private static IEnumerator _current;
        private static bool _running;

        public static void Start(IEnumerator routine)
        {
            Stop();
            _current = routine;
            _running = true;
            EditorApplication.update += OnUpdate;
        }

        public static void Stop()
        {
            _running = false;
            _current = null;
            EditorApplication.update -= OnUpdate;
        }

        private static void OnUpdate()
        {
            if (!_running || _current == null)
            {
                Stop();
                return;
            }
            if (!_current.MoveNext())
                Stop();
        }

        public static bool IsRunning => _running;
    }
}
