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
using UnityEngine.Rendering;

namespace PluginMaster
{
    #region DATA & SETTINGS
    [Serializable]
    public class ShapeData : ISerializationCallbackReceiver
    {
        public enum ShapeState { NONE, RADIUS, EDIT }
        private const string COMMAND_NAME = "Edit Shape";
        [SerializeField] private ShapeState _state = ShapeState.NONE;
        [SerializeField] private float _radius = 0f;
        [SerializeField] private float _arcAngle = 360f;
        [SerializeField] private List<Vector3> _points = new List<Vector3>();
        [SerializeField] private int _selectedPointIdx = -1;
        [SerializeField] private Vector3 _normal = Vector3.up;
        [SerializeField] private Plane _plane;
        [SerializeField] private int _firstVertexIdxAfterIntersection = 2;
        [SerializeField] private int _lastVertexIdxBeforeIntersection = 1;
        [SerializeField] private Vector3[] _arcIntersections = new Vector3[2];
        private int _circleSideCount = 8;
        public ShapeState state
        {
            get => _state;
            set
            {
                if (_state == value) return;
                ToolProperties.RegisterUndo(COMMAND_NAME);
                _state = value;
            }
        }
        public Vector3 normal
        {
            get => _normal;
            set
            {
                if (_normal == value) return;
                _normal = value;
            }
        }
        public Vector3[] points => _points.ToArray();
        public Vector3 GetPoint(int idx)
        {
            if (idx < 0) idx = points.Length + idx;
            return _points.Count > idx ? _points[idx] : Vector3.zero;
        }
        public Vector3 selectedPoint => _points[_selectedPointIdx];
        public int circleSideCount => _circleSideCount;
        public void SetCenter(Vector3 value, Vector3 normal)
        {
            if (_points.Count == 0)
            {
                _points.Add(value);
                _points.Add(value);

            }
            else if (_points[0] != value)
            {
                _points[1] = _points[0] = value;
            }
            _normal = normal;
            _plane = new Plane(_normal, _points[0]);
            if (ShapeManager.settings.projectInNormalDir) ShapeManager.settings.UpdateProjectDirection(-_normal);
        }

        public void SetRadius(Vector3 point)
        {
            _points[1] = point;
            _radius = (_points[1] - _points[0]).magnitude;
            if (ShapeManager.settings.shapeType == ShapeSettings.ShapeType.CIRCLE) UpdateCircleSideCount();
        }
        public float radius => _radius;
        public Vector3 radiusPoint => _points[1];
        public Plane plane => _plane;
        public Vector3 center => _points[0];
        public int pointCount => _points.Count;
        public float arcAngle => _arcAngle;
        public Vector3 GetArcIntersection(int idx) => _arcIntersections[idx];
        public int firstVertexIdxAfterIntersection => _firstVertexIdxAfterIntersection;
        public int lastVertexIdxBeforeIntersection => _lastVertexIdxBeforeIntersection;

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
        public void SetHandlePoints(Vector3[] vertices)
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            if (_points.Count > 2) _points.RemoveRange(2, _points.Count - 2);
            var midPoints = new List<Vector3>();
            for (int i = 1; i < vertices.Length; ++i)
            {
                _points.Add(vertices[i]);
                if (ShapeManager.settings.shapeType == ShapeSettings.ShapeType.POLYGON)
                    midPoints.Add((vertices[i] - vertices[i - 1]) / 2 + vertices[i - 1]);
            }
            if (ShapeManager.settings.shapeType == ShapeSettings.ShapeType.POLYGON)
            {
                midPoints.Add((vertices[vertices.Length - 1] - vertices[0]) / 2 + vertices[0]);
                _points.AddRange(midPoints);
            }
            var arcPoint = _points[1] + (_points[1] - _points[0]);
            _points.Add(arcPoint);
            _points.Add(arcPoint);
            _arcIntersections[0] = _points[1];
            _arcIntersections[1] = _points[1];
        }

        public Vector3[] vertices => _points.GetRange(1,
            ShapeManager.settings.shapeType == ShapeSettings.ShapeType.POLYGON
            ? ShapeManager.settings.sidesCount : _circleSideCount).ToArray();

        public Quaternion planeRotation
        {
            get
            {
                var forward = Vector3.Cross(_normal, Vector3.right);
                if (forward.sqrMagnitude < 0.000001) forward = Vector3.Cross(_normal, Vector3.down);
                return Quaternion.LookRotation(forward, _normal);
            }
        }

        private static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1,
            Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {
            Vector3 lineVec3 = linePoint2 - linePoint1;
            Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);
            float planarFactor = Mathf.Abs(90 - Vector3.Angle(lineVec3, crossVec1and2));
            if (planarFactor < 0.01f && crossVec1and2.sqrMagnitude > 0.001f)
            {
                float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
                intersection = linePoint1 + (lineVec1 * s);
                var min = Vector3.Max(Vector3.Min(linePoint1, linePoint1 + lineVec1),
                    Vector3.Min(linePoint2, linePoint2 + lineVec2));
                var max = Vector3.Min(Vector3.Max(linePoint1, linePoint1 + lineVec1),
                    Vector3.Max(linePoint2, linePoint2 + lineVec2));
                var tolerance = Vector3.one * 0.001f;
                var minComp = intersection + tolerance - min;
                var maxComp = max + tolerance - intersection;
                var result = minComp.x >= 0 && minComp.y >= 0 && minComp.z >= 0
                    && maxComp.x >= 0 && maxComp.y >= 0 && maxComp.z >= 0;
                return result;
            }
            else
            {
                intersection = Vector3.zero;
                return false;
            }
        }

        public void UpdateIntersections()
        {
            var centerToArc1 = GetPoint(-1) - ShapeData.instance.center;
            var centerToArc2 = GetPoint(-2) - ShapeData.instance.center;

            bool firstPointFound = false;
            bool lastPointFound = false;
            var sidesCount = ShapeManager.settings.shapeType == ShapeSettings.ShapeType.POLYGON
                ? ShapeManager.settings.sidesCount : _circleSideCount;
            int GetNextVertexIdx(int currentIdx) => currentIdx == sidesCount ? 1 : currentIdx + 1;
            for (int i = 1; i <= sidesCount; ++i)
            {
                var startPoint = ShapeData.instance.GetPoint(i);
                var endIdx = GetNextVertexIdx(i);
                var endPoint = ShapeData.instance.GetPoint(endIdx);
                var startToEnd = endPoint - startPoint;
                if (!firstPointFound)
                {
                    if (LineLineIntersection(out Vector3 intersection, ShapeData.instance.center, centerToArc1,
                        startPoint, startToEnd))
                    {
                        firstPointFound = true;
                        _firstVertexIdxAfterIntersection = endIdx;
                        _arcIntersections[0] = intersection;
                    }
                }
                if (!lastPointFound)
                {
                    if (LineLineIntersection(out Vector3 intersection, ShapeData.instance.center, centerToArc2,
                        startPoint, startToEnd))
                    {
                        lastPointFound = true;
                        _lastVertexIdxBeforeIntersection = i;
                        _arcIntersections[1] = intersection;
                    }
                }
                if (firstPointFound && lastPointFound) break;
            }

        }

        public Quaternion rotation
        {
            get
            {
                var radiusVector = radiusPoint - center;
                return Quaternion.LookRotation(radiusVector, _normal);
            }
            set
            {
                var prevRadiusVector = radiusPoint - center;
                var prev = Quaternion.LookRotation(prevRadiusVector, normal);
                _plane.normal = _normal = value * Vector3.up;
                var delta = value * Quaternion.Inverse(prev);
                for (int i = 0; i < _points.Count - 2; ++i) _points[i] = delta * (_points[i] - center) + center;
                _points[_points.Count - 1] = delta * (_points[_points.Count - 1] - center).normalized
                    * _radius * 2f + center;
                _points[_points.Count - 2] = delta * (_points[_points.Count - 2] - center).normalized
                    * _radius * 2f + center;
                UpdateIntersections();
                if (ShapeManager.settings.projectInNormalDir) ShapeManager.settings.UpdateProjectDirection(-_normal);
            }
        }

        public void MovePoint(int idx, Vector3 position)
        {
            if (position == _points[idx]) return;

            ToolProperties.RegisterUndo(COMMAND_NAME);
            var delta = position - _points[idx];
            if (idx == 0)
            {
                for (int i = 0; i < _points.Count; ++i) _points[i] += delta;
                _arcIntersections[0] += delta;
                _arcIntersections[1] += delta;
            }
            else
            {
                var normalDelta = Vector3.Project(delta, _normal);
                var centerToPoint = _points[idx] - center;
                var radiusDelta = Vector3.Project(delta, centerToPoint);
                var newRadius = position - center - normalDelta;
                var angle = Vector3.SignedAngle(centerToPoint, newRadius, _normal);
                var rotation = Quaternion.AngleAxis(angle, _normal);
                if ((ShapeManager.settings.shapeType == ShapeSettings.ShapeType.CIRCLE && idx == 1)
                  || (ShapeManager.settings.shapeType == ShapeSettings.ShapeType.POLYGON
                  && idx <= ShapeManager.settings.sidesCount * 2))
                {
                    _radius = newRadius.magnitude;
                    var radiusScale = _radius < 0.1f ? 1f : 1f + radiusDelta.magnitude / _radius
                        * (Vector3.Dot(centerToPoint, radiusDelta) >= 0 ? 1f : -1f);
                    for (int i = 0; i < _points.Count - 2; ++i) _points[i] = rotation * (_points[i] - center)
                            * radiusScale + normalDelta + center;
                    _points[_points.Count - 1] = rotation * (_points[_points.Count - 1] - center).normalized
                        * _radius * 2f + center + normalDelta;
                    _points[_points.Count - 2] = rotation * (_points[_points.Count - 2] - center).normalized
                        * _radius * 2f + center + normalDelta;
                    if (ShapeManager.settings.shapeType == ShapeSettings.ShapeType.CIRCLE)
                        if (UpdateCircleSideCount()) Update(false);
                }
                else
                {
                    _points[idx] = rotation * (_points[idx] - center) + center;
                    if (normalDelta != Vector3.zero)
                    {
                        for (int i = 0; i < _points.Count; ++i) _points[i] += normalDelta;
                    }
                    _arcAngle = Vector3.SignedAngle(GetPoint(-1) - center, GetPoint(-2) - center, normal);
                    if (_arcAngle <= 0) _arcAngle += 360;
                }
                UpdateIntersections();
            }
        }

        public bool UpdateCircleSideCount()
        {
             var perimenter = 2 * Mathf.PI * _radius;
             var maxItemSize = 1f;
             if (PaletteManager.selectedBrush != null)
             {
                 maxItemSize = float.MinValue;
                 for (int i = 0; i < PaletteManager.selectedBrush.itemCount; ++i)
                     maxItemSize = Mathf.Max(BrushstrokeManager.GetLineSpacing(i, ShapeManager.settings), maxItemSize);
             }
             var prevCount = _circleSideCount;
             _circleSideCount = Mathf.FloorToInt(perimenter / maxItemSize);
             var sideLenght = 2 * _radius * Mathf.Sin(Mathf.PI / _circleSideCount);
             if (sideLenght <= maxItemSize) --_circleSideCount;
             _circleSideCount = Mathf.Max(_circleSideCount, 8);
             return prevCount != _circleSideCount;
        }

        public void Reset()
        {
            ToolProperties.RegisterUndo(COMMAND_NAME);
            _arcIntersections[0] = _arcIntersections[1] = Vector3.zero;
            _points.Clear();
            _selectedPointIdx = -1;
            _state = ShapeState.NONE;
            _radius = 0f;
            _arcAngle = 360f;
            _normal = Vector3.up;
            _plane = new Plane();
            _firstVertexIdxAfterIntersection = 2;
            _lastVertexIdxBeforeIntersection = 1;
            _circleSideCount = 8;
        }

        public void Update(bool clearSelection)
        {
            if (_points.Count < 2) return;
            ToolProperties.RegisterUndo(COMMAND_NAME);
            if (clearSelection) _selectedPointIdx = -1;
            var arcPoints = _points.GetRange(_points.Count - 2, 2);
            var center = _points[0];
            var polygonVertices = PWBIO.GetPolygonVertices();
            _points.Clear();
            _points.Add(center);
            _points.AddRange(polygonVertices);
            if (ShapeManager.settings.shapeType == ShapeSettings.ShapeType.POLYGON)
            {
                for (int i = 1; i < polygonVertices.Length; ++i)
                    _points.Add((polygonVertices[i] - polygonVertices[i - 1]) / 2 + polygonVertices[i - 1]);
                _points.Add((polygonVertices[polygonVertices.Length - 1] - polygonVertices[0]) / 2 + polygonVertices[0]);
            }
            _points.AddRange(arcPoints);
        }

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            PWBIO.repaint = true;
        }

        private ShapeData() { }
        private static ShapeData _instance = null;
        public static ShapeData instance
        {
            get
            {
                if (_instance == null) _instance = new ShapeData();
                return _instance;
            }
        }
    }

    [Serializable]
    public class ShapeSettings : LineSettings
    {
        public enum ShapeType { CIRCLE, POLYGON }
        [SerializeField] private ShapeType _shapeType = ShapeType.POLYGON;
        [SerializeField] private int _sidesCount = 5;
        [SerializeField] private bool _axisNormalToSurface = true;
        [SerializeField] private Vector3 _normal = Vector3.up;
        [SerializeField] private bool _projectInNormalDir = true;

        public ShapeType shapeType
        {
            get => _shapeType;
            set
            {
                if (_shapeType == value) return;
                _shapeType = value;
                OnDataChanged();
            }
        }
        public int sidesCount
        {
            get => _sidesCount;
            set
            {
                value = Mathf.Max(value, 3);
                if (_sidesCount == value) return;
                _sidesCount = value;
                OnDataChanged();
            }
        }
        public bool axisNormalToSurface
        {
            get => _axisNormalToSurface;
            set
            {
                if (_axisNormalToSurface == value) return;
                _axisNormalToSurface = value;
                OnDataChanged();
            }
        }
        public Vector3 normal
        {
            get => _normal;
            set
            {
                if (_normal == value) return;
                _normal = value;
                OnDataChanged();
            }
        }
        public bool projectInNormalDir
        {
            get => _projectInNormalDir;
            set
            {
                if (_projectInNormalDir == value) return;
                _projectInNormalDir = value;
                OnDataChanged();
            }
        }
        public override void Copy(IToolSettings other)
        {
            var otherShapeSettings = other as ShapeSettings;
            if (otherShapeSettings == null) return;
            base.Copy(other);
            _shapeType = otherShapeSettings._shapeType;
            _sidesCount = otherShapeSettings._sidesCount;
            _axisNormalToSurface = otherShapeSettings._axisNormalToSurface;
            _normal = otherShapeSettings._normal;
            _projectInNormalDir = otherShapeSettings._projectInNormalDir;
        }

        public override LineSettings Clone()
        {
            var clone = new ShapeSettings();
            clone.Copy(this);
            return clone;
        }

        public override void DataChanged()
        {
            base.DataChanged();
            ShapeData.instance.Update(true);
        }
    }

    [Serializable]
    public class ShapeManager : ToolManagerBase<ShapeSettings> { }
    #endregion

    #region PWBIO
    public static partial class PWBIO
    {
        private static ShapeData _shapeData = ShapeData.instance;

        public static void ResetShapeState()
        {
            _snappedToVertex = false;
            _shapeData.Reset();
        }
        private static void ShapeDuringSceneGUI(SceneView sceneView)
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                if (_shapeData.state == ShapeData.ShapeState.EDIT && _shapeData.selectedPointIdx > 0)
                    _shapeData.selectedPointIdx = -1;
                else if (_shapeData.state == ShapeData.ShapeState.NONE) ToolManager.DeselectTool();
                else ResetShapeState();
            }
            switch (_shapeData.state)
            {
                case ShapeData.ShapeState.NONE:
                    ShapeStateNone(sceneView.in2DMode);
                    break;
                case ShapeData.ShapeState.RADIUS:
                    ShapeStateRadius(sceneView.in2DMode);
                    break;
                case ShapeData.ShapeState.EDIT:
                    ShapeStateEdit(sceneView.camera);
                    break;
            }
        }

        private static Vector3 ClosestPointOnPlane(Vector3 point)
        {
            var plane = new Plane(_shapeData.planeRotation * Vector3.up, _shapeData.center);
            return plane.ClosestPointOnPlane(point);
        }

        private static void ShapeStateNone(bool in2DMode)
        {
            if (Event.current.button == 0 && Event.current.type == EventType.MouseDown && !Event.current.alt)
                _shapeData.state = ShapeData.ShapeState.RADIUS;
            if (MouseDot(out Vector3 point, out Vector3 normal, ShapeManager.settings.mode, in2DMode,
                ShapeManager.settings.paintOnPalettePrefabs, ShapeManager.settings.paintOnMeshesWithoutCollider, false))
            {
                point = SnapAndUpdateGridOrigin(point, SnapManager.settings.snappingEnabled,
                   ShapeManager.settings.paintOnPalettePrefabs, ShapeManager.settings.paintOnMeshesWithoutCollider,
                   false);
                _shapeData.SetCenter(point,
                    ShapeManager.settings.axisNormalToSurface ? normal : ShapeManager.settings.normal);
            }
            DrawDotHandleCap(_shapeData.GetPoint(0));
        }
        private static void ShapeStateRadius(bool in2DMode)
        {
            if (Event.current.button == 0 && Event.current.type == EventType.MouseDown && !Event.current.alt)
            {
                _shapeData.SetHandlePoints(GetPolygonVertices());
                _shapeData.state = ShapeData.ShapeState.EDIT;
                _updateStroke = true;
            }
            var mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if (_snapToVertex)
            {
                if (SnapToVertex(mouseRay, out RaycastHit closestVertexInfo, in2DMode))
                    mouseRay.origin = closestVertexInfo.point - mouseRay.direction;
            }

            var radiusPoint = _shapeData.center;
            if (_shapeData.plane.Raycast(mouseRay, out float distance))
                radiusPoint = mouseRay.GetPoint(distance);
            radiusPoint = SnapAndUpdateGridOrigin(radiusPoint, SnapManager.settings.snappingEnabled,
                   ShapeManager.settings.paintOnPalettePrefabs, ShapeManager.settings.paintOnMeshesWithoutCollider,
                   false);
            radiusPoint = ClosestPointOnPlane(radiusPoint);
            _shapeData.SetRadius(radiusPoint);
            DrawShapeLines();
            DrawDotHandleCap(_shapeData.center);
            DrawDotHandleCap(_shapeData.radiusPoint);
        }
        private static void ShapeStateEdit(Camera camera)
        {
            var isCircle = ShapeManager.settings.shapeType == ShapeSettings.ShapeType.CIRCLE;
            var isPolygon = ShapeManager.settings.shapeType == ShapeSettings.ShapeType.POLYGON;
            if (Event.current.type == EventType.Repaint)
            {
                if (_updateStroke)
                {
                    BrushstrokeManager.UpdateShapeBrushstroke();
                    SceneView.RepaintAll();
                    _updateStroke = false;
                }
                ShapeStrokePreview(camera);
              
                DrawShapeLines();
                DrawDotHandleCap(_shapeData.center);
                if (isPolygon)
                    foreach (var vertex in _shapeData.vertices) DrawDotHandleCap(vertex);
                else DrawDotHandleCap(_shapeData.radiusPoint);
                if (_shapeData.selectedPointIdx >= 0)
                    DrawDotHandleCap(_shapeData.selectedPoint, 1f, 1.2f);
                DrawDotHandleCap(_shapeData.GetPoint(-1));
                DrawDotHandleCap(_shapeData.GetPoint(-2));
            }
            else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                ResetShapeState();
                Paint(ShapeManager.settings);
            }
            else if (Event.current.button == 1 && Event.current.type == EventType.MouseDrag && Event.current.shift)
            {
                var deltaSign = Mathf.Sign(Event.current.delta.x + Event.current.delta.y);
                ShapeManager.settings.gapSize += Mathf.PI * _shapeData.radius * deltaSign * 0.001f;
                ToolProperties.RepainWindow();
                Event.current.Use();
            }
            
            bool clickOnPoint = false;
            for (int i = 0; i < _shapeData.pointCount; ++i)
            {
                if (isCircle && i == 2) i = _shapeData.pointCount - 2;
                var controlId = GUIUtility.GetControlID(FocusType.Passive);
                if (!clickOnPoint)
                {
                    float distFromMouse = HandleUtility.DistanceToRectangle(_shapeData.GetPoint(i),
                        _shapeData.planeRotation, 0f);
                    HandleUtility.AddControl(controlId, distFromMouse);
                    if (HandleUtility.nearestControl != controlId) continue;
                    if (isPolygon) DrawDotHandleCap(_shapeData.GetPoint(i));
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
                    {
                        _shapeData.selectedPointIdx = i;
                        clickOnPoint = true;
                        Event.current.Use();
                    }
                }
            }

            if (_shapeData.selectedPointIdx >= 0)
            {
                var prevPosition = _shapeData.selectedPoint;
                var snappedPoint = Handles.PositionHandle(prevPosition, _shapeData.planeRotation);
                snappedPoint = SnapAndUpdateGridOrigin(snappedPoint, SnapManager.settings.snappingEnabled,
                   ShapeManager.settings.paintOnPalettePrefabs, ShapeManager.settings.paintOnMeshesWithoutCollider,
                   false);
                snappedPoint = ClosestPointOnPlane(snappedPoint);
                if (prevPosition != snappedPoint)
                {
                    _shapeData.MovePoint(_shapeData.selectedPointIdx, snappedPoint);
                    _updateStroke = true;
                }
                if(_shapeData.selectedPointIdx == 0)
                {
                    var prevRotation = _shapeData.rotation;
                    var rotation = Handles.RotationHandle(prevRotation, _shapeData.center);
                    if(prevRotation != rotation)
                    {
                        _shapeData.rotation = rotation;
                        _updateStroke = true;
                    }
                }
            }
        }

        public static Vector3[] GetPolygonVertices()
        {
            var tangent = Vector3.Cross(Vector3.left, _shapeData.normal);
            if (tangent.sqrMagnitude < 0.000001) tangent = Vector3.Cross(Vector3.forward, _shapeData.normal);
            var bitangent = Vector3.Cross(_shapeData.normal, tangent);

            var polygonSides = ShapeManager.settings.shapeType == ShapeSettings.ShapeType.CIRCLE
                ? _shapeData.circleSideCount : ShapeManager.settings.sidesCount;

            var periPoints = new List<Vector3>();
            var centerToRadius = _shapeData.radiusPoint - _shapeData.center;
            var sign = Vector3.Dot(Vector3.Cross(tangent, centerToRadius), _shapeData.normal) > 0 ? 1f : -1f;
            float mouseAngle = Vector3.Angle(tangent, centerToRadius) * Mathf.Deg2Rad * sign;

            for (int i = 0; i < polygonSides; ++i)
            {
                var radians = TAU * i / polygonSides + mouseAngle;
                var tangentDir = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
                var worldDir = TangentSpaceToWorld(tangent, bitangent, tangentDir).normalized;
                periPoints.Add(_shapeData.center + (worldDir * _shapeData.radius));
            }
            return periPoints.ToArray();
        }

        public static Vector3[] GetArcVertices(float radius)
        {
            var tangent = Vector3.Cross(Vector3.left, _shapeData.normal);
            if (tangent.sqrMagnitude < 0.000001) tangent = Vector3.Cross(Vector3.forward, _shapeData.normal);
            var bitangent = Vector3.Cross(_shapeData.normal, tangent);

            const float polygonSideSize = 0.3f;
            const int minPolygonSides = 8;
            const int maxPolygonSides = 60;
            var polygonSides = Mathf.Clamp((int)(TAU * radius / polygonSideSize), minPolygonSides, maxPolygonSides);

            var periPoints = new List<Vector3>();
            var centerToRadius = _shapeData.GetPoint(-1) - _shapeData.center;
            var sign = Vector3.Dot(Vector3.Cross(tangent, centerToRadius), _shapeData.normal) > 0 ? 1 : -1;
            float firstAngle = Vector3.Angle(tangent, centerToRadius) * Mathf.Deg2Rad * sign;
            var sideDelta = TAU / polygonSides * Mathf.Sign(_shapeData.arcAngle);

            for (int i = 0; i <= polygonSides; ++i)
            {
                var delta = sideDelta * i;
                bool arcComplete = false;
                if (Mathf.Abs(delta * Mathf.Rad2Deg) > Mathf.Abs(_shapeData.arcAngle))
                {
                    delta = _shapeData.arcAngle * Mathf.Deg2Rad;
                    arcComplete = true;
                }
                var radians = delta + firstAngle;
                var tangentDir = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
                var worldDir = TangentSpaceToWorld(tangent, bitangent, tangentDir).normalized;
                periPoints.Add(_shapeData.center + (worldDir * radius));
                if (arcComplete) break;
            }
            return periPoints.ToArray();
        }

        private static void DrawShapeLines()
        {
            if (_shapeData.radius < 0.0001) return;
            var points = new List<Vector3>(_shapeData.state == ShapeData.ShapeState.RADIUS
                ? GetPolygonVertices() : _shapeData.vertices);
            points.Add(points[0]);
            var pointsArray = points.ToArray();
            Handles.zTest = CompareFunction.Always;
            Handles.color = new Color(0f, 0f, 0f, 0.7f);
            Handles.DrawAAPolyLine(8, pointsArray);
            Handles.color = new Color(1f, 1f, 1f, 0.7f);
            Handles.DrawAAPolyLine(4, pointsArray);
            if (_shapeData.state != ShapeData.ShapeState.EDIT) return;
            var arcLines = new Vector3[] { _shapeData.GetPoint(-1), _shapeData.center, _shapeData.GetPoint(-2) };
            Handles.color = new Color(0f, 0f, 0f, 0.7f);
            Handles.DrawAAPolyLine(4, arcLines);
            Handles.color = new Color(1f, 1f, 1f, 0.7f);
            Handles.DrawAAPolyLine(2, arcLines);
            var arcPoints = GetArcVertices(_shapeData.radius * 1.5f);
            Handles.color = new Color(0f, 0f, 0f, 0.7f);
            Handles.DrawAAPolyLine(4, arcPoints);
            Handles.color = new Color(1f, 1f, 1f, 0.7f);
            Handles.DrawAAPolyLine(2, arcPoints);
        }

        private static void ShapeStrokePreview(Camera camera)
        {
            _paintStroke.Clear();
            var settings = ShapeManager.settings;
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

                var normal = -settings.projectionDirection;
                var tangent = (ShapeData.instance.center - strokeItem.tangentPosition).normalized;
                if(settings.objectsOrientedAlongTheLine) tangent = Vector3.Cross(normal, tangent);

                var itemRotation = Quaternion.identity;
                var itemPosition = strokeItem.tangentPosition;

                var ray = new Ray(itemPosition + normal * height, -normal);
                if (settings.mode != LineSettings.PaintMode.ON_SHAPE)
                {
                    if (MouseRaycast(ray, out RaycastHit itemHit, out GameObject collider,
                        height * 2f, -1,
                        settings.paintOnPalettePrefabs, settings.paintOnMeshesWithoutCollider))
                    {
                        itemPosition = itemHit.point;
                        normal = itemHit.normal;
                    }
                    else if (settings.mode == LineSettings.PaintMode.ON_SURFACE) continue;
                }

                if (brushSettings.rotateToTheSurface)
                {
                    var itemTangent = GetTangent(normal);
                    itemRotation = Quaternion.LookRotation(itemTangent, normal);
                    itemPosition += normal * brushSettings.surfaceDistance;
                }
                else itemPosition += normal * brushSettings.surfaceDistance;
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
