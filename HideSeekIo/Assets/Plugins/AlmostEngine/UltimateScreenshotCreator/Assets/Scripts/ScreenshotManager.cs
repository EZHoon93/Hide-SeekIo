using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AlmostEngine.Screenshot
{
    /// <summary>
    /// The ScreenshotManager manages the capture process using a ScreenshotConfig.
    /// It also handles the hotkeys, burst mode, and the preview specific features like guides.
    /// </summary>
    /// 
    [ExecuteInEditMode]
    [RequireComponent(typeof(AudioSource))]
    public class ScreenshotManager : MonoBehaviour
    {
        public ScreenshotConfig m_Config = new ScreenshotConfig();
        protected ScreenshotTaker m_ScreenshotTaker;

        #region CAPTURE PROCESS

        public bool m_IsBurstActive = false;
        public bool m_IsCapturing = false;

        #endregion


        #region DELEGATES

        public static UnityAction onCaptureBeginDelegate = () =>
        {
        };
        public static UnityAction onCaptureEndDelegate = () =>
        {
        };

        public delegate void ExportDelegate(ScreenshotResolution res);

        /// <summary>
        /// Delegate called when the capture is a success.
        /// </summary>
        public static ExportDelegate onResolutionExportSuccessDelegate = (ScreenshotResolution res) =>
        {
        };
        /// <summary>
        /// Delegate called when the capture is a failure.
        /// </summary>
        public static ExportDelegate onResolutionExportFailureDelegate = (ScreenshotResolution res) =>
        {
        };

        #endregion


        #region BEHAVIOR METHODS

        public void Awake()
        {
            Reset();
            ClearCache();

            if (m_Config.m_DontDestroyOnLoad && Application.isPlaying)
            {
                DontDestroyOnLoad(this.gameObject);
            }

            // Load settings in ingame preview mode
            if (Application.isPlaying && m_Config.m_PreviewInGameViewWhilePlaying == true)
            {
                InitIngamePreview();
            }

        }

        void OnDestroy()
        {
#if UNITY_EDITOR
#if UNITY_2019_1_OR_NEWER
				SceneView.duringSceneGui -= HandleEventsDelegate;
#else
            SceneView.onSceneGUIDelegate -= HandleEventsDelegate;
#endif
#endif
        }

        public void Reset()
        {
            GameObject.DestroyImmediate(m_CurrentComposerInstance);
            StopAllCoroutines();

            m_IsCapturing = false;
            m_IsBurstActive = false;

            InitScreenshotTaker();
        }

        public void ClearCache()
        {
            m_Config.ClearCache();
            if (m_ScreenshotTaker != null)
            {
                m_ScreenshotTaker.ClearRenderTextureCache();
            }
        }

        void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && EditorApplication.isCompiling)
            {
                Reset();
                ClearCache();
                return;
            }
#endif

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                RegisterUpdate();
            }
#endif

            if (Application.isPlaying)
            {
                HandleHotkeys();
            }
        }

        protected void InitScreenshotTaker()
        {
            if (m_ScreenshotTaker == null)
            {
                m_ScreenshotTaker = GameObject.FindObjectOfType<ScreenshotTaker>();
            }
            if (m_ScreenshotTaker == null)
            {
                m_ScreenshotTaker = gameObject.GetComponent<ScreenshotTaker>();
            }
            if (m_ScreenshotTaker == null)
            {
                m_ScreenshotTaker = gameObject.AddComponent<ScreenshotTaker>();
            }

            m_ScreenshotTaker.m_GameViewResizingWaitingMode = m_Config.m_GameViewResizingWaitingMode;
            m_ScreenshotTaker.m_GameViewResizingWaitingFrames = m_Config.m_ResizingWaitingFrames;
            m_ScreenshotTaker.m_GameViewResizingWaitingTime = m_Config.m_ResizingWaitingTime;

        }

        protected void HandleHotkeys()
        {
            if (m_Config.m_AlignHotkey.IsPressed())
            {
                m_Config.AlignToView();
            }

            if (m_Config.m_PauseHotkey.IsPressed())
            {
                m_Config.TogglePause();
            }

            if (m_Config.m_UpdatePreviewHotkey.IsPressed())
            {
                UpdatePreview();
            }

            if (m_Config.m_CaptureHotkey.IsPressed())
            {
                if (m_IsBurstActive)
                {
                    StopBurst();
                }
                else
                {
                    Capture();
                }
            }
        }

#if UNITY_EDITOR
        protected void RegisterUpdate()
        {
#if UNITY_EDITOR
#if UNITY_2019_1_OR_NEWER
					SceneView.duringSceneGui -= HandleEventsDelegate;
					SceneView.duringSceneGui += HandleEventsDelegate;
#else
            SceneView.onSceneGUIDelegate -= HandleEventsDelegate;
            SceneView.onSceneGUIDelegate += HandleEventsDelegate;
#endif
#endif

        }

        protected void HandleEventsDelegate(SceneView sceneview)
        {
            HandleEditorHotkeys();
        }

        public void HandleEditorHotkeys()
        {
            Event e = Event.current;
            if (m_Config.m_UpdatePreviewHotkey.IsPressed(e))
            {
                UpdatePreview();
                e.Use();
            }
            if (m_Config.m_CaptureHotkey.IsPressed(e))
            {
                if (m_IsBurstActive)
                {
                    StopBurst();
                }
                else
                {
                    Capture();
                }
                e.Use();
            }
            if (m_Config.m_PauseHotkey.IsPressed(e))
            {
                m_Config.TogglePause();
                e.Use();
            }

            if (m_Config.m_AlignHotkey.IsPressed(e))
            {
                m_Config.AlignToView();
                e.Use();
            }
        }

#endif

        #endregion

        #region CAPTURE

        /// <summary>
        /// Captures the active resolutions.
        /// </summary>
        public void Capture()
        {
            StartCoroutine(CaptureAllCoroutine());

        }

        /// <summary>
        /// Updates all active resolutions.
        /// </summary>
        public void UpdateAll()
        {
            StartCoroutine(UpdateAllCoroutine());
        }

        /// <summary>
        /// Updates the resolutions.
        /// </summary>
        public void UpdateResolutions(List<ScreenshotResolution> resolutions)
        {
            StartCoroutine(CaptureCoroutine(resolutions, false, false));
        }

        public IEnumerator UpdateAllCoroutine()
        {
            // Get resolutions to capture
            List<ScreenshotResolution> resolutions = m_Config.GetActiveResolutions();
            m_Config.UpdateGameviewResolution();

            // Capture the resolutions
            yield return StartCoroutine(CaptureCoroutine(resolutions, false, false));
        }

        public IEnumerator CaptureAllCoroutine()
        {
            // Get resolutions to capture
            List<ScreenshotResolution> resolutions = m_Config.GetActiveResolutions();
            m_Config.UpdateGameviewResolution();

            // Capture the resolutions
            yield return StartCoroutine(CaptureCoroutine(resolutions));
        }

        public Texture2D GetLastScreenshotTexture()
        {
            var res = m_Config.GetFirstActiveResolution();
            if (res != null)
                return res.m_Texture;
            return null;
        }

        public IEnumerator CaptureCoroutine(List<ScreenshotResolution> resolutions, bool export = true, bool playSoundMask = true)
        {

            // Burst
            if (m_Config.m_ShotMode == ScreenshotConfig.ShotMode.BURST && !Application.isPlaying)
            {
                Debug.LogError("In burst mode the application needs to be playing.");
                yield break;
            }

            // Prevent multiple capture process		
            if (m_IsCapturing == true)
            {
                Debug.LogError("A capture process is already running.");
                yield break;
            }

            // We set capturing to true to prevent conflicts
            m_IsCapturing = true;

            // Hide guides if in-game preview
            if (Application.isPlaying && m_Config.m_PreviewInGameViewWhilePlaying && m_Config.m_ShowGuidesInPreview)
            {
                HideGuides();
            }

            // Notify capture start
            onCaptureBeginDelegate();

            // Capture
            if (m_Config.m_ShotMode == ScreenshotConfig.ShotMode.ONE_SHOT)
            {
                yield return StartCoroutine(UpdateCoroutine(resolutions, m_Config.GetActiveCameras(), m_Config.m_Overlays, export, playSoundMask));

            }
            else if (m_Config.m_ShotMode == ScreenshotConfig.ShotMode.BURST)
            {
                m_IsBurstActive = true;

                // Capture sequence
                for (int i = 0; i < m_Config.m_MaxBurstShotsNumber && m_IsBurstActive; ++i)
                {

                    yield return StartCoroutine(UpdateCoroutine(resolutions, m_Config.GetActiveCameras(), m_Config.m_Overlays, export, playSoundMask));

                    yield return new WaitForSeconds(1f / (float)m_Config.m_ShotPerSeconds);
                }

                m_IsBurstActive = false;
            }

            // Notify capture end
            onCaptureEndDelegate();

            //Restore guides if in-game preview
            if (Application.isPlaying && m_Config.m_PreviewInGameViewWhilePlaying && m_Config.m_ShowGuidesInPreview)
            {
                ShowGuides();
            }
            else
            {
                HideGuides();
            }

#if UNITY_EDITOR
            // Refresh the gameview to trigger a paint event
            SceneView.RepaintAll();
#endif

            // Liberate the token
            m_IsCapturing = false;

        }

        public void StopBurst()
        {
            // Set m_IsBurstActive to false so its coroutine loop will end
            m_IsBurstActive = false;
        }

        ScreenshotComposer m_CurrentComposerInstance = null;




        public ScreenshotBatch m_CurrentBatch = null;
        public string m_CurrentBatchName = "";
        protected IEnumerator UpdateCoroutine(List<ScreenshotResolution> resolutions, List<ScreenshotCamera> cameras, List<ScreenshotOverlay> overlays, bool export = true, bool playSoundMask = true, bool isPreview = false)
        {
            InitScreenshotTaker();
            System.DateTime startTime = System.DateTime.Now;

            // BATCHES
            var batches = m_Config.GetActiveBatches();
            for (int b = 0; (m_Config.m_BatchMode == ScreenshotConfig.BatchMode.BATCHES && b < batches.Count) || b == 0; ++b)
            {

                // Get batches if any
                m_CurrentBatch = null;
                if (m_Config.m_BatchMode == ScreenshotConfig.BatchMode.BATCHES && batches.Count > 0)
                {
                    m_CurrentBatch = batches[b];
                }
                m_CurrentBatchName = m_CurrentBatch == null ? "" : m_CurrentBatch.m_Name;

                // Get the resolutions to be captured, if overwritten by batches
                List<ScreenshotResolution> resToCapture = new List<ScreenshotResolution>();
                if (!isPreview && m_CurrentBatch != null && m_CurrentBatch.m_OverrideActiveResolutions)
                {
                    resToCapture.AddRange(m_CurrentBatch.m_ActiveResolutions.Where(x => x.m_Active == true).Select(x => m_Config.m_Resolutions[x.m_Id]).ToList());
                }
                else
                {
                    resToCapture.AddRange(resolutions);
                }

                // Get the composition to be captured, if overwritten by batches
                List<ScreenshotComposition> compositionsToCapture = new List<ScreenshotComposition>();
                if (m_CurrentBatch != null && m_CurrentBatch.m_OverrideActiveComposer)
                {
                    compositionsToCapture.AddRange(m_CurrentBatch.m_ActiveCompositions.Where(x => x.m_Active == true).Select(x => m_Config.m_Compositions[x.m_Id]).ToList());
                }
                else
                {
                    compositionsToCapture.AddRange(m_Config.GetActiveCompositions());
                }


                // COMPOSERS
                // Capture each composition
                for (int c = 0; (m_Config.m_CompositionMode == ScreenshotConfig.CompositionMode.COMPOSITION && c < compositionsToCapture.Count) || c == 0; ++c)
                {

                    // Get composer if any
                    ScreenshotComposition currentComposition = null;
                    m_CurrentComposerInstance = null;
                    if (m_Config.m_CompositionMode == ScreenshotConfig.CompositionMode.COMPOSITION && compositionsToCapture.Count > 0 && compositionsToCapture[c].m_Composer != null)
                    {
                        m_CurrentComposerInstance = GameObject.Instantiate<ScreenshotComposer>(compositionsToCapture[c].m_Composer);
                        m_CurrentComposerInstance.hideFlags = HideFlags.DontSave;
                        currentComposition = compositionsToCapture[c];
                    }


                    // RESOLUTIONS
                    // Capture each resolution
                    foreach (ScreenshotResolution resolution in resToCapture)
                    {

                        // LAYERS
                        for (int l = 0; (m_Config.m_ExportToDifferentLayers && l < cameras.Count) || l == 0; ++l)
                        {

                            // Get layer if any
                            ScreenshotCamera separatedLayerCamera = null;
                            List<ScreenshotCamera> currentCameras;

                            if (m_Config.m_ExportToDifferentLayers && cameras.Count > 1)
                            {
                                // Capture only the current layer camera
                                separatedLayerCamera = cameras[l];
                                currentCameras = new List<ScreenshotCamera> { separatedLayerCamera };
                            }
                            else
                            {
                                // Capture all camera
                                currentCameras = new List<ScreenshotCamera>(cameras);
                            }

                            // PRE PROCESS
                            if (m_CurrentBatch != null)
                            {
                                foreach (ScreenshotProcess p in m_CurrentBatch.m_PreProcess)
                                {
                                    if (p == null)
                                        continue;
                                    p.m_Batch = m_CurrentBatch;
                                    p.Process(resolution);
                                    yield return StartCoroutine(p.ProcessCoroutine(resolution));
                                }
                            }

                            // Sound
                            if (m_Config.m_PlaySoundOnCapture && playSoundMask)
                            {
                                PlaySound();
                            }

                            // CAPTURE
                            if (m_CurrentComposerInstance == null)
                            {

                                yield return StartCoroutine(m_ScreenshotTaker.CaptureAllCoroutine(new List<ScreenshotResolution> { resolution },
                                    currentCameras,
                                    overlays,
                                    (m_Config.m_CaptureMode == ScreenshotTaker.CaptureMode.GAMEVIEW_RESIZING && m_Config.m_ResolutionCaptureMode == ScreenshotConfig.ResolutionMode.GAME_VIEW) ? ScreenshotTaker.CaptureMode.FIXED_GAMEVIEW : m_Config.m_CaptureMode,
                                    (int)m_Config.m_MultisamplingAntiAliasing,
                                    m_Config.m_CaptureActiveUICanvas,
                                    m_Config.m_ColorFormat,
                                    m_Config.m_RecomputeAlphaLayer,
                                    m_Config.m_StopTimeOnCapture,
                                    true,
                                    m_Config.m_ForceUICullingLayer));
                            }
                            else
                            {

                                yield return StartCoroutine(m_CurrentComposerInstance.CaptureCoroutine(resolution,
                                    currentCameras,
                                    overlays,
                                    (m_Config.m_CaptureMode == ScreenshotTaker.CaptureMode.GAMEVIEW_RESIZING && m_Config.m_ResolutionCaptureMode == ScreenshotConfig.ResolutionMode.GAME_VIEW) ? ScreenshotTaker.CaptureMode.FIXED_GAMEVIEW : m_Config.m_CaptureMode,
                                    (int)m_Config.m_MultisamplingAntiAliasing,
                                    m_Config.m_CaptureActiveUICanvas,
                                    m_Config.m_ColorFormat,
                                    m_Config.m_RecomputeAlphaLayer,
                                    m_Config.m_StopTimeOnCapture,
                                    true,
                                    m_Config.m_ForceUICullingLayer));

                            }


                            // POST PROCESS
                            if (m_CurrentBatch != null)
                            {
                                foreach (ScreenshotProcess p in m_CurrentBatch.m_PostProcess)
                                {
                                    if (p == null)
                                        continue;
                                    p.m_Batch = m_CurrentBatch;
                                    p.Process(resolution);
                                    yield return StartCoroutine(p.ProcessCoroutine(resolution));
                                }
                            }


                            // EXPORT
                            if (export)
                            {
                                string currentComposerName = currentComposition == null ? "" : currentComposition.m_Name;
                                string currentCameraName = separatedLayerCamera == null ? "" : separatedLayerCamera.m_Camera.name;

                                // Update the filenames with the camera name as layer name
                                UpdateFileName(resolution, startTime, currentCameraName, m_CurrentBatchName, currentComposerName);

                                // Export
                                if (TextureExporter.ExportToFile(resolution.m_Texture, resolution.m_FileName, m_Config.m_FileFormat, (int)m_Config.m_JPGQuality, m_Config.m_ExportToPhoneGallery))
                                {
                                    Debug.Log("Screenshot created : " + resolution.m_FileName);
                                    onResolutionExportSuccessDelegate(resolution);
                                }
                                else
                                {
                                    Debug.LogError("Failed to create : " + resolution.m_FileName);
                                    onResolutionExportFailureDelegate(resolution);
                                }
                            }



                        }
                    }

                    if (m_CurrentComposerInstance != null)
                    {
                        GameObject.DestroyImmediate(m_CurrentComposerInstance.gameObject);
                    }
                }
            }

        }

        public void UpdateFileName(ScreenshotResolution resolution, System.DateTime time, string currentCamera = "", string currentBatch = "", string currentComposer = "")
        {

            // Update the filenames
            m_Config.UpdateFileName(resolution, time);

            // Parse symbols etc.
            resolution.m_FileName = resolution.m_FileName.Replace("{layer}", currentCamera);
            resolution.m_FileName = resolution.m_FileName.Replace("{batch}", currentBatch);
            resolution.m_FileName = resolution.m_FileName.Replace("{composer}", currentComposer);

            if (!m_Config.m_OverwriteFiles)
            {
                resolution.m_FileName = PathUtils.PreventOverwrite(resolution.m_FileName);
            }
        }


        #endregion


        #region SOUND

        void PlaySound()
        {
            if (GetComponent<AudioSource>() == null)
                return;

            if (m_Config.m_ShotSound == null)
                return;

            GetComponent<AudioSource>().PlayOneShot(m_Config.m_ShotSound);
        }

        #endregion


        #region PREVIEW METHODS


        List<ScreenshotResolution> m_PreviewResolutions = new List<ScreenshotResolution>();
        List<ScreenshotOverlay> m_PreviewOverlayList = new List<ScreenshotOverlay>();
        ScreenshotOverlay m_GuidesOverlay;

        public virtual void UpdatePreview()
        {
            StartCoroutine(UpdatePreviewCoroutine());
        }

        /// <summary>
        /// Updates the preview coroutine, using the preview relative settings like guides.
        /// </summary>
        public IEnumerator UpdatePreviewCoroutine()
        {
            if (m_IsCapturing)
                yield break;

            m_IsCapturing = true;

            // Delegate call
            onCaptureBeginDelegate();

            // Update resolutions
            m_PreviewResolutions.Clear();
            m_PreviewResolutions.Add(m_Config.GetFirstActiveResolution());
            m_Config.UpdateGameviewResolution();

            // Update overlays & guides
            m_PreviewOverlayList.Clear();
            m_PreviewOverlayList.AddRange(m_Config.m_Overlays);
            if (m_Config.m_ShowGuidesInPreview)
            {
                m_GuidesOverlay = new ScreenshotOverlay(m_Config.m_GuideCanvas);
                m_PreviewOverlayList.Add(m_GuidesOverlay);
                ShowGuides();
            }

            // Capture preview
            yield return StartCoroutine(UpdateCoroutine(m_PreviewResolutions, m_Config.GetActiveCameras(), m_PreviewOverlayList, false, false, true));

            // Restore guides
            if (Application.isPlaying && m_Config.m_PreviewInGameViewWhilePlaying && m_Config.m_ShowGuidesInPreview)
            {
                ShowGuides();
            }
            else
            {
                HideGuides();
            }

            // Delegate call
            onCaptureEndDelegate();

            m_IsCapturing = false;

        }

        GameObject m_GuideInstance;

        protected void InitIngamePreview()
        {
            m_PreviewOverlayList.Clear();
            m_PreviewOverlayList.AddRange(m_Config.m_Overlays);
            if (m_Config.m_ShowGuidesInPreview)
            {
                m_GuideInstance = GameObject.Instantiate(m_Config.m_GuideCanvas.gameObject);
                m_GuideInstance.hideFlags = HideFlags.DontSave;
                m_GuideInstance.SetActive(true);
            }

            InitScreenshotTaker();
            m_ScreenshotTaker.ApplySettings(m_Config.GetActiveCameras(), m_PreviewOverlayList, m_Config.m_CaptureMode, m_Config.m_CaptureActiveUICanvas, m_Config.m_ForceUICullingLayer);
        }

        protected void ShowGuides()
        {
            if (m_Config.m_ShowGuidesInPreview && m_Config.m_GuideCanvas != null)
            {
                m_Config.m_GuideCanvas.gameObject.SetActive(true);
                m_Config.m_GuideCanvas.enabled = true;
                SetColorRecursive(m_Config.m_GuideCanvas, m_Config.m_GuidesColor);

                if (m_GuideInstance != null)
                {
                    m_GuideInstance.SetActive(true);
                    SetColorRecursive(m_GuideInstance.GetComponent<Canvas>(), m_Config.m_GuidesColor);
                }
            }
        }

        protected void SetColorRecursive(Canvas canvas, Color color)
        {
            Image[] images = canvas.GetComponentsInChildren<Image>();
            foreach (Image image in images)
            {
                image.color = color;
            }
        }

        protected void HideGuides()
        {
            if (m_Config.m_PreviewInGameViewWhilePlaying == true && Application.isPlaying && m_Config.m_ShowGuidesInPreview && !m_IsCapturing)
                return;

            if (m_Config.m_GuideCanvas != null)
            {
                m_Config.m_GuideCanvas.gameObject.SetActive(false);
            }

            if (m_GuideInstance != null)
            {
                m_GuideInstance.SetActive(false);
            }
        }

        #endregion


    }

}
