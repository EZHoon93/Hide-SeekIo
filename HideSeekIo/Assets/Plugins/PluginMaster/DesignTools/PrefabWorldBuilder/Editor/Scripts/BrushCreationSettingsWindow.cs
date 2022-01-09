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

using UnityEditor;
using UnityEngine;

namespace PluginMaster
{
    public class BrushCreationSettingsWindow : EditorWindow
    {
        [SerializeField] PWBData _data = null;

        private bool _defaultBrushSettingsGroupOpen = false;
        private bool _defaultThumbnailSettingsGroupOpen = false;
        private bool _brushPosGroupOpen = false;
        private bool _brushRotGroupOpen = false;
        private bool _brushScaleGroupOpen = false;

        private Vector2 _mainScrollPosition = Vector2.zero;
        [MenuItem("Tools/Plugin Master/Prefab World Builder/Brush Creation Settings", false, 1140)]
        public static void ShowWindow() => GetWindow<BrushCreationSettingsWindow>();

        private static string UNDO_MSG = "Brush Creation Settings";

        private void OnEnable()
        {
            _data = PWBCore.staticData;
            Undo.undoRedoPerformed += Repaint;
            titleContent = new GUIContent(PaletteManager.selectedPalette.name + " - Brush Creation Settings");
            
        }

        private void OnDisable() => Undo.undoRedoPerformed -= Repaint;

        private void OnGUI()
        {
            if (PaletteManager.selectedPalette == null) return;
            EditorGUIUtility.labelWidth = 60;
            var settings = PaletteManager.selectedPalette.brushCreationSettings.Clone();
            using (var scrollView = new EditorGUILayout.ScrollViewScope(_mainScrollPosition,
                false, false, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUIStyle.none))
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    _mainScrollPosition = scrollView.scrollPosition;
                    settings.includeSubfolders = EditorGUILayout.ToggleLeft("Include subfolders",
                        settings.includeSubfolders);
                    settings.createLablesForEachDroppedFolder = EditorGUILayout.ToggleLeft("Add labels for each folder",
                        settings.createLablesForEachDroppedFolder);
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        settings.addLabelsToDroppedPrefabs = EditorGUILayout.ToggleLeft("Add labels to prefabs",
                            settings.addLabelsToDroppedPrefabs);
                        using (new EditorGUI.DisabledGroupScope(!settings.addLabelsToDroppedPrefabs))
                        {
                            EditorGUIUtility.labelWidth = 40;
                            settings.labelsCSV = EditorGUILayout.TextField("Labels:", settings.labelsCSV);
                        }
                    }

#if UNITY_2019_1_OR_NEWER
                    _defaultBrushSettingsGroupOpen = EditorGUILayout.BeginFoldoutHeaderGroup(_defaultBrushSettingsGroupOpen,
                        "Default Brush Settings");
#else
                    _defaultBrushSettingsGroupOpen = EditorGUILayout.Foldout(_defaultBrushSettingsGroupOpen,
                    "Default Brush Settings");
#endif
                    if (_defaultBrushSettingsGroupOpen)
                    {
                        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            BrushProperties.BrushFields(settings.defaultBrushSettings, ref _brushPosGroupOpen,
                                ref _brushRotGroupOpen, ref _brushScaleGroupOpen, this, UNDO_MSG);
                            GUILayout.Space(10);
                            if (GUILayout.Button("Reset to factory settings"))
                            {
                                settings.FactoryResetDefaultBrushSettings();
                                GUI.FocusControl(null);
                                Repaint();
                            }
                        }
                    }
#if UNITY_2019_1_OR_NEWER
                    EditorGUILayout.EndFoldoutHeaderGroup();
#endif
                    _defaultThumbnailSettingsGroupOpen
                        = EditorGUILayout.BeginFoldoutHeaderGroup(_defaultThumbnailSettingsGroupOpen,
                        "Default Thumbnail Settings");
                    if(_defaultThumbnailSettingsGroupOpen)
                    {
                        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            ThumbnailEditorWindow.ThumbnailSettingsGUI(settings.defaultThumbnailSettings);
                            GUILayout.Space(10);
                            if (GUILayout.Button("Reset to factory settings"))
                            {
                                settings.FactoryResetDefaultThumbnailSettings();
                                GUI.FocusControl(null);
                                Repaint();
                            }
                        }
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();

                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        GUI.FocusControl(null);
                        Repaint();
                    }
                    if(check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        PaletteManager.selectedPalette.brushCreationSettings.Copy(settings);
                        PWBCore.SetSavePending();
                    }
                }
            }
        }

        [MenuItem("Assets/Clear Labels", false, 2000)]
        private static void ClearLabels()
        {
            var selection = Selection.GetFiltered<Object>(SelectionMode.Assets);
            foreach (var asset in selection) AssetDatabase.ClearLabels(asset);
        }
    }
}
