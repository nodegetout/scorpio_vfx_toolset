#ifndef _VFX_COMMON_MODULE_
#define _VFX_COMMON_MODULE_

half4 ApplyCommonDissolve(half3 dissolveParams, half4 dissolveOutlineParams, half3 dissolveTintColor, half edgeColorBlendMode, half4 color)
{
    // dissolveParams : x - dissolveTexValue, y - softSize, z - dissolveStep
    // dissolveOutlineParams: x - outlineOn, y - outlineSoft, z - outlineWidth, w - dissolveOutlineParams
    half  stepTmp			 = dissolveParams.z - dissolveOutlineParams.z + 0.25;
    half  percent			 = smoothstep(stepTmp - dissolveParams.y, stepTmp, dissolveParams.x);
    half  dissolveThreshold  = smoothstep(dissolveParams.z, dissolveParams.z + dissolveOutlineParams.y, dissolveParams.x);
    half3 dissolveColor  = lerp(color.rgb, dissolveTintColor.rgb * dissolveOutlineParams.w, dissolveOutlineParams.x);

    // change different blend mode
    color.rgb *= lerp(dissolveThreshold, 1, step(0.5, edgeColorBlendMode));
    color.rgb += (dissolveColor  - dissolveColor * dissolveThreshold);
    
    color.a   *= percent;
    return color;
}

half2 ApplyDualLayerNoise(half2 noiseUV0, half2 layer1Speed, sampler2D noiseLayer1Tex, half2 noiseStrength, half noiseLayer0Value)
{
    half2 uv2 = noiseUV0 + frac(_Time.xx * layer1Speed.xy);
    uv2 = uv2 * noiseStrength + noiseLayer0Value * noiseStrength;
    half layer2NoiseValue = tex2D(noiseLayer1Tex, uv2).r;
    return noiseLayer0Value * layer2NoiseValue;
}

half3 ApplyMixDiffuse(sampler2D mixDiffuseMap, float4 mixDiffParams,half4 mixTintColor, half3 color)
{
    // params: x - luminance, y - power, zw - mixDiffUV
    half4 mixMapValue    = SampleColorTex(mixDiffuseMap, mixDiffParams.zw);
    half alphaMultiplier = lerp(mixTintColor.a * mixMapValue.a, mixMapValue.g * mixMapValue.a, step(0.5, mixDiffParams.x));
    half3 mixColor       = mixMapValue.rgb * mixTintColor.rgb;
    return lerp(color.rgb, mixColor, alphaMultiplier * mixDiffParams.y);
}

// flow map
inline half4 ApplyFlowMap(sampler2D baseMap, real2 uv, float3 flowDir, real _inkFlowSpeed, real _inkFlowScale)
{
    float2 direction = flowDir.xy * 2.0 - 1.0;
    real phase0 = frac(_Time.y * _inkFlowSpeed);
    real phase1 = frac(_Time.y * _inkFlowSpeed + 0.5);
    real4 albedo0 = SampleColorTex(baseMap, uv - direction.xy * phase0 * _inkFlowScale * flowDir.z);
    real4 albedo1 = SampleColorTex(baseMap, uv - direction.xy * phase1 * _inkFlowScale * flowDir.z);
    real flow = abs(1 - phase0 * 2.0);
    return lerp(albedo0, albedo1, flow);
}

#endif