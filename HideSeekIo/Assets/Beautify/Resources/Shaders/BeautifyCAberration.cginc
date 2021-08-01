#ifndef BEAUTIFY_CABERRATION
#define BEAUTIFY_CABERRATION

	#include "UnityCG.cginc"

	UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
	uniform float4 _MainTex_TexelSize;
	uniform float4 _MainTex_ST;

	#include "BeautifyDistortion.cginc"

    struct appdata {
    	float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
    };
    
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

	
	float4 fragChromaticAberration (v2fSimple i) : SV_Target {
		UNITY_SETUP_INSTANCE_ID(i);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        float4 pixel = GetDistortedColor(i.uv);
  		return pixel;
	}
	
	float4 fragChromaticAberrationFast (v2fSimple i) : SV_Target {
		UNITY_SETUP_INSTANCE_ID(i);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        float4 pixel = GetDistortedColorFast(i.uv);
  		return pixel;
	}

#endif // BEAUTIFY_CABERRATION