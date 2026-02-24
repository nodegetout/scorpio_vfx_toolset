using System.Collections.Generic;

namespace HeroShowRenderingGUI.VFX
{
    public static class PropertyDrawerFuncMap
    {
        public static readonly Dictionary<PropertyType, VectorParamsDrawer> k_CommonEffectShaderPropertyFuncMap = new Dictionary<PropertyType, VectorParamsDrawer>()
        {
            // { PropertyType.UVParamsProperty,           VectorParamsDrawerConfigs.s_UVParamsDrawer},
            { PropertyType.DouYinEffectParamsProperty,  VectorParamsDrawerConfigs.s_DouYinEffectParamsDrawer },
            { PropertyType.MeshBaseMapTogglesProperty,  VectorParamsDrawerConfigs.s_MeshEffectBaseMapTogglesDrawer},
            { PropertyType.PsBaseMapTogglesProperty,    VectorParamsDrawerConfigs.s_PsEffectBaseMapTogglesDrawer },
            { PropertyType.MaskMapTogglesProperty,      VectorParamsDrawerConfigs.s_MaskMapTogglesDrawer },
            { PropertyType.MaskMapParamsProperty,        VectorParamsDrawerConfigs.s_MaskMapParamsDrawer },
            { PropertyType.MixMapParamsProperty,        VectorParamsDrawerConfigs.s_MixMapParamsDrawer },
            { PropertyType.FresnelParamsProperty,       VectorParamsDrawerConfigs.s_FresnelParamsDrawer },
            { PropertyType.DissolveParamsProperty,      VectorParamsDrawerConfigs.s_DissolveParamsDrawer },
            { PropertyType.DissolveEdgeParamsProperty,  VectorParamsDrawerConfigs.s_DissolveEdgeParamsDrawer },
            { PropertyType.FlowMapParamsProperty,       VectorParamsDrawerConfigs.s_FlowMapParamsDrawer },
            { PropertyType.NoiseStrengthParamsProperty, VectorParamsDrawerConfigs.s_NoiseStrengthParamsDrawer },
            { PropertyType.VertexOffsetParamsProperty,  VectorParamsDrawerConfigs.s_VertexOffsetParamsDrawer },
            { PropertyType.GradientParamsProperty,      VectorParamsDrawerConfigs.s_GradientParamsDrawer },
            { PropertyType.ColorGradingParamsProperty,  VectorParamsDrawerConfigs.s_ColorGradingParamsDrawer }
        };
    }
}