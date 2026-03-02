#ifndef _VFX_COMMON_PASS_
#define _VFX_COMMON_PASS_

Varyings Vertex(Attributes input)
{
    Varyings output = (Varyings)0;
	// varyings data uv mapping info : 
	// uv0.xy --> origin uv0.xy
	// uv0.zw --> diffUV
	// uv1.xy --> maskUV
	// uv1.zw --> mixBaseUV
	
	output.uv0.xy = input.uv0.xy;

	#if defined(_REQUIRE_CUSTOMDATA)
		float4 customDataUV1 = input.uv0.xyxy + input.uv1;
	#endif

	// diffUV
    #if defined(_REQUIRE_CUSTOMDATA)
		float2 diffUV = customDataUV1.xy;
	#else
		float2 diffUV = lerp(input.uv0.xy, input.uv1.xy, step(0.5, _BaseMapSwitchUV1));
    #endif
	
	diffUV = RotateScaleUV(diffUV, _MainTexAngle, _MainTexScale);
	output.uv0.zw = diffUV;

	// maskUV
	#if defined(_REQUIRE_CUSTOMDATA)
		float2 maskUV = customDataUV1.zw;
	#else
		float2 maskUV = lerp(input.uv0.xy, input.uv1.xy, step(0.5, _MaskMapSwitchUV1));
	#endif
	
	maskUV = RotateScaleUV(maskUV, _MaskAngle, _MaskScale);
	output.uv1.xy = maskUV;

	// mixBaseUV
    #if defined(_MIX_BASE_ON)
		float2 mixDiffUV = input.uv0.xy;
		mixDiffUV  = RotateScaleUV(mixDiffUV, _MixBaseMapUVParams.w, _MixBaseMapUVParams.z);
	// #else
		output.uv1.zw = mixDiffUV;
    #endif

	// dissolve
	#if defined(_FALLOFF_DISSOLVE_ON)
		float2 dissolveUV = input.uv0.xy;
	    #if defined(_REQUIRE_CUSTOMDATA)
		dissolveUV += input.uv2.zw;
		output.uv2.zw = max(0, input.uv2.xy);
	    #endif
		// output.uv2.xy = RotateUV(dissolveUV, _DissolveAngle);
		output.uv2.xy = TRANSFORM_TEX(output.uv2.xy, _DissolveTex);
	#endif


	#if defined(VERTEX_REQUIRE_NORMAL)
		half3 normalWS = TransformObjectToWorldNormal(input.normalOS);
	#endif
	
	#if defined(_ENABLE_VERTEX_OFFSET)
		float2 uvVal =  frac(_Time.y * _VertexMotionSpeed) + input.uv0.xy;
		float maskVal = tex2Dlod( _VertexOffsetNoiseMap, float4( uvVal * _VertexOffsetNoiseMap_ST.xy + _VertexOffsetNoiseMap_ST.zw, 0, 0.0) ).r;
		float3 dirVal = _MotionDir==0? input.normalOS:_VertexDir.xyz;
		float uvOffset = _VertexScale;
		float3 offset = dirVal * ( uvOffset * saturate( pow(max(maskVal, 5e-10),_VertexPower) ) );
		float3 vertexValU = lerp(offset, 0, step( _VertexOffsetParams.z ,input.uv0.x ));
		float3 vertexValV = lerp(offset, 0, step( _VertexOffsetParams.w ,input.uv0.y ));
						
		input.positionOS.xyz += max(vertexValU, vertexValV);
	#endif
    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
	
    #if defined(FRAGMENT_REQUIRE_NORMAL)
		output.normalWS.xyz  = normalWS;
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
	// 处理属性颜色
	half4 linearBaseColor        = Gamma20ToLinear(_BaseColor);
	half4 linearBaseFrontColor   = Gamma20ToLinear(_BaseFrontColor);
	half4 linearDiffuseBackColor = Gamma20ToLinear(_DiffuseBackColor);

	#if defined(FRAGMENT_REQUIRE_VERTEXCOLOR)
		half4 linearVertexColor = Gamma20ToLinear(input.vertexColor);
	#endif

	#if defined(_MIX_BASE_ON)
	half4 linearMixTintColor = Gamma20ToLinear(_MixTintColor);
	#endif
	
	#if defined(_GRADIENT_ON)
	half4 linearLeftColor  = Gamma20ToLinear(_LeftColor);
	half4 linearRightColor = Gamma20ToLinear(_RightColor);
	#endif
	
	#if defined(_FALLOFF_DISSOLVE_ON)
	half4 linearDissolveColor = Gamma20ToLinear(_DissolveColor);
	#endif
	
	#if defined(_COLOUR_ON)
	half4 linearSaturationLeftColor  = Gamma20ToLinear(_SaturationLeftColor);
	half4 linearSaturationRightColor = Gamma20ToLinear(_SaturationRightColor);
	#endif
	
	#if defined(_FRESNEL_ON)
	half4 linearFresnelColor   = Gamma20ToLinear(_FresnelColor);
	#endif

	float2 oneHourCycledTime = GetOneHourCycledTime();
	
    half4 faceColor = half4(linearBaseFrontColor.rgb + _FrontIntensity, linearBaseFrontColor.a);
	
	if(_DoubleSideOn > 0.5)
	{
		half4 backColor = half4(linearDiffuseBackColor.rgb + _BackIntensity, linearDiffuseBackColor.a);
		faceColor = facing >= 0 ? faceColor : backColor;
	}
	
	#if defined(_ENABLE_SCREEN_UV)
		float4 screenUV = input.screenPos/input.screenPos.w;
		screenUV.xy = screenUV.xy * _ScreenParams.zw;
	#endif
	
    //mask--------------------------------------
	#if defined(_ENABLE_SCREEN_UV)
		float2 maskUV = screenUV.xy;
	#else
		float2 maskUV = input.uv1.xy;
	#endif

	if (_EnableMaskMapPolarUV > 0.5)
	{
		maskUV = TransformCartesianToPolar(maskUV);
	}
	maskUV = TRANSFORM_TEX(maskUV, _Mask);
	maskUV += frac(_Time.yy * _MaskUVParams.xy);
    half4 maskMapValue = tex2D(_Mask, maskUV);
	if (_MaskMapParams.x > 0.5)
	{
		maskMapValue.rgb = maskMapValue.aaa;
	}
	maskMapValue.r = 1 - saturate(_MaskMapParams.y* (1 - maskMapValue.r));

	
    //diff--------------------------------------
	#if defined(_ENABLE_SCREEN_UV)
		float2 diffUV = screenUV.xy;
	#else
		float2 diffUV = input.uv0.zw;
	#endif

	if (_EnableBaseMapPolarUV > 0.5)
	{
		diffUV = TransformCartesianToPolar(diffUV);
	}
	diffUV = TRANSFORM_TEX(diffUV, _BaseMap);

    #if defined(_NOISE_ON)
        float2 noiseUV = TRANSFORM_TEX(input.uv0.xy, _NoiseTex1);
        noiseUV = noiseUV + frac(_Time.xx * _GChannel.xy);
		noiseUV = tex2D(_NoiseTex1, noiseUV).rg;

	    UNITY_BRANCH
		if(_DualDirectionNoise > 0.5)
		{
			noiseUV = ApplyDualLayerNoise(noiseUV, _GChannel.zw, _NoiseTex2,_NoiseStrength.zw, noiseUV.x * _NoiseStrength);
		}
	    else
	    {
		    noiseUV *= _NoiseStrength.xy;
	    }
		noiseUV *= lerp(1.0, 1 - maskMapValue.b, step(0.5, _EffectByMask));
		diffUV += noiseUV * step(_NoiseUnEffectDiff, 0.5);
    #endif

    
	diffUV += _DouYinEffectParams.yz * step(0.5, _DouYinEffectParams.x);
    diffUV += frac(_Time.yy * _BaseUVParams.xy);


	#if defined(_FLOW_MAP_ON)
		half3 flowMapDir = tex2D(_FlowMap, input.uv0.xy).rgb;
		half4 diffuseMapValue = ApplyFlowMap(_BaseMap, diffUV, flowMapDir, _FlowMapParams.x, _FlowMapParams.y);
	#else
		half4 diffuseMapValue = SampleColorTex(_BaseMap, diffUV);
	#endif

	UNITY_BRANCH
	if (_DouYinEffectParams.x > 0.5)
	{
		half3 col_offset1 = SampleColorTex(_BaseMap, diffUV + _DouYinEffectParams.yz).rgb;
		half3 col_offset2 = SampleColorTex(_BaseMap, diffUV - _DouYinEffectParams.yz).rgb;
		// half3 blendFirstColor = half3(diffuseMapValue.r, diffuseMapValue.g, col_offset1.b);
		diffuseMapValue.rgb = half3(col_offset2.r, diffuseMapValue.g, col_offset1.b + col_offset1.b * col_offset2.r * 0.1);
	}
	
    if (_BaseMultiplyLuminance > 0.5)
    {
    	// use diffuseMapValue.g as approximately luminance value
        diffuseMapValue.a *= diffuseMapValue.g;
    }
    half4 finalColor = diffuseMapValue;
	if (_UnMult > 0.5)
	{
		finalColor.rgb *=  diffuseMapValue.a;
	}
	finalColor *= faceColor;
	finalColor *= linearBaseColor;
	
    #if defined(_MIX_BASE_ON)
		#if defined(_ENABLE_SCREEN_UV)
		float2 mixDiffUV = screenUV.xy;
		#else
		float2 mixDiffUV = input.uv1.zw;
		#endif

		if (_MixBasePolarUVOn > 0.5)
		{
			mixDiffUV = TransformCartesianToPolar(mixDiffUV);
		}
		mixDiffUV  = TRANSFORM_TEX(mixDiffUV, _MixDiffuse);
		// y - _EnableNoiseEffect, z - _EnableRampMode
		half2 enableFlag = step(0.5, _MixMapParams.yz);
		mixDiffUV = lerp(mixDiffUV, float2(1 - diffuseMapValue.r, 0), enableFlag.y);
		#if defined(_NOISE_ON)
			mixDiffUV += noiseUV * step(0.5, enableFlag.x);
	    #endif

		mixDiffUV += frac(_Time.yy* _MixBaseMapUVParams.xy);
		if (_MaskNotEffectMixMap < 0.5)
		{
			linearMixTintColor.a *= maskMapValue.r;
		}
        finalColor.rgb = ApplyMixDiffuse(_MixDiffuse, half4(_MixMapParams.xw, mixDiffUV), linearMixTintColor, finalColor.rgb);
    #endif

    if (_MaskNotEffectDiff < 0.5)
    {
        finalColor.a *= maskMapValue.r;
    }
	
	#if defined(_FALLOFF_DISSOLVE_ON)
		half direction = 0.5;
		half2 mainUV = input.uv0.xy;
		if(_DissolveDir == 0) {direction = mainUV.x;}
		if(_DissolveDir == 1) {direction = mainUV.y;}
		half dissolveMapValue = tex2D(_DissolveTex,frac(mainUV * _DissolveNoiseParam.xy + frac(oneHourCycledTime * _DissolveNoiseParam.zw))).r;
		half edgeMask;
		half dissolve = DissolveWithEdge(direction - _DissolveThreshold, dissolveMapValue, _DissolveFallOff, _DissolveColorThreshold, _DissolveColorFallOff, edgeMask);

		finalColor.rgb  = lerp(finalColor.rgb, _DissolveColor.rgb, edgeMask);
		finalColor.a   *= dissolve;
	#endif

    //gradient--------------------------------------
    #if defined(_GRADIENT_ON)
		float uvWeight = lerp(input.uv0.x, input.uv0.y, _UVWeights);
		uvWeight = lerp(diffUV.x, uvWeight, _GradientSameDiffOn);
		half4 gradientCol = lerp(linearLeftColor,linearRightColor,smoothstep(_LeftWeights+_Gradient,_RightWeights+_Gradient, uvWeight));
		finalColor *= gradientCol;
    #endif

    //fresnel--------------------------------------
    #if defined(_FRESNEL_ON)
	    half3 viewDirWS = normalize(_WorldSpaceCameraPos.xyz - input.positionWS.xyz);
		// half fresnelVar = CalFresnelWS(viewDirWS.xyz, input.normalWS.xyz, _FresnelScale, _FresnelPower);
		half vertexNdotV = dot(input.normalWS.xyz, viewDirWS.xyz);
		half fresnelVar = CalFallOffFresnel(vertexNdotV, _FresnelScale, _FresnelPower);
		#if defined(_REQUIRE_CUSTOMDATA)
			half2 fresnelMapUV    =  input.uv0.xy;
		#else
			half2 fresnelMapUV    = _FresnelMapUse2U < 0.5 ? input.uv0.xy : input.uv0.zw;
		#endif
			half4 fresnelMapValue = SampleColorTex(_FresnelMap, TRANSFORM_TEX(fresnelMapUV, _FresnelMap));
			fresnelMapValue.rgb   = linearFresnelColor.rgb * fresnelVar * fresnelMapValue;

			half fresnelWeight = 1 - fresnelMapValue * linearFresnelColor.a * fresnelMapValue.a;
			finalColor.rgb *= lerp(fresnelWeight, 1, step(0.5, _FresnelParams.w));
			finalColor.rgb += fresnelMapValue.rgb -  fresnelMapValue.rgb * fresnelWeight;
		
			if (_TransparentFresnelPart > 0.5)
			{
				finalColor.a *= saturate(1 - fresnelVar);
			}
    #endif

    #if defined(FRAGMENT_REQUIRE_VERTEXCOLOR)
		finalColor *= linearVertexColor;
    #endif

	#if defined(_ENABLE_PLANAR_SOFT_PARTICLE)
		float verticalDistance = input.positionWS.y - _HorizontalPlaneY;
		// _ContactRange Range(0.001, 1))
		half softAlpha = saturate(verticalDistance * rcp(_ContactRange));
		finalColor.a *= softAlpha;
	#endif

    //color adjust-----------------------------------------
    #if defined(_COLOUR_ON)
		finalColor = AdjustColorWithHSV(diffuseMapValue, half3(_EffectHue, _EffectSaturation, _EffectContrast),
		   linearSaturationLeftColor, linearSaturationRightColor, _SaturationLeftColorWeights, _SaturationRightColorWeights,
		   finalColor);
    #endif
	
    return OutputFXColor(finalColor);
}

#endif
