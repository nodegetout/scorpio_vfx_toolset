using System;
using System.Collections.Generic;

namespace ScorpioEditor.ShaderMemoryTest
{
    /// <summary>
    /// 本地关键字组合：9 个布尔 + 三选一（FlowMap/Noise/None）。
    /// </summary>
    public struct LocalKeywordCombination
    {
        public const int BinaryCount = 9;

        public uint BinaryBits;
        public ParticleEffectModularKeywords.FlowMapNoiseOption FlowMapNoise;

        public int EnabledCount
        {
            get
            {
                int c = PopCount(BinaryBits);
                if (FlowMapNoise != ParticleEffectModularKeywords.FlowMapNoiseOption.None)
                    c += 1;
                return c;
            }
        }

        public bool IsValid(int maxLocalFeatureCount)
        {
            return EnabledCount <= maxLocalFeatureCount;
        }

        private static int PopCount(uint v)
        {
            int n = 0;
            while (v != 0) { n += (int)(v & 1); v >>= 1; }
            return n;
        }
    }

    /// <summary>
    /// 完整关键字组合：全局 HDR 开/关 + 本地组合。
    /// </summary>
    public struct KeywordCombination
    {
        public bool GlobalHdrOn;
        public LocalKeywordCombination Local;
    }

    /// <summary>
    /// 枚举所有满足“本地开启数 ≤ maxLocalFeatureCount”的 (全局, 本地) 组合。
    /// </summary>
    public static class KeywordCombinationGenerator
    {
        public static List<KeywordCombination> EnumerateAll(int maxLocalFeatureCount)
        {
            var list = new List<KeywordCombination>();
            uint binaryMax = 1u << LocalKeywordCombination.BinaryCount;
            var flowOptions = (ParticleEffectModularKeywords.FlowMapNoiseOption[])Enum.GetValues(typeof(ParticleEffectModularKeywords.FlowMapNoiseOption));

            for (uint b = 0; b < binaryMax; b++)
            {
                for (int f = 0; f < flowOptions.Length; f++)
                {
                    var local = new LocalKeywordCombination
                    {
                        BinaryBits = b,
                        FlowMapNoise = flowOptions[f]
                    };
                    if (!local.IsValid(maxLocalFeatureCount))
                        continue;

                    list.Add(new KeywordCombination { GlobalHdrOn = false, Local = local });
                    list.Add(new KeywordCombination { GlobalHdrOn = true, Local = local });
                }
            }

            return list;
        }

        /// <summary>
        /// 若 maxCount > 0，截断或随机采样到 maxCount 条（随机种子可固定以便复现）。
        /// </summary>
        public static List<KeywordCombination> EnumerateAll(int maxLocalFeatureCount, int maxCount, bool useRandomSample, int randomSeed = 42)
        {
            var full = EnumerateAll(maxLocalFeatureCount);
            if (maxCount <= 0 || full.Count <= maxCount)
                return full;

            if (!useRandomSample)
                return full.GetRange(0, maxCount);

            var rng = new Random(randomSeed);
            var indices = new List<int>(full.Count);
            for (int i = 0; i < full.Count; i++) indices.Add(i);
            for (int i = indices.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (indices[i], indices[j]) = (indices[j], indices[i]);
            }
            var result = new List<KeywordCombination>(maxCount);
            for (int i = 0; i < maxCount; i++)
                result.Add(full[indices[i]]);
            return result;
        }
    }
}
