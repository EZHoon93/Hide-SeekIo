#ifndef BEAUTIFY_PPSLUM_FX
#define BEAUTIFY_PPSLUM_FX

	// Copyright 2019 Ramiro Oliva (Kronnect) - All Rights Reserved.

	#include "BeautifyOptions.hlsl"

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	TEXTURE2D_SAMPLER2D(_BloomTex, sampler_BloomTex);
	float4 	  _BloomTex_TexelSize;
    	TEXTURE2D_SAMPLER2D(_CombineTex, sampler_CombineTex);
	TEXTURE2D_SAMPLER2D(_BloomTex1, sampler_BloomTex1);
	TEXTURE2D_SAMPLER2D(_BloomTex2, sampler_BloomTex2);
	TEXTURE2D_SAMPLER2D(_BloomTex3, sampler_BloomTex3);
	TEXTURE2D_SAMPLER2D(_BloomTex4, sampler_BloomTex4);
	float4    _MainTex_TexelSize;
	float4    _MainTex_ST;
    float4 	  _Bloom;
	float4 	  _BloomWeights;
	float4 	  _BloomWeights2;
    float4 	  _AFTint;
	float     _BlurScale;
    float3    _AFData;

	#if BEAUTIFY_BLOOM_USE_DEPTH || BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
	TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
	float      _BloomDepthThreshold;
    float      _AFDepthThreshold;
	#endif

	#if BEAUTIFY_BLOOM_USE_LAYER
	TEXTURE2D_SAMPLER2D(_BloomSourceTex, sampler_BloomSourceTex);
	TEXTURE2D_SAMPLER2D(_BloomSourceDepth, sampler_BloomSourceDepth);
	float      _BloomLayerZBias;
	#endif

    
	struct VaryingsBasic {
	    float4 vertex : SV_POSITION;
	    float2 uv: TEXCOORD0;
	};

	struct VaryingsCross {
	    float4 vertex : SV_POSITION;
	    float2 uv: TEXCOORD0;
	    float2 uv1: TEXCOORD1;
	    float2 uv2: TEXCOORD2;
	    float2 uv3: TEXCOORD3;
	    float2 uv4: TEXCOORD4;
	};

	struct VaryingsLum {
		float4 vertex : SV_POSITION;
		float2 uv: TEXCOORD0;
		#if BEAUTIFY_BLOOM_USE_DEPTH || BEAUTIFY_BLOOM_USE_LAYER || BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
		float2 depthUV: TEXCOORD1;
		#endif
		#if BEAUTIFY_BLOOM_USE_LAYER && UNITY_SINGLE_PASS_STEREO
		float2 depthUVNonStereo: TEXCOORD2;
		#endif
	};

	struct VaryingsCrossLum {
		float4 vertex : SV_POSITION;
		float2 uv: TEXCOORD0;
		float2 uv1: TEXCOORD1;
		float2 uv2: TEXCOORD2;
		float2 uv3: TEXCOORD3;
		float2 uv4: TEXCOORD4;
		#if BEAUTIFY_BLOOM_USE_DEPTH || BEAUTIFY_BLOOM_USE_LAYER || BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
		float2 depthUV: TEXCOORD5;
		#endif
		#if BEAUTIFY_BLOOM_USE_LAYER && UNITY_SINGLE_PASS_STEREO
		float2 depthUVNonStereo: TEXCOORD6;
		#endif
	};

	VaryingsBasic vert(AttributesDefault v) {
    	VaryingsBasic o;
    	o.vertex = float4(v.vertex.xy, 0.0, 1.0);
	    o.uv = TransformTriangleVertexToUV(v.vertex.xy);
		#if UNITY_UV_STARTS_AT_TOP
    		o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
		#endif
		o.uv = TransformStereoScreenSpaceTex(o.uv, 1.0);
    	return o;
	}

	VaryingsLum vertLum(AttributesDefault v) {
		VaryingsLum o;
		o.vertex = float4(v.vertex.xy, 0.0, 1.0);
        o.uv = TransformTriangleVertexToUV(v.vertex.xy);
		#if BEAUTIFY_BLOOM_USE_LAYER && UNITY_SINGLE_PASS_STEREO
			o.depthUVNonStereo = o.uv;
			o.depthUV = TransformStereoScreenSpaceTex(o.uv, 1.0);
		#else
			#if BEAUTIFY_BLOOM_USE_DEPTH || BEAUTIFY_BLOOM_USE_LAYER || BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
			o.depthUV = o.uv;
			#endif
		#endif
		#if UNITY_UV_STARTS_AT_TOP
    		o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
		#endif
		o.uv = TransformStereoScreenSpaceTex(o.uv, 1.0);
		return o;
	}

	inline float Brightness(float3 c) {
		return max(c.r, max(c.g, c.b));
	}

	float4 fragLum (VaryingsLum i) : SV_Target {
		#if BEAUTIFY_BLOOM_USE_LAYER
		float4 c = SAMPLE_TEXTURE2D(_BloomSourceTex, sampler_BloomSourceTex, i.uv);
		#else
		float4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
		#endif
		c = clamp(c, 0.0.xxxx, _BloomWeights2.zzzz);
   		#if UNITY_COLORSPACE_GAMMA
		c.rgb = SRGBToLinear(c.rgb);
		#endif
		#if BEAUTIFY_BLOOM_USE_DEPTH || BEAUTIFY_BLOOM_USE_LAYER || BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
		float depth01 = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.depthUV));
		#endif
		#if BEAUTIFY_BLOOM_USE_DEPTH
		c.rgb *= (1.0 - depth01 * _BloomDepthThreshold);
        #elif BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
        c.rgb *= (1.0 - depth01 * _AFDepthThreshold);
		#endif
        
		#if BEAUTIFY_BLOOM_USE_LAYER
			#if UNITY_SINGLE_PASS_STEREO
				float depth02 = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_BloomSourceDepth, i.depthUVNonStereo)));
			#else
				float depth02 = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_BloomSourceDepth, i.depthUV)));
			#endif
			float isTransparent = (depth02 >= 1) && any(c.rgb>0);
			float nonEclipsed = isTransparent || (depth01 > depth02 - _BloomLayerZBias);
			c.rgb *= nonEclipsed;
		#endif
		c.a = Brightness(c.rgb);
        #if defined(USE_AF_THRESHOLD)
        c.rgb = max(c.rgb - _AFData.yyy, 0);
        #else
		c.rgb = max(c.rgb - _Bloom.www, 0);
        #endif
   		return c;
   	}

   	VaryingsCross vertCross(AttributesDefault v) {
    	VaryingsCross o;
		o.vertex = float4(v.vertex.xy, 0.0, 1.0);
        o.uv = TransformTriangleVertexToUV(v.vertex.xy);
        #if UNITY_UV_STARTS_AT_TOP
           o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
        #endif
		float3 offsets = _MainTex_TexelSize.xyx * float3(1,1,-1);
		#if UNITY_SINGLE_PASS_STEREO
		offsets.xz *= 2.0;
		#endif
        o.uv1 = TransformStereoScreenSpaceTex(o.uv - offsets.xy, 1.0);
        o.uv2 = TransformStereoScreenSpaceTex(o.uv - offsets.zy, 1.0);
        o.uv3 = TransformStereoScreenSpaceTex(o.uv + offsets.zy, 1.0);
        o.uv4 = TransformStereoScreenSpaceTex(o.uv + offsets.xy, 1.0);
        o.uv = TransformStereoScreenSpaceTex(o.uv, 1.0);           
		return o;
	}

	VaryingsCrossLum vertCrossLum(AttributesDefault v) {
		VaryingsCrossLum o;
		o.vertex = float4(v.vertex.xy, 0.0, 1.0);
        o.uv = TransformTriangleVertexToUV(v.vertex.xy);
		#if BEAUTIFY_BLOOM_USE_LAYER && UNITY_SINGLE_PASS_STEREO
			o.depthUVNonStereo = o.uv;
			#if UNITY_UV_STARTS_AT_TOP
    			o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
			#endif
			o.depthUV = TransformStereoScreenSpaceTex(o.uv, 1.0);
			float3 offsets = _MainTex_TexelSize.xyx * float3(1, 1, -1);
            o.uv1 = TransformStereoScreenSpaceTex(o.uv - offsets.xy, 1.0);
            o.uv2 = TransformStereoScreenSpaceTex(o.uv - offsets.zy, 1.0);
            o.uv3 = TransformStereoScreenSpaceTex(o.uv + offsets.zy, 1.0);
            o.uv4 = TransformStereoScreenSpaceTex(o.uv + offsets.xy, 1.0);
            o.uv = TransformStereoScreenSpaceTex(o.uv, 1.0);        
		#else
			#if BEAUTIFY_BLOOM_USE_DEPTH || BEAUTIFY_BLOOM_USE_LAYER || BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
            o.depthUV = TransformStereoScreenSpaceTex(o.uv, 1.0);
			#endif
			o.uv = TransformTriangleVertexToUV(v.vertex.xy);
            #if UNITY_UV_STARTS_AT_TOP
                o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
            #endif
			float3 offsets = _MainTex_TexelSize.xyx * float3(1, 1, -1);
			o.uv1 = TransformStereoScreenSpaceTex(o.uv - offsets.xy, 1.0);
			o.uv2 = TransformStereoScreenSpaceTex(o.uv - offsets.zy, 1.0);
			o.uv3 = TransformStereoScreenSpaceTex(o.uv + offsets.zy, 1.0);
			o.uv4 = TransformStereoScreenSpaceTex(o.uv + offsets.xy, 1.0);
            o.uv = TransformStereoScreenSpaceTex(o.uv, 1.0);
		#endif
		return o;
	}

   	float4 fragLumAntiflicker(VaryingsCrossLum i) : SV_Target {
		#if BEAUTIFY_BLOOM_USE_LAYER
			float4 c1 = tex2D(_BloomSourceTex, i.uv1);
			float4 c2 = tex2D(_BloomSourceTex, i.uv2);
			float4 c3 = tex2D(_BloomSourceTex, i.uv3);
			float4 c4 = tex2D(_BloomSourceTex, i.uv4);
		#else
			float4 c1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv1);
			float4 c2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv2);
			float4 c3 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv3);
			float4 c4 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv4);
		#endif

		c1 = clamp(c1, 0.0.xxxx, _BloomWeights2.zzzz);
		c2 = clamp(c2, 0.0.xxxx, _BloomWeights2.zzzz);
		c3 = clamp(c3, 0.0.xxxx, _BloomWeights2.zzzz);
		c4 = clamp(c4, 0.0.xxxx, _BloomWeights2.zzzz);

		#if BEAUTIFY_BLOOM_USE_DEPTH || BEAUTIFY_BLOOM_USE_LAYER || BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
		float depth01 = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.depthUV));
		#endif
        
		#if BEAUTIFY_BLOOM_USE_DEPTH
        float depthAtten = 1.0 - depth01 * _BloomDepthThreshold;
		c1.rgb *= depthAtten;
		c2.rgb *= depthAtten;
		c3.rgb *= depthAtten;
		c4.rgb *= depthAtten;
        #elif BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
        float depthAtten = 1.0 - depth01 * _AFDepthThreshold;
        c1.rgb *= depthAtten;
        c2.rgb *= depthAtten;
        c3.rgb *= depthAtten;
        c4.rgb *= depthAtten;
        #endif

                        
		#if BEAUTIFY_BLOOM_USE_LAYER
		#if UNITY_SINGLE_PASS_STEREO
		float depth02 = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_BloomSourceDepth, i.depthUVNonStereo)));
		#else
		float depth02 = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_BloomSourceDepth, i.depthUV)));
		#endif
		float isTransparent = (depth02 >= 1) && any(c1.rgb>0);
		float nonEclipsed = isTransparent || (depth01 > depth02 - _BloomLayerZBias );
		c1.rgb *= nonEclipsed;
		c2.rgb *= nonEclipsed;
		c3.rgb *= nonEclipsed;
		c4.rgb *= nonEclipsed;
		#endif
		
		c1.a = Brightness(c1.rgb);
		c2.a = Brightness(c2.rgb);
		c3.a = Brightness(c3.rgb);
		c4.a = Brightness(c4.rgb);
	    
	    float w1 = 1.0 / (c1.a + 1.0);
	    float w2 = 1.0 / (c2.a + 1.0);
	    float w3 = 1.0 / (c3.a + 1.0);
	    float w4 = 1.0 / (c4.a + 1.0);

	    float dd  = 1.0 / (w1 + w2 + w3 + w4);
	    c1 = (c1 * w1 + c2 * w2 + c3 * w3 + c4 * w4) * dd;
        
   		#if UNITY_COLORSPACE_GAMMA
		c1.rgb = SRGBToLinear(c1.rgb);
		#endif

        #if defined(USE_AF_THRESHOLD)
        c1.rgb = max(c1.rgb - _AFData.yyy, 0);
        #else
        c1.rgb = max(c1.rgb - _Bloom.www, 0);
        #endif

   		return c1;
	}

	float4 fragBloomCompose (VaryingsBasic i) : SV_Target {
		float4 b0 = SAMPLE_TEXTURE2D( _BloomTex  , sampler_BloomTex, i.uv );
		float4 b1 = SAMPLE_TEXTURE2D( _BloomTex1 , sampler_BloomTex1, i.uv );
		float4 b2 = SAMPLE_TEXTURE2D( _BloomTex2 , sampler_BloomTex2, i.uv );
		float4 b3 = SAMPLE_TEXTURE2D( _BloomTex3 , sampler_BloomTex3, i.uv );
		float4 b4 = SAMPLE_TEXTURE2D( _BloomTex4 , sampler_BloomTex4, i.uv );
		float4 b5 = SAMPLE_TEXTURE2D( _MainTex   , sampler_MainTex, i.uv );
		float4 pixel = b0 * _BloomWeights.x + b1 * _BloomWeights.y + b2 * _BloomWeights.z + b3 * _BloomWeights.w + b4 * _BloomWeights2.x + b5 * _BloomWeights2.y;
		return pixel;
	}

	float4 fragResample(VaryingsCross i) : SV_Target {
		float4 c1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv1);
		float4 c2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv2);
		float4 c3 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv3);
		float4 c4 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv4);
			    
	    float w1 = 1.0 / (c1.a + 1.0);
	    float w2 = 1.0 / (c2.a + 1.0);
	    float w3 = 1.0 / (c3.a + 1.0);
	    float w4 = 1.0 / (c4.a + 1.0);
	    
	    float dd  = 1.0 / (w1 + w2 + w3 + w4);
	    float4 v = (c1 * w1 + c2 * w2 + c3 * w3 + c4 * w4) * dd;
        #if defined(COMBINE_BLOOM)
        float4 o = SAMPLE_TEXTURE2D(_BloomTex, sampler_BloomTex, i.uv);
        v += o;
        #endif
        return v;
	}

	float4 fragResampleAF(VaryingsCross i) : SV_Target {
		float4 c1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv1);
		float4 c2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv2);
		float4 c3 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv3);
		float4 c4 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv4);
			    
	    float w1 = 1.0 / (c1.a + 1.0);
	    float w2 = 1.0 / (c2.a + 1.0);
	    float w3 = 1.0 / (c3.a + 1.0);
	    float w4 = 1.0 / (c4.a + 1.0);
	    
	    float dd  = 1.0 / (w1 + w2 + w3 + w4);
	    c1 = (c1 * w1 + c2 * w2 + c3 * w3 + c4 * w4) * dd;
	    c1.rgb = lerp(c1.rgb, Brightness(c1.rgb) * _AFTint.rgb, _AFTint.a);
	    c1.rgb *= _AFData.xxx;
        
        #if defined(COMBINE_BLOOM)
        float4 o = SAMPLE_TEXTURE2D(_BloomTex, sampler_BloomTex, i.uv);
        c1 += o;
        #endif
        
	    return c1;
	}

	float4 fragCopy(VaryingsBasic i) : SV_Target {
		return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
	}
	
    float4 fragCombine(VaryingsBasic i) : SV_Target {
        float4 c1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
        float4 c2 = SAMPLE_TEXTURE2D(_CombineTex, sampler_CombineTex, i.uv);
        return c1 + c2;
    }
    
    
	float4 fragDebugBloom (VaryingsBasic i) : SV_Target {
		return SAMPLE_TEXTURE2D(_BloomTex, sampler_BloomTex, i.uv) * _Bloom.xxxx;
	}
	
	float4 fragResampleFastAF(VaryingsBasic i) : SV_Target {
		float4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
	    c.rgb = lerp(c.rgb, Brightness(c.rgb) * _AFTint.rgb, _AFTint.a);
	    c.rgb *= _AFData.xxx;
	    return c;
	}	
	
	VaryingsCross vertBlurH(AttributesDefault v) {
    	VaryingsCross o;
		o.vertex = float4(v.vertex.xy, 0.0, 1.0);
    	o.uv = TransformTriangleVertexToUV(v.vertex.xy);
        #if UNITY_UV_STARTS_AT_TOP
            o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
        #endif
		float2 inc = float2(_MainTex_TexelSize.x * 1.3846153846 * _BlurScale, 0);	
#if UNITY_SINGLE_PASS_STEREO
		inc.x *= 2.0;
#endif
    	o.uv1 = TransformStereoScreenSpaceTex(o.uv - inc, 1.0);	
    	o.uv2 = TransformStereoScreenSpaceTex(o.uv + inc, 1.0);	
		float2 inc2 = float2(_MainTex_TexelSize.x * 3.2307692308 * _BlurScale, 0);	
#if UNITY_SINGLE_PASS_STEREO
		inc2.x *= 2.0;
#endif
		o.uv3 = TransformStereoScreenSpaceTex(o.uv - inc2, 1.0);
    	o.uv4 = TransformStereoScreenSpaceTex(o.uv + inc2, 1.0);	
        o.uv = TransformStereoScreenSpaceTex(o.uv, 1.0);    
		return o;
	}	
	
	VaryingsCross vertBlurV(AttributesDefault v) {
    	VaryingsCross o;
		o.vertex = float4(v.vertex.xy, 0.0, 1.0);
    	o.uv = TransformTriangleVertexToUV(v.vertex.xy);
        #if UNITY_UV_STARTS_AT_TOP
            o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
        #endif
    	float2 inc = float2(0, _MainTex_TexelSize.y * 1.3846153846 * _BlurScale);	
    	o.uv1 = TransformStereoScreenSpaceTex(o.uv - inc, 1.0);	
    	o.uv2 = TransformStereoScreenSpaceTex(o.uv + inc, 1.0);	
    	float2 inc2 = float2(0, _MainTex_TexelSize.y * 3.2307692308 * _BlurScale);	
    	o.uv3 = TransformStereoScreenSpaceTex(o.uv - inc2, 1.0);	
    	o.uv4 = TransformStereoScreenSpaceTex(o.uv + inc2, 1.0);	
        o.uv = TransformStereoScreenSpaceTex(o.uv, 1.0);    
    	return o;
	}
	
	float4 fragBlur (VaryingsCross i): SV_Target {
		float4 pixel = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * 0.2270270270
					+ (SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv1) + SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv2)) * 0.3162162162
					+ (SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv3) + SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv4)) * 0.0702702703;
   		return pixel;
	}	

#endif