#ifndef _VFX_COMMON_MODULE_
#define _VFX_COMMON_MODULE_

float3 ApplyVertexOffsetWS(float2 uv, float4 tilingOffset, float2 uvSpeed, sampler2D noiseMap, float3 scale)
{
    half2 noiseUV = uv.xy * tilingOffset.xy + tilingOffset.zw;
    half2 flowNoiseUV = _Time.yy * uvSpeed.xy + noiseUV;
    half simpleBerlinNoise = tex2Dlod(noiseMap, float4(flowNoiseUV, 0, 1));
    simpleBerlinNoise = simpleBerlinNoise * 2.0 -1.0;
    float3 offsetVector = simpleBerlinNoise * scale;
    return offsetVector;
}

float2 CalNoiseUV(sampler2D noiseTex, float4 noiseTex_ST, float2 uv, float4 uvParams, float strength)
{
    float2 noiseTUV = uv.xy * uvParams.xy + frac(_Time.yy * uvParams.zw);
    half4  noiseMapValue = tex2D(noiseTex, noiseTUV);
    half2  noiseUV  = strength * noiseMapValue.g;

    #if defined(_DUAL_LAYER_NOISE)
    float2 invDirNoiseUV = uv.xy * uvParams.xy - (_Time.yy * uvParams.zw) - 0.25;
    half4  invDirNoiseMapValue = tex2D(noiseTex, TRANSFORM_TEX(invDirNoiseUV, noiseTex));
    noiseUV = strength * max(noiseMapValue.g, invDirNoiseMapValue.g) * noiseMapValue.g * invDirNoiseMapValue.g;
    #else
    noiseUV = strength * noiseMapValue.g;
    #endif
    
    return noiseUV;
}

half4 ApplyCommonDissolve(half3 dissolveParams, half3 dissolveOutlineParams, half3 dissolveTintColor, half dissolveTintColorPower, half4 color)
{
    // dissolveParams : x - dissolveTexValue, y - softSize, z - dissolveStep
    // dissolveOutlineParams: x - outlineWidth, y - outlineSoft, z - outlineOn
    half  stepTmp			 = dissolveParams.z - dissolveOutlineParams.x + 0.25;
    half  percent			 = smoothstep(stepTmp - dissolveParams.y, stepTmp, dissolveParams.x);
    half  dissolveThreshold  = smoothstep(dissolveParams.z, dissolveParams.z + dissolveOutlineParams.y, dissolveParams.x);
    half3 dissolveColor  = lerp(color.rgb, dissolveTintColor.rgb * dissolveTintColorPower, dissolveOutlineParams.z);
    
    color.rgb  = lerp(dissolveColor, color.rgb, dissolveThreshold);
    color.a   *= percent;
    return color;
}

half2 ApplyDualLayerNoise(half2 noiseUV0, half2 layer1Speed, sampler2D noiseLayer1Tex, half noiseLayer1Strength, half noiseLayer0Value)
{
    half2 uv2 = noiseUV0 + frac(_Time.xx * layer1Speed.xy);
    half2 noiseUV = uv2 + noiseLayer0Value.rr;
    noiseUV *= noiseLayer1Strength;
    half4 layer2NoiseValue = tex2D(noiseLayer1Tex, noiseUV);
    noiseUV = noiseLayer0Value.rr * layer2NoiseValue.rr;
    return noiseUV;
}

half3 ApplyMixDiffuse(sampler2D mixDiffuseMap, float2 mixDiffUV, half mixMultiplyLuminance, half mixDiffusePower, half3 color)
{
    half4 mixMapValue = tex2D(mixDiffuseMap, mixDiffUV);
            	
    UNITY_BRANCH
    if (mixMultiplyLuminance > 0.5)
    {
        mixMapValue.a *= Luminance(mixMapValue.rgb);
        color.rgb = mixMapValue.a * mixMapValue.rgb + (1 - mixMapValue.a) * color.rgb;
    }else
    {
        color.rgb = lerp(1.0, mixMapValue.rgb * mixMapValue.a, mixDiffusePower) * color.rgb;
    }
    return color;
}

#endif