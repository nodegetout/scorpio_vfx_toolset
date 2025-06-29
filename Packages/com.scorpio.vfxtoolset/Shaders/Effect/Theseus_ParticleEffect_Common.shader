Shader "Theseus/VFX/ParticleEffect_Common"
{
    Properties
    {
    	//  #   GroupStart:合并设置_MergeSettings
    	//  #   Enum_DstBlend:混合模式:_DstBlend:Add|Blend
    	[Space(5)]
		[Enum(Add,1,Blend,10)]_DstBlend("混合模式", Float) = 10
    	//  #   Enum_CullingMode:剔除模式:_CullMode:CullingOff|FrontCulling|BackCulling
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull",Float) = 0
    	//  #   Enum_ZWrite:深度测试:_ZWrite:Off|On
		[Enum(Off, 0, On, 1)]_ZWrite("深度测试", Float) = 0
    	//  #   Enum_ZTest:总是最前:_ZTest:Off|On
		[Enum(On, 0,Off, 4)]_ZTest("总是最前", Float) = 4
    	//  #   GroupEnd
		
    	//  #   GroupStart:UV模式设置_UVModeSettings
    	[Space(5)]
		[Warning(coord1.xy_DiffUV __ coord1.zw_Mask __ coord2.xy_DissolveInstensity  __ coord2.zw_DissolveUV)]
    	//  #   Toggle:KeyWords:是否启用CustomData:_REQUIRE_CUSTOMDATA
		[Toggle(_REQUIRE_CUSTOMDATA)]_RequireCustomData("开启CustomData",Float)=0
    	//  #   Toggle:KeyWords:是否启用屏幕UV:_ENABLE_SCREEN_UV
    	[Toggle(_ENABLE_SCREEN_UV)]_EnableScreenUV("切换屏幕UV", Float) = 0
    	//  #   GroupEnd
    	
    	//  #   GroupStart:主图设置_BaseMapSettings
    	[Space(5)]
		[MainTexture]_BaseMap("主贴图", 2D) = "white" {}
    	//  #   Toggle:Uniform:预乘Alpha:_UnMult
		[Toggle(UnMult)]_UnMult("预乘Alpha",Float)=0
    	[Toggle]_BaseMultiplyLuminance("去黑",Float)=0
    	[Toggle]_EnableBaseMapPolarUV("开启极坐标",Float)=0
		_BaseColor("TintColor", Color) = (1,1,1,1)
		_FrontIntensity("FrontIntensity",Range(0, 2))=1
    	_BaseUVParams("BaseUVParams", Vector) = (0,0,1,0)
    	//  #   GroupEnd
    	
    	//  #   GroupStart:遮罩设置_MaskSettings
    	[Space(5)]
		_Mask("Mask", 2D) = "white" {}
    	_MaskUVParams("MaskUVParams", Vector) = (0,0,1,0)
    	[Toggle]_EnableMaskMapPolarUV("开启极坐标",Float)=0
    	[Toggle]_MaskNotEffectDiff("MaskNotEffectDiff", Float) = 0
		[Toggle]_MaskedDissolve("遮罩G通道影响溶解(禁动画中K开关)", Float) = 0
		[Toggle]_EffectByMask("Mask影响Noise(禁动画中K开关)", Float)=0
    	//  #   GroupEnd
    	
    	//  #   GroupStart:双面设置_DoubleSideSettings
		[Space(5)]
		[Toggle(_DOUBLE_SIDE_ON)]_DoubleSideOn("开启双面",Float)=0
		_BackIntensity("BackIntensity",Range(0, 2)) = 1
		_DiffuseBackColor("TextureBackColor", Color) = (1,1,1,1)
    	//  #   GroupEnd

    	//  #   GroupStart:混合贴图设置_MixMapSettings
		[Space(5)]
		[Toggle(_MIX_BASE_ON)]_MixBaseOn("开启混合贴图",Float)=0
		_MixDiffuse("和主贴图混合的贴图", 2D) = "white" {}
		[Toggle]_MixMultiplyLuminance("去黑",Float)=0
		[Toggle]_EnableRampMode("Ramp模式",Float)=0
		_MixDiffusePower("混合贴图强度", Range(0, 1)) = 1
		_MixRampUVOffset("Ramp混合UV偏移", Range(-1, 1)) = 1
    	_MixBaseMapUVParams("MixBaseMapUVParams", Vector) = (0,0,1,0)
    	//  #   GroupEnd
    			
    	//  #   GroupStart:菲涅尔设置_FresnelSettings
		[Space(5)]
		[Toggle(_FRESNEL_ON)] _FresnelOn("EnableFresnel", Float) = 0
    	[Toggle]_TransparentFresnelPart("TransparentFresnelPart", Float) = 0
		_FresnelPower("FresnelPower", Range(0, 20)) = 0
        _FresnelScale("FresnelScale", Range(0, 20))=1
        [HDR]_FresnelColor("FresnelColor", Color) = (0,0,0,1)
    	//  #   GroupEnd
		
    	//  #   GroupStart:溶解扰动设置_DissolveNoiseSettings
		[Space(5)]
    	[Toggle(_DISSOLVE_NOISE_ON)] _DissolveNoiseOn("EnableDissolveNoise", Float) = 0
		_DissolveTex("溶解和Noise", 2D) = "white" {}
		_DissolveAngle("溶解和Noise旋转", Range( 0 , 720)) = 0
		_DissolveStep("溶解",Float) = -1
		_SoftSize("溶解软硬",Range(0,2)) = 0
		[Toggle]_DissolveOutlineOn("溶解边缘叠色",Float) = 1
		_DissolveOutlineWidth("溶解边缘",Range(0,0.5)) = 0.001
		_DissolveOutlineSoft("溶解边缘_软硬",Range(0,0.5)) = 0
	    _DissolveColorPW("溶解边缘_强度",Float) = 1
		_DissolveColor("溶解边缘_颜色",Color) = (1,1,1,1)
		
		[Space(5)]
		[Toggle]_NoiseUnEffectDiff("Noise不影响主贴图",Float) = 0
	    _NoiseStrength("扭曲强度",Range(-2, 2))=0
		[Space(5)]
    	[Toggle(_DUAL_LAYER_NOISE)] _DualDirectionNoise("开启双层Noise效果(禁动画中K开关)", Float) = 0
		_NoiseTex2("Layer2 Noise", 2D) = "black" {}
		_NoiseStrength1("扭曲强度1",Range(-2, 2))=0
		_GChannel("G通xy控Tiling zw控速度",Vector) = (1,1,0,0)
    	//  #   GroupEnd
    	
    	//  #   GroupStart:顶点扰动设置_VertexOffsetSettings
    	[Space(5)]
		[Toggle(_ENABLE_VERTEX_OFFSET)]_ToggleVertexOffset("开启噪声偏移(禁动画中K开关)", Float) = 0
    	_VertexOffsetNoiseMap("VertexOffsetNoiseMap", 2D) = "black" {}
		_VONoiseTiling("偏移噪声Tiling(xy)与Offset(zw)", Vector) = (5,5,0,0)
        _VOSpeedParams("偏移噪声UV移动速度", Vector) = (1,0,1,1)
        _VOScale("噪声偏移缩放xyz轴", Vector) = (0,0,0,0)
    	//  #   GroupEnd
		
    	//  #   GroupStart:颜色渐变设置_GradientSettings
		[Space(5)]
		[Toggle(_GRADIENT_ON)]_GradientOn("左右渐变颜色开关(禁动画中K开关)",Float) = 0
		[Toggle]_GradientSameDiffOn("左右渐变开启Diff相同UV(禁动画中K开关)",Float) = 0
		_LeftColor("左侧渐变色",Color) = (1,1,1,1)
		_RightColor("右侧渐变色",Color) = (1,1,1,1)
		_UVWeights("UV权重",Range(0,1)) = 0
		_LeftWeights("左侧渐变色权重",Range(0,1)) = 0
		_RightWeights("右侧渐变色权重",Range(0,1)) = 1
		_Gradient("渐变色权重偏移",Range(-1,1)) = 0
    	//  #   GroupEnd
		
    	//  #   GroupStart:调色设置_ColorGradingSettings
		[Space(5)]
		[Toggle(_COLOUR_ON)]_ColourOn("色彩开关(禁动画中K开关)",Float) = 0
		_EffectHue("色相", Range(-0.5,0.5)) = 0
		_EffectSaturation("饱和度", Range(0,2)) = 1
		_EffectContrast("对比度", Range(0,2)) = 1
		_SaturationRightColor("灰度渐变亮色",Color)=(1,1,1,1)
		_SaturationLeftColor("灰度渐变暗色",Color)=(1,1,1,1)
		_SaturationRightColorWeights("灰度渐变亮色权重",Range(0.5,1))=1
		_SaturationLeftColorWeights("灰度渐变暗色权重",Range(0,0.5))=0
    	//  #   GroupEnd
	    
    	//  #   GroupStart:模板缓冲设置_StencilBufferSettings
    	[Space(5)]
		[Enum(UnityEngine.Rendering.CompareFunction)]_StencilComp("StencilComp",Float)=8
		[Enum(UnityEngine.Rendering.StencilOp)]_StencilPass("StencilPass",Float)=0
		[Enum(UnityEngine.Rendering.StencilOp)]_StencilFail("StencilFail",Float)=0
		_StencilRef("StencilRef",float)=0
    	//  #   GroupEnd
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
            
			#pragma shader_feature_local __ _FRESNEL_ON
			#pragma shader_feature_local __ _ENABLE_SCREEN_UV
			#pragma shader_feature_local __ _REQUIRE_CUSTOMDATA
			#pragma shader_feature_local __ _ENABLE_PLANAR_SOFT_PARTICLE
            
            #pragma shader_feature_local_vertex   __ _ENABLE_VERTEX_OFFSET
            
			#pragma shader_feature_local_fragment __ _DOUBLE_SIDE_ON
			#pragma shader_feature_local_fragment __ _DISSOLVE_NOISE_ON
			#pragma shader_feature_local_fragment __ _DUAL_LAYER_NOISE
			#pragma shader_feature_local_fragment __ _MIX_BASE_ON
            #pragma shader_feature_local_fragment __ _GRADIENT_ON
			#pragma shader_feature_local_fragment __ _COLOUR_ON
            
            #define VERTEX_REQUIRE_VERTEXCOLOR
            #define FRAGMENT_REQUIRE_VERTEXCOLOR
            
            #include "./VFXCore.hlsl"
            #include "./VFX_CommonInput.hlsl"
            #include "./VFX_CommonPass.hlsl"
            
            ENDHLSL
        }
    }
CustomEditor "com.scorpio.vfxtoolset.Editor.CommonEffectShaderGUI"
}