using UnityEditor;
using UnityEngine;

namespace ScorpioEditor
{
    /// <summary>
    /// 通用模块化 ShaderGUI 基类。
    ///
    /// ── Shader 编写约定 ──────────────────────────────────────────────
    ///
    /// 父模块开始（附在 [HideInInspector] Float 属性，属性名以 _ModuleBegin_ 开头）：
    ///   模块标题写在属性的 displayName 中，支持中文及任意字符。
    ///   [HideInInspector][ModuleBegin]                 _ModuleBegin_Xxx ("标题", Float) = 0
    ///   [HideInInspector][ModuleBegin(_KEYWORD_ON)]    _ModuleBegin_Xxx ("标题", Float) = 0
    ///   [HideInInspector][ModuleBegin(_PropName, prop)] _ModuleBegin_Xxx ("标题", Float) = 0
    ///
    /// 子模块开始（属性名以 _SubModuleBegin_ 开头，用法同上）：
    ///   [HideInInspector][SubModuleBegin(_KEYWORD_ON)] _SubModuleBegin_Xxx ("标题", Float) = 0
    ///
    /// 模块结束（直接附在最后一个 body 属性上，无需额外占位属性）：
    ///   [ModuleEnd]           → 结束父模块
    ///   [ModuleEnd(sub)]      → 仅结束子模块
    ///   [ModuleEnd(sub, end)] → 结束子模块同时结束父模块
    ///
    /// 规则：第一个模块之前和最后一个模块之后可以有裸属性（header/footer），
    ///       模块之间不允许出现裸属性，否则 Console 会输出警告。
    /// </summary>
    public class ScorpioModuleShaderGUIBase : ShaderGUI
    {
        // ── 属性名前缀约定（用于识别 Begin 标记属性）─────────────────
        private const string PrefixModuleBegin    = "_ModuleBegin_";
        private const string PrefixSubModuleBegin = "_SubModuleBegin_";

        // ── EditorPrefs key 前缀 ──────────────────────────────────────
        private const string EditorPrefsPrefix = "ScorpioModuleGUI_";

        // ─────────────────────────────────────────────────────────────
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Material material = materialEditor.target as Material;
            Undo.RecordObject(material, "修改材质属性");

            // 第一遍：触发所有 Begin 属性的 GetPropertyHeight，
            // 让 Drawer 构造函数注册信息写入 DrawerInfoRegistry。
            // （Unity 在 ShaderProperty 内部会调用 Drawer.GetPropertyHeight）
            foreach (var p in properties)
            {
                if (p.name.StartsWith(PrefixModuleBegin) || p.name.StartsWith(PrefixSubModuleBegin))
                    materialEditor.GetPropertyHeight(p);
            }

            var frameData = CollectModules(properties, material);
            DrawGUI(materialEditor, material, frameData);
        }

        // ── 解析阶段 ──────────────────────────────────────────────────
        private static ModuleShaderGUIFrameData CollectModules(
            MaterialProperty[] properties, Material material)
        {
            var data             = new ModuleShaderGUIFrameData();
            bool modulesStarted  = false;
            bool modulesFinished = false;
            ModuleEntry currentParent = null;
            ModuleEntry currentChild  = null;

            foreach (var prop in properties)
            {
                string name = prop.name;

                // ── Begin 标记属性（按名称前缀识别，height=0 不绘制）──
                if (name.StartsWith(PrefixModuleBegin) || name.StartsWith(PrefixSubModuleBegin))
                {
                    // 尝试从注册表读取 Drawer 参数
                    if (!DrawerInfoRegistry.TryGet(name, out var info))
                    {
                        Debug.LogWarning(
                            $"[ScorpioModuleShaderGUI] 属性 '{name}' 缺少 ModuleBegin/SubModuleBegin Drawer，" +
                            $"请检查 Shader: {material.shader.name}");
                        continue;
                    }

                    if (info.Level == ModuleLevel.Parent)
                    {
                        modulesStarted  = true;
                        modulesFinished = false;
                        currentParent   = new ModuleEntry
                        {
                            Level        = ModuleLevel.Parent,
                            Title        = info.Title,
                            ToggleType   = info.ToggleType,
                            ToggleTarget = info.ToggleTarget
                        };
                        currentChild = null;
                        data.Modules.Add(currentParent);
                    }
                    else // Child
                    {
                        if (currentParent == null)
                        {
                            Debug.LogWarning(
                                $"[ScorpioModuleShaderGUI] SubModuleBegin '{name}' 没有对应的父模块，" +
                                $"请检查 Shader: {material.shader.name}");
                            continue;
                        }
                        currentChild   = new ModuleEntry
                        {
                            Level        = ModuleLevel.Child,
                            Title        = info.Title,
                            ToggleType   = info.ToggleType,
                            ToggleTarget = info.ToggleTarget
                        };
                        data.Modules.Add(currentChild);
                    }
                    continue;
                }

                // ── 普通属性（可能携带 [ModuleEnd] Drawer）────────────
                if (!modulesStarted)
                {
                    data.HeaderProperties.Add(prop);
                }
                else if (modulesFinished)
                {
                    data.FooterProperties.Add(prop);
                }
                else
                {
                    // 状态机保证：modulesStarted=true 且 modulesFinished=false 时，currentParent 非空
                    Debug.Assert(currentParent != null,
                        $"[ScorpioModuleShaderGUI] 内部状态异常：属性 '{name}' 在模块内但 currentParent 为空。");

                    // 加入当前模块 body
                    if (currentChild != null)
                        currentChild.BodyProperties.Add(prop);
                    else
                        currentParent.BodyProperties.Add(prop);

                    // 检查是否带有 ModuleEnd Drawer
                    if (DrawerInfoRegistry.TryGet(name, out var endInfo) && endInfo.IsEnd)
                    {
                        switch (endInfo.EndScope)
                        {
                            case ModuleEndScope.Child:
                                if (currentChild == null)
                                    Debug.LogWarning(
                                        $"[ScorpioModuleShaderGUI] [ModuleEnd(sub)] 在 '{name}' 上但没有当前子模块。" +
                                        $"Shader: {material.shader.name}");
                                currentChild = null;
                                break;

                            case ModuleEndScope.ChildAndParent:
                                if (currentChild == null)
                                    Debug.LogWarning(
                                        $"[ScorpioModuleShaderGUI] [ModuleEnd(sub,end)] 在 '{name}' 上但没有当前子模块。" +
                                        $"Shader: {material.shader.name}");
                                currentChild    = null;
                                currentParent   = null;
                                modulesFinished = true;
                                break;

                            default: // ModuleEndScope.Parent
                                currentChild    = null;
                                currentParent   = null;
                                modulesFinished = true;
                                break;
                        }
                    }
                }
            }

            return data;
        }

        // ── 绘制阶段 ──────────────────────────────────────────────────
        private void DrawGUI(MaterialEditor materialEditor, Material material, ModuleShaderGUIFrameData data)
        {
            foreach (var prop in data.HeaderProperties)
                materialEditor.ShaderProperty(prop, prop.displayName);

            int count = data.Modules.Count;
            for (int i = 0; i < count; i++)
            {
                var module = data.Modules[i];

                if (module.Level == ModuleLevel.Child)
                {
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

            // Property 模式且 ToggleTarget 为空：自动取 body 第一个属性作为开关属性
            string toggleTarget = module.ToggleTarget;
            if (module.ToggleType == ModuleToggleType.Property
                && string.IsNullOrEmpty(toggleTarget)
                && module.BodyProperties.Count > 0)
                toggleTarget = module.BodyProperties[0].name;

            bool isEnabled = GetToggleState(material, module.ToggleType, toggleTarget);

            bool newExpanded = DrawModuleHeader(module.Title, isChild, isExpanded, isEnabled, hasToggle,
                out bool newEnabled);

            if (newExpanded != isExpanded)
                SetFoldoutState(material, module.Title, newExpanded);

            if (newEnabled != isEnabled && hasToggle)
                SetToggleState(material, module.ToggleType, toggleTarget, newEnabled);

            if (newExpanded && module.BodyProperties.Count > 0)
            {
                EditorGUI.indentLevel += isChild ? 2 : 1;
                foreach (var prop in module.BodyProperties)
                {
                    // 跳过 property 开关属性本身（由 Header Toggle 控制）
                    if (module.ToggleType == ModuleToggleType.Property &&
                        prop.name == toggleTarget)
                        continue;

                    materialEditor.ShaderProperty(prop, prop.displayName);
                }
                EditorGUI.indentLevel -= isChild ? 2 : 1;
            }
        }

        // ── 绘制模块标题栏（ShurikenModuleTitle 风格）────────────────
        private static bool DrawModuleHeader(string title, bool isChild,
            bool isExpanded, bool isEnabled, bool hasToggle, out bool newEnabled)
        {
            newEnabled = isEnabled;

            var headerStyle = new GUIStyle("ShurikenModuleTitle")
            {
                font          = new GUIStyle(EditorStyles.boldLabel).font,
                border        = new RectOffset(15, 7, 4, 4),
                fixedHeight   = isChild ? 20f : 22f,
                contentOffset = new Vector2(hasToggle ? 36f : 20f, -2f)
            };

            float leftPad    = isChild ? 16f : 0f;
            var   fullRect   = GUILayoutUtility.GetRect(16f, headerStyle.fixedHeight, headerStyle,
                                   GUILayout.ExpandWidth(true));
            var   headerRect = new Rect(fullRect.x + leftPad, fullRect.y,
                                   fullRect.width - leftPad, fullRect.height);

            GUI.Box(headerRect, title, headerStyle);

            var e = Event.current;

            // Foldout 箭头
            var foldRect = new Rect(headerRect.x + 4f,
                headerRect.y + (headerRect.height - 13f) * 0.5f, 13f, 13f);
            if (e.type == EventType.Repaint)
                EditorStyles.foldout.Draw(foldRect, false, false, isExpanded, false);

            // 模块开关 Toggle
            Rect toggleRect = default;
            if (hasToggle)
            {
                toggleRect = new Rect(headerRect.x + 20f,
                    headerRect.y + (headerRect.height - 14f) * 0.5f, 14f, 14f);
                EditorGUI.BeginChangeCheck();
                newEnabled = GUI.Toggle(toggleRect, isEnabled, GUIContent.none,
                    new GUIStyle("ShurikenToggle"));
                EditorGUI.EndChangeCheck();
            }

            // 点击标题栏翻转 Foldout
            if (e.type == EventType.MouseDown && headerRect.Contains(e.mousePosition)
                && (!hasToggle || !toggleRect.Contains(e.mousePosition)))
            {
                isExpanded = !isExpanded;
                e.Use();
            }

            return isExpanded;
        }

        // ── EditorPrefs 折叠状态 ──────────────────────────────────────
        private static string FoldoutKey(Material m, string t)
            => $"{EditorPrefsPrefix}{m.GetInstanceID()}_{t}";

        private static bool GetFoldoutState(Material m, string t)
            => EditorPrefs.GetBool(FoldoutKey(m, t), false);

        private static void SetFoldoutState(Material m, string t, bool v)
            => EditorPrefs.SetBool(FoldoutKey(m, t), v);

        // ── Toggle 状态读写 ───────────────────────────────────────────
        private static bool GetToggleState(Material material, ModuleToggleType toggleType, string toggleTarget)
        {
            switch (toggleType)
            {
                case ModuleToggleType.Keyword:
                    return material.IsKeywordEnabled(toggleTarget);

                case ModuleToggleType.Property:
                    if (string.IsNullOrEmpty(toggleTarget)) return false;
                    var prop = FindProperty(toggleTarget,
                        MaterialEditor.GetMaterialProperties(new Object[] { material }), false);
                    return prop != null && prop.floatValue > 0.5f;

                default:
                    return true;
            }
        }

        private static void SetToggleState(Material material,
            ModuleToggleType toggleType, string toggleTarget, bool value)
        {
            switch (toggleType)
            {
                case ModuleToggleType.Keyword:
                    if (value) material.EnableKeyword(toggleTarget);
                    else       material.DisableKeyword(toggleTarget);
                    break;

                case ModuleToggleType.Property:
                    if (string.IsNullOrEmpty(toggleTarget)) break;
                    var prop = FindProperty(toggleTarget,
                        MaterialEditor.GetMaterialProperties(new Object[] { material }), false);
                    if (prop != null)
                        prop.floatValue = value ? 1f : 0f;
                    break;
            }

            EditorUtility.SetDirty(material);
        }
    }
}

