#ifndef BEAUTIFY_PPSEA_FX
#define BEAUTIFY_PPSEA_FX

    // Copyright 2019 Ramiro Oliva (Kronnect) - All Rights Reserved.

    #include "BeautifyOptions.hlsl"

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	TEXTURE2D_SAMPLER2D(_EALumSrc, sampler_EALumSrc);
    TEXTURE2D_SAMPLER2D(_EAHist, sampler_EAHist);
	float4    _MainTex_TexelSize;
	float4    _MainTex_ST;
	float4    _EyeAdaptation;

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

	VaryingsBasic vert(AttributesDefault v) {
    	VaryingsBasic o;
    	o.vertex = float4(v.vertex.xy, 0.0, 1.0);
	    o.uv = TransformTriangleVertexToUV(v.vertex.xy);
		#if UNITY_UV_STARTS_AT_TOP
    		o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
		#endif
    	return o;
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


    float getLuma(float3 rgb) {
        const float3 lum = float3(0.299, 0.587, 0.114);
        return dot(rgb, lum);
    }

    float4 fragScreenLum (VaryingsBasic i) : SV_Target {
        float4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
        #if UNITY_COLORSPACE_GAMMA
        c.rgb = SRGBToLinear(c.rgb);
        #endif
        c.r = log(1.0 + getLuma(c.rgb));
        return c.rrrr;
    }  
    
    float4 fragReduceScreenLum (VaryingsCross i) : SV_Target {
        float4 c1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv1);
        float4 c2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv2);
        float4 c3 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv3);
        float4 c4 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv4);
        c1.g = max( c1.g, max( c2.g, max( c3.g, c4.g )));
        c1.r = (c1.r + c2.r + c3.r + c4.r) * 0.25;
        return c1;
    }       

    float4 fragBlendScreenLum (VaryingsBasic i) : SV_Target {
        float4 c     = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, 0.5.xx);
        float4 p     = SAMPLE_TEXTURE2D(_EAHist, sampler_EAHist, 0.5.xx);
        float speed  = c.r < p.r ? _EyeAdaptation.z: _EyeAdaptation.w;
        c.a = speed * unity_DeltaTime.x;
        return c;
    }  
    
    float4 fragBlend (VaryingsBasic i) : SV_Target {
        float4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, 0.5.xx);
        c.a = 1.0;
        return c;
    }  

#endif