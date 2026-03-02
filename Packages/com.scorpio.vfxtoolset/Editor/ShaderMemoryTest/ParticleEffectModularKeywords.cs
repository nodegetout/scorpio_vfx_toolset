namespace ScorpioEditor.ShaderMemoryTest
{
    /// <summary>
    /// 与 Theseus_ParticleEffect_CommonSF_Modular.shader 一致的全局/本地关键字定义。
    /// </summary>
    public static class ParticleEffectModularKeywords
    {
        public const string GlobalHdr = "_COLOR_HDR_";

        public static readonly string[] LocalBinary =
        {
            "_REQUIRE_CUSTOMDATA",
            "_ENABLE_SCREEN_UV",
            "_MIX_BASE_ON",
            "_ENABLE_VERTEX_OFFSET",
            "_FALLOFF_DISSOLVE_ON",
            "_GRADIENT_ON",
            "_COLOUR_ON",
            "_FRESNEL_ON",
            "_ENABLE_PLANAR_SOFT_PARTICLE"
        };

        /// <summary>三选一：None = 不启用，FlowMap = _FLOW_MAP_ON，Noise = _NOISE_ON（互斥）</summary>
        public enum FlowMapNoiseOption
        {
            None = 0,
            FlowMap = 1,
            Noise = 2
        }

        public const string FlowMapKeyword = "_FLOW_MAP_ON";
        public const string NoiseKeyword = "_NOISE_ON";
    }
}
