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
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace PluginMaster
{
    #region DATA & SETTINGS 
    [Serializable]
    public class LineSettings : PaintOnSurfaceToolSettings, IPaintToolSettings
    {
        public enum SpacingType { BOUNDS, CONSTANT }
        [SerializeField] private PaintMode _mode = PaintMode.AUTO;
        [SerializeField] private Vector3 _projectionDirection = Vector3.down;
        [SerializeField] private bool _objectsOrientedAlongTheLine = true;
        [SerializeField] private AxesUtils.Axis _axisOrientedAlongTheLine = AxesUtils.Axis.X;
        [SerializeField] private SpacingType _spacingType = SpacingType.BOUNDS;
        [SerializeField] private float _gapSize = 0f;
        [SerializeField] private float _spacing = 10f;
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

        public Vector3 projectionDirection
        {
            get => _projectionDirection;
            set
            {
                if (_projectionDirection == value) return;
                _projectionDirection = value;
                OnDataChanged();
            }
        }
        public void UpdateProjectDirection(Vector3 value) =>_projectionDirection = value;

        public bool objectsOrientedAlongTheLine
        {
            get => _objectsOrientedAlongTheLine;
            set
            {
                if (_objectsOrientedAlongTheLine == value) return;
                _objectsOrientedAlongTheLine = value;
                OnDataChanged();
            }
        }

        public AxesUtils.Axis axisOrientedAlongTheLine
        {
            get => _axisOrientedAlongTheLine;
            set
            {
                if (_axisOrientedAlongTheLine == value) return;
                _axisOrientedAlongTheLine = value;
                OnDataChanged();
            }
        }

        public SpacingType spacingType
        {
            get => _spacingType;
            set
            {
                if (_spacingType == value) return;
                _spacingType = value;
                OnDataChanged();
            }
        }

        public float spacing
        {
            get => _spacing;
            set
            {
                value = Mathf.Max(value, 0.01f);
                if (_spacing == value) return;
                _spacing = value;
                OnDataChanged();
            }
        }

        public float gapSize
        {
            get => _gapSize;
            set
            {
                if (_gapSize == value) return;
                _gapSize = value;
                OnDataChanged();
            }
        }

        [SerializeField] private PaintToolSettings _paintTool = new PaintToolSettings();
        public Transform parent { get => _paintTool.parent; set => _paintTool.parent = value; }
        public bool overwritePrefabLayer
        { get => _paintTool.overwritePrefabLayer; set => _paintTool.overwritePrefabLayer = value; }
        public int layer { get => _paintTool.layer; set => _paintTool.layer = value; }
        public bool autoCreateParent { get => _paintTool.autoCreateParent; set => _paintTool.autoCreateParent = value; }
        public bool createSubparent { get => _paintTool.createSubparent; set => _paintTool.createSubparent = value; }
        public bool overwriteBrushProperties
        { get => _paintTool.overwriteBrushProperties; set => _paintTool.overwriteBrushProperties = value; }
        public BrushSettings brushSettings => _paintTool.brushSettings;

        public LineSettings() : base()
        {
            OnDataChanged += DataChanged;
            _paintTool.OnDataChanged += DataChanged;
        }

        public override void DataChanged()
        {
            base.DataChanged();
            UpdateStroke();
            SceneView.RepaintAll();
        }

        protected virtual void UpdateStroke() => PWBIO.UpdateStroke();

        public override void Copy(IToolSettings other)
        {
            var otherLineSettings = other as LineSettings;
            if (otherLineSettings == null) return;
            base.Copy(other);
            _mode = otherLineSettings._mode;
            _projectionDirection = otherLineSettings._projectionDirection;
            _objectsOrientedAlongTheLine = otherLineSettings._objectsOrientedAlongTheLine;
            _axisOrientedAlongTheLine = otherLineSettings._axisOrientedAlongTheLine;
            _spacingType = otherLineSettings._spacingType;
            _spacing = otherLineSettings._spacing;
            _paintTool.Copy(otherLineSettings._paintTool);
            _gapSize = otherLineSettings._gapSize;
        }

        public virtual LineSettings Clone()
        {
            var clone = new LineSettings();
            clone.Copy(this);
            return clone;
        }
    }

    [Serializable]
    public class LineSegment
    {
        public enum SegmentType { STRAIGHT, CURVE }
        public SegmentType type = SegmentType.CURVE;
        public List<Vector3> points = new List<Vector3>();
    }

    [Serializable]
    public class LinePoint
    {
        public LineSegment.SegmentType type = LineSegment.SegmentType.CURVE;
        public Vector3 position = Vector3.zero;

        public LinePoint(LineSegment.SegmentType type, Vector3 position)
        {
            this.type = type;
            this.position = position;
        }
    }

    [Serializable]
    public class LineData : ISerializationCallbackReceiver
    {
        public enum LineState
        {
            NONE,
            STRAIGHT_LINE,
            BEZIER
        }
        private const string COMMAND_NAME = "Edit Line";
        [SerializeField] private LineState _state = LineState.NONE;
        [SerializeField] private List<LinePoint> _linePoints = new List<LinePoint>();
        [SerializeField] private int _selectedPointIdx = -1;
        [SerializeField] private List<int> _selection = new List<int>();
        [SerializeField] private bool _closed = false;
        private Vector3[] _points = null;
        public LineState state
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
        public int pointsCount => _points.Length;
        public Vector3 GetPoint(int idx) => _points[idx];
        public Vector3 selectedPoint => _points[_selectedPointIdx];

        public void SetPoint(int idx, Vector3 value, bool registerUndo)
        {
            if (_points.Length <= 1) Initialize();
            if (_points[idx] == value) return;
            if (registerUndo) ToolProperties.RegisterUndo(COMMAND_NAME);
            var delta = value - _points[idx];
            _points[idx] = _linePoints[idx].position = value;

            foreach (var selectedIdx in _selection)
            {
                if (selectedIdx == idx) continue;
                _linePoints[selectedIdx].position += delta;
                _points[selectedIdx] = _linePoints[selectedIdx].position;
            }
        }

        public void RemoveSelectedPoints()
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            var toRemove = new List<int>(_selection);
            if (!toRemove.Contains(_selectedPointIdx)) toRemove.Add(_selectedPointIdx);
            toRemove.Sort();
            if (toRemove.Count >= _points.Length - 1)
            {
                Initialize();
                return;
            }
            for (int i = toRemove.Count - 1; i >= 0; --i) _linePoints.RemoveAt(toRemove[i]);
            _selectedPointIdx = -1;
            _selection.Clear();
            UpdatePoints();
        }

        public void InsertPoint(int idx, Vector3 point)
        {
            if (idx < 0) return;
            idx = Mathf.Max(idx, 1);
            ToolProperties.RegisterUndo(COMMAND_NAME);
            _linePoints.Insert(idx, new LinePoint(_linePoints[idx].type, point));
            UpdatePoints();
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
        public void AddToSelection(int idx)
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            if (!_selection.Contains(idx)) _selection.Add(idx);
        }
        public void SelectAll()
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            _selection.Clear();
            for (int i = 0; i < pointsCount; ++i) _selection.Add(i);
            if (_selectedPointIdx < 0) _selectedPointIdx = 0;
        }
        public void RemoveFromSelection(int idx)
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            if (_selection.Contains(idx)) _selection.Remove(idx);
        }
        public void ClearSelection()
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            _selection.Clear();
        }
        public bool IsSelected(int idx) => _selection.Contains(idx);
        public int selectionCount => _selection.Count;
        public void SetSegmentType(LineSegment.SegmentType type)
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            for (int i = 0; i < _selection.Count; ++i)
            {
                var idx = _selection[i];
                _linePoints[idx].type = type;
            }
        }
        public LineSegment[] GetSegments()
        {
            var segments = new List<LineSegment>();

            var type = _linePoints[0].type;
            for (int i = 0; i < pointsCount; ++i)
            {
                var segment = new LineSegment();
                segments.Add(segment);
                segment.type = type;
                segment.points.Add(_linePoints[i].position);

                do
                {
                    ++i;
                    if (i >= pointsCount) break;
                    type = _linePoints[i].type;
                    if (type == segment.type) segment.points.Add(_linePoints[i].position);
                } while (type == segment.type);
                if (i >= pointsCount) break;
                i -= 2;
            }
            if (_closed)
            {
                if (_linePoints[0].type == _linePoints.Last().type)
                    segments.Last().points.Add(_linePoints[0].position);
                else
                {
                    var segment = new LineSegment();
                    segment.type = _linePoints[0].type;
                    segment.points.Add(_linePoints.Last().position);
                    segment.points.Add(_linePoints[0].position);
                    segments.Add(segment);
                }
            }
            return segments.ToArray();
        }
        public void Reset()
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            Initialize();
        }

        private void UpdatePoints()
        {
            var pointsList = new List<Vector3>();
            foreach (var point in _linePoints) pointsList.Add(point.position);
            _points = pointsList.ToArray();
        }

        public void ToggleClosed()
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            _closed = !_closed;
        }

        public bool closed => _closed;
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            UpdatePoints();
            PWBIO.repaint = true;
        }

        private void Initialize()
        {
            _selectedPointIdx = -1;
            _selection.Clear();
            _state = LineState.NONE;
            _linePoints.Clear();
            for (int i = 0; i < 4; ++i) _linePoints.Add(new LinePoint(LineSegment.SegmentType.CURVE, Vector3.zero));
            UpdatePoints();
        }
        private LineData() => Initialize();
        private static LineData _instance = null;
        public static LineData instance
        {
            get
            {
                if (_instance == null) _instance = new LineData();
                if (_instance.points.Length == 0) _instance.Initialize();
                return _instance;
            }
        }
    }

    [Serializable]
    public class LineManager : ToolManagerBase<LineSettings> { }
    #endregion

    #region PWBIO
    public static partial class PWBIO
    {
        private static LineData _lineData = LineData.instance;
        private static bool _selectingLinePoints = false;
        private static Rect _selectionRect = new Rect();
        private static float _lineLenght = 0f;
        private static List<Vector3> _lineMidpoints = new List<Vector3>();
        public static void ResetLineState()
        {
            _snappedToVertex = false;
            _lineMidpoints.Clear();
            _lineLenght = 0f;
            _selectingLinePoints = false;
            _lineData.Reset();
        }

        private static void LineDuringSceneGUI(SceneView sceneView)
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                if (_lineData.state == LineData.LineState.BEZIER && _lineData.selectedPointIdx > 0)
                {
                    _lineData.selectedPointIdx = -1;
                    _lineData.ClearSelection();
                }
                else if (_lineData.state == LineData.LineState.NONE) ToolManager.DeselectTool();
                else ResetLineState();
            }
            switch (_lineData.state)
            {
                case LineData.LineState.NONE:
                    LineStateNone(sceneView.in2DMode);
                    break;
                case LineData.LineState.STRAIGHT_LINE:
                    LineStateStraightLine(sceneView.in2DMode);
                    break;
                case LineData.LineState.BEZIER:
                    LineStateBezier(sceneView.camera);
                    break;
            }
        }

        private static void LineStateNone(bool in2DMode)
        {
            if (Event.current.button == 0 && Event.current.type == EventType.MouseDown && !Event.current.alt)
            {
                _lineData.state = LineData.LineState.STRAIGHT_LINE;
                Event.current.Use();
            }
            if (MouseDot(out Vector3 point, out Vector3 normal, LineManager.settings.mode, in2DMode,
                LineManager.settings.paintOnPalettePrefabs, LineManager.settings.paintOnMeshesWithoutCollider, false))
            {
                point = SnapAndUpdateGridOrigin(point, SnapManager.settings.snappingEnabled,
                    LineManager.settings.paintOnPalettePrefabs, LineManager.settings.paintOnMeshesWithoutCollider,
                    false);
                _lineData.SetPoint(0, point, false);
                _lineData.SetPoint(3, point, false);
            }
            DrawDotHandleCap(_lineData.GetPoint(0));
        }

        private static void LineStateStraightLine(bool in2DMode)
        {
            if (Event.current.button == 0 && Event.current.type == EventType.MouseDown && !Event.current.alt)
            {
                var lineThird = (_lineData.GetPoint(3) - _lineData.GetPoint(0)) / 3f;
                _lineData.state = LineData.LineState.BEZIER;
                _lineData.SetPoint(1, _lineData.GetPoint(0) + lineThird, false);
                _lineData.SetPoint(2, _lineData.GetPoint(1) + lineThird, false);
                _updateStroke = true;
            }
            if (MouseDot(out Vector3 point, out Vector3 normal, LineManager.settings.mode, in2DMode,
                LineManager.settings.paintOnPalettePrefabs, LineManager.settings.paintOnMeshesWithoutCollider, false))
            {
                point = SnapAndUpdateGridOrigin(point, SnapManager.settings.snappingEnabled,
                    LineManager.settings.paintOnPalettePrefabs, LineManager.settings.paintOnMeshesWithoutCollider,
                    false);
                _lineData.SetPoint(3, point, false);
            }

            Handles.color = new Color(0f, 0f, 0f, 0.7f);
            Handles.DrawAAPolyLine(8, new Vector3[] { _lineData.GetPoint(0), _lineData.GetPoint(3) });
            Handles.color = new Color(1f, 1f, 1f, 0.7f);
            Handles.DrawAAPolyLine(4, new Vector3[] { _lineData.GetPoint(0), _lineData.GetPoint(3) });
            DrawDotHandleCap(_lineData.GetPoint(0));
            DrawDotHandleCap(_lineData.GetPoint(3));
        }

        private static float GetLineLength(Vector3[] points, out float[] lengthFromFirstPoint)
        {
            float lineLength = 0f;
            lengthFromFirstPoint = new float[points.Length];
            var segmentLength = new float[points.Length];
            lengthFromFirstPoint[0] = 0f;
            for (int i = 1; i < points.Length; ++i)
            {
                segmentLength[i - 1] = (points[i] - points[i - 1]).magnitude;
                lineLength += segmentLength[i - 1];
                lengthFromFirstPoint[i] = lineLength;
            }
            return lineLength;
        }

        private static Vector3[] GetLineMidpoints(Vector3[] points)
        {
            if(points.Length == 0) return new Vector3[0];
            var midpoints = new List<Vector3>();
            var subSegments = new List<List<Vector3>>();
            var pathPoints = _lineData.points;
            bool IsAPathPoint(Vector3 point) => pathPoints.Contains(point);
            subSegments.Add(new List<Vector3>());
            subSegments.Last().Add(points[0]);
            for (int i = 1; i < points.Length -1; ++i)
            {
                var point = points[i];
                subSegments.Last().Add(point);
                if (IsAPathPoint(point))
                {
                    subSegments.Add(new List<Vector3>());
                    subSegments.Last().Add(point);
                }
            }
            subSegments.Last().Add(points.Last());
            Vector3 GetLineMidpoint(Vector3[] subSegmentPoints)
            {
                var midpoint = subSegmentPoints[0];
                float[] lengthFromFirstPoint = null;
                var halfLineLength = GetLineLength(subSegmentPoints, out lengthFromFirstPoint) / 2f;
                for (int i = 1; i < subSegmentPoints.Length; ++i)
                {
                    if (lengthFromFirstPoint[i] < halfLineLength) continue;
                    var dir = (subSegmentPoints[i] - subSegmentPoints[i - 1]).normalized;
                    var localLength = halfLineLength - lengthFromFirstPoint[i - 1];
                    midpoint = subSegmentPoints[i - 1] + dir * localLength;
                    break;
                }
                return midpoint;
            }
            foreach (var subSegment in subSegments) midpoints.Add(GetLineMidpoint(subSegment.ToArray()));
            return midpoints.ToArray();
        }

        private static void LineStateBezier(Camera camera)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var segments = _lineData.GetSegments();
                var linePoints = new List<Vector3>();
                _lineMidpoints.Clear();
                foreach (var segment in segments)
                {
                    var segmentPoints = new List<Vector3>();
                    if (segment.type == LineSegment.SegmentType.STRAIGHT) segmentPoints.AddRange(segment.points);
                    else segmentPoints.AddRange(BezierPath.GetBezierPoints(segment.points.ToArray()));
                    linePoints.AddRange(segmentPoints);
                    if (segmentPoints.Count == 0) continue;
                    _lineMidpoints.AddRange(GetLineMidpoints(segmentPoints.ToArray()));
                }

                var pointArray = linePoints.ToArray();
                _lineLenght = 0;
                for (int i = 1; i < pointArray.Length; ++i)
                    _lineLenght += (pointArray[i] - pointArray[i - 1]).magnitude;
                if (_updateStroke)
                {
                    BrushstrokeManager.UpdateLineBrushstroke(pointArray);
                    SceneView.RepaintAll();
                    _updateStroke = false;
                }
                LineStrokePreview(camera, LineManager.settings, linePoints.Last());
                Handles.zTest = CompareFunction.Always;
                Handles.color = new Color(0f, 0f, 0f, 0.7f);
                Handles.DrawAAPolyLine(8, pointArray);
                Handles.color = new Color(1f, 1f, 1f, 0.7f);
                Handles.DrawAAPolyLine(4, pointArray);
                if (_selectingLinePoints)
                {
                    var rays = new Ray[]
                    {
                        HandleUtility.GUIPointToWorldRay(_selectionRect.min),
                        HandleUtility.GUIPointToWorldRay(new Vector2(_selectionRect.xMax, _selectionRect.yMin)),
                        HandleUtility.GUIPointToWorldRay(_selectionRect.max),
                        HandleUtility.GUIPointToWorldRay(new Vector2(_selectionRect.xMin, _selectionRect.yMax))
                    };
                    var verts = new Vector3[4];
                    for (int i = 0; i < 4; ++i) verts[i] = rays[i].origin + rays[i].direction;
                    Handles.DrawSolidRectangleWithOutline(verts,
                    new Color(0f, 0.5f, 0.5f, 0.3f), new Color(0f, 0.5f, 0.5f, 1f));
                }
            }
            else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                ResetLineState();
                Paint(LineManager.settings);
            }
            else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
            {
                _lineData.RemoveSelectedPoints();
                _updateStroke = true;
            }
            else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.A
                && Event.current.control && Event.current.shift)
            {
                _lineData.SelectAll();
            }
            else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.D
                && Event.current.control && Event.current.shift)
            {
                _lineData.selectedPointIdx = -1;
                _lineData.ClearSelection();
            }
            else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.PageUp)
            {
                _lineData.SetSegmentType(LineSegment.SegmentType.CURVE);
                _updateStroke = true;
            }
            else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.PageDown)
            {
                _lineData.SetSegmentType(LineSegment.SegmentType.STRAIGHT);
                _updateStroke = true;
            }
            else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.End)
            {
                _lineData.ToggleClosed();
                _updateStroke = true;
            }
            else if (Event.current.button == 1 && Event.current.type == EventType.MouseDrag && Event.current.shift)
            {
                var deltaSign = Mathf.Sign(Event.current.delta.x + Event.current.delta.y);
                LineManager.settings.gapSize += _lineLenght * deltaSign * 0.001f;
                ToolProperties.RepainWindow();
                Event.current.Use();
            }

            bool clickOnPoint = false;
            bool leftMouseDown = Event.current.button == 0 && Event.current.type == EventType.MouseDown;

            if (_selectingLinePoints && !Event.current.control)
            {
                _lineData.selectedPointIdx = -1;
                _lineData.ClearSelection();
            }
            for (int i = 0; i < _lineData.pointsCount; ++i)
            {
                var controlId = GUIUtility.GetControlID(FocusType.Passive);
                if (_selectingLinePoints)
                {
                    var GUIPos = HandleUtility.WorldToGUIPoint(_lineData.GetPoint(i));
                    var rect = _selectionRect;
                    if (_selectionRect.size.x < 0 || _selectionRect.size.y < 0)
                    {
                        var max = Vector2.Max(_selectionRect.min, _selectionRect.max);
                        var min = Vector2.Min(_selectionRect.min, _selectionRect.max);
                        var size = max - min;
                        rect = new Rect(min, size);
                    }
                    if (rect.Contains(GUIPos))
                    {
                        if (!Event.current.control && _lineData.selectedPointIdx < 0)
                            _lineData.selectedPointIdx = i;
                        _lineData.AddToSelection(i);
                    }
                }
                else if (!clickOnPoint)
                {
                    float distFromMouse
                        = HandleUtility.DistanceToRectangle(_lineData.GetPoint(i), Quaternion.identity, 0f);
                    HandleUtility.AddControl(controlId, distFromMouse);
                    if (leftMouseDown && HandleUtility.nearestControl == controlId)
                    {
                        if (!Event.current.control)
                        {
                            _lineData.selectedPointIdx = i;
                            _lineData.ClearSelection();
                        }
                        if (Event.current.control || _lineData.selectionCount == 0)
                        {
                            if (_lineData.IsSelected(i)) _lineData.RemoveFromSelection(i);
                            else _lineData.AddToSelection(i);
                        }
                        clickOnPoint = true;
                        Event.current.Use();
                    }
                }
                if (Event.current.type != EventType.Repaint) continue;
                DrawDotHandleCap(_lineData.GetPoint(i), 1, 1, _lineData.IsSelected(i));
            }
            for (int i = 0; i < _lineMidpoints.Count; ++i)
            {
                var point = _lineMidpoints[i];
                var controlId = GUIUtility.GetControlID(FocusType.Passive);
                float distFromMouse
                       = HandleUtility.DistanceToRectangle(point, Quaternion.identity, 0f);
                HandleUtility.AddControl(controlId, distFromMouse);
                DrawDotHandleCap(point, 0.4f);
                if (HandleUtility.nearestControl == controlId)
                {
                    DrawDotHandleCap(point);
                    if(leftMouseDown)
                    {
                        _lineData.InsertPoint(i+1, point);
                        _lineData.selectedPointIdx = i + 1;
                        _lineData.ClearSelection();
                        _updateStroke = true;
                        clickOnPoint = true;
                        Event.current.Use();
                    }
                }
            }
            if (_lineData.selectedPointIdx >= 0)
            {
                var prevPosition = _lineData.selectedPoint;
                _lineData.SetPoint(_lineData.selectedPointIdx,
                    Handles.PositionHandle(_lineData.selectedPoint, Quaternion.identity), true);

                var point = SnapAndUpdateGridOrigin(_lineData.selectedPoint, SnapManager.settings.snappingEnabled,
                        LineManager.settings.paintOnPalettePrefabs, LineManager.settings.paintOnMeshesWithoutCollider,
                        false);
                _lineData.SetPoint(_lineData.selectedPointIdx, point, true);
                if (prevPosition != _lineData.selectedPoint) _updateStroke = true;
            }
            if (Event.current.shift && leftMouseDown && !clickOnPoint)
            {
                _selectingLinePoints = true;
                _selectionRect = new Rect(Event.current.mousePosition, Vector2.zero);
            }
            else if (Event.current.type == EventType.MouseDrag && _selectingLinePoints)
            {
                _selectionRect.size = Event.current.mousePosition - _selectionRect.position;
            }
            else if (_selectingLinePoints && Event.current.button == 0
                && (Event.current.type == EventType.MouseUp || Event.current.type == EventType.Ignore))
                _selectingLinePoints = false;
        }

        private static void LineStrokePreview(Camera camera, LineSettings settings, Vector3 lastPoint)
        {
            _paintStroke.Clear();
            for (int i = 0; i < BrushstrokeManager.brushstroke.Length; ++i)
            {
                var strokeItem = BrushstrokeManager.brushstroke[i];
                var prefab = strokeItem.settings.prefab;
                if (prefab == null) continue;
                var bounds = BoundsUtils.GetBoundsRecursive(prefab.transform);
                var size = bounds.size;
                var pivotToCenter = bounds.center - prefab.transform.position;
                var pivotToCenterLocal = prefab.transform.InverseTransformVector(pivotToCenter);
                var height = Mathf.Max(size.x, size.y, size.z) * 2;
                Vector3 segmentDir = Vector3.zero;
                if (settings.objectsOrientedAlongTheLine && BrushstrokeManager.brushstroke.Length > 1)
                {
                    segmentDir = i < BrushstrokeManager.brushstroke.Length - 1
                        ? BrushstrokeManager.brushstroke[i + 1].tangentPosition
                        - BrushstrokeManager.brushstroke[i].tangentPosition
                        : lastPoint - BrushstrokeManager.brushstroke[i].tangentPosition;
                }
                if (BrushstrokeManager.brushstroke.Length == 1)
                    segmentDir = lastPoint - BrushstrokeManager.brushstroke[0].tangentPosition;
                else if (i == BrushstrokeManager.brushstroke.Length - 1)
                {
                    var onLineSize = AxesUtils.GetAxisValue(size, settings.axisOrientedAlongTheLine)
                        + settings.gapSize;
                    var segmentSize = segmentDir.magnitude;
                    if (segmentSize > onLineSize) segmentDir = segmentDir.normalized * onLineSize;
                }
                var normal = -settings.projectionDirection;
                var otherAxes = AxesUtils.GetOtherAxes((AxesUtils.SignedAxis)(-settings.projectionDirection));
                var tangetAxis = otherAxes[settings.objectsOrientedAlongTheLine ? 0 : 1];
                Vector3 itemTangent = (AxesUtils.SignedAxis)(tangetAxis);
                var itemRotation = Quaternion.LookRotation(itemTangent, normal);
                var lookAt = Quaternion.LookRotation((Vector3)(AxesUtils.SignedAxis)
                    (settings.axisOrientedAlongTheLine), Vector3.up);
                if (segmentDir != Vector3.zero) itemRotation = Quaternion.LookRotation(segmentDir, normal) * lookAt;
                var pivotPosition = strokeItem.tangentPosition
                    - segmentDir.normalized * Mathf.Abs(AxesUtils.GetAxisValue(pivotToCenterLocal, tangetAxis));
                var itemPosition = pivotPosition + segmentDir / 2;

                var ray = new Ray(itemPosition + normal * height, -normal);
                if (settings.mode != LineSettings.PaintMode.ON_SHAPE)
                {
                    if (MouseRaycast(ray, out RaycastHit itemHit,
                        out GameObject collider, height * 2f, -1,
                        settings.paintOnPalettePrefabs, settings.paintOnMeshesWithoutCollider))
                    {
                        itemPosition = itemHit.point;
                        normal = itemHit.normal;
                    }
                    else if (settings.mode == LineSettings.PaintMode.ON_SURFACE) continue;
                }

                
                BrushSettings brushSettings = strokeItem.settings;
                if (settings.overwriteBrushProperties) brushSettings = settings.brushSettings;
                if (brushSettings.rotateToTheSurface && segmentDir != Vector3.zero)
                    itemRotation = Quaternion.LookRotation(segmentDir, normal) * lookAt;

                itemPosition += normal * brushSettings.surfaceDistance;
                itemRotation *= Quaternion.Euler(strokeItem.additionalAngle);
                if (brushSettings.embedInSurface && settings.mode != LineSettings.PaintMode.ON_SHAPE)
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
    #endregion
}
