/*
Copyright (c) 2021 Omar Duarte
Unauthorized copying of this file, via any medium is strictly prohibited.
Writen by Omar Duarte, 2021.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace PluginMaster
{
    #region DATA & SETTINGS
    [Serializable]
    public class ReplacerSettings : CircleToolBase, IModifierTool
    {
        [SerializeField] private ModifierToolSettings _modifierTool = new ModifierToolSettings();
        public ReplacerSettings() => _modifierTool.OnDataChanged += DataChanged;
        public ModifierToolSettings.Command command { get => _modifierTool.command; set => _modifierTool.command = value; }
        public bool modifyAllButSelected
        {
            get => _modifierTool.modifyAllButSelected;
            set => _modifierTool.modifyAllButSelected = value;
        }

        [SerializeField] private bool _keepTargetSize = false;
        [SerializeField] private bool _maintainProportions = false;
        [SerializeField] private bool _outermostPrefabFilter = true;

        public bool keepTargetSize
        {
            get => _keepTargetSize;
            set
            {
                if (_keepTargetSize == value) return;
                _keepTargetSize = value;
                DataChanged();
            }
        }
        public bool maintainProportions
        {
            get => _maintainProportions;
            set
            {
                if (_maintainProportions == value) return;
                _maintainProportions = value;
                DataChanged();
            }
        }
        public bool outermostPrefabFilter
        {
            get => _outermostPrefabFilter;
            set
            {
                if (_outermostPrefabFilter == value) return;
                _outermostPrefabFilter = value;
                DataChanged();
            }
        }
        public override void Copy(IToolSettings other)
        {
            var otherReplacer = other as ReplacerSettings;
            if (otherReplacer == null) return;
            base.Copy(other);
            _modifierTool.Copy(otherReplacer);
            _keepTargetSize = otherReplacer._keepTargetSize;
            _maintainProportions = otherReplacer._maintainProportions;
            _outermostPrefabFilter = otherReplacer._outermostPrefabFilter;
        }
    }

    [Serializable]
    public class ReplacerManager : ToolManagerBase<ReplacerSettings> { }
    #endregion

    #region PWBIO
    public static partial class PWBIO
    {
        private static List<GameObject> _toReplace = new List<GameObject>();
        private static List<Renderer> _replaceRenderers = new List<Renderer>();
        private static bool _replaceAllSelected = false;
        private static void ReplacerMouseEvents()
        {
            var settings = ReplacerManager.settings;
            if (Event.current.button == 0 && !Event.current.alt
                && (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag))
            {
                Replace();
                Event.current.Use();
            }
            if (Event.current.type == EventType.ScrollWheel && Event.current.control)
            {
                settings.radius = UpdateRadius(settings.radius);
                Event.current.Use();
            }
            if (Event.current.button == 1)
            {
                if (Event.current.type == EventType.MouseDown && (Event.current.control || Event.current.shift))
                {
                    _pinned = true;
                    _pinMouse = Event.current.mousePosition;
                    Event.current.Use();
                }
                else if (Event.current.type == EventType.MouseUp) _pinned = false;

                else if (Event.current.type == EventType.MouseDrag && Event.current.shift)
                {
                    var delta = Mathf.Sign(Event.current.delta.x);
                    settings.radius = Mathf.Max(settings.radius * (1f + delta * 0.03f), 0.05f);
                    ToolProperties.RepainWindow();
                    Event.current.Use();
                }
            }
        }

        private static void ReplacerDuringSceneGUI(SceneView sceneView)
        {
            if (PaletteManager.selectedBrushIdx < 0) return;
            if(_replaceAllSelected)
            {
                BrushstrokeManager.UpdateBrushstroke();
                _paintStroke.Clear();
                _toReplace.Clear();
                _replaceAllSelected = false;
                _toReplace.AddRange(SelectionManager.topLevelSelection);
                foreach (var selected in _toReplace)
                    ReplacePreview(sceneView.camera, selected.transform);
                Replace();
                return;
            }
            ReplacerMouseEvents();
            if (Event.current.type == EventType.Repaint)
            {
                var mousePos = Event.current.mousePosition;
                if (_pinned) mousePos = _pinMouse;
                var mouseRay = HandleUtility.GUIPointToWorldRay(mousePos);

                if (_octree == null) UpdateOctree();
                var center = mouseRay.GetPoint(_lastHitDistance);
                if (MouseRaycast(mouseRay, out RaycastHit mouseHit, out GameObject collider,
                    float.MaxValue, -1, true, true))
                {
                    _lastHitDistance = mouseHit.distance;
                    center = mouseHit.point;
                }
                DrawReplacerCircle(center, mouseRay, sceneView.camera);
            }
        }
        public static void ResetReplacer()
        {
            foreach (var renderer in _replaceRenderers)
            {
                if (renderer == null) continue;
                renderer.enabled = true;
            }
            _toReplace.Clear();
            _replaceRenderers.Clear();
            _paintStroke.Clear();
        }

        private static void ReplacePreview(Camera camera, Transform target)
        {
            if (BrushstrokeManager.brushstroke.Length == 0) return;
            var strokeItem = BrushstrokeManager.brushstroke[0];
            var prefab = strokeItem.settings.prefab;
            if (prefab == null) return;
            var itemRotation = target.rotation;
            var targetBounds = BoundsUtils.GetBoundsRecursive(target, target.rotation);
            var itemBounds = BoundsUtils.GetBoundsRecursive(prefab.transform, prefab.transform.rotation);
            var itemPosition = targetBounds.center - Vector3.Scale(itemBounds.center, strokeItem.scaleMultiplier);

            var scaleMult = strokeItem.scaleMultiplier;
            if (ReplacerManager.settings.keepTargetSize)
            {
                var targetSize = targetBounds.size;
                var itemSize = itemBounds.size;
                if (ReplacerManager.settings.maintainProportions)
                {
                    var targetMagnitude = Mathf.Max(targetSize.x, targetSize.y, targetSize.z);
                    var itemMagnitude = Mathf.Max(itemSize.x, itemSize.y, itemSize.z);
                    scaleMult = Vector3.one * (targetMagnitude / itemMagnitude);
                }
                else scaleMult = new Vector3(targetSize.x / itemSize.x, targetSize.y / itemSize.y, targetSize.z / itemSize.z);
            }
            var itemScale = Vector3.Scale(prefab.transform.localScale, scaleMult);

            var layer = target.gameObject.layer;
            Transform parentTransform = target.parent;
            _paintStroke.Add(new PaintStrokeItem(prefab, itemPosition,
                itemRotation * prefab.transform.rotation,
                itemScale, layer, parentTransform));
            var rootToWorld = Matrix4x4.TRS(itemPosition, itemRotation, scaleMult)
                * Matrix4x4.Translate(-prefab.transform.position);
            PreviewBrushItem(prefab, rootToWorld, layer, camera);
        }

        private static void Replace()
        {
            if (_toReplace.Count == 0) return;
            if (_paintStroke.Count != _toReplace.Count) return;
            const string COMMAND_NAME = "Replace";
            foreach (var renderer in _replaceRenderers) renderer.enabled = true;
            _replaceRenderers.Clear();
            for (int i = 0; i < _toReplace.Count; ++i)
            {
                var target = _toReplace[i];
                if (target == null) continue;
                var item = _paintStroke[i];
                if (item.prefab == null) continue;
                if (ReplacerManager.settings.outermostPrefabFilter)
                {
                    var nearestRoot = PrefabUtility.GetNearestPrefabInstanceRoot(target);
                    if (nearestRoot != null) target = nearestRoot;
                }
                else
                {
                    var parent = target.transform.parent.gameObject;
                    if (parent != null)
                    {
                        GameObject outermost = null;
                        do
                        {
                            outermost = PrefabUtility.GetOutermostPrefabInstanceRoot(target);
                            if (outermost == null) break;
                            if (outermost == target) break;
                            PrefabUtility.UnpackPrefabInstance(outermost,
                                PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
                        } while (outermost != parent);
                    }
                }
                var type = PrefabUtility.GetPrefabAssetType(item.prefab);
                GameObject obj = type == PrefabAssetType.NotAPrefab ? GameObject.Instantiate(item.prefab)
                    : (GameObject)PrefabUtility.InstantiatePrefab(PrefabUtility.IsPartOfPrefabAsset(item.prefab)
                    ? item.prefab : PrefabUtility.GetCorrespondingObjectFromSource(item.prefab));
                Undo.RegisterCreatedObjectUndo(obj, COMMAND_NAME);
                obj.transform.SetPositionAndRotation(item.position, item.rotation);
                obj.transform.localScale = item.scale;
                var root = PrefabUtility.GetOutermostPrefabInstanceRoot(obj);
                if (root != null) root.transform.parent = item.parent;
                else obj.transform.parent = item.parent;
                PWBCore.AddTempCollider(obj);
                AddPaintedObject(obj);
                BrushstrokeManager.UpdateBrushstroke();                
                var tempColliders = PWBCore.GetTempColliders(target);
                if (tempColliders != null) foreach (var tempCollider in tempColliders) Undo.DestroyObjectImmediate(tempCollider);
                Undo.DestroyObjectImmediate(target);
            }
            _paintStroke.Clear();
            _toReplace.Clear();
        }

        public static void ReplaceAllSelected()
        {
            _replaceAllSelected = true;
        }

        private static void DrawReplacerCircle(Vector3 center, Ray mouseRay, Camera camera)
        {
            var settings = ReplacerManager.settings;
            const float polygonSideSize = 0.3f;
            const int minPolygonSides = 8;
            const int maxPolygonSides = 60;
            var polygonSides = Mathf.Clamp((int)(TAU * settings.radius / polygonSideSize),
                minPolygonSides, maxPolygonSides);

            var periPoints = new List<Vector3>();
            for (int i = 0; i < polygonSides; ++i)
            {
                var radians = TAU * i / (polygonSides - 1f);
                var tangentDir = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
                var worldDir = TangentSpaceToWorld(camera.transform.right, camera.transform.up, tangentDir);
                periPoints.Add(center + (worldDir * settings.radius));
            }
            Handles.zTest = CompareFunction.Always;
            Handles.color = new Color(0f, 0f, 0f, 1f);
            Handles.DrawAAPolyLine(6, periPoints.ToArray());
            Handles.color = new Color(1f, 1f, 1f, 0.6f);
            Handles.DrawAAPolyLine(4, periPoints.ToArray());

            if (_octree == null) UpdateOctree();
            var nearbyObjects = _octree.GetNearby(mouseRay, settings.radius);

            _toReplace.Clear();
            _paintStroke.Clear();
            if (settings.outermostPrefabFilter)
            {
                foreach (var nearby in nearbyObjects)
                {

                    var outermost = PrefabUtility.GetOutermostPrefabInstanceRoot(nearby);
                    if (outermost == null) _toReplace.Add(nearby);
                    else if (!_toReplace.Contains(outermost)) _toReplace.Add(outermost);
                }
            }
            else _toReplace.AddRange(nearbyObjects);

            var toReplace = _toReplace.ToArray();
            _toReplace.Clear();
            for (int i = 0; i < toReplace.Length; ++i)
            {
                var obj = toReplace[i];
                if (obj == null) continue;
                var magnitude = BoundsUtils.GetAverageMagnitude(obj.transform);
                if (settings.radius < magnitude / 2) continue;
                _toReplace.Add(obj);
            }

            foreach (var renderer in _replaceRenderers) renderer.enabled = true;
            _replaceRenderers.Clear();
            toReplace = _toReplace.ToArray();
            _toReplace.Clear();
            for (int i = 0; i < toReplace.Length; ++i)
            {
                var obj = toReplace[i];
                var isChild = false;
                foreach (var listed in toReplace)
                {
                    if (obj.transform.IsChildOf(listed.transform) && listed != obj)
                    {
                        isChild = true;
                        break;
                    }
                }
                if (isChild) continue;
                _toReplace.Add(obj);
                _replaceRenderers.AddRange(obj.GetComponentsInChildren<Renderer>().Where(r => r.enabled == true));
                foreach (var renderer in _replaceRenderers) renderer.enabled = false;
                ReplacePreview(camera, obj.transform);
            }

        }
    }

    #endregion
}