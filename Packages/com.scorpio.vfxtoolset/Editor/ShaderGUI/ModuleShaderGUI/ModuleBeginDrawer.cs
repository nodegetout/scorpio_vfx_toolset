using UnityEditor;
using UnityEngine;

namespace ScorpioEditor
{
    /// <summary>
    /// 标记父模块开始。
    /// 用法一（无开关）：   [ModuleBegin(Title)]
    /// 用法二（keyword 开关）：[ModuleBegin(Title, _KEYWORD_ON)]
    /// 用法三（property 开关）：[ModuleBegin(Title, _PropertyName, prop)]
    ///
    /// 约定：附加该 Drawer 的 Float 属性名必须以 _ModuleBegin_ 开头，并加 [HideInInspector]。
    /// </summary>
    public class ModuleBeginDrawer : MaterialPropertyDrawer
    {
        // 供 ScorpioModuleShaderGUIBase 读取的编码字符串，存储在 displayName 中
        // 格式：__ModuleBegin__|Title|ToggleType|ToggleTarget
        // 该字段写入属性的 displayName 是通过 shader 里的 displayName 约定传递的，
        // 实际解析在 ScorpioModuleShaderGUIBase 里完成，Drawer 本身只需保证高度为 0。

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return 0f;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            // 不绘制任何内容，模块标记属性完全隐藏
        }
    }
}

