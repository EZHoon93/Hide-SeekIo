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

namespace PluginMaster
{
    #region DATA & SETTINGS
    [Serializable]
    public class SelectionToolSettings : SelectionToolBase, IToolSettings
    {
        [SerializeField] private bool _move = true;
        [SerializeField] private bool _rotate = false;
        [SerializeField] private bool _scale = false;
        [SerializeField] private Space _handleSpace = Space.Self;
        [SerializeField] private Space _boxSpace = Space.Self;

        public Space handleSpace
        {
            get => _handleSpace;
            set
            {
                if (_handleSpace == value) return;
                _handleSpace = value;
                if (_handleSpace == Space.World) _scale = false;
                DataChanged();
            }
        }

        public bool move
        {
            get => _move;
            set
            {
                if (_move == value) return;
                _move = value;
                DataChanged();
            }
        }
        public bool rotate
        {
            get => _rotate;
            set
            {
                if (_rotate == value) return;
                _rotate = value;
                DataChanged();
            }
        }
        public bool scale
        {
            get => _scale;
            set
            {
                if (_scale == value) return;
                _scale = value;
                if (_scale) _handleSpace = Space.Self;
                DataChanged();
            }
        }

        public Space boxSpace
        {
            get => _boxSpace;
            set
            {
                if (_boxSpace == value) return;
                _boxSpace = value;
                DataChanged();
            }
        }

        public override void Copy(IToolSettings other)
        {
            var otherSelectionToolSettings = other as SelectionToolSettings;
            if (otherSelectionToolSettings == null) return;
            base.Copy(other);
            _move = otherSelectionToolSettings._move;
            _rotate = otherSelectionToolSettings._rotate;
            _scale = otherSelectionToolSettings._scale;
            _handleSpace = otherSelectionToolSettings._handleSpace;
            _boxSpace = otherSelectionToolSettings._boxSpace;
        }
    }

    [Serializable]
    public class SelectionToolManager : ToolManagerBase<SelectionToolSettings> { }
    #endregion

    #region PWBIO
    public static partial class PWBIO
    {
        private static int _selectedBoxPointIdx = -1;
        private static Quaternion _selectionRotation = Quaternion.identity;
        private static Vector3 _selectionScale = Vector3.one;
        private static Vector3 _snappedPoint;
        private static bool _snappedPointIsVisible = false;
        private static bool _snappedPointIsSelected = false;
        private static (Vector3 position, GameObject[] selection) _selectionMoveFrom;
        private static bool _selectionMoving = false;
        private static bool _editingSelectionHandlePosition = false;
        private static Vector3 _tempSelectionHandle = Vector3.zero;
        private static bool _selectionChanged = false;
        private static Bounds _selectionBounds;
        private static void SelectionDuringSceneGUI(SceneView sceneView)
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                _snappedPointIsSelected = false;
                _selectionMoving = false;
                if (_selectedBoxPointIdx >= 0 && _selectedBoxPointIdx != 10) _selectedBoxPointIdx = 10;
                else
                {
                    ResetUnityCurrentTool();
                    ToolManager.DeselectTool();
                    return;
                }
            }
            if (Tools.current != Tool.None) Tools.current = Tool.None;
            if (SelectionManager.topLevelSelection.Length == 0) return;

            var points = SelectionPoints(sceneView.camera);

            SelectionInput(points, sceneView.in2DMode);
            if (_selectionMoving)
            {
                Handles.CircleHandleCap(0, _selectionMoveFrom.position, sceneView.camera.transform.rotation,
                    HandleUtility.GetHandleSize(_selectionMoveFrom.position) * 0.06f, EventType.Repaint);
                if (_selectedBoxPointIdx >= 0)
                    Handles.DrawLine(_selectionMoveFrom.position, points[_selectedBoxPointIdx]);
            }

            bool mouseDown = Event.current.button == 0 && Event.current.type == EventType.MouseDown;
            bool clickOnPoint = false;

            bool SelectPoint(Vector3 point, int i)
            {
                if (_editingSelectionHandlePosition) return false;
                if (clickOnPoint) return false;
                var controlId = GUIUtility.GetControlID(FocusType.Passive);
                var distFromMouse = HandleUtility.DistanceToRectangle(point, Quaternion.identity, 0f);
                HandleUtility.AddControl(controlId, distFromMouse);
                if (HandleUtility.nearestControl != controlId) return false;
                DrawDotHandleCap(point, 1f, 1.2f);
                if (!mouseDown) return false;
                _selectedBoxPointIdx = i;
                clickOnPoint = true;
                Event.current.Use();
                return true;
            }

            for (int i = 0; i < points.Count; ++i)
            {
                if (SelectPoint(points[i], i)) _snappedPointIsSelected = false;
                if (clickOnPoint) break;
            }

            if (_snappedPointIsVisible || _snappedPointIsSelected)
            {
                points.Add(_snappedPoint);
                if (SelectPoint(_snappedPoint, points.Count - 1)) _snappedPointIsSelected = true;
            }
            if (_selectionChanged)
            {
                _tempSelectionHandle = Vector3.zero;
                _selectionChanged = false;
            }

            if (_editingSelectionHandlePosition)
            {
                _selectedBoxPointIdx = 11;
                Handles.CircleHandleCap(0, points[11], sceneView.camera.transform.rotation,
                    HandleUtility.GetHandleSize(points[11]) * 0.06f, EventType.Repaint);
            }
            if (_selectedBoxPointIdx >= 0)
            {
                var rotation = GetSelectionRotation();
                if (_editingSelectionHandlePosition)
                {
                    var delta = points[_selectedBoxPointIdx];
                    points[_selectedBoxPointIdx] = Handles.PositionHandle(points[_selectedBoxPointIdx], rotation);
                    delta = points[_selectedBoxPointIdx] - delta;
                    _tempSelectionHandle += delta;
                }
                else
                {
                    MoveSelection(rotation, points, sceneView);
                    RotateSelection(rotation, points);
                    ScaleSelection(rotation, points);
                }
            }
            else _editingSelectionHandlePosition = false;
        }

        private static void SelectionInput(List<Vector3> points, bool in2DMode)
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.W)
            {
                SelectionToolManager.settings.move = !SelectionToolManager.settings.move;
                PWBToolbar.RepaintWindow();
            }
            else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.E)
            {
                SelectionToolManager.settings.rotate = !SelectionToolManager.settings.rotate;
                PWBToolbar.RepaintWindow();
            }
            else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.R)
            {
                SelectionToolManager.settings.scale = !SelectionToolManager.settings.scale;
                PWBToolbar.RepaintWindow();
            }
            else if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.U
                || (_editingSelectionHandlePosition && (Event.current.keyCode == KeyCode.Escape
                || Event.current.keyCode == KeyCode.Return))))
            {
                _editingSelectionHandlePosition = !_editingSelectionHandlePosition;
            }
            else if (_snappedToVertex && _selectedBoxPointIdx < 0)
            {
                _snappedPointIsVisible = false;
                var mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                if (SnapToVertex(mouseRay, out RaycastHit snappedHit, in2DMode, SelectionManager.topLevelSelection))
                {
                    _snappedPoint = snappedHit.point;
                    _snappedPointIsVisible = true;
                }
            }
            else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return
                && _selectedBoxPointIdx >= 0)
            {
                _editingSelectionHandlePosition = false;
                if (_selectionMoving)
                {
                    var delta = points[_selectedBoxPointIdx] - _selectionMoveFrom.position;
                    foreach (var obj in _selectionMoveFrom.selection)
                    {
                        if (obj == null) continue;
                        Undo.RecordObject(obj.transform, "Move Selection");
                        obj.transform.position += delta;
                    }
                    _selectionMoving = false;
                    SelectionManager.UpdateSelection();
                    _selectedBoxPointIdx = -1;
                }
                else
                {
                    _selectionMoveFrom = (points[_selectedBoxPointIdx], SelectionManager.topLevelSelection);
                    _selectionMoving = true;
                }
            }
            else if (Event.current.type == EventType.KeyDown && Event.current.control
                && Event.current.keyCode == KeyCode.UpArrow) RotateSelection90Deg(Vector3.right, points);
            else if (Event.current.type == EventType.KeyDown && Event.current.control
                && Event.current.keyCode == KeyCode.DownArrow) RotateSelection90Deg(Vector3.left, points);
            else if (Event.current.type == EventType.KeyDown && Event.current.control
                && Event.current.keyCode == KeyCode.RightArrow) RotateSelection90Deg(Vector3.up, points);
            else if (Event.current.type == EventType.KeyDown && Event.current.control
               && Event.current.keyCode == KeyCode.LeftArrow) RotateSelection90Deg(Vector3.down, points);
            else if (Event.current.type == EventType.KeyDown && Event.current.control
                && Event.current.keyCode == KeyCode.PageUp) RotateSelection90Deg(Vector3.forward, points);
            else if (Event.current.type == EventType.KeyDown && Event.current.control
                && Event.current.keyCode == KeyCode.PageDown) RotateSelection90Deg(Vector3.back, points);
            else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.X)
            {
                ToolProperties.RegisterUndo("Tool properties");
                SelectionToolManager.settings.handleSpace = SelectionToolManager.settings.handleSpace == Space.Self
                    ? Space.World : Space.Self;
                if (SelectionToolManager.settings.handleSpace == Space.World) ResetSelectionRotation();
                SceneView.RepaintAll();
                ToolProperties.RepainWindow();
                Event.current.Use();
            }
        }

        private static void EmbedSelectionInSurface(Quaternion rotation)
        {
            PWBCore.SetActiveTempColliders(SelectionManager.topLevelSelection, false);
            var placeOnSurfaceData = new PlaceOnSurfaceUtils.PlaceOnSurfaceData();
            placeOnSurfaceData.projectionDirectionSpace = Space.World;
            placeOnSurfaceData.rotateToSurface = false;
            var objHeight = new float[SelectionManager.topLevelSelection.Length];
            for(int i = 0; i < SelectionManager.topLevelSelection.Length; ++i)
            {
                var obj = SelectionManager.topLevelSelection[i];
                objHeight[i] = BoundsUtils.GetMagnitude(obj.transform);
                obj.SetActive(false);
            }
            for (int i = 0; i < SelectionManager.topLevelSelection.Length; ++i)
            {
                var obj = SelectionManager.topLevelSelection[i];
                var bottomVertices = BoundsUtils.GetBottomVertices(obj.transform, Space.World);
                var height = objHeight[i];
                var TRS = obj.transform.localToWorldMatrix;
                var surfceDistance = SelectionToolManager.settings.embedAtPivotHeight
                    ? GetPivotDistanceToSurfaceSigned(obj.transform.position, height, true, true)
                    : GetBottomDistanceToSurfaceSigned(bottomVertices, TRS, height, true, true);
                surfceDistance -= SelectionToolManager.settings.surfaceDistance;
                obj.transform.position += new Vector3(0f, -surfceDistance, 0f);
                if (SelectionToolManager.settings.rotateToTheSurface)
                {
                    var down = obj.transform.rotation * Vector3.down;
                    var ray = new Ray(obj.transform.position - down * height, down);
                    if (MouseRaycast(ray, out RaycastHit hitInfo, out GameObject collider,
                    float.MaxValue, -1, true, true))
                    {
                        var tangent = Vector3.Cross(hitInfo.normal, Vector3.left);
                        if (tangent.sqrMagnitude < 0.000001) tangent = Vector3.Cross(hitInfo.normal, Vector3.back);
                        tangent = tangent.normalized;
                        obj.transform.rotation = Quaternion.LookRotation(tangent, hitInfo.normal);
                    }
                }
            }
            foreach (var obj in SelectionManager.topLevelSelection) obj.SetActive(true);
            _selectionBounds = BoundsUtils.GetSelectionBounds(SelectionManager.topLevelSelection, rotation);
            PWBCore.SetActiveTempColliders(SelectionManager.topLevelSelection, true);
        }

        public static void EmbedSelectionInSurface() => EmbedSelectionInSurface(_selectionRotation);

        private static void RotateSelection90Deg(Vector3 axis, List<Vector3> points)
        {
            var rotation = _selectionRotation;
            foreach (var obj in SelectionManager.topLevelSelection)
            {
                if (obj == null)
                {
                    SelectionManager.UpdateSelection();
                    return;
                }
                Undo.RecordObject(obj.transform, "Rotate Selection");
                obj.transform.RotateAround(points[_selectedBoxPointIdx < 0 ? 10 : _selectedBoxPointIdx],
                    rotation * axis, 90);
            }
            _selectionRotation = rotation * Quaternion.AngleAxis(90, axis);
            var localCenter = _selectionBounds.center - points[_selectedBoxPointIdx];
            _selectionBounds.center = (Quaternion.AngleAxis(90, axis) * localCenter) + points[_selectedBoxPointIdx];
            if (SelectionToolManager.settings.embedInSurface) EmbedSelectionInSurface();
            PWBCore.UpdateTempCollidersTransforms(SelectionManager.topLevelSelection);
        }

        private static void MoveSelection(Quaternion rotation, List<Vector3> points, SceneView sceneView)
        {
            if (!SelectionToolManager.settings.move) return;
            if (SelectionToolManager.settings.handleSpace == Space.World) rotation = Quaternion.identity;
            else if (SelectionManager.topLevelSelection.Length == 1)
            {
                rotation = SelectionManager.topLevelSelection[0].transform.rotation;
            }
            var prevPosition = points[_selectedBoxPointIdx];
            points[_selectedBoxPointIdx] = Handles.PositionHandle(points[_selectedBoxPointIdx], rotation);
            if (_snappedPointIsSelected) _snappedPoint = points[_selectedBoxPointIdx];

            points[_selectedBoxPointIdx] = SnapAndUpdateGridOrigin(points[_selectedBoxPointIdx],
                SnapManager.settings.snappingEnabled, true, true, false);

            if (prevPosition == points[_selectedBoxPointIdx]) return;
            var delta = points[_selectedBoxPointIdx] - prevPosition;

            foreach (var obj in SelectionManager.topLevelSelection)
            {
                if (obj == null)
                {
                    SelectionManager.UpdateSelection();
                    return;
                }
                Undo.RecordObject(obj.transform, "Move Selection");
                obj.transform.position += delta;
            }
            _selectionBounds.center += delta;
            if (SelectionToolManager.settings.embedInSurface) EmbedSelectionInSurface();
            PWBCore.UpdateTempCollidersTransforms(SelectionManager.topLevelSelection);
        }

        private static void RotateSelection(Quaternion rotation, List<Vector3> points)
        {
            if (!SelectionToolManager.settings.rotate) return;
            if (SelectionToolManager.settings.handleSpace == Space.Self && SelectionManager.topLevelSelection.Length == 1)
            {
                rotation = SelectionManager.topLevelSelection[0].transform.rotation;
            }
            var prevRotation = rotation;
            var newRotation = Handles.RotationHandle(prevRotation, points[_selectedBoxPointIdx]);
            if (prevRotation == newRotation) return;
            _selectionRotation = newRotation;
            var angle = Quaternion.Angle(prevRotation, newRotation);
            var axis = Vector3.Cross(prevRotation * Vector3.forward, newRotation * Vector3.forward);
            if (axis == Vector3.zero) axis = Vector3.Cross(prevRotation * Vector3.up, newRotation * Vector3.up);
            axis.Normalize();
            foreach (var obj in SelectionManager.topLevelSelection)
            {
                if (obj == null)
                {
                    SelectionManager.UpdateSelection();
                    return;
                }
                Undo.RecordObject(obj.transform, "Rotate Selection");
                obj.transform.RotateAround(points[_selectedBoxPointIdx], axis, angle);
            }
            var localCenter = _selectionBounds.center - points[_selectedBoxPointIdx];
            _selectionBounds.center = (Quaternion.AngleAxis(angle, axis) * localCenter)
                + points[_selectedBoxPointIdx];
            if (SelectionToolManager.settings.embedInSurface) EmbedSelectionInSurface(_selectionRotation);
            PWBCore.UpdateTempCollidersTransforms(SelectionManager.topLevelSelection);
        }

        private static void ScaleSelection(Quaternion rotation, List<Vector3> points)
        {
            if (!SelectionToolManager.settings.scale) return;
            var prevScale = _selectionScale;
            var newScale = Handles.ScaleHandle(prevScale, points[_selectedBoxPointIdx],
                rotation, HandleUtility.GetHandleSize(points[_selectedBoxPointIdx]) * 1.4f);
            if (prevScale == newScale) return;
            _selectionScale = newScale;
            var scaleFactor = new Vector3(
                prevScale.x == 0 ? newScale.x : newScale.x / prevScale.x,
                prevScale.y == 0 ? newScale.y : newScale.y / prevScale.y,
                prevScale.z == 0 ? newScale.z : newScale.z / prevScale.z);
            var pivot = new GameObject();
            pivot.hideFlags = HideFlags.HideAndDontSave;
            pivot.transform.position = points[_selectedBoxPointIdx];
            pivot.transform.rotation = rotation;
            foreach (var obj in SelectionManager.topLevelSelection)
            {
                if (obj == null)
                {
                    SelectionManager.UpdateSelection();
                    break;
                }
                Undo.RecordObject(obj.transform, "Scale Selection");
                pivot.transform.localScale = Vector3.one;
                var localPosition = pivot.transform.InverseTransformPoint(obj.transform.position);
                pivot.transform.localScale = scaleFactor;
                obj.transform.position = pivot.transform.TransformPoint(localPosition);
                obj.transform.localScale = Vector3.Scale(obj.transform.localScale, scaleFactor);
            }
            GameObject.DestroyImmediate(pivot);
            var pivotToCenter = _selectionBounds.center - points[_selectedBoxPointIdx];
            _selectionBounds.center = points[_selectedBoxPointIdx] + Vector3.Scale(pivotToCenter, scaleFactor);
            _selectionBounds.size = Vector3.Scale(_selectionBounds.size, scaleFactor);
            if (SelectionToolManager.settings.embedInSurface) EmbedSelectionInSurface();
            PWBCore.UpdateTempCollidersTransforms(SelectionManager.topLevelSelection);
        }
        public static void ResetSelectionRotation()
        {
            _selectionRotation = Quaternion.identity;
            UpdateSelection();
        }

        private static Quaternion GetSelectionRotation()
        {
            var rotation = _selectionRotation;
            if (SelectionManager.topLevelSelection.Length == 1)
            {
                if (SelectionManager.topLevelSelection[0] == null) SelectionManager.UpdateSelection();
                else if(SelectionToolManager.settings.boxSpace == Space.Self)
                    rotation = SelectionManager.topLevelSelection[0].transform.rotation;
            }
            else if (SelectionToolManager.settings.handleSpace == Space.Self)
            {
                var count = 0;
                var avgForward = Vector3.forward;
                var avgUp = Vector3.up;
                foreach (var obj in SelectionManager.topLevelSelection)
                {
                    if (obj == null) continue;
                    ++count;
                    avgForward += obj.transform.rotation * Vector3.forward;
                    avgUp += obj.transform.rotation * Vector3.up;
                }
                avgForward /= count;
                avgUp /= count;
                rotation = Quaternion.LookRotation(avgForward, avgUp);
            }
            return rotation;
        }

        private static List<Vector3> SelectionPoints(Camera camera)
        {
            var rotation = GetSelectionRotation();
            var bounds = _selectionBounds;
            var halfSizeRotated = rotation * bounds.size / 2;
            var min = bounds.center - halfSizeRotated;
            var max = bounds.center + halfSizeRotated;
            var points = new List<Vector3>
            {
                min,
                min + rotation * new Vector3(bounds.size.x, 0f, 0f),
                min + rotation * new Vector3(bounds.size.x, 0f, bounds.size.z),
                min + rotation * new Vector3(0f, 0f, bounds.size.z),
                min + rotation * new Vector3(0f, bounds.size.y, 0f),
                min + rotation * new Vector3(bounds.size.x, bounds.size.y, 0f),
                max,
                min + rotation * new Vector3(0f, bounds.size.y, bounds.size.z),
                min + rotation * new Vector3(bounds.size.x, 0f, bounds.size.z) / 2,
                max - rotation * new Vector3(bounds.size.x, 0f, bounds.size.z) / 2,
            };

            var visibleIdx = GetVisiblePoints(points.ToArray(), camera);

            points.Add(bounds.center);
            points.Add(bounds.center + _selectionRotation * _tempSelectionHandle);

            void DrawLine(Vector3[] line, float alpha)
            {
                Handles.color = new Color(0f, 0f, 0f, 0.5f);
                Handles.DrawAAPolyLine(10, line);
                Handles.color = new Color(1f, 1f, 1f, 0.3f * alpha);
                Handles.DrawAAPolyLine(4, line);
            }

            var visibleLines = new List<Vector3[]>();
            float ocludedAlpha = 0.5f;
            for (int i = 0; i < 8; ++i)
            {
                var visibleLine = visibleIdx.Contains(i) && visibleIdx.Contains(i + 4);
                if (i < 4)
                {
                    var vLine = new Vector3[] { points[i],
                        points[i] + rotation * new Vector3(0f, bounds.size.y, 0f) };
                    if (visibleLine) visibleLines.Add(vLine);
                    else DrawLine(vLine, ocludedAlpha);
                    points.Add(vLine[0] + (vLine[1] - vLine[0]) / 2);
                }
                int nextI = ((i + 1) % 4) + 4 * (i / 4);
                visibleLine = visibleIdx.Contains(i) && visibleIdx.Contains(nextI);
                var hLine = new Vector3[] { points[i], points[nextI] };
                if (visibleLine) visibleLines.Add(hLine);
                else DrawLine(hLine, ocludedAlpha);
                var midpoint = hLine[0] + (hLine[1] - hLine[0]) / 2;
                points.Add(midpoint);
                if (i < 4) points.Add(midpoint + rotation * new Vector3(0f, bounds.size.y / 2, 0f));
            }
            foreach (var line in visibleLines) DrawLine(line, 1f);
            for (int i = 0; i < 8; ++i)
            {
                var alpha = visibleIdx.Contains(i) ? 1f : 0.3f;
                DrawDotHandleCap(points[i], alpha);
            }
            DrawDotHandleCap(points[11], 1);
            return points;
        }

        #region VISIBLE POINTS
        private static int[] GetVisiblePoints(Vector3[] points, Camera camera)
        {
            var resultSet = new HashSet<int>(GrahamScan(points));
            if (resultSet.Count == 6)
            {
                var ocluded = new List<int>();
                for (int i = 0; i < points.Length; ++i)
                {
                    if (resultSet.Contains(i)) continue;
                    ocluded.Add(i);
                }
                if ((ocluded[0] / 4 == ocluded[1] / 4) || (ocluded[1] == ocluded[0] + 4))
                    return resultSet.ToArray();
                var nearestIdx = camera.transform.InverseTransformPoint(points[ocluded[0]]).z
                    < camera.transform.InverseTransformPoint(points[ocluded[1]]).z ? ocluded[0] : ocluded[1];
                resultSet.Add(nearestIdx);
            }
            return resultSet.ToArray();
        }
        private static int[] GrahamScan(Vector3[] points)
        {
            var screenPoints = new List<BoxPoint>();
            for (int i = 0; i < points.Length; ++i)
                screenPoints.Add(new BoxPoint(i, HandleUtility.WorldToGUIPoint(points[i])));
            var p0 = screenPoints[0];
            foreach (var value in screenPoints)
            {
                if (p0.point.y > value.point.y) p0 = value;
            }
            var order = new List<BoxPoint>();
            foreach (var point in screenPoints)
            {
                if (p0 != point) order.Add(point);
            }
            order = MergeSort(p0, order);
            var result = new List<BoxPoint>();
            result.Add(p0);
            result.Add(order[0]);
            result.Add(order[1]);
            order.RemoveAt(0);
            order.RemoveAt(0);
            foreach (var value in order) KeepLeft(result, value);
            var resultIdx = new int[result.Count];
            for (int i = 0; i < result.Count; ++i) resultIdx[i] = result[i];
            return resultIdx;
        }

        private class BoxPoint
        {
            public int idx = -1;
            public Vector2 point = Vector2.zero;
            public BoxPoint(int idx, Vector2 point) => (this.idx, this.point) = (idx, point);
            public override int GetHashCode()
            {
                int hashCode = 386348313;
                hashCode = hashCode * -1521134295 + idx.GetHashCode();
                hashCode = hashCode * -1521134295 + point.GetHashCode();
                return hashCode;
            }
            public bool Equals(BoxPoint other) => GetHashCode() == other.GetHashCode();
            public override bool Equals(object obj) => Equals(obj as BoxPoint);
            public static bool operator ==(BoxPoint l, BoxPoint r) => l.Equals(r);
            public static bool operator !=(BoxPoint l, BoxPoint r) => !l.Equals(r);
            public static implicit operator Vector2(BoxPoint value) => value.point;
            public static implicit operator int(BoxPoint value) => value.idx;
        }

        private static List<BoxPoint> MergeSort(BoxPoint p0, List<BoxPoint> pointList)
        {
            if (pointList.Count == 1) return pointList;
            var sortedList = new List<BoxPoint>();
            int middle = pointList.Count / 2;
            var leftArray = pointList.GetRange(0, middle);
            var rightArray = pointList.GetRange(middle, pointList.Count - middle);
            leftArray = MergeSort(p0, leftArray);
            rightArray = MergeSort(p0, rightArray);
            int leftptr = 0;
            int rightptr = 0;
            for (int i = 0; i < leftArray.Count + rightArray.Count; i++)
            {
                if (leftptr == leftArray.Count)
                {
                    sortedList.Add(rightArray[rightptr]);
                    rightptr++;
                }
                else if (rightptr == rightArray.Count)
                {
                    sortedList.Add(leftArray[leftptr]);
                    leftptr++;
                }
                else if (GetAngle(p0, leftArray[leftptr]) < GetAngle(p0, rightArray[rightptr]))
                {
                    sortedList.Add(leftArray[leftptr]);
                    leftptr++;
                }
                else
                {
                    sortedList.Add(rightArray[rightptr]);
                    rightptr++;
                }
            }
            return sortedList;
        }

        private static double GetAngle(Vector2 p1, Vector2 p2)
        {
            float xDiff = p2.x - p1.x;
            float yDiff = p2.y - p1.y;
            return Mathf.Atan2(yDiff, xDiff) * 180f / Mathf.PI;
        }

        private static void KeepLeft(List<BoxPoint> hull, BoxPoint point)
        {
            int turn(Vector2 p, Vector2 q, Vector2 r)
                => ((q.x - p.x) * (r.y - p.y) - (r.x - p.x) * (q.y - p.y)).CompareTo(0);
            while (hull.Count > 1 && turn(hull[hull.Count - 2], hull[hull.Count - 1], point) != 1)
                hull.RemoveAt(hull.Count - 1);
            if (hull.Count == 0 || hull[hull.Count - 1] != point) hull.Add(point);
        }
        #endregion
    }
    #endregion
}
