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
using UnityEditor;
using UnityEngine;

namespace PluginMaster
{
    public class SnapSettingsWindow : EditorWindow
    {
        private static readonly string[] _gridTypeOptions = { "Rectangular", "Radial" };
        private static SnapSettingsWindow _instance = null;
        [MenuItem("Tools/Plugin Master/Prefab World Builder/Grid and Snapping Settings", false, 1150)]
        public static void ShowWindow() => _instance = GetWindow<SnapSettingsWindow>("Grid and Snapping Settings");

        private GameObject _activeGameObject = null;

        public static void RepaintWindow()
        {
            if (_instance != null) _instance.Repaint();
        }

        private void OnEnable()
        {
            _activeGameObject = Selection.activeGameObject;
        }

        private void OnGUI()
        {
            minSize = new Vector2(350, SnapManager.settings.radialGridEnabled ? 290 : 310);
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    if (SnapManager.settings.radialGridEnabled)
                    {
                        SnapManager.settings.radialStep = EditorGUILayout.FloatField("Radial Snap Value:",
                            SnapManager.settings.radialStep);
                    }
                    else
                    {
                        SnapManager.settings.step = EditorGUILayout.Vector3Field("Snap Value:",
                            SnapManager.settings.step);
                    }
                    if (check.changed) SceneView.RepaintAll();
                }
                if (!SnapManager.settings.radialGridEnabled)
                {
                    using (new EditorGUI.DisabledGroupScope(_activeGameObject == null))
                    {
                        if (GUILayout.Button("Set the snap value to the size of the active gameobject"))
                        {
                            var bounds = BoundsUtils.GetBounds(_activeGameObject.transform);
                            SnapManager.settings.step = bounds.size;
                            SceneView.RepaintAll();
                        }
                    }
                }
            }
            if (SnapManager.settings.radialGridEnabled)
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        SnapManager.settings.radialSectors = EditorGUILayout.IntField("Radial Sectors:",
                            SnapManager.settings.radialSectors);
                        if (check.changed) SceneView.RepaintAll();
                    }
                }
            }
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    SnapManager.settings.origin = EditorGUILayout.Vector3Field("Grid Origin",
                        SnapManager.settings.origin);
                    if (check.changed) SceneView.RepaintAll();
                }
                using (new EditorGUI.DisabledGroupScope(_activeGameObject == null))
                {
                    if (GUILayout.Button("Set the origin to the active gameobject position"))
                    {
                        SnapManager.settings.origin = _activeGameObject.transform.position;
                        SceneView.RepaintAll();
                    }
                }
            }
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var euler = SnapManager.settings.rotation.eulerAngles;
                    euler = new Vector3(Mathf.RoundToInt(euler.x * 100000) / 100000f,
                        Mathf.RoundToInt(euler.y * 100000) / 100000f,
                        Mathf.RoundToInt(euler.z * 100000) / 100000f);
                    SnapManager.settings.rotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", euler));
                    if (check.changed) SceneView.RepaintAll();
                }
                using (new EditorGUI.DisabledGroupScope(_activeGameObject == null))
                {
                    if (GUILayout.Button("Set the rotation to the active gameobject rotation"))
                    {
                        SnapManager.settings.rotation = _activeGameObject.transform.rotation;
                        SceneView.RepaintAll();
                    }
                }
            }
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var idx = SnapManager.settings.radialGridEnabled ? 1 : 0;
                    idx = EditorGUILayout.Popup("Grid type:", idx, _gridTypeOptions);
                    if (check.changed)
                    {
                        SnapManager.settings.radialGridEnabled = idx == 0 ? false : true;
                        PWBToolbar.RepaintWindow();
                    }
                }
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    SnapManager.settings.lockedGrid = EditorGUILayout.ToggleLeft("Lock the grid origin in place",
                        SnapManager.settings.lockedGrid);
                    if (check.changed) PWBToolbar.RepaintWindow();
                }
                using (new EditorGUI.DisabledGroupScope(!SnapManager.settings.lockedGrid))
                {
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var showPositionHandle = EditorGUILayout.ToggleLeft("Show position handle",
                            SnapManager.settings.showPositionHandle);
                        if (check.changed) SnapManager.settings.showPositionHandle = showPositionHandle;
                    }
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var showRotationHandle = EditorGUILayout.ToggleLeft("Show rotation handle",
                            SnapManager.settings.showRotationHandle);
                        if (check.changed) SnapManager.settings.showRotationHandle = showRotationHandle;
                    }
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var showScaleHandle = EditorGUILayout.ToggleLeft("Show spacing handle",
                            SnapManager.settings.showScaleHandle);
                        if (check.changed) SnapManager.settings.showScaleHandle = showScaleHandle;
                    }
                }
            }
        }

        private void OnSelectionChange() => _activeGameObject = Selection.activeGameObject;
    }
}