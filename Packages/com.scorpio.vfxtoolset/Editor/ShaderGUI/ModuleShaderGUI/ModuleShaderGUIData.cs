using System.Collections.Generic;
using UnityEditor;

namespace ScorpioEditor
{
    // ── 模块层级 ─────────────────────────────────────────────
    public enum ModuleLevel
    {
        Parent,
        Child
    }

    // ── 开关类型 ─────────────────────────────────────────────
    public enum ModuleToggleType
    {
        None,       // 无开关
        Keyword,    // shader_feature keyword
        Property    // float property 直接作为 0/1 开关
    }

    // ── 单个模块描述 ──────────────────────────────────────────
    public class ModuleEntry
    {
        public ModuleLevel           Level;
        public string                Title;
        public ModuleToggleType      ToggleType;
        public string                ToggleTarget;   // keyword 名 或 property 名
        public bool                  AlsoEndParent;  // SubModuleEnd 时是否同时关闭父模块（仅子模块末尾使用）
        public List<MaterialProperty> BodyProperties = new List<MaterialProperty>();
    }

    // ── GUI 类解析出的帧数据 ──────────────────────────────────
    public class ModuleShaderGUIFrameData
    {
        public List<MaterialProperty> HeaderProperties = new List<MaterialProperty>();
        public List<ModuleEntry>      Modules          = new List<ModuleEntry>();
        public List<MaterialProperty> FooterProperties = new List<MaterialProperty>();
    }
}

