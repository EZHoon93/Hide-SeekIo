/// <summary>
/// Copyright 2016-2019 Ramiro Oliva (Kronnect) - All rights reserved
/// </summary>

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using BoolParameter = UnityEngine.Rendering.PostProcessing.BoolParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;
using ColorParameter = UnityEngine.Rendering.PostProcessing.ColorParameter;
using TextureParameter = UnityEngine.Rendering.PostProcessing.TextureParameter;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;
using Vector2Parameter = UnityEngine.Rendering.PostProcessing.Vector2Parameter;

namespace BeautifyForPPS {

    public enum BeautifyTonemapOperator {
        Linear = 0,
        ACES = 1
    }

    [Serializable]
    public sealed class BeautifyTonemapOperatorParameter : ParameterOverride<BeautifyTonemapOperator> { }

    public enum BeautifyDoFFocusMode {
        FixedDistance,
        AutoFocus,
        FollowTarget
    }

    [Serializable]
    public sealed class BeautifyDoFFocusModeParameter : ParameterOverride<BeautifyDoFFocusMode> { }


    [Serializable]
    public sealed class BeautifyDoFFilterModeParameter : ParameterOverride<FilterMode> { }


    [Serializable]
    public sealed class BeautifyLayerMaskParameter : ParameterOverride<LayerMask> { }



    [Serializable]
    [PostProcess(typeof(BeautifyRenderer), PostProcessEvent.BeforeStack, "Kronnect/Beautify", true)]
    public sealed class Beautify : PostProcessEffectSettings {

        #region Compare settings

        public BoolParameter compareMode = new BoolParameter { value = false };

        [Range(-Mathf.PI, Mathf.PI)]
        public FloatParameter compareLineAngle = new FloatParameter { value = 1.4f };

        [Range(0.0001f, 0.05f)]
        public FloatParameter compareLineWidth = new FloatParameter { value = 0.002f };

        #endregion


        #region Sharpen settings

        [Range(0f, 25f)]
        public FloatParameter sharpenIntensity = new FloatParameter { value = 4f };

        [Range(0f, 0.05f)]
        public FloatParameter sharpenDepthThreshold = new FloatParameter { value = 0.035f };

        [Range(0, 1f)]
        public FloatParameter sharpenMinDepth = new FloatParameter { value = 0f };

        [Range(0, 1.1f)]
        public FloatParameter sharpenMaxDepth = new FloatParameter { value = 0.999f };

        [Range(0f, 0.2f)]
        public FloatParameter sharpenRelaxation = new FloatParameter { value = 0.08f };

        [Range(0, 1f)]
        public FloatParameter sharpenClamp = new FloatParameter { value = 0.45f };

        [Range(0, 1f)]
        public FloatParameter sharpenMotionSensibility = new FloatParameter { value = 0.5f };

        #endregion

        #region Color tweaks

        [Range(0f, 2f)]
        public FloatParameter daltonize = new FloatParameter { value = 0f };

        [Range(0f, 1f)]
        public FloatParameter sepia = new FloatParameter { value = 0f };

        [Range(-2f, 3f)]
        public FloatParameter saturate = new FloatParameter { value = 0f };

        [Range(0f, 2f)]
        public FloatParameter brightness = new FloatParameter { value = 1f };

        [Range(0.5f, 1.5f)]
        public FloatParameter contrast = new FloatParameter { value = 1f };

        [Range(0.5f, 1.5f)]
        public ColorParameter tintColor = new ColorParameter { value = new Color(1, 1, 1, 0) };

        public BeautifyTonemapOperatorParameter tonemap = new BeautifyTonemapOperatorParameter { value = BeautifyTonemapOperator.Linear };

        public FloatParameter tonemapExposurePre = new FloatParameter { value = 1f };

        public FloatParameter tonemapBrightnessPost = new FloatParameter { value = 1f };

        #endregion


        #region Lut

        public BoolParameter lut = new BoolParameter { value = false };
        public FloatParameter lutIntensity = new FloatParameter { value = 1f };
        public TextureParameter lutTexture = new TextureParameter { value = null };

        #endregion


        #region Bloom & Flares effects

        [Range(0, 10f)]
        public FloatParameter bloomIntensity = new FloatParameter { value = 0f };

        [Range(0f, 5f)]
        public FloatParameter bloomThreshold = new FloatParameter { value = 0.75f };

        public FloatParameter bloomMaxBrightness = new FloatParameter { value = 1000f };

        [Range(0, 1f)] public FloatParameter bloomDepthAtten = new FloatParameter { value = 0f };

        public BoolParameter bloomAntiflicker = new BoolParameter { value = false };

        public BoolParameter bloomUltra = new BoolParameter { value = false };
        public BoolParameter bloomDebug = new BoolParameter { value = false };

        public BoolParameter bloomCustomize = new BoolParameter { value = false };
        [Range(0, 1f)] public FloatParameter bloomWeight0 = new FloatParameter { value = 0.5f };
        [Range(0, 1f)] public FloatParameter bloomWeight1 = new FloatParameter { value = 0.5f };
        [Range(0, 1f)] public FloatParameter bloomWeight2 = new FloatParameter { value = 0.5f };
        [Range(0, 1f)] public FloatParameter bloomWeight3 = new FloatParameter { value = 0.5f };
        [Range(0, 1f)] public FloatParameter bloomWeight4 = new FloatParameter { value = 0.5f };
        [Range(0, 1f)] public FloatParameter bloomWeight5 = new FloatParameter { value = 0.5f };

        [Range(0, 3f)] public FloatParameter bloomBoost0 = new FloatParameter { value = 0f };
        [Range(0, 3f)] public FloatParameter bloomBoost1 = new FloatParameter { value = 0f };
        [Range(0, 3f)] public FloatParameter bloomBoost2 = new FloatParameter { value = 0f };
        [Range(0, 3f)] public FloatParameter bloomBoost3 = new FloatParameter { value = 0f };
        [Range(0, 3f)] public FloatParameter bloomBoost4 = new FloatParameter { value = 0f };
        [Range(0, 3f)] public FloatParameter bloomBoost5 = new FloatParameter { value = 0f };

        [Range(0f, 10f)]
        public FloatParameter anamorphicFlaresIntensity = new FloatParameter { value = 0f };
        [Range(0f, 5f)]
        public FloatParameter anamorphicFlaresThreshold = new FloatParameter { value = 0.75f };
        public BoolParameter anamorphicFlaresVertical = new BoolParameter { value = false };
        [Range(0.1f, 2f)]
        public FloatParameter anamorphicFlaresSpread = new FloatParameter { value = 1f };

        [Range(0, 1f)] public FloatParameter anamorphicFlaresDepthAtten = new FloatParameter { value = 0f };

        public BoolParameter anamorphicFlaresAntiflicker = new BoolParameter { value = false };
        public BoolParameter anamorphicFlaresUltra = new BoolParameter { value = false };
        public ColorParameter anamorphicFlaresTint = new ColorParameter { value = new Color(0.5f, 0.5f, 1f, 0f) };

        [Range(0f, 1f)]
        public FloatParameter sunFlaresIntensity = new FloatParameter { value = 0.0f };
        public ColorParameter sunFlaresTint = new ColorParameter { value = new Color(1, 1, 1) };
        [Range(0f, 1f)]
        public FloatParameter sunFlaresSolarWindSpeed = new FloatParameter { value = 0.01f };
        public BoolParameter sunFlaresRotationDeadZone = new BoolParameter { value = false };
        [Range(1, 5)] public IntParameter sunFlaresDownsampling = new IntParameter { value = 1 };


        public BeautifyLayerMaskParameter sunFlaresLayerMask = new BeautifyLayerMaskParameter { value = -1 };
        [Range(0f, 1f)]
        public FloatParameter sunFlaresSunIntensity = new FloatParameter { value = 0.1f };
        [Range(0f, 1f)]
        public FloatParameter sunFlaresSunDiskSize = new FloatParameter { value = 0.05f };
        [Range(0f, 10f)]
        public FloatParameter sunFlaresSunRayDiffractionIntensity = new FloatParameter { value = 3.5f };
        [Range(0f, 1f)]
        public FloatParameter sunFlaresSunRayDiffractionThreshold = new FloatParameter { value = 0.13f };

        [Header("Corona Rays Group 1")]
        [Range(0f, 0.2f)]
        public FloatParameter sunFlaresCoronaRays1Length = new FloatParameter { value = 0.02f };
        [Range(2f, 30f)]
        public FloatParameter sunFlaresCoronaRays1Streaks = new FloatParameter { value = 12 };
        [Range(0f, 0.1f)]
        public FloatParameter sunFlaresCoronaRays1Spread = new FloatParameter { value = 0.001f };
        [Range(0f, 2f * Mathf.PI)]
        public FloatParameter sunFlaresCoronaRays1AngleOffset = new FloatParameter { value = 0f };

        [Header("Corona Rays Group 2")]
        [Range(0f, 0.2f)]
        public FloatParameter sunFlaresCoronaRays2Length = new FloatParameter { value = 0.05f };
        [Range(2f, 30f)]
        public FloatParameter sunFlaresCoronaRays2Streaks = new FloatParameter { value = 12f };
        [Range(0f, 0.1f)]
        public FloatParameter sunFlaresCoronaRays2Spread = new FloatParameter { value = 0.1f };
        [Range(0f, 2f * Mathf.PI)]
        public FloatParameter sunFlaresCoronaRays2AngleOffset = new FloatParameter { value = 0f };

        [Header("Ghost 1")]
        [Range(0f, 1f)]
        public FloatParameter sunFlaresGhosts1Size = new FloatParameter { value = 0.03f };
        [Range(-3f, 3f)]
        public FloatParameter sunFlaresGhosts1Offset = new FloatParameter { value = 1.04f };
        [Range(0f, 1f)]
        public FloatParameter sunFlaresGhosts1Brightness = new FloatParameter { value = 0.037f };

        [Header("Ghost 2")]
        [Range(0f, 1f)]
        public FloatParameter sunFlaresGhosts2Size = new FloatParameter { value = 0.1f };
        [Range(-3f, 3f)]
        public FloatParameter sunFlaresGhosts2Offset = new FloatParameter { value = 0.71f };
        [Range(0f, 1f)]
        public FloatParameter sunFlaresGhosts2Brightness = new FloatParameter { value = 0.03f };

        [Header("Ghost 3")]
        [Range(0f, 1f)]
        public FloatParameter sunFlaresGhosts3Size = new FloatParameter { value = 0.24f };
        [Range(3f, -3f)]
        public FloatParameter sunFlaresGhosts3Brightness = new FloatParameter { value = 0.025f };
        [Range(0f, 1f)]
        public FloatParameter sunFlaresGhosts3Offset = new FloatParameter { value = 0.31f };

        [Header("Ghost 4")]
        [Range(0f, 1f)]
        public FloatParameter sunFlaresGhosts4Size = new FloatParameter { value = 0.016f };
        [Range(-3f, 3f)]
        public FloatParameter sunFlaresGhosts4Offset = new FloatParameter { value = 0f };
        [Range(0f, 1f)]
        public FloatParameter sunFlaresGhosts4Brightness = new FloatParameter { value = 0.017f };

        [Header("Halo")]
        [Range(0f, 1f)]
        public FloatParameter sunFlaresHaloOffset = new FloatParameter { value = 0.22f };
        [Range(0f, 50f)]
        public FloatParameter sunFlaresHaloAmplitude = new FloatParameter { value = 15.1415f };
        [Range(0f, 1f)]
        public FloatParameter sunFlaresHaloIntensity = new FloatParameter { value = 0.01f };

        #endregion


        #region Lens Dirt
        public FloatParameter lensDirtIntensity = new FloatParameter { value = 0f };
        [Range(0f, 1f)]
        public FloatParameter lensDirtThreshold = new FloatParameter { value = 0.5f };
        [Range(0f, 1f)]
        public TextureParameter lensDirtTexture = new TextureParameter { value = null };
        [Range(3, 5)]
        public IntParameter lensDirtSpread = new IntParameter { value = 3 };
        #endregion


        #region Eye Adaptation

        public BoolParameter eyeAdaptation = new BoolParameter { value = false };
        [Range(0f, 1f)]
        public FloatParameter eyeAdaptationMinExposure = new FloatParameter { value = 0.2f };
        [Range(1f, 100f)]
        public FloatParameter eyeAdaptationMaxExposure = new FloatParameter { value = 5f };
        [Range(0f, 1f)]
        public FloatParameter eyeAdaptationSpeedToLight = new FloatParameter { value = 0.4f };
        [Range(0f, 1f)]
        public FloatParameter eyeAdaptationSpeedToDark = new FloatParameter { value = 0.2f };

        #endregion


        #region Purkinje effect

        public BoolParameter purkinje = new BoolParameter { value = false };
        [Range(0f, 5f)]
        public FloatParameter purkinjeAmount = new FloatParameter { value = 1f };
        [Range(0f, 1f)]
        public FloatParameter purkinjeLuminanceThreshold = new FloatParameter { value = 0.15f };

        #endregion


        #region Vignetting

        [Range(0, 1f)]
        public FloatParameter vignettingOuterRing = new FloatParameter { value = 0f };
        [Range(0, 1f)]
        public FloatParameter vignettingInnerRing = new FloatParameter { value = 0 };
        [Range(0, 1)]
        public FloatParameter vignettingFade = new FloatParameter { value = 0 };
        public BoolParameter vignettingCircularShape = new BoolParameter { value = false };
        public FloatParameter vignettingAspectRatio = new FloatParameter { value = 1f };
        [Range(0, 1f)]
        public FloatParameter vignettingBlink = new FloatParameter { value = 0f };
        public ColorParameter vignettingColor = new ColorParameter { value = new Color(0f, 0f, 0f, 1f) };
        public TextureParameter vignettingMask = new TextureParameter { value = null };

        #endregion

        #region Depth of Field

        public BoolParameter depthOfField = new BoolParameter { value = false };
        public BoolParameter depthOfFieldDebug = new BoolParameter { value = false };
        public BeautifyDoFFocusModeParameter depthOfFieldFocusMode = new BeautifyDoFFocusModeParameter { value = BeautifyDoFFocusMode.FixedDistance };
        public FloatParameter depthOfFieldAutofocusMinDistance = new FloatParameter { value = 0 };
        public FloatParameter depthOfFieldAutofocusMaxDistance = new FloatParameter { value = 10000 };
        public Vector2Parameter depthofFieldAutofocusViewportPoint = new Vector2Parameter { value = new Vector2(0.5f, 0.5f) };
        public BeautifyLayerMaskParameter depthOfFieldAutofocusLayerMask = new BeautifyLayerMaskParameter { value = -1 };
        public BeautifyLayerMaskParameter depthOfFieldExclusionLayerMask = new BeautifyLayerMaskParameter { value = 0 };
        [Range(1, 4)]
        public FloatParameter depthOfFieldExclusionLayerMaskDownsampling = new FloatParameter { value = 1f };
        public BoolParameter depthOfFieldTransparencySupport = new BoolParameter { value = false };
        public BeautifyLayerMaskParameter depthOfFieldTransparencyLayerMask = new BeautifyLayerMaskParameter { value = -1 };
        [Range(1, 4)]
        public FloatParameter depthOfFieldTransparencySupportDownsampling = new FloatParameter { value = 1f };
        [Range(0.9f, 1f)]
        public FloatParameter depthOfFieldExclusionBias = new FloatParameter { value = 0.99f };
        [Range(1f, 100f)]
        public FloatParameter depthOfFieldDistance = new FloatParameter { value = 1f };
        [Range(0.001f, 5f)]
        public FloatParameter depthOfFieldFocusSpeed = new FloatParameter { value = 1f };
        [Range(1, 5)]
        public IntParameter depthOfFieldDownsampling = new IntParameter { value = 2 };
        [Range(2, 16)]
        public IntParameter depthOfFieldMaxSamples = new IntParameter { value = 4 };
        [Range(0.005f, 0.5f)]
        public FloatParameter depthOfFieldFocalLength = new FloatParameter { value = 0.050f };
        public FloatParameter depthOfFieldAperture = new FloatParameter { value = 2.8f };
        public BoolParameter depthOfFieldForegroundBlur = new BoolParameter { value = true };
        public BoolParameter depthOfFieldForegroundBlurHQ = new BoolParameter { value = false };
        public FloatParameter depthOfFieldForegroundDistance = new FloatParameter { value = 0.25f };
        public BoolParameter depthOfFieldBokeh = new BoolParameter { value = true };
        [Range(0.5f, 3f)]
        public FloatParameter depthOfFieldBokehThreshold = new FloatParameter { value = 1f };
        [Range(0f, 8f)]
        public FloatParameter depthOfFieldBokehIntensity = new FloatParameter { value = 2f };
        public FloatParameter depthOfFieldMaxBrightness = new FloatParameter { value = 1000f };
        [Range(0, 1f)]
        public FloatParameter depthOfFieldMaxDistance = new FloatParameter { value = 1f };
        [SerializeField]
        public BeautifyDoFFilterModeParameter depthOfFieldFilterMode = new BeautifyDoFFilterModeParameter { value = FilterMode.Bilinear };

        #endregion

        #region Outline
        public BoolParameter outline = new BoolParameter { value = false };
        public ColorParameter outlineColor = new ColorParameter { value = new Color(0, 0, 0, 0.8f) };
        #endregion

    }

    public sealed class BeautifyRenderer : PostProcessEffectRenderer<Beautify> {

        struct BloomMipData {
            public int rtDown, rtUp, width, height;
            public int rtDownOriginal, rtUpOriginal;
        }

        const string SKW_BLOOM = "BEAUTIFY_BLOOM";
        const string SKW_BLOOM_USE_DEPTH = "BEAUTIFY_BLOOM_USE_DEPTH";
        const string SKW_DIRT = "BEAUTIFY_DIRT";
        const string SKW_TONEMAP_ACES = "BEAUTIFY_TONEMAP_ACES";
        const string SKW_VIGNETTING = "BEAUTIFY_VIGNETTING";
        const string SKW_VIGNETTING_MASK = "BEAUTIFY_VIGNETTING_MASK";
        const string SKW_PURKINJE = "BEAUTIFY_PURKINJE";
        const string SKW_EYE_ADAPTATION = "BEAUTIFY_EYE_ADAPTATION";
        const string SKW_LUT = "BEAUTIFY_LUT";
        const string SKW_DEPTH_OF_FIELD = "BEAUTIFY_DEPTH_OF_FIELD";
        //const string SKW_DEPTH_OF_FIELD_TRANSPARENT = "BEAUTIFY_DEPTH_OF_FIELD_TRANSPARENT";
        const string SKW_ANAMORPHIC_FLARES_USE_DEPTH = "BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH";
        const string SKW_OUTLINE = "BEAUTIFY_OUTLINE";

        const int PYRAMID_COUNT_BLOOM = 5;
        const int PYRAMID_COUNT_EA = 9;
        const int SHADER_RAW_COPY_PASS = 12;


        Vector3 camPrevForward, camPrevPos;
        float currSens;
        BloomMipData[] rt, rtAF;
        int rtSF;
        int tempBlurRT;
        int tempBloomCustomComposeRT;
        int tempBlurOneDirRT;
        int tempCompareRT;
        float sunFlareCurrentIntensity;
        Vector4 sunLastScrPos;
        float sunLastRot;
        float sunFlareTime;
        int[] rtEA;
        RenderTexture rtEAacum, rtEAHist;
        Texture2D dirtTexture;

        BeautifySettings sceneSettings;

        int rtDoF;
        int tempBlurDoFAlphaRT, tempBlurDoFTemp1RT, tempBlurDoFTemp2RT;
        float dofPrevDistance, dofLastAutofocusDistance;
        Vector4 dofLastBokehData;
        Camera sceneCamera;
        RenderTextureFormat rtFormat;

        public override DepthTextureMode GetCameraFlags() {
            return DepthTextureMode.Depth;
        }

        public override void Init() {
            tempCompareRT = Shader.PropertyToID("_BeautifyCompare");
            tempBlurRT = Shader.PropertyToID("_BeautifyTempBlur0");
            tempBloomCustomComposeRT = Shader.PropertyToID("_BeautifyTempCustomBloomCompose");
            tempBlurOneDirRT = Shader.PropertyToID("_BeautifyTempBlurOneDir0");
            rtSF = Shader.PropertyToID("_BeautifyTempSF0");
            rtDoF = Shader.PropertyToID("_BeautifyDoF");
            tempBlurDoFAlphaRT = Shader.PropertyToID("_BeautifyTempBlurAlphaDoF");
            tempBlurDoFTemp1RT = Shader.PropertyToID("_BeautifyTempBlurPass1DoF");
            tempBlurDoFTemp2RT = Shader.PropertyToID("_BeautifyTempBlurPass2DoF");

            // Bloom buffers
            if (rt == null || rt.Length != PYRAMID_COUNT_BLOOM + 1) {
                rt = new BloomMipData[PYRAMID_COUNT_BLOOM + 1];
                for (int k = 0; k < rt.Length; k++) {
                    rt[k].rtDown = rt[k].rtDownOriginal = Shader.PropertyToID("_BeautifyBloomDownMip" + k);
                    rt[k].rtUp = rt[k].rtUpOriginal = Shader.PropertyToID("_BeautifyBloomUpMip" + k);
                }
            }
            // Anamorphic flare buffers
            if (rtAF == null || rtAF.Length != PYRAMID_COUNT_BLOOM + 1) {
                rtAF = new BloomMipData[PYRAMID_COUNT_BLOOM + 1];
                for (int k = 0; k < rtAF.Length; k++) {
                    rtAF[k].rtDown = Shader.PropertyToID("_BeautifyAFDownMip" + k);
                    rtAF[k].rtUp = Shader.PropertyToID("_BeautifyAFUpMip" + k);
                }
            }
            // Eye adaptation buffers
            if (rtEA == null || rtEA.Length != PYRAMID_COUNT_EA) {
                rtEA = new int[PYRAMID_COUNT_EA];
                for (int k = 0; k < rtEA.Length; k++) {
                    rtEA[k] = Shader.PropertyToID("_BeautifyEAMip" + k);
                }
            }

        }


        public override void Render(PostProcessRenderContext context) {

            rtFormat = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf) ? RenderTextureFormat.ARGBHalf : context.sourceFormat;

            PropertySheet sheet = context.propertySheets.Get(Shader.Find("Hidden/BeautifyPPSCore"));
            sheet.ClearKeywords();
            // Restore temorary rt ids
            for (int k = 0; k < rt.Length; k++) {
                rt[k].rtDown = rt[k].rtDownOriginal;
                rt[k].rtUp = rt[k].rtUpOriginal;
            }

            // Compute motion sensibility 
            Vector3 camCurForward = context.camera.transform.forward;
            Vector3 camCurPos = context.camera.transform.position;
            float angleDiff = Vector3.Angle(camPrevForward, camCurForward) * settings.sharpenMotionSensibility;
            float posDiff = (camCurPos - camPrevPos).sqrMagnitude * 10f * settings.sharpenMotionSensibility;

            float diff = angleDiff + posDiff;
            if (diff > 0.1f) {
                camPrevForward = camCurForward;
                camPrevPos = camCurPos;
                if (diff > settings.sharpenMotionSensibility)
                    diff = settings.sharpenMotionSensibility;
                currSens += diff;
                float min = settings.sharpenIntensity * settings.sharpenMotionSensibility * 0.75f;
                float max = settings.sharpenIntensity * (1f + settings.sharpenMotionSensibility) * 0.5f;
                currSens = Mathf.Clamp(currSens, min, max);
            } else {
                currSens *= 0.75f;
            }
            float tempSharpen = Mathf.Clamp(settings.sharpenIntensity - currSens, 0, settings.sharpenIntensity);

            // Sharpen
            sheet.properties.SetVector("_Sharpen", new Vector4(tempSharpen, settings.sharpenDepthThreshold, settings.sharpenClamp, settings.sharpenRelaxation));
            bool isOrtho = (context.camera != null && context.camera.orthographic);
            sheet.properties.SetVector("_Params", new Vector4(settings.sepia, settings.daltonize, (settings.sharpenMaxDepth + settings.sharpenMinDepth) * 0.5f, Mathf.Abs(settings.sharpenMaxDepth - settings.sharpenMinDepth) * 0.5f + (isOrtho ? 1000.0f : 0f)));
            // Color tweaks
            sheet.properties.SetVector("_ColorBoost", new Vector3(settings.brightness, settings.contrast, settings.saturate));
            sheet.properties.SetColor("_TintColor", settings.tintColor);
            if (settings.tonemap == BeautifyTonemapOperator.ACES) {
                sheet.EnableKeyword(SKW_TONEMAP_ACES);
            }
            sheet.properties.SetColor("_FXColor", new Color(settings.tonemapExposurePre, settings.tonemapBrightnessPost, 0, settings.lutIntensity));

            // LUT
            if (settings.lut && settings.lutTexture.value != null) {
                sheet.EnableKeyword(SKW_LUT);
                sheet.properties.SetTexture("_LUTTex", settings.lutTexture);
            }

            // Flares (Bloom / Anamorphic flares / Lens dirt / Sun Flares)
            DoFlaresEffects(context, sheet);


            if (settings.bloomDebug && (settings.bloomIntensity > 0 || settings.anamorphicFlaresIntensity > 0 || settings.sunFlaresIntensity > 0)) {
                context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 3);
                return;
            }

            // Eye adaptation
            DoEyeAdaptation(context, sheet);

            // Vignetting
            float outerRing = 1f - settings.vignettingOuterRing;
            float innerRing = 1f - settings.vignettingInnerRing;
            bool vignettingEnabled = outerRing < 1 || innerRing < 1f || settings.vignettingFade > 0 || settings.vignettingBlink > 0;
            if (vignettingEnabled) {
                Color vignettingColorAdjusted = settings.vignettingColor;
                float vb = 1f - settings.vignettingBlink * 2f;
                if (vb < 0) vb = 0;
                vignettingColorAdjusted.r *= vb;
                vignettingColorAdjusted.g *= vb;
                vignettingColorAdjusted.b *= vb;
                vignettingColorAdjusted.a = outerRing;
                sheet.properties.SetColor("_Vignetting", vignettingColorAdjusted);
                if (context.camera != null) {
                    sheet.properties.SetFloat("_VignettingAspectRatio", (settings.vignettingCircularShape && settings.vignettingBlink <= 0) ? 1.0f / context.camera.aspect : settings.vignettingAspectRatio + 1.001f / (1.001f - settings.vignettingBlink) - 1f);
                }

                if (settings.vignettingMask.value != null) {
                    sheet.properties.SetTexture("_VignettingMask", settings.vignettingMask.value);
                    sheet.EnableKeyword(SKW_VIGNETTING_MASK);
                } else {
                    sheet.EnableKeyword(SKW_VIGNETTING);
                }
            }
            // Purkinje and vignetting data
            if (settings.purkinje || vignettingEnabled) {
                float vd = settings.vignettingFade + settings.vignettingBlink * 0.5f;
                if (settings.vignettingBlink > 0.99f) vd = 1f;
                if (innerRing >= outerRing) {
                    innerRing = outerRing - 0.0001f;
                }
                Vector4 purkinjeData = new Vector4(settings.purkinjeAmount, settings.purkinjeLuminanceThreshold, vd, innerRing);
                sheet.properties.SetVector("_Purkinje", purkinjeData);
                if (settings.purkinje) {
                    sheet.EnableKeyword(SKW_PURKINJE);
                }
            }

            // DoF
            DoDoF(context, sheet);
            if (settings.depthOfField && settings.depthOfFieldDebug) {
                context.command.BlitFullscreenTriangle(rtDoF, context.destination, sheet, 22);
                return;
            }

            // Outline
            if (settings.outline) {
                sheet.EnableKeyword(SKW_OUTLINE);
                sheet.properties.SetColor("_Outline", settings.outlineColor);
            }

            // compare view
            if (settings.compareMode) {
                context.GetScreenSpaceTemporaryRT(context.command, tempCompareRT, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear);
                RenderTargetIdentifier rtiCompare = new RenderTargetIdentifier(tempCompareRT);
                context.command.BlitFullscreenTriangle(context.source, rtiCompare, sheet, 1);
                context.command.SetGlobalTexture("_CompareTex", rtiCompare);
                sheet.properties.SetVector("_CompareParams", new Vector4(Mathf.Cos(settings.compareLineAngle), Mathf.Sin(settings.compareLineAngle), -Mathf.Cos(settings.compareLineAngle), settings.compareLineWidth));
                context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
            } else {
                context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 1);
            }
        }

        void DoFlaresEffects(PostProcessRenderContext context, PropertySheet sheet) {

            MaterialPropertyBlock bMat = sheet.properties;
            bool sunFlareEnabled = false;
            if (settings.sunFlaresIntensity > 0) {
                CheckSun(context);
                sunFlareEnabled = sceneSettings != null && sceneSettings.sun != null;
            }

            if (settings.lensDirtIntensity > 0 || settings.bloomIntensity > 0 || settings.anamorphicFlaresIntensity > 0 || sunFlareEnabled) {


                float bloomWeightsSum = 0.00001f + settings.bloomWeight0 + settings.bloomWeight1 + settings.bloomWeight2 + settings.bloomWeight3 + settings.bloomWeight4 + settings.bloomWeight5;
                sheet.properties.SetVector("_BloomWeights2", new Vector4(settings.bloomWeight4 / bloomWeightsSum + settings.bloomBoost4, settings.bloomWeight5 / bloomWeightsSum + settings.bloomBoost5, settings.bloomMaxBrightness, bloomWeightsSum));
                float aspectRatio = (float)context.height / context.width;
                int rtBloom = -1;
                UpdateMaterialBloomIntensityAndThreshold(bMat);

                if (settings.bloomIntensity > 0 || (settings.lensDirtIntensity > 0 && settings.anamorphicFlaresIntensity <= 0)) {

                    sheet.properties.SetVector("_BloomWeights", new Vector4(settings.bloomWeight0 / bloomWeightsSum + settings.bloomBoost0, settings.bloomWeight1 / bloomWeightsSum + settings.bloomBoost1, settings.bloomWeight2 / bloomWeightsSum + settings.bloomBoost2, settings.bloomWeight3 / bloomWeightsSum + settings.bloomBoost3));

                    if (settings.bloomDepthAtten > 0) {
                        sheet.EnableKeyword(SKW_BLOOM_USE_DEPTH);
                        sheet.properties.SetFloat("_BloomDepthThreshold", settings.bloomDepthAtten);
                    }

                    int size = settings.bloomUltra ? (int)(context.camera.pixelHeight / 4) * 4 : 512;
                    for (int k = 0; k <= PYRAMID_COUNT_BLOOM; k++) {
                        rt[k].width = size;
                        rt[k].height = Mathf.Max(1, (int)(size * aspectRatio));
                        context.GetScreenSpaceTemporaryRT(context.command, rt[k].rtDown, 0, rtFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, rt[k].width, rt[k].height);
                        context.GetScreenSpaceTemporaryRT(context.command, rt[k].rtUp, 0, rtFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, rt[k].width, rt[k].height);
                        size /= 2;
                    }

                    rtBloom = rt[0].rtDown;
                    if (settings.bloomAntiflicker) {
                        context.command.BlitFullscreenTriangle(context.source, rtBloom, sheet, 9);
                    } else {
                        context.command.BlitFullscreenTriangle(context.source, rtBloom, sheet, 2);
                    }
                    BlurThis(context, sheet, rtBloom, rt[0].width, rt[0].height);

                    // Blitting down...
                    for (int k = 0; k < PYRAMID_COUNT_BLOOM; k++) {
                        context.command.BlitFullscreenTriangle(rt[k].rtDown, rt[k + 1].rtDown, sheet, 7);
                        BlurThis(context, sheet, rt[k + 1].rtDown, rt[k + 1].width, rt[k + 1].height);
                    }

                    if (settings.bloomIntensity > 0) {
                        // Blitting up...
                        rtBloom = rt[PYRAMID_COUNT_BLOOM].rtDown;
                        for (int k = PYRAMID_COUNT_BLOOM; k > 0; k--) {
                            context.command.SetGlobalTexture("_BloomTex", rt[k - 1].rtDown);
                            context.command.BlitFullscreenTriangle(rtBloom, rt[k - 1].rtUp, sheet, 8);
                            rtBloom = rt[k - 1].rtUp;
                        }
                        if (settings.bloomCustomize) {
                            context.command.SetGlobalTexture("_BloomTex4", new RenderTargetIdentifier(rt[4].rtUp));
                            context.command.SetGlobalTexture("_BloomTex3", new RenderTargetIdentifier(rt[3].rtUp));
                            context.command.SetGlobalTexture("_BloomTex2", new RenderTargetIdentifier(rt[2].rtUp));
                            context.command.SetGlobalTexture("_BloomTex1", new RenderTargetIdentifier(rt[1].rtUp));
                            context.command.SetGlobalTexture("_BloomTex", new RenderTargetIdentifier(rt[0].rtUp));
                            context.GetScreenSpaceTemporaryRT(context.command, tempBloomCustomComposeRT, 0, rtFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, rt[0].width, rt[0].height);
                            rtBloom = tempBloomCustomComposeRT;
                            context.command.BlitFullscreenTriangle(rt[PYRAMID_COUNT_BLOOM].rtUp, rtBloom, sheet, 6);
                        }
                    }
                }

                // anamorphic flares
                if (settings.anamorphicFlaresIntensity > 0) {

                    if (settings.anamorphicFlaresDepthAtten > 0) {
                        sheet.EnableKeyword(SKW_ANAMORPHIC_FLARES_USE_DEPTH);
                        sheet.properties.SetFloat("_AFDepthThreshold", settings.anamorphicFlaresDepthAtten);
                    }

                    int sizeAF = settings.anamorphicFlaresUltra ? (int)(context.camera.pixelHeight / 4) * 4 : 512;

                    for (int origSize = sizeAF, k = 0; k <= PYRAMID_COUNT_BLOOM; k++) {
                        if (settings.anamorphicFlaresVertical) {
                            rtAF[k].width = origSize;
                            rtAF[k].height = Mathf.Max(1, (int)(sizeAF * aspectRatio / settings.anamorphicFlaresSpread));

                        } else {
                            rtAF[k].width = Mathf.Max(1, (int)(sizeAF * aspectRatio / settings.anamorphicFlaresSpread));
                            rtAF[k].height = origSize;
                        }
                        context.GetScreenSpaceTemporaryRT(context.command, rtAF[k].rtDown, 0, rtFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, rtAF[k].width, rtAF[k].height);
                        context.GetScreenSpaceTemporaryRT(context.command, rtAF[k].rtUp, 0, rtFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, rtAF[k].width, rtAF[k].height);
                        sizeAF /= 2;
                    }

                    if (settings.anamorphicFlaresAntiflicker) {
                        context.command.BlitFullscreenTriangle(context.source, rtAF[0].rtDown, sheet, 18);
                    } else {
                        context.command.BlitFullscreenTriangle(context.source, rtAF[0].rtDown, sheet, 17);
                    }

                    BlurThisOneDirection(context, sheet, ref rtAF[0].rtDown, rtAF[0].width, rtAF[0].height, settings.anamorphicFlaresVertical);

                    for (int k = 0; k < PYRAMID_COUNT_BLOOM; k++) {
                        context.command.BlitFullscreenTriangle(rtAF[k].rtDown, rtAF[k + 1].rtDown, sheet, 7);
                        BlurThisOneDirection(context, sheet, ref rtAF[k + 1].rtDown, rtAF[k + 1].width, rtAF[k + 1].height, settings.anamorphicFlaresVertical);
                    }

                    int last = rtAF[PYRAMID_COUNT_BLOOM].rtDown;
                    for (int k = PYRAMID_COUNT_BLOOM; k > 0; k--) {
                        context.command.SetGlobalTexture("_BloomTex", rtAF[k].rtDown);
                        if (k == 1) {
                            context.command.BlitFullscreenTriangle(last, rtAF[k - 1].rtUp, sheet, 10); // applies intensity in last stage
                        } else {
                            context.command.BlitFullscreenTriangle(last, rtAF[k - 1].rtUp, sheet, 8);
                        }
                        last = rtAF[k - 1].rtUp;
                    }
                    if (settings.bloomIntensity > 0) {
                        if (settings.lensDirtIntensity > 0) {
                            BlendOneOne(context, sheet, rtAF[settings.lensDirtSpread].rtUp, ref rt[settings.lensDirtSpread].rtUp, ref rt[settings.lensDirtSpread].rtDown);
                            //context.command.BlitFullscreenTriangle(rtAF[settings.lensDirtSpread].rtUp, rt[settings.lensDirtSpread].rtUp, sheet, 11);
                        }
                        BlendOneOne(context, sheet, rtAF[0].rtUp, ref rtBloom, ref rt[0].rtDown);
                        //context.command.BlitFullscreenTriangle(rtAF[0].rtUp, rtBloom, sheet, 11);
                    } else {
                        rtBloom = rtAF[0].rtUp;
                    }
                    bMat.SetColor("_AFTint", settings.anamorphicFlaresTint);
                }

                if (sunFlareEnabled) {
                    bMat.SetVector("_SunData", new Vector4(settings.sunFlaresSunIntensity, settings.sunFlaresSunDiskSize, settings.sunFlaresSunRayDiffractionIntensity, settings.sunFlaresSunRayDiffractionThreshold));
                    bMat.SetVector("_SunCoronaRays1", new Vector4(settings.sunFlaresCoronaRays1Length, Mathf.Max(settings.sunFlaresCoronaRays1Streaks / 2f, 1), Mathf.Max(settings.sunFlaresCoronaRays1Spread, 0.0001f), settings.sunFlaresCoronaRays1AngleOffset));
                    bMat.SetVector("_SunCoronaRays2", new Vector4(settings.sunFlaresCoronaRays2Length, Mathf.Max(settings.sunFlaresCoronaRays2Streaks / 2f, 1), Mathf.Max(settings.sunFlaresCoronaRays2Spread + 0.0001f), settings.sunFlaresCoronaRays2AngleOffset));
                    bMat.SetVector("_SunGhosts1", new Vector4(0, settings.sunFlaresGhosts1Size, settings.sunFlaresGhosts1Offset, settings.sunFlaresGhosts1Brightness));
                    bMat.SetVector("_SunGhosts2", new Vector4(0, settings.sunFlaresGhosts2Size, settings.sunFlaresGhosts2Offset, settings.sunFlaresGhosts2Brightness));
                    bMat.SetVector("_SunGhosts3", new Vector4(0, settings.sunFlaresGhosts3Size, settings.sunFlaresGhosts3Offset, settings.sunFlaresGhosts3Brightness));
                    bMat.SetVector("_SunGhosts4", new Vector4(0, settings.sunFlaresGhosts4Size, settings.sunFlaresGhosts4Offset, settings.sunFlaresGhosts4Brightness));
                    bMat.SetVector("_SunHalo", new Vector3(settings.sunFlaresHaloOffset, settings.sunFlaresHaloAmplitude, settings.sunFlaresHaloIntensity * 100f));

                    // check if Sun is visible
                    Vector3 sunWorldPosition = context.camera.transform.position - sceneSettings.sun.transform.forward * 1000f;
                    Vector3 sunScrPos = context.camera.WorldToViewportPoint(sunWorldPosition);
                    float flareIntensity = 0;
                    if (sunScrPos.z > 0 && sunScrPos.x >= -0.1f && sunScrPos.x < 1.1f && sunScrPos.y >= -0.1f && sunScrPos.y < 1.1f) {
                        Vector2 dd = sunScrPos - Vector3.one * 0.5f;
                        flareIntensity = settings.sunFlaresIntensity * Mathf.Clamp01((0.6f - Mathf.Max(Mathf.Abs(dd.x), Mathf.Abs(dd.y))) / 0.6f);
                        if (settings.bloomIntensity <= 0 && settings.anamorphicFlaresIntensity <= 0) { // ensure _Bloom.x is 1 into the shader for sun flares to be visible if no bloom nor anamorphic flares are enabled
                            bMat.SetVector("_Bloom", Vector4.one);
                        } else {
                            flareIntensity /= (settings.bloomIntensity + 0.0001f);
                        }
                    }
                    sunFlareCurrentIntensity = Mathf.Lerp(sunFlareCurrentIntensity, flareIntensity, Application.isPlaying ? 0.5f : 1f);
                    if (sunFlareCurrentIntensity > 0) {
                        if (flareIntensity > 0) {
                            sunLastScrPos = sunScrPos;
                        }
                        bMat.SetColor("_SunTint", settings.sunFlaresTint.value * sunFlareCurrentIntensity);
                        sunLastScrPos.z = 0.5f + sunFlareTime * settings.sunFlaresSolarWindSpeed;
                        Vector2 sfDist = new Vector2(0.5f - sunLastScrPos.y, sunLastScrPos.x - 0.5f);
                        if (!settings.sunFlaresRotationDeadZone || sfDist.sqrMagnitude > 0.00025f) {
                            sunLastRot = Mathf.Atan2(sfDist.x, sfDist.y);
                        }
                        sunLastScrPos.w = sunLastRot;
                        sunFlareTime += Time.deltaTime;
                        bMat.SetVector("_SunPos", sunLastScrPos);
                        int width = context.camera.pixelWidth / settings.sunFlaresDownsampling;
                        int height = context.camera.pixelHeight / settings.sunFlaresDownsampling;

                        context.GetScreenSpaceTemporaryRT(context.command, rtSF, 0, rtFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, width, height);

                        int sfRenderPass = rtBloom >= 0 ? 20 : 19;
                        context.command.BlitFullscreenTriangle(rtBloom, rtSF, sheet, sfRenderPass);
                        if (settings.lensDirtIntensity > 0) {
                            if (settings.bloomIntensity > 0) {
                                BlendOneOne(context, sheet, rtSF, ref rt[settings.lensDirtSpread].rtUp, ref rt[settings.lensDirtSpread].rtDown);
                                //context.command.BlitFullscreenTriangle(rtSF, rt[settings.lensDirtSpread].rtUp, sheet, 11);
                            }
                        }
                        rtBloom = rtSF;
                    }
                }
                if (rtBloom >= 0) {
                    context.command.SetGlobalTexture("_BloomTex", new RenderTargetIdentifier(rtBloom));
                    sheet.EnableKeyword(SKW_BLOOM);
                }

                if (settings.lensDirtIntensity > 0) {
                    context.command.SetGlobalTexture("_ScreenLum", new RenderTargetIdentifier((settings.anamorphicFlaresIntensity > 0 && settings.bloomIntensity <= 0) ? rtAF[settings.lensDirtSpread].rtUp : rt[settings.lensDirtSpread].rtUp));
                }

            }

            if (settings.lensDirtIntensity > 0) {
                Vector4 dirtData = new Vector4(1.0f, settings.lensDirtIntensity * settings.lensDirtIntensity, settings.lensDirtThreshold, Mathf.Max(settings.bloomIntensity, 1f));
                bMat.SetVector("_Dirt", dirtData);
                Texture tex = settings.lensDirtTexture.value;
                if (tex == null) {
                    if (dirtTexture == null) {
                        dirtTexture = Resources.Load<Texture2D>("Textures/dirt2") as Texture2D;
                    }
                    tex = dirtTexture;
                }
                if (tex != null) {
                    bMat.SetTexture("_OverlayTex", tex);
                    sheet.EnableKeyword(SKW_DIRT);
                }
            }
        }


        void BlendOneOne(PostProcessRenderContext context, PropertySheet sheet, int source, ref int destination, ref int tempBuffer) {
            context.command.SetGlobalTexture("_CombineTex", destination); // _BloomTex used as temporary rt for combining
            context.command.BlitFullscreenTriangle(source, tempBuffer, sheet, 11);
            // swap buffers
            int tmp = destination;
            destination = tempBuffer;
            tempBuffer = tmp;
        }

        void DoDoF(PostProcessRenderContext context, PropertySheet sheet) {
            // DoF!
            if (!settings.depthOfField) return;
            MaterialPropertyBlock bMat = sheet.properties;

#if UNITY_EDITOR
            if (sceneCamera == null && context.camera != null && context.camera.cameraType == CameraType.SceneView) {
                sceneCamera = context.camera;
            }

            if (context.camera != sceneCamera) {
#endif

                sheet.EnableKeyword(SKW_DEPTH_OF_FIELD);

                //if (settings.depthOfFieldTransparencySupport.value || settings.depthOfFieldExclusionLayerMask.value != 0) {
                //    sheet.EnableKeyword(SKW_DEPTH_OF_FIELD_TRANSPARENT);
                //}
                UpdateDepthOfFieldData(context, sheet);

                int width = context.camera.pixelWidth / settings.depthOfFieldDownsampling;
                int height = context.camera.pixelHeight / settings.depthOfFieldDownsampling;
                context.GetScreenSpaceTemporaryRT(context.command, rtDoF, 0, rtFormat, RenderTextureReadWrite.Linear, settings.depthOfFieldFilterMode, width, height);
                context.command.BlitFullscreenTriangle(context.source, rtDoF, sheet, 21);

                if (settings.depthOfFieldForegroundBlur && settings.depthOfFieldForegroundBlurHQ) {
                    BlurThisAlpha(context, sheet, rtDoF, width, height, 16);
                }

                int pass = settings.depthOfFieldBokeh ? 23 : 24;
                BlurThisDoF(context, sheet, rtDoF, width, height, pass);
                context.command.SetGlobalTexture("_DoFTex", new RenderTargetIdentifier(rtDoF));
#if UNITY_EDITOR
            }
#endif
        }


        void DoEyeAdaptation(PostProcessRenderContext context, PropertySheet sheet) {

            bool requiresLuminanceComputation = Application.isPlaying && (settings.eyeAdaptation || settings.purkinje);
            if (!requiresLuminanceComputation) return;

            MaterialPropertyBlock bMat = sheet.properties;

            int sizeEA = (int)Mathf.Pow(2, rtEA.Length);

            for (int k = 0; k < rtEA.Length; k++) {
                int width = sizeEA;
                int height = sizeEA;
                context.GetScreenSpaceTemporaryRT(context.command, rtEA[k], 0, rtFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, width, height);
                sizeEA /= 2;
            }

            context.command.BlitFullscreenTriangle(context.source, rtEA[0], sheet, SHADER_RAW_COPY_PASS);

            int lumRT = rtEA.Length - 1;
            const int basePass = 13;
            for (int k = 0; k < lumRT; k++) {
                context.command.BlitFullscreenTriangle(rtEA[k], rtEA[k + 1], sheet, k == 0 ? basePass : basePass + 1);
            }
            context.command.SetGlobalTexture("_EALumSrc", new RenderTargetIdentifier(rtEA[lumRT]));
            if (rtEAacum == null) {
                RenderTextureDescriptor rtEASmallDesc = new RenderTextureDescriptor(2, 2, rtFormat, 0);
                rtEASmallDesc.vrUsage = UnityEngine.XR.XRSettings.eyeTextureDesc.vrUsage;
                rtEAacum = new RenderTexture(rtEASmallDesc);
                rtEAacum.Create();
                context.command.BlitFullscreenTriangle(rtEA[lumRT], rtEAacum, sheet, SHADER_RAW_COPY_PASS);
                rtEAHist = new RenderTexture(rtEASmallDesc);
                rtEAHist.Create();
                context.command.BlitFullscreenTriangle(rtEAacum, rtEAHist, sheet, SHADER_RAW_COPY_PASS);
            } else {
                //rtEAacum.MarkRestoreExpected();
                context.command.BlitFullscreenTriangle(rtEA[lumRT], rtEAacum, sheet, basePass + 2);
                context.command.BlitFullscreenTriangle(rtEAacum, rtEAHist, sheet, basePass + 3);
            }
            bMat.SetTexture("_EAHist", rtEAHist);

            Vector4 eaData = new Vector4(settings.eyeAdaptationMinExposure, settings.eyeAdaptationMaxExposure, settings.eyeAdaptationSpeedToDark, settings.eyeAdaptationSpeedToLight);
            bMat.SetVector("_EyeAdaptation", eaData);
            sheet.EnableKeyword(SKW_EYE_ADAPTATION);

        }

        void UpdateMaterialBloomIntensityAndThreshold(MaterialPropertyBlock mat) {
            float bloomThreshold = settings.bloomThreshold;
            float anamorphicThreshold = settings.anamorphicFlaresThreshold;
            if (QualitySettings.activeColorSpace == ColorSpace.Linear) {
                bloomThreshold *= bloomThreshold;
                anamorphicThreshold *= anamorphicThreshold;
            }
            if (settings.anamorphicFlaresIntensity > 0) {
                float intensity = settings.anamorphicFlaresIntensity / (settings.bloomIntensity + 0.0001f);
                mat.SetVector("_AFData", new Vector3(intensity, anamorphicThreshold, 0));
            }
            Vector4 b4 = new Vector4(settings.bloomIntensity + (settings.anamorphicFlaresIntensity > 0 ? 0.0001f : 0f), 0, 0, bloomThreshold);
            mat.SetVector("_Bloom", b4);
        }

        void BlurThis(PostProcessRenderContext context, PropertySheet sheet, int rt, int width, int height, float blurScale = 1f) {
            sheet.properties.SetFloat("_BlurScale", blurScale);
            context.GetScreenSpaceTemporaryRT(context.command, tempBlurRT, 0, rtFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, width, height);
            context.command.BlitFullscreenTriangle(rt, tempBlurRT, sheet, 4);
            context.command.BlitFullscreenTriangle(tempBlurRT, rt, sheet, 5);
            context.command.ReleaseTemporaryRT(tempBlurRT);
        }

        void BlurThisOneDirection(PostProcessRenderContext context, PropertySheet sheet, ref int rt, int width, int height, bool vertical, float blurScale = 1f) {
            sheet.properties.SetFloat("_BlurScale", blurScale);
            context.GetScreenSpaceTemporaryRT(context.command, tempBlurOneDirRT, 0, rtFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, width, height);
            context.command.BlitFullscreenTriangle(rt, tempBlurOneDirRT, sheet, vertical ? 5 : 4);
            //context.command.ReleaseTemporaryRT(rt);
            int aux = rt;
            rt = tempBlurOneDirRT;
            tempBlurOneDirRT = aux;
        }


        void BlurThisDoF(PostProcessRenderContext context, PropertySheet sheet, int rt, int width, int height, int renderPass) {
            context.GetScreenSpaceTemporaryRT(context.command, tempBlurDoFTemp1RT, 0, rtFormat, RenderTextureReadWrite.Linear, settings.depthOfFieldFilterMode, width, height);
            context.GetScreenSpaceTemporaryRT(context.command, tempBlurDoFTemp2RT, 0, rtFormat, RenderTextureReadWrite.Linear, settings.depthOfFieldFilterMode, width, height);
            UpdateDepthOfFieldBlurData(sheet, new Vector2(0.44721f, -0.89443f));
            context.command.BlitFullscreenTriangle(rt, tempBlurDoFTemp1RT, sheet, renderPass);
            UpdateDepthOfFieldBlurData(sheet, new Vector2(-1f, 0f));
            context.command.BlitFullscreenTriangle(tempBlurDoFTemp1RT, tempBlurDoFTemp2RT, sheet, renderPass);
            UpdateDepthOfFieldBlurData(sheet, new Vector2(0.44721f, 0.89443f));
            context.command.BlitFullscreenTriangle(tempBlurDoFTemp2RT, rt, sheet, renderPass);
            context.command.ReleaseTemporaryRT(tempBlurDoFTemp2RT);
            context.command.ReleaseTemporaryRT(tempBlurDoFTemp1RT);
        }


        void BlurThisAlpha(PostProcessRenderContext context, PropertySheet sheet, int rt, int width, int height, float blurScale = 1f) {
            sheet.properties.SetFloat("_BlurScale", blurScale);
            context.GetScreenSpaceTemporaryRT(context.command, tempBlurDoFAlphaRT, 0, rtFormat, RenderTextureReadWrite.Linear, FilterMode.Bilinear, width, height);
            context.command.BlitFullscreenTriangle(rt, tempBlurDoFAlphaRT, sheet, 25);
            context.command.BlitFullscreenTriangle(tempBlurDoFAlphaRT, rt, sheet, 26);
            context.command.ReleaseTemporaryRT(tempBlurDoFAlphaRT);
        }


        void CheckSun(PostProcessRenderContext context) {

            CheckSceneSettings(context);
            if (sceneSettings == null) return;

            // Fetch a valid Sun reference
            if (sceneSettings.sun == null) {
                Light[] lights = GameObject.FindObjectsOfType<Light>();
                for (int k = 0; k < lights.Length; k++) {
                    Light light = lights[k];
                    if (light.type == LightType.Directional && light.enabled && light.isActiveAndEnabled) {
                        sceneSettings.sun = light.transform;
                        break;
                    }
                }
            }
        }

        void UpdateDepthOfFieldData(PostProcessRenderContext context, PropertySheet sheet) {
            // TODO: get focal length from camera FOV: FOV = 2 arctan (x/2f) x = diagonal of film (0.024mm)
            CheckSceneSettings(context);
            if (sceneSettings == null) return;
            float d = settings.depthOfFieldDistance;
            switch ((int)settings.depthOfFieldFocusMode.value) {
                case (int)BeautifyDoFFocusMode.AutoFocus:
                    UpdateDoFAutofocusDistance(context);
                    d = dofLastAutofocusDistance > 0 ? dofLastAutofocusDistance : context.camera.farClipPlane;
                    BeautifySettings.depthOfFieldCurrentFocalPointDistance = dofLastAutofocusDistance;
                    break;
                case (int)BeautifyDoFFocusMode.FollowTarget:
                    if (sceneSettings.depthOfFieldTarget != null) {
                        Vector3 spos = context.camera.WorldToScreenPoint(sceneSettings.depthOfFieldTarget.position);
                        if (spos.z < 0) {
                            d = context.camera.farClipPlane;
                        } else {
                            d = Vector3.Distance(context.camera.transform.position, sceneSettings.depthOfFieldTarget.position);
                        }
                    }
                    break;
            }

            if (sceneSettings.OnBeforeFocus != null) {
                d = sceneSettings.OnBeforeFocus(d);
            }

            dofPrevDistance = Mathf.Lerp(dofPrevDistance, d, Application.isPlaying ? settings.depthOfFieldFocusSpeed * Time.deltaTime * 30f : 1f);
            float dofCoc = settings.depthOfFieldAperture * (settings.depthOfFieldFocalLength / Mathf.Max(dofPrevDistance - settings.depthOfFieldFocalLength, 0.001f)) * (1f / 0.024f);
            dofLastBokehData = new Vector4(dofPrevDistance, dofCoc, 0, 0);
            MaterialPropertyBlock bMat = sheet.properties;
            bMat.SetVector("_BokehData", dofLastBokehData);
            bMat.SetVector("_BokehData2", new Vector4(settings.depthOfFieldForegroundBlur ? settings.depthOfFieldForegroundDistance : context.camera.farClipPlane, settings.depthOfFieldMaxSamples, settings.depthOfFieldBokehThreshold, settings.depthOfFieldBokehIntensity * settings.depthOfFieldBokehIntensity));
            bMat.SetVector("_BokehData3", new Vector3(settings.depthOfFieldMaxBrightness, settings.depthOfFieldMaxDistance * (context.camera.farClipPlane + 1f), 0));
        }

        void UpdateDepthOfFieldBlurData(PropertySheet sheet, Vector2 blurDir) {
            float downsamplingRatio = 1f / (float)settings.depthOfFieldDownsampling;
            blurDir *= downsamplingRatio;
            dofLastBokehData.z = blurDir.x;
            dofLastBokehData.w = blurDir.y;
            sheet.properties.SetVector("_BokehData", dofLastBokehData);
        }

        void UpdateDoFAutofocusDistance(PostProcessRenderContext context) {
            Vector3 p = settings.depthofFieldAutofocusViewportPoint;
            p.z = 10f;
            Ray r = context.camera.ViewportPointToRay(p);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, context.camera.farClipPlane, settings.depthOfFieldAutofocusLayerMask.value)) {
                dofLastAutofocusDistance = Mathf.Clamp(hit.distance, settings.depthOfFieldAutofocusMinDistance, settings.depthOfFieldAutofocusMaxDistance);
            } else {
                dofLastAutofocusDistance = context.camera.farClipPlane;
            }
        }


        void CheckSceneSettings(PostProcessRenderContext context) {
            if (sceneSettings == null) {
                sceneSettings = context.camera.GetComponent<BeautifySettings>();
                if (sceneSettings == null) {
                    sceneSettings = context.camera.gameObject.AddComponent<BeautifySettings>();
                }
            }
        }


    }

}