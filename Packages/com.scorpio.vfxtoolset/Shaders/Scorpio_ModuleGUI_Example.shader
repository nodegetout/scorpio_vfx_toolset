// Scorpio Modular ShaderGUI 示例 Shader
// 渲染管线：URP Unlit（HLSL）
//
// ── 属性命名约定 ─────────────────────────────────────────────────────
//   Begin 标记属性：必须 [HideInInspector]，属性名前缀决定其角色。
//     _ModuleBegin_Xxx    → 父模块开始
//     _SubModuleBegin_Xxx → 子模块开始
//
//   模块标题写在属性的 displayName 中，支持中文及任意字符：
//     [HideInInspector][ModuleBegin] _ModuleBegin_Xxx ("模块标题", Float) = 0
//
//   开关参数写在 Drawer 括号里：
//     [ModuleBegin]                   → 无开关
//     [ModuleBegin(_KEYWORD_ON)]       → keyword 开关
//     [ModuleBegin(_PropName, prop)]   → property 开关
//
//   End 标记直接附在模块最后一个 body 属性上，无需额外占位属性：
//     [ModuleEnd]           → 结束父模块
//     [ModuleEnd(sub)]      → 仅结束子模块
//     [ModuleEnd(sub, end)] → 结束子模块同时结束父模块

Shader "Scorpio/Examples/ModuleGUI_Example"
{
    Properties
    {
        // ── 模块前的 Header 属性（正常显示）────────────────────────────
        _MainTex ("Main Texture", 2D) = "white" {}

        // ══════════════════════════════════════════════════════════════
        // 父模块【基础颜色】— Keyword 开关 _BASE_COLOR_ON
        // 标题写在 displayName，括号只传开关参数，支持中文
        // ══════════════════════════════════════════════════════════════
        [HideInInspector][ModuleBegin(_BASE_COLOR_ON)] _ModuleBegin_BaseColor ("基础颜色", Float) = 0

            _BaseColor     ("颜色叠加", Color)      = (1,1,1,1)
            _BaseIntensity ("颜色强度", Range(0,2)) = 1.0

            // ── 子模块【菲涅尔】— Keyword 开关 _FRESNEL_ON ──────────
            [HideInInspector][SubModuleBegin(_FRESNEL_ON)] _SubModuleBegin_Fresnel ("菲涅尔", Float) = 0

                _FresnelColor ("菲涅尔颜色", Color)        = (1,1,1,1)
                _FresnelPower ("菲涅尔强度", Range(0.1,5)) = 1.0

            // 最后一个子模块属性，同时结束子模块和父模块
            [ModuleEnd(sub, end)] _FresnelBias ("菲涅尔偏移", Range(0,1)) = 0.1

        // ══════════════════════════════════════════════════════════════
        // 父模块【溶解】— Property 开关，自动取 body 第一个属性（_DissolveOn）
        // ══════════════════════════════════════════════════════════════
        [HideInInspector][ModuleBegin(prop)] _ModuleBegin_Dissolve ("溶解", Float) = 0

            // 开关属性本身加 HideInInspector，由 Header Toggle 控制
            [HideInInspector] _DissolveOn    ("溶解开关", Float)          = 0
            _DissolveNoiseTex ("溶解噪声图",  2D)                         = "white" {}
            _DissolveAmount   ("溶解程度",    Range(0,1))                 = 0.5
            _DissolveBias     ("溶解偏移",    Range(-0.5,0.5))            = 0.0
            _DissolveEdgeColor("边缘颜色",    Color)                      = (1,0.3,0,1)

        // 最后一个属性，结束父模块
        [ModuleEnd] _DissolveEdgeWidth ("边缘宽度", Range(0.01,0.5)) = 0.1
    }

    SubShader
    {
        Tags
        {
            "RenderType"      = "Transparent"
            "Queue"           = "Transparent"
            "RenderPipeline"  = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            Name "UNLIT_FORWARD"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #pragma shader_feature_local _BASE_COLOR_ON
            #pragma shader_feature_local _FRESNEL_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);          SAMPLER(sampler_MainTex);
            TEXTURE2D(_DissolveNoiseTex); SAMPLER(sampler_DissolveNoiseTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _BaseColor;
                float  _BaseIntensity;

                float4 _FresnelColor;
                float  _FresnelPower;
                float  _FresnelBias;

                float  _DissolveOn;
                float4 _DissolveNoiseTex_ST;
                float  _DissolveAmount;
                float  _DissolveBias;
                float4 _DissolveEdgeColor;
                float  _DissolveEdgeWidth;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float2 dissolveUV : TEXCOORD1;
                float3 normalWS   : TEXCOORD2;
                float3 viewDirWS  : TEXCOORD3;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = posInputs.positionCS;
                OUT.uv         = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.dissolveUV = TRANSFORM_TEX(IN.uv, _DissolveNoiseTex);

                VertexNormalInputs normInputs = GetVertexNormalInputs(IN.normalOS);
                OUT.normalWS  = normInputs.normalWS;
                OUT.viewDirWS = GetWorldSpaceViewDir(posInputs.positionWS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                #if _BASE_COLOR_ON
                    col *= _BaseColor * _BaseIntensity;

                    #if _FRESNEL_ON
                        float3 N      = normalize(IN.normalWS);
                        float3 V      = normalize(IN.viewDirWS);
                        float fresnel = _FresnelBias + (1.0 - _FresnelBias)
                                        * pow(1.0 - saturate(dot(N, V)), _FresnelPower);
                        col.rgb += _FresnelColor.rgb * fresnel;
                    #endif
                #endif

                if (_DissolveOn > 0.5)
                {
                    half noise     = SAMPLE_TEXTURE2D(_DissolveNoiseTex, sampler_DissolveNoiseTex, IN.dissolveUV).r;
                    half threshold = _DissolveAmount + _DissolveBias;
                    half edge      = smoothstep(threshold, threshold + _DissolveEdgeWidth, noise);
                    col.a *= edge;
                    if (noise < threshold + _DissolveEdgeWidth && noise > threshold)
                        col.rgb = lerp(_DissolveEdgeColor.rgb, col.rgb, edge);
                }

                return col;
            }
            ENDHLSL
        }
    }

    CustomEditor "ScorpioEditor.ScorpioModuleShaderGUIBase"
}
