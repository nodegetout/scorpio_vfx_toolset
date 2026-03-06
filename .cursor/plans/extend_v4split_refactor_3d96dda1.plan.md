---
name: Extend V4Split Refactor
overview: 扩展 Vector4Split 重构范围：让 TwoVector2/Vector3Float 模式也支持 MaterialPropertyDraw 新语法，并将 4 个 shader 中所有剩余的旧语法 Vector4Split 全部迁移。
todos:
  - id: fix-twovec2-mesh
    content: 迁移 Theseus_MeshEffect_CommonSF.shader 中残留的 TwoVector2 _DissolveNoiseParam 到新语法
    status: completed
  - id: fix-twovec2-particle
    content: 迁移 Theseus_ParticleEffect_CommonSF.shader 中残留的 TwoVector2 _DissolveNoiseParam 到新语法
    status: completed
  - id: migrate-mesh-modular
    content: 全量迁移 Theseus_MeshEffect_CommonSF_Modular.shader 所有 Vector4Split 到新语法
    status: completed
  - id: migrate-particle-modular
    content: 全量迁移 Theseus_ParticleEffect_CommonSF_Modular.shader 所有 Vector4Split 到新语法
    status: completed
isProject: false
---

# 扩展 Vector4Split 重构：TwoVector2 / Vector3Float + Modular Shader

## 现状

C# 端的 `MaterialPropertyDrawParser.TryParse` 对段数的校验为：

```csharp
int expectedCount = mode == SplitMode.FourFloats ? 4 : 2;
```

这意味着 TwoVector2 期望 2 个 `MaterialPropertyDraw`，Vector3Float 也期望 2 个。这已经是正确的 -- 无需修改段数逻辑。

但 `Vector4SplitDrawer` 的无参构造 `Vector4SplitDrawer()` 硬编码为 `FourFloats`。当 TwoVector2/Vector3Float 使用新语法时，需要能在 attribute 中指定 SplitMode，否则都会被当作 FourFloats。

## 问题

TwoVector2 和 Vector3Float 使用新语法时，需要保留 SplitMode 参数：

```hlsl
// 旧语法
[Vector4Split(TwoVector2)]_DissolveNoiseParam("溶解纹理参数 ## 溶解纹理Tiling @ 溶解纹理UV流速", Vector) = (1,1,0,0)

// 新语法 -- 仍需传 SplitMode
[MaterialPropertyDraw(溶解纹理Tiling, Float, 1)]
[MaterialPropertyDraw(溶解纹理UV流速, Float, 0)]
[Vector4Split(TwoVector2)]_DissolveNoiseParam("溶解纹理参数", Vector) = (1,1,0,0)
```

当前 `Vector4SplitDrawer(string mode)` 构造函数已经存在，所以 `[Vector4Split(TwoVector2)]` 配合 `[MaterialPropertyDraw]` 已经可以工作 -- `TryGetValidated` 会先检测到 `MaterialPropertyDraw` attribute 并优先使用新路径解析。

结论：**C# 端无需修改**。现有代码已经完全支持 TwoVector2 和 Vector3Float 的新语法。

## 需要迁移的 Shader

### 1. 已迁移 shader 中残留的 TwoVector2 旧语法

涉及 [Theseus_MeshEffect_CommonSF.shader](Assets/Shaders/ShaderTheseus/Effect/Theseus_MeshEffect_CommonSF.shader) L108 和 [Theseus_ParticleEffect_CommonSF.shader](Assets/Shaders/ShaderTheseus/Effect/Theseus_ParticleEffect_CommonSF.shader) L110：

```hlsl
// 旧：
[Vector4Split(TwoVector2)]_DissolveNoiseParam("溶解纹理参数 ## 溶解纹理Tiling @ 溶解纹理UV流速", Vector) = (1,1,0,0)

// 新：
[MaterialPropertyDraw(溶解纹理Tiling, Float, 1)]
[MaterialPropertyDraw(溶解纹理UV流速, Float, 0)]
[Vector4Split(TwoVector2)]_DissolveNoiseParam("溶解纹理参数", Vector) = (1,1,0,0)
```

### 2. Modular shader 全量迁移

[Theseus_MeshEffect_CommonSF_Modular.shader](Assets/Shaders/ShaderTheseus/Effect/Theseus_MeshEffect_CommonSF_Modular.shader) 和 [Theseus_ParticleEffect_CommonSF_Modular.shader](Assets/Shaders/ShaderTheseus/Effect/Theseus_ParticleEffect_CommonSF_Modular.shader) 中所有 `Vector4Split` 属性均使用旧语法，需要全部迁移为新语法。

两个 Modular shader 中需要迁移的属性清单（与非 Modular 版本完全对应）：

- `_BaseMapToggles` -- FourFloats, 4 个 Toggle
- `_DouYinEffectParams` -- FourFloats, Toggle + Range + Range + Hidden
- `_BaseUVParams` -- FourFloats + ModuleEnd, Float + Float + Range + Range
- `_MaskUVParams` -- FourFloats, Float + Float + Range + Range
- `_MaskMapParams` -- FourFloats, Toggle + Range + Hidden + Hidden
- `_MaskMapToggles` -- FourFloats + ModuleEnd, 4 个 Toggle
- `_MixMapParams` -- FourFloats, 3 Toggle + Range
- `_MixBaseMapUVParams` -- FourFloats + ModuleEnd, Float + Float + Range + Range
- `_DissolveNoiseParam` -- TwoVector2, 2 个 Float 标签
- `_DissolveControlParams` -- FourFloats, 4 个 Range
- `_FlowMapParams` -- FourFloats + ModuleEnd, Float + Range + Hidden + Hidden
- `_NoiseStrength` -- FourFloats, 4 个 Range
- `_FresnelParams` -- FourFloats + ModuleEnd, Toggle + Range + Range + Toggle
- `_VertexOffsetParams` -- FourFloats, Float + Float + Range + Range
- `_GradientParams` -- FourFloats + ModuleEnd, 4 个 Range
- `_ColorGradingParams` -- FourFloats, 3 Range + Hidden

注意 Modular 版本的某些默认值与非 Modular 版本略有不同（如 `_DissolveControlParams`、`_FresnelParams`），迁移时需保持原值。
