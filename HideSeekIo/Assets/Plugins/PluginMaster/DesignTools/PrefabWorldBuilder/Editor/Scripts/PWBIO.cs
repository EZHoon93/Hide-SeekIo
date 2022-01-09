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
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace PluginMaster
{
    [InitializeOnLoad]
    public static partial class PWBIO
    {
        #region SELECTION
        static PWBIO()
        {
            SelectionManager.selectionChanged += UpdateSelection;
            Undo.undoRedoPerformed += UpdateOctree;
            SceneView.duringSceneGui += DuringSceneGUI;
        }
        
        public static void UpdateSelection()
        {
            if (SelectionManager.topLevelSelection.Length == 0)
            {
                if (tool == ToolManager.PaintTool.EXTRUDE)
                {
                    _initialExtrudePosition = _extrudeHandlePosition = _selectionSize = Vector3.zero;
                    _extrudeDirection = Vector3Int.zero;
                }
                return;
            }
            if (tool == ToolManager.PaintTool.EXTRUDE)
            {
                var selectionBounds = ExtrudeManager.settings.space == Space.World
                    ? BoundsUtils.GetSelectionBounds(SelectionManager.topLevelSelection)
                    : BoundsUtils.GetSelectionBounds(SelectionManager.topLevelSelection,
                    ExtrudeManager.settings.rotationAccordingTo == ExtrudeSettings.RotationAccordingTo.FRIST_SELECTED
                    ? SelectionManager.topLevelSelection.First().transform.rotation
                    : SelectionManager.topLevelSelection.Last().transform.rotation);
                _initialExtrudePosition = _extrudeHandlePosition = selectionBounds.center;
                _selectionSize = selectionBounds.size;
                _extrudeDirection = Vector3Int.zero;
            }
            else if (tool == ToolManager.PaintTool.SELECTION)
            {
                _selectedBoxPointIdx = 10;
                _selectionRotation = Quaternion.identity;
                _selectionChanged = true;
                _editingSelectionHandlePosition = false;
                var rotation = GetSelectionRotation();
                _selectionBounds = BoundsUtils.GetSelectionBounds(SelectionManager.topLevelSelection, rotation);
                _selectionRotation = rotation;
            }
        }
        #endregion

        #region COMMON
        private const float TAU = Mathf.PI * 2;
        private static int _controlId;
        public static int controlId {set => _controlId = value; }
        private static ToolManager.PaintTool tool => ToolManager.tool;

        private static Tool _unityCurrentTool = Tool.None;

        private static Camera _sceneViewCamera = null;

        public static bool repaint { get; set; }
        public static void SaveUnityCurrentTool() => _unityCurrentTool = Tools.current;

        public static void DuringSceneGUI(SceneView sceneView)
        {
            if (sceneView.in2DMode)
            {
                SnapManager.settings.gridOnZ = true;
                PWBToolbar.RepaintWindow();
            }
            if (repaint)
            {
                if (tool == ToolManager.PaintTool.SHAPE) BrushstrokeManager.UpdateShapeBrushstroke();
                sceneView.Repaint();
                repaint = false;
            }

            PaletteInput(sceneView);

            _sceneViewCamera = sceneView.camera;

            GridDuringSceneGui(sceneView);

            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape
                && (tool == ToolManager.PaintTool.PIN || tool == ToolManager.PaintTool.BRUSH
                || tool == ToolManager.PaintTool.GRAVITY || tool == ToolManager.PaintTool.ERASER
                || tool == ToolManager.PaintTool.REPLACER))
                ToolManager.DeselectTool();

            if (ToolManager.tool == ToolManager.PaintTool.NONE)
            {
                if (PaletteManager.pickingBrushes) HandleUtility.AddDefaultControl(_controlId);
                return;
            }
            if (PaletteManager.selectedBrushIdx == -1 && (tool == ToolManager.PaintTool.PIN || tool == ToolManager.PaintTool.BRUSH
                || tool == ToolManager.PaintTool.GRAVITY || tool == ToolManager.PaintTool.LINE || tool == ToolManager.PaintTool.SHAPE))
                return;
            if (Event.current.type == EventType.MouseEnterWindow) _pinned = false;

            if (Event.current.type == EventType.MouseMove || Event.current.type == EventType.MouseDrag)
            {
                sceneView.Focus();
                sceneView.Repaint();
            }
            else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.V)
                _snapToVertex = true;
            else if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.V)
                _snapToVertex = false;

            switch (tool)
            {
                case ToolManager.PaintTool.PIN:
                    PinDuringSceneGUI(sceneView);
                    break;
                case ToolManager.PaintTool.BRUSH:
                    BrushDuringSceneGUI(sceneView);
                    break;
                case ToolManager.PaintTool.GRAVITY:
                    GravityToolDuringSceneGUI(sceneView);
                    break;
                case ToolManager.PaintTool.ERASER:
                    EraserDuringSceneGUI(sceneView);
                    break;
                case ToolManager.PaintTool.EXTRUDE:
                    ExtrudeDuringSceneGUI(sceneView);
                    break;
                case ToolManager.PaintTool.LINE:
                    LineDuringSceneGUI(sceneView);
                    break;
                case ToolManager.PaintTool.SHAPE:
                    ShapeDuringSceneGUI(sceneView);
                    break;
                case ToolManager.PaintTool.TILING:
                    TilingDuringSceneGUI(sceneView);
                    break;
                case ToolManager.PaintTool.SELECTION:
                    SelectionDuringSceneGUI(sceneView);
                    break;
                case ToolManager.PaintTool.MIRROR:
                    MirrorDuringSceneGUI(sceneView);
                    break;
                case ToolManager.PaintTool.REPLACER:
                    ReplacerDuringSceneGUI(sceneView);
                    break;
            }

            if ((tool != ToolManager.PaintTool.EXTRUDE && tool != ToolManager.PaintTool.SELECTION && tool != ToolManager.PaintTool.MIRROR)
                && Event.current.type == EventType.Layout) HandleUtility.AddDefaultControl(_controlId);
        }

        private static float UpdateRadius(float radius)
            => Mathf.Max(radius * (1f + Mathf.Sign(Event.current.delta.y) * 0.05f), 0.05f);
        private static Vector3 TangentSpaceToWorld(Vector3 tangent, Vector3 bitangent, Vector2 tangentSpacePos)
            => (tangent * tangentSpacePos.x + bitangent * tangentSpacePos.y);

        private static void UpdateStrokeDirection(Vector3 hitPoint)
        {
            var dir = hitPoint - _prevMousePos;
            if (dir.sqrMagnitude > 0.3f)
            {
                _strokeDirection = hitPoint - _prevMousePos;
                _prevMousePos = hitPoint;
            }
        }

        public static void ResetUnityCurrentTool() => Tools.current = _unityCurrentTool;

        private static bool MouseDot(out Vector3 point, out Vector3 normal,
            PaintOnSurfaceToolSettings.PaintMode mode, bool in2DMode,
            bool paintOnPalettePrefabs, bool castOnMeshesWithoutCollider, bool snapOnGrid)
        {
            point = Vector3.zero;
            normal = Vector3.up;
            var mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector3 SnapPoint(Vector3 hitPoint, ref Vector3 snapNormal)
            {
                if (_snapToVertex)
                {
                    if (SnapToVertex(mouseRay, out RaycastHit snappedHit, in2DMode))
                    {
                        _snappedToVertex = true;
                        hitPoint = snappedHit.point;
                        snapNormal = snappedHit.normal;
                    }
                }
                else if (SnapManager.settings.snappingEnabled)
                {
                    hitPoint = SnapPosition(hitPoint, snapOnGrid, true);
                    mouseRay.origin = hitPoint - mouseRay.direction;
                    if (Physics.Raycast(mouseRay, out RaycastHit hitInfo)) snapNormal = hitInfo.normal;
                    else if (RaycastMesh.Raycast(mouseRay, out RaycastHit meshHitInfo, out GameObject c,
                        _octree.GetNearby(mouseRay, 1), float.MaxValue)) snapNormal = meshHitInfo.normal;
                }
                return hitPoint;
            }

            RaycastHit surfaceHit;
            bool surfaceFound = MouseRaycast(mouseRay, out surfaceHit, out GameObject collider,
                float.MaxValue, -1, paintOnPalettePrefabs, castOnMeshesWithoutCollider);
            if (mode != PaintOnSurfaceToolSettings.PaintMode.ON_SHAPE && surfaceFound)
            {
                normal = surfaceHit.normal;
                point = SnapPoint(surfaceHit.point, ref normal);
                return true;
            }
            if (mode != PaintOnSurfaceToolSettings.PaintMode.ON_SURFACE)
            {
                if (surfaceFound)
                {
                    point = SnapPoint(surfaceHit.point, ref normal);
                    var direction = SnapManager.settings.rotation * Vector3.down;
                    var ray = new Ray(point - direction, direction);
                    if (MouseRaycast(ray, out RaycastHit hitInfo, out collider, float.MaxValue, -1,
                        paintOnPalettePrefabs, castOnMeshesWithoutCollider)) point = hitInfo.point;
                    UpdateGridOrigin(point);
                    return true;
                }
                if (GridRaycast(mouseRay, out RaycastHit gridHit))
                {
                    point = SnapPoint(gridHit.point, ref normal);
                    return true;
                }
            }
            return false;
        }

        private static bool _updateStroke = false;
        public static void UpdateStroke()
        {
            _updateStroke = true;
            SceneView.RepaintAll();
        }
        #endregion

        #region OCTREE
        private const float MIN_OCTREE_NODE_SIZE = 0.5f;
        private static PointOctree _octree = new PointOctree(10, Vector3.zero, MIN_OCTREE_NODE_SIZE);
        private static List<GameObject> _paintedObjects = new List<GameObject>();
        public static void UpdateOctree()
        {
            if (PaletteManager.paletteCount == 0) return;
            if ((tool == ToolManager.PaintTool.PIN || tool == ToolManager.PaintTool.BRUSH || tool == ToolManager.PaintTool.GRAVITY
                || tool == ToolManager.PaintTool.LINE || tool == ToolManager.PaintTool.SHAPE || tool == ToolManager.PaintTool.TILING)
                && PaletteManager.selectedBrushIdx < 0) return;

            var allObjects = GameObject.FindObjectsOfType<GameObject>();
            _octree = null;
            _paintedObjects.Clear();
            var allPrefabsPaths = new List<string>();
            bool AddPrefabPath(MultibrushItemSettings item)
            {
                if (item.prefab == null) return false;
                var path = AssetDatabase.GetAssetPath(item.prefab);
                if (allPrefabsPaths.Contains(path)) return false;
                allPrefabsPaths.Add(path);
                return true;
            }
            if (tool == ToolManager.PaintTool.ERASER || tool == ToolManager.PaintTool.REPLACER)
            {
                IModifierTool settings = EraserManager.settings;
                if (tool == ToolManager.PaintTool.REPLACER) settings = ReplacerManager.settings;
                if (settings.command == ModifierToolSettings.Command.MODIFY_PALETTE_PREFABS)
                    foreach (var brush in PaletteManager.selectedPalette.brushes)
                        foreach (var item in brush.items) AddPrefabPath(item);
                else if (PaletteManager.selectedBrush != null
                    && settings.command == ModifierToolSettings.Command.MODIFY_BRUSH_PREFABS)
                    foreach (var item in PaletteManager.selectedBrush.items) AddPrefabPath(item);
                SelectionManager.UpdateSelection();
                bool modifyAll = settings.command == ModifierToolSettings.Command.MODIFY_ALL;
                bool modifyAllButSelected = settings.modifyAllButSelected;
                foreach (var obj in allObjects)
                {
                    if (!obj.activeInHierarchy) continue;
                    if (!modifyAll && !PrefabUtility.IsAnyPrefabInstanceRoot(obj)) continue;
                    var prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
                    bool isBrush = allPrefabsPaths.Contains(prefabPath);
                    if (!isBrush && !modifyAll) continue;
                    if (modifyAllButSelected && SelectionManager.selection.Contains(obj)) continue;
                    AddPaintedObject(obj);
                }
            }
            else
            {
                foreach (var brush in PaletteManager.selectedPalette.brushes)
                    foreach (var item in brush.items) AddPrefabPath(item);
                foreach (var obj in allObjects)
                {
                    if (!obj.activeInHierarchy) continue;
                    if (!PrefabUtility.IsAnyPrefabInstanceRoot(obj)) continue;
                    var prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
                    bool isBrush = allPrefabsPaths.Contains(prefabPath);
                    if (isBrush) AddPaintedObject(obj);
                }
            }
            if (_octree == null) _octree = new PointOctree(10, Vector3.zero, MIN_OCTREE_NODE_SIZE);
        }

        private static void AddPaintedObject(GameObject obj)
        {
            if (_octree == null) _octree = new PointOctree(10, obj.transform.position, MIN_OCTREE_NODE_SIZE);
            _octree.Add(obj);
            _paintedObjects.Add(obj);
        }

        public static bool OctreeContains(int objId) => _octree.Contains(objId);
        #endregion

        #region STROKE & PAINT
        private const string PWB_OBJ_NAME = "Prefab World Builder";
        private static Vector3 _prevMousePos = Vector3.zero;
        private static Vector3 _strokeDirection = Vector3.forward;
        private static Transform _autoParent = null;
        private static Dictionary<string, Transform> _subParents = new Dictionary<string, Transform>();
        private static Mesh quadMesh;

        private class PaintStrokeItem
        {
            public readonly GameObject prefab = null;
            public readonly Vector3 position = Vector3.zero;
            public readonly Quaternion rotation = Quaternion.identity;
            public readonly Vector3 scale = Vector3.one;
            public readonly int layer = 0;
            private Transform _parent = null;
            public Transform parent { get => _parent; set => _parent = value; }

            public PaintStrokeItem(GameObject prefab, Vector3 position, Quaternion rotation,
                Vector3 scale, int layer, Transform parent)
            {
                this.prefab = prefab;
                this.position = position;
                this.rotation = rotation;
                this.scale = scale;
                this.layer = layer;
                _parent = parent;
            }
        }
        private static List<PaintStrokeItem> _paintStroke = new List<PaintStrokeItem>();

        private static void BrushstrokeMouseEvents(BrushToolBase settings)
        {
            if (PaletteManager.selectedBrush == null) return;
            if (Event.current.button == 0 && !Event.current.alt && Event.current.type == EventType.MouseUp
                && PaletteManager.selectedBrush.patternMachine != null
                && PaletteManager.selectedBrush.restartPatternForEachStroke)
            {
                PaletteManager.selectedBrush.patternMachine.Reset();
                BrushstrokeManager.UpdateBrushstroke();
            }
            else if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Space)
            {
                BrushstrokeManager.UpdateBrushstroke();
                repaint = true;
            }
            else if (Event.current.type == EventType.ScrollWheel && Event.current.control && Event.current.alt
                && settings.brushShape != BrushToolSettings.BrushShape.POINT)
            {
                settings.density += (int)Mathf.Sign(Event.current.delta.y);
                ToolProperties.RepainWindow();
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
                else if (Event.current.type == EventType.MouseUp && !Event.current.control && !Event.current.shift)
                    _pinned = false;
                if (Event.current.type == EventType.MouseDrag && Event.current.control && !Event.current.shift)
                {
                    var delta = _pinMouse.x - Event.current.mousePosition.x;
                    _brushAngle = delta * 1.8f; //180deg/100px
                    Event.current.Use();
                }
                else if (Event.current.type == EventType.MouseDrag && !Event.current.control && Event.current.shift)
                {
                    var delta = Mathf.Sign(Event.current.delta.x);
                    settings.radius = Mathf.Max(settings.radius * (1f + delta * 0.03f), 0.05f);
                    if (settings is BrushToolSettings)
                    {
                        if (BrushManager.settings.heightType == BrushToolSettings.HeightType.RADIUS)
                            BrushManager.settings.maxHeightFromCenter = BrushManager.settings.radius;
                    }
                    ToolProperties.RepainWindow();
                    Event.current.Use();
                }
            }
            if ((Event.current.keyCode == KeyCode.LeftControl || Event.current.keyCode == KeyCode.RightControl
                || Event.current.keyCode == KeyCode.RightShift || Event.current.keyCode == KeyCode.LeftShift)
                && Event.current.type == EventType.KeyUp) _pinned = false;
        }

        private static void PreviewBrushItem(GameObject prefab, Matrix4x4 rootToWorld, int layer,
            Camera camera, bool redMaterial = false, bool reverseTriangles = false)
        {
            var meshesAndRenderers = new List<(Mesh mesh, Renderer renderer)>();
            var filters = prefab.GetComponentsInChildren<MeshFilter>();
            foreach (var filter in filters)
            {
                var mesh = filter.sharedMesh;
                if (mesh == null) continue;
                var renderer = filter.GetComponent<MeshRenderer>();
                if (renderer == null) continue;
                meshesAndRenderers.Add((mesh, renderer));
            }
            var skinedMeshRenderers = prefab.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in skinedMeshRenderers)
            {
                var mesh = renderer.sharedMesh;
                if (mesh == null) continue;
                meshesAndRenderers.Add((mesh, renderer));
            }
            foreach (var item in meshesAndRenderers)
            {
                var mesh = item.mesh;
                var childToWorld = rootToWorld * item.renderer.transform.localToWorldMatrix;
                if (!redMaterial)
                {
                    var materials = item.renderer.sharedMaterials;
                    if (materials == null && materials.Length > 0 && materials.Length >= mesh.subMeshCount) continue;
                    for (int subMeshIdx = 0; subMeshIdx < mesh.subMeshCount; ++subMeshIdx)
                    {
                        var material = materials[subMeshIdx];
                        if (reverseTriangles)
                        {
                            var tempMesh = (Mesh)GameObject.Instantiate(mesh);
                            tempMesh.triangles = mesh.triangles.Reverse().ToArray();
                            tempMesh.subMeshCount = mesh.subMeshCount;
                            int vCount = 0;
                            for (int i = 0; i < mesh.subMeshCount; ++i)
                            {
                                var desc = mesh.GetSubMesh(mesh.subMeshCount - i - 1);
                                desc.indexStart = vCount;
                                tempMesh.SetSubMesh(i, desc);
                                vCount += desc.indexCount;
                            }
                            material = materials[mesh.subMeshCount - subMeshIdx - 1];
                            Graphics.DrawMesh(tempMesh, childToWorld, material, layer, camera, subMeshIdx);
                            tempMesh = null;
                        }
                        else Graphics.DrawMesh(mesh, childToWorld, material, layer, camera, subMeshIdx);
                    }
                }
                else
                {
                    for (int subMeshIdx = 0; subMeshIdx < mesh.subMeshCount; ++subMeshIdx)
                        Graphics.DrawMesh(mesh, childToWorld, transparentRedMaterial, layer, camera, subMeshIdx);
                }
            }
            var SpriteRenderers = prefab.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in SpriteRenderers) DrawSprite(spriteRenderer, rootToWorld, camera);
        }

        private static void DrawSprite(SpriteRenderer renderer, Matrix4x4 matrix, Camera camera)
        {
            if (quadMesh == null)
            {
                quadMesh = new Mesh
                {
                    vertices = new[] { new Vector3(-.5f, .5f, 0), new Vector3(.5f, .5f, 0),
                        new Vector3(-.5f, -.5f, 0), new Vector3(.5f, -.5f, 0) },
                    normals = new[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward },
                    triangles = new[] { 0, 2, 3, 3, 1, 0 }
                };
            }
            quadMesh.uv = renderer.sprite.uv;

            var mpb = new MaterialPropertyBlock();
            mpb.SetTexture("_MainTex", renderer.sprite.texture);
            mpb.SetColor("_Color", renderer.color);

            matrix *= renderer.transform.localToWorldMatrix;
            matrix *= Matrix4x4.Scale(new Vector3(
                renderer.sprite.textureRect.width / renderer.sprite.pixelsPerUnit,
                renderer.sprite.textureRect.height / renderer.sprite.pixelsPerUnit, 1));

            Graphics.DrawMesh(quadMesh, matrix, renderer.sharedMaterial, 0, camera, 0, mpb);
        }

        private static GameObject[] Paint(IPaintToolSettings settings, string commandName = "Paint")
        {
            var paintedObjects = new List<GameObject>();
            if (_paintStroke.Count == 0)
            {
                if (BrushstrokeManager.brushstroke.Length == 0) BrushstrokeManager.UpdateBrushstroke();
                return paintedObjects.ToArray();
            }

            foreach (var item in _paintStroke)
            {
                if (item.prefab == null) continue;
                var type = PrefabUtility.GetPrefabAssetType(item.prefab);
                GameObject obj = type == PrefabAssetType.NotAPrefab ? GameObject.Instantiate(item.prefab)
                    : (GameObject)PrefabUtility.InstantiatePrefab(PrefabUtility.IsPartOfPrefabAsset(item.prefab)
                    ? item.prefab : PrefabUtility.GetCorrespondingObjectFromSource(item.prefab));
                if (settings.overwritePrefabLayer) obj.layer = settings.layer;
                obj.transform.SetPositionAndRotation(item.position, item.rotation);
                obj.transform.localScale = item.scale;
                var root = PrefabUtility.GetOutermostPrefabInstanceRoot(obj);
                if(item.parent == null && settings.autoCreateParent)
                {
                    item.parent = GetParent(settings, item.prefab.name, true);
                }
                if (root != null) root.transform.parent = item.parent;
                else obj.transform.parent = item.parent;
                PWBCore.AddTempCollider(obj);
                paintedObjects.Add(obj);
                AddPaintedObject(obj);
                Undo.RegisterCreatedObjectUndo(obj, commandName);
            }
            if (_paintStroke.Count > 0) BrushstrokeManager.UpdateBrushstroke();
            _paintStroke.Clear();
            return paintedObjects.ToArray();
        }

        public static void ResetAutoParent() => _autoParent = null;
        
        private static Transform GetParent(IPaintToolSettings settings, string subParentName, bool create)
        {
            if (settings.autoCreateParent && create)
            {
                if (_autoParent == null)
                {
                    var pwbObj = GameObject.Find(PWB_OBJ_NAME);
                    if (pwbObj == null) _autoParent = new GameObject(PWB_OBJ_NAME).transform;
                    else _autoParent = pwbObj.transform;
                }
                if(settings.createSubparent)
                {
                    create = false;
                    if (!_subParents.ContainsKey(subParentName))
                    {
                        _subParents.Add(subParentName, null);
                        create = true;
                    }
                    else if (_subParents[subParentName] == null) create = true;
                    if(create)
                    {
                        GameObject obj = null;
                        if(_autoParent.childCount > 0)
                        {
                            for(int i = 0; i < _autoParent.childCount; ++i)
                            {
                                var child = _autoParent.GetChild(i);
                                if (child.name != subParentName) continue;
                                var componentCount = child.GetComponents<Component>().Length;
                                if (componentCount > 1) continue;
                                obj = child.gameObject;
                                break;
                            }
                        }
                        if (obj == null)
                        {
                            obj = new GameObject(subParentName);
                            var subParent = obj.transform;
                            subParent.parent = _autoParent;
                            subParent.localPosition = Vector3.zero;
                            subParent.localRotation = Quaternion.identity;
                            subParent.localScale = Vector3.one;
                        }
                        _subParents[subParentName] = obj.transform;
                    }
                    return _subParents[subParentName];
                }
                return _autoParent;
            }
            return settings.parent;
        }

        private static bool IsVisible(ref GameObject obj)
        {
            if (obj == null) return false;
            var parentRenderer = obj.GetComponentInParent<Renderer>();
            var parentTerrain = obj.GetComponentInParent<Terrain>();
            if (parentRenderer != null) obj = parentRenderer.gameObject;
            else if (parentTerrain != null) obj = parentTerrain.gameObject;
            else
            {
                var parent = obj.transform.parent;
                if (parent != null)
                {
                    var siblingRenderer = parent.GetComponentInChildren<Renderer>();
                    var siblingTerrain = parent.GetComponentInChildren<Terrain>();
                    if (siblingRenderer != null) obj = parent.gameObject;
                    else if (siblingTerrain != null) obj = parent.gameObject;

                }
            }
            var renderers = obj.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                foreach (var renderer in renderers)
                    if (renderer.enabled) return true;
            }
            var terrains = obj.GetComponentsInChildren<Terrain>();
            if (terrains.Length > 0)
            {
                foreach (var terrain in terrains)
                    if (terrain.enabled) return true;
            }
            return false;
        }

        private static bool MouseRaycast(Ray mouseRay, out RaycastHit mouseHit,
            out GameObject collider, float maxDistance, LayerMask layerMask,
            bool paintOnPalettePrefabs, bool castOnMeshesWithoutCollider, string[] tags = null,
            GameObject[] invisibleExeptions = null)
        {
            mouseHit = new RaycastHit();
            collider = null;
            bool validHit = Physics.Raycast(mouseRay, out mouseHit, maxDistance, layerMask);
            bool physicsHit = validHit;
            if (validHit && !castOnMeshesWithoutCollider)
            {
                var obj = mouseHit.collider.gameObject;
                var hitParent = obj.transform.parent;
                if (hitParent != null && hitParent.gameObject.GetInstanceID() == PWBCore.parentColliderId)
                    physicsHit = false;
            }

            GameObject[] nearbyObjects = null;
            if (!physicsHit)
            {
                if (!castOnMeshesWithoutCollider) return false;
                nearbyObjects = _octree.GetNearby(mouseRay, 1f);
                validHit = RaycastMesh.Raycast(mouseRay, out mouseHit, out collider, nearbyObjects, maxDistance);
            }
            else collider = mouseHit.collider.gameObject;
            if (validHit)
            {
                RaycastHit[] hitArray = null;
                GameObject[] colliders = null;
                if (physicsHit) hitArray = Physics.RaycastAll(mouseRay, maxDistance, layerMask);
                else RaycastMesh.RaycastAll(mouseRay, out hitArray, out colliders, nearbyObjects, maxDistance);
                validHit = false;
                var minDistance = float.MaxValue;
                for (int i = 0; i < hitArray.Length; ++i)
                {
                    var obj = physicsHit ? hitArray[i].collider.gameObject : colliders[i];
                    if (tags != null) if (!tags.Contains(obj.tag)) continue;
                    var hitParent = obj.transform.parent;
                    if (hitParent != null && hitParent.gameObject.GetInstanceID() == PWBCore.parentColliderId)
                        obj = PWBCore.GetGameObjectFromTempColliderId(obj.GetInstanceID());

                    if (!paintOnPalettePrefabs && PaletteManager.selectedPalette.ContainsSceneObject(obj)) continue;
                    var checkInvisibility = invisibleExeptions == null || !invisibleExeptions.Contains(obj);
                    if(checkInvisibility) if (!IsVisible(ref obj)) continue;

                    if (hitArray[i].distance < minDistance)
                    {
                        minDistance = hitArray[i].distance;
                        validHit = true;
                        mouseHit = hitArray[i];
                        collider = obj;
                        continue;
                    }
                }
            }
            return validHit;
        }

        public static float GetBottomDistanceToSurface(Vector3[] bottomVertices, Matrix4x4 TRS,
            float maxDistance, bool paintOnPalettePrefabs, bool castOnMeshesWithoutCollider)
        {
            float distance = 0f;
            var euler = TRS.rotation.eulerAngles;
            var down = TRS.rotation * Vector3.down;
            foreach (var vertex in bottomVertices)
            {
                var origin = TRS.MultiplyPoint(vertex);
                var ray = new Ray(origin - down * maxDistance, down);
                if (MouseRaycast(ray, out RaycastHit hitInfo, out GameObject collider,
                    maxDistance * 2, -1, paintOnPalettePrefabs, castOnMeshesWithoutCollider))
                {
                    float negDistance = 0f;
                    if (hitInfo.distance < maxDistance) negDistance = maxDistance - hitInfo.distance;
                    distance = Mathf.Max(hitInfo.distance - maxDistance + negDistance, distance);
                }
            }
            return distance;
        }

        public static float GetBottomDistanceToSurfaceSigned(Vector3[] bottomVertices, Matrix4x4 TRS,
            float maxDistance, bool paintOnPalettePrefabs, bool castOnMeshesWithoutCollider)
        {
            float distance = 0f;
            var down = Vector3.down;
            foreach (var vertex in bottomVertices)
            {
                var origin = TRS.MultiplyPoint(vertex);
                var ray = new Ray(origin - down * maxDistance, down);
                if (MouseRaycast(ray, out RaycastHit hitInfo, out GameObject collider,
                    float.MaxValue, -1, paintOnPalettePrefabs, castOnMeshesWithoutCollider))
                {
                    var d = hitInfo.distance - maxDistance;
                    if (Mathf.Abs(d) > Mathf.Abs(distance)) distance = d;
                }
            }
            return distance;
        }

        public static float GetPivotDistanceToSurfaceSigned(Vector3 pivot,
            float maxDistance, bool paintOnPalettePrefabs, bool castOnMeshesWithoutCollider)
        {
            var ray = new Ray(pivot + Vector3.up * maxDistance, Vector3.down);
            if (MouseRaycast(ray, out RaycastHit hitInfo, out GameObject collider,
                    float.MaxValue, -1, paintOnPalettePrefabs, castOnMeshesWithoutCollider))
                return hitInfo.distance - maxDistance;
            return 0;
        }
        #endregion

        #region BRUSH SHAPE INDICATOR
        private static void DrawCricleIndicator(Vector3 hitPoint, Vector3 hitNormal,
            float radius, float height, Vector3 tangent, Vector3 bitangent,
            Vector3 normal, bool paintOnPalettePrefabs, bool castOnMeshesWithoutCollider,
            int layerMask = -1, string[] tags = null)
        {
            Handles.zTest = CompareFunction.Always;
            const float normalOffset = 0.01f;
            const float polygonSideSize = 0.3f;
            const int minPolygonSides = 12;
            const int maxPolygonSides = 60;
            var polygonSides = Mathf.Clamp((int)(TAU * radius / polygonSideSize), minPolygonSides, maxPolygonSides);

            Handles.color = new Color(0f, 0f, 0f, 0.5f);
            var periPoints = new List<Vector3>();
            var periPointsShadow = new List<Vector3>();

            for (int i = 0; i < polygonSides; ++i)
            {
                var radians = TAU * i / (polygonSides - 1f);
                var tangentDir = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
                var worldDir = TangentSpaceToWorld(tangent, bitangent, tangentDir);
                var periPoint = hitPoint + (worldDir * (radius));
                var periRay = new Ray(periPoint + normal * height, -normal);
                if (MouseRaycast(periRay, out RaycastHit periHit, out GameObject collider,
                    height * 2, layerMask, paintOnPalettePrefabs, castOnMeshesWithoutCollider, tags))
                {
                    var periHitPoint = periHit.point + hitNormal * normalOffset;
                    var shadowPoint = periHitPoint + worldDir * 0.2f;
                    periPoints.Add(periHitPoint);
                    periPointsShadow.Add(shadowPoint);
                }
                else
                {
                    if (periPoints.Count > 0 && i == polygonSides - 1)
                    {
                        periPoints.Add(periPoints[0]);
                        periPointsShadow.Add(periPointsShadow[0]);
                    }
                    else
                    {
                        float binSearchRadius = radius;
                        float delta = -binSearchRadius / 2;

                        for (int j = 0; j < 8; ++j)
                        {
                            binSearchRadius += delta;
                            periPoint = hitPoint + (worldDir * binSearchRadius);
                            periRay = new Ray(periPoint + normal * height, -normal);
                            if (MouseRaycast(periRay, out RaycastHit binSearchPeriHit,
                                out GameObject binSearchCollider, height * 2, layerMask,
                                paintOnPalettePrefabs, castOnMeshesWithoutCollider, tags))
                            {
                                delta = Mathf.Abs(delta) / 2;
                                periHit = binSearchPeriHit;
                            }
                            else delta = -Mathf.Abs(delta) / 2;
                            if (Mathf.Abs(delta) < 0.01) break;
                        }
                        if (periHit.point == Vector3.zero)
                            continue;
                        var periHitPoint = periHit.point + hitNormal * normalOffset;
                        var shadowPoint = periHitPoint + worldDir * 0.2f;
                        periPoints.Add(periHitPoint);
                        periPointsShadow.Add(shadowPoint);
                    }
                }
            }
            if (periPoints.Count > 0)
            {
                Handles.color = new Color(1f, 1f, 1f, 0.5f);
                Handles.DrawAAPolyLine(3, periPoints.ToArray());
                Handles.color = new Color(0f, 0f, 0f, 0.5f);
                Handles.DrawAAPolyLine(6, periPointsShadow.ToArray());
            }
            else
            {
                Handles.color = new Color(1f, 1f, 1f, 0.5f);
                Handles.DrawWireDisc(hitPoint + hitNormal * normalOffset, hitNormal, radius);
                Handles.color = new Color(0f, 0f, 0f, 0.5f);
                Handles.DrawWireDisc(hitPoint + hitNormal * normalOffset, hitNormal, radius + 0.2f);
            }
        }

        private static void DrawSquareIndicator(Vector3 hitPoint, Vector3 hitNormal,
            float radius, float height, Vector3 tangent, Vector3 bitangent,
            Vector3 normal, bool paintOnPalettePrefabs, bool castOnMeshesWithoutCollider,
            int layerMask = -1, string[] tags = null)
        {
            Handles.zTest = CompareFunction.Always;
            const float normalOffset = 0.01f;

            const int minSideSegments = 4;
            const int maxSideSegments = 15;
            var segmentsPerSide = Mathf.Clamp((int)(radius * 2 / 0.3f), minSideSegments, maxSideSegments);
            var segmentCount = segmentsPerSide * 4;
            float segmentSize = radius * 2f / segmentsPerSide;
            float SQRT2 = Mathf.Sqrt(2f);
            Handles.color = new Color(0f, 0f, 0f, 0.5f);
            var periPoints = new List<Vector3>();

            for (int i = 0; i < segmentCount; ++i)
            {
                int sideIdx = i / segmentsPerSide;
                int segmentIdx = i % segmentsPerSide;
                var periPoint = hitPoint;
                if (sideIdx == 0) periPoint += tangent * (segmentSize * segmentIdx - radius) + bitangent * radius;
                else if (sideIdx == 1) periPoint += bitangent * (radius - segmentSize * segmentIdx) + tangent * radius;
                else if (sideIdx == 2) periPoint += tangent * (radius - segmentSize * segmentIdx) - bitangent * radius;
                else periPoint += bitangent * (segmentSize * segmentIdx - radius) - tangent * radius;
                var worldDir = (periPoint - hitPoint).normalized;
                var periRay = new Ray(periPoint + normal * height, -normal);
                if (MouseRaycast(periRay, out RaycastHit periHit, out GameObject collider,
                    height * 2, layerMask, paintOnPalettePrefabs, castOnMeshesWithoutCollider, tags))
                {
                    var periHitPoint = periHit.point + hitNormal * normalOffset;
                    periPoints.Add(periHitPoint);
                }
                else
                {
                    float binSearchRadius = radius * SQRT2;
                    float delta = -binSearchRadius / 2;

                    for (int j = 0; j < 8; ++j)
                    {
                        binSearchRadius += delta;
                        periPoint = hitPoint + (worldDir * binSearchRadius);
                        periRay = new Ray(periPoint + normal * height, -normal);
                        if (MouseRaycast(periRay, out RaycastHit binSearchPeriHit,
                            out GameObject binSearchCollider, height * 2, layerMask,
                            paintOnPalettePrefabs, castOnMeshesWithoutCollider, tags))
                        {
                            delta = Mathf.Abs(delta) / 2;
                            periHit = binSearchPeriHit;
                        }
                        else delta = -Mathf.Abs(delta) / 2;
                        if (Mathf.Abs(delta) < 0.01) break;
                    }
                    if (periHit.point == Vector3.zero)
                        continue;
                    var periHitPoint = periHit.point + hitNormal * normalOffset;
                    var shadowPoint = periHitPoint + worldDir * 0.2f;
                    periPoints.Add(periHitPoint);

                }
            }
            periPoints.Add(periPoints[0]);
            if (periPoints.Count > 0)
            {
                Handles.color = new Color(0f, 0f, 0f, 0.7f);
                Handles.DrawAAPolyLine(8, periPoints.ToArray());

                Handles.color = new Color(1f, 1f, 1f, 0.7f);
                Handles.DrawAAPolyLine(4, periPoints.ToArray());
            }
        }
        #endregion

        #region HANDLES
        private static void DrawDotHandleCap(Vector3 point, float alpha = 1f,
            float scale = 1f, bool selected = false)
        {
            Handles.color = new Color(0f, 0f, 0f, 0.7f * alpha);
            var handleSize = HandleUtility.GetHandleSize(point);
            var sizeDelta = handleSize * 0.0125f;
            Handles.DotHandleCap(0, point, Quaternion.identity, handleSize * 0.0325f * scale, EventType.Repaint);
            var fillColor = selected ? Color.cyan : Handles.preselectionColor;
            fillColor.a *= alpha;
            Handles.color = fillColor;
            Handles.DotHandleCap(0, point, Quaternion.identity,
                handleSize * 0.0325f * scale - sizeDelta, EventType.Repaint);
        }
        #endregion

        #region DRAG AND DROP
        public class SceneDragReceiver : ISceneDragReceiver
        {
            private int _brushID = -1;
            public int brushId { get => _brushID; set => _brushID = value; }
            public void PerformDrag(Event evt) { }
            public void StartDrag() { }
            public void StopDrag() { }
            public DragAndDropVisualMode UpdateDrag(Event evt, EventType eventType)
            {
                PrefabPalette.instance.DeselectAllButThis(_brushID);
                ToolManager.tool = ToolManager.PaintTool.PIN;
                return DragAndDropVisualMode.Generic;
            }
        }
        private static SceneDragReceiver _sceneDragReceiver = new SceneDragReceiver();
        public static SceneDragReceiver sceneDragReceiver => _sceneDragReceiver;
        #endregion

        #region PALETTE
        private static void PaletteInput(SceneView sceneView)
        {
            if (Event.current.isScrollWheel && Event.current.control && Event.current.shift)
            {
                Event.current.Use();
                if (Event.current.delta.y > 0) PaletteManager.SelectNextBrush();
                else PaletteManager.SelectPreviousBrush();
                PrefabPalette.RepainWindow();
                sceneView.Repaint();
                repaint = true;
                AsyncRepaint();
            }
            if(PaletteManager.pickingBrushes)
            {
                if(Event.current.button == 0 && Event.current.type == EventType.MouseDown)
                {
                    var mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    if (MouseRaycast(mouseRay, out RaycastHit mouseHit, out GameObject collider,
                        float.MaxValue, -1, true, true))
                    {
                        var target = collider.gameObject;
                        var outermostPrefab = PrefabUtility.GetOutermostPrefabInstanceRoot(target);
                        if (outermostPrefab != null) target = outermostPrefab;
                        var brushIdx = PaletteManager.selectedPalette.FindBrushIdx(target);
                        PaletteManager.SelectBrush(brushIdx);
                    }
                    Event.current.Use();
                }
            }
        }
        async static void AsyncRepaint()
        {
            await System.Threading.Tasks.Task.Delay(500);
            repaint = true;
        }
        #endregion
    }
}