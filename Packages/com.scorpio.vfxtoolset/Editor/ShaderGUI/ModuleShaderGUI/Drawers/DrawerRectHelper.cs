using UnityEditor;
using UnityEngine;

namespace ScorpioEditor
{
    /// <summary>
    /// MaterialPropertyDrawer 通用 Rect 行分配辅助工具。
    /// 独立文件，供所有自定义 Drawer 复用。
    ///
    /// ── 使用说明 ──────────────────────────────────────────────────────
    ///   • GetLineRect(totalRect, i)  → 从 GetPropertyHeight 返回的总高度区域切出第 i 行，
    ///     自动应用当前 EditorGUI.indentLevel，无需调用方手动处理缩进。
    ///   • CalcTotalHeight(lineCount) → 供 GetPropertyHeight 返回值使用。
    ///   • CalcHelpBoxHeight(msg)     → 供错误分支的 GetPropertyHeight 使用，高度随文本自适应。
    /// </summary>
    public static class DrawerRectHelper
    {
        // ── 常量 ──────────────────────────────────────────────────────

        /// <summary>行间距（像素）。</summary>
        public const float LineSpacing = 2f;

        // ── 高度计算 ──────────────────────────────────────────────────

        /// <summary>单行高度 = singleLineHeight + LineSpacing。</summary>
        public static float LineHeight => EditorGUIUtility.singleLineHeight + LineSpacing;

        /// <summary>
        /// 计算 <paramref name="lineCount"/> 行所需总高度。
        /// 用于 MaterialPropertyDrawer.GetPropertyHeight 的返回值。
        /// </summary>
        public static float CalcTotalHeight(int lineCount)
            => lineCount * LineHeight;

        /// <summary>
        /// 计算 HelpBox 所需高度（随文本内容自适应换行）。
        /// 用于校验失败分支的 GetPropertyHeight。
        /// <para>
        /// 注意：在 GetPropertyHeight 调用时 <c>position.width</c> 无效，
        /// 此处使用 <c>EditorGUIUtility.currentViewWidth</c> 估算宽度。
        /// </para>
        /// </summary>
        public static float CalcHelpBoxHeight(string message)
        {
            float viewWidth = EditorGUIUtility.currentViewWidth > 0f
                ? EditorGUIUtility.currentViewWidth
                : 350f; // 安全 fallback

            // HelpBox 内容宽度需扣除 Inspector 左右边距和图标宽度
            float contentWidth = Mathf.Max(viewWidth - 60f, 100f);
            var   content      = new GUIContent(message);
            float textHeight   = EditorStyles.helpBox.CalcHeight(content, contentWidth);

            // 保证最低高度（图标至少需要 38px 才能完整显示）
            return Mathf.Max(textHeight, 38f);
        }

        // ── 动态控件高度 ──────────────────────────────────────────────

        /// <summary>
        /// 动态计算 <see cref="EditorGUI.Vector2Field"/> 所需高度。
        /// 等于两倍 singleLineHeight + standardVerticalSpacing，随 DPI 自适应。
        /// </summary>
        public static float CalcVector2FieldHeight()
            => EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing;

        /// <summary>
        /// 动态计算 <see cref="EditorGUI.Vector3Field"/> 所需高度。
        /// 等于两倍 singleLineHeight + standardVerticalSpacing，随 DPI 自适应。
        /// </summary>
        public static float CalcVector3FieldHeight()
            => EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing;

        // ── Rect 行分配 ───────────────────────────────────────────────

        /// <summary>
        /// 从 <paramref name="totalRect"/> 切出第 <paramref name="lineIndex"/> 行（0-based），
        /// 并调用 <see cref="EditorGUI.IndentedRect"/> 自动处理当前缩进级别。
        /// 适用于所有行等高（<see cref="LineHeight"/>）的场景（如 FourFloats 模式）。
        /// </summary>
        /// <param name="totalRect">MaterialPropertyDrawer.OnGUI 收到的完整区域。</param>
        /// <param name="lineIndex">行索引，从 0 开始。</param>
        public static Rect GetLineRect(Rect totalRect, int lineIndex)
        {
            float lh  = LineHeight;
            var   raw = new Rect(
                totalRect.x,
                totalRect.y + lineIndex * lh,
                totalRect.width,
                EditorGUIUtility.singleLineHeight);

            return EditorGUI.IndentedRect(raw);
        }

        /// <summary>
        /// 从 <paramref name="totalRect"/> 按绝对 <paramref name="yOffset"/> 偏移切出指定
        /// <paramref name="height"/> 高度的 Rect，并应用 <see cref="EditorGUI.IndentedRect"/>。
        /// 适用于各行高度不等的场景（如 TwoVector2 / Vector3Float 模式）。
        /// </summary>
        /// <param name="totalRect">MaterialPropertyDrawer.OnGUI 收到的完整区域。</param>
        /// <param name="yOffset">距 totalRect 顶部的像素偏移量。</param>
        /// <param name="height">该行的像素高度。</param>
        public static Rect GetRectAtOffset(Rect totalRect, float yOffset, float height)
        {
            var raw = new Rect(
                totalRect.x,
                totalRect.y + yOffset,
                totalRect.width,
                height);

            return EditorGUI.IndentedRect(raw);
        }
    }
}

