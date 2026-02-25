using UnityEditor;
using UnityEngine;

namespace ScorpioEditor
{
    /// <summary>
    /// 统一的模块结束标记，直接附加在模块的最后一个 body 属性上（无需额外占位属性）。
    ///
    /// 用法：
    ///   [ModuleEnd]           → 结束父模块
    ///   [ModuleEnd(sub)]      → 仅结束子模块（父模块继续）
    ///   [ModuleEnd(sub, end)] → 结束子模块，同时结束父模块
    ///
    /// 注意：该 Drawer 不影响属性本身的绘制高度和外观，属性仍正常渲染。
    /// </summary>
    public class ModuleEndDrawer : MaterialPropertyDrawer
    {
        // 无参：结束父模块
        public ModuleEndDrawer()
        {
            _scope = ModuleEndScope.Parent;
        }

        // 一个参数："sub" → 仅结束子模块
        public ModuleEndDrawer(string arg0)
        {
            _scope = arg0 == "sub" ? ModuleEndScope.Child : ModuleEndScope.Parent;
        }

        // 两个参数："sub", "end" → 结束子模块同时结束父模块
        public ModuleEndDrawer(string arg0, string arg1)
        {
            _scope = (arg0 == "sub" && arg1 == "end")
                ? ModuleEndScope.ChildAndParent
                : ModuleEndScope.Parent;
        }

        private readonly ModuleEndScope _scope;

        private void EnsureRegistered(string propName)
        {
            // 仅注册 End 信息；若该属性已经有 Begin 信息（不应出现），跳过
            if (DrawerInfoRegistry.TryGet(propName, out var existing) && existing.IsEnd) return;

            DrawerInfoRegistry.Register(propName, new DrawerInfo
            {
                IsEnd    = true,
                EndScope = _scope
            });
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            EnsureRegistered(prop.name);
            // 纯标记 Drawer，不占高度；实际绘制由 ScorpioModuleShaderGUIBase 负责
            return 0f;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            EnsureRegistered(prop.name);
            // 纯标记 Drawer，不绘制任何内容；
            // 属性的实际渲染由 ScorpioModuleShaderGUIBase.DrawModuleBody 通过
            // editor.DefaultShaderProperty 完成，避免与同属性上的其他 Drawer（如 Vector4SplitDrawer）冲突。
        }
    }
}
