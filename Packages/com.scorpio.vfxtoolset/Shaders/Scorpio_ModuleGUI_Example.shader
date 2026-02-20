// Scorpio Modular ShaderGUI 示例 Shader
// 渲染管线：URP Unlit（HLSL）
//
// ── 属性命名约定 ─────────────────────────────────────────────────────
//   Begin 标记属性：必须 [HideInInspector]，属性名前缀决定其角色。
//     _ModuleBegin_Xxx    → 父模块开始
//     _SubModuleBegin_Xxx → 子模块开始
//   开关参数写在 Drawer 括号里，无需在 displayName 中编码：
//     [ModuleBegin(Title)]                  → 无开关
//     [ModuleBegin(Title, _KEYWORD_ON)]      → keyword 开关
//     [ModuleBegin(Title, _PropName, prop)]  → property 开关
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
        // 父模块【Base Color】— Keyword 开关 _BASE_COLOR_ON
        // ══════════════════════════════════════════════════════════════
        [HideInInspector][ModuleBegin(Base Color, _BASE_COLOR_ON)]
            _ModuleBegin_BaseColor ("", Float) = 0

            _BaseColor     ("Base Color Tint", Color)      = (1,1,1,1)
            _BaseIntensity ("Color Intensity", Range(0,2)) = 1.0

            // ── 子模块【Fresnel】— Keyword 开关 _FRESNEL_ON ─────────
            [HideInInspector][SubModuleBegin(Fresnel, _FRESNEL_ON)]
                _SubModuleBegin_Fresnel ("", Float) = 0

                _FresnelColor ("Fresnel Color", Color)        = (1,1,1,1)
                _FresnelPower ("Fresnel Power", Range(0.1,5)) = 1.0

            // 最后一个子模块属性，同时结束子模块和父模块
            [ModuleEnd(sub, end)]
                _FresnelBias ("Fresnel Bias",  Range(0,1))   = 0.1

        // ══════════════════════════════════════════════════════════════
        // 父模块【Dissolve】— Property 开关（使用 _DissolveOn 属性值）
        // ══════════════════════════════════════════════════════════════
        [HideInInspector][ModuleBegin(Dissolve, _DissolveOn, prop)]
            _ModuleBegin_Dissolve ("", Float) = 0

            // 开关属性本身加 HideInInspector，由 Header Toggle 控制
            [HideInInspector] _DissolveOn ("Dissolve Enabled", Float) = 0
            _DissolveNoiseTex  ("Dissolve Noise",  2D)               = "white" {}
            _DissolveAmount    ("Dissolve Amount", Range(0,1))        = 0.5
            _DissolveBias      ("Dissolve Bias",   Range(-0.5,0.5))   = 0.0
            _DissolveEdgeColor ("Edge Color",      Color)             = (1,0.3,0,1)

        // 最后一个属性，结束父模块
        [ModuleEnd]
            _DissolveEdgeWidth ("Edge Width", Range(0.01,0.5)) = 0.1
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
