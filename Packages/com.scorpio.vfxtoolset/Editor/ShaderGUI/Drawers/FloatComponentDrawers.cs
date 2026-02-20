using UnityEditor;
using UnityEngine;

namespace ScorpioEditor
{
    // ── 接口定义 ──────────────────────────────────────────────────────

    /// <summary>
    /// 单个浮点分量的绘制接口。
    /// 实现类负责在给定 <see cref="Rect"/> 内渲染控件并返回编辑后的值。
    /// </summary>
    public interface IFloatComponentDrawer
    {
        /// <summary>
        /// 在 <paramref name="rect"/> 内绘制控件。
        /// </summary>
        /// <param name="rect">绘制区域（已由 DrawerRectHelper 处理好缩进）。</param>
        /// <param name="label">显示标签文本。</param>
        /// <param name="value">当前值。</param>
        /// <param name="showMixed">是否显示混合值状态（多材质编辑）。</param>
        /// <param name="labelWidth">
        /// 标签列宽度覆盖值。<br/>
        /// &gt; 0 时临时覆盖 <see cref="EditorGUIUtility.labelWidth"/>，绘制后自动还原；<br/>
        /// ≤ 0 时使用系统默认值。
        /// </param>
        /// <returns>用户编辑后的新值。</returns>
        float Draw(Rect rect, string label, float value, bool showMixed, float labelWidth);

        /// <summary>该控件占用的高度（像素）。</summary>
        float GetHeight();
    }

    // ── 各实现类 ──────────────────────────────────────────────────────

    /// <summary>普通浮点输入框（<see cref="EditorGUI.FloatField"/>）。</summary>
    public class FloatComponentDrawer : IFloatComponentDrawer
    {
        public float Draw(Rect rect, string label, float value, bool showMixed, float labelWidth)
        {
            EditorGUI.showMixedValue = showMixed;
            float prev = EditorGUIUtility.labelWidth;
            if (labelWidth > 0f) EditorGUIUtility.labelWidth = labelWidth;

            float result = EditorGUI.FloatField(rect, label, value);

            if (labelWidth > 0f) EditorGUIUtility.labelWidth = prev;
            EditorGUI.showMixedValue = false;
            return result;
        }

        public float GetHeight() => EditorGUIUtility.singleLineHeight;
    }

    /// <summary>
    /// 滑动条（<see cref="EditorGUI.Slider"/>）。
    /// <para>
    /// 构造时传入 min / max 范围；<paramref name="labelWidth"/> &gt; 0 时临时覆盖
    /// <see cref="EditorGUIUtility.labelWidth"/>，绘制后自动还原。
    /// </para>
    /// </summary>
    public class SliderComponentDrawer : IFloatComponentDrawer
    {
        private readonly float _min;
        private readonly float _max;

        public SliderComponentDrawer(float min, float max)
        {
            _min = min;
            _max = max;
        }

        public float Draw(Rect rect, string label, float value, bool showMixed, float labelWidth)
        {
            EditorGUI.showMixedValue = showMixed;
            float prevWidth = EditorGUIUtility.labelWidth;
            if (labelWidth > 0f) EditorGUIUtility.labelWidth = labelWidth;

            float result = EditorGUI.Slider(rect, label, value, _min, _max);

            if (labelWidth > 0f) EditorGUIUtility.labelWidth = prevWidth;
            EditorGUI.showMixedValue = false;
            return result;
        }

        public float GetHeight() => EditorGUIUtility.singleLineHeight;
    }

    /// <summary>
    /// 勾选框（<see cref="EditorGUI.Toggle"/>），返回 0f（未勾选）或 1f（已勾选）。
    /// </summary>
    public class ToggleComponentDrawer : IFloatComponentDrawer
    {
        public float Draw(Rect rect, string label, float value, bool showMixed, float labelWidth)
        {
            EditorGUI.showMixedValue = showMixed;
            float prev = EditorGUIUtility.labelWidth;
            if (labelWidth > 0f) EditorGUIUtility.labelWidth = labelWidth;

            bool result = EditorGUI.Toggle(rect, label, value > 0.5f);

            if (labelWidth > 0f) EditorGUIUtility.labelWidth = prev;
            EditorGUI.showMixedValue = false;
            return result ? 1f : 0f;
        }

        public float GetHeight() => EditorGUIUtility.singleLineHeight;
    }

    // ── 工厂 ─────────────────────────────────────────────────────────

    /// <summary>
    /// 根据 <see cref="ComponentConfig"/> 创建对应的 <see cref="IFloatComponentDrawer"/>。
    /// <para>扩展新类型时只需在 <c>switch</c> 中添加一个 <c>case</c>。</para>
    /// </summary>
    public static class ComponentDrawerFactory
    {
        /// <summary>
        /// 根据 <paramref name="config"/> 的 <see cref="ComponentConfig.DrawType"/> 创建绘制器。
        /// </summary>
        /// <param name="config">分量配置。</param>
        /// <param name="labelWidth">标签宽度覆盖值（≤ 0 表示使用系统默认）。</param>
        public static IFloatComponentDrawer Create(ComponentConfig config, float labelWidth)
        {
            // 防御性处理：config 为 null 时降级为默认 FloatComponentDrawer
            if (config == null)
                return new FloatComponentDrawer();

            switch (config.DrawType)
            {
                case FloatDrawType.Slider:
                    return new SliderComponentDrawer(config.Min, config.Max);

                case FloatDrawType.Toggle:
                    return new ToggleComponentDrawer();

                case FloatDrawType.Float:
                default:
                    return new FloatComponentDrawer();
            }
        }
    }
}

