using System.Collections.Generic;
using UnityEditor;

namespace ScorpioEditor
{
    // ── 模块层级 ──────────────────────────────────────────────────────
    public enum ModuleLevel
    {
        Parent,
        Child
    }

    // ── 开关类型 ──────────────────────────────────────────────────────
    public enum ModuleToggleType
    {
        None,       // 无开关
        Keyword,    // shader_feature keyword（EnableKeyword / DisableKeyword）
        Property    // float property 直接作为 0/1 开关
    }

    // ── ModuleEnd 的作用范围 ──────────────────────────────────────────
    public enum ModuleEndScope
    {
        Parent,         // [ModuleEnd]          → 结束父模块
        Child,          // [ModuleEnd(sub)]      → 仅结束子模块
        ChildAndParent  // [ModuleEnd(sub, end)] → 结束子模块同时结束父模块
    }

    // ── Drawer 向 GUI 类传递的原始信息 ───────────────────────────────
    // 由 ModuleBeginDrawer / SubModuleBeginDrawer / ModuleEndDrawer
    // 在构造函数中写入，GUI 类在 CollectModules 时查表读取。
    public class DrawerInfo
    {
        // Begin 系列
        public ModuleLevel      Level;          // Parent / Child
        public string           Title;
        public ModuleToggleType ToggleType;
        public string           ToggleTarget;   // keyword 名 或 property 名

        // End 系列
        public bool   IsEnd;
        public ModuleEndScope EndScope;
    }

    // ── 静态注册表（propertyName → DrawerInfo）───────────────────────
    public static class DrawerInfoRegistry
    {
        private static readonly Dictionary<string, DrawerInfo> _map =
            new Dictionary<string, DrawerInfo>();

        public static void Register(string propertyName, DrawerInfo info)
        {
            _map[propertyName] = info;
        }

        public static bool TryGet(string propertyName, out DrawerInfo info)
            => _map.TryGetValue(propertyName, out info);
    }

    // ── 单个模块描述（GUI 绘制阶段使用）─────────────────────────────
    public class ModuleEntry
    {
        public ModuleLevel            Level;
        public string                 Title;
        public ModuleToggleType       ToggleType;
        public string                 ToggleTarget;
        public List<MaterialProperty> BodyProperties = new List<MaterialProperty>();
    }

    // ── GUI 类解析出的帧数据 ──────────────────────────────────────────
    public class ModuleShaderGUIFrameData
    {
        public List<MaterialProperty> HeaderProperties = new List<MaterialProperty>();
        public List<ModuleEntry>      Modules          = new List<ModuleEntry>();
        public List<MaterialProperty> FooterProperties = new List<MaterialProperty>();
    }
}

