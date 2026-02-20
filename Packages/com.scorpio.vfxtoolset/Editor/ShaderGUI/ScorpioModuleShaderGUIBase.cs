using UnityEditor;
using UnityEngine;

namespace ScorpioEditor
{
    /// <summary>
    /// 通用模块化 ShaderGUI 基类。
    ///
    /// Shader 编写约定
    /// ───────────────────────────────────────────────────────
    ///  父模块开始：  [HideInInspector][ModuleBegin] _ModuleBegin_Xxx ("Title|None|", Float) = 0
    ///               [HideInInspector][ModuleBegin] _ModuleBegin_Xxx ("Title|Keyword|_KEYWORD_ON", Float) = 0
    ///               [HideInInspector][ModuleBegin] _ModuleBegin_Xxx ("Title|Property|_PropName", Float) = 0
    ///
    ///  子模块开始：  [HideInInspector][SubModuleBegin] _SubModuleBegin_Xxx ("Title|None|", Float) = 0
    ///               同上支持 Keyword / Property 开关
    ///
    ///  子模块结束：  [HideInInspector][SubModuleEnd]   _SubModuleEnd_Xxx  ("0", Float) = 0   ← 仅结束子模块
    ///               [HideInInspector][SubModuleEnd]   _SubModuleEnd_Xxx  ("1", Float) = 0   ← 同时结束父模块
    ///
    ///  父模块结束：  [HideInInspector][ModuleEnd]      _ModuleEnd_Xxx     ("", Float) = 0
    ///
    /// displayName 格式（Begin 系列）：   "Title|ToggleType|ToggleTarget"
    ///   ToggleType  = None / Keyword / Property
    ///   ToggleTarget = keyword 名称 或 property 名称（无开关时留空）
    ///
    /// displayName 格式（SubModuleEnd）："0" 或 "1"（1 = 同时结束父模块）
    ///
    /// 规则：第一个模块之前和最后一个模块之后可以有裸属性（header/footer），
    ///       模块之间不允许出现裸属性，否则 Console 会输出警告。
    /// </summary>
    public class ScorpioModuleShaderGUIBase : ShaderGUI
    {
        // ── 属性名前缀约定 ────────────────────────────────────────────
        private const string PrefixModuleBegin    = "_ModuleBegin_";
        private const string PrefixSubModuleBegin = "_SubModuleBegin_";
        private const string PrefixModuleEnd      = "_ModuleEnd_";
        private const string PrefixSubModuleEnd   = "_SubModuleEnd_";

        // ── EditorPrefs key 前缀 ──────────────────────────────────────
        private const string EditorPrefsPrefix = "ScorpioModuleGUI_";

        // ─────────────────────────────────────────────────────────────
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Material material = materialEditor.target as Material;
            Undo.RecordObject(material, "修改材质属性");

            var frameData = CollectModules(properties, material);
            DrawGUI(materialEditor, material, frameData);
        }

        // ── 解析阶段 ──────────────────────────────────────────────────
        private ModuleShaderGUIFrameData CollectModules(MaterialProperty[] properties, Material material)
        {
            var data             = new ModuleShaderGUIFrameData();
            bool modulesStarted  = false;   // 是否已遇到第一个 Begin
            bool modulesFinished = false;   // 是否所有模块已结束
            ModuleEntry currentParent = null;
            ModuleEntry currentChild  = null;

            foreach (var prop in properties)
            {
                string name = prop.name;

                // ── 父模块 Begin ──────────────────────────────────────
                if (name.StartsWith(PrefixModuleBegin))
                {
                    modulesStarted  = true;
                    modulesFinished = false;
                    currentParent   = ParseBeginEntry(prop, ModuleLevel.Parent);
                    currentChild    = null;
                    data.Modules.Add(currentParent);
                    continue;
                }

                // ── 子模块 Begin ──────────────────────────────────────
                if (name.StartsWith(PrefixSubModuleBegin))
                {
                    if (currentParent == null)
                    {
                        Debug.LogWarning($"[ScorpioModuleShaderGUI] SubModuleBegin '{name}' 没有对应的父模块，请检查 Shader。");
                        continue;
                    }
                    currentChild = ParseBeginEntry(prop, ModuleLevel.Child);
                    data.Modules.Add(currentChild);
                    continue;
                }

                // ── 子模块 End ────────────────────────────────────────
                if (name.StartsWith(PrefixSubModuleEnd))
                {
                    if (currentChild == null)
                    {
                        Debug.LogWarning($"[ScorpioModuleShaderGUI] SubModuleEnd '{name}' 没有对应的子模块，请检查 Shader。");
                        continue;
                    }
                    // displayName "1" = 同时结束父模块
                    bool alsoEndParent = prop.displayName.Trim() == "1";
                    currentChild.AlsoEndParent = alsoEndParent;
                    currentChild = null;
                    if (alsoEndParent)
                    {
                        currentParent   = null;
                        modulesFinished = true;
                    }
                    continue;
                }

                // ── 父模块 End ────────────────────────────────────────
                if (name.StartsWith(PrefixModuleEnd))
                {
                    if (currentParent == null)
                    {
                        Debug.LogWarning($"[ScorpioModuleShaderGUI] ModuleEnd '{name}' 没有对应的父模块，请检查 Shader。");
                        continue;
                    }
                    currentParent   = null;
                    currentChild    = null;
                    modulesFinished = true;
                    continue;
                }

                // ── 普通属性 ──────────────────────────────────────────
                if (!modulesStarted)
                {
                    // 模块前的 header 属性
                    data.HeaderProperties.Add(prop);
                }
                else if (modulesFinished)
                {
                    // 模块后的 footer 属性
                    data.FooterProperties.Add(prop);
                }
                else if (currentChild != null)
                {
                    currentChild.BodyProperties.Add(prop);
                }
                else if (currentParent != null)
                {
                    currentParent.BodyProperties.Add(prop);
                }
                else
                {
                    // 夹在两个模块之间的裸属性 —— 警告
                    Debug.LogWarning($"[ScorpioModuleShaderGUI] 属性 '{name}' 位于两个模块之间，将被跳过。" +
                                     $" 请将其移至所有模块之前或之后，或包裹进某个模块。Shader: {material.shader.name}");
                }
            }

            return data;
        }

        // ── 解析 Begin 属性的 displayName 为 ModuleEntry ──────────────
        // displayName 格式：  "Title|ToggleType|ToggleTarget"
        // 示例：  "Base Color|Keyword|_BASE_ON"
        //         "Dissolve|Property|_DissolveOn"
        //         "Fresnel|None|"
        private static ModuleEntry ParseBeginEntry(MaterialProperty prop, ModuleLevel level)
        {
            var entry  = new ModuleEntry { Level = level };
            var parts  = prop.displayName.Split('|');

            entry.Title        = parts.Length > 0 ? parts[0].Trim() : prop.name;
            string toggleTypeStr = parts.Length > 1 ? parts[1].Trim() : "None";
            entry.ToggleTarget = parts.Length > 2 ? parts[2].Trim() : string.Empty;

            switch (toggleTypeStr.ToLowerInvariant())
            {
                case "keyword":  entry.ToggleType = ModuleToggleType.Keyword;  break;
                case "property": entry.ToggleType = ModuleToggleType.Property; break;
                default:         entry.ToggleType = ModuleToggleType.None;     break;
            }

            return entry;
        }

        // ── 绘制阶段 ──────────────────────────────────────────────────
        private void DrawGUI(MaterialEditor materialEditor, Material material, ModuleShaderGUIFrameData data)
        {
            // Header 属性（模块前）
            foreach (var prop in data.HeaderProperties)
                materialEditor.ShaderProperty(prop, prop.displayName);

            // 模块列表
            int count = data.Modules.Count;
            for (int i = 0; i < count; i++)
            {
                var module = data.Modules[i];

                // 子模块在父模块 Foldout 展开时才显示，判断其前一个父模块是否展开
                if (module.Level == ModuleLevel.Child)
                {
                    // 找到属于当前子模块的父模块（向前搜索最近的 Parent）
                    bool parentExpanded = true;
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (data.Modules[j].Level == ModuleLevel.Parent)
                        {
                            parentExpanded = GetFoldoutState(material, data.Modules[j].Title);
                            break;
                        }
                    }
                    if (!parentExpanded) continue;
                }

                DrawModule(materialEditor, material, module);
            }

            // Footer 属性（模块后）
            if (data.FooterProperties.Count > 0)
            {
                EditorGUILayout.Space(4f);
                foreach (var prop in data.FooterProperties)
                    materialEditor.ShaderProperty(prop, prop.displayName);
            }
        }

        // ── 绘制单个模块 ──────────────────────────────────────────────
        private void DrawModule(MaterialEditor materialEditor, Material material, ModuleEntry module)
        {
            EditorGUILayout.Space(2f);

            bool isChild    = module.Level == ModuleLevel.Child;
            bool isExpanded = GetFoldoutState(material, module.Title);
            bool hasToggle  = module.ToggleType != ModuleToggleType.None;
            bool isEnabled  = GetToggleState(material, module);

            bool newExpanded = DrawModuleHeader(module.Title, isChild, isExpanded, isEnabled, hasToggle,
                out bool newEnabled);

            // 持久化状态
            if (newExpanded != isExpanded)
                SetFoldoutState(material, module.Title, newExpanded);

            if (newEnabled != isEnabled && hasToggle)
                SetToggleState(material, module, newEnabled);

            // 绘制内容
            if (newExpanded && module.BodyProperties.Count > 0)
            {
                EditorGUI.indentLevel += isChild ? 2 : 1;
                foreach (var prop in module.BodyProperties)
                {
                    // 跳过开关属性本身（它由 Header Toggle 控制）
                    if (module.ToggleType == ModuleToggleType.Property &&
                        prop.name == module.ToggleTarget)
                        continue;

                    materialEditor.ShaderProperty(prop, prop.displayName);
                }
                EditorGUI.indentLevel -= isChild ? 2 : 1;
            }
        }

        // ── 绘制模块标题栏（含 Toggle + Foldout + 标题文字）────────────
        // 参考 ParticleSystemRenderer / ShurikenModuleTitle 风格
        private static bool DrawModuleHeader(string title, bool isChild,
            bool isExpanded, bool isEnabled, bool hasToggle, out bool newEnabled)
        {
            newEnabled = isEnabled;

            // ── 样式 ─────────────────────────────────────────────────
            var headerStyle = new GUIStyle("ShurikenModuleTitle")
            {
                font          = new GUIStyle(EditorStyles.boldLabel).font,
                border        = new RectOffset(15, 7, 4, 4),
                fixedHeight   = isChild ? 20f : 22f,
                // contentOffset: leave room for foldout arrow (and toggle if present)
                contentOffset = new Vector2(hasToggle ? 36f : 20f, -2f)
            };

            float leftPad = isChild ? 16f : 0f;

            // ── 整体 Rect ────────────────────────────────────────────
            var fullRect = GUILayoutUtility.GetRect(16f, headerStyle.fixedHeight, headerStyle,
                GUILayout.ExpandWidth(true));

            // 子模块向右缩进
            var headerRect = new Rect(fullRect.x + leftPad, fullRect.y,
                fullRect.width - leftPad, fullRect.height);

            GUI.Box(headerRect, title, headerStyle);

            var e = Event.current;

            // ── Foldout 箭头（左侧）────────────────────────────────
            var foldRect = new Rect(headerRect.x + 4f, headerRect.y + (headerRect.height - 13f) * 0.5f, 13f, 13f);
            if (e.type == EventType.Repaint)
                EditorStyles.foldout.Draw(foldRect, false, false, isExpanded, false);

            // ── 模块开关 Toggle（仅在有开关时显示）──────────────────
            Rect toggleRect = default;
            if (hasToggle)
            {
                toggleRect = new Rect(headerRect.x + 20f, headerRect.y + (headerRect.height - 14f) * 0.5f, 14f, 14f);
                EditorGUI.BeginChangeCheck();
                newEnabled = GUI.Toggle(toggleRect, isEnabled, GUIContent.none, new GUIStyle("ShurikenToggle"));
                EditorGUI.EndChangeCheck();
            }

            // ── 点击标题栏翻转 Foldout（避开 toggle 区域）────────────
            if (e.type == EventType.MouseDown && headerRect.Contains(e.mousePosition)
                && (!hasToggle || !toggleRect.Contains(e.mousePosition)))
            {
                isExpanded = !isExpanded;
                e.Use();
            }

            return isExpanded;
        }

        // ── EditorPrefs 折叠状态 ──────────────────────────────────────
        private static string FoldoutKey(Material material, string moduleTitle)
            => $"{EditorPrefsPrefix}{material.GetInstanceID()}_{moduleTitle}";

        private static bool GetFoldoutState(Material material, string title)
            => EditorPrefs.GetBool(FoldoutKey(material, title), false);

        private static void SetFoldoutState(Material material, string title, bool value)
            => EditorPrefs.SetBool(FoldoutKey(material, title), value);

        // ── Toggle 状态读写 ───────────────────────────────────────────
        private static bool GetToggleState(Material material, ModuleEntry module)
        {
            switch (module.ToggleType)
            {
                case ModuleToggleType.Keyword:
                    return material.IsKeywordEnabled(module.ToggleTarget);

                case ModuleToggleType.Property:
                    var prop = FindProperty(module.ToggleTarget,
                        MaterialEditor.GetMaterialProperties(new Object[] { material }), false);
                    return prop != null && prop.floatValue > 0.5f;

                default:
                    return true; // 无开关默认启用
            }
        }

        private static void SetToggleState(Material material,
            ModuleEntry module, bool value)
        {
            switch (module.ToggleType)
            {
                case ModuleToggleType.Keyword:
                    if (value) material.EnableKeyword(module.ToggleTarget);
                    else       material.DisableKeyword(module.ToggleTarget);
                    break;

                case ModuleToggleType.Property:
                    var prop = FindProperty(module.ToggleTarget,
                        MaterialEditor.GetMaterialProperties(new Object[] { material }), false);
                    if (prop != null)
                        prop.floatValue = value ? 1f : 0f;
                    break;

                case ModuleToggleType.None:
                    // 无开关，无需写入
                    break;
            }

            EditorUtility.SetDirty(material);
        }
    }
}

