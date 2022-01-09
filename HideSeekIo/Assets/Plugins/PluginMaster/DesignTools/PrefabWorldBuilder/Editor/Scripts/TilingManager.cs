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
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PluginMaster
{
    #region DATA & SETTINGS
    [Serializable]
    public class TilingSettings : PaintOnSurfaceToolSettings, IPaintToolSettings
    {
        #region TILING SETTINGS

        public enum CellSizeType
        {
            SMALLEST_OBJECT,
            BIGGEST_OBJECT,
            CUSTOM
        }

        [SerializeField] private PaintMode _mode = PaintMode.AUTO;
        [SerializeField] private CellSizeType _cellSizeType = CellSizeType.SMALLEST_OBJECT;
        [SerializeField] private Vector2 _cellSize = Vector2.one;
        [SerializeField] private Quaternion _rotation = Quaternion.identity;
        [SerializeField] private Vector2 _spacing = Vector2.zero;
        [SerializeField] private AxesUtils.SignedAxis _axisAlignedWithNormal = AxesUtils.SignedAxis.UP;
        public PaintMode mode
        {
            get => _mode;
            set
            {
                if (_mode == value) return;
                _mode = value;
                OnDataChanged();
            }
        }
        
        public Quaternion rotation
        {
            get => _rotation;
            set
            {
                if (_rotation == value) return;
                _rotation = value;
                OnDataChanged();
            }
        }
        public CellSizeType cellSizeType
        {
            get => _cellSizeType;
            set
            {
                if (_cellSizeType == value) return;
                _cellSizeType = value;
                UpdateCellSize();
            }
        }
        public Vector2 cellSize
        {
            get => _cellSize;
            set
            {
                if (_cellSize == value) return;
                _cellSize = value;
                OnDataChanged();
            }
        }
        public Vector2 spacing
        {
            get => _spacing;
            set
            {
                if (_spacing == value) return;
                _spacing = value;
                OnDataChanged();
            }
        }
        public AxesUtils.SignedAxis axisAlignedWithNormal
        {
            get => _axisAlignedWithNormal;
            set
            {
                if (_axisAlignedWithNormal == value) return;
                _axisAlignedWithNormal = value;
                UpdateCellSize();
                OnDataChanged();
            }
        }
        public void UpdateCellSize()
        {
            if (ToolManager.tool != ToolManager.PaintTool.TILING) return;
            if (_cellSizeType != CellSizeType.CUSTOM)
            {
                if (PaletteManager.selectedBrush == null) return;
                var cellSize = Vector3.one * (_cellSizeType == CellSizeType.SMALLEST_OBJECT
                    ? float.MaxValue : float.MinValue);
                foreach (var item in PaletteManager.selectedBrush.items)
                {
                    var prefab = item.prefab;
                    if (prefab == null) continue;
                    var scaleMultiplier = _cellSizeType == CellSizeType.SMALLEST_OBJECT
                        ? item.minScaleMultiplier : item.maxScaleMultiplier;
                    var itemSize = Vector3.Scale(BoundsUtils.GetBoundsRecursive(prefab.transform).size,
                        scaleMultiplier);
                    cellSize = _cellSizeType == CellSizeType.SMALLEST_OBJECT
                        ? Vector3.Min(cellSize, itemSize) : Vector3.Max(cellSize, itemSize);
                }
                if (_axisAlignedWithNormal.axis == AxesUtils.Axis.Y) cellSize.y = cellSize.z;
                else if (_axisAlignedWithNormal.axis == AxesUtils.Axis.X)
                {
                    cellSize.x = cellSize.y;
                    cellSize.y = cellSize.z;
                }
                if (cellSize.x == 0) cellSize.x = 0.5f;
                if (cellSize.y == 0) cellSize.y = 0.5f;
                if (cellSize.z == 0) cellSize.z = 0.5f;
                _cellSize = cellSize;
                ToolProperties.RepainWindow();
                UnityEditor.SceneView.RepaintAll();
            }
            OnDataChanged();
        }
        #endregion

        #region ON DATA CHANGED
        public TilingSettings() : base() => _paintTool.OnDataChanged += DataChanged;

        public override void DataChanged()
        {
            base.DataChanged();
            PWBIO.UpdateStroke();
        }
        #endregion

        #region PAINT TOOL
        [SerializeField] private PaintToolSettings _paintTool = new PaintToolSettings();
        public Transform parent { get => _paintTool.parent; set => _paintTool.parent = value; }
        public bool overwritePrefabLayer { get => _paintTool.overwritePrefabLayer;
            set => _paintTool.overwritePrefabLayer = value; }
        public int layer { get => _paintTool.layer; set => _paintTool.layer = value; }
        public bool autoCreateParent { get => _paintTool.autoCreateParent; set => _paintTool.autoCreateParent = value; }
        public bool createSubparent { get => _paintTool.createSubparent; set => _paintTool.createSubparent = value; }
        public bool overwriteBrushProperties { get => _paintTool.overwriteBrushProperties;
            set => _paintTool.overwriteBrushProperties = value; }
        public BrushSettings brushSettings => _paintTool.brushSettings;
        #endregion

        public override void Copy(IToolSettings other)
        {
            var otherTilingSettings = other as TilingSettings;
            base.Copy(other);
            _paintTool.Copy(otherTilingSettings._paintTool);
            _mode = otherTilingSettings._mode;
            _cellSizeType = otherTilingSettings._cellSizeType;
            _cellSize = otherTilingSettings._cellSize;
            _rotation = otherTilingSettings._rotation;
            _spacing = otherTilingSettings._spacing;
            _axisAlignedWithNormal = otherTilingSettings._axisAlignedWithNormal;
        }

        public TilingSettings Clone()
        {
            var clone = new TilingSettings();
            clone.Copy(this);
            return clone;
        }
    }

    [Serializable]
    public class TilingData : ISerializationCallbackReceiver
    {
        public enum TilingState
        {
            NONE,
            RECTANGLE,
            EDIT
        }
        [SerializeField] private TilingState _state = TilingState.NONE;
        [SerializeField] private Vector3[] _points = new Vector3[9];
        [SerializeField] private int _selectedPointIdx = -1;
        private const string COMMAND_NAME = "Edit Tiling";

        public TilingState state
        {
            get => _state;
            set
            {
                if (_state == value) return;
                ToolProperties.RegisterUndo(COMMAND_NAME);
                _state = value;
            }
        }
        public Vector3[] points => _points;
        public Vector3 GetPoint(int idx) => _points[idx];
        public Vector3 selectedPoint => _points[_selectedPointIdx];
        public void SetPoint(int idx, Vector3 value, bool registerUndo)
        {
            if (_points[idx] == value) return;
            if (registerUndo) ToolProperties.RegisterUndo(COMMAND_NAME);
            _points[idx] = value;
        }

        public void SetPoint(int idx, float value, AxesUtils.Axis axis, bool registerUndo)
        {
            if (AxesUtils.GetAxisValue(_points[idx], axis) == value) return;
            if (registerUndo) ToolProperties.RegisterUndo(COMMAND_NAME);
            AxesUtils.SetAxisValue(ref _points[idx], axis, value);
        }

        public void AddValue(int idx, Vector3 value)
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            _points[idx] += value;
        }

        public void AddValue(int idx, float value, AxesUtils.Axis axis)
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            AxesUtils.AddValueToAxis(ref _points[idx], axis, value);
        }
        public int selectedPointIdx
        {
            get => _selectedPointIdx;
            set
            {
                if (_selectedPointIdx == value) return;
                ToolProperties.RegisterUndo(COMMAND_NAME);
                _selectedPointIdx = value;
            }
        }

        public void Reset()
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            _selectedPointIdx = -1;
            _state = TilingState.NONE;
        }

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() => PWBIO.repaint = true;

        private TilingData() { }
        private static TilingData _instance = null;
        public static TilingData instance
        {
            get
            {
                if (_instance == null) _instance = new TilingData();
                return _instance;
            }
        }
    }

    [Serializable]
    public class TilingManager : ToolManagerBase<TilingSettings> { }
    #endregion

    #region PWBIO
    public static partial class PWBIO
    {
        private static TilingData _tilingData = TilingData.instance;

        private static List<Vector3> _tilingCenters = new List<Vector3>();

        public static void ResetTilingState()
        {
            _snappedToVertex = false;
            _tilingData.Reset();
            _paintStroke.Clear();
        }

        private static void TilingDuringSceneGUI(SceneView sceneView)
        {
            if (sceneView.in2DMode && _tilingData.state != TilingData.TilingState.NONE)
            {
                ResetTilingState();
                TilingManager.settings.rotation = Quaternion.Euler(90, 0, 0);
                ToolProperties.RepainWindow();
            }
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                if (_tilingData.state == TilingData.TilingState.EDIT && _tilingData.selectedPointIdx > 0)
                    _tilingData.selectedPointIdx = -1;
                else if (_tilingData.state == TilingData.TilingState.NONE) ToolManager.DeselectTool();
                else ResetTilingState();
            }
            switch (_tilingData.state)
            {
                case TilingData.TilingState.NONE:
                    TilingStateNone(sceneView.in2DMode);
                    break;
                case TilingData.TilingState.RECTANGLE:
                    TilingStateRectangle(sceneView.in2DMode);
                    break;
                case TilingData.TilingState.EDIT:
                    TilingStateEdit(sceneView.camera);
                    break;
            }
        }

        private static void TilingStateNone(bool in2DMode)
        {
            if (Event.current.button == 0 && Event.current.type == EventType.MouseDown && !Event.current.alt)
            {
                _tilingData.state = TilingData.TilingState.RECTANGLE;
                TilingManager.settings.UpdateCellSize();
            }
            if (MouseDot(out Vector3 point, out Vector3 normal, TilingManager.settings.mode, in2DMode,
                TilingManager.settings.paintOnPalettePrefabs, TilingManager.settings.paintOnMeshesWithoutCollider, false))
            {
                point = SnapAndUpdateGridOrigin(point, SnapManager.settings.snappingEnabled,
                   TilingManager.settings.paintOnPalettePrefabs, TilingManager.settings.paintOnMeshesWithoutCollider
                   , false);
                _tilingData.SetPoint(2, point, false);
                _tilingData.SetPoint(0, point, false);
            }
            DrawDotHandleCap(_tilingData.GetPoint(0));
        }

        private static void DrawTilingRectangle()
        {
            var settings = TilingManager.settings;
            var cornerPoints = new Vector3[] { _tilingData.GetPoint(0), _tilingData.GetPoint(1),
                _tilingData.GetPoint(2), _tilingData.GetPoint(3), _tilingData.GetPoint(0) };
            Handles.color = new Color(0f, 0f, 0f, 0.7f);
            Handles.DrawAAPolyLine(8, cornerPoints);
            Handles.color = new Color(1f, 1f, 1f, 0.7f);
            Handles.DrawAAPolyLine(4, cornerPoints);
            foreach (var point in cornerPoints)
            {
                var handleSize = HandleUtility.GetHandleSize(point) * 0.5f;
                Handles.color = new Color(0f, 0f, 0f, 0.7f);
                Handles.DrawAAPolyLine(6, new Vector3[] { point, point + settings.rotation * Vector3.up * handleSize });
                Handles.color = new Color(1f, 1f, 1f, 0.7f);
                Handles.DrawAAPolyLine(2, new Vector3[] { point, point + settings.rotation * Vector3.up * handleSize });
            }
        }

        private static void UpdateMidpoints()
        {
            for (int i = 0; i < 4; ++i)
            {
                var nextI = (i + 1) % 4;
                var point = _tilingData.GetPoint(i);
                var nextPoint = _tilingData.GetPoint(nextI);
                _tilingData.SetPoint(i + 4, point + (nextPoint - point) / 2, false);
            }
            _tilingData.SetPoint(8, _tilingData.GetPoint(0)
                + (_tilingData.GetPoint(2) - _tilingData.GetPoint(0)) / 2, false);
        }

        private static void DrawCells()
        {
            _tilingCenters.Clear();
            var settings = TilingManager.settings;
            var tangentDir = _tilingData.GetPoint(1) - _tilingData.GetPoint(0);
            var tangentSize = tangentDir.magnitude;
            tangentDir.Normalize();
            var bitangentDir = _tilingData.GetPoint(3) - _tilingData.GetPoint(0);
            var bitangentSize = bitangentDir.magnitude;
            bitangentDir.Normalize();
            var cellTangent = tangentDir * Mathf.Abs(settings.cellSize.x);
            var cellBitangent = bitangentDir * Mathf.Abs(settings.cellSize.y);
            var vertices = new Vector3[] { Vector3.zero, cellTangent, cellTangent + cellBitangent, cellBitangent };
            var offset = _tilingData.GetPoint(0);
            void DrawCell()
            {
                var linePoints = new Vector3[5];
                for (int i = 0; i <= 4; ++i) linePoints[i] = vertices[i % 4] + offset;
                _tilingCenters.Add(linePoints[0] + (linePoints[2] - linePoints[0]) / 2);
                Handles.color = new Color(0f, 0f, 0f, 0.3f);
                Handles.DrawAAPolyLine(6, linePoints);
                Handles.color = new Color(1f, 1f, 1f, 0.3f);
                Handles.DrawAAPolyLine(2, linePoints);
            }

            float tangentOffset = 0;
            while (Mathf.Abs(tangentOffset) + Mathf.Abs(settings.cellSize.x) < tangentSize)
            {
                float bitangentOffset = 0;
                while (Mathf.Abs(bitangentOffset) + Mathf.Abs(settings.cellSize.y) < bitangentSize)
                {
                    DrawCell();
                    bitangentOffset += settings.cellSize.y + settings.spacing.y;
                    offset = _tilingData.GetPoint(0) + tangentDir * Mathf.Abs(tangentOffset)
                        + bitangentDir * Mathf.Abs(bitangentOffset);
                }
                tangentOffset += settings.cellSize.x + settings.spacing.x;
                offset = _tilingData.GetPoint(0) + tangentDir * Mathf.Abs(tangentOffset);
            }
        }


        private static void TilingStateRectangle(bool in2DMode)
        {
            var settings = TilingManager.settings;
            if (Event.current.button == 0 && Event.current.type == EventType.MouseDown && !Event.current.alt)
            {
                UpdateMidpoints();
                _tilingData.state = TilingData.TilingState.EDIT;
                _updateStroke = true;
            }
            
            var mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            var plane = new Plane(settings.rotation * Vector3.up, _tilingData.GetPoint(0));

            if (plane.Raycast(mouseRay, out float distance))
            {
                var point = mouseRay.GetPoint(distance);
                _tilingData.SetPoint(2, point, false);
                var diagonal = point - _tilingData.GetPoint(0);
                var tangent = Vector3.Project(diagonal, settings.rotation * Vector3.right);
                var bitangent = Vector3.Project(diagonal, settings.rotation * Vector3.forward);
                _tilingData.SetPoint(1, _tilingData.GetPoint(0) + tangent, false);
                _tilingData.SetPoint(3, _tilingData.GetPoint(0) + bitangent, false);
                DrawCells();
                DrawTilingRectangle();
                for (int i = 0; i < 4; ++i) DrawDotHandleCap(_tilingData.GetPoint(i));
                return;
            }
            DrawDotHandleCap(_tilingData.GetPoint(0));
        }

        private static void TilingStateEdit(Camera camera)
        {
            bool mouseDown = Event.current.button == 0 && Event.current.type == EventType.MouseDown;

            if (Event.current.type == EventType.Repaint)
            {
                if (_updateStroke)
                {
                    BrushstrokeManager.UpdateRectBrushstroke(_tilingCenters.ToArray());
                    SceneView.RepaintAll();
                    _updateStroke = false;
                }
                TilingStrokePreview(camera);
            }
            DrawCells();
            DrawTilingRectangle();
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                Paint(TilingManager.settings);
                ResetTilingState();
            }
            else if (Event.current.button == 1
                && Event.current.type == EventType.MouseDrag && Event.current.shift)
            {
                var deltaSign = -Mathf.Sign(Event.current.delta.x + Event.current.delta.y);
                var otherAxes = AxesUtils.GetOtherAxes(AxesUtils.Axis.Y);
                var spacing = Vector3.zero;
                AxesUtils.SetAxisValue(ref spacing, otherAxes[0], TilingManager.settings.spacing.x);
                AxesUtils.SetAxisValue(ref spacing, otherAxes[1], TilingManager.settings.spacing.y);
                var axisIdx = Event.current.control ? 1 : 0;
                var size = _tilingData.GetPoint(2) - _tilingData.GetPoint(axisIdx);
                var axisSize = AxesUtils.GetAxisValue(size, otherAxes[axisIdx]);
                AxesUtils.AddValueToAxis(ref spacing, otherAxes[axisIdx], axisSize * deltaSign * 0.005f);
                TilingManager.settings.spacing = new Vector2(AxesUtils.GetAxisValue(spacing, otherAxes[0]),
                    AxesUtils.GetAxisValue(spacing, otherAxes[1]));
                ToolProperties.RepainWindow();
                Event.current.Use();
            }
            else if (Event.current.type == EventType.KeyDown && Event.current.control
               && Event.current.keyCode == KeyCode.UpArrow) RotateTiling(90, Vector3.right, true);
            else if (Event.current.type == EventType.KeyDown && Event.current.control
                && Event.current.keyCode == KeyCode.DownArrow) RotateTiling(90, Vector3.left, true);
            else if (Event.current.type == EventType.KeyDown && Event.current.control
                && Event.current.keyCode == KeyCode.RightArrow) RotateTiling(90, Vector3.up, true);
            else if (Event.current.type == EventType.KeyDown && Event.current.control
               && Event.current.keyCode == KeyCode.LeftArrow) RotateTiling(90, Vector3.down, true);
            else if (Event.current.type == EventType.KeyDown && Event.current.control
                && Event.current.keyCode == KeyCode.PageUp) RotateTiling(90, Vector3.forward, true);
            else if (Event.current.type == EventType.KeyDown && Event.current.control
                && Event.current.keyCode == KeyCode.PageDown) RotateTiling(90, Vector3.back, true);
            bool clickOnPoint = false;
            for (int i = 0; i < 9; ++i)
            {
                var controlId = GUIUtility.GetControlID(FocusType.Passive);
                if (!clickOnPoint)
                {
                    float distFromMouse
                        = HandleUtility.DistanceToRectangle(_tilingData.GetPoint(i), Quaternion.identity, 0f);
                    HandleUtility.AddControl(controlId, distFromMouse);
                    if (mouseDown && HandleUtility.nearestControl == controlId)
                    {
                        _tilingData.selectedPointIdx = i;
                        clickOnPoint = true;
                        Event.current.Use();
                    }
                }
                if (Event.current.type != EventType.Repaint) continue;
                DrawDotHandleCap(_tilingData.GetPoint(i));
            }

            if (_tilingData.selectedPointIdx >= 0)
            {
                var prevPosition = _tilingData.selectedPoint;
                _tilingData.SetPoint(_tilingData.selectedPointIdx,
                    Handles.PositionHandle(_tilingData.selectedPoint, TilingManager.settings.rotation), true);
                var snappedPoint = SnapAndUpdateGridOrigin(_tilingData.selectedPoint, SnapManager.settings.snappingEnabled,
                   TilingManager.settings.paintOnPalettePrefabs, TilingManager.settings.paintOnMeshesWithoutCollider,
                   false);
                _tilingData.SetPoint(_tilingData.selectedPointIdx, snappedPoint, true);

                if (prevPosition != _tilingData.selectedPoint)
                {
                    _updateStroke = true;
                    var delta = _tilingData.selectedPoint - prevPosition;
                    if (_tilingData.selectedPointIdx < 4)
                    {
                        var nextCornerIdx = (_tilingData.selectedPointIdx + 1) % 4;
                        var oppositeCornerIdx = (_tilingData.selectedPointIdx + 2) % 4;
                        var prevCornerIdx = (_tilingData.selectedPointIdx + 3) % 4;

                        var nextVector = _tilingData.GetPoint(nextCornerIdx) - prevPosition;
                        var prevVector = _tilingData.GetPoint(prevCornerIdx) - prevPosition;
                        var deltaNext = Vector3.Project(delta, nextVector);
                        var deltaPrev = Vector3.Project(delta, prevVector);
                        var deltaNormal = delta - deltaNext - deltaPrev;
                        _tilingData.AddValue(nextCornerIdx, deltaPrev + deltaNormal);
                        _tilingData.AddValue(prevCornerIdx, deltaNext + deltaNormal);
                        _tilingData.AddValue(oppositeCornerIdx, deltaNormal);
                    }
                    else if (_tilingData.selectedPointIdx < 8)
                    {
                        var prevCornerIdx = _tilingData.selectedPointIdx - 4;
                        var nextCornerIdx = (_tilingData.selectedPointIdx - 3) % 4;
                        var oppositeSideIdx = (_tilingData.selectedPointIdx - 2) % 4 + 4;
                        var parallel = _tilingData.GetPoint(nextCornerIdx) - _tilingData.GetPoint(prevCornerIdx);
                        var perpendicular = _tilingData.GetPoint(oppositeSideIdx) - prevPosition;
                        var deltaParallel = Vector3.Project(delta, parallel);
                        var deltaPerpendicular = Vector3.Project(delta, perpendicular);
                        var deltaNormal = delta - deltaParallel - deltaPerpendicular;
                        for (int i = 0; i < 4; ++i) _tilingData.AddValue(i, deltaParallel + deltaNormal);
                        _tilingData.AddValue(prevCornerIdx, deltaPerpendicular);
                        _tilingData.AddValue(nextCornerIdx, deltaPerpendicular);
                    }
                    else for (int i = 0; i < 4; ++i) _tilingData.AddValue(i, delta);
                    UpdateMidpoints();
                }
                if (_tilingData.selectedPointIdx == 8)
                {
                    var prevRotation = TilingManager.settings.rotation;
                    TilingManager.settings.rotation = Handles.RotationHandle(TilingManager.settings.rotation,
                        _tilingData.GetPoint(8));
                    if (TilingManager.settings.rotation != prevRotation)
                    {
                        var angle = Quaternion.Angle(prevRotation, TilingManager.settings.rotation);
                        var axis = Vector3.Cross(prevRotation * Vector3.forward,
                            TilingManager.settings.rotation * Vector3.forward);
                        if (axis == Vector3.zero) axis = Vector3.Cross(prevRotation * Vector3.up,
                            TilingManager.settings.rotation * Vector3.up);
                        axis.Normalize();
                        RotateTiling(angle, axis, false);
                        ToolProperties.RepainWindow();
                    }
                }
            }
        }

        private static void RotateTiling(float angle, Vector3 axis, bool updateDataRotation)
        {
            _updateStroke = true;
            var delta = Quaternion.AngleAxis(angle, axis);
            for (int i = 0; i < 8; ++i)
            {
                var centerToPoint = _tilingData.GetPoint(i) - _tilingData.GetPoint(8);
                var rotatedPos = (delta * centerToPoint) + _tilingData.GetPoint(8);
                _tilingData.SetPoint(i, rotatedPos, false);
            }
            if (updateDataRotation) TilingManager.settings.rotation *= delta;
        }

        public static void UpdateTilingRotation(Quaternion delta)
        {
            _updateStroke = true;
            for (int i = 0; i < 8; ++i)
            {
                var centerToPoint = _tilingData.GetPoint(i) - _tilingData.GetPoint(8);
                var rotatedPos = (delta * centerToPoint) + _tilingData.GetPoint(8);
                _tilingData.SetPoint(i, rotatedPos, false);
            }
        }

        private static void TilingStrokePreview(Camera camera)
        {
            _paintStroke.Clear();
            var settings = TilingManager.settings;
            for (int i = 0; i < BrushstrokeManager.brushstroke.Length; ++i)
            {
                var strokeItem = BrushstrokeManager.brushstroke[i];
                BrushSettings brushSettings = strokeItem.settings;
                if (settings.overwriteBrushProperties) brushSettings = settings.brushSettings;
                var prefab = strokeItem.settings.prefab;
                if (prefab == null) continue;
                var bounds = BoundsUtils.GetBoundsRecursive(prefab.transform);
                var size = bounds.size;
                var height = Mathf.Max(size.x, size.y, size.z) * 2;

                var itemPosition = strokeItem.tangentPosition;
                var normal = Vector3.up;
                var ray = new Ray(itemPosition + Vector3.up * height, Vector3.down);
                if (settings.mode != TilingSettings.PaintMode.ON_SHAPE)
                {
                    if (MouseRaycast(ray, out RaycastHit itemHit,
                        out GameObject collider, height * 2f, -1,
                        settings.paintOnPalettePrefabs, settings.paintOnMeshesWithoutCollider))
                    {
                        itemPosition = itemHit.point;
                        normal = itemHit.normal;
                    }
                    else if (settings.mode == TilingSettings.PaintMode.ON_SURFACE) continue;
                }

                
                var itemRotation = settings.rotation;
                Vector3 itemTangent = itemRotation * Vector3.forward;
                if (brushSettings.rotateToTheSurface
                    && settings.mode != PaintOnSurfaceToolSettings.PaintMode.ON_SHAPE)
                {
                    itemRotation = Quaternion.LookRotation(itemTangent, normal);
                    itemPosition += normal * brushSettings.surfaceDistance;
                }
                else itemPosition += normal * brushSettings.surfaceDistance;
                var axisAlignedWithNormal = (Vector3)settings.axisAlignedWithNormal;
                if (settings.axisAlignedWithNormal.axis != AxesUtils.Axis.Y) axisAlignedWithNormal *= -1;
                itemRotation *= Quaternion.FromToRotation(Vector3.up, axisAlignedWithNormal);

                itemRotation *= Quaternion.Euler(strokeItem.additionalAngle);
                var previewRotation = itemRotation;
                if (brushSettings.embedInSurface)
                {
                    var TRS = Matrix4x4.TRS(itemPosition, itemRotation, strokeItem.scaleMultiplier);
                    var bottomDistanceToSurfce = GetBottomDistanceToSurface(strokeItem.settings.bottomVertices,
                        TRS, strokeItem.settings.height * strokeItem.scaleMultiplier.y, settings.paintOnPalettePrefabs,
                        settings.paintOnMeshesWithoutCollider);
                    if (!brushSettings.embedAtPivotHeight)
                        bottomDistanceToSurfce -= strokeItem.settings.bottomMagnitude;
                    itemPosition += itemRotation * new Vector3(0f, -bottomDistanceToSurfce, 0f);
                }
                itemPosition += itemRotation * (brushSettings.localPositionOffset);

                var centerToPivot = strokeItem.settings.prefab.transform.position - bounds.center;
                var centerToPivotRotated = Quaternion.FromToRotation(Vector3.up, axisAlignedWithNormal) * centerToPivot;
                centerToPivotRotated.y = 0;
                itemPosition += settings.rotation * centerToPivotRotated;

                var itemScale = Vector3.Scale(prefab.transform.localScale, strokeItem.scaleMultiplier);

                var layer = settings.overwritePrefabLayer ? settings.layer : prefab.layer;
                Transform parentTransform = GetParent(settings, prefab.name, false);

                _paintStroke.Add(new PaintStrokeItem(prefab, itemPosition,
                    itemRotation * Quaternion.Euler(prefab.transform.eulerAngles),
                    itemScale, layer, parentTransform));
                var previewRootToWorld = Matrix4x4.TRS(itemPosition, previewRotation, strokeItem.scaleMultiplier)
                    * Matrix4x4.Translate(-prefab.transform.position);
                PreviewBrushItem(prefab, previewRootToWorld, layer, camera);
            }
        }
    }
    #endregion
}
