---
name: Fresnel Enum 叠加模式
overview: 将 _FresnelParams.w 分量在 ShaderGUI 上从 Toggle 改为 Enum 下拉框显示（Multiply=0, Add=1），需要修改 4 个 CommonSF shader 文件中的属性声明。
todos:
  - id: update-mesh-sf
    content: 修改 Theseus_MeshEffect_CommonSF.shader 第119-120行：Toggle->Enum + 添加 {Multiply=0, Add=1}
    status: completed
  - id: update-particle-sf
    content: 修改 Theseus_ParticleEffect_CommonSF.shader 第118-119行：Toggle->Enum + 添加 {Multiply=0, Add=1}
    status: completed
  - id: update-mesh-modular
    content: 修改 Theseus_MeshEffect_CommonSF_Modular.shader 第118-119行：Toggle->Enum + 添加 {Multiply=0, Add=1}
    status: completed
  - id: update-particle-modular
    content: 修改 Theseus_ParticleEffect_CommonSF_Modular.shader 第121-122行：Toggle->Enum + 添加 {Multiply=0, Add=1}
    status: completed
isProject: false
---

# Fresnel 叠加模式改为 Enum 下拉框

## 现状分析

当前 `_FresnelParams` 的第4个分量（w）使用 `Toggle` 类型绘制，在 ShaderGUI 上显示为勾选框。需要改为 `Enum` 下拉框，显示 "Multiply" 和 "Add" 两个选项。

现有的 `Vector4SplitDrawer` 框架已经完整支持 Enum 类型：

- 构造函数参数传 `Enum`（替代 `Toggle`）
- displayName 中用 `{Multiply=0, Add=1}` 语法指定枚举选项

着色器混合逻辑（[VFX_CommonShaderFeaturePass.hlsl](Assets/Shaders/ShaderTheseus/Effect/VFX_CommonShaderFeaturePass.hlsl) 第300行 `step(0.5, _FresnelParams.w)`）无需修改，因为 0/1 值的语义不变。

## 修改内容

4 个 shader 文件中 `_FresnelParams` 属性声明的修改（修改方式完全一致）：

**修改前：**

```hlsl
[Vector4Split(Toggle, Range, Range, Toggle)]
_FresnelParams("Fresnel参数 ## 反向Fresnel Alpha | 范围(0, 2) | 强度(0, 1) | 叠加模式", Vector) = (...)
```

**修改后：**

```hlsl
[Vector4Split(Toggle, Range, Range, Enum)]
_FresnelParams("Fresnel参数 ## 反向Fresnel Alpha | 范围(0, 2) | 强度(0, 1) | 叠加模式{Multiply=0, Add=1}", Vector) = (...)
```

## 涉及文件

- [Theseus_MeshEffect_CommonSF.shader](Assets/Shaders/ShaderTheseus/Effect/Theseus_MeshEffect_CommonSF.shader) 第119-120行
- [Theseus_ParticleEffect_CommonSF.shader](Assets/Shaders/ShaderTheseus/Effect/Theseus_ParticleEffect_CommonSF.shader) 第118-119行
- [Theseus_MeshEffect_CommonSF_Modular.shader](Assets/Shaders/ShaderTheseus/Effect/Theseus_MeshEffect_CommonSF_Modular.shader) 第118-119行
- [Theseus_ParticleEffect_CommonSF_Modular.shader](Assets/Shaders/ShaderTheseus/Effect/Theseus_ParticleEffect_CommonSF_Modular.shader) 第121-122行

## 不修改的文件

- `VFX_CommonShaderFeaturePass.hlsl` / `VFX_CommonObsoletePass.hlsl` — 着色器混合逻辑不变（`step(0.5, _FresnelParams.w)` 兼容 0/1 枚举值）
- `VFX_CommonInput.hlsl` — 变量声明不变
- `FloatComponentDrawers.cs` / `Vector4SplitDrawerData.cs` — Enum 绘制能力已完善，无需改动
- `Theseus_ParticleEffect_Common.shader` / `Theseus_ParticleEffect_CommonDistortion.shader` — 旧版 shader，没有使用 Vector4Split，不在此次改动范围

