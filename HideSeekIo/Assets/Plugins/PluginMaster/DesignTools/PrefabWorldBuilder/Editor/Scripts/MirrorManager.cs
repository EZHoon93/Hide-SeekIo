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

namespace PluginMaster
{
    #region DATA & SETTINGS
    [Serializable]
    public class MirrorSettings : SelectionToolBase, IPaintToolSettings, IToolSettings, ISerializationCallbackReceiver
    {
        public enum MirrorAction { TRANSFORM, CREATE}
        [SerializeField] private bool _reflectRotation = true;
        [SerializeField] private MirrorAction _action = MirrorAction.CREATE;
        [SerializeField] private bool _sameParentAsSource = true;
        [SerializeField] private Pose _mirrorPose = new Pose(Vector3.zero, Quaternion.LookRotation(Vector3.right, Vector3.up));
        [SerializeField] private bool _invertScale = false;

        private const string COMMAND_NAME = "Edit Mirror";
        public bool reflectRotation
        {
            get => _reflectRotation;
            set
            {
                if (_reflectRotation == value) return;
                _reflectRotation = value;
                DataChanged();
            }
        }

        public MirrorAction action
        {
            get => _action;
            set
            {
                if (_action == value) return;
                _action = value;
                DataChanged();
            }
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
        public Pose mirrorPose => _mirrorPose;

        public Vector3 mirrorPosition
        {
            get => _mirrorPose.position;
            set
            {
                if (_mirrorPose.position == value) return;
                ToolProperties.RegisterUndo(COMMAND_NAME);
                _mirrorPose.position = value;
                DataChanged();
            }
        }
        public Quaternion mirrorRotation
        {
            get => _mirrorPose.rotation;
            set
            {
                if (_mirrorPose.rotation == value) return;
                ToolProperties.RegisterUndo(COMMAND_NAME);
                _mirrorPose.rotation = value;
                DataChanged();
            }
        }
        public bool invertScale
        {
            get => _invertScale;
            set
            {
                if (_invertScale == value) return;
                _invertScale = value;
                DataChanged();
            }
        }

        public override void Copy(IToolSettings other)
        {
            var otherMirrorSettings = other as MirrorSettings;
            if (otherMirrorSettings == null) return;
            base.Copy(other);
            _paintTool.Copy(otherMirrorSettings._paintTool);
            _reflectRotation = otherMirrorSettings.reflectRotation;
            _action = otherMirrorSettings._action;
            _sameParentAsSource = otherMirrorSettings._sameParentAsSource;
            _mirrorPose = otherMirrorSettings._mirrorPose;
            _invertScale = otherMirrorSettings._invertScale;
        }

        public MirrorSettings Clone()
        {
            var clone = new MirrorSettings();
            clone.Copy(this);
            return clone;
        }

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() => PWBIO.repaint = true;

        #region PAINT TOOL
        [SerializeField] private PaintToolSettings _paintTool = new PaintToolSettings();
        public Transform parent { get => _paintTool.parent; set => _paintTool.parent = value; }
        public bool overwritePrefabLayer { get => _paintTool.overwritePrefabLayer;
            set => _paintTool.overwritePrefabLayer = value; }
        public int layer { get => _paintTool.layer; set => _paintTool.layer = value; }
        public bool autoCreateParent { get => _paintTool.autoCreateParent; set => _paintTool.autoCreateParent = value; }
        public bool createSubparent { get => _paintTool.createSubparent; set => _paintTool.createSubparent = value; }
        public bool overwriteBrushProperties {get => _paintTool.overwriteBrushProperties;
            set => _paintTool.overwriteBrushProperties = value; }
        public BrushSettings brushSettings => _paintTool.brushSettings;
        #endregion
    }

    [Serializable]
    public class MirrorManager : ToolManagerBase<MirrorSettings> { }
    #endregion

    #region PWBIO
    public static partial class PWBIO
    {
        private static bool _showMirrorHandles = true;
        private static GameObject _mirrorObject = null;
        private static Transform _mirroredTransform = null;

        public static void InitializeMirrorPose()
        {
            if (_sceneViewCamera == null) return;
            MirrorManager.settings.mirrorPosition = Vector3.zero;
            var camRay = new Ray(_sceneViewCamera.transform.position, _sceneViewCamera.transform.forward);
            if (Physics.Raycast(camRay, out RaycastHit hit, float.MaxValue, -1))
                MirrorManager.settings.mirrorPosition = hit.point;
            if (GridRaycast(camRay, out RaycastHit gridHit))
                MirrorManager.settings.mirrorPosition = gridHit.point;
            MirrorManager.settings.mirrorPosition = SnapAndUpdateGridOrigin(MirrorManager.settings.mirrorPosition,
                SnapManager.settings.snappingEnabled, true, true, false);
            MirrorManager.settings.mirrorRotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
            _showMirrorHandles = true;
        }

        private static void MirrorDuringSceneGUI(SceneView sceneView)
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                if (_showMirrorHandles) _showMirrorHandles = false;
                else ToolManager.DeselectTool();
            }
            DrawMirror();
            if (SelectionManager.topLevelSelection.Length == 0) return;
            PreviewMirror();
            MirrorInput();
        }

        private static void MirrorInput()
        {
            void Rotate90(Vector3 axis) => MirrorManager.settings.mirrorRotation *= Quaternion.AngleAxis(90, axis);
            if (Event.current.type == EventType.KeyDown && Event.current.control
               && Event.current.keyCode == KeyCode.UpArrow) Rotate90(Vector3.right);
            else if (Event.current.type == EventType.KeyDown && Event.current.control
                && Event.current.keyCode == KeyCode.DownArrow) Rotate90(Vector3.left);
            else if (Event.current.type == EventType.KeyDown && Event.current.control
                && Event.current.keyCode == KeyCode.RightArrow) Rotate90(Vector3.up);
            else if (Event.current.type == EventType.KeyDown && Event.current.control
               && Event.current.keyCode == KeyCode.LeftArrow) Rotate90(Vector3.down);
            else if (Event.current.type == EventType.KeyDown && Event.current.control
                && Event.current.keyCode == KeyCode.PageUp) Rotate90(Vector3.forward);
            else if (Event.current.type == EventType.KeyDown && Event.current.control
                && Event.current.keyCode == KeyCode.PageDown) Rotate90(Vector3.back);

            var confirm = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return;
            if (!confirm) return;
            if (MirrorManager.settings.action == MirrorSettings.MirrorAction.CREATE)
                Paint(MirrorManager.settings, "Mirror");
            else
            {
                foreach (var item in _paintStroke)
                {
                    Undo.RecordObject(item.prefab.transform, "Mirror");
                    item.prefab.transform.position = item.position;
                    item.prefab.transform.rotation = item.rotation;
                }
            }
            ToolManager.DeselectTool();
        }

        private static void DrawMirror()
        {
            Handles.color = Color.yellow;
            var handleSize = HandleUtility.GetHandleSize(Vector3.zero);
            Handles.RectangleHandleCap(0, MirrorManager.settings.mirrorPosition
                - MirrorManager.settings.mirrorPose.forward * handleSize * 0.02f,
                MirrorManager.settings.mirrorRotation, handleSize, EventType.Repaint);
            Handles.RectangleHandleCap(0, MirrorManager.settings.mirrorPosition
                + MirrorManager.settings.mirrorPose.forward * handleSize * 0.02f,
                MirrorManager.settings.mirrorRotation, handleSize, EventType.Repaint);
            Handles.color = Color.black;
            Handles.RectangleHandleCap(0, MirrorManager.settings.mirrorPosition,
                MirrorManager.settings.mirrorPose.rotation, handleSize * 1.2f, EventType.Repaint);
            if (_showMirrorHandles)
            {
                var prevPose = MirrorManager.settings.mirrorPose;
                MirrorManager.settings.mirrorPosition = Handles.PositionHandle(MirrorManager.settings.mirrorPosition,
                    MirrorManager.settings.mirrorRotation);
                MirrorManager.settings.mirrorPosition = SnapAndUpdateGridOrigin(MirrorManager.settings.mirrorPosition,
                SnapManager.settings.snappingEnabled, true, true, false);
                MirrorManager.settings.mirrorRotation = Handles.RotationHandle(MirrorManager.settings.mirrorRotation,
                    MirrorManager.settings.mirrorPosition);
                if (prevPose != MirrorManager.settings.mirrorPose) ToolProperties.RepainWindow();
            }
            else
            {
                DrawDotHandleCap(MirrorManager.settings.mirrorPosition);
                var controlId = GUIUtility.GetControlID(FocusType.Passive);
                var distFromMouse = HandleUtility.DistanceToRectangle(MirrorManager.settings.mirrorPosition,
                    Quaternion.identity, 0f);
                HandleUtility.AddControl(controlId, distFromMouse);
                if (HandleUtility.nearestControl == controlId && Event.current.button == 0
                    && Event.current.type == EventType.MouseDown) _showMirrorHandles = true;
            }
        }

        private static void PreviewMirror()
        {
            _paintStroke.Clear();
            if (_mirrorObject == null)
            {
                _mirrorObject = new GameObject("PluginMasterMirror");
                _mirrorObject.hideFlags = HideFlags.HideAndDontSave;
                _mirroredTransform = new GameObject("PluginMasterMirrorTempTransform").transform;
                _mirroredTransform.gameObject.hideFlags = HideFlags.HideAndDontSave;
            }
            var settings = MirrorManager.settings;
            _mirrorObject.transform.position = settings.mirrorPosition;
            _mirrorObject.transform.rotation = settings.mirrorRotation;
            foreach (var obj in SelectionManager.topLevelSelection)
            {
                if (obj == null) continue;
                _mirrorObject.transform.localScale = Vector3.one;
                _mirroredTransform.position = obj.transform.position;
                _mirroredTransform.rotation = obj.transform.rotation;
                _mirroredTransform.localScale = obj.transform.lossyScale;
                _mirroredTransform.parent = _mirrorObject.transform;
                _mirrorObject.transform.localScale = new Vector3(1f, 1f, -1f);
                if (!MirrorManager.settings.reflectRotation) _mirroredTransform.rotation = obj.transform.rotation;

                var previewScale = Vector3.one;
                var scale = _mirroredTransform.localScale;
                if (settings.invertScale)
                {
                    scale *= -1;
                    previewScale *= -1;
                    var angle = new Vector3(180 - _mirroredTransform.rotation.eulerAngles.x,
                        settings.reflectRotation
                        ? _mirroredTransform.rotation.eulerAngles.y + 180
                        : 180 - _mirroredTransform.rotation.eulerAngles.y,
                        _mirroredTransform.rotation.eulerAngles.z);
                    _mirroredTransform.rotation = Quaternion.Euler(angle);
                }
                if (settings.embedInSurface)
                {
                    var TRS = Matrix4x4.TRS(_mirroredTransform.position, _mirroredTransform.rotation, previewScale);
                    var bottomVertices = BoundsUtils.GetBottomVertices(obj.transform, Space.Self);
                    var height = BoundsUtils.GetMagnitude(obj.transform) * 3;
                    var surfceDistance = settings.embedAtPivotHeight
                    ? GetPivotDistanceToSurfaceSigned(_mirroredTransform.position, height, true, true)
                    : GetBottomDistanceToSurfaceSigned(bottomVertices, TRS, height, true, true);
                    surfceDistance -= settings.surfaceDistance;
                    _mirroredTransform.position += new Vector3(0f, -surfceDistance, 0f);
                    if (settings.rotateToTheSurface)
                    {
                        var down = _mirroredTransform.rotation * Vector3.down;
                        var ray = new Ray(_mirroredTransform.position - down * height, down);
                        if (MouseRaycast(ray, out RaycastHit hitInfo, out GameObject collider,
                        float.MaxValue, -1, true, true))
                        {
                            var tangent = Vector3.Cross(hitInfo.normal, Vector3.left);
                            if (tangent.sqrMagnitude < 0.000001) tangent = Vector3.Cross(hitInfo.normal, Vector3.back);
                            tangent = tangent.normalized;
                            _mirroredTransform.rotation = Quaternion.LookRotation(tangent, hitInfo.normal);
                        }
                    }
                }
                var matrix = Matrix4x4.TRS(_mirroredTransform.position, _mirroredTransform.rotation, previewScale)
                    * Matrix4x4.Rotate(Quaternion.Inverse(obj.transform.rotation))
                    * Matrix4x4.Translate(-obj.transform.position);
                var layer = settings.overwritePrefabLayer ? settings.layer : obj.layer;
                var reverseTriagles = scale.x * scale.y * scale.z < 0;
                PreviewBrushItem(obj, matrix, layer, _sceneViewCamera, false, reverseTriagles);
                var parent = settings.sameParentAsSource
                    ? obj.transform.parent : GetParent(settings, obj.name, false);

                _paintStroke.Add(new PaintStrokeItem(obj, _mirroredTransform.position,
                    _mirroredTransform.rotation, scale, layer, parent));
            }
        }
    }
    #endregion
}

