---
name: Fix MaterialDecoratorDrawer Error
overview: 修复 MaterialPropertyDrawDecorator.cs 的编译错误。该项目的 Unity 版本不存在 MaterialDecoratorDrawer 类型，但经验证 Shader.GetPropertyAttributes() API 会返回所有 attribute 字符串，无论是否有对应的 C# Drawer 类。因此直接删除该文件即可。
todos:
  - id: delete-decorator
    content: 删除 MaterialPropertyDrawDecorator.cs 文件以修复编译错误
    status: completed
isProject: false
---

# 修复 MaterialPropertyDrawDecorator.cs 编译错误

## 问题

[MaterialPropertyDrawDecorator.cs](Assets/Editor/Theseus/ShaderGUI/ModuleShaderGUI/Drawers/MaterialPropertyDrawDecorator.cs) 继承了 `MaterialDecoratorDrawer`，但该项目的 Unity 版本中不存在此类型，导致 3 个编译错误：

- CS0246: `MaterialDecoratorDrawer` 类型找不到
- CS0115: `GetPropertyHeight` 无合适基类方法
- CS0115: `OnGUI(Rect)` 无合适基类方法

## 分析

`MaterialPropertyDraw` 仅作为纯数据标记 -- 它的作用是让 `Shader.GetPropertyAttributes(idx)` 返回对应的字符串，供 `Vector4SplitDrawer` 解析。

经验证：`Shader.GetPropertyAttributes()` 是底层 API，会返回 shader Property 上写的所有 `[xxx]` 字符串，**无论是否存在对应的 C# Drawer 类**。因此这个 Decorator 文件完全不需要存在。

## 修复

删除 [MaterialPropertyDrawDecorator.cs](Assets/Editor/Theseus/ShaderGUI/ModuleShaderGUI/Drawers/MaterialPropertyDrawDecorator.cs)。
