Shader "Hidden/BeautifyPPSCore" {

HLSLINCLUDE
    #pragma target 3.0
ENDHLSL


Subshader {	

  ZTest Always Cull Off ZWrite Off

  Pass { // 0
      HLSLPROGRAM
      #pragma vertex VertCompare
      #pragma fragment FragCompare
      #include "BeautifyCore.hlsl"
      ENDHLSL
  }

  Pass { // 1
      HLSLPROGRAM
      #pragma vertex VertBeautify
      #pragma fragment FragBeautify
      //#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
      #pragma multi_compile __ BEAUTIFY_LUT
      #pragma multi_compile __ BEAUTIFY_DIRT
	  #pragma multi_compile __ BEAUTIFY_BLOOM
      #pragma multi_compile __ BEAUTIFY_TONEMAP_ACES
      #pragma multi_compile __ BEAUTIFY_EYE_ADAPTATION
      #pragma multi_compile __ BEAUTIFY_PURKINJE
      #pragma multi_compile __ BEAUTIFY_VIGNETTING BEAUTIFY_VIGNETTING_MASK
      #pragma multi_compile __ BEAUTIFY_DEPTH_OF_FIELD 
      #pragma multi_compile __ BEAUTIFY_OUTLINE
	  //#pragma multi_compile __ BEAUTIFY_DEPTH_OF_FIELD BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
      #include "BeautifyCore.hlsl"
      ENDHLSL
  }

  Pass { // 2
      HLSLPROGRAM
      #pragma vertex vertLum
      #pragma fragment fragLum
	  //#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
	  #pragma multi_compile __ BEAUTIFY_BLOOM_USE_DEPTH
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }  

  Pass { // 3
      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment fragDebugBloom
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }  

  Pass { // 4
      HLSLPROGRAM
      #pragma vertex vertBlurH
      #pragma fragment fragBlur
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }    
      
  Pass { // 5
	  HLSLPROGRAM
      #pragma vertex vertBlurV
      #pragma fragment fragBlur
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }    

  Pass { // 6
	  HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment fragBloomCompose
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }   

  Pass { // 7
	  HLSLPROGRAM
      #pragma vertex vertCross
      #pragma fragment fragResample
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  } 

  Pass { // 8
      //Blend One One
	  HLSLPROGRAM
      #pragma vertex vertCross
      #pragma fragment fragResample
      #define COMBINE_BLOOM
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }   

  Pass { // 9
	  HLSLPROGRAM
      #pragma vertex vertCrossLum
      #pragma fragment fragLumAntiflicker
      //#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
      #pragma multi_compile __ BEAUTIFY_BLOOM_USE_DEPTH
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  } 

   Pass { // 10
      //Blend One One
	  HLSLPROGRAM
      #pragma vertex vertCross
      #pragma fragment fragResampleAF
      #define COMBINE_BLOOM
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }

  Pass { // 11
      //Blend One One
	  HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment fragCombine
      #define COMBINE_BLOOM
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  } 

  Pass { // 12 Raw copy
      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment fragCopy
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  } 

  Pass { // 13 Compute Screen Lum
      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment fragScreenLum
      //#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
      #include "BeautifyPPSEA.hlsl"
      ENDHLSL
  }      
  
  Pass { // 14 Reduce Screen Lum
      HLSLPROGRAM
      #pragma vertex vertCross
      #pragma fragment fragReduceScreenLum
      #include "BeautifyPPSEA.hlsl"
      ENDHLSL
  }  

  Pass { // 15 Blend Screen Lum
      Blend SrcAlpha OneMinusSrcAlpha
      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment fragBlendScreenLum
      #include "BeautifyPPSEA.hlsl"
      ENDHLSL
  }      
  
  Pass { // 16 Simple Blend
      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment fragBlend
      #include "BeautifyPPSEA.hlsl"
      ENDHLSL
  }  

  Pass { // 17 AF Lum
      HLSLPROGRAM
      #pragma vertex vertLum
      #pragma fragment fragLum
      //#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
      #pragma multi_compile __ BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
      #define USE_AF_THRESHOLD
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }  

  Pass { // 18 AF Lum AntiFlicker
      HLSLPROGRAM
      #pragma vertex vertCrossLum
      #pragma fragment fragLumAntiflicker
      //#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
      #pragma multi_compile __ BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
      #define USE_AF_THRESHOLD
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  } 

 Pass { // 19 Sun Flares
      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment fragSF
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyPPSSF.hlsl"
      ENDHLSL
  }
  
    Pass { // 20 Sun Flares Additive
      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment fragSFAdditive
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyPPSSF.hlsl"
      ENDHLSL
  }

 Pass { // 21 DoF CoC
      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment fragCoC
      #pragma fragmentoption ARB_precision_hint_fastest
      //#pragma multi_compile __ BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
      //#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  } 
 
  Pass { // 22 DoF CoC Debug
      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment fragCoCDebug
      #pragma fragmentoption ARB_precision_hint_fastest
      //#pragma multi_compile __ BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  } 
 
  Pass { // 23 DoF Blur
      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment fragBlur
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  }    

 Pass { // 24 DoF Blur wo/Bokeh
      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment fragBlurNoBokeh
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  }    

Pass { // 25 DoF Blur CoC
      HLSLPROGRAM
      #pragma vertex vertBlurH
      #pragma fragment fragBlurCoC
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  }    

Pass { // 26 DoF Blur CoC
      HLSLPROGRAM
      #pragma vertex vertBlurV
      #pragma fragment fragBlurCoC
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  }    


}
FallBack Off
}
