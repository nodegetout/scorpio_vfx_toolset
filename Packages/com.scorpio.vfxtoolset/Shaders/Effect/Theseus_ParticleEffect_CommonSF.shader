Shader "Hidden/Theseus/VFX/ParticleEffect_CommonSF"
{
    Properties
    {
    	[Space(5)]
		[Enum(Add,1,Blend,10)]_DstBlend("混合模式", Float) = 10
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull",Float) = 0
		[Enum(Off, 0, On, 1)]_ZWrite("深度测试", Float) = 0
		[Enum(On, 0,Off, 4)]_ZTest("总是最前", Float) = 4
	    
		[Header(CustomData ... coord1.xy__DiffUV ... coord1.zw__Mask ... coord2.xy__DissolveInstensity  ... coord2.zw__DissolveUV)]
    	[Space(10)]
    	[Toggle(_REQUIRE_CUSTOMDATA)]_RequireCustomData("开启CustomData", Float) = 0
    	[Toggle(_ENABLE_SCREEN_UV)]_EnableScreenUV("切换屏幕UV", Float) = 0
	    
    	[Space(5)]
		[MainTexture]_BaseMap("主贴图", 2D) = "white" {}
    	_BaseMapToggles("BaseMapToggles", Vector) = (0,0,0,0)
		[HDR]_BaseColor("TintColor", Color) = (1,1,1,1)
		[HDR]_BaseFrontColor("FrontTintColor", Color) = (1,1,1,1)
		_FrontIntensity("FrontIntensity",Range(0, 10))=0
    	_DouYinEffectParams("DouYinEffectParams", Vector) = (0, 0.04, -0.04, 0)
    	_BaseUVParams("BaseUVParams", Vector) = (0,0,1,0)
	    
    	[Space(5)]
		_Mask("Mask", 2D) = "white" {}
    	_MaskUVParams("MaskUVParams", Vector) = (0,0,1,0)
    	_MaskMapParams("MaskUVParams", Vector) = (0,1,0,0)
    	[Toggle]_EnableMaskMapPolarUV("开启极坐标",Float)=0
    	_MaskMapToggles("MaskMapToggles", Vector) = (0,0,0,0)
	    
		[Space(5)]
		[Toggle]_DoubleSideOn("开启双面",Float)=0
		_BackIntensity("BackIntensity",Range(0, 10)) = 0
		[HDR]_DiffuseBackColor("TextureBackColor", Color) = (1,1,1,1)

		[Space(5)]
		[Toggle(_MIX_BASE_ON)]_MixBaseOn("开启混合贴图",Float)=0
		_MixDiffuse("和主贴图混合的贴图", 2D) = "black" {}
    	[HDR]_MixTintColor("MixTintColor", Color) = (1,1,1,1)
    	[Toggle]_MixBasePolarUVOn("开启极坐标",Float)=0
		_MixMapParams("MixMapParams", Vector) = (0, 0, 0, 1)
    	_MixBaseMapUVParams("MixBaseMapUVParams", Vector) = (0,0,1,0)

		[Space(5)]
		[Toggle(_FRESNEL_ON)] _FresnelOn("EnableFresnel", Float) = 0
    	_FresnelMap("FresnelMap", 2D) = "white" {}
		[Toggle] _FresnelMapUse2U("FresnelMapUse2U", Float) = 0
		_FresnelParams("FresnelParams", Vector) = (0, 0, 1, 0)
        [HDR]_FresnelColor("FresnelColor", Color) = (1,1,1,1)

		[Space(5)]
    	[Toggle(_DISSOLVE_ON)] _DissolveOn("EnableDissolve", Float) = 0
		_DissolveTex("溶解和Noise", 2D) = "white" {}
		_DissolveParams("DissolveParams", Vector) = (0, -1, 0, 0)
    	_DissolveEdgeParams("DissolveEdgeParams", Vector) = (1, 0.001, 0, 1)
		[HDR]_DissolveColor("溶解边缘_颜色",Color) = (1,1,1,1)
    	
    	[Space(5)]
    	[Toggle(_FLOW_MAP_ON)] _FlowMapOn("EnableFlowMap", Float) = 0
		_FlowMap("FlowMap", 2D) = "white" {}
		_FlowMapParams("FlowMapParams", Vector) = (0.5, 1, 0, 0)
		
		[Space(5)]
    	[Toggle(_NOISE_ON)] _NoiseOn("EnableNoise", Float) = 0
		[Toggle]_NoiseUnEffectDiff("Noise不影响主贴图",Float) = 0
    	[Toggle] _DualDirectionNoise("开启双层Noise效果(禁动画中K开关)", Float) = 0
    	_NoiseTex1("Layer1 Noise", 2D) = "black" {}
    	_NoiseTex2("Layer2 Noise", 2D) = "black" {}
		_NoiseStrength("扭曲强度", Vector)=(0.5, 0.5, 0.5, 0.5)
		_GChannel("G通xy控Tiling zw控速度",Vector) = (1,0,-1,0)

    	[Space(5)]
		[Toggle(_ENABLE_VERTEX_OFFSET)]_EnableVertexOffset("开启噪声偏移(禁动画中K开关)", Float) = 0
    	_VertexOffsetNoiseMap("VertexOffsetNoiseMap", 2D) = "black" {}
    	[Enum(Normal,0,Vertex,1)]_MotionDir("运动方向",float)=0
		_VertexDir("VertexDir", Vector) = (0,0,0,0)
		_VertexOffsetParams("VertexOffsetParams", Vector) = (0, 1, 1,0)
		_VertexMotionSpeed("VertexMotionSpeed", Vector) = (0,0,0,0)

    	[Space(5)]
		[Toggle(_ENABLE_PLANAR_SOFT_PARTICLE)]_EnablePlanarSoftParticle("开启软粒子(禁动画中K开关)", Float) = 0
    	_ContactRange("渐变范围", Range(0.001, 1)) = 0.5
    	_HorizontalPlaneY("水平面Y值", Float) = 0
		
		[Space(5)]
		[Toggle(_GRADIENT_ON)]_GradientOn("左右渐变颜色开关(禁动画中K开关)",Float) = 0
		[Toggle]_GradientSameDiffOn("左右渐变开启Diff相同UV(禁动画中K开关)",Float) = 0
		_LeftColor("左侧渐变色",Color) = (1,1,1,1)
		_RightColor("右侧渐变色",Color) = (1,1,1,1)
    	_GradientParams("GradientParams", Vector) = (0, 0, 1, 0)
		
		[Space(5)]
		[Toggle(_COLOUR_ON)]_ColourOn("色彩开关(禁动画中K开关)",Float) = 0
		_ColorGradingParams("ColorGradingParams", Vector) = (0, 1, 1, 0)
		_SaturationRightColor("灰度渐变亮色",Color)=(1,1,1,1)
		_SaturationLeftColor("灰度渐变暗色",Color)=(1,1,1,1)
		_SaturationRightColorWeights("灰度渐变亮色权重",Range(0.5,1))=1
		_SaturationLeftColorWeights("灰度渐变暗色权重",Range(0,0.5))=0

    	[Space(5)]
		[Enum(UnityEngine.Rendering.CompareFunction)]_StencilComp("StencilComp",Float)=8
		[Enum(UnityEngine.Rendering.StencilOp)]_StencilPass("StencilPass",Float)=0
		[Enum(UnityEngine.Rendering.StencilOp)]_StencilFail("StencilFail",Float)=0
		_StencilRef("StencilRef",float)=0
    }
    SubShader
    {
        Tags
        {
            "IgnoreProjector" = "True"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        Pass
        {
            Blend SrcAlpha [_DstBlend]
            
			Cull [_CullMode]
			ZWrite [_ZWrite]
			ZTest[_ZTest]
			
			Stencil
			{
				Ref [_StencilRef]
				Comp [_StencilComp]
				Pass [_StencilPass]
				Fail [_StencilFail]
			}
            
            HLSLPROGRAM
            #pragma vertex   Vertex
            #pragma fragment Fragment

            // global keywords
			#pragma multi_compile __ _COLOR_HDR_

            // local keywords
			#pragma shader_feature_local _REQUIRE_CUSTOMDATA
			#pragma shader_feature_local _ENABLE_SCREEN_UV
			#pragma shader_feature_local _MIX_BASE_ON
			#pragma shader_feature_local _ENABLE_VERTEX_OFFSET
            #pragma shader_feature_local _DISSOLVE_ON
			#pragma shader_feature_local _FRESNEL_ON
			#pragma shader_feature_local __ _FLOW_MAP_ON _NOISE_ON
			#pragma shader_feature_local _ENABLE_PLANAR_SOFT_PARTICLE
			#pragma shader_feature_local _GRADIENT_ON
			#pragma shader_feature_local _COLOUR_ON
            
            #define VERTEX_REQUIRE_VERTEXCOLOR
            #define FRAGMENT_REQUIRE_VERTEXCOLOR

            #if defined(_FRESNEL_ON) | defined(_ENABLE_PLANAR_SOFT_PARTICLE)
				#define REQUIRE_POSITIONWS
            #endif
            
            #define FRAGMENT_REQUIRE_UV1
            
            #define VERTEX_REQUIRE_UV1
            #if defined(_REQUIRE_CUSTOMDATA)
				#if defined(_DISSOLVE_ON)
				    #define VERTEX_REQUIRE_UV2
				    #define FRAGMENT_REQUIRE_UV2
				#endif
			#endif
            
            #include "Packages/com.scorpio.vfxtoolset/Shaders/Effect/VFXCore.hlsl"
            #include "Packages/com.scorpio.vfxtoolset/Shaders/Effect/VFX_CommonInput.hlsl"
            #include "Packages/com.scorpio.vfxtoolset/Shaders/Effect/VFX_CommonModule.hlsl"
            #include "Packages/com.scorpio.vfxtoolset/Shaders/Effect/VFX_CommonShaderFeaturePass.hlsl"
            
            ENDHLSL
        }
    }
CustomEditor "HeroShowRenderingGUI.VFX.ShaderFeatureCommonEffectShaderGUI"
}