using UnityEditor;
using UnityEngine;

namespace ScorpioEditor
{
    /// <summary>
    /// 标记父模块开始，构造参数写入 DrawerInfoRegistry 供 ScorpioModuleShaderGUIBase 读取。
    /// 模块标题通过属性的 displayName 传入，支持中文及任意字符。
    ///
    /// 用法：
    ///   [ModuleBegin]              → 无开关，可折叠
    ///   [ModuleBegin(1)]           → 无开关，始终展开（无折叠箭头、不可收起）
    ///   [ModuleBegin(_KEYWORD_ON)] → keyword 开关
    ///   [ModuleBegin(prop)]        → property 开关，开关属性名自动取模块 body 第一个属性
    ///   [ModuleBegin(_KEYWORD, 1)] → keyword 开关 + 始终展开
    ///   [ModuleBegin(prop, 1)]     → property 开关 + 始终展开
    ///
    /// 附加在 [HideInInspector] Float 属性上，属性名以 _ModuleBegin_ 开头。
    /// </summary>
    public class ModuleBeginDrawer : MaterialPropertyDrawer
    {
        // 无开关，可折叠：[ModuleBegin]
        public ModuleBeginDrawer() : this(null, 0f) { }

        // keyword/prop 开关，可折叠：[ModuleBegin(_KEYWORD_ON)] / [ModuleBegin(prop)]
        public ModuleBeginDrawer(string arg) : this(arg, 0f) { }

        // 无开关，始终展开：[ModuleBegin(1)]
        public ModuleBeginDrawer(float alwaysExpanded) : this(null, alwaysExpanded) { }

        // keyword/prop 开关 + 始终展开：[ModuleBegin(_KEYWORD_ON, 1)]
        public ModuleBeginDrawer(string arg, float alwaysExpanded)
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
                Level          = ModuleLevel.Parent,
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
