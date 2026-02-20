using UnityEditor;
using UnityEngine;

namespace ScorpioEditor
{
    /// <summary>
    /// 标记子模块开始，构造参数写入 DrawerInfoRegistry。
    ///
    /// 用法：
    ///   [SubModuleBegin(Title)]
    ///   [SubModuleBegin(Title, _KEYWORD_ON)]
    ///   [SubModuleBegin(Title, _PropName, prop)]
    ///
    /// 附加在任意 [HideInInspector] Float 属性上，属性名以 _SubModuleBegin_ 开头。
    /// </summary>
    public class SubModuleBeginDrawer : MaterialPropertyDrawer
    {
        public SubModuleBeginDrawer(string title)
            : this(title, null, null) { }

        public SubModuleBeginDrawer(string title, string toggleTarget)
            : this(title, toggleTarget, null) { }

        public SubModuleBeginDrawer(string title, string toggleTarget, string toggleMode)
        {
            _pendingTitle  = title;
            _pendingTarget = toggleTarget ?? string.Empty;
            _pendingMode   = toggleMode;
        }

        private readonly string _pendingTitle;
        private readonly string _pendingTarget;
        private readonly string _pendingMode;

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
                Level        = ModuleLevel.Child,
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
        }
    }
}
