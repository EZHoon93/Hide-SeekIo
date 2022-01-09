using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace AlmostEngine.Screenshot
{
    /// <summary>
    /// The Screenshot taker is a component used to capture screenshots, with various capture modes and custom settings.
    /// </summary>
    [ExecuteInEditMode]
    public class ScreenshotTaker : MonoBehaviour
    {
        public enum ColorFormat
        {
            RGB,
            RGBA
        }
        ;

        public enum CaptureMode
        {
            GAMEVIEW_RESIZING,
            RENDER_TO_TEXTURE,
            FIXED_GAMEVIEW
        }
        ;
        public enum GameViewResizingWaitingMode
        {
            FRAMES,
            TIME
        }
        ;


        /// <summary>
        /// The texture containing the last screenshot taken.
        /// </summary>
        public Texture2D m_Texture;


        [HideInInspector]
        public GameViewResizingWaitingMode m_GameViewResizingWaitingMode;
        [HideInInspector]
        public float m_GameViewResizingWaitingTime = 1f;
        [HideInInspector]
        public int m_GameViewResizingWaitingFrames = 2;




#if (UNITY_EDITOR) && (!UNITY_5_4_OR_NEWER)
		[HideInInspector]
		public bool m_NeedRestoreLayout;

		[Tooltip ("If true, the editor layout is saved and restored before and after each capture process.")]
		public bool m_ForceLayoutPreservation = true;
#endif

        [HideInInspector]
        public static bool m_IsRunning = false;
        List<ScreenshotCamera> m_Cameras = new List<ScreenshotCamera>();
        List<ScreenshotCamera> m_SceneCameras = new List<ScreenshotCamera>();
        List<ScreenshotOverlay> m_Overlays = new List<ScreenshotOverlay>();
        List<ScreenshotOverlay> m_DisabledOverlays = new List<ScreenshotOverlay>();


        #region BEHAVIOR

        void Update()
        {
#if UNITY_EDITOR
            if (EditorApplication.isCompiling)
            {
                Reset();
                return;
            }
#endif
        }

        public void Reset()
        {
            StopAllCoroutines();

            RestoreTime();
            RestoreSettings();

            m_IsRunning = false;
        }

        void OnDestroy()
        {
            Reset();
        }

        #endregion

        #region CACHE

        // public bool m_UseRenderTextureCache = true;

        /// <summary>
        /// Clears the cache of RenderTexture used to capture the screenshots.
        /// </summary>
        public void ClearRenderTextureCache()
        {
            m_RenderTextureCache.Clear();
        }

        #endregion

        #region EVENTS

        /// <summary>
        /// Delegate called when the capture starts.
        /// </summary>
        public static UnityAction<ScreenshotResolution> onResolutionUpdateStartDelegate = (ScreenshotResolution res) =>
        {
        };

        /// <summary>
        /// Delegate called when the capture ends.
        /// </summary>
        public static UnityAction<ScreenshotResolution> onResolutionUpdateEndDelegate = (ScreenshotResolution res) =>
        {
        };

        /// <summary>
        /// Delegate called when the screen is resized.
        /// </summary>
        public static UnityAction<ScreenshotResolution> onResolutionScreenResizedDelegate = (ScreenshotResolution res) =>
        {
        };

        #endregion

        #region STATIC_API

        /// <summary>
        /// Captures the current screen at its current resolution.
        /// Must be called from a coroutine after a WaitForEndOfFrame.
        /// </summary>
        public static Texture2D CaptureScreenToTexture(ScreenshotTaker.ColorFormat colorFormat = ScreenshotTaker.ColorFormat.RGB)
        {
            int width = Screen.width;
            int height = Screen.height;
            Texture2D texture = null;
            GetOrCreateTexture(ref texture, width, height, colorFormat);
            CaptureScreenToTexture(texture);
            return texture;
        }

        public static Texture2D CaptureCamerasToTexture(int width, int height, List<Camera> cameras, ScreenshotTaker.ColorFormat colorFormat = ScreenshotTaker.ColorFormat.RGB)
        {
            Texture2D texture = null;
            GetOrCreateTexture(ref texture, width, height, colorFormat);
            RenderTexture renderTexture = new RenderTexture(width, height, 24);
            CaptureCamerasToTexture(cameras, texture, renderTexture);
            return texture;
        }


        #endregion


        #region COROUTINES_API 


        /// <summary>
        /// Captures the current screen at its current resolution.
        /// The texture will be resized if needed to match the capture settings.
        /// </summary>
        public IEnumerator CaptureScreenToTextureCoroutine(Texture2D texture,
                                                            bool captureGameUI = true,
                                                            ScreenshotTaker.ColorFormat colorFormat = ScreenshotTaker.ColorFormat.RGB,
                                                            bool recomputeAlphaMask = false)
        {
            Vector2 size = GameViewController.GetCurrentGameViewSize();
            yield return StartCoroutine(CaptureToTextureCoroutine(texture, (int)size.x, (int)size.y, null, null,
                ScreenshotTaker.CaptureMode.FIXED_GAMEVIEW,
                8, captureGameUI, colorFormat, recomputeAlphaMask));
        }

        /// <summary>
        /// Captures the scene with the specified width, height, using the mode RENDER_TO_TEXTURE.
        /// Screenspace Overlay Canvas can not be captured with this mode.
        /// The texture will be resized if needed to match the capture settings.
        /// </summary>
        public IEnumerator CaptureCamerasToTextureCoroutine(Texture2D texture, int width, int height,
                                                             List<Camera> cameras,
                                                             int antiAliasing = 8,
                                                             ScreenshotTaker.ColorFormat colorFormat = ScreenshotTaker.ColorFormat.RGB,
                                                             bool recomputeAlphaMask = false)
        {
            yield return StartCoroutine(CaptureToTextureCoroutine(texture, width, height, cameras, null, ScreenshotTaker.CaptureMode.RENDER_TO_TEXTURE, antiAliasing, true, colorFormat, recomputeAlphaMask));
        }

        /// <summary>
        /// Captures the game with the specified width, height.
        /// The texture will be resized if needed to match the capture settings.
        /// </summary>
        public IEnumerator CaptureToTextureCoroutine(Texture2D texture, int width, int height,
                                                      List<Camera> cameras = null,
                                                      List<Canvas> canvas = null,
                                                      ScreenshotTaker.CaptureMode captureMode = ScreenshotTaker.CaptureMode.RENDER_TO_TEXTURE,
                                                      int antiAliasing = 8,
                                                      bool captureGameUI = true,
                                                      ScreenshotTaker.ColorFormat colorFormat = ScreenshotTaker.ColorFormat.RGB,
                                                      bool recomputeAlphaMask = false)
        {
            // Check texture
            if (texture == null)
            {
                Debug.LogError("The texture can not be null. You must provide a texture initialized with any width and height.");
                yield break;
            }

            // Update resolution item
            ScreenshotResolution captureResolution = new ScreenshotResolution(width, height);
            captureResolution.m_Texture = texture;

            yield return StartCoroutine(CaptureResolutionCoroutine(captureResolution, cameras, canvas, captureMode, antiAliasing, captureGameUI, colorFormat, recomputeAlphaMask));

        }


        public IEnumerator CaptureResolutionCoroutine(ScreenshotResolution captureResolution,
                                                       List<Camera> cameras = null,
                                                       List<Canvas> canvas = null,
                                                       ScreenshotTaker.CaptureMode captureMode = ScreenshotTaker.CaptureMode.RENDER_TO_TEXTURE,
                                                       int antiAliasing = 8,
                                                       bool captureGameUI = true,
                                                       ScreenshotTaker.ColorFormat colorFormat = ScreenshotTaker.ColorFormat.RGB,
                                                       bool recomputeAlphaMask = false)
        {

            // Create camera items
            List<ScreenshotCamera> screenshotCameras = new List<ScreenshotCamera>();
            if (cameras != null)
            {
                foreach (Camera camera in cameras)
                {
                    ScreenshotCamera scamera = new ScreenshotCamera(camera);
                    screenshotCameras.Add(scamera);
                }
            }

            // Create the overlays items
            List<ScreenshotOverlay> screenshotCanvas = new List<ScreenshotOverlay>();
            if (canvas != null)
            {
                foreach (Canvas c in canvas)
                {
                    ScreenshotOverlay scanvas = new ScreenshotOverlay(c);
                    screenshotCanvas.Add(scanvas);
                }
            }

            yield return StartCoroutine(CaptureAllCoroutine(new List<ScreenshotResolution> { captureResolution },
                screenshotCameras, screenshotCanvas,
                captureMode, antiAliasing, captureGameUI, colorFormat, recomputeAlphaMask));

        }


        #endregion


        #region Capture



        public IEnumerator CaptureAllCoroutine(List<ScreenshotResolution> resolutions,
                                                List<ScreenshotCamera> cameras,
                                                List<ScreenshotOverlay> overlays,
                                                CaptureMode captureMode,
                                                int antiAliasing = 8,
                                                bool captureGameUI = true,
                                                ColorFormat colorFormat = ColorFormat.RGB,
                                                bool recomputeAlphaMask = false,
                                                bool stopTime = false,
                                                bool restore = true,
                                                bool forceUICullingLayer = false)
        {

            //			Debug.Log ("Capture all");

            if (resolutions == null)
            {
                Debug.LogError("Resolution list is null.");
                yield break;
            }
            if (cameras == null)
            {
                Debug.LogError("Cameras list is null.");
                yield break;
            }
            if (cameras.Count == 0 && captureMode == CaptureMode.RENDER_TO_TEXTURE)
            {
                cameras.Add(new ScreenshotCamera(Camera.main));
            }
            if (overlays == null)
            {
                Debug.LogError("Overlays list is null.");
                yield break;
            }
            if (captureMode == CaptureMode.RENDER_TO_TEXTURE && !UnityVersion.HasPro())
            {
                Debug.LogError("RENDER_TO_TEXTURE requires Unity Pro or Unity 5.0 and later.");
                yield break;
            }

            // If a capture is in progress we wait until we can take the screenshot
            if (m_IsRunning == true)
            {
                Debug.LogWarning("A capture process is already running.");
            }
            while (m_IsRunning == true)
            {
                yield return null;
            }


#if (!UNITY_EDITOR && !UNITY_STANDALONE_WIN)
						if (captureMode == CaptureMode.GAMEVIEW_RESIZING) {
								Debug.LogError ("GAMEVIEW_RESIZING capture mode is only available for Editor and Windows Standalone.");
								yield break;
						}
#endif

            // Init
            m_IsRunning = true;

            // Stop the time so all screenshots are exactly the same
            if (Application.isPlaying && stopTime)
            {
                StopTime();
            }


            // Apply settings: enable and disable the cameras and canvas
            ApplySettings(cameras, overlays, captureMode, captureGameUI, forceUICullingLayer);


            // Save the screen config to be restored after the capture process
            if (captureMode == CaptureMode.GAMEVIEW_RESIZING)
            {
                GameViewController.SaveCurrentGameViewSize();

#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
								yield return null;
								yield return new WaitForEndOfFrame();
#endif
            }

            // Capture all resolutions
            foreach (ScreenshotResolution resolution in resolutions)
            {
                if (!resolution.IsValid())
                    continue;

                // Delegate call
                onResolutionUpdateStartDelegate(resolution);

                if (colorFormat == ColorFormat.RGBA && recomputeAlphaMask)
                {
                    yield return StartCoroutine(CaptureAlphaMaskCoroutine(resolution, captureMode, antiAliasing, colorFormat));
                }
                else
                {
                    yield return StartCoroutine(CaptureResolutionTextureCoroutine(resolution, captureMode, antiAliasing, colorFormat));
                }

                // Delegate call
                onResolutionUpdateEndDelegate(resolution);




#if UNITY_EDITOR
                // Dirty hack: we force a gameview repaint, to prevent the coroutine to stay locked.
                if (!Application.isPlaying)
                {
                    GameViewUtils.GetGameView().Repaint();
                }
#endif


            }

            // Restore screen config
            if (restore && captureMode == CaptureMode.GAMEVIEW_RESIZING)
            {
                GameViewController.RestoreGameViewSize();
            }


#if (UNITY_EDITOR && !UNITY_5_4_OR_NEWER)
			// Call restore layout for old unity versions
			if (restore && captureMode == CaptureMode.GAMEVIEW_RESIZING) {		
				m_NeedRestoreLayout = true;
			} 
#endif

#if UNITY_EDITOR
            // Dirty hack, try to force an editor Update() to get the gameview back to normal
            if (!Application.isPlaying)
            {
                GameViewUtils.GetGameView().Repaint();
            }
#endif

            // Restore everything
            if (Application.isPlaying && stopTime)
            {
                RestoreTime();
            }
            if (Application.isEditor || restore)
            {
                RestoreSettings();
            }

            // End
            m_IsRunning = false;
        }


        /// <summary>
        /// Captures the resolution texture.
        /// </summary>
        IEnumerator CaptureResolutionTextureCoroutine(ScreenshotResolution resolution, CaptureMode captureMode, int antiAliasing, ColorFormat colorFormat)
        {


            // Init texture
            m_Texture = GetOrCreateTexture(resolution, colorFormat, captureMode == CaptureMode.FIXED_GAMEVIEW ? true : false);

            if (captureMode == CaptureMode.GAMEVIEW_RESIZING)
            {

                // Force screen size change
                GameViewController.SetGameViewSize(m_Texture.width, m_Texture.height);
                yield return new WaitForEndOfFrame();

                // Force wait
                if (!Application.isPlaying)
                {
                    // Useless texture update in editor mode when game is not running,
                    // that takes some computational times to be sure that the UI is updated at least one time before the capture
                    if (MultiDisplayUtils.IsMultiDisplay())
                    {
                        yield return MultiDisplayCopyRenderBufferToTextureCoroutine(m_Texture);
                    }
                    else
                    {
                        CaptureScreenToTexture(m_Texture);
                    }
                }

                // Delegate call to notify screen is resized
                onResolutionScreenResizedDelegate(resolution);


                // Wait several frames
                // Particularly needed for special effects using several frame to compute their effects, like temporal anti aliasing
                if (m_GameViewResizingWaitingMode == GameViewResizingWaitingMode.FRAMES || !Application.isPlaying)
                {
                    for (int i = 0; i < m_GameViewResizingWaitingFrames; ++i)
                    {
#if UNITY_EDITOR
                        if (!Application.isPlaying)
                        {
                            GameViewUtils.GetGameView().Repaint();
                        }
#endif
                        yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
                        if (!Application.isPlaying && i < 2)
                        {
                            // Useless update to force waiting for the gamview to be correctly updated
                            CaptureScreenToTexture(m_Texture);
                        }
#endif
                    }
                }
                else
                {
#if (UNITY_5_4_OR_NEWER)
                    yield return new WaitForSecondsRealtime(m_GameViewResizingWaitingTime);
#else
					if (Time.timeScale > 0f) {
						yield return new WaitForSeconds (m_GameViewResizingWaitingTime);
					}
#endif
                    yield return new WaitForEndOfFrame();
                }



                // Capture the screen content
                if (MultiDisplayUtils.IsMultiDisplay())
                {
                    yield return MultiDisplayCopyRenderBufferToTextureCoroutine(m_Texture);
                }
                else
                {
                    CaptureScreenToTexture(m_Texture);
                }

                //				Debug.Log ("End Capture");

            }
            else if (captureMode == CaptureMode.RENDER_TO_TEXTURE)
            {

                // Wait for the end of rendering
                yield return new WaitForEndOfFrame();

                // Do not need to wait anything, just capture the cameras
                RenderTexture renderTexture = GetOrCreateRenderTexture(resolution, antiAliasing);
                CaptureCamerasToTexture(m_Cameras, m_Texture, renderTexture);


            }
            else if (captureMode == CaptureMode.FIXED_GAMEVIEW)
            {
#if UNITY_EDITOR
                // Force repaint in case the gameview is not focus to prevent a lock
                if (!Application.isPlaying)
                {
                    GameViewUtils.GetGameView().Repaint();
                }
#endif

                // Wait for the end of rendering
                yield return new WaitForEndOfFrame();

                // Capture the screen content
                if (MultiDisplayUtils.IsMultiDisplay())
                {
                    yield return MultiDisplayCopyRenderBufferToTextureCoroutine(m_Texture);
                }
                else
                {
                    CaptureScreenToTexture(m_Texture);
                }
            }

        }


        static void CaptureScreenToTexture(Texture2D targetTexture)
        {
            targetTexture.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0);
            targetTexture.Apply(false);
        }

        public void CaptureCamerasToTexture(List<ScreenshotCamera> cameras, Texture2D targetTexture, RenderTexture renderTexture)
        {
            if (cameras == null)
            {
                Debug.LogError("Cameras is null");
                return;
            }
            List<Camera> cams = cameras.Where(x => x.m_Active == true && x.m_Camera != null && x.m_Camera.enabled == true).Select(x => x.m_Camera).ToList();
            CaptureCamerasToTexture(cams, targetTexture, renderTexture);
        }

        public static void CaptureCamerasToTexture(List<Camera> cameras, Texture2D targetTexture, RenderTexture renderTexture)
        {
            // Remember active render texture
            RenderTexture previousActiveRt = RenderTexture.active;
            // Set active render texture
            RenderTexture.active = renderTexture;

            // Render all cameras in custom render texture
            foreach (Camera camera in cameras)
            {
                // Remember target texture
                RenderTexture previousCamRT = camera.targetTexture;
                // Render into texture
                camera.targetTexture = renderTexture;
                camera.Render();
                // Restore target texture
                camera.targetTexture = previousCamRT;
            }

            // Copy render buffer to texture
            targetTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            targetTexture.Apply(false);

            // Restore active render texture
            RenderTexture.active = previousActiveRt;
        }

        #endregion


        #region Transparency

        IEnumerator CaptureAlphaMaskCoroutine(ScreenshotResolution resolution, CaptureMode captureMode, int antiAliasing, ColorFormat colorFormat)
        {


            // Backup first camera clear mode
            var firstCamera = GetFirstActiveCamera();
            var clearMode = firstCamera.clearFlags;
            var clearColor = firstCamera.backgroundColor;

            // Set clear white alpha
            firstCamera.clearFlags = CameraClearFlags.Color;
            firstCamera.backgroundColor = new Color(1f, 1f, 1f, 0.1f);

            // Capture the texture
            yield return StartCoroutine(CaptureResolutionTextureCoroutine(resolution, captureMode, antiAliasing, colorFormat));

            // Copy white texture
            Texture2D whiteTexture = new Texture2D(resolution.m_Texture.width, resolution.m_Texture.height, resolution.m_Texture.format, false, false);
            CloneTexture(resolution.m_Texture, whiteTexture);

            // Set clear black alpha
            firstCamera.clearFlags = CameraClearFlags.Color;
            firstCamera.backgroundColor = new Color(0f, 0f, 0f, 0.1f);

            // Capture the texture, again
            yield return StartCoroutine(CaptureResolutionTextureCoroutine(resolution, captureMode, antiAliasing, colorFormat));

            // Copy black texture
            Texture2D blackTexture = new Texture2D(resolution.m_Texture.width, resolution.m_Texture.height, resolution.m_Texture.format, false, false);
            CloneTexture(resolution.m_Texture, blackTexture);

            // Compute diff
            Color color;
            for (int x = 0; x < resolution.m_Texture.width; ++x)
            {
                for (int y = 0; y < resolution.m_Texture.height; ++y)
                {
                    var alpha = whiteTexture.GetPixel(x, y).r - blackTexture.GetPixel(x, y).r;
                    alpha = 1f - alpha;
                    if (alpha == 0)
                    {
                        color = Color.clear;
                    }
                    else
                    {
                        color = whiteTexture.GetPixel(x, y);
                    }
                    color.a = alpha;
                    resolution.m_Texture.SetPixel(x, y, color);
                }
            }
            resolution.m_Texture.Apply();

            // Clean
            DestroyImmediate(whiteTexture);
            DestroyImmediate(blackTexture);

            // Restore clear settings
            firstCamera.clearFlags = clearMode;
            firstCamera.backgroundColor = clearColor;

        }

        static void CloneTexture(Texture2D src, Texture2D dest)
        {
#if UNITY_5_4_OR_NEWER
            Graphics.CopyTexture(src, dest);
#else
			dest.Resize(src.width, src.height, src.format, false);
            for (int x = 0; x < src.width; ++x)
            {
                for (int y = 0; y < src.height; ++y)
                {
					dest.SetPixel(x,y,src.GetPixel(x,y));
				}
			}
			dest.Apply();
#endif
        }

        #endregion

        #region Multi Display


        Camera GetFirstActiveCamera()
        {
            // Get last camera on the list
            for (int i = 0; i < m_Cameras.Count; i++)
            {
                if (m_Cameras[i].m_Active && m_Cameras[i].m_Camera != null && m_Cameras[i].m_Camera.enabled == true)
                {
                    return m_Cameras[i].m_Camera;
                }
            }
            // If not cameras on the list, get the active camera for display 1 with the higher depth
            Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
            Camera best = null;
            foreach (Camera cam in cameras)
            {

#if (UNITY_5_4_OR_NEWER)
                if (cam.enabled == true && cam.targetDisplay == 0)
                {
#else
				if (cam.enabled == true) {
#endif
                    if (best == null || cam.depth < best.depth)
                    {
                        best = cam;
                    }
                }
            }
            if (best != null)
                return best;
            // Return camera tagged as MainCamera
            return Camera.main;
        }

        Camera GetLastActiveCamera()
        {
            // Get last camera on the list
            for (int i = m_Cameras.Count - 1; i >= 0; i--)
            {
                if (m_Cameras[i].m_Active && m_Cameras[i].m_Camera != null && m_Cameras[i].m_Camera.enabled == true)
                {
                    return m_Cameras[i].m_Camera;
                }
            }
            // If not cameras on the list, get the active camera for display 1 with the higher depth
            Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
            Camera best = null;
            foreach (Camera cam in cameras)
            {

#if (UNITY_5_4_OR_NEWER)
                if (cam.enabled == true && cam.targetDisplay == 0)
                {
#else
				if (cam.enabled == true) {
#endif
                    if (best == null || cam.depth > best.depth)
                    {
                        best = cam;
                    }
                }
            }
            if (best != null)
                return best;
            // Return camera tagged as MainCamera
            return Camera.main;
        }

        IEnumerator MultiDisplayCopyRenderBufferToTextureCoroutine(Texture2D targetTexture)
        {
            // We get the last camera on the list or the last rendered camera
            Camera cameraToCapture = GetLastActiveCamera();

            // On multi display we need to wait for the last camera to capture to be rendered
            if (cameraToCapture != null)
            {
                MultiDisplayCameraCapture captureMultiCam = cameraToCapture.GetComponent<MultiDisplayCameraCapture>();
                // Add a capture camera component and start the capture process
                if (captureMultiCam == null)
                {
                    captureMultiCam = cameraToCapture.gameObject.AddComponent<MultiDisplayCameraCapture>();
                }
                captureMultiCam.CaptureCamera(targetTexture);
                // Wait for capture
                while (!captureMultiCam.CopyIsOver())
                {
                    yield return null;
                }
                // Clean
                GameObject.DestroyImmediate(captureMultiCam);

            }
            else
            {
                // Just read the actual render buffer
                CaptureScreenToTexture(targetTexture);
            }
        }

        #endregion


        #region TEXTURE


        public static Texture2D GetOrCreateTexture(ScreenshotResolution resolution, ColorFormat colorFormat, bool noScale = false)
        {
            // Compute real dimensions
            int width = noScale ? resolution.m_Width : resolution.ComputeTargetWidth();
            int height = noScale ? resolution.m_Height : resolution.ComputeTargetHeight();

            // Init the texture
            GetOrCreateTexture(ref resolution.m_Texture, width, height, colorFormat);

            return resolution.m_Texture;
        }

        public static void GetOrCreateTexture(ref Texture2D texture, int width, int height, ColorFormat colorFormat)
        {
            // Create texture if needed
            TextureFormat format = colorFormat == ColorFormat.RGBA ? TextureFormat.ARGB32 : TextureFormat.RGB24;
            if (texture == null)
            {
                texture = new Texture2D(width, height, format, false);
            }
            else if (texture.width != width || texture.height != height || texture.format != format)
            {
                texture.Resize(width, height, format, false);
            }
        }

        Dictionary<ScreenshotResolution, RenderTexture> m_RenderTextureCache = new Dictionary<ScreenshotResolution, RenderTexture>();

        RenderTexture GetOrCreateRenderTexture(ScreenshotResolution resolution, int antiAliasing = 0, int depth = 32)
        {
            // Compute real resolutions
            int width = resolution.ComputeTargetWidth();
            int height = resolution.ComputeTargetHeight();

            // Get from cache
            RenderTexture rt = null;
            if (m_RenderTextureCache.ContainsKey(resolution))
            {
                rt = m_RenderTextureCache[resolution];
            }
            // Create render texture if needed
            GetOrCreateRenderTexture(ref rt, width, height, antiAliasing, depth);
            // Save to cache
            m_RenderTextureCache[resolution] = rt;

            return rt;
        }

        public static void GetOrCreateRenderTexture(ref RenderTexture renderTexture, int width, int height, int antiAliasing = 0, int depth = 32)
        {
            if (renderTexture == null || renderTexture.width != width || renderTexture.height != height || renderTexture.antiAliasing != antiAliasing || renderTexture.depth != depth)
            {
                renderTexture = new RenderTexture(width, height, depth, RenderTextureFormat.ARGB32);
                if (antiAliasing != 0)
                {
                    renderTexture.antiAliasing = antiAliasing;
                }
            }
        }

        #endregion


        #region GENERAL SETTINGS

        List<RectTransform> disabledUIElements = new List<RectTransform>();

        public void ApplySettings(List<ScreenshotCamera> cameras, List<ScreenshotOverlay> overlays, CaptureMode captureMode, bool renderUI, bool forceUICullingLayer)
        {

            // SET CAMERAS	
            m_Cameras = cameras;
            m_SceneCameras = FindAllOtherCameras(m_Cameras);
            if (captureMode != CaptureMode.RENDER_TO_TEXTURE && m_Cameras.Count > 0)
            {
                DisableCameras(m_SceneCameras);
            }
            ApplyCameraSettings(m_Cameras, captureMode);

            // SET OVERLAYS
            m_Overlays = overlays.Where(x => x.m_Active == true).ToList();
            m_DisabledOverlays.Clear();
            if (forceUICullingLayer && m_Cameras.Count > 0 && renderUI == true)
            {
                // If camera are enabled and we want to render the ui
                // We disable all canvas that are not in any of the camera culling layers
                m_DisabledOverlays = FindAllOtherCanvas(m_Overlays).Where(ov => (ov.m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                                                                                && (m_Cameras.FindAll(ca => (ca.m_CullingMask & (1 << ov.m_Canvas.gameObject.layer)) != 0).Count == 0)
                                                                            ).ToList();
            }
            else if (captureMode != CaptureMode.RENDER_TO_TEXTURE && renderUI == false)
            {
                // If we do not want to render the UI, we disable all canvas
                m_DisabledOverlays = FindAllOtherCanvas(m_Overlays);
            }
            DisableCanvas(m_DisabledOverlays);
            ApplyOverlaySettings(m_Overlays);

            // Force UI culling mask
            if (forceUICullingLayer && m_Cameras.Count > 0 && renderUI == true)
            {
                disabledUIElements = GameObject.FindObjectsOfType<RectTransform>().Where(r => r.gameObject.activeInHierarchy
                                                                                                && (r.GetComponentInParent<Canvas>() != null)
                                                                                                && (m_Cameras.FindAll(ca => (ca.m_CullingMask & (1 << r.gameObject.layer)) != 0).Count == 0)
                                                                                            ).ToList();
                foreach (var r in disabledUIElements)
                {
                    r.gameObject.SetActive(false);
                }
            }
        }

        void RestoreSettings()
        {
            // Restore cameras
            RestoreCameraSettings(m_Cameras);
            RestoreCameraSettings(m_SceneCameras);

            // Restore overlays
            RestoreOverlaySettings(m_Overlays);
            RestoreOverlaySettings(m_DisabledOverlays);

            // Restore disabled ui elements
            foreach (var r in disabledUIElements)
            {
                if (r != null)
                {
                    r.gameObject.SetActive(true);
                }
            }
            disabledUIElements.Clear();
        }

        float m_PreviousTimeScale = 1f;

        void StopTime()
        {
            m_PreviousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        void RestoreTime()
        {
            Time.timeScale = m_PreviousTimeScale;
        }

        #endregion


        #region CANVAS SETTINGS

        List<ScreenshotOverlay> FindAllOtherCanvas(List<ScreenshotOverlay> overlays)
        {
            List<ScreenshotOverlay> sceneUIOverlaysCanvas = new List<ScreenshotOverlay>();

            // Find all canvas using screenspaceoverlay on the scene
            Canvas[] canvas = GameObject.FindObjectsOfType<Canvas>();
            foreach (Canvas canva in canvas)
            {
                if (canva.gameObject.activeInHierarchy == true
                    && canva.enabled == true)
                {

                    // Ignore overlays canvas
                    bool contains = false;
                    foreach (ScreenshotOverlay overlay in overlays)
                    {
                        if (overlay.m_Canvas == canva && overlay.m_Active)
                            contains = true;
                    }
                    if (contains)
                        continue;

                    // Add canvas to list
                    sceneUIOverlaysCanvas.Add(new ScreenshotOverlay(canva));
                }
            }
            return sceneUIOverlaysCanvas;
        }

        void DisableCanvas(List<ScreenshotOverlay> overlays)
        {
            if (overlays == null)
                return;
            foreach (ScreenshotOverlay overlay in overlays)
            {
                if (overlay == null)
                    continue;
                overlay.Disable();
            }
        }

        void ApplyOverlaySettings(List<ScreenshotOverlay> overlays)
        {
            if (overlays == null)
                return;
            foreach (ScreenshotOverlay overlay in overlays)
            {
                if (overlay == null)
                    continue;
                if (overlay.m_Active && overlay.m_Canvas != null)
                {
                    overlay.ApplySettings();
                }
            }
        }

        public void RestoreOverlaySettings(List<ScreenshotOverlay> overlays)
        {
            if (overlays == null)
                return;
            foreach (ScreenshotOverlay overlay in overlays)
            {
                if (overlay == null)
                    continue;
                if (overlay.m_Active && overlay.m_Canvas != null)
                {
                    overlay.RestoreSettings();
                }
            }
        }

        #endregion


        #region CAMERAS SETTINGS


        List<ScreenshotCamera> FindAllOtherCameras(List<ScreenshotCamera> cameras)
        {
            List<ScreenshotCamera> cams = new List<ScreenshotCamera>();
            Camera[] sceneCameras = GameObject.FindObjectsOfType<Camera>();
            foreach (Camera camera in sceneCameras)
            {
                bool contains = false;
                foreach (ScreenshotCamera cam in cameras)
                {
                    if (cam.m_Camera == camera && cam.m_Active == true)
                    {
                        contains = true;
                    }
                }
                if (!contains)
                {
                    cams.Add(new ScreenshotCamera(camera));
                }
            }
            return cams;
        }

        void DisableCameras(List<ScreenshotCamera> cameras)
        {
            if (cameras == null)
                return;

            foreach (ScreenshotCamera camera in cameras)
            {
                if (camera == null)
                    continue;
                camera.Disable();
            }
        }

        public void RestoreCameraSettings(List<ScreenshotCamera> cameras)
        {
            if (cameras == null)
                return;

            foreach (ScreenshotCamera camera in cameras)
            {
                if (camera == null)
                    continue;
                if (camera.m_Active == false)
                    continue;
                if (camera.m_Camera == null)
                    continue;
                camera.RestoreSettings();
            }
        }

        void ApplyCameraSettings(List<ScreenshotCamera> cameras, CaptureMode captureMode)
        {
            if (cameras == null)
                return;

            foreach (ScreenshotCamera camera in cameras)
            {
                if (camera == null)
                    continue;
                if (camera.m_Active == false)
                    continue;
                if (camera.m_Camera == null)
                    continue;

                camera.ApplySettings(captureMode == CaptureMode.RENDER_TO_TEXTURE);
            }
        }

        #endregion



    }
}
