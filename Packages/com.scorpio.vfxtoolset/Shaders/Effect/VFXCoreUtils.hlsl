#ifndef _VFX_CORE_UTILS_
#define _VFX_CORE_UTILS_

//=========================  UV Utils  =========================
// UV rotation
float2 RotateUVWithCenter(float2 uv, float2 center, float angle)
{
    float2 maskUV = uv-center;
    // maskUV += _NoiseEffectMaskStreng * noiseUV;
    float MaskcosR = cos((angle * 1/180.0) * PI);
    float MasksinR = sin((angle * 1/180.0) * PI);
    half2x2 MaskRot = half2x2(MaskcosR,-MasksinR,MasksinR,MaskcosR);
    maskUV = mul(MaskRot,maskUV)+center;
    return maskUV;
}

half2 RotateScaleUVByCenter(float2 uv, float2 center, half angle, half scale)
{
    // half rotAngle = angle*half(0.017453292519943295);
    float rotAngle = angle*PI/180.0;
    float sinTheta = sin(rotAngle);
    float cosTheta = cos(rotAngle);
    half2x2 RotMatrix = half2x2(cosTheta, -sinTheta, sinTheta, cosTheta);
    float2 rotateUV= uv - center;   
    rotateUV=mul(RotMatrix,rotateUV);
    float invScale = 1/scale;
    rotateUV *= invScale;
    rotateUV += center;
    return rotateUV;
}

half2 RotateScaleUV(float2 uv, half angle, half scale)
{
    return RotateScaleUVByCenter(uv, float2(0.5, 0.5), angle, scale);
}

float2 RotateUV(float2 uv, half angle)
{
    return RotateScaleUVByCenter(uv, float2(0.5, 0.5), angle, 1.0);
}

// UV panner
float2 UVPanner(float2 uv, float2 tiling, float2 speed, float time)
{
    return uv * tiling + speed * time;
}

float4 UVPanner(float4 uv, float4 tiling, float4 speed, float time)
{
    return uv * tiling + speed * time;
}

float2 TransformCartesianToPolar(float2 uv, float2 center)
{
    float2 centeredUV = uv - center;
    return float2(length(centeredUV) * 2,  atan2(centeredUV.x, centeredUV.y) * INV_TWO_PI);
}

//=========================  Color Utils  =========================
half4 AdjustColorWithHSV(real4 diffuseColor, real3 hsvParams, half4 saturationLeftColor, half4 saturationRightColor, half leftColorWeight, half rightColorWeight, real4 outColor)
{
    outColor.rgb  = RgbToHsv(outColor.rgb);
    outColor.rgb  = lerp(0,HsvToRgb(half3((outColor.r + hsvParams.r)%360, outColor.g * hsvParams.g, outColor.b)), hsvParams.b);
    // return half4(hsvParams, 1);
    half colorWeight = smoothstep(leftColorWeight,rightColorWeight, diffuseColor.r*diffuseColor.a);
    outColor.rgb *= lerp(saturationLeftColor.rgb, saturationRightColor.rgb, colorWeight);
    outColor.a   *= lerp(saturationLeftColor.a,saturationRightColor.a, colorWeight);
    return outColor;
}


//========================= Normal　Effects Calculation  =========================
float CalFresnelWS(half3 viewDirWS, half3 normalWS, float scale, float power)
{
    half nDotV = dot(viewDirWS.xyz, normalWS.xyz);
    return scale * pow(max(1-nDotV,0.001), power);
}

#endif