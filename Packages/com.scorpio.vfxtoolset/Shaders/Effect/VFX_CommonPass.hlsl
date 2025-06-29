#ifndef _VFX_COMMON_PASS_
#define _VFX_COMMON_PASS_

#include "./VFX_CommonModule.hlsl"

Varyings Vertex(Attributes input)
{
    Varyings output = (Varyings)0;
    #if defined(_REQUIRE_CUSTOMDATA)
		float2 noiseUV     = input.uv0.xy;
		float4 uv1Control  = input.uv0.xyxy + input.uv1;
		float2 dissolveCoe = max(0, input.uv2.xy);
		float2 dissolveUV  = input.uv0 + input.uv2.zw;
		output.uv0.xy = noiseUV;
		output.uv0.zw = uv1Control.zw;
		output.uv1.xy = uv1Control.xy;
		output.uv1.zw = dissolveUV;
        output.uv2.xy = dissolveCoe;
    #else
		output.uv0.xy = input.uv0.xy;
		#ifndef _ENABLE_SCREEN_UV
			output.uv0.zw = input.uv1.xy;
		#endif
    #endif

    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);

	#if defined(_ENABLE_VERTEX_OFFSET)
		positionWS = positionWS + ApplyVertexOffsetWS(output.uv0.xy, _VONoiseTiling, _VOSpeedParams, _VertexOffsetNoiseMap, _VOScale);
	#endif

    #if defined(REQUIRE_NORMAL)
		output.normalWS.xyz  = TransformObjectToWorldNormal(input.normalOS);
    #endif

    #if defined(VERTEX_REQUIRE_VERTEXCOLOR)
		output.vertexColor = input.vertexColor;
    #endif

	#if defined(REQUIRE_POSITIONWS)
	output.positionWS.xyz = positionWS;
	#endif

    output.positionHCS = TransformWorldToHClip(positionWS);
	
	#if defined(_ENABLE_SCREEN_UV)
	output.screenPos = ComputeScreenPos(output.positionHCS);
	#endif
	
    return output;
}

half4 Fragment(Varyings input, half facing : VFACE) : SV_Target
{
    half4 faceColor = half4(_BaseColor.rgb * _FrontIntensity, _BaseColor.a);

    #if defined(_DOUBLE_SIDE_ON)
        half4 backColor = half4(_DiffuseBackColor.rgb * _BackIntensity, _DiffuseBackColor.a);
        faceColor = facing >= 0 ? faceColor : backColor;
    #endif
	
	#if defined(_ENABLE_SCREEN_UV)
		float4 screenUV = input.screenPos/input.screenPos.w;
		screenUV.xy = screenUV.xy * _ScreenParams.zw;
	#endif
	

    //mask--------------------------------------
    #if defined(_REQUIRE_CUSTOMDATA)
        float2 maskUV = input.uv0.zw;
    #else
		#if defined(_ENABLE_SCREEN_UV)
			float2 maskUV = screenUV.xy;
		#else
			float2 maskUV = input.uv0.xy;
		    if (_MaskMapSwitchUV1)
		    {
    			maskUV = input.uv0.zw;
		    }
	    #endif
    #endif

    if (_EnableMaskMapPolarUV > 0.5)
    {
        maskUV = TransformCartesianToPolar(maskUV, 0.5);
    }
    maskUV += frac(_Time.yy * _MaskUVParams.xy);
    maskUV = RotateScaleUV(maskUV, _MaskAngle, _MaskScale);
    half4 maskMapValue = tex2D(_Mask, TRANSFORM_TEX(maskUV, _Mask));

    //diff--------------------------------------
    #if defined(_REQUIRE_CUSTOMDATA)
		float2 diffUV = input.uv1.xy;
    #else
		#if defined(_ENABLE_SCREEN_UV)
			float2 diffUV = screenUV.xy;
		#else
			float2 diffUV = input.uv0.xy;
			if (_BaseMapSwitchUV1)
			{
				diffUV = input.uv0.zw;
			}
		#endif
    #endif

    #if defined(_DISSOLVE_NOISE_ON)
        float2 noiseUV = TRANSFORM_TEX(input.uv0.xy, _DissolveTex);
        float2 layer1UV = noiseUV + frac(_Time.xx * _GChannel.xy);
		half4 noiseMapValue = tex2D(_DissolveTex, layer1UV);

	    #if defined(_DUAL_LAYER_NOISE)
	         noiseUV = ApplyDualLayerNoise(noiseUV, _GChannel.zw, _NoiseTex2, _NoiseStrength1, noiseMapValue.r * _NoiseStrength);
	    #else
             noiseUV = _NoiseStrength * noiseMapValue.g;
	    #endif

        if (_EffectByMask > 0.5)
		{
			noiseUV = lerp(noiseUV,0,maskMapValue.b);
		}
	
		if(!_NoiseUnEffectDiff)
		{
			diffUV += noiseUV;
		}
    #endif

    if (_EnableBaseMapPolarUV > 0.5)
    {
        diffUV = TransformCartesianToPolar(diffUV, 0.5);
    }

    float2 rotatedDiffUV = RotateScaleUV(diffUV, _MainTexAngle, _MainTexScale);
    float2 finalDiffUV = TRANSFORM_TEX(rotatedDiffUV, _BaseMap);
    finalDiffUV += _Time.yy * _BaseUVParams.xy;
    half4 diffuseMapValue = tex2D(_BaseMap, finalDiffUV);
    if (_BaseMultiplyLuminance > 0.5)
    {
        diffuseMapValue.a *= Luminance(diffuseMapValue.rgb);
    }
    half3 diffColor = half3(diffuseMapValue.rgb/**_DiffusePower*/);
    half alphaWeight = lerp(1, diffuseMapValue.a, _UnMult);
    diffColor *= alphaWeight;

    half4 finalColor;
    finalColor.rgb = diffColor;
    finalColor.a = diffuseMapValue.a * _BaseColor.a;

    #if defined(_MIX_BASE_ON)
	    #if defined(_REQUIRE_CUSTOMDATA)
			float2 mixDiffUV = input.uv1.xy;
	    #else
			float2 mixDiffUV = input.uv0.xy;
			if (_EnableRampMode < 0.5)
			{
				mixDiffUV  = RotateScaleUV(mixDiffUV, _MixBaseMapUVParams.w, _MixBaseMapUVParams.z);
				mixDiffUV  = TRANSFORM_TEX(mixDiffUV, _MixDiffuse);
				mixDiffUV += _Time.yy* _MixBaseMapUVParams.xy;
			}else
			{
				mixDiffUV = float2(diffColor.r, 0) + _MixRampUVOffset;
			}
	    #endif
        finalColor.rgb = ApplyMixDiffuse(_MixDiffuse, mixDiffUV, _MixMultiplyLuminance, _MixDiffusePower, finalColor.rgb);
    #endif

    if (_MaskNotEffectDiff < 0.5)
    {
        finalColor.a *= maskMapValue.r;
    }

    //dissolve_noise--------------------------------------
    #if defined(_DISSOLVE_NOISE_ON)
	    #if defined(_REQUIRE_CUSTOMDATA)
            float2 dissolveUV = input.uv1.zw;
	    #else
            float2 dissolveUV = input.uv0.xy;
	    #endif
            dissolveUV        = RotateUV(dissolveUV, _DissolveAngle);
			half4 dissolveVar = tex2D(_DissolveTex,TRANSFORM_TEX(dissolveUV + noiseUV, _DissolveTex));
            	
			half softTmp         = _SoftSize;
			half dissolveStepTmp = _DissolveStep;

	    #if defined(_REQUIRE_CUSTOMDATA)
            			// input.uv2.x - DissolveCoe.x,  input.uv2.y - DissolveCoe.y
			softTmp         += input.uv2.x;
			dissolveStepTmp += input.uv2.y;
	    #endif
            	
		if (_MaskedDissolve > 0.5)
		{
			dissolveStepTmp = lerp(dissolveStepTmp, 0, maskMapValue.g);
		}
           
		half3 dissolveParams = half3(dissolveVar.r, softTmp, dissolveStepTmp);
		half3 dissolveOutlineParams = half3(_DissolveOutlineWidth, _DissolveOutlineSoft, _DissolveOutlineOn);
		finalColor = ApplyCommonDissolve(dissolveParams, dissolveOutlineParams, _DissolveColor, _DissolveColorPW, finalColor);
    #endif

    //gradient--------------------------------------
    #if defined(_GRADIENT_ON)
        float uvWeight = lerp(input.uv0.x, input.uv0.y, _UVWeights);
        half tmpX = lerp(finalDiffUV.x, uvWeight, _GradientSameDiffOn);
        half4 gradientCol = lerp(_LeftColor,_RightColor,smoothstep(_LeftWeights+_Gradient,_RightWeights+_Gradient,tmpX));
		finalColor.rgb *= gradientCol.rgb;
		finalColor.a *= gradientCol.a;
    #endif

    //fresnel--------------------------------------
    #if defined(_FRESNEL_ON)
		half3 viewDirWS = normalize(_WorldSpaceCameraPos.xyz - input.positionWS.xyz);
		half fresnelVar = CalFresnelWS(viewDirWS.xyz, input.normalWS.xyz, _FresnelScale, _FresnelPower);
		half3 fresnel   = _FresnelColor.rgb * fresnelVar * _FresnelColor.a;
            	
		finalColor.rgb  = saturate(finalColor.rgb + fresnel);
            	
		if (_TransparentFresnelPart > 0.5)
		{
			finalColor.a *= saturate(1 - fresnelVar);
		}
    #endif

    #if defined(FRAGMENT_REQUIRE_VERTEXCOLOR)
		finalColor.rgb *= input.vertexColor.rgb;
	    finalColor.a *= input.vertexColor.a;
    #endif

	#if defined(_ENABLE_PLANAR_SOFT_PARTICLE)
		float verticalDistance = input.positionWS.y - _HorizontalPlaneY;
		float SoftAlpha = saturate((verticalDistance) / _ContactRange);
		finalColor.a *= SoftAlpha;
	#endif

    finalColor.rgb *= faceColor.rgb;
    finalColor.a *= faceColor.a;

    //color adjust-----------------------------------------
    #if defined(_COLOUR_ON)
        finalColor = AdjustColorWithHSV(diffuseMapValue, half3(_EffectHue, _EffectSaturation, _EffectContrast),
            _SaturationLeftColor, _SaturationRightColor, _SaturationLeftColorWeights, _SaturationRightColorWeights,
            finalColor);
    #endif

    // TODO: tonemapping!!!
    // TonemapForward(finalColor.rgb);
    return finalColor;
}

#endif
