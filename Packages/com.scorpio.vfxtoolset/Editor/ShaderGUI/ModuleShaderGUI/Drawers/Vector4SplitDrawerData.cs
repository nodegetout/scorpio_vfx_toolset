using System;
using System.Text.RegularExpressions;

namespace ScorpioEditor
{
    // ── 拆分模式 ──────────────────────────────────────────────────────

    /// <summary>
    /// Vector4 属性的拆分显示模式。
    /// </summary>
    public enum SplitMode
    {
        /// <summary>拆为 4 个独立 float 分量，每个分量可独立配置绘制类型。</summary>
        FourFloats,

        /// <summary>拆为 2 个 Vector2（xy / zw），使用 Vector2Field 渲染。</summary>
        TwoVector2,

        /// <summary>拆为 1 个 Vector3（xyz）+ 1 个独立 float（w），float 可配置绘制类型。</summary>
        Vector3Float,
    }

    // ── 浮点分量绘制类型 ───────────────────────────────────────────────

    /// <summary>
    /// 独立 float 分量的绘制控件类型。
    /// </summary>
    public enum FloatDrawType
    {
        /// <summary>普通输入框（EditorGUI.FloatField）。</summary>
        Float,

        /// <summary>滑动条（EditorGUI.Slider），需配合 min/max 参数使用。</summary>
        Slider,

        /// <summary>勾选框（EditorGUI.Toggle），值为 0 或 1。</summary>
        Toggle,

        /// <summary>隐藏分量，不绘制不占高度，值保持不变。</summary>
        Hidden,
    }

    // ── 分量配置 ──────────────────────────────────────────────────────

    /// <summary>
    /// 单个分量的显示配置。
    /// </summary>
    public class ComponentConfig
    {
        /// <summary>在 Inspector 上显示的标签文本。</summary>
        public string       Label;

        /// <summary>绘制控件类型。</summary>
        public FloatDrawType DrawType;

        /// <summary>Slider 最小值（仅 DrawType == Slider 时有效）。</summary>
        public float Min;

        /// <summary>Slider 最大值（仅 DrawType == Slider 时有效）。</summary>
        public float Max;
    }

    // ── displayName 解析器 ────────────────────────────────────────────

    /// <summary>
    /// 负责将 Shader 属性的 displayName 解析为标题 + 各分量配置。
    ///
    /// ── displayName 格式 ─────────────────────────────────────────────
    ///
    ///   简洁标题 ## 段1配置 @ 段2配置 @ ...
    ///
    ///   • <c>##</c>  分隔"显示标题"和"分量配置区"（必须存在）
    ///   • <c>@</c>   分隔各配置段（段间空格会被 trim）
    ///   • <c>|</c>   分隔标签与绘制类型（首个 | 有效，后续作为标签一部分；可缺省，缺省时默认 Float）
    ///
    ///   DrawType 取值：
    ///     Float              → 普通浮点输入框（默认）
    ///     Slider(min, max)   → 滑动条，括号内填范围，支持逗号前后有空格
    ///     Toggle             → 勾选框（0/1）
    ///     Hidden             → 隐藏分量，不绘制不占高度
    ///
    ///   段数约束：
    ///     FourFloats   → 4 段
    ///     TwoVector2   → 2 段（| 后内容静默忽略，无 DrawType）
    ///     Vector3Float → 2 段（第1段无 DrawType，第2段可配置 DrawType）
    ///
    /// ── 示例 ─────────────────────────────────────────────────────────
    ///
    ///   FourFloats：
    ///     "UV参数 ## X速度|Float @ Y速度 @ 缩放|Slider(0, 10) @ 开关|Toggle"
    ///
    ///   TwoVector2：
    ///     "流速旋转 ## 流速XY @ 缩放旋转ZW"
    ///
    ///   Vector3Float：
    ///     "偏移参数 ## 方向XYZ @ 强度|Slider(0, 5)"
    /// </summary>
    public static class Vector4SplitDisplayNameParser
    {
        private static readonly Regex s_SliderRegex =
            new Regex(@"Slider\(\s*(-?[\d.]+)\s*,\s*(-?[\d.]+)\s*\)", RegexOptions.IgnoreCase);

        /// <summary>
        /// 尝试将 <paramref name="displayName"/> 解析为标题 + 分量配置数组。
        /// </summary>
        /// <param name="displayName">Shader 属性的 displayName 原始字符串。</param>
        /// <param name="mode">拆分模式，决定期望的段数。</param>
        /// <param name="title">解析出的简洁标题（<c>##</c> 之前的部分）。</param>
        /// <param name="configs">解析出的各段分量配置数组。失败时为 null。</param>
        /// <param name="errorMsg">校验失败时的错误提示（供 HelpBox 显示）。失败时非 null。</param>
        /// <returns>解析成功返回 <c>true</c>，失败返回 <c>false</c>。</returns>
        public static bool TryParse(
            string displayName, SplitMode mode,
            out string title, out ComponentConfig[] configs, out string errorMsg)
        {
            configs  = null;
            errorMsg = null;

            // ── 步骤1：检查 ## 分隔符 ────────────────────────────────
            int separatorIdx = displayName.IndexOf("##", StringComparison.Ordinal);
            if (separatorIdx < 0)
            {
                title    = displayName.Trim();
                errorMsg = $"[Vector4Split] Missing '##' separator in displayName \"{displayName}\".\n"
                         + "Format: \"Title ## Seg1|DrawType @ Seg2|DrawType @ ...\"";
                return false;
            }

            title = displayName.Substring(0, separatorIdx).Trim();
            string configPart = displayName.Substring(separatorIdx + 2).Trim();

            // ── 步骤2：按 @ 分段 ─────────────────────────────────────
            string[] segments     = configPart.Split('@');
            int      expectedCount = mode == SplitMode.FourFloats ? 4 : 2;

            if (segments.Length != expectedCount)
            {
                errorMsg = $"[Vector4Split({mode})] Expected {expectedCount} segments separated by '@', "
                         + $"but got {segments.Length}. DisplayName: \"{displayName}\"";
                return false;
            }

            // ── 步骤3：解析各段 ──────────────────────────────────────
            configs = new ComponentConfig[segments.Length];
            for (int i = 0; i < segments.Length; i++)
            {
                string seg = segments[i].Trim();

                // TwoVector2 和 Vector3Float 第1段 → 无 DrawType
                bool skipDrawType = (mode == SplitMode.TwoVector2)
                                 || (mode == SplitMode.Vector3Float && i == 0);

                configs[i] = ParseSegment(seg, skipDrawType);
            }

            return true;
        }

        // ── 私有辅助 ──────────────────────────────────────────────────

        private static ComponentConfig ParseSegment(string seg, bool skipDrawType)
        {
            var config = new ComponentConfig
            {
                DrawType = FloatDrawType.Float,
                Min      = 0f,
                Max      = 1f,
            };

            // 按首个 | 拆分标签和类型
            int pipeIdx = seg.IndexOf('|');
            if (pipeIdx < 0 || skipDrawType)
            {
                // 无 | 或不需要 DrawType：整个段作为标签
                config.Label = seg.Trim();
                return config;
            }

            config.Label       = seg.Substring(0, pipeIdx).Trim();
            string drawTypePart = seg.Substring(pipeIdx + 1).Trim();

            // 解析 Slider(min, max)
            var sliderMatch = s_SliderRegex.Match(drawTypePart);
            if (sliderMatch.Success)
            {
                config.DrawType = FloatDrawType.Slider;
                float.TryParse(sliderMatch.Groups[1].Value, out float min);
                float.TryParse(sliderMatch.Groups[2].Value, out float max);
                config.Min = min;
                config.Max = max;
                return config;
            }

            // 解析 Toggle / Hidden / Float（大小写不敏感）
            if (drawTypePart.Equals("Toggle", StringComparison.OrdinalIgnoreCase))
            {
                config.DrawType = FloatDrawType.Toggle;
            }
            else if (drawTypePart.Equals("Hidden", StringComparison.OrdinalIgnoreCase))
            {
                config.DrawType = FloatDrawType.Hidden;
            }
            else
            {
                // Float 或未知类型，默认 Float
                config.DrawType = FloatDrawType.Float;
            }

            return config;
        }
    }
}

