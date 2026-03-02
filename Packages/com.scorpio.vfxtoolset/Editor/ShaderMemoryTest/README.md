# Shader Memory Test（ParticleEffect Modular Android 内存测试）

用于对 `Theseus_ParticleEffect_CommonSF_Modular.shader` 在 Android 平台进行 shader 变体内存占用测试：枚举合法关键字组合，生成材质资产与测试场景，打 Android 包后用 Memory Profiler 抓取并分析结果。

**结果输出位置**：所有生成结果（材质 `.mat`、测试场景 `.unity`、SVC `.shadervariants`）均写入**本地工程**（当前 Unity 项目的 `Assets` 目录），不写入包内。默认路径为 `Assets/ShaderMemoryTest/`，可在窗口中修改。

## 为何需要 Shader Variant Collection？

若 Memory Profiler 显示该 shader 仅 ~173 kB，而编辑器编译缓存（Temp/Compiled-*.shader）有数十 MB，说明**构建时大部分变体被 strip 掉了**。Unity 的 shader variant stripping 会剔除未被显式保留的变体。仅靠材质+场景引用，构建分析可能无法保留全部变体。通过生成 **Shader Variant Collection (SVC)** 并加入 **Preloaded Shaders**，可显式保留所有测试变体，使 Memory Profiler 能测到完整 shader 内存。

## 使用步骤

1. **打开窗口**  
   菜单：`Window → Scorpio VFX → Shader Memory Test (ParticleEffect Modular)`。

2. **配置**
   - **Shader Name**：保持默认 `Hidden/Theseus/VFX/ParticleEffect_CommonSF_Modular`。
   - **Max Local Feature Count**：本地 shader feature 最多同时开启数（默认 7）。
   - **Materials Output Dir**：材质 `.mat` 输出目录，须为本地工程 `Assets/` 下路径（如 `Assets/ShaderMemoryTest/Materials`）。
   - **Test Scene Path**：测试场景保存路径，须为本地工程 `Assets/` 下 `.unity` 路径（如 `Assets/ShaderMemoryTest/ShaderMemoryTestScene.unity`）。
   - **SVC Output Path**：Shader Variant Collection 输出路径（如 `Assets/ShaderMemoryTest/ParticleEffectModular_TestVariants.shadervariants`），用于保留变体避免 strip。
   - **Max Material Count**：0 表示不限制；大于 0 时只生成前 N 个或随机采样 N 个（见 Use Random Sample）。
   - **Use Random Sample**：当 Max Material Count > 0 时，为 true 则随机采样，否则取前 N 个。

3. **生成材质**  
   点击 **「1. 生成材质资产」**。会按上述约束枚举组合，为每种组合创建材质并设置关键字，再以 `.mat` 形式保存到配置的输出目录（`AssetDatabase.CreateAsset` + `SaveAssets`），保证序列化并参与后续打包。

4. **生成/更新场景**  
   点击 **「2. 生成/更新测试场景」**。会从材质输出目录加载所有材质，在场景中为每个材质创建一个 Quad 并引用该材质，然后保存场景到配置的 Test Scene Path。若该路径已有场景则会打开并覆盖根节点下的测试物体。

5. **生成 SVC 并加入 Preloaded Shaders**（关键步骤，避免变体被 strip）  
   - 点击 **「2.5 生成 Shader Variant Collection」**：创建 `.shadervariants` 资产，显式包含所有合法关键字组合对应的变体。
   - 点击 **「3. 将 SVC 加入 Preloaded Shaders」**：将 SVC 注册到 Graphics Settings，确保构建时保留这些变体。也可手动在 Edit → Project Settings → Graphics → Preloaded Shaders 中拖入 SVC。

6. **打包**  
   - 在 Build Settings 中切换平台为 Android，将上述测试场景加入 **Scenes In Build**。
   - 执行构建，得到 APK。

7. **抓取与分析**  
   - 在真机或模拟器上运行该 APK，进入测试场景（或确保场景被加载）。
   - 使用 Unity **Memory Profiler**（或 `com.unity.memoryprofiler`）抓取快照。
   - 在快照中查看该 Shader 的 GPU 资源、变体数量，以及相关 Material 的数量与内存。修复后，Memory Profiler 中的 shader 内存应显著大于仅材质引用时的 ~173 kB。

## 关键字与约束

- **全局**：`_COLOR_HDR_`（2 态）。
- **本地**：9 个二选一 + 1 个三选一（`__` / `_FLOW_MAP_ON` / `_NOISE_ON` 互斥）。仅生成「本地开启数 ≤ Max Local Feature Count」的组合，再与 2 种全局态组合得到最终材质数。

## 注意事项

- 材质必须保存为资产（.mat）并 `SaveAssets`，否则不会序列化，打包时不会包含。
- **必须生成 SVC 并加入 Preloaded Shaders**，否则构建会 strip 大部分变体，Memory Profiler 只能看到 173 kB 量级，无法反映完整 shader 内存。
- 54 MB 为编辑器编译缓存（文本形式），与运行时 GPU 内存不可直接对比；修复后 Memory Profiler 数值会增大，但未必达到 54 MB 量级。
- 测试完成后可从 Graphics Settings → Preloaded Shaders 中移除 SVC，以减少正式包体与构建时间。
