#ifndef BEAUTIFY_CORE_FX
#define BEAUTIFY_CORE_FX

	// Copyright 2019 Ramiro Oliva (Kronnect) - All Rights Reserved.

	#include "BeautifyOptions.hlsl"
    #include "BeautifyOrtho.hlsl"
    #include "BeautifyACESFitted.hlsl"

    float4 _CompareParams;
    TEXTURE2D_SAMPLER2D(_CompareTex, sampler_CompareTex);
	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
	TEXTURE2D_SAMPLER2D(_BloomTex, sampler_BloomTex);
    TEXTURE2D_SAMPLER2D(_ScreenLum, sampler_ScreenLum);
    TEXTURE2D_SAMPLER2D(_OverlayTex, sampler_OverlayTex);
    TEXTURE2D_SAMPLER2D(_LUTTex, sampler_LUTTex);

    float4 _MainTex_TexelSize;
	float4 _Params;
	float4 _Sharpen;
	float4 _Bloom;
    float4 _Dirt;    // x = brightness based, y = intensity, z = threshold, w = bloom contribution    
    float4 _FXColor;
    float3 _ColorBoost;
    float4 _TintColor;
    float4 _Purkinje;
    float4 _EyeAdaptation;
    float4 _BokehData;
    float4 _BokehData2;
    float4 _Outline;

    TEXTURE2D_SAMPLER2D(_EAHist, sampler_EAHist);
    TEXTURE2D_SAMPLER2D(_EALumSrc, sampler_EALumSrc);
    #if BEAUTIFY_VIGNETTING || BEAUTIFY_VIGNETTING_MASK
        float4 _Vignetting;
        float _VignettingAspectRatio;
        TEXTURE2D_SAMPLER2D(_VignettingMask, sampler_VignettingMask);
    #endif

    #if BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT || BEAUTIFY_DEPTH_OF_FIELD
        TEXTURE2D_SAMPLER2D(_DoFTex, sampler_DoFTex);
        float4 _DoFTex_TexelSize;
    #endif
    //#if BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
    //    TEXTURE2D_SAMPLER2D(_DepthTexture, sampler_DepthTexture);
    //    TEXTURE2D_SAMPLER2D(_DofExclusionTexture, sampler_DoFExclusionTexture);
    //#endif



	struct VaryingsBeautify {
    	float4 vertex : SV_POSITION;
    	float2 uv : TEXCOORD0;
	    float2 uvStereo : TEXCOORD1;
    	float2 depthUV : TEXCOORD2;	  
  	    float2 uvStereoN: TEXCOORD3;
	    float2 uvStereoS: TEXCOORD4;
	    float2 uvStereoW: TEXCOORD5;
	    float2 uvStereoE: TEXCOORD6;
	};

    struct VaryingsBeautifyCompare {
        float4 vertex : SV_POSITION;
        float2 uv : TEXCOORD0;
        float2 uvStereo : TEXCOORD1;
    };

	VaryingsBeautify VertBeautify(AttributesDefault v) {
	    VaryingsBeautify o;
	    o.vertex = float4(v.vertex.xy, 0.0, 1.0);
	    o.uv = TransformTriangleVertexToUV(v.vertex.xy);
		#if UNITY_UV_STARTS_AT_TOP
    		o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
		#endif
    	o.uvStereo = TransformStereoScreenSpaceTex(o.uv, 1.0);
        o.depthUV = o.uvStereo + 0.000001; // workaround for driver/compiler bug
    	float3 uvInc = float3(_MainTex_TexelSize.x, _MainTex_TexelSize.y, 0);
    	o.uvStereoN = o.depthUV + uvInc.zy;
    	o.uvStereoE = o.depthUV + uvInc.xz;
    	o.uvStereoW = o.depthUV - uvInc.xz;
    	o.uvStereoS = o.depthUV - uvInc.zy;
    	return o;
	}

    VaryingsBeautifyCompare VertCompare(AttributesDefault v) {
        VaryingsBeautifyCompare o;
        o.vertex = float4(v.vertex.xy, 0.0, 1.0);
        o.uv = TransformTriangleVertexToUV(v.vertex.xy);
        #if UNITY_UV_STARTS_AT_TOP
            o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
        #endif
        o.uvStereo = TransformStereoScreenSpaceTex(o.uv, 1.0);
        return o;
    }

	inline float getLuma(float3 rgb) { 
		const float3 lum = float3(0.299, 0.587, 0.114);
		return dot(rgb, lum);
	}

    float3 getNormal(float depth, float depth1, float depth2, float2 offset1, float2 offset2) {
        float3 p1 = float3(offset1, depth1 - depth);
        float3 p2 = float3(offset2, depth2 - depth);
        float3 normal = cross(p1, p2);
        return normalize(normal);
    }

    float getCoc(VaryingsBeautify i) {
   /* #if BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
        float depthTex = DecodeFloatRGBA(SAMPLE_TEXTURE2D_LOD(_DepthTexture, sampler_DepthTexture, i.uv, 0));
        float exclusionDepth = DecodeFloatRGBA(SAMPLE_TEXTURE2D_LOD(_DofExclusionTexture, sampler_DoFExclusionTexture, i.uv, 0));
        float depth  = BEAUTIFY_GET_DEPTH_01(SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, i.depthUV, 0));
        depth = min(depth, depthTex);
        if (exclusionDepth < depth) return 0;
        depth *= _ProjectionParams.z;
    #else*/
        float depth  = BEAUTIFY_GET_DEPTH_EYE(SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, i.depthUV, 0));
    //#endif
        float xd     = abs(depth - _BokehData.x) - _BokehData2.x * (depth < _BokehData.x);
        return 0.5 * _BokehData.y * xd/depth;   // radius of CoC
    }

		
	void beautifyPass(VaryingsBeautify i, inout float3 rgbM) {
		float3 uvInc      = float3(_MainTex_TexelSize.x, _MainTex_TexelSize.y, 0);
		float  depthS     = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uvStereoS));
		float  depthW     = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uvStereoW));
		float  depthE     = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uvStereoE));		
		float  depthN     = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uvStereoN));
		float  lumaM      = getLuma(rgbM);

		// daltonize
		float3 rgb0       = 1.0.xxx - saturate(rgbM.rgb);
		       rgbM.r    *= 1.0 + rgbM.r * rgb0.g * rgb0.b * _Params.y;
			   rgbM.g    *= 1.0 + rgbM.g * rgb0.r * rgb0.b * _Params.y;
			   rgbM.b    *= 1.0 + rgbM.b * rgb0.r * rgb0.g * _Params.y;	
			   rgbM      *= lumaM / (getLuma(rgbM) + 0.0001);

		// sharpen
		float  maxDepth   = max(depthN, depthS);
		       maxDepth   = max(maxDepth, depthW);
		       maxDepth   = max(maxDepth, depthE);
		float  minDepth   = min(depthN, depthS);
		       minDepth   = min(minDepth, depthW);
		       minDepth   = min(minDepth, depthE);
		float  dDepth     = maxDepth - minDepth + 0.00001;

		float  lumaDepth  = saturate(_Sharpen.y / dDepth);
		float3 rgbS       = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uvStereoS).rgb;
	    float3 rgbW       = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uvStereoW).rgb;
	    float3 rgbE       = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uvStereoE).rgb;
	    float3 rgbN       = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uvStereoN).rgb;
	    
    	float  lumaN      = getLuma(rgbN);
    	float  lumaE      = getLuma(rgbE);
    	float  lumaW      = getLuma(rgbW);
    	float  lumaS      = getLuma(rgbS);
		
    	float  maxLuma    = max(lumaN,lumaS);
    	       maxLuma    = max(maxLuma, lumaW);
    	       maxLuma    = max(maxLuma, lumaE);
	    float  minLuma    = min(lumaN,lumaS);
	           minLuma    = min(minLuma, lumaW);
	           minLuma    = min(minLuma, lumaE) - 0.000001;
	    float  lumaPower  = 2.0 * lumaM - minLuma - maxLuma;
		float  lumaAtten  = saturate(_Sharpen.w / (maxLuma - minLuma));
		float  depthClamp = abs(depthW - _Params.z) < _Params.w;		
		       rgbM      *= 1.0 + clamp(lumaPower * lumaAtten * lumaDepth * _Sharpen.x, -_Sharpen.z, _Sharpen.z) * depthClamp;

        #if BEAUTIFY_DEPTH_OF_FIELD || BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
            float4 dofPix     = SAMPLE_TEXTURE2D(_DoFTex, sampler_DoFTex, i.uvStereo);
            #if UNITY_COLORSPACE_GAMMA
               dofPix.rgb = LinearToSRGB(dofPix.rgb);
            #endif
            if (_DoFTex_TexelSize.z < _MainTex_TexelSize.z) {
                float  CoC = getCoc(i) / COC_BASE;
                dofPix.a   = lerp(CoC, dofPix.a, _DoFTex_TexelSize.z / _MainTex_TexelSize.z);
            }
            rgbM = lerp(rgbM, dofPix.rgb, saturate(dofPix.a * COC_BASE)); 
        #endif

        #if BEAUTIFY_OUTLINE
            #if !defined(BEAUTIFY_OUTLINE_SOBEL)
                #if !BEAUTIFY_NIGHT_VISION
                    float depth       = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uvStereo));
                    float3  normalNW  = getNormal(depth, depthN, depthW, uvInc.zy, float2(-uvInc.x, -uvInc.z));
                #endif
                float3 normalSE   = getNormal(depth, depthS, depthE, -uvInc.zy,  uvInc.xz);
                float  dnorm      = dot(normalNW, normalSE);
                rgbM              = lerp(rgbM, _Outline.rgb, (float)(dnorm  < _Outline.a));
            #else
                float4 uv4 = float4(i.uv, 0, 0);
                #if BEAUTIFY_NIGHT_VISION || BEAUTIFY_THERMAL_VISION
                    float3 rgbS       = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 - uvInc.zyzz).rgb;
                    float3 rgbN       = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 + uvInc.zyzz).rgb;
                    float3 rgbW       = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 - uvInc.xzzz).rgb;
                    float3 rgbE       = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 + uvInc.xzzz).rgb;
                #endif
                float3 rgbSW = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 - uvInc.xyzz).rgb;    // was tex2Dlod
                float3 rgbNE = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 + uvInc.xyzz).rgb;
                float3 rgbSE = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 + float4( uvInc.x, -uvInc.y, 0, 0)).rgb;
                float3 rgbNW = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 + float4(-uvInc.x,  uvInc.y, 0, 0)).rgb;
                float3 gx  = rgbSW * -1.0;
                       gx += rgbSE *  1.0;
                       gx += rgbW  * -2.0;
                       gx += rgbE  *  2.0;
                       gx += rgbNW * -1.0;
                       gx += rgbNE *  1.0;
                float3 gy  = rgbSW * -1.0;
                       gy += rgbS  * -2.0;
                       gy += rgbSE * -1.0;
                       gy += rgbNW *  1.0;
                       gy += rgbN  *  2.0;
                       gy += rgbNE *  1.0;
                float olColor = (length(gx * gx + gy * gy) - _Outline.a) > 0.0;
                rgbM = lerp(rgbM, _Outline.rgb, olColor); 
            #endif
        #endif

		#if UNITY_COLORSPACE_GAMMA
		rgbM = SRGBToLinear(rgbM);
		#endif

		#if BEAUTIFY_BLOOM
		rgbM += SAMPLE_TEXTURE2D(_BloomTex, sampler_BloomTex, i.uvStereo).rgb * _Bloom.xxx;
		#endif

        #if BEAUTIFY_DIRT
            float3 scrLum = SAMPLE_TEXTURE2D(_ScreenLum, sampler_ScreenLum, i.uvStereo).rgb;
            #if BEAUTIFY_BLOOM
            scrLum *= _Dirt.www;
            #endif
            float4 dirt = SAMPLE_TEXTURE2D(_OverlayTex, sampler_OverlayTex, i.uv);
            rgbM       += saturate(0.5.xxx - _Dirt.zzz + scrLum) * dirt.rgb * _Dirt.y; 
        #endif

        #if BEAUTIFY_EYE_ADAPTATION || BEAUTIFY_PURKINJE
        float4 avgLum = SAMPLE_TEXTURE2D(_EAHist, sampler_EAHist, 0.5.xx);
        #endif

        #if BEAUTIFY_EYE_ADAPTATION
            float srcLum  = SAMPLE_TEXTURE2D(_EALumSrc, sampler_EALumSrc, 0.5.xx).r;
            float  diff   = srcLum / (avgLum.r + 0.0001);
            float pixLum  = max(0,log(1.0 + getLuma(rgbM)));
            diff   = pow(pixLum / (avgLum.r + 0.0001), abs(diff-1.0));
            diff   = clamp(diff, _EyeAdaptation.x, _EyeAdaptation.y);
            rgbM   = rgbM * diff;
        #endif

        #if BEAUTIFY_TONEMAP_ACES
             rgbM *= _FXColor.r;
             rgbM    = ACESFitted(rgbM);
             rgbM *= _FXColor.g;
        #endif

		#if UNITY_COLORSPACE_GAMMA
		rgbM    = LinearToSRGB(rgbM);
		#endif

        #if BEAUTIFY_LUT
            #if !UNITY_COLORSPACE_GAMMA
            rgbM = LinearToSRGB(rgbM);
            #endif
        
            const float3 lutST = float3(1.0/1024, 1.0/32, 32-1);
            float3 lookUp = saturate(rgbM) * lutST.zzz;
            lookUp.xy = lutST.xy * (lookUp.xy + 0.5);
            float slice = floor(lookUp.z);
            lookUp.x += slice * lutST.y;
            float2 lookUpNextSlice = float2(lookUp.x + lutST.y, lookUp.y);
            float3 lut = lerp(SAMPLE_TEXTURE2D(_LUTTex, sampler_LUTTex, lookUp.xy).rgb, SAMPLE_TEXTURE2D(_LUTTex, sampler_LUTTex, lookUpNextSlice).rgb, lookUp.z - slice);
            rgbM = lerp(rgbM, lut, _FXColor.a);
            
            #if !UNITY_COLORSPACE_GAMMA
            rgbM = SRGBToLinear(rgbM);
            #endif
        #endif


 		// sepia
		float3 sepia      = float3(
   		            	   			dot(rgbM, float3(0.393, 0.769, 0.189)),
               						dot(rgbM, float3(0.349, 0.686, 0.168)),
               						dot(rgbM, float3(0.272, 0.534, 0.131))
               					  );
        rgbM      = lerp(rgbM, sepia, _Params.x);

        // saturate
        float maxComponent = max(rgbM.r, max(rgbM.g, rgbM.b));
        float minComponent = min(rgbM.r, min(rgbM.g, rgbM.b));
        float sat = saturate(maxComponent - minComponent);
        rgbM *= 1.0 + _ColorBoost.z * (1.0 - sat) * (rgbM - getLuma(rgbM));
        rgbM = lerp(rgbM, rgbM * _TintColor.rgb, _TintColor.a);
        rgbM = (rgbM - 0.5.xxx) * _ColorBoost.y + 0.5.xxx;
        rgbM *= _ColorBoost.x;

        #if BEAUTIFY_PURKINJE
              lumaM    = getLuma(rgbM);
        float3 shifted  = saturate(float3(lumaM / (1.0 + _Purkinje.x * 1.14), lumaM, lumaM * (1.0 + _Purkinje.x * 2.99)));
              rgbM     = lerp(shifted, rgbM, saturate(exp(avgLum.g) - _Purkinje.y));
        #endif

#if BEAUTIFY_VIGNETTING
            float2 vd = float2(i.uv.x  - 0.5, (i.uv.y - 0.5) * _VignettingAspectRatio);
            rgbM = lerp(_Vignetting.rgb, rgbM, saturate( saturate((dot(vd, vd) - _Vignetting.a) / (_Purkinje.w - _Vignetting.a) ) - _Purkinje.z)) ;
#elif BEAUTIFY_VIGNETTING_MASK
            float2 vd = float2(i.uv.x - 0.5, (i.uv.y - 0.5) * _VignettingAspectRatio);
            float  vmask = SAMPLE_TEXTURE2D(_VignettingMask, sampler_VignettingMask, i.uv).a;
            rgbM = lerp(rgbM, lumaM * _Vignetting.rgb, saturate(_Purkinje.z + vmask * _Vignetting.a * dot(vd, vd)));
#endif

	}

	float4 FragBeautify (VaryingsBeautify i) : SV_Target {
   		float4 pixel = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uvStereo);
   		beautifyPass(i, pixel.rgb);
   		return pixel;
	}


    float4 FragCompare (VaryingsBeautify i) : SV_Target {
        // separator line + antialias
        float2 dd     = i.uv - 0.5.xx;
        float  co     = dot(_CompareParams.xy, dd);
        float  dist   = distance( _CompareParams.xy * co, dd );
        float4 aa     = saturate( (_CompareParams.w - dist) / abs(_MainTex_TexelSize.y) );

        float4 pixel  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uvStereo);
        float4 pixelNice = SAMPLE_TEXTURE2D(_CompareTex, sampler_CompareTex, i.uv);
        
        // are we on the beautified side?
        float t       = dot(dd, _CompareParams.yz) > 0;
        pixel         = lerp(pixel, pixelNice, t);
        return pixel + aa;
    }

#endif // BEAUTIFY_CORE_FX