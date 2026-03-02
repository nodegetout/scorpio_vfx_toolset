Shader "Hidden/Theseus/VFX/MeshEffect_CommonSF_Modular"
{
    Properties
    {
        // ══════════════════════════════════════════════════════════════
        // 【合并阶段设置】 — 无开关
        // ══════════════════════════════════════════════════════════════
        [HideInInspector][ModuleBegin(1)] _ModuleBegin_MergeStage ("合并阶段设置", Float) = 0
            [Enum(Add,1,Blend,10)]_DstBlend("混合模式", Float) = 10
            [Enum(UnityEngine.Rendering.CullMode)]_CullMode("剔除模式", Float) = 0
            [Enum(Off, 0, On, 1)]_ZWrite("深度测试", Float) = 0
        [ModuleEnd][Enum(On, 0, Off, 4)]_ZTest("总是最前", Float) = 4

        // ══════════════════════════════════════════════════════════════
        // 【UV模式设置】 — 无开关
        // ══════════════════════════════════════════════════════════════
        [HideInInspector][ModuleBegin(1)] _ModuleBegin_UVMode ("UV模式设置", Float) = 0
        [ModuleEnd][Toggle(_ENABLE_SCREEN_UV)]_EnableScreenUV("切换屏幕UV", Float) = 0

        // ══════════════════════════════════════════════════════════════
        // 【模板缓存设置】 — 无开关
        // ══════════════════════════════════════════════════════════════
        [HideInInspector][ModuleBegin(1)] _ModuleBegin_Stencil ("模板缓存设置", Float) = 0
            _StencilRef("模板参考值", Range(0, 255)) = 0
            [Enum(UnityEngine.Rendering.CompareFunction)]_StencilComp("比较方式", Float) = 8
            [Enum(UnityEngine.Rendering.StencilOp)]_StencilPass("通过运算", Float) = 0
        [ModuleEnd][Enum(UnityEngine.Rendering.StencilOp)]_StencilFail("失败运算", Float) = 0

        // ══════════════════════════════════════════════════════════════
        // 【主贴图设置】 — 无开关
        // ══════════════════════════════════════════════════════════════
        [HideInInspector][ModuleBegin(1)] _ModuleBegin_Main ("主贴图设置", Float) = 0
            [MainTexture]_BaseMap("主贴图", 2D) = "white" {}
            [Vector4Split(FourFloats)]_BaseMapToggles("主贴图开关 ## 开启预乘Alpha(禁动画中K开关)|Toggle @ 去黑底(禁动画中K开关)|Toggle @ 开启极坐标(禁动画中K开关)|Toggle @ 切换为2U(禁动画中K开关)|Toggle", Vector) = (0,0,0,0)
            [HDR]_BaseColor("整体叠色", Color) = (1,1,1,1)
            [HDR]_BaseFrontColor("前面叠色", Color) = (1,1,1,1)
            _FrontIntensity("颜色强度", Range(0, 10)) = 0
            [Vector4Split(FourFloats)]_DouYinEffectParams("抖音色效果 ## 开启抖音色效果|Toggle @ OffsetX|Slider(-1, 1) @ OffsetY|Slider(-1, 1) @ _|Hidden", Vector) = (0, 0.04, -0.04, 0)
        [ModuleEnd][Vector4Split(FourFloats)]_BaseUVParams("主贴图UV参数 ## U方向流速 @ V方向流速 @ 缩放|Slider(0, 10) @ 旋转|Slider(0, 720)", Vector) = (0,0,1,0)

        // ══════════════════════════════════════════════════════════════
        // 【遮罩设置】 — 无开关
        // ══════════════════════════════════════════════════════════════
        [HideInInspector][ModuleBegin(1)] _ModuleBegin_Mask ("遮罩设置", Float) = 0
            _Mask("Mask贴图", 2D) = "white" {}
            [Vector4Split(FourFloats)]_MaskUVParams("遮罩UV参数 ## U方向流速 @ V方向流速 @ 缩放|Slider(0, 10) @ 旋转|Slider(0, 720)", Vector) = (0,0,1,0)
            [Vector4Split(FourFloats)]_MaskMapParams("遮罩参数 ## 兼容纯Alpha图|Toggle @ 遮罩强度|Slider(0, 1) @ _|Hidden @ _|Hidden", Vector) = (0,1,0,0)
            [Toggle]_MaskMapSwitchUV1("切换2U", Float) = 0
        [ModuleEnd][Vector4Split(FourFloats)]_MaskMapToggles("遮罩通道开关 ## R通道不影响主贴图Alpha(禁动画中K开关)|Toggle @ R通道不影响混合贴图Alpha(禁动画中K开关)|Toggle @ G通道影响溶解(禁动画中K开关)|Toggle @ B通道影响扰动(禁动画中K开关)|Toggle", Vector) = (0,0,0,0)

        // ══════════════════════════════════════════════════════════════
        // 【双面设置】 — Property 开关 _DoubleSideOn
        // ══════════════════════════════════════════════════════════════
        [HideInInspector][ModuleBegin(prop)] _ModuleBegin_DoubleSide ("双面设置", Float) = 0
            _BackIntensity("背面颜色强度", Range(0, 10)) = 0
        [ModuleEnd][HDR]_DiffuseBackColor("背面颜色", Color) = (1,1,1,1)

        // ══════════════════════════════════════════════════════════════
        // 【混合贴图设置】 — Keyword 开关 _MIX_BASE_ON
        // ══════════════════════════════════════════════════════════════
        [HideInInspector][ModuleBegin(_MIX_BASE_ON)] _ModuleBegin_MixBase ("混合贴图设置", Float) = 0
            _MixDiffuse("混合贴图", 2D) = "black" {}
            [HDR]_MixTintColor("混合贴图叠色", Color) = (1,1,1,1)
            [Vector4Split(FourFloats)]_MixMapParams("混合参数 ## 去黑|Toggle @ 开启Noise影响|Toggle @ 开启Ramp混色模式|Toggle @ 混合贴图强度|Slider(0, 2)", Vector) = (0, 0, 0, 1)
        [ModuleEnd][Vector4Split(FourFloats)]_MixBaseMapUVParams("混合贴图UV参数 ## U方向流速 @ V方向流速 @ 缩放|Slider(0, 10) @ 旋转|Slider(0, 720)", Vector) = (0,0,1,0)

        // ══════════════════════════════════════════════════════════════
        // 【溶解设置】 — Keyword 开关 _FALLOFF_DISSOLVE_ON
        // ══════════════════════════════════════════════════════════════
        [ModuleBegin(_FALLOFF_DISSOLVE_ON)]_ModuleBegin_FalloffDissolve("溶解设置", float) = 0
        _DissolveTex("溶解贴图", 2D) = "black" {}
        [Enum(X, 0, Y, 1, NoUV, 2)]_DissolveDir("溶解方向切换，默认根据UV横向溶解", Int) = 0
		[Vector4Split(TwoVector2)]_DissolveNoiseParam("溶解纹理参数 ## 溶解纹理Tiling @ 溶解纹理UV流速",Vector) = (1,1,0,0)
		[Vector4Split(FourFloats)]_DissolveControlParams("溶解控制参数 ## 溶解阈值|Slider(-2, 2) @ 溶解软硬|Slider(0, 1) @ 溶解边缘阈值|Slider(-2, 2) @ 溶解边缘软硬|Slider(0, 1)", Vector) = (0, 01, 0, 1)
		[ModuleEnd][HDR]_DissolveColor("溶解边缘颜色", Color) = (1,1,1,1)
//        _DissolveThreshold("溶解阈值",range(-2,2)) = 0
//		_DissolveFallOff("溶解边缘软硬",range(0,1)) = 1
//		_DissolveColorThreshold("溶解边缘颜色阈值", range(-2,2)) = 0
//		[ModuleEnd]_DissolveColorFallOff("溶解边缘颜色软硬", range(0,1)) = 1

        // ══════════════════════════════════════════════════════════════
        // 【FlowMap设置】 — Keyword 开关 _FLOW_MAP_ON
        // ══════════════════════════════════════════════════════════════
        [HideInInspector][ModuleBegin(_FLOW_MAP_ON)] _ModuleBegin_FlowMap ("FlowMap设置", Float) = 0
            _FlowMap("FlowMap贴图", 2D) = "white" {}
        [ModuleEnd][Vector4Split(FourFloats)]_FlowMapParams("FlowMap参数 ## 流动速度 @ 扰动强度|Slider(0, 1) @ _|Hidden @ _|Hidden", Vector) = (0.5, 1, 0, 0)

        // ══════════════════════════════════════════════════════════════
        // 【扰动设置】 — Keyword 开关 _NOISE_ON
        // ══════════════════════════════════════════════════════════════
        [HideInInspector][ModuleBegin(_NOISE_ON)] _ModuleBegin_Noise ("扰动设置", Float) = 0
            [Toggle]_NoiseUnEffectDiff("Noise不影响主贴图(禁动画中K开关)", Float) = 0
            [Toggle]_DualDirectionNoise("开启双层Noise效果(禁动画中K开关)", Float) = 0
            _NoiseTex1("扰动贴图", 2D) = "black" {}
            _NoiseTex2("第二层noise图", 2D) = "black" {}
            [Vector4Split(FourFloats)]_NoiseStrength("扭曲强度 ## 第一层扭曲强度U|Slider(-2, 2) @ 第一层扭曲强度V|Slider(-2, 2) @ 第二层扭曲强度U|Slider(-2, 2) @ 第二层扭曲强度V|Slider(-2, 2)", Vector) = (0.5, 0.5, 0.5, 0.5)
        [ModuleEnd]_GChannel("UV控制(xy第一层noise速度 zw第二层noise速度)", Vector) = (1,0,-1,0)

        // ══════════════════════════════════════════════════════════════
        // 【Fresnel设置】 — Keyword 开关 _FRESNEL_ON
        // ══════════════════════════════════════════════════════════════
        [HideInInspector][ModuleBegin(_FRESNEL_ON)] _ModuleBegin_Fresnel ("Fresnel设置", Float) = 0
            _FresnelMap("叠乘贴图", 2D) = "white" {}
            [Toggle]_FresnelMapUse2U("使用2U", Float) = 0
            [HDR]_FresnelColor("颜色", Color) = (1,1,1,1)
        [ModuleEnd][Vector4Split(FourFloats)]_FresnelParams("Fresnel参数 ## 反向Fresnel Alpha|Toggle @ 范围|Slider(0, 2) @ 强度|Slider(0, 1) @ 叠加模式|Toggle", Vector) = (0, 0, 1, 0)

        // ══════════════════════════════════════════════════════════════
        // 【顶点偏移设置】 — Keyword 开关 _ENABLE_VERTEX_OFFSET
        // ══════════════════════════════════════════════════════════════
        [HideInInspector][ModuleBegin(_ENABLE_VERTEX_OFFSET)] _ModuleBegin_VertexOffset ("顶点偏移设置", Float) = 0
            _VertexOffsetNoiseMap("顶点偏移噪声图(R通道)", 2D) = "white" {}
            [Enum(Normal,0,Vertex,1)]_MotionDir("运动方向", Float) = 0
            _VertexDir("顶点方向", Vector) = (0,0,0,0)
            [Vector4Split(FourFloats)]_VertexOffsetParams("顶点偏移参数 ## VertexScale @ VertexPower @ VertexScaleHeightU|Slider(0, 1) @ VertexScaleHeightV|Slider(0, 1)", Vector) = (0, 1, 1, 0)
        [ModuleEnd]_VertexMotionSpeed("VertexMotionSpeed", Vector) = (0,0,0,0)

        // ══════════════════════════════════════════════════════════════
        // 【渐变设置】 — Keyword 开关 _GRADIENT_ON
        // ══════════════════════════════════════════════════════════════
        [HideInInspector][ModuleBegin(_GRADIENT_ON)] _ModuleBegin_Gradient ("渐变设置", Float) = 0
            [Toggle]_GradientSameDiffOn("左右渐变开启Diff相同UV(禁动画中K开关)", Float) = 0
            _LeftColor("左侧渐变色", Color) = (1,1,1,1)
            _RightColor("右侧渐变色", Color) = (1,1,1,1)
        [ModuleEnd][Vector4Split(FourFloats)]_GradientParams("渐变参数 ## UV权重|Slider(0, 1) @ 左侧渐变色权重|Slider(0, 1) @ 右侧渐变色权重|Slider(0, 2) @ 渐变色权重偏移|Slider(-1, 1)", Vector) = (0, 0, 1, 0)

        // ══════════════════════════════════════════════════════════════
        // 【软粒子设置】 — Keyword 开关 _ENABLE_PLANAR_SOFT_PARTICLE
        // ══════════════════════════════════════════════════════════════
        [HideInInspector][ModuleBegin(_ENABLE_PLANAR_SOFT_PARTICLE)] _ModuleBegin_SoftParticle ("软粒子设置", Float) = 0
            _ContactRange("软化范围", Range(0.001, 1)) = 0.5
        [ModuleEnd]_HorizontalPlaneY("水平面Y值", Float) = 0

        // ══════════════════════════════════════════════════════════════
        // 【调色设置】 — Keyword 开关 _COLOUR_ON
        // ══════════════════════════════════════════════════════════════
        [HideInInspector][ModuleBegin(_COLOUR_ON)] _ModuleBegin_Colour ("调色设置", Float) = 0
            [Vector4Split(FourFloats)]_ColorGradingParams("调色参数 ## 色相|Slider(-0.5, 0.5) @ 饱和度|Slider(0, 2) @ 对比度|Slider(0, 2) @ _|Hidden", Vector) = (0, 1, 1, 0)
            _SaturationRightColor("灰度渐变亮色", Color) = (1,1,1,1)
            _SaturationLeftColor("灰度渐变暗色", Color) = (1,1,1,1)
            _SaturationRightColorWeights("灰度渐变亮色权重", Range(0.5, 1)) = 1
        [ModuleEnd]_SaturationLeftColorWeights("灰度渐变暗色权重", Range(0, 0.5)) = 0
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

            // global multi compile keywords
			#pragma multi_compile __ _COLOR_HDR_

            // local shader feature keywords
			#pragma shader_feature_local _ENABLE_SCREEN_UV
			#pragma shader_feature_local _MIX_BASE_ON
			#pragma shader_feature_local _ENABLE_VERTEX_OFFSET
            #pragma shader_feature_local _FALLOFF_DISSOLVE_ON
			#pragma shader_feature_local _GRADIENT_ON
			#pragma shader_feature_local _COLOUR_ON
			#pragma shader_feature_local _FRESNEL_ON
			#pragma shader_feature_local __ _FLOW_MAP_ON _NOISE_ON
			#pragma shader_feature_local _ENABLE_PLANAR_SOFT_PARTICLE
            
            #define VERTEX_REQUIRE_UV1
            #define FRAGMENT_REQUIRE_UV1
            
            #if defined(_FRESNEL_ON) | defined(_ENABLE_PLANAR_SOFT_PARTICLE)
				#define REQUIRE_POSITIONWS
            #endif

            #if defined(_FALLOFF_DISSOLVE_ON)
            #define VERTEX_REQUIRE_UV2
            #define FRAGMENT_REQUIRE_UV2
            #endif


            #include "Packages/com.scorpio.vfxtoolset/Shaders/Effect/VFXCore.hlsl"
            #include "Packages/com.scorpio.vfxtoolset/Shaders/Effect/VFX_CommonInput.hlsl"
            #include "Packages/com.scorpio.vfxtoolset/Shaders/Effect/VFX_CommonModule.hlsl"
            #include "Packages/com.scorpio.vfxtoolset/Shaders/Effect/VFX_CommonShaderFeaturePass.hlsl"
            
            ENDHLSL
        }
    }
CustomEditor "ScorpioEditor.ScorpioModuleShaderGUIBase"
}
