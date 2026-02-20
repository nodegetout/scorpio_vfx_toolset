// Scorpio Modular ShaderGUI 示例 Shader
// 渲染管线：URP Unlit（HLSL）
//
// 属性命名约定：
//   模块标记属性必须添加 [HideInInspector]，名称前缀决定其角色：
//     _ModuleBegin_Xxx    → 父模块开始
//     _SubModuleBegin_Xxx → 子模块开始
//     _ModuleEnd_Xxx      → 父模块结束
//     _SubModuleEnd_Xxx   → 子模块结束（displayName "1" = 同时结束父模块）
//
//   Begin 系列 displayName 格式：  "Title|ToggleType|ToggleTarget"
//     ToggleType = None / Keyword / Property
//   SubModuleEnd displayName：     "0" 或 "1"

Shader "Scorpio/Examples/ModuleGUI_Example"
{
    Properties
    {
        // ── 模块前的 Header 属性（正常显示）────────────────────────────
        _MainTex ("Main Texture", 2D) = "white" {}

        // ══════════════════════════════════════════════════════════════
        // 父模块【Base Color】 — Keyword 开关 _BASE_COLOR_ON
        // ══════════════════════════════════════════════════════════════
        [HideInInspector][ModuleBegin] _ModuleBegin_BaseColor ("Base Color|Keyword|_BASE_COLOR_ON", Float) = 0

            _BaseColor      ("Base Color Tint",  Color)        = (1,1,1,1)
            _BaseIntensity  ("Color Intensity",  Range(0,2))   = 1.0

            // ── 子模块【Fresnel】 — Keyword 开关 _FRESNEL_ON ──────────
            [HideInInspector][SubModuleBegin] _SubModuleBegin_Fresnel ("Fresnel|Keyword|_FRESNEL_ON", Float) = 0

                _FresnelColor ("Fresnel Color",  Color)       = (1,1,1,1)
                _FresnelPower ("Fresnel Power",  Range(0.1,5))= 1.0
                _FresnelBias  ("Fresnel Bias",   Range(0,1))  = 0.1

            // SubModuleEnd displayName "1" = 子模块结束同时结束父模块
            [HideInInspector][SubModuleEnd] _SubModuleEnd_Fresnel ("1", Float) = 0

        // ══════════════════════════════════════════════════════════════
        // 父模块【Dissolve】 — Property 开关（使用 _DissolveOn 属性值）
        // ══════════════════════════════════════════════════════════════
        [HideInInspector][ModuleBegin] _ModuleBegin_Dissolve ("Dissolve|Property|_DissolveOn", Float) = 0

            // 开关属性本身加 HideInInspector，由 Header Toggle 控制，不在 body 中绘制
            [HideInInspector] _DissolveOn       ("Dissolve Enabled",     Float)       = 0
            _DissolveNoiseTex  ("Dissolve Noise", 2D)                                 = "white" {}
            _DissolveAmount    ("Dissolve Amount",Range(0,1))                         = 0.5
            _DissolveBias      ("Dissolve Bias",  Range(-0.5,0.5))                    = 0.0
            _DissolveEdgeColor ("Edge Color",     Color)                              = (1,0.3,0,1)
            _DissolveEdgeWidth ("Edge Width",     Range(0.01,0.5))                    = 0.1

        [HideInInspector][ModuleEnd] _ModuleEnd_Dissolve ("", Float) = 0
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

            // ── 属性声明 ──────────────────────────────────────────────
            TEXTURE2D(_MainTex);      SAMPLER(sampler_MainTex);
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

            // ── 顶点输入 / 输出 ───────────────────────────────────────
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
                // ── 主贴图采样 ─────────────────────────────────────────
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                // ── Base Color 模块 ────────────────────────────────────
                #if _BASE_COLOR_ON
                    col *= _BaseColor * _BaseIntensity;

                    // ── Fresnel 子模块 ─────────────────────────────────
                    #if _FRESNEL_ON
                        float3 N       = normalize(IN.normalWS);
                        float3 V       = normalize(IN.viewDirWS);
                        float  fresnel = _FresnelBias + (1.0 - _FresnelBias)
                                         * pow(1.0 - saturate(dot(N, V)), _FresnelPower);
                        col.rgb += _FresnelColor.rgb * fresnel;
                    #endif
                #endif

                // ── Dissolve 模块（property 开关）─────────────────────
                if (_DissolveOn > 0.5)
                {
                    half noise    = SAMPLE_TEXTURE2D(_DissolveNoiseTex, sampler_DissolveNoiseTex, IN.dissolveUV).r;
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

