using UnityEditor;
using UnityEngine;

namespace ScorpioEditor
{
    /// <summary>
    /// 标记父模块开始，构造参数写入 DrawerInfoRegistry 供 ScorpioModuleShaderGUIBase 读取。
    ///
    /// 用法（三种开关模式）：
    ///   [ModuleBegin(Title)]                         → 无开关
    ///   [ModuleBegin(Title, _KEYWORD_ON)]             → keyword 开关（默认）
    ///   [ModuleBegin(Title, _PropName, prop)]         → property 开关
    ///
    /// 附加在任意 [HideInInspector] Float 属性上，属性名以 _ModuleBegin_ 开头。
    /// </summary>
    public class ModuleBeginDrawer : MaterialPropertyDrawer
    {
        // 无开关：[ModuleBegin(Title)]
        public ModuleBeginDrawer(string title)
            : this(title, null, null) { }

        // keyword 开关：[ModuleBegin(Title, _KEYWORD_ON)]
        public ModuleBeginDrawer(string title, string toggleTarget)
            : this(title, toggleTarget, null) { }

        // property 开关：[ModuleBegin(Title, _PropName, prop)]
        public ModuleBeginDrawer(string title, string toggleTarget, string toggleMode)
        {
            // MaterialPropertyDrawer 构造时拿不到 propertyName，
            // 注册延迟到第一次 OnGUI / GetPropertyHeight，届时通过 prop.name 取得。
            _pendingTitle      = title;
            _pendingTarget     = toggleTarget ?? string.Empty;
            _pendingMode       = toggleMode;
        }

        private readonly string _pendingTitle;
        private readonly string _pendingTarget;
        private readonly string _pendingMode;   // null / "prop"

        private void EnsureRegistered(string propName)
        {
            if (DrawerInfoRegistry.TryGet(propName, out _)) return;

            ModuleToggleType toggleType;
            if (string.IsNullOrEmpty(_pendingTarget))
                toggleType = ModuleToggleType.None;
            else if (_pendingMode == "prop")
                toggleType = ModuleToggleType.Property;
            else
                toggleType = ModuleToggleType.Keyword;

            DrawerInfoRegistry.Register(propName, new DrawerInfo
            {
                Level        = ModuleLevel.Parent,
                Title        = _pendingTitle,
                ToggleType   = toggleType,
                ToggleTarget = _pendingTarget,
                IsEnd        = false
            });
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            EnsureRegistered(prop.name);
            return 0f;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            EnsureRegistered(prop.name);
            // 不绘制任何内容
        }
    }
}
