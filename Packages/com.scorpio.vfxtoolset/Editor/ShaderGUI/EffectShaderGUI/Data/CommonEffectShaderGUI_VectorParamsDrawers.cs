namespace HeroShowRenderingGUI.VFX
{
    public static class VectorParamsDrawerConfigs
    {
        public static readonly VectorParamsDrawer k_UVParamsDrawer = new VectorParamsDrawer(
            new VectorComponent("贴图U方向流速"),
            new VectorComponent("贴图V方向流速"),
            new FloatSliderComponent("贴图缩放", 0, 10f),
            new FloatSliderComponent("贴图旋转", 0.0f, 720.0f)
        );
        
        public static readonly VectorParamsDrawer s_DouYinEffectParamsDrawer = new VectorParamsDrawer(
            new ToggleComponent("是否开启抖音色效果"),
            new FloatSliderComponent("OffsetX", -1f, 1f),
            new FloatSliderComponent("OffsetY", -1f, 1f),
            new HiddenComponent(string.Empty)
        );

        public static readonly VectorParamsDrawer s_MeshEffectBaseMapTogglesDrawer = new VectorParamsDrawer(
            new ToggleComponent("开启预乘Alpha(禁动画中K开关)"),
            new ToggleComponent("去黑底(禁动画中K开关)"),
            new ToggleComponent("开启极坐标(禁动画中K开关)"),
            new ToggleComponent("切换为2U(禁动画中K开关)")
        );

        public static readonly VectorParamsDrawer s_PsEffectBaseMapTogglesDrawer = new VectorParamsDrawer(
            new ToggleComponent("开启预乘Alpha(禁动画中K开关)"),
            new ToggleComponent("去黑底(禁动画中K开关)"),
            new ToggleComponent("开启极坐标(禁动画中K开关)"),
            new HiddenComponent(string.Empty)
        );

        public static readonly VectorParamsDrawer s_MaskMapTogglesDrawer = new VectorParamsDrawer(
            new ToggleComponent("R通道不影响主贴图Alpha(禁动画中K开关)"),
            new ToggleComponent("R通道不影响混合贴图Alpha(禁动画中K开关)"),
            new ToggleComponent("G通道影响溶解(禁动画中K开关)"),
            new ToggleComponent("B通道影响扰动(禁动画中K开关)")
        );
        
        public static readonly VectorParamsDrawer s_MaskMapParamsDrawer = new VectorParamsDrawer(
            new ToggleComponent("兼容纯Alpha图"),
            new FloatSliderComponent("遮罩强度", 0f, 1.0f),
            new HiddenComponent(string.Empty),
            new HiddenComponent(string.Empty)
        );

        public static readonly VectorParamsDrawer s_MixMapParamsDrawer = new VectorParamsDrawer(
            new ToggleComponent("去黑"),
            new ToggleComponent("开启Noise影响"),
            new ToggleComponent("开启Ramp混色模式"),
            new FloatSliderComponent("混合贴图强度", 0f, 2.0f)
        );
        
        public static readonly VectorParamsDrawer s_FresnelParamsDrawer = new VectorParamsDrawer(
            new ToggleComponent("反向Fresnel Alpha"),
            new FloatSliderComponent("范围", 0.0f, 2.0f),
            new FloatSliderComponent("强度", 0.0f, 1.0f),
            new EnumComponent("混合模式", new []{ "覆盖", "叠加"})
        );

        public static readonly VectorParamsDrawer s_DissolveParamsDrawer = new VectorParamsDrawer(
            new FloatSliderComponent("贴图旋转", 0f, 720f),
            new VectorComponent("溶解强度"),
            new FloatSliderComponent("溶解软硬", 0f, 2f),
            new EnumComponent("边缘叠色模式", new string[]{ "覆盖", "叠加"})
        );

        public static readonly VectorParamsDrawer s_DissolveEdgeParamsDrawer = new VectorParamsDrawer(
            new ToggleComponent("开启溶解边缘叠色(禁动画中K开关)"),
            new FloatSliderComponent("边缘宽度", 0, 0.5f),
            new FloatSliderComponent("边缘软硬", 0, 0.5f),
            new VectorComponent("边缘强度")
        );
        
        public static readonly VectorParamsDrawer s_FlowMapParamsDrawer = new VectorParamsDrawer(
            new VectorComponent("流动速度"),
            new FloatSliderComponent("扰动强度", 0, 1.0f),
            new HiddenComponent(string.Empty),
            new HiddenComponent(string.Empty)
        );
        
        
        
        public static readonly VectorParamsDrawer s_NoiseStrengthParamsDrawer = new VectorParamsDrawer(
            new FloatSliderComponent("第一层扭曲强度U", -2f, 2f),
            new FloatSliderComponent("第一层扭曲强度V", -2f, 2f),
            new FloatSliderComponent("第二层扭曲强度U", -2f, 2f),
            new FloatSliderComponent("第二层扭曲强度V", -2f, 2f)
        );

        public static readonly VectorParamsDrawer s_VertexOffsetParamsDrawer = new VectorParamsDrawer(
            new VectorComponent("VertexScale"),
            new VectorComponent("VertexPower"),
            new FloatSliderComponent("VertexScaleHeightU", 0.0f , 1.0f),
            new FloatSliderComponent("VertexScaleHeightV", 0.0f , 1.0f)
        );

        public static readonly VectorParamsDrawer s_GradientParamsDrawer = new VectorParamsDrawer(
            new FloatSliderComponent("UV权重", 0f, 1.0f),
            new FloatSliderComponent("左侧渐变色权重", 0f, 1.0f),
            new FloatSliderComponent("右侧渐变色权重", 0f, 2.0f),
            new FloatSliderComponent("渐变色权重偏移", -1f, 1.0f)
            );

        public static readonly VectorParamsDrawer s_ColorGradingParamsDrawer = new VectorParamsDrawer(
            new FloatSliderComponent("色相", -0.5f, 0.5f),
            new FloatSliderComponent("饱和度", 0.0f, 2.0f),
            new FloatSliderComponent("对比度", 0.0f, 2.0f),
            new HiddenComponent(string.Empty)
        );
    }
}