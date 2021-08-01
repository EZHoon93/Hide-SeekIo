#ifndef BEAUTIFY_DISTORTION
#define BEAUTIFY_DISTORTION

#if BEAUTIFY_CHROMATIC_ABERRATION

float2 _ChromaticAberrationData;
#define CHROMATIC_ABERRATION_INTENSITY _ChromaticAberrationData.x
#define CHROMATIC_ABERRATION_SMOOTHING _ChromaticAberrationData.y
#define CHROMATIC_ABERRATION_MAX_SAMPLES 32

float4 GetDistortedColor(float2 uv) {

    float2 coords  = 2.0 * uv - 1.0;
    float  dst     = dot(coords, coords);
    float2 delta   = coords * (dst * CHROMATIC_ABERRATION_INTENSITY);
    int samples    = clamp( (int)(dst * CHROMATIC_ABERRATION_SMOOTHING) + 1, 3, CHROMATIC_ABERRATION_MAX_SAMPLES);
    float4 abColor = float4(0,0,0,1.0);
    float3 weight  = float3(0,0,0);
    UNITY_UNROLL
    for (int k=0;k<CHROMATIC_ABERRATION_MAX_SAMPLES;k++) {
        if (k<samples) {
            float h = (k+0.5) / samples;
            float3 spec = saturate( abs( fmod(h * 6.0.xxx + float3(0.0,4.0,2.0), 6.0) - 3.0) - 1.0 ); // hue to rgb
		    float3 rgb = SAMPLE_RAW_DEPTH_TEXTURE_LOD(_MainTex, float4(uv - delta * h, 0, 0)).xyz;
		    abColor.xyz += rgb * spec;
		    weight += spec;
        }
    }
    abColor.xyz /= weight + 0.0001;

    return abColor;
}


float4 GetDistortedColorFast(float2 uv) {

    float2 coords  = 2.0 * uv - 1.0;
    float  dst     = dot(coords, coords);
    float2 delta   = coords * (dst * CHROMATIC_ABERRATION_INTENSITY);
    float r = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uv).r;
    float g = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uv - delta).g;
    float b = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uv - delta * 2.0).b;
    return float4(r, g, b, 1.0);
}

#endif // BEAUTIFY_CHROMATIC_ABERRATION
#endif // BEAUTIFY_DISTORTION