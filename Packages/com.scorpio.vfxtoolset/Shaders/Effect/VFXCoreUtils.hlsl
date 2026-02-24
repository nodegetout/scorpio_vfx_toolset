#ifndef _VFX_CORE_UTILS_
#define _VFX_CORE_UTILS_

//=========================  UV Utils  =========================
// UV rotation
float2 RotateScaleUVByCenter(float2 uv, float2 center, half angle, half scale)
{
    // half rotAngle = angle*half(0.017453292519943295);
    float rotAngle = DegToRad(angle);
    float invScale = 1/scale;
    float sinTheta = sin(rotAngle) * invScale;
    float cosTheta = cos(rotAngle) * invScale;
    half2x2 RotMatrix = half2x2(cosTheta, -sinTheta, sinTheta, cosTheta);
    float2 rotateUV = uv - center;   
    rotateUV=mul(RotMatrix,rotateUV);
    rotateUV += center;
    return rotateUV;
}

float2 RotateScaleUV(float2 uv, half angle, half scale)
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

float2 TransformCartesianToPolar(float2 uv)
{
    // uv 假设是 [0,1]，先中心化
    float2 c = uv * 2 - 1;
    float r = length(c);
    float theta = atan2(c.y, c.x) * INV_TWO_PI; // 1/(2π) ≈ 0.15915494
    
    return float2(r, theta);
}

//=========================  Color Utils  =========================
half4 AdjustColorWithHSV(real4 diffuseColor, real3 hsvParams, half4 saturationLeftColor, half4 saturationRightColor, half leftColorWeight, half rightColorWeight, real4 outColor)
{
    outColor.rgb  = RgbToHsv(outColor.rgb);
    outColor.rgb  = lerp(0,HsvToRgb(half3((outColor.r + hsvParams.r)%360, outColor.g * hsvParams.g, outColor.b)), hsvParams.b);
    half colorWeight = smoothstep(leftColorWeight, rightColorWeight, diffuseColor.r*diffuseColor.a);
    outColor *= lerp(saturationLeftColor, saturationRightColor, colorWeight);
    return outColor;
}

inline half3 GammaToLinearSpace (half3 sRGB)
{
    // Approximate version from http://chilliant.blogspot.com.au/2012/08/srgb-approximations-for-hlsl.html?m=1
    return sRGB * (sRGB * (sRGB * 0.305306011h + 0.682171111h) + 0.012522878h);

    // Precise version, useful for debugging.
    //return half3(GammaToLinearSpaceExact(sRGB.r), GammaToLinearSpaceExact(sRGB.g), GammaToLinearSpaceExact(sRGB.b));
}

inline half4 GammaToLinearSpace (half4 color)
{
    return half4(GammaToLinearSpace(color.rgb), color.a);
}

inline half3 LinearToGammaSpace(half3 linRGB)
{
    linRGB = max(linRGB, half3(0.h, 0.h, 0.h));
    // An almost-perfect approximation from http://chilliant.blogspot.com.au/2012/08/srgb-approximations-for-hlsl.html?m=1
    return max(1.055h * pow(linRGB, 0.416666667h) - 0.055h, 0.h);

    // Exact version, useful for debugging.
    //return half3(LinearToGammaSpaceExact(linRGB.r), LinearToGammaSpaceExact(linRGB.g), LinearToGammaSpaceExact(linRGB.b));
}

half4 CustomLinearColor(half4 color)
{
    color.rgb *= color.rgb;
    return color;
}

// sample color texture for color calculation
inline half4 SampleColorTex(sampler2D tex, float2 uv)
{
    half4 mapColorValue = tex2D(tex, uv);
    mapColorValue.rgb   = FastSRGBToLinear(mapColorValue.rgb);
    
    return mapColorValue;
}

// temporal output color
half4 OutputFXColor(half4 color)
{
    #ifndef _COLOR_HDR_
    color.rgb = FastLinearToSRGB(color.rgb);
    #endif
    return color;
}


//========================= Normal　Effects Calculation  =========================
float CalFresnelWS(half3 viewDirWS, half3 normalWS, float scale, float power)
{
    half nDotV = dot(viewDirWS.xyz, normalWS.xyz);
    return scale * pow(max(1-nDotV,0.001), power);
}

inline float SafeSimpleSmoothStep( float x, float edge0, float edge1)
{
    return saturate((x - edge0) / max(0.001, edge1 - edge0));
}

half CalFallOffFresnel(half NoV, half range, half fallOff)
{
    half fresnel = SafeSimpleSmoothStep(NoV, range, range + fallOff);
    return 1 - fresnel;
}

#endif