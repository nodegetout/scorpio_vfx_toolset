using UnityEditor;
using UnityEngine;

namespace ScorpioEditor
{
    /// <summary>
    /// 标记子模块开始，构造参数写入 DrawerInfoRegistry。
    /// 模块标题通过属性的 displayName 传入，支持中文及任意字符。
    ///
    /// 用法：
    ///   [SubModuleBegin]              → 无开关，可折叠
    ///   [SubModuleBegin(1)]           → 无开关，始终展开
    ///   [SubModuleBegin(_KEYWORD_ON)] → keyword 开关
    ///   [SubModuleBegin(prop)]        → property 开关，开关属性名自动取模块 body 第一个属性
    ///   [SubModuleBegin(_KEYWORD, 1)] → keyword 开关 + 始终展开
    ///   [SubModuleBegin(prop, 1)]     → property 开关 + 始终展开
    ///
    /// 附加在 [HideInInspector] Float 属性上，属性名以 _SubModuleBegin_ 开头。
    /// </summary>
    public class SubModuleBeginDrawer : MaterialPropertyDrawer
    {
        public SubModuleBeginDrawer() : this(null, 0f) { }

        public SubModuleBeginDrawer(string arg) : this(arg, 0f) { }

        public SubModuleBeginDrawer(float alwaysExpanded) : this(null, alwaysExpanded) { }

        public SubModuleBeginDrawer(string arg, float alwaysExpanded)
        {
            _alwaysExpanded = alwaysExpanded > 0.5f;

            if (arg == "prop")
            {
                _toggleType   = ModuleToggleType.Property;
                _toggleTarget = string.Empty;
            }
            else if (!string.IsNullOrEmpty(arg))
            {
                _toggleType   = ModuleToggleType.Keyword;
                _toggleTarget = arg;
            }
            else
            {
                _toggleType   = ModuleToggleType.None;
                _toggleTarget = string.Empty;
            }
        }

        private readonly ModuleToggleType _toggleType;
        private readonly string           _toggleTarget;
        private readonly bool             _alwaysExpanded;

        private void EnsureRegistered(string propName, string displayName)
        {
            if (DrawerInfoRegistry.TryGet(propName, out _)) return;

            DrawerInfoRegistry.Register(propName, new DrawerInfo
            {
                Level          = ModuleLevel.Child,
                Title          = displayName,
                ToggleType     = _toggleType,
                ToggleTarget   = _toggleTarget,
                AlwaysExpanded = _alwaysExpanded,
                IsEnd          = false
            });
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            EnsureRegistered(prop.name, prop.displayName);
            return 0f;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            EnsureRegistered(prop.name, prop.displayName);
        }
    }
}
