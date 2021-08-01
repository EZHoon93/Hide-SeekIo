	// Copyright 2016-2021 Ramiro Oliva (Kronnect) - All Rights Reserved.
	
	#include "UnityCG.cginc"
	#include "BeautifyAdvancedParams.cginc"
	#include "BeautifyOrtho.cginc"
	#include "BeautifyCommon.cginc"

	UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
	UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
	
	uniform float4 _MainTex_TexelSize;
	uniform float4 _MainTex_ST;
	uniform float4 _Outline;
	half      _BlurScale;
	half      _OutlineIntensityMultiplier;

    struct appdata {
    	float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
    };
    
	struct v2f {
	    float4 pos : SV_POSITION;
	    float2 uv: TEXCOORD0;
    	float4 depthUV : TEXCOORD1;	    
		UNITY_VERTEX_INPUT_INSTANCE_ID
		UNITY_VERTEX_OUTPUT_STEREO
	};

	struct v2fCross {
		float4 pos : SV_POSITION;
		float2 uv: TEXCOORD0;
	    float2 uv1: TEXCOORD1;
	    float2 uv2: TEXCOORD2;
	    float2 uv3: TEXCOORD3;
	    float2 uv4: TEXCOORD4;
		UNITY_VERTEX_INPUT_INSTANCE_ID
		UNITY_VERTEX_OUTPUT_STEREO
	};


	v2f vert(appdata v) {
    	v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_TRANSFER_INSTANCE_ID(v, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    	o.pos = UnityObjectToClipPos(v.vertex);
   		o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
    	o.depthUV = float4(o.uv, 0, 0);
    	#if UNITY_UV_STARTS_AT_TOP
    	if (_MainTex_TexelSize.y < 0) {
	        // Depth texture is inverted WRT the main texture
    	    o.depthUV.y = 1.0 - o.depthUV.y;
		}
    	#endif
    	return o;
	}

	float OutlinePass(v2f i) {

		float3 uvInc      = float3(_MainTex_TexelSize.x, _MainTex_TexelSize.y, 0);

		#if !defined(BEAUTIFY_OUTLINE_SOBEL)
	        float  depth      = Linear01Depth(BEAUTIFY_DEPTH_LOD(_CameraDepthTexture, i.depthUV));
			float  depthS     = Linear01Depth(BEAUTIFY_DEPTH_LOD(_CameraDepthTexture, i.depthUV - uvInc.zyzz));
			float  depthW     = Linear01Depth(BEAUTIFY_DEPTH_LOD(_CameraDepthTexture, i.depthUV - uvInc.xzzz));
			float  depthE     = Linear01Depth(BEAUTIFY_DEPTH_LOD(_CameraDepthTexture, i.depthUV + uvInc.xzzz));		
			float  depthN     = Linear01Depth(BEAUTIFY_DEPTH_LOD(_CameraDepthTexture, i.depthUV + uvInc.zyzz));
   			float3 normalNW   = getNormal(depth, depthN, depthW, uvInc.zy, float2(-uvInc.x, -uvInc.z));
   			float3 normalSE   = getNormal(depth, depthS, depthE, -uvInc.zy,  uvInc.xz);
			float  dnorm      = dot(normalNW, normalSE);
   			return (float)(dnorm  < _Outline.a);
   		#else
	   		float4 uv4 = float4(i.uv, 0, 0);
			float3 rgbS   = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 - uvInc.zyzz).rgb;
	   		float3 rgbN   = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 + uvInc.zyzz).rgb;
	    	float3 rgbW   = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 - uvInc.xzzz).rgb;
    		float3 rgbE   = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 + uvInc.xzzz).rgb;
			float3 rgbSW = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, uv4 - uvInc.xyzz).rgb;	// was tex2Dlod
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
			return olColor; 
   		#endif
	}
	
	float4 fragOutline (v2f i) : SV_Target {
		UNITY_SETUP_INSTANCE_ID(i);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
   		float outline = OutlinePass(i);
  		return outline;
	}


	v2fCross vertCross(appdata v) {
		v2fCross o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_TRANSFER_INSTANCE_ID(v, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
		float3 offsets = _MainTex_TexelSize.xyx * half3(1.75,1.75,0);
		#if UNITY_SINGLE_PASS_STEREO
		    offsets.x *= 2.0;
		#endif
		o.uv1 = UnityStereoScreenSpaceUVAdjust(v.texcoord - offsets.zy, _MainTex_ST);
		o.uv2 = UnityStereoScreenSpaceUVAdjust(v.texcoord - offsets.xz, _MainTex_ST);
		o.uv3 = UnityStereoScreenSpaceUVAdjust(v.texcoord + offsets.xz, _MainTex_ST);
		o.uv4 = UnityStereoScreenSpaceUVAdjust(v.texcoord + offsets.zy, _MainTex_ST);
		return o;
	}

	v2fCross vertBlurH(appdata v) {
    	v2fCross o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_TRANSFER_INSTANCE_ID(v, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    	o.pos = UnityObjectToClipPos(v.vertex);
		#if UNITY_UV_STARTS_AT_TOP
    	if (_MainTex_TexelSize.y < 0) {
	        // Texture is inverted WRT the main texture
    	    v.texcoord.y = 1.0 - v.texcoord.y;
    	}
    	#endif   
    	o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
		half2 inc = half2(_MainTex_TexelSize.x * 1.3846153846 * _BlurScale, 0);	
#if UNITY_SINGLE_PASS_STEREO
		inc.x *= 2.0;
#endif
    	o.uv1 = UnityStereoScreenSpaceUVAdjust(v.texcoord - inc, _MainTex_ST);	
    	o.uv2 = UnityStereoScreenSpaceUVAdjust(v.texcoord + inc, _MainTex_ST);	
		half2 inc2 = half2(_MainTex_TexelSize.x * 3.2307692308 * _BlurScale, 0);	
#if UNITY_SINGLE_PASS_STEREO
		inc2.x *= 2.0;
#endif
		o.uv3 = UnityStereoScreenSpaceUVAdjust(v.texcoord - inc2, _MainTex_ST);
    	o.uv4 = UnityStereoScreenSpaceUVAdjust(v.texcoord + inc2, _MainTex_ST);	
		return o;
	}	
	
	v2fCross vertBlurV(appdata v) {
    	v2fCross o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_TRANSFER_INSTANCE_ID(v, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    	o.pos = UnityObjectToClipPos(v.vertex);
		#if UNITY_UV_STARTS_AT_TOP
    	if (_MainTex_TexelSize.y < 0) {
	        // Texture is inverted WRT the main texture
    	    v.texcoord.y = 1.0 - v.texcoord.y;
    	}
    	#endif   
    	o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
    	half2 inc = half2(0, _MainTex_TexelSize.y * 1.3846153846 * _BlurScale);	
    	o.uv1 = UnityStereoScreenSpaceUVAdjust(v.texcoord - inc, _MainTex_ST);	
    	o.uv2 = UnityStereoScreenSpaceUVAdjust(v.texcoord + inc, _MainTex_ST);	
    	half2 inc2 = half2(0, _MainTex_TexelSize.y * 3.2307692308 * _BlurScale);	
    	o.uv3 = UnityStereoScreenSpaceUVAdjust(v.texcoord - inc2, _MainTex_ST);	
    	o.uv4 = UnityStereoScreenSpaceUVAdjust(v.texcoord + inc2, _MainTex_ST);	
    	return o;
	}
	
	half4 fragBlur (v2fCross i): SV_Target {
		UNITY_SETUP_INSTANCE_ID(i);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		half4 pixel = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv) * 0.2270270270
					+ (UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv1) + UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv2)) * 0.3162162162
					+ (UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv3) + UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv4)) * 0.0702702703;
   		return pixel;
	}	

	struct v2fSimple {
		float4 pos : SV_POSITION;
		float2 uv: TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
		UNITY_VERTEX_OUTPUT_STEREO
	};

	v2fSimple vertSimple(appdata v) {
		v2fSimple o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_TRANSFER_INSTANCE_ID(v, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    	o.pos = UnityObjectToClipPos(v.vertex);
    	o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
		
		#if UNITY_UV_STARTS_AT_TOP
    	if (_MainTex_TexelSize.y < 0) {
	        // Depth texture is inverted WRT the main texture
    	    o.uv.y = 1.0 - o.uv.y;
    	}
    	#endif    	
    	return o;
	}


	half4 fragCopy(v2fSimple i) : SV_Target {
		UNITY_SETUP_INSTANCE_ID(i);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		half outline = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv).r;
		half4 color = half4(_Outline.rgb, outline);
		color *= _OutlineIntensityMultiplier;
		color.a = saturate(color.a);
		return color;
	}