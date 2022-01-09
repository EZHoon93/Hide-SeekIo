/*
Copyright (c) 2020 Omar Duarte
Unauthorized copying of this file, via any medium is strictly prohibited.
Writen by Omar Duarte, 2020.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace PluginMaster
{
    #region DATA & SETTINGS
    [Serializable]
    public class BrushToolSettings : BrushToolBase, IPaintOnSurfaceToolSettings, ISerializationCallbackReceiver
    {
        [SerializeField] private PaintOnSurfaceToolSettings _paintOnSurfaceToolSettings = new PaintOnSurfaceToolSettings();

        [SerializeField] private float _maxHeightFromCenter = 2f;
        public enum HeightType { CUSTOM, RADIUS }
        [SerializeField] private HeightType _heightType = HeightType.RADIUS;

        public enum AvoidOverlappingType { DISABLED, WITH_PALETTE_PREFABS, WITH_BRUSH_PREFABS, WITH_SAME_PREFABS }
        [SerializeField] private AvoidOverlappingType _avoidOverlapping = AvoidOverlappingType.WITH_PALETTE_PREFABS;

        [SerializeField] private LayerMask _layerFilter = -1;
        [SerializeField] private List<string> _tagFilter = null;
        [SerializeField] private RandomUtils.Range _slopeFilter = new RandomUtils.Range(0, 60);

        public BrushToolSettings() : base() => _paintOnSurfaceToolSettings.OnDataChanged += DataChanged;

        public bool paintOnMeshesWithoutCollider
        {
            get => _paintOnSurfaceToolSettings.paintOnMeshesWithoutCollider;
            set => _paintOnSurfaceToolSettings.paintOnMeshesWithoutCollider = value;
        }
        public bool paintOnSelectedOnly
        {
            get => _paintOnSurfaceToolSettings.paintOnSelectedOnly;
            set => _paintOnSurfaceToolSettings.paintOnSelectedOnly = value;
        }
        public bool paintOnPalettePrefabs
        {
            get => _paintOnSurfaceToolSettings.paintOnPalettePrefabs;
            set => _paintOnSurfaceToolSettings.paintOnPalettePrefabs = value;
        }

        public float maxHeightFromCenter
        {
            get => _maxHeightFromCenter;
            set
            {
                if (_maxHeightFromCenter == value) return;
                _maxHeightFromCenter = value;
                DataChanged();
            }
        }
        public HeightType heightType
        {
            get => _heightType;
            set
            {
                if (_heightType == value) return;
                _heightType = value;
                DataChanged();
            }
        }
        public AvoidOverlappingType avoidOverlapping
        {
            get => _avoidOverlapping;
            set
            {
                if (_avoidOverlapping == value) return;
                _avoidOverlapping = value;
                DataChanged();
            }
        }

        public virtual LayerMask layerFilter
        {
            get => _layerFilter;
            set
            {
                if (_layerFilter == value) return;
                _layerFilter = value;
                DataChanged();
            }
        }
        public virtual List<string> tagFilter
        {
            get
            {
                if (_tagFilter == null) UpdateTagFilter();
                return _tagFilter;
            }
            set
            {
                if (_tagFilter == value) return;
                _tagFilter = value;
                DataChanged();
            }
        }
        public virtual RandomUtils.Range slopeFilter
        {
            get => _slopeFilter;
            set
            {
                if (_slopeFilter == value) return;
                _slopeFilter = value;
                DataChanged();
            }
        }
        public override void Copy(IToolSettings other)
        {
            var otherBrushToolSettings = other as BrushToolSettings;
            if (otherBrushToolSettings == null) return;
            base.Copy(other);
            _paintOnSurfaceToolSettings.Copy(otherBrushToolSettings._paintOnSurfaceToolSettings);
            _maxHeightFromCenter = otherBrushToolSettings._maxHeightFromCenter;
            _heightType = otherBrushToolSettings._heightType;
            _avoidOverlapping = otherBrushToolSettings._avoidOverlapping;
            _layerFilter = otherBrushToolSettings._layerFilter;
            _tagFilter = otherBrushToolSettings._tagFilter == null ? null
                : new List<string>(otherBrushToolSettings._tagFilter);
            _slopeFilter = new RandomUtils.Range(otherBrushToolSettings._slopeFilter);
        }

        private void UpdateTagFilter()
        {
            if (_tagFilter != null) return;
            _tagFilter = new List<string>(UnityEditorInternal.InternalEditorUtility.tags);
        }
        public void OnBeforeSerialize() => UpdateTagFilter();
        public void OnAfterDeserialize() => UpdateTagFilter();
    }

    [Serializable]
    public class BrushManager : ToolManagerBase<BrushToolSettings> { }
    #endregion

    #region PWBIO
    public static partial class PWBIO
    {
        private static float _brushAngle = 0f;
        private static bool BrushRaycast(Ray ray, out RaycastHit hit, float maxDistance,
            LayerMask layerMask, BrushToolSettings settings)
        {
            hit = new RaycastHit();
            bool result = false;
            var noColliderDistance = float.MaxValue;
            var meshRaycastResult = result;
            if (MouseRaycast(ray, out RaycastHit hitInfo, out GameObject collider, maxDistance,
                layerMask, settings.paintOnPalettePrefabs, true, settings.tagFilter.ToArray()))
            {
                var nearestRoot = PrefabUtility.GetNearestPrefabInstanceRoot(collider);
                bool isAPaintedObject = false;
                while (nearestRoot != null)
                {
                    isAPaintedObject = isAPaintedObject || _paintedObjects.Contains(nearestRoot);
                    var parent = nearestRoot.transform.parent == null ? null
                        : nearestRoot.transform.parent.gameObject;
                    nearestRoot = parent == null ? null : PrefabUtility.GetNearestPrefabInstanceRoot(parent);
                }
                bool selectedOnlyFilter = !settings.paintOnSelectedOnly
                    || SelectionManager.selection.Contains(collider)
                    || PWBCore.CollidersContains(SelectionManager.selection, collider.name);
                bool paletteFilter = !isAPaintedObject || settings.paintOnPalettePrefabs;
                var filterResult = selectedOnlyFilter && paletteFilter;
                result = result || filterResult;
                if (filterResult && (hitInfo.distance < noColliderDistance || !meshRaycastResult))
                    hit = hitInfo;
            }
            return result;
        }

        private static void BrushDuringSceneGUI(SceneView sceneView)
        {
            BrushstrokeMouseEvents(BrushManager.settings);
            if (Event.current.type == EventType.Repaint)
            {
                var mousePos = Event.current.mousePosition;
                if (_pinned) mousePos = _pinMouse;
                var mouseRay = HandleUtility.GUIPointToWorldRay(mousePos);
                bool snappedToVertex = false;
                var closestVertexInfo = new RaycastHit();
                if (_snapToVertex)
                    snappedToVertex = SnapToVertex(mouseRay, out closestVertexInfo, sceneView.in2DMode);
                if (snappedToVertex) mouseRay.origin = closestVertexInfo.point - mouseRay.direction;
                if (BrushRaycast(mouseRay, out RaycastHit hit, float.MaxValue, -1, BrushManager.settings))
                    DrawBrush(sceneView, hit);
                else _paintStroke.Clear();
            }
            if (Event.current.button == 0 && !Event.current.alt
                && (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag))
            {
                Paint(BrushManager.settings);
                Event.current.Use();
            }
            else if (Event.current.type == EventType.ScrollWheel && PaletteManager.selectedBrush != null
            && BrushManager.settings.brushShape != BrushToolSettings.BrushShape.POINT)
            {
                if (Event.current.control && !Event.current.alt)
                {
                    BrushManager.settings.radius = UpdateRadius(BrushManager.settings.radius);
                    if (BrushManager.settings.heightType == BrushToolSettings.HeightType.RADIUS)
                    {
                        BrushManager.settings.maxHeightFromCenter = BrushManager.settings.radius;
                        ToolProperties.RepainWindow();
                    }
                    Event.current.Use();
                }
                else if (Event.current.control && Event.current.alt)
                {
                    BrushManager.settings.density += (int)Mathf.Sign(Event.current.delta.y);
                    ToolProperties.RepainWindow();
                    Event.current.Use();
                }
            }
        }

        private static Vector3 GetTangent(Vector3 normal)
        {
            var rotation = Quaternion.AngleAxis(_brushAngle, Vector3.up);
            var tangent = Vector3.Cross(normal, rotation * Vector3.right);
            if (tangent.sqrMagnitude < 0.000001) tangent = Vector3.Cross(normal, rotation * Vector3.forward);
            tangent.Normalize();
            return tangent;
        }

        private static void DrawBrush(SceneView sceneView, RaycastHit hit)
        {
            var settings = BrushManager.settings;
            UpdateStrokeDirection(hit.point);
            if (PaletteManager.selectedBrush == null) return;

            hit.point = SnapAndUpdateGridOrigin(hit.point, SnapManager.settings.snappingEnabled,
                settings.paintOnPalettePrefabs, settings.paintOnMeshesWithoutCollider, false);

            var tangent = GetTangent(hit.normal);
            var bitangent = Vector3.Cross(hit.normal, tangent);

            if (settings.brushShape == BrushToolSettings.BrushShape.POINT)
            {
                DrawCricleIndicator(hit.point, hit.normal, 0.1f, settings.maxHeightFromCenter,
                tangent, bitangent, hit.normal, settings.paintOnPalettePrefabs, true,
                settings.layerFilter, settings.tagFilter.ToArray());
            }
            else
            {
                Handles.zTest = CompareFunction.Always;
                Handles.color = Color.green;
                Handles.DrawAAPolyLine(3, hit.point, hit.point + hit.normal * settings.maxHeightFromCenter);
                if (settings.brushShape == BrushToolSettings.BrushShape.CIRCLE)
                {
                    DrawCricleIndicator(hit.point, hit.normal, settings.radius, settings.maxHeightFromCenter, tangent,
                        bitangent, hit.normal, settings.paintOnPalettePrefabs, true,
                        settings.layerFilter, settings.tagFilter.ToArray());
                }
                else if (settings.brushShape == BrushToolSettings.BrushShape.SQUARE)
                {
                    DrawSquareIndicator(hit.point, hit.normal, settings.radius, settings.maxHeightFromCenter, tangent,
                        bitangent, hit.normal, settings.paintOnPalettePrefabs, true,
                        settings.layerFilter, settings.tagFilter.ToArray());
                }
            }

            BrushstrokePreview(hit.point, hit.normal, tangent, bitangent, sceneView.camera);
        }

        private static void BrushstrokePreview(Vector3 hitPoint, Vector3 normal,
            Vector3 tangent, Vector3 bitangent, Camera camera)
        {
            var settings = BrushManager.settings;
            _paintStroke.Clear();
            foreach (var strokeItem in BrushstrokeManager.brushstroke)
            {
                var worldPos = hitPoint + TangentSpaceToWorld(tangent, bitangent,
                    new Vector2(strokeItem.tangentPosition.x, strokeItem.tangentPosition.y));
                var height = settings.heightType == BrushToolSettings.HeightType.CUSTOM
                    ? settings.maxHeightFromCenter : settings.radius;
                var ray = new Ray(worldPos + normal * height, -normal);
                if (BrushRaycast(ray, out RaycastHit itemHit, height * 2f, settings.layerFilter, settings))
                {
                    var slope = Mathf.Abs(Vector3.Angle(Vector3.up, itemHit.normal));
                    if (slope > 90f) slope = 180f - slope;
                    if (slope < settings.slopeFilter.min
                        || slope > settings.slopeFilter.max) continue;
                    var prefab = strokeItem.settings.prefab;
                    if (prefab == null) continue;
                    BrushSettings brushSettings = strokeItem.settings;
                    if (settings.overwriteBrushProperties)
                    {
                        brushSettings = settings.brushSettings;
                    }
                    var itemRotation = Quaternion.AngleAxis(_brushAngle, Vector3.up);
                    var itemPosition = itemHit.point;
                    if (brushSettings.rotateToTheSurface)
                    {
                        var itemTangent = GetTangent(itemHit.normal);
                        itemRotation = Quaternion.LookRotation(itemTangent, itemHit.normal);
                        itemPosition += itemHit.normal * brushSettings.surfaceDistance;
                    }
                    else itemPosition += normal * brushSettings.surfaceDistance;

                    if (settings.avoidOverlapping != BrushToolSettings.AvoidOverlappingType.DISABLED)
                    {
                        var rSqr = settings.minSpacing * settings.minSpacing;
                        var d = settings.density / 100f;
                        var densitySpacing = Mathf.Sqrt(rSqr / d);
                        var nearbyObjectsAtDensitySpacing = _octree.GetNearby(itemPosition, densitySpacing);
                        if (nearbyObjectsAtDensitySpacing.Length > 0)
                        {
                            var brushObjectsNearby = false;
                            foreach (var obj in nearbyObjectsAtDensitySpacing)
                            {
                                if (settings.avoidOverlapping
                                    == BrushToolSettings.AvoidOverlappingType.WITH_BRUSH_PREFABS
                                    && PaletteManager.selectedBrush.ContainsSceneObject(obj))
                                {
                                    brushObjectsNearby = true;
                                    break;
                                }
                                else if (settings.avoidOverlapping
                                    == BrushToolSettings.AvoidOverlappingType.WITH_PALETTE_PREFABS
                                    && PaletteManager.selectedPalette.ContainsSceneObject(obj))
                                {
                                    brushObjectsNearby = true;
                                    break;
                                }
                                else if (settings.avoidOverlapping
                                        == BrushToolSettings.AvoidOverlappingType.WITH_SAME_PREFABS)
                                {
                                    var outermostPrefab = PrefabUtility.GetOutermostPrefabInstanceRoot(obj);
                                    if (outermostPrefab == null) continue;
                                    var source = PrefabUtility.GetCorrespondingObjectFromSource(outermostPrefab);
                                    if (source == null) continue;
                                    if (prefab == source)
                                    {
                                        brushObjectsNearby = true;
                                        break;
                                    }
                                }
                            }
                            if (brushObjectsNearby) continue;
                        }
                    }
                    if (settings.orientAlongBrushstroke)
                    {
                        itemRotation = Quaternion.Euler(settings.additionalOrientationAngle)
                            * Quaternion.LookRotation(_strokeDirection, itemRotation * Vector3.up);
                        itemPosition = hitPoint + itemRotation * (itemPosition - hitPoint);
                    }
                    itemRotation *= Quaternion.Euler(strokeItem.additionalAngle);
                    if (brushSettings.embedInSurface)
                    {
                        var TRS = Matrix4x4.TRS(itemPosition, itemRotation, strokeItem.scaleMultiplier);
                        var bottomDistanceToSurfce = GetBottomDistanceToSurface(
                            strokeItem.settings.bottomVertices, TRS,
                            strokeItem.settings.height * strokeItem.scaleMultiplier.y,
                            settings.paintOnPalettePrefabs, settings.paintOnMeshesWithoutCollider);
                        if (!brushSettings.embedAtPivotHeight)
                            bottomDistanceToSurfce -= strokeItem.settings.bottomMagnitude;
                        itemPosition += itemRotation * new Vector3(0f, -bottomDistanceToSurfce, 0f);
                    }
                    itemPosition += itemRotation * brushSettings.localPositionOffset;

                    var rootToWorld = Matrix4x4.TRS(itemPosition, itemRotation, strokeItem.scaleMultiplier)
                        * Matrix4x4.Translate(-prefab.transform.position);
                    var itemScale = Vector3.Scale(prefab.transform.localScale, strokeItem.scaleMultiplier);

                    var layer = settings.overwritePrefabLayer ? settings.layer : prefab.layer;
                    Transform parentTransform = GetParent(settings, prefab.name, false);
                    _paintStroke.Add(new PaintStrokeItem(prefab, itemPosition,
                        itemRotation * Quaternion.Euler(prefab.transform.eulerAngles),
                        itemScale, layer, parentTransform));
                    PreviewBrushItem(prefab, rootToWorld, layer, camera);
                }
            }
        }
    }
    #endregion
}
