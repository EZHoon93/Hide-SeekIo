using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEditor;
using UnityEditor.Rendering.PostProcessing;
using System;
using System.Collections;

namespace BeautifyForPPS {
    [PostProcessEditor(typeof(Beautify))]
    public class BeautifyEditor : PostProcessEffectEditor<Beautify> {

        const int MAX_SECTIONS = 15;
        static bool[] expandSection = new bool[MAX_SECTIONS];
        const string SECTION_PREFS = "BeautifyExpandSection";
        static string[] sectionNames = {
                                                "Sharpen",
            "Dither",
                                                "Color Tweaks",
                                                "Bloom",
            "Anamorphic Flares",
            "Sun Flares",
            "Lens Dirt",
            "Eye Adaptation",
            "Depth Of Field",
            "Purkinje",
            "Vignetting",
            "Outline"
                                };
        bool layer1, layer2, layer3, layer4, layer5, layer6;

        enum Section {
            Sharpen,
            Dither,
            ColorTweaks,
            Bloom,
            AnamorphicFlares,
            SunFlares,
            LensDirt,
            EyeAdaptation,
            DepthOfField,
            Purkinje,
            Vignetting,
            Outline
        }
        static GUIStyle titleLabelStyle, sectionHeaderStyle;
        static Color titleColor;


        SerializedParameterOverride compareMode, compareLineAngle, compareLineWidth;
        SerializedParameterOverride sharpenIntensity, sharpenDepthThreshold, sharpenMinDepth, sharpenMaxDepth, sharpenRelaxation, sharpenClamp, sharpenMotionSensibility;
        //        SerializedParameterOverride dither, ditherDepth;
        SerializedParameterOverride daltonize, sepia, saturate, brightness, contrast, tintColor;
        SerializedParameterOverride tonemap, tonemapExposurePre, tonemapBrightnessPost;
        SerializedParameterOverride bloomDebug, bloomThreshold, bloomIntensity, bloomDepthAtten, bloomAntiflicker, bloomUltra, bloomMaxBrightness, bloomCustomize;
        SerializedParameterOverride bloomWeight0, bloomWeight1, bloomWeight2, bloomWeight3, bloomWeight4, bloomWeight5;
        SerializedParameterOverride bloomBoost0, bloomBoost1, bloomBoost2, bloomBoost3, bloomBoost4, bloomBoost5;
        SerializedParameterOverride anamorphicFlaresThreshold, anamorphicFlaresDepthAtten, anamorphicFlaresIntensity, anamorphicFlaresVertical, anamorphicFlaresSpread, anamorphicFlaresAntiflicker, anamorphicFlaresUltra, anamorphicFlaresTint;
        SerializedParameterOverride sunFlaresLayerMask, sunFlaresIntensity, sunFlaresTint, sunFlaresDownsampling, sunFlaresSunIntensity, sunFlaresSunDiskSize, sunFlaresSunRayDiffractionIntensity;
        SerializedParameterOverride sunFlaresSunRayDiffractionThreshold, sunFlaresSolarWindSpeed, sunFlaresRotationDeadZone;
        SerializedParameterOverride sunFlaresCoronaRays1Length, sunFlaresCoronaRays1Streaks, sunFlaresCoronaRays1Spread, sunFlaresCoronaRays1AngleOffset;
        SerializedParameterOverride sunFlaresCoronaRays2Length, sunFlaresCoronaRays2Streaks, sunFlaresCoronaRays2Spread, sunFlaresCoronaRays2AngleOffset;
        SerializedParameterOverride sunFlaresGhosts1Size, sunFlaresGhosts1Offset, sunFlaresGhosts1Brightness;
        SerializedParameterOverride sunFlaresGhosts2Size, sunFlaresGhosts2Offset, sunFlaresGhosts2Brightness;
        SerializedParameterOverride sunFlaresGhosts3Size, sunFlaresGhosts3Offset, sunFlaresGhosts3Brightness;
        SerializedParameterOverride sunFlaresGhosts4Size, sunFlaresGhosts4Offset, sunFlaresGhosts4Brightness;
        SerializedParameterOverride sunFlaresHaloOffset, sunFlaresHaloAmplitude, sunFlaresHaloIntensity;

        SerializedParameterOverride lensDirtTexture, lensDirtThreshold, lensDirtIntensity, lensDirtSpread;
        SerializedParameterOverride eyeAdaptation, eyeAdaptationMinExposure, eyeAdaptationMaxExposure, eyeAdaptationSpeedToDark, eyeAdaptationSpeedToLight;
        SerializedParameterOverride purkinje, purkinjeAmount, purkinjeLuminanceThreshold;
        SerializedParameterOverride lut, lutIntensity, lutTexture;
        SerializedParameterOverride vignettingOuterRing, vignettingInnerRing, vignettingColor, vignettingFade, vignettingCircularShape, vignettingAspectRatio, vignettingBlink, vignettingMask;

        SerializedParameterOverride depthOfField;
        //SerializedParameterOverride depthOfFieldTransparencySupport, depthOfFieldExclusionLayerMask, depthOfFieldTransparencyLayerMask;
        SerializedParameterOverride depthOfFieldDebug, depthOfFieldMaxSamples, depthOfFieldFocusMode, depthofFieldAutofocusViewportPoint, depthOfFieldAutofocusMinDistance, depthOfFieldAutofocusMaxDistance, depthOfFieldAutofocusLayerMask;
        SerializedParameterOverride depthOfFieldDistance, depthOfFieldFocusSpeed, depthOfFieldFocalLength, depthOfFieldAperture, depthOfFieldForegroundBlur, depthOfFieldForegroundBlurHQ;
        SerializedParameterOverride depthOfFieldForegroundDistance, depthOfFieldMaxBrightness, depthOfFieldMaxDistance, depthOfFieldBokeh, depthOfFieldBokehThreshold, depthOfFieldBokehIntensity;
        SerializedParameterOverride depthOfFieldDownsampling;
        SerializedParameterOverride depthOfFieldExclusionBias, depthOfFieldExclusionLayerMaskDownsampling;
        SerializedParameterOverride outline, outlineColor;
        //SerializedParameterOverride depthOfFieldTransparencySupportDownsampling, depthOfFieldFilterMode;



        public override void OnEnable() {
            titleColor = EditorGUIUtility.isProSkin ? new Color(0.52f, 0.66f, 0.9f) : new Color(0.12f, 0.16f, 0.4f);
            for (int k = 0; k < expandSection.Length; k++) {
                expandSection[k] = EditorPrefs.GetBool(SECTION_PREFS + k, true);
            }

            compareMode = FindParameterOverride(x => x.compareMode);
            compareLineAngle = FindParameterOverride(x => x.compareLineAngle);
            compareLineWidth = FindParameterOverride(x => x.compareLineWidth);

            sharpenIntensity = FindParameterOverride(x => x.sharpenIntensity);
            sharpenDepthThreshold = FindParameterOverride(x => x.sharpenDepthThreshold);
            sharpenMinDepth = FindParameterOverride(x => x.sharpenMinDepth);
            sharpenMaxDepth = FindParameterOverride(x => x.sharpenMaxDepth);
            sharpenRelaxation = FindParameterOverride(x => x.sharpenRelaxation);
            sharpenClamp = FindParameterOverride(x => x.sharpenClamp);
            sharpenMotionSensibility = FindParameterOverride(x => x.sharpenMotionSensibility);
            /*
                        dither = FindParameterOverride(x => x.dither);
                        ditherDepth = FindParameterOverride(x => x.ditherDepth);
            */
            daltonize = FindParameterOverride(x => x.daltonize);
            sepia = FindParameterOverride(x => x.sepia);
            saturate = FindParameterOverride(x => x.saturate);
            brightness = FindParameterOverride(x => x.brightness);
            contrast = FindParameterOverride(x => x.contrast);
            tintColor = FindParameterOverride(x => x.tintColor);
            tonemap = FindParameterOverride(x => x.tonemap);
            tonemapExposurePre = FindParameterOverride(x => x.tonemapExposurePre);
            tonemapBrightnessPost = FindParameterOverride(x => x.tonemapBrightnessPost);

            lut = FindParameterOverride(x => x.lut);
            lutIntensity = FindParameterOverride(x => x.lutIntensity);
            lutTexture = FindParameterOverride(x => x.lutTexture);

            bloomIntensity = FindParameterOverride(x => x.bloomIntensity);
            bloomThreshold = FindParameterOverride(x => x.bloomThreshold);
            bloomMaxBrightness = FindParameterOverride(x => x.bloomMaxBrightness);
            bloomDepthAtten = FindParameterOverride(x => x.bloomDepthAtten);
            bloomAntiflicker = FindParameterOverride(x => x.bloomAntiflicker);
            bloomUltra = FindParameterOverride(x => x.bloomUltra);
            bloomCustomize = FindParameterOverride(x => x.bloomCustomize);
            bloomWeight0 = FindParameterOverride(x => x.bloomWeight0);
            bloomWeight1 = FindParameterOverride(x => x.bloomWeight1);
            bloomWeight2 = FindParameterOverride(x => x.bloomWeight2);
            bloomWeight3 = FindParameterOverride(x => x.bloomWeight3);
            bloomWeight4 = FindParameterOverride(x => x.bloomWeight4);
            bloomWeight5 = FindParameterOverride(x => x.bloomWeight5);
            bloomBoost0 = FindParameterOverride(x => x.bloomBoost0);
            bloomBoost1 = FindParameterOverride(x => x.bloomBoost1);
            bloomBoost2 = FindParameterOverride(x => x.bloomBoost2);
            bloomBoost3 = FindParameterOverride(x => x.bloomBoost3);
            bloomBoost4 = FindParameterOverride(x => x.bloomBoost4);
            bloomBoost5 = FindParameterOverride(x => x.bloomBoost5);
            bloomDebug = FindParameterOverride(x => x.bloomDebug);

            anamorphicFlaresIntensity = FindParameterOverride(x => x.anamorphicFlaresIntensity);
            anamorphicFlaresThreshold = FindParameterOverride(x => x.anamorphicFlaresThreshold);
            anamorphicFlaresDepthAtten = FindParameterOverride(x => x.anamorphicFlaresDepthAtten);
            anamorphicFlaresVertical = FindParameterOverride(x => x.anamorphicFlaresVertical);
            anamorphicFlaresSpread = FindParameterOverride(x => x.anamorphicFlaresSpread);
            anamorphicFlaresAntiflicker = FindParameterOverride(x => x.anamorphicFlaresAntiflicker);
            anamorphicFlaresUltra = FindParameterOverride(x => x.anamorphicFlaresUltra);
            anamorphicFlaresTint = FindParameterOverride(x => x.anamorphicFlaresTint);

            sunFlaresLayerMask = FindParameterOverride(x => x.sunFlaresLayerMask);
            sunFlaresIntensity = FindParameterOverride(x => x.sunFlaresIntensity);
            sunFlaresTint = FindParameterOverride(x => x.sunFlaresTint);
            sunFlaresDownsampling = FindParameterOverride(x => x.sunFlaresDownsampling);
            sunFlaresSunIntensity = FindParameterOverride(x => x.sunFlaresSunIntensity);
            sunFlaresSunDiskSize = FindParameterOverride(x => x.sunFlaresSunDiskSize);
            sunFlaresSunRayDiffractionIntensity = FindParameterOverride(x => x.sunFlaresSunRayDiffractionIntensity);
            sunFlaresSunRayDiffractionThreshold = FindParameterOverride(x => x.sunFlaresSunRayDiffractionThreshold);
            sunFlaresSolarWindSpeed = FindParameterOverride(x => x.sunFlaresSolarWindSpeed);
            sunFlaresRotationDeadZone = FindParameterOverride(x => x.sunFlaresRotationDeadZone);
            sunFlaresCoronaRays1Length = FindParameterOverride(x => x.sunFlaresCoronaRays1Length);
            sunFlaresCoronaRays1Streaks = FindParameterOverride(x => x.sunFlaresCoronaRays1Streaks);
            sunFlaresCoronaRays1Spread = FindParameterOverride(x => x.sunFlaresCoronaRays1Spread);
            sunFlaresCoronaRays1AngleOffset = FindParameterOverride(x => x.sunFlaresCoronaRays1AngleOffset);
            sunFlaresCoronaRays2Length = FindParameterOverride(x => x.sunFlaresCoronaRays2Length);
            sunFlaresCoronaRays2Streaks = FindParameterOverride(x => x.sunFlaresCoronaRays2Streaks);
            sunFlaresCoronaRays2Spread = FindParameterOverride(x => x.sunFlaresCoronaRays2Spread);
            sunFlaresCoronaRays2AngleOffset = FindParameterOverride(x => x.sunFlaresCoronaRays2AngleOffset);
            sunFlaresGhosts1Size = FindParameterOverride(x => x.sunFlaresGhosts1Size);
            sunFlaresGhosts1Offset = FindParameterOverride(x => x.sunFlaresGhosts1Offset);
            sunFlaresGhosts1Brightness = FindParameterOverride(x => x.sunFlaresGhosts1Brightness);
            sunFlaresGhosts2Size = FindParameterOverride(x => x.sunFlaresGhosts2Size);
            sunFlaresGhosts2Offset = FindParameterOverride(x => x.sunFlaresGhosts2Offset);
            sunFlaresGhosts2Brightness = FindParameterOverride(x => x.sunFlaresGhosts2Brightness);
            sunFlaresGhosts3Size = FindParameterOverride(x => x.sunFlaresGhosts3Size);
            sunFlaresGhosts3Offset = FindParameterOverride(x => x.sunFlaresGhosts3Offset);
            sunFlaresGhosts3Brightness = FindParameterOverride(x => x.sunFlaresGhosts3Brightness);
            sunFlaresGhosts4Size = FindParameterOverride(x => x.sunFlaresGhosts4Size);
            sunFlaresGhosts4Offset = FindParameterOverride(x => x.sunFlaresGhosts4Offset);
            sunFlaresGhosts4Brightness = FindParameterOverride(x => x.sunFlaresGhosts4Brightness);
            sunFlaresHaloOffset = FindParameterOverride(x => x.sunFlaresHaloOffset);
            sunFlaresHaloAmplitude = FindParameterOverride(x => x.sunFlaresHaloAmplitude);
            sunFlaresHaloIntensity = FindParameterOverride(x => x.sunFlaresHaloIntensity);

            lensDirtThreshold = FindParameterOverride(x => x.lensDirtThreshold);
            lensDirtIntensity = FindParameterOverride(x => x.lensDirtIntensity);
            lensDirtTexture = FindParameterOverride(x => x.lensDirtTexture);
            lensDirtSpread = FindParameterOverride(x => x.lensDirtSpread);

            eyeAdaptation = FindParameterOverride(x => x.eyeAdaptation);
            eyeAdaptationMaxExposure = FindParameterOverride(x => x.eyeAdaptationMaxExposure);
            eyeAdaptationMinExposure = FindParameterOverride(x => x.eyeAdaptationMinExposure);
            eyeAdaptationSpeedToDark = FindParameterOverride(x => x.eyeAdaptationSpeedToDark);
            eyeAdaptationSpeedToLight = FindParameterOverride(x => x.eyeAdaptationSpeedToLight);

            purkinje = FindParameterOverride(x => x.purkinje);
            purkinjeAmount = FindParameterOverride(x => x.purkinjeAmount);
            purkinjeLuminanceThreshold = FindParameterOverride(x => x.purkinjeLuminanceThreshold);

            vignettingOuterRing = FindParameterOverride(x => x.vignettingOuterRing);
            vignettingInnerRing = FindParameterOverride(x => x.vignettingInnerRing);
            vignettingColor = FindParameterOverride(x => x.vignettingColor);
            vignettingFade = FindParameterOverride(x => x.vignettingFade);
            vignettingCircularShape = FindParameterOverride(x => x.vignettingCircularShape);
            vignettingAspectRatio = FindParameterOverride(x => x.vignettingAspectRatio);
            vignettingBlink = FindParameterOverride(x => x.vignettingBlink);
            vignettingMask = FindParameterOverride(x => x.vignettingMask);

            // Depth of Field
            depthOfField = FindParameterOverride(x => x.depthOfField);
            //depthOfFieldTransparencySupport = FindParameterOverride(x => x.depthOfFieldTransparencySupport);
            //depthOfFieldExclusionLayerMask = FindParameterOverride(x => x.depthOfFieldExclusionLayerMask);
            depthOfFieldDebug = FindParameterOverride(x => x.depthOfFieldDebug);
            depthOfFieldFocusMode = FindParameterOverride(x => x.depthOfFieldFocusMode);
            depthofFieldAutofocusViewportPoint = FindParameterOverride(x => x.depthofFieldAutofocusViewportPoint);
            depthOfFieldAutofocusMinDistance = FindParameterOverride(x => x.depthOfFieldAutofocusMinDistance);
            depthOfFieldAutofocusMaxDistance = FindParameterOverride(x => x.depthOfFieldAutofocusMaxDistance);
            depthOfFieldAutofocusLayerMask = FindParameterOverride(x => x.depthOfFieldAutofocusLayerMask);
            depthOfFieldDistance = FindParameterOverride(x => x.depthOfFieldDistance);
            depthOfFieldFocusSpeed = FindParameterOverride(x => x.depthOfFieldFocusSpeed);
            depthOfFieldFocalLength = FindParameterOverride(x => x.depthOfFieldFocalLength);
            depthOfFieldAperture = FindParameterOverride(x => x.depthOfFieldAperture);
            depthOfFieldForegroundBlur = FindParameterOverride(x => x.depthOfFieldForegroundBlur);
            depthOfFieldForegroundBlurHQ = FindParameterOverride(x => x.depthOfFieldForegroundBlurHQ);
            depthOfFieldForegroundDistance = FindParameterOverride(x => x.depthOfFieldForegroundDistance);
            depthOfFieldMaxBrightness = FindParameterOverride(x => x.depthOfFieldMaxBrightness);
            depthOfFieldMaxDistance = FindParameterOverride(x => x.depthOfFieldMaxDistance);
            depthOfFieldBokeh = FindParameterOverride(x => x.depthOfFieldBokeh);
            depthOfFieldBokehThreshold = FindParameterOverride(x => x.depthOfFieldBokehThreshold);
            depthOfFieldBokehIntensity = FindParameterOverride(x => x.depthOfFieldBokehIntensity);
            depthOfFieldDownsampling = FindParameterOverride(x => x.depthOfFieldDownsampling);
            depthOfFieldMaxSamples = FindParameterOverride(x => x.depthOfFieldMaxSamples);
            //depthOfFieldExclusionBias = FindParameterOverride(x => x.depthOfFieldExclusionBias);
            //depthOfFieldExclusionLayerMaskDownsampling = FindParameterOverride(x => x.depthOfFieldExclusionLayerMaskDownsampling);
            //depthOfFieldTransparencySupportDownsampling = FindParameterOverride(x => x.depthOfFieldTransparencySupportDownsampling);
            //depthOfFieldTransparencyLayerMask = FindParameterOverride(x => x.depthOfFieldTransparencyLayerMask);
            //depthOfFieldFilterMode = FindParameterOverride(x => x.depthOfFieldFilterMode);

            outline = FindParameterOverride(x => x.outline);
            outlineColor = FindParameterOverride(x => x.outlineColor);


        }

        public override void OnDisable() {
            base.OnDisable();
            // Save folding sections state
            for (int k = 0; k < expandSection.Length; k++) {
                EditorPrefs.SetBool(SECTION_PREFS + k, expandSection[k]);
            }
        }

        public override void OnInspectorGUI() {

            if (sectionHeaderStyle == null) {
                sectionHeaderStyle = new GUIStyle(EditorStyles.foldout);
            }
            sectionHeaderStyle.SetFoldoutColor();

            if (titleLabelStyle == null) {
                titleLabelStyle = new GUIStyle(EditorStyles.label);
            }
            titleLabelStyle.normal.textColor = titleColor;
            titleLabelStyle.fontStyle = FontStyle.Bold;

            if (Shader.Find("Beautify/Beautify") != null) {
                EditorGUILayout.HelpBox("One or more Beautify shaders for built-in pipeline are present. Consider removing them to reduce build time and size.", MessageType.Warning);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Locate built-in Beautify shaders", GUILayout.Width(250))) {
                    LocateLegacyShaders();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
            }

            EditorGUILayout.BeginHorizontal();
            PropertyField(compareMode, new GUIContent("Compare Mode", "Shows a side by side comparison."));
            if (GUILayout.Button("Help", GUILayout.Width(40))) {
                if (!EditorUtility.DisplayDialog("Beautify", "To learn more about a property in this inspector move the mouse over the label for a quick description (tooltip).\n\nPlease check README file in the root of the asset for details and contact support.\n\nIf you like Beautify, please rate it on the Asset Store. For feedback and suggestions visit our support forum on kronnect.com.", "Close", "Visit Support Forum")) {
                    Application.OpenURL("http://kronnect.com/taptapgo");
                }
            }
            EditorGUILayout.EndHorizontal();
            if (compareMode.value.boolValue && compareMode.overrideState.boolValue) {
                EditorGUI.indentLevel++;
                PropertyField(compareLineAngle, new GUIContent("Angle", "Angle of the separator line."));
                PropertyField(compareLineWidth, new GUIContent("Width", "Width of the separator line."));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Separator();

            expandSection[(int)Section.Sharpen] = EditorGUILayout.Foldout(expandSection[(int)Section.Sharpen], sectionNames[(int)Section.Sharpen], sectionHeaderStyle);
            if (expandSection[(int)Section.Sharpen]) {
                PropertyField(sharpenIntensity, new GUIContent("Intensity", "Intensity of sharpening effect."));
                PropertyField(sharpenMinDepth, new GUIContent("Min Depth", "Any pixel outside this depth range won't be affected by sharpen."));
                PropertyField(sharpenMaxDepth, new GUIContent("Max Depth", "Any pixel outside this depth range won't be affected by sharpen. Reduce range to create a subtle depth-of-field-like effect due to the unsharpen background."));
                PropertyField(sharpenDepthThreshold, new GUIContent("Depth Threshold", "Reduces sharpen if depth difference around a pixel exceeds this value. Useful to prevent artifacts around wires or thin objects."));
                PropertyField(sharpenRelaxation, new GUIContent("Luminance Relax.", "Soften sharpen around a pixel with high contrast. Reduce this value to remove ghosting and protect fine drawings or wires over a flat surface."));
                PropertyField(sharpenClamp, new GUIContent("Clamp", "Maximum modification."));
                PropertyField(sharpenMotionSensibility, new GUIContent("Motion Sensibility", "Increase to reduce sharpen to simulate a cheap motion blur and to reduce flickering when camera rotates or moves. This slider controls the amount of camera movement/rotation that contributes to sharpen reduction. Set this to 0 to disable this feature."));
                EditorGUILayout.Separator();
            }

            /*
            expandSection[(int)Section.Dither] = EditorGUILayout.Foldout(expandSection[(int)Section.Dither], sectionNames[(int)Section.Dither], sectionHeaderStyle);
            if (expandSection[(int)Section.Dither]) {
                PropertyField(dither, new GUIContent("Dither", "Simulates more colors than RGB quantization can produce. Removes banding artifacts in gradients, like skybox. This setting controls the dithering strength."));
                PropertyField(ditherDepth, new GUIContent("   Min Depth", "Will only remove bands on pixels beyond this depth. Useful if you only want to remove sky banding (set this to 0.99)"));
                EditorGUILayout.Separator();
            }
            */

            expandSection[(int)Section.ColorTweaks] = EditorGUILayout.Foldout(expandSection[(int)Section.ColorTweaks], sectionNames[(int)Section.ColorTweaks], sectionHeaderStyle);
            if (expandSection[(int)Section.ColorTweaks]) {
                PropertyField(daltonize, new GUIContent("Daltonize", "Similar to vibrance but mostly accentuate primary red, green and blue colors to compensate protanomaly (red deficiency), deuteranomaly (green deficiency) and tritanomaly (blue deficiency). This effect does not shift color hue hence it won't help completely red, green or blue color blindness. The effect will vary depending on each subject so this effect should be enabled on user demand."));
                PropertyField(tonemap, new GUIContent("Tonemap", "Tonemap operator."));
                if (tonemap.value.intValue != (int)BeautifyForPPS.BeautifyTonemapOperator.Linear) {
                    PropertyField(tonemapExposurePre, new GUIContent("Exposure", "Brightness multiplier applied before tonemap is applied."));
                    PropertyField(tonemapBrightnessPost, new GUIContent("Brightness Correction", "Brightness multiplier applied after tonemap is applied."));
                }
                PropertyField(lut, new GUIContent("LUT", "Enables Look-Up-Texture transformation"));
                if (lut.value.boolValue) {
                    EditorGUILayout.BeginHorizontal();
                    PropertyField(lutTexture, new GUIContent("   Texture"));
                    if (GUILayout.Button("Help", GUILayout.Width(50))) {
                        EditorUtility.DisplayDialog("LUT Requirements", "LUT textures must be of 1024x32 dimensions.\nA sample Sepia LUT texture can be found in Beautify/Resources/Textures folder.\n\nEnsure the following import settings are set in your LUT textures:\n- Uncheck sRGM Texture (no gamma conversion)\n- No compression\n- Disable mip mapping\n- Aniso set to 0\n- Filtering set to Bilinear\n- Wrapping set to Clamp", "Ok");
                    }
                    EditorGUILayout.EndHorizontal();
                    CheckLUTSettings((Texture2D)lutTexture.value.objectReferenceValue);
                    PropertyField(lutIntensity, new GUIContent("   Intensity", "Intensity of the LUT transformation."));
                }

                PropertyField(sepia, new GUIContent("Sepia", "Enables Sepia color transformation effect."));
                PropertyField(saturate, new GUIContent("Vibrance", "Improves pixels color depending on their saturation."));
                PropertyField(tintColor, new GUIContent("Tint Color", "Blends image with an optional color. Alpha specifies intensity."));
                PropertyField(contrast, new GUIContent("Final Contrast", "Final image contrast adjustment. Allows you to create more vivid images."));
                PropertyField(brightness, new GUIContent("Final Brightness", "Final image brightness adjustment."));

                EditorGUILayout.Separator();
            }

            expandSection[(int)Section.Bloom] = EditorGUILayout.Foldout(expandSection[(int)Section.Bloom], sectionNames[(int)Section.Bloom], sectionHeaderStyle);
            if (expandSection[(int)Section.Bloom]) {
                PropertyField(bloomIntensity, new GUIContent("Intensity", "Bloom multiplier."));
                PropertyField(bloomThreshold, new GUIContent("Threshold", "Brightness threshold."));
                PropertyField(bloomDepthAtten, new GUIContent("Depth Atten", "Reduces bloom effect on distance."));
                PropertyField(bloomMaxBrightness, new GUIContent("Clamp Brightness", "Clamps maximum pixel brightness to prevent out of range bright spots."));
                PropertyField(bloomAntiflicker, new GUIContent("Reduce Flicker", "Enables an additional filter to reduce excess of flicker."));
                PropertyField(bloomUltra, new GUIContent("Ultra", "Increase bloom fidelity."));
                PropertyField(bloomDebug, new GUIContent("Debug"));

                bool prevCustomize = bloomCustomize.value.boolValue;
                PropertyField(bloomCustomize, new GUIContent("Customize", "Edit bloom style parameters."));
                if (bloomCustomize.value.boolValue) {
                    if (!prevCustomize) {
                        layer1 = layer2 = layer3 = layer4 = layer5 = layer6 = true;
                    }

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Presets", GUILayout.Width(120));
                    if (GUILayout.Button("Focused")) {
                        bloomWeight0.overrideState.boolValue = bloomWeight1.overrideState.boolValue = bloomWeight2.overrideState.boolValue = bloomWeight3.overrideState.boolValue = bloomWeight4.overrideState.boolValue = bloomWeight5.overrideState.boolValue = true;
                        bloomBoost0.overrideState.boolValue = bloomBoost1.overrideState.boolValue = bloomBoost2.overrideState.boolValue = bloomBoost3.overrideState.boolValue = bloomBoost4.overrideState.boolValue = bloomBoost5.overrideState.boolValue = true;
                        bloomWeight0.value.floatValue = 1f;
                        bloomWeight1.value.floatValue = 0.9f;
                        bloomWeight2.value.floatValue = 0.75f;
                        bloomWeight3.value.floatValue = 0.6f;
                        bloomWeight4.value.floatValue = 0.35f;
                        bloomWeight5.value.floatValue = 0.1f;
                        bloomBoost0.value.floatValue = bloomBoost1.value.floatValue = bloomBoost2.value.floatValue = bloomBoost3.value.floatValue = bloomBoost4.value.floatValue = bloomBoost5.value.floatValue = 0;
                    }
                    if (GUILayout.Button("Regular")) {
                        bloomWeight0.overrideState.boolValue = bloomWeight1.overrideState.boolValue = bloomWeight2.overrideState.boolValue = bloomWeight3.overrideState.boolValue = bloomWeight4.overrideState.boolValue = bloomWeight5.overrideState.boolValue = true;
                        bloomBoost0.overrideState.boolValue = bloomBoost1.overrideState.boolValue = bloomBoost2.overrideState.boolValue = bloomBoost3.overrideState.boolValue = bloomBoost4.overrideState.boolValue = bloomBoost5.overrideState.boolValue = true;
                        bloomWeight0.value.floatValue = 0.25f;
                        bloomWeight1.value.floatValue = 0.33f;
                        bloomWeight2.value.floatValue = 0.8f;
                        bloomWeight3.value.floatValue = 1f;
                        bloomWeight4.value.floatValue = 1f;
                        bloomWeight5.value.floatValue = 1f;
                        bloomBoost0.value.floatValue = bloomBoost1.value.floatValue = bloomBoost2.value.floatValue = bloomBoost3.value.floatValue = bloomBoost4.value.floatValue = bloomBoost5.value.floatValue = 0;
                    }
                    if (GUILayout.Button("Blurred")) {
                        bloomWeight0.overrideState.boolValue = bloomWeight1.overrideState.boolValue = bloomWeight2.overrideState.boolValue = bloomWeight3.overrideState.boolValue = bloomWeight4.overrideState.boolValue = bloomWeight5.overrideState.boolValue = true;
                        bloomBoost0.overrideState.boolValue = bloomBoost1.overrideState.boolValue = bloomBoost2.overrideState.boolValue = bloomBoost3.overrideState.boolValue = bloomBoost4.overrideState.boolValue = bloomBoost5.overrideState.boolValue = true;
                        bloomWeight0.value.floatValue = 0.05f;
                        bloomWeight1.value.floatValue = 0.075f;
                        bloomWeight2.value.floatValue = 0.1f;
                        bloomWeight3.value.floatValue = 0.2f;
                        bloomWeight4.value.floatValue = 0.4f;
                        bloomWeight5.value.floatValue = 1f;
                        bloomBoost0.value.floatValue = bloomBoost1.value.floatValue = bloomBoost2.value.floatValue = bloomBoost3.value.floatValue = 0;
                        bloomBoost4.value.floatValue = 0.5f;
                        bloomBoost5.value.floatValue = 1f;
                    }
                    EditorGUILayout.EndHorizontal();

                    layer1 = EditorGUILayout.Foldout(layer1, "Layer 1");
                    if (layer1) {
                        PropertyField(bloomWeight0, new GUIContent("Weight", "First layer bloom weight."));
                        PropertyField(bloomBoost0, new GUIContent("Boost", "Intensity bonus for first layer."));
                    }
                    layer2 = EditorGUILayout.Foldout(layer2, "Layer 2");
                    if (layer2) {
                        PropertyField(bloomWeight1, new GUIContent("Weight", "Second layer bloom weight."));
                        PropertyField(bloomBoost1, new GUIContent("Boost", "Intensity bonus for second layer."));
                    }
                    layer3 = EditorGUILayout.Foldout(layer3, "Layer 3");
                    if (layer3) {
                        PropertyField(bloomWeight2, new GUIContent("Weight", "Third layer bloom weight."));
                        PropertyField(bloomBoost2, new GUIContent("Boost", "Intensity bonus for third layer."));
                    }
                    layer4 = EditorGUILayout.Foldout(layer4, "Layer 4");
                    if (layer4) {
                        PropertyField(bloomWeight3, new GUIContent("Weight", "Fourth layer bloom weight."));
                        PropertyField(bloomBoost3, new GUIContent("Boost", "Intensity bonus for fourth layer."));
                    }
                    layer5 = EditorGUILayout.Foldout(layer5, "Layer 5");
                    if (layer5) {
                        PropertyField(bloomWeight4, new GUIContent("Weight", "Fifth layer bloom weight."));
                        PropertyField(bloomBoost4, new GUIContent("Boost", "Intensity bonus for fifth layer."));
                    }
                    layer6 = EditorGUILayout.Foldout(layer6, "Layer 6");
                    if (layer6) {
                        PropertyField(bloomWeight5, new GUIContent("Weight", "Sixth layer bloom weight."));
                        PropertyField(bloomBoost5, new GUIContent("Boost", "Intensity bonus for sixth layer."));
                    }
                }
                EditorGUILayout.Separator();
            }

            expandSection[(int)Section.LensDirt] = EditorGUILayout.Foldout(expandSection[(int)Section.LensDirt], sectionNames[(int)Section.LensDirt], sectionHeaderStyle);
            if (expandSection[(int)Section.LensDirt]) {
                PropertyField(lensDirtIntensity, new GUIContent("Intensity", "This slider controls the maximum brightness of lens dirt effect."));
                PropertyField(lensDirtTexture, new GUIContent("Dirt Texture", "Texture used for the lens dirt effect."));
                PropertyField(lensDirtThreshold, new GUIContent("Threshold", "This slider controls the visibility of lens dirt. A high value will make lens dirt only visible when looking directly towards a light source. A lower value will make lens dirt visible all time."));
                PropertyField(lensDirtSpread, new GUIContent("Spread", "Spread amount of light over the lens."));
                EditorGUILayout.Separator();
            }

            expandSection[(int)Section.AnamorphicFlares] = EditorGUILayout.Foldout(expandSection[(int)Section.AnamorphicFlares], sectionNames[(int)Section.AnamorphicFlares], sectionHeaderStyle);
            if (expandSection[(int)Section.AnamorphicFlares]) {
                PropertyField(anamorphicFlaresIntensity, new GUIContent("Intensity"));
                PropertyField(anamorphicFlaresThreshold, new GUIContent("Threshold"));
                PropertyField(anamorphicFlaresTint, new GUIContent("Tint Color"));
                PropertyField(anamorphicFlaresDepthAtten, new GUIContent("Depth Atten"));
                PropertyField(anamorphicFlaresVertical, new GUIContent("Vertical"));
                PropertyField(anamorphicFlaresSpread, new GUIContent("Spread"));
                PropertyField(anamorphicFlaresAntiflicker, new GUIContent("Anti Flicker"));
                PropertyField(anamorphicFlaresUltra, new GUIContent("Ultra"));
                PropertyField(bloomDebug, new GUIContent("Debug"));
                EditorGUILayout.Separator();
            }

            expandSection[(int)Section.SunFlares] = EditorGUILayout.Foldout(expandSection[(int)Section.SunFlares], sectionNames[(int)Section.SunFlares], sectionHeaderStyle);
            if (expandSection[(int)Section.SunFlares]) {
                PropertyField(sunFlaresIntensity, new GUIContent("Global Intensity", "Global intensity for the sun flares buffer."));
                PropertyField(bloomDebug, new GUIContent("Debug"));
                PropertyField(sunFlaresTint, new GUIContent("Tint", "Global flares tint color."));
                PropertyField(sunFlaresDownsampling, new GUIContent("Downsampling", "Reduces sun flares buffer dimensions to improve performance."));
                PropertyField(sunFlaresSunIntensity, new GUIContent("Sun Intensity", "Intensity for the Sun's disk and corona rays."));
                PropertyField(sunFlaresSunDiskSize, new GUIContent("Sun Disk Size", "Size of Sun disk."));
                PropertyField(sunFlaresSunRayDiffractionIntensity, new GUIContent("Diffraction Intensity", "Intensity for the Sun's rays diffraction."));
                PropertyField(sunFlaresSunRayDiffractionThreshold, new GUIContent("Diffraction Threshold", "Theshold of the Sun's rays diffraction."));
                PropertyField(sunFlaresSolarWindSpeed, new GUIContent("Solar Wind Speed", "Animation speed for the diffracted rays."));
                PropertyField(sunFlaresRotationDeadZone, new GUIContent("Rotation DeadZone", "Prevents ray rotation when looking directly to the Sun."));
                PropertyField(sunFlaresLayerMask, new GUIContent("Occlusion Mask", "Specifies which objects can occlude Sun thus deactivate the Sun flares effect."));
                PropertyField(sunFlaresCoronaRays1Length, new GUIContent("Length", "Length of solar corona rays group 1."));
                PropertyField(sunFlaresCoronaRays1Streaks, new GUIContent("Streaks", "Number of streaks for group 1."));
                PropertyField(sunFlaresCoronaRays1Spread, new GUIContent("Spread", "Light spread factor for group 1."));
                PropertyField(sunFlaresCoronaRays1AngleOffset, new GUIContent("Angle Offset", "Rotation offset for group 1."));
                PropertyField(sunFlaresCoronaRays2Length, new GUIContent("Length", "Length of solar corona rays group 2."));
                PropertyField(sunFlaresCoronaRays2Streaks, new GUIContent("Streaks", "Number of streaks for group 2."));
                PropertyField(sunFlaresCoronaRays2Spread, new GUIContent("Spread", "Light spread factor for group 2."));
                PropertyField(sunFlaresCoronaRays2AngleOffset, new GUIContent("Angle Offset", "Rotation offset for group 2."));
                PropertyField(sunFlaresGhosts1Size, new GUIContent("Size"));
                PropertyField(sunFlaresGhosts1Offset, new GUIContent("Offset"));
                PropertyField(sunFlaresGhosts1Brightness, new GUIContent("Brightness"));
                PropertyField(sunFlaresGhosts2Size, new GUIContent("Size"));
                PropertyField(sunFlaresGhosts2Offset, new GUIContent("Offset"));
                PropertyField(sunFlaresGhosts2Brightness, new GUIContent("Brightness"));
                PropertyField(sunFlaresGhosts3Size, new GUIContent("Size"));
                PropertyField(sunFlaresGhosts3Offset, new GUIContent("Offset"));
                PropertyField(sunFlaresGhosts3Brightness, new GUIContent("Brightness"));
                PropertyField(sunFlaresGhosts4Size, new GUIContent("Size"));
                PropertyField(sunFlaresGhosts4Offset, new GUIContent("Offset"));
                PropertyField(sunFlaresGhosts4Brightness, new GUIContent("Brightness"));
                PropertyField(sunFlaresHaloOffset, new GUIContent("Offset"));
                PropertyField(sunFlaresHaloAmplitude, new GUIContent("Amplitude"));
                PropertyField(sunFlaresHaloIntensity, new GUIContent("Intensity"));
                EditorGUILayout.Separator();
            }

            expandSection[(int)Section.DepthOfField] = EditorGUILayout.Foldout(expandSection[(int)Section.DepthOfField], sectionNames[(int)Section.DepthOfField], sectionHeaderStyle);
            if (expandSection[(int)Section.DepthOfField]) {
                PropertyField(depthOfField, new GUIContent("Enable", "Enables depth of field effect."));
                if (depthOfField.value.boolValue) {
                    PropertyField(depthOfFieldDebug, new GUIContent("Debug", "Enable to see depth of field focus area."));
                }
                PropertyField(depthOfFieldFocusMode, new GUIContent("Focus Mode", "Select auto focus mode."));
                switch (depthOfFieldFocusMode.value.intValue) {
                    case (int)BeautifyDoFFocusMode.AutoFocus:
                        PropertyField(depthOfFieldAutofocusLayerMask, new GUIContent("   Layer Mask", "Select which layers can be used for autofocus option."));
                        PropertyField(depthOfFieldAutofocusMinDistance, new GUIContent("   Min Distance", "Minimum distance accepted for any focused object."));
                        PropertyField(depthOfFieldAutofocusMaxDistance, new GUIContent("   Max Distance", "Maximum distance accepted for any focused object."));
                        PropertyField(depthofFieldAutofocusViewportPoint, new GUIContent("   Viewport Point", "Viewport position from where cast the ray."));
                        string dist = "---";
                        if (Application.isPlaying && depthOfField.value.boolValue) {
                            dist = BeautifySettings.depthOfFieldCurrentFocalPointDistance.ToString("F2");
                        }
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("(Current Distance: " + dist + ")");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                        break;
                    case (int)BeautifyDoFFocusMode.FollowTarget:
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("(Focus Target: assign target in Scene Settings component)");
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                        break;
                    case (int)BeautifyDoFFocusMode.FixedDistance:
                        PropertyField(depthOfFieldDistance, new GUIContent("   Distance", "Distance to focus point."));
                        break;
                }
                PropertyField(depthOfFieldFocusSpeed, new GUIContent("Focus Speed", "1=immediate focus on distance or target."));
                PropertyField(depthOfFieldFocalLength, new GUIContent("Focal Length", "Focal length of the virtual lens."));
                PropertyField(depthOfFieldAperture, new GUIContent("Aperture", "Diameter of the aperture (mm)."));
                PropertyField(depthOfFieldForegroundBlur, new GUIContent("Foreground Blur", "Enables blur in front of focus object."));
                if (depthOfFieldForegroundBlur.value.boolValue) {
                    PropertyField(depthOfFieldForegroundDistance, new GUIContent("   Near Offset", "Distance from focus plane for foreground blur."));
                    PropertyField(depthOfFieldForegroundBlurHQ, new GUIContent("   High Quality", "Improves depth of field foreground blur."));
                }
                PropertyField(depthOfFieldMaxBrightness, new GUIContent("Clamp Brightness", "Clamps maximum pixel brightness to prevent out of range bright spots."));
                PropertyField(depthOfFieldMaxDistance, new GUIContent("Max Distance", "DoF discarded beyond this distance."));
                PropertyField(depthOfFieldBokeh, new GUIContent("Bokeh", "Bright spots will be augmented in defocused areas."));
                if (depthOfFieldBokeh.value.boolValue) {
                    PropertyField(depthOfFieldBokehThreshold, new GUIContent("   Threshold", "Brightness threshold to be considered as 'bright' spot."));
                    PropertyField(depthOfFieldBokehIntensity, new GUIContent("   Intensity", "Intensity multiplier for bright spots in defocused areas."));
                }
                PropertyField(depthOfFieldDownsampling, new GUIContent("Downsampling", "Reduces screen buffer size to improve performance."));
                EditorGUILayout.BeginHorizontal();
                PropertyField(depthOfFieldMaxSamples, new GUIContent("Sample Count", "Determines the maximum number of samples to be gathered in the effect."));
                GUILayout.Label("(" + ((depthOfFieldMaxSamples.value.intValue - 1) * 2 + 1) + " samples)", GUILayout.Width(80));
                EditorGUILayout.EndHorizontal();

                //PropertyField(depthOfFieldExclusionLayerMask, new GUIContent("   Exclusion Mask", "Select which layers will always remain in focus."));
                //if (depthOfFieldExclusionLayerMask.value.intValue != 0) {
                //    PropertyField(depthOfFieldExclusionBias, new GUIContent("       Depth Bias", "Depth offset for the exclusion mask computation."));
                //    PropertyField(depthOfFieldExclusionLayerMaskDownsampling, new GUIContent("       Downsampling", "This value is added to the DoF downsampling factor for the exclusion mask creation. Increase to improve performance. "));
                //    PropertyField(depthOfFieldFilterMode, new GUIContent("       Filter Mode", "Texture filter mode."));
                //}
                //if (depthOfFieldExclusionLayerMask.value.intValue != 0) {
                //    EditorGUILayout.LabelField(new GUIContent("   Transparency", "Enable transparency support."), new GUIContent("Enabled due to Excusion Mask"));
                //} else {
                //    PropertyField(depthOfFieldTransparencySupport, new GUIContent("   Transparency", "Enable transparency support."));
                //}

                //if (depthOfFieldTransparencySupport.value.boolValue || depthOfFieldExclusionLayerMask.value.intValue != 0) {
                //    PropertyField(depthOfFieldTransparencySupportDownsampling, new GUIContent("       Downsampling", "This value is added to the DoF downsampling factor for the transparency mask creation. Increase to improve performance."));
                //}
                //if (depthOfFieldTransparencySupport.value.boolValue) {
                //    PropertyField(depthOfFieldTransparencyLayerMask, new GUIContent("       Layer Mask", "Objects to include in the transparent queue."));
                //}
            }

            expandSection[(int)Section.EyeAdaptation] = EditorGUILayout.Foldout(expandSection[(int)Section.EyeAdaptation], sectionNames[(int)Section.EyeAdaptation], sectionHeaderStyle);
            if (expandSection[(int)Section.EyeAdaptation]) {
                PropertyField(eyeAdaptation, new GUIContent("Enable", "Enables eye adaptation effect. Simulates retina response to quick luminance changes in the scene."));
                PropertyField(eyeAdaptationMinExposure, new GUIContent("Min Exposure"));
                PropertyField(eyeAdaptationMaxExposure, new GUIContent("Max Exposure"));
                PropertyField(eyeAdaptationSpeedToDark, new GUIContent("Dark Adapt Speed"));
                PropertyField(eyeAdaptationSpeedToLight, new GUIContent("Light Adapt Speed"));
                EditorGUILayout.Separator();
            }

            expandSection[(int)Section.Purkinje] = EditorGUILayout.Foldout(expandSection[(int)Section.Purkinje], sectionNames[(int)Section.Purkinje], sectionHeaderStyle);
            if (expandSection[(int)Section.Purkinje]) {
                PropertyField(purkinje, new GUIContent("Enable", "Simulates achromatic vision plus spectrum shift to blue in the dark."));
                PropertyField(purkinjeAmount, new GUIContent("Shift Amount", "Spectrum shift to blue. A value of zero will not shift colors and stay in grayscale."));
                PropertyField(purkinjeLuminanceThreshold, new GUIContent("Threshold", "Increase this value to augment the purkinje effect (applies to higher luminance levels)."));
                EditorGUILayout.Separator();
            }

            expandSection[(int)Section.Outline] = EditorGUILayout.Foldout(expandSection[(int)Section.Outline], sectionNames[(int)Section.Outline], sectionHeaderStyle);
            if (expandSection[(int)Section.Outline]) {
                PropertyField(outline, new GUIContent("Outline", "Enables outline effect"));
                PropertyField(outlineColor, new GUIContent("Color", "Color (RGB) and Threshold (A) of outline effect"));
            }

            expandSection[(int)Section.Vignetting] = EditorGUILayout.Foldout(expandSection[(int)Section.Vignetting], sectionNames[(int)Section.Vignetting], sectionHeaderStyle);
            if (expandSection[(int)Section.Vignetting]) {
                PropertyField(vignettingOuterRing, new GUIContent("Outer Ring", "Size of vignetting effect."));
                PropertyField(vignettingInnerRing, new GUIContent("Inner Ring", "Controls the gradient of the vignette by adjusting the size of the inner ring."));
                PropertyField(vignettingFade, new GUIContent("Fade Out", "Fade out effect to the vignette color."));
                PropertyField(vignettingBlink, new GUIContent("Blink", "Manages blink effect for testing purposes. Use Beautify.instance.Blink to quickly produce a blink effect."));
                PropertyField(vignettingCircularShape, new GUIContent("Circular Shape", "Ignores screen aspect ratio showing a circular shape."));
                if (!vignettingCircularShape.value.boolValue) {
                    PropertyField(vignettingAspectRatio, new GUIContent("Aspect Ratio", "Custom aspect ratio."));
                }
                PropertyField(vignettingColor, new GUIContent("Color", "The color for the vignetting effect."));
                PropertyField(vignettingMask, new GUIContent("Mask Texture", "Texture used for masking vignette effect. Alpha channel will be used to determine which areas remain untouched (1=color untouched, less than 1=vignette effect)"));
                EditorGUILayout.Separator();
            }
        }

        void LocateLegacyShaders() {
            Shader shader = Shader.Find("Beautify/Beautify");
            if (shader == null) return;
            Selection.activeObject = shader;
            EditorGUIUtility.PingObject(shader);
        }

        #region LUT settings

        public static void CheckLUTSettings(Texture2D tex) {
            if (Application.isPlaying || tex == null) return;
            string path = AssetDatabase.GetAssetPath(tex);
            if (string.IsNullOrEmpty(path)) return;
            TextureImporter imp = (TextureImporter)TextureImporter.GetAtPath(path);
            if (imp == null) return;
            if (imp.textureType != TextureImporterType.Default || imp.sRGBTexture || imp.mipmapEnabled || imp.textureCompression != TextureImporterCompression.Uncompressed || imp.wrapMode != TextureWrapMode.Clamp || imp.filterMode != FilterMode.Bilinear) {
                EditorGUILayout.HelpBox("Texture has invalid import settings.", MessageType.Warning);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Fix Texture Import Settings", GUILayout.Width(200))) {
                    imp.textureType = TextureImporterType.Default;
                    imp.sRGBTexture = false;
                    imp.mipmapEnabled = false;
                    imp.textureCompression = TextureImporterCompression.Uncompressed;
                    imp.wrapMode = TextureWrapMode.Clamp;
                    imp.filterMode = FilterMode.Bilinear;
                    imp.anisoLevel = 0;
                    imp.SaveAndReimport();
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }
        #endregion


    }

}
