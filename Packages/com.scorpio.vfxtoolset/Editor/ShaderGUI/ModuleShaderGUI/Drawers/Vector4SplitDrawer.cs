using System;
using UnityEditor;
using UnityEngine;

namespace ScorpioEditor
{
    /// <summary>
    /// 将 Vector4 类型的 MaterialProperty 拆分绘制的自定义 Drawer。
    ///
    /// ── Attribute 语法 ───────────────────────────────────────────────
    ///
    ///   [Vector4Split(SplitMode)]
    ///   [Vector4Split(SplitMode, labelWidth)]
    ///
    ///   SplitMode 取值（字符串，大小写不敏感）：
    ///     FourFloats   → 拆为 4 个独立 float，各自可配置绘制类型
    ///     TwoVector2   → 拆为 2 个 Vector2（xy / zw），使用 Vector2Field 渲染
    ///     Vector3Float → 拆为 1 个 Vector3（xyz）+ 1 个独立 float（w）
    ///
    ///   labelWidth（可选，float）：
    ///     > 0 时临时覆盖 EditorGUIUtility.labelWidth，仅影响 float 分量控件的标签列宽；
    ///     ≤ 0 或不传时使用系统默认值。
    ///
    /// ── displayName 格式 ─────────────────────────────────────────────
    ///
    ///   简洁标题 ## 段1配置 @ 段2配置 @ ...
    ///
    ///   • ##  分隔"Inspector 显示标题"与"分量配置区"（必须存在，缺失触发 warning）
    ///   • @   分隔各配置段
    ///   • |   分隔标签与绘制类型（DrawType 可缺省，默认 Float）
    ///
    ///   DrawType 取值：
    ///     Float              → 普通浮点输入框（默认）
    ///     Slider(min, max)   → 滑动条（支持逗号前后有空格）
    ///     Toggle             → 勾选框（0 / 1）
    ///     Hidden             → 隐藏分量，不绘制不占高度
    ///
    ///   段数约束（段数不符触发 warning）：
    ///     FourFloats   → 4 段
    ///     TwoVector2   → 2 段（| 后 DrawType 内容静默忽略）
    ///     Vector3Float → 2 段（第1段静默忽略 DrawType，第2段可配置 DrawType）
    ///
    /// ── 使用示例 ─────────────────────────────────────────────────────
    ///
    ///   [Vector4Split(FourFloats, 100)]
    ///   _UVParams ("UV参数 ## X速度|Float @ Y速度 @ 缩放|Slider(0, 10) @ 开关|Toggle", Vector) = (0,0,1,0)
    ///
    ///   [Vector4Split(TwoVector2)]
    ///   _FlowParams ("流速旋转 ## 流速XY @ 缩放旋转ZW", Vector) = (0,0,1,0)
    ///
    ///   [Vector4Split(Vector3Float)]
    ///   _OffsetParams ("偏移参数 ## 方向XYZ @ 强度|Slider(0, 5)", Vector) = (0,0,0,1)
    ///
    /// ── 三重校验（任一失败显示 warning HelpBox，不绘制分量控件）────────
    ///
    ///   1. prop.type != Vector   → 类型错误提示
    ///   2. displayName 缺少 ##   → 格式错误提示
    ///   3. 段数与 SplitMode 不符 → 段数错误提示
    ///
    /// ── 多材质编辑 ───────────────────────────────────────────────────
    ///
    ///   FourFloats：逐分量（x/y/z/w）比较所有 targets，独立设置 showMixedValue。
    ///   TwoVector2：按段（xy / zw）整体比较。
    ///   Vector3Float：xyz 整体比较，w 单独比较。
    ///   写回时遍历所有 editor.targets 同步赋值 vectorValue。
    /// </summary>
    public class Vector4SplitDrawer : MaterialPropertyDrawer
    {
        // ── 字段 ────────────────────────────────────────────────────
        private readonly SplitMode _mode;
        private readonly float     _labelWidth;
        private readonly bool      _modeValid;
        private readonly string    _modeErrorMsg;
        private readonly string[]  _rawTypes;  // null = 旧语法（## / @ 格式）

        // drawer 缓存：避免每帧 new，_cachedDisplayName 用于热重载失效检测
        private IFloatComponentDrawer[] _cachedDrawers;
        private string                  _cachedDisplayName;

        // ── 构造函数 ─────────────────────────────────────────────────

        /// <summary>[Vector4Split] — 无参，默认 FourFloats 模式。</summary>
        public Vector4SplitDrawer() : this("FourFloats", -1f, null) { }

        /// <summary>[Vector4Split(mode)] — 旧语法，无类型参数。</summary>
        public Vector4SplitDrawer(string mode) : this(mode, -1f, null) { }

        /// <summary>[Vector4Split(mode, labelWidth)] — 旧语法 + 自定义 labelWidth。</summary>
        public Vector4SplitDrawer(string mode, float labelWidth) : this(mode, labelWidth, null) { }

        /// <summary>[Vector4Split(mode, t1, t2)] — TwoVector2 / Vector3Float，2 个分量类型。</summary>
        public Vector4SplitDrawer(string mode, string t1, string t2)
            : this(mode, -1f, new[] { t1, t2 }) { }

        /// <summary>[Vector4Split(mode, t1, t2, t3, t4)] — FourFloats，4 个分量类型。</summary>
        public Vector4SplitDrawer(string mode, string t1, string t2, string t3, string t4)
            : this(mode, -1f, new[] { t1, t2, t3, t4 }) { }

        /// <summary>[Vector4Split(mode, labelWidth, t1, t2)] — 2 个分量类型 + labelWidth。</summary>
        public Vector4SplitDrawer(string mode, float labelWidth, string t1, string t2)
            : this(mode, labelWidth, new[] { t1, t2 }) { }

        /// <summary>[Vector4Split(mode, labelWidth, t1, t2, t3, t4)] — 4 个分量类型 + labelWidth。</summary>
        public Vector4SplitDrawer(string mode, float labelWidth, string t1, string t2, string t3, string t4)
            : this(mode, labelWidth, new[] { t1, t2, t3, t4 }) { }

        /// <summary>核心构造函数。</summary>
        private Vector4SplitDrawer(string mode, float labelWidth, string[] rawTypes)
        {
            _labelWidth = labelWidth;
            _rawTypes   = rawTypes;

            if (Enum.TryParse(mode, true, out SplitMode parsed))
            {
                _mode      = parsed;
                _modeValid = true;
            }
            else
            {
                _modeValid    = false;
                _modeErrorMsg = $"[Vector4Split] Unknown SplitMode \"{mode}\". "
                              + "Valid values: FourFloats, TwoVector2, Vector3Float.";
            }
        }

        // ── GetPropertyHeight ────────────────────────────────────────

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (!TryGetValidated(prop, editor, out ComponentConfig[] configs, out string errorMsg))
                return DrawerRectHelper.CalcHelpBoxHeight(errorMsg);

            switch (_mode)
            {
                case SplitMode.FourFloats:
                    // 动态累加：Hidden 分量不占高度
                    var drawers = BuildDrawers(configs);
                    float total = 0f;
                    for (int i = 0; i < drawers.Length; i++)
                    {
                        float h = drawers[i].GetHeight();
                        if (h > 0f)
                            total += h + DrawerRectHelper.LineSpacing;
                    }
                    return total;

                case SplitMode.TwoVector2:
                    // 两个 Vector2Field，每个占两行高度，中间加一个行间距
                    return DrawerRectHelper.CalcVector2FieldHeight() * 2f
                         + DrawerRectHelper.LineSpacing;

                case SplitMode.Vector3Float:
                    // 一个 Vector3Field（两行高）+ 一个 float 行（含间距）
                    return DrawerRectHelper.CalcVector3FieldHeight()
                         + DrawerRectHelper.LineHeight;

                default:
                    return DrawerRectHelper.CalcTotalHeight(2);
            }
        }

        // ── OnGUI ────────────────────────────────────────────────────

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (!TryGetValidated(prop, editor, out ComponentConfig[] configs, out string errorMsg))
            {
                EditorGUI.HelpBox(position, errorMsg, MessageType.Warning);
                return;
            }

            // 懒初始化 / 热重载失效检测：displayName 变化时重建 drawers
            if (_cachedDrawers == null || prop.displayName != _cachedDisplayName)
            {
                _cachedDrawers     = BuildDrawers(configs);
                _cachedDisplayName = prop.displayName;
            }

            switch (_mode)
            {
                case SplitMode.FourFloats:
                    DrawFourFloats(position, prop, configs, editor);
                    break;
                case SplitMode.TwoVector2:
                    DrawTwoVector2(position, prop, configs, editor);
                    break;
                case SplitMode.Vector3Float:
                    DrawVector3Float(position, prop, configs, editor);
                    break;
            }
        }

        // ── 校验 + 解析（合并，消除重复 TryParse）───────────────────

        /// <summary>
        /// 合并三重校验与配置解析。
        /// 若构造函数传入了 type 字符串，使用 <see cref="Vector4SplitPipeParser"/>（新语法）；
        /// 否则回退到 <see cref="Vector4SplitDisplayNameParser"/>（旧 <c>## / @</c> 语法）。
        /// </summary>
        private bool TryGetValidated(MaterialProperty prop, MaterialEditor editor,
                                     out ComponentConfig[] configs,
                                     out string errorMsg)
        {
            configs  = null;
            errorMsg = null;

            if (!_modeValid)
            {
                errorMsg = _modeErrorMsg;
                return false;
            }

            if (prop.type != MaterialProperty.PropType.Vector)
            {
                errorMsg = $"[Vector4Split] Used on a non-Vector property \"{prop.name}\" "
                         + $"(type: {prop.type}). This drawer only supports Vector properties.";
                return false;
            }

            if (_rawTypes != null)
                return Vector4SplitPipeParser.TryParse(
                    prop.displayName, _mode, _rawTypes,
                    out configs, out errorMsg);

            return Vector4SplitDisplayNameParser.TryParse(
                prop.displayName, _mode,
                out _, out configs, out errorMsg);
        }

        // ── Drawer 缓存构建 ──────────────────────────────────────────

        /// <summary>
        /// 根据 configs 构建 IFloatComponentDrawer 数组。
        /// 仅在懒初始化或 displayName 热重载变化时调用，避免每帧 new。
        /// </summary>
        private IFloatComponentDrawer[] BuildDrawers(ComponentConfig[] configs)
        {
            var drawers = new IFloatComponentDrawer[configs.Length];
            for (int i = 0; i < configs.Length; i++)
                drawers[i] = ComponentDrawerFactory.Create(configs[i], _labelWidth);
            return drawers;
        }

        // ── FourFloats ───────────────────────────────────────────────

        private void DrawFourFloats(
            Rect position, MaterialProperty prop,
            ComponentConfig[] configs, MaterialEditor editor)
        {
            Vector4 current = prop.vectorValue;
            string  name    = prop.name;

            System.Func<Vector4, float>[] selectors =
            {
                v => v.x, v => v.y, v => v.z, v => v.w
            };

            float[] values  = { current.x, current.y, current.z, current.w };
            float[] newVals = { current.x, current.y, current.z, current.w };
            bool changed = false;
            float yOffset = 0f;

            for (int i = 0; i < 4; i++)
            {
                // Hidden 分量：跳过绘制，保留原值
                if (configs[i] != null && configs[i].DrawType == FloatDrawType.Hidden)
                    continue;

                bool mixed = IsMixedFloat(editor, name, selectors[i]);
                string label = configs[i]?.Label ?? string.Empty;

                Rect lineRect = DrawerRectHelper.GetRectAtOffset(position, yOffset,
                    EditorGUIUtility.singleLineHeight);

                EditorGUI.BeginChangeCheck();
                float tmp = _cachedDrawers[i].Draw(lineRect, label, values[i], mixed, _labelWidth);
                if (EditorGUI.EndChangeCheck()) { newVals[i] = tmp; changed = true; }

                yOffset += DrawerRectHelper.LineHeight;
            }

            if (changed)
                SetVectorValueAll(editor, name, new Vector4(newVals[0], newVals[1], newVals[2], newVals[3]));
        }

        // ── TwoVector2 ───────────────────────────────────────────────

        private void DrawTwoVector2(
            Rect position, MaterialProperty prop,
            ComponentConfig[] configs, MaterialEditor editor)
        {
            Vector4 current = prop.vectorValue;
            string  name    = prop.name;

            var xy = new Vector2(current.x, current.y);
            var zw = new Vector2(current.z, current.w);

            bool mixedXY = IsMixedVec2(editor, name, v => new Vector2(v.x, v.y));
            bool mixedZw = IsMixedVec2(editor, name, v => new Vector2(v.z, v.w));

            // 动态高度：Vector2Field 内部占两行，按真实高度分配 Rect，消除输入框挤压
            float vec2H  = DrawerRectHelper.CalcVector2FieldHeight();
            Rect  rectXY = DrawerRectHelper.GetRectAtOffset(position, 0f, vec2H);
            Rect  rectZw = DrawerRectHelper.GetRectAtOffset(position, vec2H + DrawerRectHelper.LineSpacing, vec2H);

            string labelXY = configs[0]?.Label ?? string.Empty;
            string labelZw = configs[1]?.Label ?? string.Empty;

            Vector2 newXY   = xy;
            Vector2 newZw   = zw;
            bool    changed = false;

            EditorGUI.showMixedValue = mixedXY;
            EditorGUI.BeginChangeCheck();
            Vector2 tmpXY = EditorGUI.Vector2Field(rectXY, labelXY, xy);
            if (EditorGUI.EndChangeCheck()) { newXY = tmpXY; changed = true; }
            EditorGUI.showMixedValue = false;

            EditorGUI.showMixedValue = mixedZw;
            EditorGUI.BeginChangeCheck();
            Vector2 tmpZw = EditorGUI.Vector2Field(rectZw, labelZw, zw);
            if (EditorGUI.EndChangeCheck()) { newZw = tmpZw; changed = true; }
            EditorGUI.showMixedValue = false;

            if (changed)
                SetVectorValueAll(editor, name, new Vector4(newXY.x, newXY.y, newZw.x, newZw.y));
        }

        // ── Vector3Float ─────────────────────────────────────────────

        private void DrawVector3Float(
            Rect position, MaterialProperty prop,
            ComponentConfig[] configs, MaterialEditor editor)
        {
            Vector4 current = prop.vectorValue;
            string  name    = prop.name;

            var xyz = new Vector3(current.x, current.y, current.z);

            bool mixedXYZ = IsMixedVec3(editor, name, v => new Vector3(v.x, v.y, v.z));
            bool mixedW   = IsMixedFloat(editor, name, v => v.w);

            // 动态高度：Vector3Field 内部占两行，w 行紧随其后，消除输入框挤压
            float vec3H   = DrawerRectHelper.CalcVector3FieldHeight();
            Rect  rectXYZ = DrawerRectHelper.GetRectAtOffset(position, 0f, vec3H);
            Rect  rectW   = DrawerRectHelper.GetRectAtOffset(
                                position,
                                vec3H + DrawerRectHelper.LineSpacing,
                                EditorGUIUtility.singleLineHeight);

            string labelXYZ = configs[0]?.Label ?? string.Empty;
            string labelW   = configs[1]?.Label ?? string.Empty;

            Vector3 newXYZ  = xyz;
            float   newW    = current.w;
            bool    changed = false;

            EditorGUI.showMixedValue = mixedXYZ;
            EditorGUI.BeginChangeCheck();
            Vector3 tmpXYZ = EditorGUI.Vector3Field(rectXYZ, labelXYZ, xyz);
            if (EditorGUI.EndChangeCheck()) { newXYZ = tmpXYZ; changed = true; }
            EditorGUI.showMixedValue = false;

            EditorGUI.BeginChangeCheck();
            float tmpW = _cachedDrawers[1].Draw(rectW, labelW, current.w, mixedW, _labelWidth);
            if (EditorGUI.EndChangeCheck()) { newW = tmpW; changed = true; }

            if (changed)
                SetVectorValueAll(editor, name, new Vector4(newXYZ.x, newXYZ.y, newXYZ.z, newW));
        }

        // ── 多材质 mixed 检测辅助 ────────────────────────────────────

        private static bool IsMixedFloat(MaterialEditor editor, string propName, Func<Vector4, float> selector)
        {
            if (editor.targets.Length <= 1) return false;
            Vector4 first = ((Material)editor.targets[0]).GetVector(propName);
            float   ref0  = selector(first);
            for (int i = 1; i < editor.targets.Length; i++)
            {
                float val = selector(((Material)editor.targets[i]).GetVector(propName));
                if (!Mathf.Approximately(val, ref0)) return true;
            }
            return false;
        }

        private static bool IsMixedVec2(MaterialEditor editor, string propName, Func<Vector4, Vector2> selector)
        {
            if (editor.targets.Length <= 1) return false;
            Vector2 ref0 = selector(((Material)editor.targets[0]).GetVector(propName));
            for (int i = 1; i < editor.targets.Length; i++)
            {
                Vector2 val = selector(((Material)editor.targets[i]).GetVector(propName));
                // 逐分量使用 Mathf.Approximately，与 IsMixedFloat 行为严格对齐
                if (!Mathf.Approximately(val.x, ref0.x) ||
                    !Mathf.Approximately(val.y, ref0.y))
                    return true;
            }
            return false;
        }

        private static bool IsMixedVec3(MaterialEditor editor, string propName, Func<Vector4, Vector3> selector)
        {
            if (editor.targets.Length <= 1) return false;
            Vector3 ref0 = selector(((Material)editor.targets[0]).GetVector(propName));
            for (int i = 1; i < editor.targets.Length; i++)
            {
                Vector3 val = selector(((Material)editor.targets[i]).GetVector(propName));
                // 逐分量使用 Mathf.Approximately，与 IsMixedFloat 行为严格对齐
                if (!Mathf.Approximately(val.x, ref0.x) ||
                    !Mathf.Approximately(val.y, ref0.y) ||
                    !Mathf.Approximately(val.z, ref0.z))
                    return true;
            }
            return false;
        }

        // ── 写回所有 targets ─────────────────────────────────────────

        private static void SetVectorValueAll(MaterialEditor editor, string propName, Vector4 value)
        {
            // 批量记录 Undo，多材质编辑时仅生成一条 Undo 记录
            Undo.RecordObjects(editor.targets, "Vector4Split Change");
            foreach (var target in editor.targets)
            {
                var mat = (Material)target;
                mat.SetVector(propName, value);
                EditorUtility.SetDirty(mat);
            }
        }
    }
}

