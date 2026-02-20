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
        // ── 字段 ─────────────────────────────────────────────────────
        private readonly SplitMode _mode;
        private readonly float     _labelWidth;
        private readonly bool      _modeValid;
        private readonly string    _modeErrorMsg;

        // ── 构造函数 ─────────────────────────────────────────────────

        /// <summary>[Vector4Split(mode)]</summary>
        public Vector4SplitDrawer(string mode) : this(mode, -1f) { }

        /// <summary>[Vector4Split(mode, labelWidth)]</summary>
        public Vector4SplitDrawer(string mode, float labelWidth)
        {
            _labelWidth = labelWidth;

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
            string errorMsg = GetValidationError(prop);
            if (errorMsg != null)
                return DrawerRectHelper.CalcHelpBoxHeight(errorMsg);

            int lineCount = _mode == SplitMode.FourFloats ? 4 : 2;
            return DrawerRectHelper.CalcTotalHeight(lineCount);
        }

        // ── OnGUI ────────────────────────────────────────────────────

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            string errorMsg = GetValidationError(prop);
            if (errorMsg != null)
            {
                EditorGUI.HelpBox(position, errorMsg, MessageType.Warning);
                return;
            }

            // 解析 displayName（此时三重校验已通过，TryParse 必然成功）
            Vector4SplitDisplayNameParser.TryParse(
                prop.displayName, _mode,
                out _, out ComponentConfig[] configs, out _);

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

        // ── 三重校验 ─────────────────────────────────────────────────

        private string GetValidationError(MaterialProperty prop)
        {
            // 校验1：SplitMode 字符串合法性
            if (!_modeValid)
                return _modeErrorMsg;

            // 校验2：prop.type 必须是 Vector
            if (prop.type != MaterialProperty.PropType.Vector)
                return $"[Vector4Split] Used on a non-Vector property \"{prop.name}\" "
                     + $"(type: {prop.type}). This drawer only supports Vector properties.";

            // 校验3：displayName 格式（## 存在 + 段数正确）
            bool ok = Vector4SplitDisplayNameParser.TryParse(
                prop.displayName, _mode,
                out _, out _, out string parseError);

            return ok ? null : parseError;
        }

        // ── FourFloats ───────────────────────────────────────────────

        private void DrawFourFloats(
            Rect position, MaterialProperty prop,
            ComponentConfig[] configs, MaterialEditor editor)
        {
            Vector4 current = prop.vectorValue;
            string  name    = prop.name;

            bool mixedX = IsMixedFloat(editor, name, v => v.x);
            bool mixedY = IsMixedFloat(editor, name, v => v.y);
            bool mixedZ = IsMixedFloat(editor, name, v => v.z);
            bool mixedW = IsMixedFloat(editor, name, v => v.w);

            var drawers = new IFloatComponentDrawer[4];
            for (int i = 0; i < 4; i++)
                drawers[i] = ComponentDrawerFactory.Create(configs[i], _labelWidth);

            float newX = current.x;
            float newY = current.y;
            float newZ = current.z;
            float newW = current.w;
            bool  changed = false;

            EditorGUI.BeginChangeCheck();
            float tmpX = drawers[0].Draw(DrawerRectHelper.GetLineRect(position, 0), configs[0].Label, current.x, mixedX, _labelWidth);
            if (EditorGUI.EndChangeCheck()) { newX = tmpX; changed = true; }

            EditorGUI.BeginChangeCheck();
            float tmpY = drawers[1].Draw(DrawerRectHelper.GetLineRect(position, 1), configs[1].Label, current.y, mixedY, _labelWidth);
            if (EditorGUI.EndChangeCheck()) { newY = tmpY; changed = true; }

            EditorGUI.BeginChangeCheck();
            float tmpZ = drawers[2].Draw(DrawerRectHelper.GetLineRect(position, 2), configs[2].Label, current.z, mixedZ, _labelWidth);
            if (EditorGUI.EndChangeCheck()) { newZ = tmpZ; changed = true; }

            EditorGUI.BeginChangeCheck();
            float tmpW = drawers[3].Draw(DrawerRectHelper.GetLineRect(position, 3), configs[3].Label, current.w, mixedW, _labelWidth);
            if (EditorGUI.EndChangeCheck()) { newW = tmpW; changed = true; }

            if (changed)
                SetVectorValueAll(editor, name, new Vector4(newX, newY, newZ, newW));
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
            bool mixedZW = IsMixedVec2(editor, name, v => new Vector2(v.z, v.w));

            Vector2 newXY = xy;
            Vector2 newZW = zw;
            bool changed = false;

            EditorGUI.showMixedValue = mixedXY;
            EditorGUI.BeginChangeCheck();
            Vector2 tmpXY = EditorGUI.Vector2Field(DrawerRectHelper.GetLineRect(position, 0), configs[0].Label, xy);
            if (EditorGUI.EndChangeCheck()) { newXY = tmpXY; changed = true; }
            EditorGUI.showMixedValue = false;

            EditorGUI.showMixedValue = mixedZW;
            EditorGUI.BeginChangeCheck();
            Vector2 tmpZW = EditorGUI.Vector2Field(DrawerRectHelper.GetLineRect(position, 1), configs[1].Label, zw);
            if (EditorGUI.EndChangeCheck()) { newZW = tmpZW; changed = true; }
            EditorGUI.showMixedValue = false;

            if (changed)
                SetVectorValueAll(editor, name, new Vector4(newXY.x, newXY.y, newZW.x, newZW.y));
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

            IFloatComponentDrawer wDrawer = ComponentDrawerFactory.Create(configs[1], _labelWidth);

            Vector3 newXYZ = xyz;
            float   newW   = current.w;
            bool    changed = false;

            EditorGUI.showMixedValue = mixedXYZ;
            EditorGUI.BeginChangeCheck();
            Vector3 tmpXYZ = EditorGUI.Vector3Field(DrawerRectHelper.GetLineRect(position, 0), configs[0].Label, xyz);
            if (EditorGUI.EndChangeCheck()) { newXYZ = tmpXYZ; changed = true; }
            EditorGUI.showMixedValue = false;

            EditorGUI.BeginChangeCheck();
            float tmpW = wDrawer.Draw(DrawerRectHelper.GetLineRect(position, 1), configs[1].Label, current.w, mixedW, _labelWidth);
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
                if (val != ref0) return true;
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
                if (val != ref0) return true;
            }
            return false;
        }

        // ── 写回所有 targets ─────────────────────────────────────────

        private static void SetVectorValueAll(MaterialEditor editor, string propName, Vector4 value)
        {
            foreach (var target in editor.targets)
            {
                var mat = (Material)target;
                Undo.RecordObject(mat, "Vector4Split Change");
                mat.SetVector(propName, value);
                EditorUtility.SetDirty(mat);
            }
        }
    }
}
