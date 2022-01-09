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
using UnityEditor;
using UnityEngine;

namespace PluginMaster
{
    #region DATA & SETTINGS
    [Serializable]
    public class GravityToolSettings : BrushToolBase
    {
        [SerializeField] private SimulateGravityData _simData = new SimulateGravityData();
        [SerializeField] private float _height = 10f;

        public SimulateGravityData simData => _simData;
        public float height
        {
            get => _height;
            set
            {
                value = Mathf.Max(value, 0f);
                if (_height == value) return;
                _height = value;
            }
        }

        public override void Copy(IToolSettings other)
        {
            var otherGravityToolSettings = other as GravityToolSettings;
            if (otherGravityToolSettings == null) return;
            base.Copy(other);
            _simData.Copy(otherGravityToolSettings._simData);
            _height = otherGravityToolSettings.height;
        }

        public GravityToolSettings Clone()
        {
            var clone = new GravityToolSettings();
            clone.Copy(this);
            return clone;
        }

        public GravityToolSettings() : base() => _brushShape = BrushShape.POINT;
    }

    [Serializable]
    public class GravityToolManager : ToolManagerBase<GravityToolSettings> { }
    #endregion

    #region PWBIO
    public static partial class PWBIO
    {
        private static Mesh _gravityLinesMesh = null;
        private static Material _gravityLinesMaterial = null;
        private static readonly int OPACITY_PROP_ID = Shader.PropertyToID("_opacity");

        private static void GravityToolDuringSceneGUI(SceneView sceneView)
        {
            BrushstrokeMouseEvents(GravityToolManager.settings);
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
                if (_octree == null) UpdateOctree();
                if (MouseRaycast(mouseRay, out RaycastHit hit, out GameObject collider,
                    float.MaxValue, -1, true, true))
                    DrawGravityBrush(hit, sceneView.camera);
            }
            else if (Event.current.button == 0 && !Event.current.alt
                && (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag))
            {
                var paintedObjects = Paint(GravityToolManager.settings);
                GravityUtils.SimulateGravity(paintedObjects, GravityToolManager.settings.simData, false);
                Event.current.Use();
            }
            else if (Event.current.type == EventType.ScrollWheel && Event.current.control
                && PaletteManager.selectedBrush != null)
            {
                GravityToolManager.settings.radius = UpdateRadius(GravityToolManager.settings.radius);
                Event.current.Use();
            }
            else if (Event.current.type == EventType.KeyDown && Event.current.control)
            {
                if (Event.current.keyCode == KeyCode.UpArrow)
                {
                    GravityToolManager.settings.height += Event.current.shift ? 0.1f : 1f;
                    ToolProperties.RepainWindow();
                }
                else if (Event.current.keyCode == KeyCode.DownArrow)
                {
                    GravityToolManager.settings.height -= Event.current.shift ? 0.1f : 1f;
                    ToolProperties.RepainWindow();
                }
            }
            if (Event.current.button == 1)
            {
                if (Event.current.type == EventType.MouseDown && Event.current.control)
                {
                    _pinned = true;
                    _pinMouse = Event.current.mousePosition;
                    Event.current.Use();
                }
                if (Event.current.type == EventType.MouseDrag && Event.current.shift && Event.current.control)
                {
                    var delta = - Mathf.Sign(Event.current.delta.y);
                    GravityToolManager.settings.height = Mathf.Max((GravityToolManager.settings.height + delta * 0.5f)
                        * (1f + delta * 0.02f), 0.05f);
                    ToolProperties.RepainWindow();
                    Event.current.Use();
                }
            }
        }

        private static void DrawGravityBrush(RaycastHit hit, Camera camera)
        {
            var settings = GravityToolManager.settings;

            hit.point = SnapAndUpdateGridOrigin(hit.point, SnapManager.settings.snappingEnabled, true, true, false);
            var tangent = GetTangent(Vector3.up);
            var bitangent = Vector3.Cross(hit.normal, tangent);

            if (settings.brushShape == BrushToolSettings.BrushShape.SQUARE)
            {
                DrawSquareIndicator(hit.point, hit.normal, settings.radius,
                    settings.height, tangent, bitangent, Vector3.up, true, true);
            }
            else
            {
                DrawCricleIndicator(hit.point, hit.normal, 
                    settings.brushShape == BrushToolBase.BrushShape.POINT ? 0.1f : settings.radius,
                    settings.height, tangent, bitangent, Vector3.up, true, true);
            }

            if (_gravityLinesMesh == null)
            {
                _gravityLinesMesh = new Mesh();
                _gravityLinesMesh.vertices = new Vector3[] { new Vector3(-1, -1, 0), new Vector3(1, -1, 0),
                    new Vector3(1, 1, 0), new Vector3(-1, 1, 0) };
                _gravityLinesMesh.uv = new Vector2[] { new Vector2(1, 0), new Vector2(0, 0),
                    new Vector2(0, 1), new Vector2(1, 1) };
                _gravityLinesMesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
                _gravityLinesMesh.RecalculateNormals();
            }
            if (_gravityLinesMaterial == null)
                _gravityLinesMaterial = Resources.Load<Material>("Materials/GravityLines");
            var camEulerY = Mathf.Abs(Vector3.SignedAngle(Vector3.up, camera.transform.up, camera.transform.forward));
            var gravityLinesOpacity = 1f - Mathf.Min((camEulerY > 90f ? 180f - camEulerY : camEulerY) / 60f, 1f);
            _gravityLinesMaterial.SetFloat(OPACITY_PROP_ID, gravityLinesOpacity);

            var hitToCamXZ = camera.transform.position - hit.point;
            hitToCamXZ.y = 0f;
            var gravityLinesRotation = Quaternion.AngleAxis(camera.transform.eulerAngles.y, Vector3.up);
            var radius = settings.brushShape == BrushToolBase.BrushShape.POINT
                ? 0.5F : settings.radius;
            var gravityLinesMatrix = Matrix4x4.TRS(hit.point + new Vector3(0f, 3f * radius, 0f),
                gravityLinesRotation, new Vector3(0.5f, 2f, 1f) * radius);
            Graphics.DrawMesh(_gravityLinesMesh, gravityLinesMatrix, _gravityLinesMaterial, 0, camera);
           
            GravityStrokePreview(hit.point + new Vector3(0f, settings.height, 0f), tangent, bitangent, camera);
        }

        private static void GravityStrokePreview(Vector3 center, Vector3 tangent, Vector3 bitangent, Camera camera)
        {
            _paintStroke.Clear();
            
            foreach (var strokeItem in BrushstrokeManager.brushstroke)
            {
                var prefab = strokeItem.settings.prefab;
                if (prefab == null) continue;
                var h = strokeItem.settings.bottomMagnitude;
                BrushSettings brushSettings = strokeItem.settings;
                if (GravityToolManager.settings.overwriteBrushProperties)
                    brushSettings = GravityToolManager.settings.brushSettings;
                var strokePosition = TangentSpaceToWorld(tangent, bitangent,
                   new Vector2(strokeItem.tangentPosition.x, strokeItem.tangentPosition.y));
                var itemPosition = strokePosition + center + new Vector3(0f, h * strokeItem.scaleMultiplier.y, 0f);
                    
                var itemRotation = Quaternion.AngleAxis(_brushAngle, Vector3.up)
                    * Quaternion.Euler(strokeItem.additionalAngle);
                if (GravityToolManager.settings.orientAlongBrushstroke)
                {
                    itemRotation = Quaternion.LookRotation(_strokeDirection, Vector3.up);
                    itemPosition = center + itemRotation * strokePosition;
                }
                itemPosition += itemRotation * brushSettings.localPositionOffset;
                

                var rootToWorld = Matrix4x4.TRS(itemPosition, itemRotation, strokeItem.scaleMultiplier)
                    * Matrix4x4.Translate(-prefab.transform.position);
                var itemScale = Vector3.Scale(prefab.transform.localScale, strokeItem.scaleMultiplier);
                var layer = GravityToolManager.settings.overwritePrefabLayer
                    ? GravityToolManager.settings.layer : prefab.layer;
                Transform parentTransform = GetParent(GravityToolManager.settings, prefab.name, false);
                _paintStroke.Add(new PaintStrokeItem(prefab, itemPosition,
                    itemRotation * Quaternion.Euler(prefab.transform.eulerAngles),
                    itemScale, layer, parentTransform));
                PreviewBrushItem(prefab, rootToWorld, layer, camera);
            }
        }

        private static Vector3 GetObjectHalfSize(Transform transform)
        {
            var size = new Vector3(0.1f, 0.1f, 0.1f);
            var childrenRenderer = transform.GetComponentsInChildren<Renderer>();
            foreach (var child in childrenRenderer) size = Vector3.Max(size, child.bounds.size);
            return size / 2f;
        }
    }
    #endregion
}
