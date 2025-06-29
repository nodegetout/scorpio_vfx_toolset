Shader "Theseus/MeshEffect/WaterSurface"
{
    Properties
    {
        [MainTexture]_AlbedoMap ("AlbedoMap", 2D) = "white" {}
        _AlbedoColor("AlbedoColor", Color) = (1,1,1,0.772549033)
        
        _DiffuseLerp ("DiffuseLerp", Range(0, 1)) = 0.983
        _DiffuseLerp1("DiffuseLerp1", Range(0, 5)) = 2
        _DiffuseLerp2("DiffuseLerp2", Range(0, 1)) = 0.575
        
        [Header(Specular)]
        _SpecColor ("Specular Color", Color) = (1,1,1,1)
        _Gloss ("Gloss", Range(8,128)) = 32
        
        [Header(Fresnel)]
        [HDR]_FresnelColor ("Fresnel Color", Color) = (1,0.5,0,1)
        _FresnelPower ("Fresnel Power", Range(0,10)) = 5
        _FresnelIntensity ("Intensity", Range(0,5)) = 1
        
        [Header(Emissive)]
        _EmissiveMap ("EmissiveMap", 2D) = "white" {}
        [HDR]_EmissiveColor("EmissiveColor", Color) = (0,0,0,1)
        
        [Header(Normal)]
        _NormalMap ("NormalMap", 2D) = "bump" {}
        _NormalScaleA("NormalScaleA", Range(0, 1)) = 0.353
        
        [Header(Wave)]
        _WaveNormalMap ("WaveNormalMap", 2D) = "bump" {}
        _WaveParamsB("WaveParamsB", Vector) = (5,3,3,12)
        _NormalScaleB ("NormalScaleB", Range(0, 1)) = 0.621
        
        _WaveMaskMap ("WaveMaskMap", 2D) = "black" {}
        
        [Header(Refraction)]
        _Scale_Refraction ("ScaleRefraction", Range(-0.5, 0.5)) = -0.021
        _RefractEnvColor("RefractEnvColor", Color) = (1,1,1,1)
        _EnvCapTex ("EnvCapTex", 2D) = "black" {}
        [HDR]_EnvColor("EnvColor", Color) = (2,2,2,1)
        
        [Header(VertexAnimation)]
        _VAParams("VertexAnimParams", Vector) = (5,3,3,12)
        _VertexAnimMap ("VertexAnimMap", 2D) = "black" {}
        _VARange ("VertexAnimRange", Range(0, 1)) = 0.621
        _VAScale ("VertexAnimScale", Range(0, 5)) = 1.0
        _VAHeightScale ("VertexAnimHeightScale", Range(0, 1)) = 0.644
        
        [Header(Dissolve)]
        _DissolveTex("DissolveTex", 2D) = "black" {}
        _DissolveTexUVR("DissolveTexUVR", Vector) = (10,5,1,12)
        _DissolveTexUVG("DissolveTexUVG", Vector) = (1,1,0,8)
        _DissolveLV("DissolveLV", Vector) = (0.183,0.563,0.08,0)
        _Dissolve("Dissolve", Range(-1, 1)) = 0
        _DissolveON("DissolveON", Range(0, 1)) = 0.031
        _EdgeWidth("EdgeWidth", Range(0, 1)) = 0.017
        [HDR]_DissolveEdgeColor("DissolveEdgeColor", Color) = (2.242347717,3.838060856,5.992156982,1)
        [HDR]_DissolveEdgeColor2("DissolveEdgeColor2", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags
        {
            "LightMode" = "ForwardBase"
            "IgnoreProjector" = "True"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }
        
        // TODO: Optimize grab pass!!
        GrabPass
        {
            "_GrabTexture"
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma skip_variants DIRLIGHTMAP_COMBINED DYNAMICLIGHTMAP_ON LIGHTMAP_SHADOW_MIXING LIGHTPROBE_SH SHADOWS_SHADOWMASK VERTEXLIGHT_ON LIGHTMAP_ON
            #include "../ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float3 normalOS  : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };

            struct Varyings
            {
                float4 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
                float4 normal : TEXCOORD1;
                float4 tangent : TEXCOORD2;
                float4 positionWS : TEXCOORD3;
                float4 screenPos : TEXCOORD4;
            };

            sampler2D _AlbedoMap;
            float4 _AlbedoMap_ST;
            half4  _AlbedoColor;
            
            sampler2D _EmissiveMap;
            half4  _EmissiveColor;

            sampler2D _NormalMap;
            float  _NormalScaleA;
            
            
            sampler2D _WaveNormalMap;
            sampler2D _WaveMaskMap;
            float4    _WaveParamsB;
            float     _NormalScaleB;
            
            float     _DiffuseLerp;
            float     _DiffuseLerp1;
            float     _DiffuseLerp2;
            
            sampler2D _VertexAnimMap;
            float4 _VAParams;
            float  _VARange;
            float  _VAScale;
            float  _VAHeightScale;
            
            sampler2D _DissolveTex;
            float4 _DissolveTex_ST;
            float4 _DissolveTexUVR;
            float4 _DissolveTexUVG;
            float4 _DissolveLV;
            float  _Dissolve;
            float  _DissolveON;
            float4  _DissolveEdgeColor;
            float4  _DissolveEdgeColor2;
            float  _EdgeWidth;
            
            sampler2D _GrabTexture;
            sampler2D _EnvCapTex;
            float  _Scale_Refraction;
            float4  _RefractEnvColor;
            float4  _EnvColor;

            half4  _SpecColor;
            float   _Gloss;

            float4 _FresnelColor;
            float _FresnelPower;
            float _FresnelIntensity;

            uniform float4 _hlslcc_mtx4x4g_ColorMatrix[4] = {float4(1,0,0,0),float4(0,1,0,0),float4(0,0,1,0),float4(0,0,0,1)};
            uniform float4 _g_ExposureScale = float4(0,0,0,1);
            uniform float4 _g_FrameIndexModX = float4(2,2,2,434);
            uniform float4 _g_FogColor = float4(0.75,0.689999998,1,0);
            uniform float _g_AlphaOutput = 0;
            uniform float4 _g_FogParam = float4(40, 60, -2, -25);

            Varyings vert(Attributes input)
            {
                 Varyings output = (Varyings) 0;
                
                output.normal.xyz = normalize(TransformObjectToWorldNormal(input.normalOS));
                output.normal.w = 1.0;

                output.tangent.xyz = TransformObjectToWorld(input.tangentOS.xyz).xyz;
                output.tangent.w   = input.tangentOS.w;

                float4 positionWS = mul(unity_ObjectToWorld, float4(input.vertex.xyz, 1.0));
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS.xyz);
                
                float2 vertexAnimUV = input.uv1.xy * _VAParams.xy + frac(_Time.xx * _VAParams.zw);
                float vertexAnimMap = tex2Dlod(_VertexAnimMap, float4(vertexAnimUV, 0, 1.0)).x;
                float vertexAnimMapOffset = vertexAnimMap * 2 - 1;
                float vertexAnimOffset = (vertexAnimMap - vertexAnimMapOffset) * _VARange + vertexAnimMapOffset;
                vertexAnimOffset *= _VAScale;
                vertexAnimOffset *= 0.01;
                output.positionWS.xyz = vertexAnimOffset * normalWS + positionWS.xyz;

                output.uv = float4(input.uv0.xy, input.uv1.xy);
                output.positionHCS = TransformWorldToHClip(float4(output.positionWS.xyz, 1.0));
                output.screenPos = ComputeScreenPos(output.positionHCS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 normalTS = UnpackNormalScale(tex2D(_NormalMap, input.uv.xy), _NormalScaleA);
                float2 waveUV  = input.uv.zw * _WaveParamsB.xy + frac(_Time.xx * _WaveParamsB.zw);
                float3 waveMap = tex2D(_WaveNormalMap, waveUV).xyz;
                float3 waveOffset = SafeNormalize(waveMap * 2 - 1)  + float3(-0.0, -0.0, -1.0);
                waveOffset = waveOffset * _NormalScaleB + float3(-0.0, -0.0, 1.0);
                normalTS = SafeNormalize(normalTS + waveOffset);
                
                float3 normalWS = SafeNormalize(input.normal);
                
                float3 binormalWS = cross(normalWS, input.tangent.xyz) * input.tangent.w;
                float3 viewDirWS = SafeNormalize(_WorldSpaceCameraPos.xyz - input.positionWS.xyz);
                normalWS = mul(float3x3(input.tangent.xyz, binormalWS.xyz, normalWS.xyz), normalTS).xyz;
                normalWS = normalize(normalWS);
                float nDotV = clamp(dot(normalWS, viewDirWS), 0, 1);
                
                float2 screenUV = (input.screenPos.xy / input.screenPos.w);

                float2 dissolveUVR = screenUV.xy * _DissolveTexUVR.xy + frac(_Time.xx * _DissolveTexUVR.zw);
                float2 dissolveTexUV = TRANSFORM_TEX(input.uv.zw, _DissolveTex);
                dissolveTexUV = dissolveTexUV * _DissolveTexUVG.xy + frac(_Time.xx * (_Dissolve * _DissolveLV.ww + _DissolveTexUVG.zw));

                float dissovleR = tex2D(_DissolveTex, dissolveUVR).r;
                float dissolveG = tex2D(_DissolveTex, input.uv.zw).g;
                float dissolveB = tex2D(_DissolveTex, dissolveTexUV).b;
                dissolveB = clamp(dissolveB * _DissolveLV.y, 0, 1);
                
                float scaleNDotV = nDotV * 0.5 + 0.5;
                float dissolve = dissovleR * _DissolveLV.x + dissolveG + dissolveB;
                dissolve = _DissolveLV.z * scaleNDotV + dissolve;
                dissolve = dissolve/(_DissolveLV.x + _DissolveLV.y + _DissolveLV.z + 1.0);
                clip(dissolve - (_Dissolve + 0.1 - _DissolveON));

                half4 albedoMap = tex2D(_AlbedoMap, input.uv.xy);
                half4 albedo = 0;
                albedo.rgb = ((albedoMap * 0.30530602 + 0.68217111) * albedoMap + 0.012522878) * albedoMap * _AlbedoColor.rgb;

                half4 emissive;
                emissive.rgb = tex2D(_EmissiveMap, input.uv.xy).rgb * _EmissiveColor.rgb;
                half3 emissiveRemap = (emissive.rgb * 0.30530602 + 0.68217111) *  emissive.rgb + 0.012522878;

                float2 waveMaskMap = tex2D(_WaveMaskMap, input.uv.xy).rg;
                float2 vertexAnimUV = input.uv.zw * _VAParams.xy + frac(_Time.xx * _VAParams.zw);
                float  vertexAnimMap = tex2D(_VertexAnimMap, vertexAnimUV).r;
                float vertexAnimOffset = vertexAnimMap * 2 - 1;
                vertexAnimOffset = (vertexAnimMap - vertexAnimOffset) * _VARange + vertexAnimOffset;
                vertexAnimOffset = vertexAnimOffset * _VAHeightScale + waveMaskMap.x;
                vertexAnimOffset = clamp(vertexAnimOffset, 0, 1);

                float diffuseLerp = _DiffuseLerp2 - _DiffuseLerp1;
                diffuseLerp = (vertexAnimOffset - _DiffuseLerp1) / diffuseLerp;
                diffuseLerp = clamp(diffuseLerp, 0, 1);
                // TODO: Smoothstep???
                diffuseLerp = diffuseLerp * diffuseLerp * (3 - 2 * diffuseLerp) * _DiffuseLerp * (1 - waveMaskMap.y);

                float3 normalHCS  = mul(unity_MatrixVP, normalWS);
                float2 fracUV = normalize(normalHCS).xy;
                fracUV = (-fracUV * _Scale_Refraction) * float2(1.0, -1.0);
                fracUV = sin((1 - nDotV) * 6.0) * fracUV +  screenUV * _ScreenParams.zw;
                float3 refraction = tex2Dlod(_GrabTexture, float4(fracUV,0,0)).rgb;

                float3 refractEnvColor = (_RefractEnvColor.rgb * refraction - albedo.rgb) * diffuseLerp + albedo.rgb;
                float3 positionVS = TransformWorldToView(input.positionWS);
                float3 normalVS   = TransformWorldToViewNormal(normalWS);
                float2 envCapUV = normalVS.xy * positionVS.xy;// - positionVS.zw * normalVS.wz;
                // envCapUV =  envCapUV * float2(-0.5, 0.5) + float2(0.5, 0.5);
                envCapUV =  envCapUV * 0.5 + 0.5;
                float3 envCap = tex2D(_EnvCapTex, envCapUV);
                envCap = ((envCap * 0.30530602 + 0.68217111) * envCap + 0.012522878) * envCap; 
                envCap *= _EnvColor.rgb;
                
                refractEnvColor.rgb = envCap * (1 - waveMaskMap.y) + refractEnvColor.rgb;
                refractEnvColor.rgb = emissiveRemap * emissive.rgb + refractEnvColor.rgb;

                dissolve = 1 - 10 * (dissolve - _Dissolve - _EdgeWidth);
                float3 dissolveColor = _DissolveEdgeColor.www * _DissolveEdgeColor.xyz;
                dissolveColor *= dissolve;
                dissolveColor = max(dissolveColor, 0);
                dissolveColor = dissolveColor + _DissolveEdgeColor2.xyz * _DissolveEdgeColor2.www;

                // fresnel
                float ndotV = dot(input.normal, viewDirWS);
                float fresnel = pow(1 - max(0.1, ndotV), _FresnelPower);
                float3 fresnelEffect = fresnel * _FresnelIntensity * _FresnelColor.rgb;

                // specular
                float3 lightDir   = normalize(_MainLightPosition.xyz);
                float3 reflectDir = reflect(-lightDir, normalWS);
                float spec = pow(max(dot(reflectDir, viewDirWS), 0), _Gloss);
                float3 specular = spec * _SpecColor.rgb;
                
                float4 finalColor;
                finalColor.rgb = dissolveColor + refractEnvColor + fresnelEffect + specular;
                
                // TODO: tonemapping!!!
                // TonemapForward(finalColor.rgb);
                return finalColor;
            }
            ENDHLSL
        }
    }

}