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

namespace PluginMaster
{
    #region DATA & SETTINGS
    [Serializable]
    public class ExtrudeSettings : SelectionToolBaseBasic, IToolSettings, IPaintToolSettings
    {
        [SerializeField] private Space _space = Space.World;
        [SerializeField] private Vector3 _spacing = Vector3.zero;
        public enum SpacingType { BOX_SIZE, CUSTOM }
        [SerializeField] private SpacingType _spacingType = SpacingType.CUSTOM;
        [SerializeField] private Vector3 _multiplier = Vector3.one;

        public enum RotationAccordingTo { FRIST_SELECTED, LAST_SELECTED }
        [SerializeField] private RotationAccordingTo _rotationAccordingTo = RotationAccordingTo.FRIST_SELECTED;

        [SerializeField] private bool _sameParentAsSource = true;

        public Space space
        {
            get => _space;
            set
            {
                if (_space == value) return;
                _space = value;
                DataChanged();
            }
        }
        public Vector3 multiplier
        {
            get => _multiplier;
            set
            {
                if (_multiplier == value) return;
                _multiplier = value;
                DataChanged();
            }
        }

        public RotationAccordingTo rotationAccordingTo
        {
            get => _rotationAccordingTo;
            set
            {
                if (_rotationAccordingTo == value) return;
                _rotationAccordingTo = value;
                DataChanged();
            }
        }

        public Vector3 spacing
        {
            get => _spacing;
            set
            {
                if (_spacing == value) return;
                _spacing = value;
                DataChanged();
            }
        }
        public SpacingType spacingType
        {
            get => _spacingType;
            set
            {
                if (_spacingType == value) return;
                _spacingType = value;
                DataChanged();
            }
        }

        public ExtrudeSettings Clone()
        {
            var clone = new ExtrudeSettings();
            clone.Copy(this);
            return clone;
        }

        public override void Copy(IToolSettings other)
        {
            var otherExtrudeSettings = other as ExtrudeSettings;
            if (otherExtrudeSettings == null) return;
            base.Copy(other);
            _paintTool.Copy(otherExtrudeSettings._paintTool);
            _sameParentAsSource = otherExtrudeSettings._sameParentAsSource;
            _space = otherExtrudeSettings._space;
            _multiplier = otherExtrudeSettings._multiplier;
            _rotationAccordingTo = otherExtrudeSettings._rotationAccordingTo;
            _spacing = otherExtrudeSettings._spacing;
            _spacingType = otherExtrudeSettings._spacingType;
        }

        public bool sameParentAsSource
        {
            get => _sameParentAsSource;
            set
            {
                if (_sameParentAsSource == value) return;
                _sameParentAsSource = value;
                DataChanged();
            }
        }

        #region PAINT TOOL
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
        #endregion
    }

    [Serializable]
    public class ExtrudeManager : ToolManagerBase<ExtrudeSettings> { }
    #endregion
    #region PWBIO
    public static partial class PWBIO
    {
        private static Vector3 _extrudeHandlePosition;
        private static Vector3Int _extrudeDirection;
        private static Vector3 _initialExtrudePosition;
        private static Vector3 _selectionSize;
        private static Vector3 _deltaSnapped;
        private static Vector3 _extrudeSpacing;
        private static void ExtrudeDuringSceneGUI(SceneView sceneView)
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                ResetUnityCurrentTool();
                ToolManager.DeselectTool();
                return;
            }
            if (SelectionManager.topLevelSelection.Length == 0) return;
            ExtrudeInput();
            if (SelectionManager.topLevelSelection.Length == 0) return;
            var settings = ExtrudeManager.settings;
            if (Tools.current != Tool.None)
            {
                SaveUnityCurrentTool();
                Tools.current = Tool.None;
            }
            var anchor = settings.rotationAccordingTo == ExtrudeSettings.RotationAccordingTo.FRIST_SELECTED
                ? SelectionManager.topLevelSelection.First().transform
                : SelectionManager.topLevelSelection.Last().transform;
            var handlePosition = Handles.PositionHandle(_extrudeHandlePosition,
                settings.space == Space.World ? Quaternion.identity : anchor.rotation);
            var handleDelta = handlePosition - _extrudeHandlePosition;
            _extrudeHandlePosition = handlePosition;
            var delta = _extrudeHandlePosition - _initialExtrudePosition;
            if (settings.space == Space.Self)
            {
                handleDelta = anchor.InverseTransformVector(handleDelta);
                delta = anchor.InverseTransformVector(delta);
            }

            if (delta.sqrMagnitude > 0.01)
            {
                var direction = Vector3Int.one;
                var absDelta = new Vector3(Mathf.Abs(handleDelta.x),
                    Mathf.Abs(handleDelta.y), Mathf.Abs(handleDelta.z));
                direction.x = (absDelta.x <= absDelta.y || absDelta.x <= absDelta.z) ? 0 : (int)Mathf.Sign(delta.x);
                direction.y = (absDelta.y <= absDelta.x || absDelta.y <= absDelta.z) ? 0 : (int)Mathf.Sign(delta.y);
                direction.z = (absDelta.z <= absDelta.x || absDelta.z <= absDelta.y) ? 0 : (int)Mathf.Sign(delta.z);
                var directionChanged = direction != Vector3Int.zero && _extrudeDirection != direction;
                if (handleDelta != Vector3.zero && directionChanged && _extrudeDirection != Vector3.zero
                    && _extrudeDirection != (direction * -1)) CreateExtrudedObjects(anchor);
                if (directionChanged) _extrudeDirection = direction;
                _extrudeSpacing = _selectionSize + (settings.spacingType == ExtrudeSettings.SpacingType.BOX_SIZE
                    ? Vector3.Scale(_selectionSize, settings.multiplier - Vector3.one)
                    : settings.spacing);
                _deltaSnapped = new Vector3(
                    Mathf.Floor((Mathf.Abs(delta.x) + _selectionSize.x / 2f) / _extrudeSpacing.x)
                    * _extrudeSpacing.x * Mathf.Sign(delta.x),
                    Mathf.Floor((Mathf.Abs(delta.y) + _selectionSize.y / 2f) / _extrudeSpacing.y)
                    * _extrudeSpacing.y * Mathf.Sign(delta.y),
                    Mathf.Floor((Mathf.Abs(delta.z) + _selectionSize.z / 2f) / _extrudeSpacing.z)
                    * _extrudeSpacing.z * Mathf.Sign(delta.z));
                if (_deltaSnapped != Vector3.zero) PreviewExtrudedObjects(sceneView.camera, anchor);
            }
        }

        private static Vector3 GetExtrudeStep(Transform anchor)
        {
            var step = Vector3.Scale(_extrudeSpacing, _extrudeDirection);
            if (ExtrudeManager.settings.space == Space.Self)
            {
                if (anchor.lossyScale.x != 0) step.x /= anchor.lossyScale.x;
                if (anchor.lossyScale.y != 0) step.y /= anchor.lossyScale.y;
                if (anchor.lossyScale.z != 0) step.z /= anchor.lossyScale.z;
            }
            return step;
        }

        private static void PreviewExtrudedObjects(Camera camera, Transform anchor)
        {
            var step = GetExtrudeStep(anchor);
            var settings = ExtrudeManager.settings;
            foreach (var obj in SelectionManager.topLevelSelection)
            {
                var delta = step;
                do
                {
                    var deltaPos = settings.space == Space.World ? delta : anchor.TransformVector(delta);
                    var localToWorld = Matrix4x4.Translate(deltaPos);
                    if (settings.embedInSurface)
                    {
                        var bottomVertices = BoundsUtils.GetBottomVertices(obj.transform, Space.Self);
                        var height = BoundsUtils.GetMagnitude(obj.transform) * 3;
                        Vector3 position = anchor.position + deltaPos;
                        var rotation = anchor.rotation;
                        var TRS = Matrix4x4.TRS(position, rotation, obj.transform.lossyScale);
                        var surfceDistance = settings.embedAtPivotHeight
                        ? GetPivotDistanceToSurfaceSigned(position, height, true, true)
                        : GetBottomDistanceToSurfaceSigned(bottomVertices, TRS, height, true, true);
                        surfceDistance -= settings.surfaceDistance;
                        position += new Vector3(0f, -surfceDistance, 0f);
                        deltaPos += new Vector3(0f, -surfceDistance, 0f);
                        localToWorld = Matrix4x4.Translate(deltaPos);
                    }

                    PreviewBrushItem(obj, localToWorld, obj.layer, camera);
                    delta += step;
                } while (Mathf.Abs(delta.x) <= Mathf.Abs(_deltaSnapped.x)
                && Mathf.Abs(delta.y) <= Mathf.Abs(_deltaSnapped.y)
                && Mathf.Abs(delta.z) <= Mathf.Abs(_deltaSnapped.z));
            }
        }
        
        private static void CreateExtrudedObjects(Transform anchor)
        {
            if (SelectionManager.topLevelSelection.Length == 0 || _extrudeDirection == Vector3Int.zero
                || _deltaSnapped == Vector3.zero) return;
            var newSelection = new List<GameObject>();
            _initialExtrudePosition += Vector3.Scale(_extrudeDirection, _deltaSnapped);
            _extrudeHandlePosition = _initialExtrudePosition;
            var step = GetExtrudeStep(anchor);
            foreach (var obj in SelectionManager.topLevelSelection)
            {
                var delta = step;
                GameObject extruded = null;
                var parent = GetParent(ExtrudeManager.settings, obj.name, true);
                if (ExtrudeManager.settings.sameParentAsSource) parent = obj.transform.parent;
                do
                {
                    var position = obj.transform.position
                        + (ExtrudeManager.settings.space == Space.World ? delta : anchor.TransformVector(delta));
                    var rotation = obj.transform.rotation;
                    extruded = PrefabUtility.IsOutermostPrefabInstanceRoot(obj)
                        ? (GameObject)PrefabUtility.InstantiatePrefab(
                            PrefabUtility.GetCorrespondingObjectFromSource(obj))
                        : GameObject.Instantiate(obj);
                    extruded.transform.position = position;
                    extruded.transform.rotation = rotation;
                    extruded.transform.localScale = obj.transform.lossyScale;
                    extruded.transform.parent = parent;
                    if (ExtrudeManager.settings.overwritePrefabLayer)
                        extruded.layer = ExtrudeManager.settings.layer;
                    Undo.RegisterCreatedObjectUndo(extruded, "Extrude");
                    delta += step;
                } while (Mathf.Abs(delta.x) <= Mathf.Abs(_deltaSnapped.x)
                && Mathf.Abs(delta.y) <= Mathf.Abs(_deltaSnapped.y)
                && Mathf.Abs(delta.z) <= Mathf.Abs(_deltaSnapped.z));
                newSelection.Add(extruded);
            }
            Selection.objects = newSelection.ToArray();
        }

        private static void ExtrudeInput()
        {
            if (SelectionManager.topLevelSelection.First() == null || SelectionManager.topLevelSelection.Last() == null)
                SelectionManager.UpdateSelection();
            if (SelectionManager.topLevelSelection.Length == 0) return;
            var anchor
                = ExtrudeManager.settings.rotationAccordingTo == ExtrudeSettings.RotationAccordingTo.FRIST_SELECTED
                ? SelectionManager.topLevelSelection.First().transform
                : SelectionManager.topLevelSelection.Last().transform;
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
                CreateExtrudedObjects(anchor);
        }
    }
    #endregion
}
