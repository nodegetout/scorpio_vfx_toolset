// Vector4Split Drawer 演示 Shader
// 渲染管线：URP Unlit（HLSL）
//
// ── Drawer 使用说明 ───────────────────────────────────────────────────
//
//   [Vector4Split(SplitMode)]
//   [Vector4Split(SplitMode, labelWidth)]
//
//   displayName 格式：
//     简洁标题 ## 段1标签|DrawType @ 段2标签|DrawType @ ...
//
//   DrawType 可选值：
//     Float              → 普通浮点输入框（默认，可省略 |Float）
//     Slider(min, max)   → 滑动条
//     Toggle             → 勾选框（0 / 1）
//
//   SplitMode 段数约束：
//     FourFloats   → 4 段
//     TwoVector2   → 2 段（Vector2Field 渲染，无 DrawType）
//     Vector3Float → 2 段（第1段 Vector3Field，第2段可配置 DrawType）
//
// ── 校验错误演示（下方两个属性） ───────────────────────────────────────
//   • 缺少 ## 分隔符 → Inspector 显示 warning HelpBox
//   • 挂在 Float 属性上 → Inspector 显示 warning HelpBox

Shader "Scorpio/Examples/Vector4Split_Example"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}

        // ══════════════════════════════════════════════════════════════
        // 演示1：FourFloats — 4个独立float，含全部DrawType，自定义labelWidth=100
        // ══════════════════════════════════════════════════════════════
        [Vector4Split(FourFloats, 100)]
        _UVParams ("UV参数 ## X速度|Float @ Y速度|Float @ 缩放|Slider(0, 10) @ 开关|Toggle", Vector) = (0, 0, 1, 0)

        // ══════════════════════════════════════════════════════════════
        // 演示2：TwoVector2 — 拆为两个 Vector2（xy / zw）
        // ══════════════════════════════════════════════════════════════
        [Vector4Split(TwoVector2)]
        _FlowParams ("流速旋转 ## 流速XY @ 缩放旋转ZW", Vector) = (0, 0, 1, 0)

        // ══════════════════════════════════════════════════════════════
        // 演示3：Vector3Float — 前3分量 Vector3Field + 第4分量 Slider
        // ══════════════════════════════════════════════════════════════
        [Vector4Split(Vector3Float)]
        _OffsetParams ("偏移参数 ## 方向XYZ @ 强度|Slider(0, 5)", Vector) = (0, 0, 0, 1)

        // ══════════════════════════════════════════════════════════════
        // 错误演示1：缺少 ## 分隔符 → Inspector 显示 warning HelpBox
        // ══════════════════════════════════════════════════════════════
        [Vector4Split(FourFloats)]
        _ErrorNoSeparator ("这是一个没有##分隔符的displayName", Vector) = (0, 0, 0, 0)

        // ══════════════════════════════════════════════════════════════
        // 错误演示2：Drawer 挂在 Float 属性上（prop.type 校验失败）
        // ══════════════════════════════════════════════════════════════
        [Vector4Split(FourFloats)]
        _ErrorWrongType ("错误类型属性（Float） ## X @ Y @ Z @ W", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType"      = "Opaque"
            "Queue"           = "Geometry"
            "RenderPipeline"  = "UniversalPipeline"
        }

        Pass
        {
            Name "UNLIT_FORWARD"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _UVParams;       // x=X速度, y=Y速度, z=缩放, w=开关
                float4 _FlowParams;     // xy=流速, zw=缩放旋转
                float4 _OffsetParams;   // xyz=方向, w=强度
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv         = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                // ── 将三个 Vector4 属性的分量可视化输出 ──────────────
                // 红通道：UVParams.x（X速度，归一化到0-1显示）
                // 绿通道：FlowParams.z（缩放，归一化到0-1显示，÷10）
                // 蓝通道：OffsetParams.w（强度，归一化到0-1显示，÷5）
                // Alpha：由主贴图决定

                float r = saturate(_UVParams.x * 0.1);          // X速度 mapped
                float g = saturate(_FlowParams.z * 0.1);        // 缩放 mapped
                float b = saturate(_OffsetParams.w * 0.2);      // 强度 mapped

                // 叠加主贴图颜色
                col.rgb = col.rgb * 0.5 + half3(r, g, b) * 0.5;

                // UVParams.w 作为开关：关闭时叠加灰色调
                if (_UVParams.w < 0.5)
                    col.rgb = dot(col.rgb, half3(0.299, 0.587, 0.114)).xxx;

                return col;
            }
            ENDHLSL
        }
    }

    // 不指定 CustomEditor，使用默认 Inspector，Drawer 独立工作
}

