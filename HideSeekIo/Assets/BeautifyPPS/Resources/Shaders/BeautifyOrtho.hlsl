#ifndef __BEAUTIFY_ORTHO_INCLUDE
#define __BEAUTIFY_ORTHO_INCLUDE

// Fix for Oculus GO & Stereo Rendering
#if SHADER_API_MOBILE && (defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED))
    #define BEAUTIFY_GET_DEPTH_01(x) saturate(Linear01Depth(x))
    #define BEAUTIFY_GET_DEPTH_EYE(x) clamp(LinearEyeDepth(x), 0, _ProjectionParams.z)
#else
    #define BEAUTIFY_GET_DEPTH_01(x) Linear01Depth(x)
    #define BEAUTIFY_GET_DEPTH_EYE(x) LinearEyeDepth(x)
#endif

#if defined(BEAUTIFY_ORTHO)
    #if UNITY_REVERSED_Z
  		#define Linear01Depth(x) (1.0-x)
       	#define LinearEyeDepth(x) ((1.0-x) * _ProjectionParams.z)
    #else
	   	#define Linear01Depth(x) (x)
   		#define LinearEyeDepth(x) (x * _ProjectionParams.z)
    #endif
#endif

#endif // __BEAUTIFY_ORTHO_INCLUDE