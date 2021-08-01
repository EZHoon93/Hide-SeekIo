#ifndef BEAUTIFY_PPSDOF_FX
#define BEAUTIFY_PPSDOF_FX

    // Copyright 2019 Ramiro Oliva (Kronnect) - All Rights Reserved.
    
    #include "BeautifyOptions.hlsl"
    #include "BeautifyOrtho.hlsl"

    TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
    TEXTURE2D_SAMPLER2D(_DepthTexture, sampler_DepthTexture);
    TEXTURE2D_SAMPLER2D(_DofExclusionTexture, sampler_DofExclusionTexture);    
    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

    float4    _MainTex_TexelSize;
    float4    _MainTex_ST;
    float4      _BokehData;
    float4    _BokehData2;
    float3    _BokehData3;
    float     _BlurScale;

    struct VaryingsDoF {
        float4 vertex : SV_POSITION;
        float2 uv: TEXCOORD0;
        float2 depthUV : TEXCOORD1;                
        float2 uvNonStereo: TEXCOORD2;
        float2 uvNonStereoInv: TEXCOORD3;
    };

    struct VaryingsDoFCross {
        float4 vertex : SV_POSITION;
        float2 uv: TEXCOORD0;
        float2 uv1: TEXCOORD1;
        float2 uv2: TEXCOORD2;
        float2 uv3: TEXCOORD3;
        float2 uv4: TEXCOORD4;
    };

    VaryingsDoF vert(AttributesDefault v) {
        VaryingsDoF o;
        o.vertex = float4(v.vertex.xy, 0.0, 1.0);
        o.uv = TransformTriangleVertexToUV(v.vertex.xy);
        #if UNITY_UV_STARTS_AT_TOP
            o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
        #endif

        o.uvNonStereo = o.uv;
        o.uvNonStereoInv = o.uv;
        o.uv  = TransformStereoScreenSpaceTex(o.uv, 1.0);
        o.depthUV = o.uv;
        
        #if UNITY_UV_STARTS_AT_TOP
        if (_MainTex_TexelSize.y < 0) {
            o.uv.y = 1.0 - o.uv.y;
            o.uvNonStereoInv.y = 1.0 - o.uvNonStereoInv.y;
        }
        #endif      
        return o;
    }
    
    float getCoc(VaryingsDoF i) {
    #if BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT
        float depthTex = DecodeFloatRGBA(SAMPLE_TEXTURE2D_LOD(_DepthTexture, sampler_DepthTexture, i.uvNonStereo, 0));
        float exclusionDepth = DecodeFloatRGBA(SAMPLE_TEXTURE2D_LOD(_DofExclusionTexture, sampler_DofExclusionTexture, i.uvNonStereo, 0));
        float depth  = BEAUTIFY_GET_DEPTH_01(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.depthUV));
        depth = min(depth, depthTex);
        if (exclusionDepth < depth) return 0;
        depth *= _ProjectionParams.z;
    #else
        float depth  = BEAUTIFY_GET_DEPTH_EYE(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.depthUV));
    #endif
        if (depth>_BokehData3.y) return 0;
        float xd     = abs(depth - _BokehData.x) - _BokehData2.x * (depth < _BokehData.x);
        return 0.5 * _BokehData.y * xd/depth;    // radius of CoC
    }
                
    float4 fragCoC (VaryingsDoF i) : SV_Target {
        float4 pixel  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
        pixel         = clamp(pixel, 0.0.xxxx, _BokehData3.xxxx);
        #if UNITY_COLORSPACE_GAMMA
        pixel.rgb     = SRGBToLinear(pixel.rgb);
        #endif
        float coc = getCoc(i) / COC_BASE;
        return float4(pixel.rgb, coc);
    }    
    
    float4 fragCoCDebug (VaryingsDoF i) : SV_Target {
        float4 pixel  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
        pixel         = clamp(pixel, 0.0.xxxx, _BokehData3.xxxx);
        float  CoC    = getCoc(i) / COC_BASE;
        pixel.a       = min(CoC, pixel.a);
        return pixel.aaaa;
    }

    float4 fragBlur (VaryingsDoF i): SV_Target {
        float4 sum     = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv );
        float  samples = ceil(sum.a * COC_BASE);
        float4 dir     = float4(_BokehData.zw * _MainTex_TexelSize.xy, 0, 0);
        #if UNITY_SINGLE_PASS_STEREO
        dir.x *= 2.0;
        #endif
               dir    *= max(1.0, samples / _BokehData2.y);
        float  jitter  = dot(float2(2.4084507, 3.2535211), i.uv * _MainTex_TexelSize.zw);
        float2 disp0   = dir.xy * (frac(jitter) + 0.5);
        float4 disp1   = float4(i.uvNonStereoInv + disp0, 0, 0);
        float4 disp2   = float4(i.uvNonStereoInv - disp0, 0, 0);
        float  w       = 1.0;

        const int sampleCount = (int)min(_BokehData2.y, samples);
        UNITY_UNROLL
        for (int k=1;k<16;k++) {
            if (k<sampleCount) {
                #if UNITY_SINGLE_PASS_STEREO
                    disp1.xy    = saturate(disp1.xy);
                    float4 pixel1       = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_MainTex, TransformStereoScreenSpaceTex(disp1.xy, 1.0), 0); // was tex2Dlod
                #else
                    float4 pixel1       = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_MainTex, disp1.xy, 0);
                #endif
                float  bt1         = saturate(pixel1.a * COC_BASE - k);
                       pixel1.rgb += _BokehData2.www * max(pixel1.rgb - _BokehData2.zzz, 0.0.xxx);
                       sum        += pixel1 * bt1;
                       w           += bt1;
                       disp1      += dir;
                #if UNITY_SINGLE_PASS_STEREO
                       disp2.xy    = saturate(disp2.xy);
                float4 pixel2      = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_MainTex, TransformStereoScreenSpaceTex(disp2.xy, 1.0), 0);
                #else
                float4 pixel2       = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_MainTex, disp2.xy, 0);
                #endif
                       float  bt2  = saturate(pixel2.a * COC_BASE - k);
                       pixel2.rgb += _BokehData2.www * max(pixel2.rgb - _BokehData2.zzz, 0.0.xxx);
                       sum        += pixel2 * bt2;
                       w          += bt2;
                       disp2      -= dir;
            }
        }
        return sum / w;
    }

    float4 fragBlurNoBokeh (VaryingsDoF i): SV_Target {
        float4 sum     = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv );
        float samples  = ceil(sum.a * COC_BASE);
        float4 dir     = float4(_BokehData.zw * _MainTex_TexelSize.xy, 0, 0);
        #if UNITY_SINGLE_PASS_STEREO
        dir.x *= 0.5;
        #endif
               dir    *= max(1.0, samples / _BokehData2.y);
        float  jitter  = dot(float2(2.4084507, 3.2535211), i.uv * _MainTex_TexelSize.zw);
        float2 disp0   = dir.xy * (frac(jitter) + 0.5);
        float4 disp1   = float4(i.uvNonStereoInv + disp0, 0, 0);
        float4 disp2   = float4(i.uvNonStereoInv - disp0, 0, 0);
        float  w       = 1.0;

        const int sampleCount = (int)min(_BokehData2.y, samples);
        UNITY_UNROLL
        for (int k=1;k<16;k++) {
            if (k<sampleCount) {
                #if UNITY_SINGLE_PASS_STEREO
                float4 pixel1      = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_MainTex, TransformStereoScreenSpaceTex(disp1.xy, 1.0), 0);
                #else
                float4 pixel1      = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_MainTex, disp1.xy, 0);
                #endif
                float  bt1         = saturate(pixel1.a * COC_BASE - k);                
                       sum        += bt1 * pixel1;
                       w           += bt1;
                       disp1      += dir;
                #if UNITY_SINGLE_PASS_STEREO
                float4 pixel2      = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_MainTex, TransformStereoScreenSpaceTex(disp2.xy, 1.0), 0);
                #else
                float4 pixel2      = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_MainTex, disp2.xy, 0);
                #endif
                float  bt2  = saturate(pixel2.a * COC_BASE - k);                
                       sum        += bt2 * pixel2;
                       w          += bt2;
                       disp2      -= dir;
            }
        }
        return sum / w;
    }



    VaryingsDoFCross vertBlurH(AttributesDefault v) {
        VaryingsDoFCross o;
        o.vertex = float4(v.vertex.xy, 0.0, 1.0);
        o.uv  = TransformTriangleVertexToUV(v.vertex.xy);
        #if UNITY_UV_STARTS_AT_TOP
        if (_MainTex_TexelSize.y < 0) {
            o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
        }
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


    VaryingsDoFCross vertBlurV(AttributesDefault v) {
        VaryingsDoFCross o;
        o.vertex = float4(v.vertex.xy, 0.0, 1.0);
        o.uv  = TransformTriangleVertexToUV(v.vertex.xy);
        #if UNITY_UV_STARTS_AT_TOP
        if (_MainTex_TexelSize.y < 0) {
            o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
        }
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


   float4 fragBlurCoC (VaryingsDoFCross i): SV_Target {
        float depth   = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv));
        float depth1  = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv1));
        float depth2  = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv2));
        float depth3  = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv3));
        float depth4  = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv4));

        const float f = 10;
        float w1      = saturate((depth - depth1)/f) * 0.3162162162; 
        float w2      = saturate((depth - depth2)/f) * 0.3162162162; 
        float w3      = saturate((depth - depth3)/f) * 0.0702702703; 
        float w4      = saturate((depth - depth4)/f) * 0.0702702703; 

        float coc1    = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv1).a;
        float coc2    = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv2).a;
        float coc3    = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv3).a;
        float coc4    = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv4).a;

        float w0      = 0.2270270270;

        half4 pixel = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

        float coc     = (pixel.a * w0 + coc1 * w1 + coc2 * w2 + coc3 * w3 + coc4 * w4) / (w0 + w1 + w2 + w3 + w4);
        pixel.a = coc;
        return pixel;
    }   


#endif // BEAUTIFY_PPSDOF_FX


