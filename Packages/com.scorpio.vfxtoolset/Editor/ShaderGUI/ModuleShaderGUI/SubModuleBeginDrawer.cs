using UnityEditor;
using UnityEngine;

namespace ScorpioEditor
{
    /// <summary>
    /// 标记子模块开始。
    /// 用法一（无开关）：    [SubModuleBegin(Title)]
    /// 用法二（keyword 开关）：[SubModuleBegin(Title, _KEYWORD_ON)]
    /// 用法三（property 开关）：[SubModuleBegin(Title, _PropertyName, prop)]
    ///
    /// 约定：附加该 Drawer 的 Float 属性名必须以 _SubModuleBegin_ 开头，并加 [HideInInspector]。
    /// </summary>
    public class SubModuleBeginDrawer : MaterialPropertyDrawer
    {
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

