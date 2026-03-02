using UnityEngine;

namespace ScorpioEditor.ShaderMemoryTest
{
    [CreateAssetMenu(fileName = "ShaderMemoryTestConfig", menuName = "Scorpio VFX/Shader Memory Test Config")]
    public class ShaderMemoryTestConfig : ScriptableObject
    {
        [Tooltip("Shader 路径，须与 Theseus_ParticleEffect_CommonSF_Modular 一致")]
        public string ShaderName = "Hidden/Theseus/VFX/ParticleEffect_CommonSF_Modular";

        [Tooltip("本地 shader feature 最多同时开启个数")]
        [Range(1, 10)]
        public int MaxLocalFeatureCount = 7;

        [Tooltip("材质资产输出目录，须为本地工程 Assets 下路径（如 Assets/ShaderMemoryTest/Materials），结果写入当前 Unity 项目而非包内")]
        public string MaterialsOutputDir = "Assets/ShaderMemoryTest/Materials";

        [Tooltip("测试场景保存路径，须为本地工程 Assets 下 .unity 路径（如 Assets/ShaderMemoryTest/ShaderMemoryTestScene.unity）")]
        public string TestScenePath = "Assets/ShaderMemoryTest/ShaderMemoryTestScene.unity";

        [Tooltip("Shader Variant Collection 输出路径，用于保留变体避免构建 strip（如 Assets/ShaderMemoryTest/ParticleEffectModular_TestVariants.shadervariants）")]
        public string SvcOutputPath = "Assets/ShaderMemoryTest/ParticleEffectModular_TestVariants.shadervariants";

        [Tooltip("最大生成材质数量，0 表示不限制")]
        public int MaxMaterialCount = 0;

        [Tooltip("当 MaxMaterialCount > 0 时，是否从全组合中随机采样（否则取前 N 个）")]
        public bool UseRandomSample = false;

        [Tooltip("随机采样时使用的种子（便于复现）")]
        public int RandomSeed = 42;
    }
}
