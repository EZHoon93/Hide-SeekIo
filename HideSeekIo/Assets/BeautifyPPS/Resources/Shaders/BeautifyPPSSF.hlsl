#ifndef BEAUTIFY_PPSSF_FX
#define BEAUTIFY_PPSSF_FX		

	// Copyright 2019 Ramiro Oliva (Kronnect) - All Rights Reserved.

	#include "BeautifyOptions.hlsl"

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);

	sampler _FlareTex;
	float4 _MainTex_ST;
	float4 _MainTex_TexelSize;
	float4	  _SunPos;
	float4    _SunData;	// x = sunIntensity, y = disk size, z = ray difraction, w = ray difraction amount
	float4    _SunCoronaRays1;  // x = length, y = streaks, z = spread, w = angle offset
	float4    _SunCoronaRays2;  // x = length, y = streaks, z = spread, w = angle offset
	float4    _SunGhosts1;  // x = reserved, y = size, 2 = pos offset, 3 = brightness
	float4    _SunGhosts2;  // x = reserved, y = size, 2 = pos offset, 3 = brightness
	float4    _SunGhosts3;  // x = reserved, y = size, 2 = pos offset, 3 = brightness
	float4    _SunGhosts4;  // x = reserved, y = size, 2 = pos offset, 3 = brightness
   	float3    _SunHalo;  // x = offset, y = amplitude, z = intensity
   	float3    _SunTint;

	struct VaryingsBasic {
	    float4 vertex : SV_POSITION;
	    float2 uv: TEXCOORD0;
		float2 uvNonStereo: TEXCOORD1;
	};
	
	VaryingsBasic vert(AttributesDefault v) {
	    VaryingsBasic o;
	    o.vertex = float4(v.vertex.xy, 0.0, 1.0);
	    o.uv = TransformTriangleVertexToUV(v.vertex.xy);
		#if UNITY_UV_STARTS_AT_TOP
    		o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
		#endif
		o.uvNonStereo = o.uv;
		o.uv = TransformStereoScreenSpaceTex(o.uv, 1.0);
    	return o;
	}
   	
	void rotate(inout float2 uv, float ang) {
		float2 sico;
		sincos(ang, sico.x, sico.y);
		float2 cosi = float2(sico.y, -sico.x);
		uv = float2(dot(cosi, uv), dot(sico, uv));
	}
	
   	float3 sunflare(float2 uv) {

		// general params
   		float2 sunPos = _SunPos.xy;

		float2 depthSunPos = sunPos;
  		#if UNITY_UV_STARTS_AT_TOP
   			if (_MainTex_TexelSize.y < 0) {
        	// Depth texture is inverted WRT the main texture
   	    	depthSunPos.y = 1.0 - depthSunPos.y;
   		}
   		#endif
   		float depth  = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, depthSunPos));
		if (depth<1) return 0;

   		float2 grd = uv - sunPos;
		float aspectRatio = _ScreenParams.y / _ScreenParams.x;
   		grd.y *= aspectRatio; 
   		float len = length(grd);

   		// sun disk
   		float s0 = pow( 1.0 + saturate(_SunData.y - len), 75) - 1.0;
        
   		// corona rays
		float gang = _SunPos.w; //atan2(0.5 - sunPos.y, sunPos.x - 0.5);
   		float ang = atan2(grd.y, grd.x) + gang;
   		float ray1 = _SunCoronaRays1.z + abs(_SunCoronaRays1.x * cos(_SunCoronaRays1.w + ang * _SunCoronaRays1.y));	// design
   		ray1 *= pow( 1.0 + len, 1.0/_SunCoronaRays1.x);	
   		s0 += 1.0 / ray1;

   		float ray2 = _SunCoronaRays2.z + abs(_SunCoronaRays2.x * sin(_SunCoronaRays2.w + ang * _SunCoronaRays2.y));	// design
   		ray2 *= pow( 1.0 + len, 1.0/_SunCoronaRays2.x);	
   		s0 += 1.0 / ray2;
   		
   		s0 *= _SunData.x;
   		
		// ghosts hexagonal
//		float gang = atan2( (sunPos.y * _ScreenParams.y / _ScreenParams.x) - 0.5, sunPos.x - 0.5); // this angle should be passed by uniform
//		grd = uv - ghost1Pos + (ghost1Pos - 0.5) * ghost1PosOffset;
//		grd.y *= _ScreenParams.y / _ScreenParams.x;
//		rotate(grd, gang);
//		grd *= ghost1Size;
//   		s0 +=  tex2D(_FlareTex, grd + 0.5.xx).r * ghost1Brighness;
//
//   		grd = uv - ghost2Pos + (ghost2Pos - 0.5) * ghost2PosOffset;
//		grd.y *= _ScreenParams.y / _ScreenParams.x;
//		rotate(grd, gang);
//		grd *= ghost2Size;
//   		s0 += tex2D(_FlareTex, grd + 0.5.xx).r * ghost2Brighness;
   		
   		float3 flare = s0.xxx;

   		// ghosts circular
   		float2 ghost1Pos  = 1.0 - sunPos;
   		grd = uv - ghost1Pos + (ghost1Pos - 0.5) * _SunGhosts1.z;
		grd.y *= aspectRatio;
		float g0 = saturate(_SunGhosts1.y / length(grd)); 
		g0 = pow(g0, 12);
   		flare += g0 * _SunGhosts1.w / len;

   		float2 ghost2Pos  = 1.0 - sunPos;
   		grd = uv - ghost2Pos + (ghost2Pos - 0.5) * _SunGhosts2.z;
		grd.y *= aspectRatio;
		g0 = saturate(_SunGhosts2.y / length(grd)); 
		g0 = pow(g0, 12);
   		flare +=  g0 * _SunGhosts2.w / len;

   		float2 ghost3Pos  = 1.0 - sunPos;
   		grd = uv - ghost3Pos + (ghost3Pos - 0.5) * _SunGhosts3.z;
		grd.y *= aspectRatio;
		g0 = saturate(_SunGhosts3.y / length(grd)); 
		g0 = pow(g0, 12);
   		flare +=  g0 * _SunGhosts3.w / len;

   		float2 ghost4Pos  = 1.0 - sunPos;
   		grd = uv - ghost4Pos + (ghost4Pos - 0.5) * _SunGhosts4.z;
		grd.y *= aspectRatio;
		g0 = saturate(_SunGhosts4.y / length(grd)); 
		g0 = pow(g0, 12);
   		flare +=  g0 * _SunGhosts4.w / len;
   		
		// light rays
		float2 uv2 = uv - 0.5.xx;
		float clen = length(uv2);
		rotate(uv2, gang);
		uv2.x *= aspectRatio;
		uv2.x *= 0.1;
		uv2 /= len;
		float lr = saturate(tex2D(_FlareTex, uv2 + _SunPos.zz).r - _SunData.w);
		float3 rays = lr * sin(float3(len, len + 0.1, len + 0.2) * 3.1415927);
		float atten = pow(1.0 + clen, 13.0);
		rays *= _SunData.z / atten;
		flare += rays;

		// halo
		float hlen = clamp( (len - _SunHalo.x) * _SunHalo.y, 0, 3.1415927);
		float3 halo = pow(sin(float3(hlen, hlen + 0.1, hlen + 0.2)), 12.0.xxx);
		halo *= _SunHalo.z / atten;
		flare += halo;
		
		return flare * _SunTint;
   	}  
   	
   	 float3 sunflareFast(float2 uv) {
   	
		// general params
   		float2 sunPos = _SunPos.xy;
   		
   		float2 grd = uv - sunPos;
   		float aspectRatio = _ScreenParams.y / _ScreenParams.x;
   		grd.y *= aspectRatio; 
   		float len = length(grd);
   		
   		// corona rays
   		float ang = atan2(grd.y, grd.x);
   		float ray1 = _SunCoronaRays1.z + abs(_SunCoronaRays1.x * cos(_SunCoronaRays1.w + ang * _SunCoronaRays1.y));	// design
   		ray1 *= pow( 1.0 + len, 1.0/_SunCoronaRays1.x);	
   		float s0 = 1.0 / ray1;

   		s0 *= _SunData.x;

   		float3 flare = s0.xxx;

   		// ghosts circular
   		float2 ghost1Pos  = 1.0 - sunPos;
   		grd = uv - ghost1Pos + (ghost1Pos - 0.5) * _SunGhosts1.z;
		grd.y *= aspectRatio;
		float g0 = saturate(_SunGhosts1.y / length(grd)); 
		g0 = pow(g0, 12);
   		flare += g0 * _SunGhosts1.w / len;

   		float2 ghost2Pos  = 1.0 - sunPos;
   		grd = uv - ghost2Pos + (ghost2Pos - 0.5) * _SunGhosts2.z;
		grd.y *= aspectRatio;
		g0 = saturate(_SunGhosts2.y / length(grd)); 
		g0 = pow(g0, 12);
   		flare +=  g0 * _SunGhosts2.w / len;

		// halo
		float hlen = clamp( (len - _SunHalo.x) * _SunHalo.y, 0, 3.1415927);
		float3 halo = pow(sin(float3(hlen, hlen + 0.1, hlen + 0.2)), 12.0.xxx);
		float clen = length(uv - 0.5.xx);
		float atten = pow(1.0 + clen, 13.0);
		halo *= _SunHalo.z / atten;
		flare += halo;
		
		return flare * _SunTint;
   	}  
   	
   	
  	float4 fragSF (VaryingsBasic i) : SV_Target {
   		return float4(sunflare(i.uvNonStereo), 1.0);
   	}  

  	float4 fragSFAdditive (VaryingsBasic i) : SV_Target {
  		float4 p = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
   		return p + float4(sunflare(i.uvNonStereo), 1.0);
   	}  
   	
  	float4 fragSFFast (VaryingsBasic i) : SV_Target {
   		return float4(sunflareFast(i.uvNonStereo), 1.0);
   	}  

  	float4 fragSFFastAdditive (VaryingsBasic i) : SV_Target {
  		float4 p = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
   		return p + float4(sunflareFast(i.uvNonStereo), 1.0);
   	} 

#endif