using UnityEditor;
using UnityEngine;

namespace ScorpioEditor
{
    /// <summary>
    /// 标记父模块开始，构造参数写入 DrawerInfoRegistry 供 ScorpioModuleShaderGUIBase 读取。
    /// 模块标题通过属性的 displayName 传入，支持中文及任意字符。
    ///
    /// 用法：
    ///   [ModuleBegin]              → 无开关
    ///   [ModuleBegin(_KEYWORD_ON)] → keyword 开关
    ///   [ModuleBegin(prop)]        → property 开关，开关属性名自动取模块 body 第一个属性
    ///
    /// 示例：
    ///   [HideInInspector][ModuleBegin(_BASE_COLOR_ON)] _ModuleBegin_BaseColor ("基础颜色", Float) = 0
    ///   [HideInInspector][ModuleBegin(prop)]           _ModuleBegin_Dissolve  ("溶解",     Float) = 0
    ///       [HideInInspector] _DissolveOn ("", Float) = 0   ← 自动识别为开关属性（body 第一个）
    ///
    /// 附加在 [HideInInspector] Float 属性上，属性名以 _ModuleBegin_ 开头。
    /// </summary>
    public class ModuleBeginDrawer : MaterialPropertyDrawer
    {
        // 无开关：[ModuleBegin]
        public ModuleBeginDrawer() : this(null) { }

        // keyword 开关：[ModuleBegin(_KEYWORD_ON)]
        // property 开关：[ModuleBegin(prop)]
        public ModuleBeginDrawer(string arg)
        {
            if (arg == "prop")
            {
                _toggleType   = ModuleToggleType.Property;
                _toggleTarget = string.Empty;   // 自动从 body[0] 收集
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
                Level        = ModuleLevel.Parent,
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
            // 不绘制任何内容
        }
    }
}
