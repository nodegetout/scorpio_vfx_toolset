using UnityEngine;

namespace ScorpioEditor.ShaderMemoryTest
{
    public static class MaterialKeywordApplier
    {
        public static void Apply(Material material, in KeywordCombination combo)
        {
            if (material == null) return;

            material.DisableKeyword(ParticleEffectModularKeywords.GlobalHdr);
            if (combo.GlobalHdrOn)
                material.EnableKeyword(ParticleEffectModularKeywords.GlobalHdr);

            foreach (string kw in ParticleEffectModularKeywords.LocalBinary)
                material.DisableKeyword(kw);
            for (int i = 0; i < ParticleEffectModularKeywords.LocalBinary.Length; i++)
            {
                if ((combo.Local.BinaryBits & (1u << i)) != 0)
                    material.EnableKeyword(ParticleEffectModularKeywords.LocalBinary[i]);
            }

            material.DisableKeyword(ParticleEffectModularKeywords.FlowMapKeyword);
            material.DisableKeyword(ParticleEffectModularKeywords.NoiseKeyword);
            if (combo.Local.FlowMapNoise == ParticleEffectModularKeywords.FlowMapNoiseOption.FlowMap)
                material.EnableKeyword(ParticleEffectModularKeywords.FlowMapKeyword);
            else if (combo.Local.FlowMapNoise == ParticleEffectModularKeywords.FlowMapNoiseOption.Noise)
                material.EnableKeyword(ParticleEffectModularKeywords.NoiseKeyword);
        }

        public static string GetCombinationName(in KeywordCombination combo)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(combo.GlobalHdrOn ? "HDR1" : "HDR0");
            sb.Append("_");
            for (int i = 0; i < LocalKeywordCombination.BinaryCount; i++)
                sb.Append((combo.Local.BinaryBits & (1u << i)) != 0 ? "1" : "0");
            sb.Append(combo.Local.FlowMapNoise == ParticleEffectModularKeywords.FlowMapNoiseOption.FlowMap ? "F" :
                     combo.Local.FlowMapNoise == ParticleEffectModularKeywords.FlowMapNoiseOption.Noise ? "N" : "0");
            return sb.ToString();
        }
    }
}
