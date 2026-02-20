using UnityEditor;
using UnityEngine;

namespace ScorpioEditor
{
    /// <summary>
    /// 标记父模块结束。
    /// 用法：[ModuleEnd]
    ///
    /// 约定：附加该 Drawer 的 Float 属性名必须以 _ModuleEnd_ 开头，并加 [HideInInspector]。
    /// </summary>
    public class ModuleEndDrawer : MaterialPropertyDrawer
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

