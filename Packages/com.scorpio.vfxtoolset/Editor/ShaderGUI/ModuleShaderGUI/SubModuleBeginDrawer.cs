using UnityEditor;
using UnityEngine;

namespace ScorpioEditor
{
    /// <summary>
    /// 标记子模块开始，构造参数写入 DrawerInfoRegistry。
    /// 模块标题通过属性的 displayName 传入，支持中文及任意字符。
    ///
    /// 用法：
    ///   [SubModuleBegin]              → 无开关
    ///   [SubModuleBegin(_KEYWORD_ON)] → keyword 开关
    ///   [SubModuleBegin(prop)]        → property 开关，开关属性名自动取模块 body 第一个属性
    ///
    /// 示例：
    ///   [HideInInspector][SubModuleBegin(_FRESNEL_ON)] _SubModuleBegin_Fresnel ("菲涅尔", Float) = 0
    ///
    /// 附加在 [HideInInspector] Float 属性上，属性名以 _SubModuleBegin_ 开头。
    /// </summary>
    public class SubModuleBeginDrawer : MaterialPropertyDrawer
    {
        // 无开关：[SubModuleBegin]
        public SubModuleBeginDrawer() : this(null) { }

        // keyword 开关：[SubModuleBegin(_KEYWORD_ON)]
        // property 开关：[SubModuleBegin(prop)]
        public SubModuleBeginDrawer(string arg)
        {
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

        private void EnsureRegistered(string propName, string displayName)
        {
            if (DrawerInfoRegistry.TryGet(propName, out _)) return;

            DrawerInfoRegistry.Register(propName, new DrawerInfo
            {
                Level        = ModuleLevel.Child,
                Title        = displayName,
                ToggleType   = _toggleType,
                ToggleTarget = _toggleTarget,
                IsEnd        = false
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
