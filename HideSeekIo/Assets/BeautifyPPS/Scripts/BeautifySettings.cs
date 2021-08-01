using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace BeautifyForPPS {

    public delegate float OnBeforeFocusEvent(float currentFocusDistance);


    public class BeautifySettings : MonoBehaviour {

        [Header("Scene Settings")]
        public Transform sun;
        public Transform depthOfFieldTarget;

        [NonSerialized]
        public OnBeforeFocusEvent OnBeforeFocus;

        [NonSerialized]
        public static float depthOfFieldCurrentFocalPointDistance;


        #region API


        static BeautifySettings _instance;
        static PostProcessLayer _layer;
        static PostProcessVolume _volume;
        static Beautify _beautify;

        /// <summary>
        /// Returns a reference to the Beautify Settings component attached to the Post Processing Layer or camera
        /// </summary>
        /// <value>The instance.</value>
        public static BeautifySettings instance {
            get {
                if (_instance == null) {
                    PostProcessLayer pps = postProcessLayer;
                    if (pps == null) return null;
                    _instance = pps.GetComponent<BeautifySettings>();
                    if (_instance == null) {
                        _instance = pps.gameObject.AddComponent<BeautifySettings>();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Returns a reference to the Post Processing Layer
        /// </summary>
        /// <value>The post process layer.</value>
        public static PostProcessLayer postProcessLayer {
            get {
                if (_layer == null) {
                    _layer = FindObjectOfType<PostProcessLayer>();
                }
                return _layer;
            }
        }

        /// <summary>
        /// Returns a reference to the Post Processing Volume which contains Beautify effect.
        /// </summary>
        /// <value>The volume.</value>
        public static PostProcessVolume volume {
            get {
                if (_volume == null) {
                    _volume = FindObjectOfType<PostProcessVolume>();
                }
                return _volume;
            }
        }

        /// <summary>
        /// Returns a reference to the profile used by the Post Processing Volume
        /// </summary>
        /// <value>The shared profile.</value>
        public static PostProcessProfile sharedProfile {
            get {
                PostProcessVolume v = volume;
                if (v == null) return null;
                return v.sharedProfile;
            }
        }


        /// <summary>
        /// Returns a copy of the profile used by the Post Processing Volume
        /// </summary>
        /// <value>The profile.</value>
        public static PostProcessProfile profile {
            get {
                PostProcessVolume v = volume;
                if (v == null) return null;
                return v.profile;
            }
        }

        /// <summary>
        /// Returns a reference to the settings of Beautify in the Post Processing Profile
        /// </summary>
        /// <value>The shared settings.</value>
        public static Beautify sharedSettings {
            get {
                if (_beautify != null) return _beautify;
                PostProcessVolume v = volume;
                if (v == null) return null;

                bool foundEffectSettings = v.sharedProfile.TryGetSettings<Beautify>(out _beautify);
                if (!foundEffectSettings) {
                    Debug.Log("Cant load Beautify settings");
                    return null;
                }
                return _beautify;
            }
        }

        /// <summary>
        /// Returns a copy of the settings of Beautify in the Post Processing Profile
        /// </summary>
        /// <value>The settings.</value>
        public static Beautify settings {
            get {
                if (_beautify != null) return _beautify;
                PostProcessVolume v = volume;
                if (v == null) return null;

                bool foundEffectSettings = v.profile.TryGetSettings<Beautify>(out _beautify);
                if (!foundEffectSettings) {
                    Debug.Log("Cant load Beautify settings");
                    return null;
                }
                return _beautify;
            }
        }


        /// <summary>
        /// Animates blink parameter
        /// </summary>
        /// <returns>The blink.</returns>
        /// <param name="duration">Duration.</param>
        public void Blink(float duration, float maxValue = 1) {
            if (duration <= 0)
                return;
            StartCoroutine(DoBlink(duration, maxValue));
        }

        IEnumerator DoBlink(float duration, float maxValue) {

            float start = Time.time;
            float t = 0;
            WaitForEndOfFrame w = new WaitForEndOfFrame();
            Beautify theSettings = sharedSettings;
            theSettings.vignettingBlink.overrideState = true;
            // Close
            do {
                t = (Time.time - start) / duration;
                if (t > 1f)
                    t = 1f;
                float easeOut = t * (2f - t);
                theSettings.vignettingBlink.value = easeOut * maxValue;
                yield return w;
            } while (t < 1f);

            // Open
            start = Time.time;
            do {
                t = (Time.time - start) / duration;
                if (t > 1f)
                    t = 1f;
                float easeIn = t * t;
                theSettings.vignettingBlink.value = (1f - easeIn) * maxValue;
                yield return w;
            } while (t < 1f);
            theSettings.vignettingBlink.overrideState = false;
        }

        #endregion


    }

}
