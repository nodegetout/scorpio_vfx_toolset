using System.Collections.Generic;
using UnityEngine;

namespace ScorpioEditor.ShaderMemoryTest
{
    public static class MaterialKeywordApplier
    {
        /// <summary>
        /// 从 KeywordCombination 推导出所有启用的关键字名称。
        /// </summary>
        public static string[] GetEnabledKeywords(in KeywordCombination combo)
        {
            var list = new List<string>();
            if (combo.GlobalHdrOn)
                list.Add(ParticleEffectModularKeywords.GlobalHdr);
            for (int i = 0; i < ParticleEffectModularKeywords.LocalBinary.Length; i++)
            {
                if ((combo.Local.BinaryBits & (1u << i)) != 0)
                    list.Add(ParticleEffectModularKeywords.LocalBinary[i]);
            }
            if (combo.Local.FlowMapNoise == ParticleEffectModularKeywords.FlowMapNoiseOption.FlowMap)
                list.Add(ParticleEffectModularKeywords.FlowMapKeyword);
            else if (combo.Local.FlowMapNoise == ParticleEffectModularKeywords.FlowMapNoiseOption.Noise)
                list.Add(ParticleEffectModularKeywords.NoiseKeyword);
            return list.ToArray();
        }

        public static void Apply(Material material, in KeywordCombination combo)
        {
            if (material == null) return;

            material.DisableKeyword(ParticleEffectModularKeywords.GlobalHdr);
            foreach (string kw in ParticleEffectModularKeywords.LocalBinary)
                material.DisableKeyword(kw);
            material.DisableKeyword(ParticleEffectModularKeywords.FlowMapKeyword);
            material.DisableKeyword(ParticleEffectModularKeywords.NoiseKeyword);

            foreach (string kw in GetEnabledKeywords(combo))
                material.EnableKeyword(kw);
        }

        /// <summary>
        /// 格式: "HDR{0|1}_{9位二进制}{F|N|0}"，固定 15 字符。
        /// </summary>
        public static string GetCombinationName(in KeywordCombination combo)
        {
            var sb = new System.Text.StringBuilder(15);
            sb.Append(combo.GlobalHdrOn ? "HDR1_" : "HDR0_");
            for (int i = 0; i < LocalKeywordCombination.BinaryCount; i++)
                sb.Append((combo.Local.BinaryBits & (1u << i)) != 0 ? '1' : '0');
            sb.Append(combo.Local.FlowMapNoise == ParticleEffectModularKeywords.FlowMapNoiseOption.FlowMap ? 'F' :
                     combo.Local.FlowMapNoise == ParticleEffectModularKeywords.FlowMapNoiseOption.Noise ? 'N' : '0');
            return sb.ToString();
        }
    }
}
