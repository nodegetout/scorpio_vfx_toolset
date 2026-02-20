using System;
using UnityEditor;
using UnityEngine;

namespace ScorpioEditor
{
    /// <summary>
    /// 已废弃。请改用 [ModuleEnd(sub)] 或 [ModuleEnd(sub, end)]。
    /// </summary>
    [Obsolete("SubModuleEndDrawer is obsolete. Use [ModuleEnd(sub)] or [ModuleEnd(sub, end)] instead.")]
    public class SubModuleEndDrawer : MaterialPropertyDrawer
    {
        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
            => 0f;

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            // 已废弃，不绘制
        }
    }
}
