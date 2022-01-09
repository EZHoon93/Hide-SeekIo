using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PluginMaster
{
    public class ThumbnailUtils
    {
        private const int LAYER = 7;
        private static readonly LayerMask LAYER_MASK = 1 << LAYER;
        public const int SIZE = 256;
        private const int MIN_SIZE = 24;
        private static Texture2D _emptyTexture = null;

        private class ThumbnailEditor
        {
            public ThumbnailSettings settings = null;
            public GameObject root = null;
            public Camera camera = null;
            public RenderTexture renderTexture = null;
            public Light light = null;
            public Transform pivot = null;
            public GameObject target = null;
        }

        public static void RenderTextureToTexture2D(RenderTexture renderTexture, Texture2D texture)
        {
            var prevActive = RenderTexture.active;
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, SIZE, SIZE), 0, 0);
            texture.Apply();
            RenderTexture.active = prevActive;
        }

        private static Texture2D emptyTexture
        {
            get
            {
                if (_emptyTexture == null) _emptyTexture = Resources.Load<Texture2D>("Sprites/Empty");
                return _emptyTexture;
            }
        }

        public static void UpdateThumbnail(ThumbnailSettings settings, Texture2D thumbnailTexture, GameObject prefab)
        {
            var magnitude = BoundsUtils.GetMagnitude(prefab.transform);
            var thumbnailEditor = new ThumbnailEditor();
            thumbnailEditor.settings = new ThumbnailSettings(settings);

            if (magnitude == 0)
            {
                if (_emptyTexture == null) _emptyTexture = Resources.Load<Texture2D>("Sprites/Empty");
                var pixels = _emptyTexture.GetPixels32();
                for (int i = 0; i < pixels.Length; ++i)
                {
                    if (pixels[i].a == 0) pixels[i] = thumbnailEditor.settings.backgroudColor;
                }
                thumbnailTexture.SetPixels32(pixels);
                thumbnailTexture.Apply();
                return;
            }
            var sceneLights = Object.FindObjectsOfType<Light>().ToDictionary(comp => comp, light => light.cullingMask);

            const string rootName = "PWBThumbnailEditor";

            do
            {
                var obj = GameObject.Find(rootName);
                if (obj == null) break;
                else GameObject.DestroyImmediate(obj);
            } while (true);

            thumbnailEditor.root = new GameObject(rootName);

            var camObj = new GameObject("PWBThumbnailEditorCam");
            thumbnailEditor.camera = camObj.AddComponent<Camera>();
            thumbnailEditor.camera.transform.parent = thumbnailEditor.root.transform;
            thumbnailEditor.camera.transform.localPosition = new Vector3(0f, 1.2f, -4f);
            thumbnailEditor.camera.transform.localRotation = Quaternion.Euler(17.5f, 0f, 0f);
            thumbnailEditor.camera.fieldOfView = 20f;
            thumbnailEditor.camera.clearFlags = CameraClearFlags.SolidColor;
            thumbnailEditor.camera.backgroundColor = thumbnailEditor.settings.backgroudColor;
            thumbnailEditor.camera.cullingMask = LAYER_MASK;
            thumbnailEditor.renderTexture = new RenderTexture(SIZE, SIZE, 24);
            thumbnailEditor.camera.targetTexture = thumbnailEditor.renderTexture;

            var lightObj = new GameObject("PWBThumbnailEditorLight");
            thumbnailEditor.light = lightObj.AddComponent<Light>();
            thumbnailEditor.light.type = LightType.Directional;
            thumbnailEditor.light.transform.parent = thumbnailEditor.root.transform;
            thumbnailEditor.light.transform.localRotation = Quaternion.Euler(thumbnailEditor.settings.lightEuler);
            thumbnailEditor.light.color = thumbnailEditor.settings.lightColor;
            thumbnailEditor.light.intensity = thumbnailEditor.settings.lightIntensity;
            thumbnailEditor.light.cullingMask = LAYER_MASK;

            var pivotObj = new GameObject("PWBThumbnailEditorPivot");
            pivotObj.layer = LAYER;
            thumbnailEditor.pivot = pivotObj.transform;
            thumbnailEditor.pivot.transform.parent = thumbnailEditor.root.transform;
            thumbnailEditor.pivot.localPosition = thumbnailEditor.settings.targetOffset;
            thumbnailEditor.pivot.transform.localRotation = Quaternion.identity;
            thumbnailEditor.pivot.transform.localScale = Vector3.one;

            thumbnailEditor.target = GameObject.Instantiate(prefab);
            var monoBehaviours = thumbnailEditor.target.GetComponentsInChildren<MonoBehaviour>();
            foreach (var monoBehaviour in monoBehaviours) monoBehaviour.enabled = false;

            magnitude = BoundsUtils.GetMagnitude(thumbnailEditor.target.transform);
            var targetScale = magnitude > 0 ? 1f / magnitude : 1f;
            var targetBounds = BoundsUtils.GetBoundsRecursive(thumbnailEditor.target.transform);
            var localPosition = (thumbnailEditor.target.transform.localPosition - targetBounds.center) * targetScale;
            thumbnailEditor.target.transform.parent = thumbnailEditor.pivot;
            thumbnailEditor.target.transform.localPosition = localPosition;
            thumbnailEditor.target.transform.localRotation = Quaternion.identity;
            thumbnailEditor.target.transform.localScale = prefab.transform.localScale * targetScale;
            thumbnailEditor.pivot.localScale = Vector3.one * thumbnailEditor.settings.zoom;
            thumbnailEditor.pivot.localRotation = Quaternion.Euler(thumbnailEditor.settings.targetEuler);

            var children = thumbnailEditor.root.GetComponentsInChildren<Transform>();
            foreach (var child in children)
            {
                child.gameObject.layer = LAYER;
                child.gameObject.hideFlags = HideFlags.HideAndDontSave;
            }
            foreach (var light in sceneLights.Keys) light.cullingMask = light.cullingMask & ~LAYER_MASK;
            thumbnailEditor.camera.Render();
            foreach (var light in sceneLights.Keys) light.cullingMask = sceneLights[light];
            RenderTextureToTexture2D(thumbnailEditor.camera.targetTexture, thumbnailTexture);

            Object.DestroyImmediate(thumbnailEditor.root);
        }

        public static void UpdateThumbnail(ThumbnailSettings settings, Texture2D thumbnailTexture, Texture2D[] subThumbnails)
        {
            if (subThumbnails.Length == 0)
            {
                thumbnailTexture.SetPixels(new Color[SIZE * SIZE]);
                thumbnailTexture.Apply();
                return;
            }
            var sqrt = Mathf.Sqrt(subThumbnails.Length);
            var sideCellsCount = Mathf.FloorToInt(sqrt);
            if (Mathf.CeilToInt(sqrt) != sideCellsCount) ++sideCellsCount;
            var spacing = (SIZE * sideCellsCount) / MIN_SIZE;
            var bigSize = SIZE * sideCellsCount + spacing * (sideCellsCount - 1);
            var texture = new Texture2D(bigSize, bigSize);
            var pixelCount = bigSize * bigSize;
            var pixels = new Color32[pixelCount];
            texture.SetPixels32(pixels);
            int subIdx = 0;
            for (int i = sideCellsCount - 1; i >= 0; --i)
            {
                for (int j = 0; j < sideCellsCount; ++j)
                {
                    var x = j * (SIZE + spacing);
                    var y = i * (SIZE + spacing);
                    var subPixels = subThumbnails[subIdx].GetPixels32();
                    texture.SetPixels32(x, y, SIZE, SIZE, subPixels);
                    ++subIdx;
                    if (subIdx == subThumbnails.Length) goto Resize;
                }
            }
        Resize:
            texture.filterMode = FilterMode.Trilinear;
            texture.Apply();
            var renderTexture = new RenderTexture(SIZE, SIZE, 24);
            var prevActive = RenderTexture.active;
            RenderTexture.active = renderTexture;
            Graphics.Blit(texture, renderTexture);
            thumbnailTexture.ReadPixels(new Rect(0, 0, SIZE, SIZE), 0, 0);
            thumbnailTexture.Apply();
            RenderTexture.active = prevActive;
            Object.DestroyImmediate(texture);
        }

        public static void UpdateThumbnail(MultibrushItemSettings brushItem)
        {
            if (brushItem.prefab == null) return;
            UpdateThumbnail(brushItem.thumbnailSettings, brushItem.thumbnailTexture, brushItem.prefab);
        }

        public static void UpdateThumbnail(MultibrushSettings brushSettings)
        {
            var brushItems = brushSettings.items;
            var subThumbnails = new List<Texture2D>();
            foreach (var item in brushItems)
            {
                if (item.includeInThumbnail) subThumbnails.Add(item.thumbnail);
                UpdateThumbnail(item);
            }
            UpdateThumbnail(brushSettings.thumbnailSettings, brushSettings.thumbnailTexture, subThumbnails.ToArray());
        }
    }
}
