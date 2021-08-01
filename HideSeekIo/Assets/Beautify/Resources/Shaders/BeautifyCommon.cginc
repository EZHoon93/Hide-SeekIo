#ifndef __BEAUTIFY_COMMON
#define __BEAUTIFY_COMMON

// Common Beautify functions

#if defined(BEAUTIFY_HARD_LIGHT)
	float3 _HardLight;

	float3 ApplyHardLight(float3 rgbM, float lumaM) {
		rgbM = saturate(rgbM);
		float3 hc = rgbM * _HardLight.y;
		if (lumaM < 0.5f) {
			hc *= 2.0 * rgbM;
		} else {
			hc = 1.0 - 2.0 * (1.0 - rgbM) * (1.0 - hc);
		}
		rgbM = lerp(rgbM, hc, _HardLight.x);
		return rgbM;
    }

#endif

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


#endif // __BEAUTIFY_COMMON