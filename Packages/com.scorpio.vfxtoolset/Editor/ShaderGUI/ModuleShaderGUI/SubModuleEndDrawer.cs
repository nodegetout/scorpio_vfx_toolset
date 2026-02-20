using UnityEditor;
using UnityEngine;

namespace ScorpioEditor
{
    /// <summary>
    /// 标记子模块结束，可选同时结束父模块。
    /// 用法一（仅结束子模块）：   [SubModuleEnd]
    /// 用法二（同时结束父模块）：  [SubModuleEnd(1)]  ← shader 中布尔参数用 1 表示 true
    ///
    /// 约定：附加该 Drawer 的 Float 属性名必须以 _SubModuleEnd_ 开头，并加 [HideInInspector]。
    /// </summary>
    public class SubModuleEndDrawer : MaterialPropertyDrawer
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

