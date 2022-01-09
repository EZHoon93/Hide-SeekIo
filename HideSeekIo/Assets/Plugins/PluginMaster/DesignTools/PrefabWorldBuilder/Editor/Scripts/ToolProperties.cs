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
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


namespace PluginMaster
{
    public class ToolProperties : EditorWindow
    {
        #region COMMON
        private const string UNDO_MSG = "Tool properties";
        private Vector2 _mainScrollPosition = Vector2.zero;
        private GUIContent _updateButtonContent = null;
        private static ToolProperties _instance = null;

        [MenuItem("Tools/Plugin Master/Prefab World Builder/Tool Properties", false, 1130)]
        public static void ShowWindow() => _instance = GetWindow<ToolProperties>("Tool Properties");

        public static void RepainWindow()
        {
            if (_instance != null) _instance.Repaint();
        }

        private void OnEnable()
        {
            _data = PWBCore.staticData;
            if (BrushManager.settings.paintOnMeshesWithoutCollider) PWBCore.UpdateTempColliders();
            _updateButtonContent
                = new GUIContent(Resources.Load<Texture2D>("Sprites/Update"), "Update Temp Colliders");
            Undo.undoRedoPerformed += OnUndoPerformed;
        }

        private void OnDisable()
        {
            PWBCore.DestroyTempColliders();
            Undo.undoRedoPerformed -= OnUndoPerformed;
        }

        private void OnGUI()
        {
            if (_instance == null) _instance = this;
            using (var scrollView = new EditorGUILayout.ScrollViewScope(_mainScrollPosition,
                false, false, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUIStyle.none))
            {
                _mainScrollPosition = scrollView.scrollPosition;
#if UNITY_2021_2_OR_NEWER
#else
                if (PWBToolbar.instance == null) PWBToolbar.ShowWindow();
#endif
                if (ToolManager.tool == ToolManager.PaintTool.PIN) PinGroup();
                else if (ToolManager.tool == ToolManager.PaintTool.BRUSH) BrushGroup();
                else if (ToolManager.tool == ToolManager.PaintTool.ERASER) EraserGroup();
                else if (ToolManager.tool == ToolManager.PaintTool.GRAVITY) GravityGroup();
                else if (ToolManager.tool == ToolManager.PaintTool.EXTRUDE) ExtrudeGroup();
                else if (ToolManager.tool == ToolManager.PaintTool.LINE) LineGroup();
                else if (ToolManager.tool == ToolManager.PaintTool.SHAPE) ShapeGroup();
                else if (ToolManager.tool == ToolManager.PaintTool.TILING) TilingGroup();
                else if (ToolManager.tool == ToolManager.PaintTool.SELECTION) SelectionGroup();
                else if (ToolManager.tool == ToolManager.PaintTool.MIRROR) MirrorGroup();
                else if (ToolManager.tool == ToolManager.PaintTool.REPLACER) ReplacerGroup();
            }
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                GUI.FocusControl(null);
                Repaint();
            }
        }

        private void OnUndoPerformed()
        {
            BrushstrokeManager.UpdateBrushstroke();
            Repaint();
            SceneView.RepaintAll();
        }
        #endregion

        #region UNDO
        [SerializeField] PWBData _data = null;
        [SerializeField] LineData _lineData = LineData.instance;
        [SerializeField] TilingData _tilingData = TilingData.instance;
        [SerializeField] MirrorSettings _mirrorSettings = MirrorManager.settings;
        [SerializeField] ShapeData shapeData = ShapeData.instance;
        public static void RegisterUndo(string commandName)
        {
            if (_instance == null) return;
            Undo.RegisterCompleteObjectUndo(_instance, commandName);
        }
        #endregion

        #region TOOL PROFILE
        private class ProfileData
        {
            public readonly IToolManager toolManager = null;
            public readonly string profileName = string.Empty;
            public ProfileData(IToolManager toolManager, string profileName)
                => (this.toolManager, this.profileName) = (toolManager, profileName);
        }
        private void ToolProfileGUI(IToolManager toolManager)
        {
            using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("Tool Profile:");
                if (GUILayout.Button(toolManager.selectedProfileName, EditorStyles.popup, GUILayout.MinWidth(100)))
                {
                    GUI.FocusControl(null);
                    var menu = new GenericMenu();
                    foreach (var profileName in toolManager.profileNames)
                        menu.AddItem(new GUIContent(profileName), profileName == toolManager.selectedProfileName,
                            SelectProfile, new ProfileData(toolManager, profileName));
                    menu.AddSeparator(string.Empty);
                    if (toolManager.selectedProfileName != ToolProfile.DEFAULT) menu.AddItem(new GUIContent("Save"),
                        false, SaveProfile, toolManager);
                    menu.AddItem(new GUIContent("Save As..."), false, SaveProfileAs,
                        new ProfileData(toolManager, toolManager.selectedProfileName));
                    if (toolManager.selectedProfileName != ToolProfile.DEFAULT)
                        menu.AddItem(new GUIContent("Delete Selected Profile"), false, DeleteProfile,
                            new ProfileData(toolManager, toolManager.selectedProfileName));
                    menu.AddItem(new GUIContent("Revert Selected Profile"), false, RevertProfile, toolManager);
                    menu.AddItem(new GUIContent("Factory Reset Selected Profile"), false,
                        FactoryResetProfile, toolManager);
                    menu.ShowAsContext();
                }
            }
        }

        private void SelectProfile(object value)
        {
            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
            GUI.FocusControl(null);
            var profiledata = value as ProfileData;
            profiledata.toolManager.selectedProfileName = profiledata.profileName;
            Repaint();
            if (ToolManager.tool == ToolManager.PaintTool.MIRROR)
                SceneView.lastActiveSceneView.LookAt(MirrorManager.settings.mirrorPosition);
            SceneView.RepaintAll();
        }

        private void SaveProfile(object value)
        {
            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
            var manager = value as IToolManager;
            manager.SaveProfile();
        }

        private void SaveProfileAs(object value)
        {
            var profiledata = value as ProfileData;
            SaveProfileWindow.ShowWindow(profiledata, OnSaveProfileDone);
        }

        private void OnSaveProfileDone(IToolManager toolManager, string profileName)
        {
            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
            toolManager.SaveProfileAs(profileName);
            Repaint();
        }
        private class SaveProfileWindow : EditorWindow
        {
            private IToolManager _toolManager = null;
            private string _profileName = string.Empty;
            private Action<IToolManager, string> OnDone;

            public static void ShowWindow(ProfileData data, Action<IToolManager, string> OnDone)
            {
                var window = GetWindow<SaveProfileWindow>(true, "Save Profile");
                window._toolManager = data.toolManager;
                window._profileName = data.profileName;
                window.OnDone = OnDone;
                window.minSize = window.maxSize = new Vector2(160, 50);
                EditorGUIUtility.labelWidth = 70;
                EditorGUIUtility.fieldWidth = 70;
            }

            private void OnGUI()
            {
                const string textFieldName = "NewProfileName";
                GUI.SetNextControlName(textFieldName);
                _profileName = EditorGUILayout.TextField(_profileName).Trim();
                GUI.FocusControl(textFieldName);
                using (new EditorGUI.DisabledGroupScope(_profileName == String.Empty))
                {
                    if (GUILayout.Button("Save"))
                    {
                        OnDone(_toolManager, _profileName);
                        Close();
                    }
                }
            }
        }

        private void DeleteProfile(object value)
        {
            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
            var profiledata = value as ProfileData;
            profiledata.toolManager.DeleteProfile();
            if (ToolManager.tool == ToolManager.PaintTool.MIRROR)
                SceneView.lastActiveSceneView.LookAt(MirrorManager.settings.mirrorPosition);
        }
        private void RevertProfile(object value)
        {
            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
            var manager = value as IToolManager;
            manager.Revert();
            if (ToolManager.tool == ToolManager.PaintTool.MIRROR)
                SceneView.lastActiveSceneView.LookAt(MirrorManager.settings.mirrorPosition);
        }
        private void FactoryResetProfile(object value)
        {
            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
            var manager = value as IToolManager;
            manager.FactoryReset();
            if (ToolManager.tool == ToolManager.PaintTool.MIRROR)
                SceneView.lastActiveSceneView.LookAt(MirrorManager.settings.mirrorPosition);
        }
        #endregion

        #region COMMON PAINT SETTINGS
        private static float _maxRadius = 50f;
        private static Vector3[] _dir =
        {
            Vector3.right, Vector3.left,
            Vector3.up, Vector3.down,
            Vector3.forward, Vector3.back
        };
        private static string[] _dirNames = new string[] { "+X", "-X", "+Y", "-Y", "+Z", "-Z" };

        private static readonly string[] _brushShapeOptions = { "Point", "Circle", "Square" };
        private static readonly string[] _spacingOptions = { "Auto", "Custom" };
        private void PaintSettingsGUI(IPaintOnSurfaceToolSettings paintOnSurfaceSettings,
            IPaintToolSettings paintSettings)
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                void UpdateTempColliders()
                {
                    if (paintOnSurfaceSettings.paintOnMeshesWithoutCollider) PWBCore.UpdateTempColliders();
                    else PWBCore.DestroyTempColliders();
                }

                using (new GUILayout.HorizontalScope())
                {
                    EditorGUIUtility.labelWidth = 150;
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var paintOnMeshesWithoutCollider = EditorGUILayout.ToggleLeft("Paint on meshes without collider",
                            paintOnSurfaceSettings.paintOnMeshesWithoutCollider);
                        if (check.changed)
                        {
                            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                            paintOnSurfaceSettings.paintOnMeshesWithoutCollider = paintOnMeshesWithoutCollider;
                            UpdateTempColliders();
                            SceneView.RepaintAll();
                        }
                    }
                    using (new EditorGUI.DisabledGroupScope(!paintOnSurfaceSettings.paintOnMeshesWithoutCollider))
                        if (GUILayout.Button(_updateButtonContent, GUILayout.Width(21), GUILayout.Height(21)))
                            PWBCore.UpdateTempColliders();
                }
                EditorGUIUtility.labelWidth = 110;
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var paintOnPalettePrefabs = EditorGUILayout.ToggleLeft("Paint on palette prefabs",
                        paintOnSurfaceSettings.paintOnPalettePrefabs);
                    var paintOnSelectedOnly = EditorGUILayout.ToggleLeft("Paint on selected only",
                        paintOnSurfaceSettings.paintOnSelectedOnly);
                    if (check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        paintOnSurfaceSettings.paintOnPalettePrefabs = paintOnPalettePrefabs;
                        paintOnSurfaceSettings.paintOnSelectedOnly = paintOnSelectedOnly;
                        UpdateTempColliders();
                        SceneView.RepaintAll();
                    }
                }
            }
            PaintToolSettingsGUI(paintSettings);
        }

        private void PaintToolSettingsGUI(IPaintToolSettings paintSettings)
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                paintSettings.autoCreateParent = EditorGUILayout.ToggleLeft("Create parent", paintSettings.autoCreateParent);
                if (paintSettings.autoCreateParent)
                {
                    paintSettings.createSubparent = EditorGUILayout.ToggleLeft("Create sub-parents per prefab",
                        paintSettings.createSubparent);
                }
                else
                {
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var parent = (Transform)EditorGUILayout.ObjectField("Parent Transform:",
                            paintSettings.parent, typeof(Transform), true);
                        if (check.changed)
                        {
                            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                            paintSettings.parent = parent;
                            SceneView.RepaintAll();
                        }
                    }
                }
            }
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var overwritePrefabLayer = EditorGUILayout.ToggleLeft("Overwrite prefab layer",
                        paintSettings.overwritePrefabLayer);
                    int layer = paintSettings.layer;
                    if (paintSettings.overwritePrefabLayer) layer = EditorGUILayout.LayerField("Layer:",
                        paintSettings.layer);
                    if (check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        paintSettings.overwritePrefabLayer = overwritePrefabLayer;
                        paintSettings.layer = layer;
                        SceneView.RepaintAll();
                    }
                }
            }
        }

        private void RadiusSlider(CircleToolBase settings)
        {
            using (new GUILayout.HorizontalScope())
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    if (settings.radius > _maxRadius)
                        _maxRadius = Mathf.Max(Mathf.Floor(settings.radius / 10) * 20f, 10f);
                    EditorGUIUtility.labelWidth = 60;
                    var radius = EditorGUILayout.Slider("Radius:", settings.radius, 0.05f, _maxRadius);
                    if (check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        settings.radius = radius;
                        SceneView.RepaintAll();
                    }
                }
                if (GUILayout.Button("|>", GUILayout.Width(20))) _maxRadius *= 2f;
                if (GUILayout.Button("|<", GUILayout.Width(20)))
                    _maxRadius = Mathf.Min(Mathf.Floor(settings.radius / 10f) * 10f + 10f, _maxRadius);
            }
        }
        private void BrushToolBaseSettingsGUI(BrushToolBase settings)
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUIUtility.labelWidth = 60;
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var brushShape = (BrushToolSettings.BrushShape)EditorGUILayout.Popup("Shape:",
                        (int)settings.brushShape, _brushShapeOptions);
                    if (check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        settings.brushShape = brushShape;
                        SceneView.RepaintAll();
                    }
                }
                if (settings.brushShape != BrushToolBase.BrushShape.POINT)
                {
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var randomize = EditorGUILayout.ToggleLeft("Randomize positions", settings.randomizePositions);
                        if (check.changed)
                        {
                            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                            settings.randomizePositions = randomize;
                            SceneView.RepaintAll();
                        }
                    }
                    RadiusSlider(settings);
                }
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var density = EditorGUILayout.IntSlider("Density:", settings.density, 0, 100);
                    if (check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        settings.density = density;
                        SceneView.RepaintAll();
                    }
                }
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUIUtility.labelWidth = 90;
                        var spacingType = (BrushToolBase.SpacingType)EditorGUILayout.Popup("Min Spacing:",
                            (int)settings.spacingType, _spacingOptions);
                        var spacing = settings.minSpacing;
                        using (new EditorGUI.DisabledGroupScope(spacingType != BrushToolBase.SpacingType.CUSTOM))
                        {
                            spacing = EditorGUILayout.FloatField("Value:", settings.minSpacing);
                        }
                        if (check.changed)
                        {
                            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                            settings.spacingType = spacingType;
                            settings.minSpacing = spacing;
                            SceneView.RepaintAll();
                        }
                    }
                }
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var orientAlongBrushstroke = EditorGUILayout.ToggleLeft("Orient Along the Brushstroke",
                            settings.orientAlongBrushstroke);
                        var additionalAngle = settings.additionalOrientationAngle;
                        if (orientAlongBrushstroke)
                            additionalAngle = EditorGUILayout.Vector3Field("Additonal angle:", additionalAngle);
                        if (check.changed)
                        {
                            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                            settings.orientAlongBrushstroke = orientAlongBrushstroke;
                            settings.additionalOrientationAngle = additionalAngle;
                            SceneView.RepaintAll();
                        }
                    }
                }
            }
        }

        private void EmbedInSurfaceSettingsGUI(SelectionToolBaseBasic settings)
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUIUtility.labelWidth = 60;
                    var embedInSurface = EditorGUILayout.ToggleLeft("Embed On the Surface",
                        settings.embedInSurface);
                    if (check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        settings.embedInSurface = embedInSurface;
                        if (embedInSurface && settings is SelectionToolSettings) PWBIO.EmbedSelectionInSurface();
                        SceneView.RepaintAll();
                    }
                }
                if (settings.embedInSurface)
                {
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var embedAtPivotHeight = EditorGUILayout.ToggleLeft("Embed At Pivot Height",
                            settings.embedAtPivotHeight);
                        if (check.changed)
                        {
                            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                            settings.embedAtPivotHeight = embedAtPivotHeight;
                            if (settings.embedInSurface && settings is SelectionToolSettings) PWBIO.EmbedSelectionInSurface();
                            SceneView.RepaintAll();
                        }
                    }
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        EditorGUIUtility.labelWidth = 110;
                        var surfaceDistance = EditorGUILayout.FloatField("Surface Distance:",
                            settings.surfaceDistance);
                        if (check.changed)
                        {
                            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                            settings.surfaceDistance = surfaceDistance;
                            if (settings is SelectionToolSettings) PWBIO.EmbedSelectionInSurface();
                            SceneView.RepaintAll();
                        }
                    }
                    if (settings is SelectionToolBase)
                    {
                        using (var check = new EditorGUI.ChangeCheckScope())
                        {
                            var selectionSettings = settings as SelectionToolBase;
                            var rotateToTheSurface = EditorGUILayout.ToggleLeft("Rotate To the Surface",
                                selectionSettings.rotateToTheSurface);
                            if (check.changed)
                            {
                                Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                                selectionSettings.rotateToTheSurface = rotateToTheSurface;
                                if (settings.embedInSurface && settings is SelectionToolSettings)
                                    PWBIO.EmbedSelectionInSurface();
                                SceneView.RepaintAll();
                            }
                        }

                    }
                }
            }
        }

        private struct BrushPropertiesGroupState
        {
            public bool brushPosGroupOpen;
            public bool brushRotGroupOpen;
            public bool brushScaleGroupOpen;
        }

        private void OverwriteBrushPropertiesGUI(IPaintToolSettings settings,
            ref BrushPropertiesGroupState state)
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var overwriteBrushProperties = EditorGUILayout.ToggleLeft("Overwrite Brush Properties",
                        settings.overwriteBrushProperties);
                    if (check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        settings.overwriteBrushProperties = overwriteBrushProperties;
                        SceneView.RepaintAll();
                    }
                }
                if (settings.overwriteBrushProperties) BrushProperties.BrushFields(settings.brushSettings,
                    ref state.brushPosGroupOpen, ref state.brushRotGroupOpen, ref state.brushScaleGroupOpen, this, UNDO_MSG);
            }
        }


        #endregion

        #region MODIFIER SETTINGS
        private static readonly string[] _modifierCommandOptions = { "All", "Palette Prefabs", "Brush Prefabs" };
        private void ModifierGroup(IModifierTool settings)
        {
            var actionLabel = settings is EraserSettings ? "Erase" : "Replace";
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUIUtility.labelWidth = 60;
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var command = (ModifierToolSettings.Command)EditorGUILayout.Popup(actionLabel + ":",
                        (int)settings.command, _modifierCommandOptions);
                    if (check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        settings.command = command;
                        PWBIO.UpdateOctree();
                    }
                }
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var allButSelected = EditorGUILayout.ToggleLeft(actionLabel + " all but selected",
                        settings.modifyAllButSelected);
                    if (check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        settings.modifyAllButSelected = allButSelected;
                        PWBIO.UpdateOctree();
                    }
                }
            }
        }
        #endregion

        #region PIN
        private static readonly string[] _pinModeNames = { "Auto", "Paint on surface", "Paint on grid" };
        private static BrushPropertiesGroupState _pinOverwriteGroupState;
        private void PinGroup()
        {
            ToolProfileGUI(PinManager.instance);
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var mode = (PinSettings.PaintMode)EditorGUILayout.Popup("Paint mode:",
                        (int)PinManager.settings.mode, _pinModeNames);
                    if (check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        PinManager.settings.mode = mode;
                        SceneView.RepaintAll();
                    }
                }
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var repeat = EditorGUILayout.ToggleLeft("Repeat multi-brush item", PinManager.settings.repeat);
                    if (check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        PinManager.settings.repeat = repeat;
                        SceneView.RepaintAll();
                    }
                }
            }
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUIUtility.labelWidth = 60;
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var flattenTerrain = EditorGUILayout.ToggleLeft("Flatten the terrain", PinManager.settings.flattenTerrain);
                    if(check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        PinManager.settings.flattenTerrain = flattenTerrain;
                    }
                }
                using (new EditorGUI.DisabledGroupScope(!PinManager.settings.flattenTerrain))
                {
                    var flatteningSettings = PinManager.settings.flatteningSettings;
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var hardness = EditorGUILayout.Slider("Hardness:", flatteningSettings.hardness, 0, 1);
                        if(check.changed)
                        {
                            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                            flatteningSettings.hardness = hardness;
                        }
                    }
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var padding = EditorGUILayout.FloatField("Padding:", flatteningSettings.padding);
                        if (check.changed)
                        {
                            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                            flatteningSettings.padding = padding;
                        }
                    }
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var clearTrees = EditorGUILayout.ToggleLeft("Clear trees", flatteningSettings.clearTrees);
                        if(check.changed)
                        {
                            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                            flatteningSettings.clearTrees = clearTrees;
                        }
                    }
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var clearDetails = EditorGUILayout.ToggleLeft("Clear details", flatteningSettings.clearDetails);
                        if (check.changed)
                        {
                            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                            flatteningSettings.clearDetails = clearDetails;
                        }
                    }
                }
            }
            PaintSettingsGUI(PinManager.settings, PinManager.settings);
            OverwriteBrushPropertiesGUI(PinManager.settings, ref _pinOverwriteGroupState);
        }
        #endregion

        #region BRUSH
        private static readonly string[] _heightTypeNames = { "Custom", "Radius" };
        private static readonly string[] _avoidOverlappingTypeNames = { "Disabled", "With Palette Prefabs",
            "With Brush Prefabs", "With Same Prefabs" };
        private static BrushPropertiesGroupState _brushOverwriteGroupState;
        private void BrushGroup()
        {
            ToolProfileGUI(BrushManager.instance);
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                BrushToolBaseSettingsGUI(BrushManager.settings);
                EditorGUIUtility.labelWidth = 150;
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var avoidOverlapping = (BrushToolSettings.AvoidOverlappingType)
                        EditorGUILayout.Popup("Avoid Overlapping:",
                        (int)BrushManager.settings.avoidOverlapping, _avoidOverlappingTypeNames);
                    if (check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        BrushManager.settings.avoidOverlapping = avoidOverlapping;
                    }
                }
                if (BrushManager.settings.brushShape != BrushToolBase.BrushShape.POINT)
                {
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        using (var check = new EditorGUI.ChangeCheckScope())
                        {
                            var heightType = (BrushToolSettings.HeightType)
                                EditorGUILayout.Popup("Max Height From center:",
                                (int)BrushManager.settings.heightType, _heightTypeNames);
                            if (check.changed)
                            {
                                Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                                BrushManager.settings.heightType = heightType;
                                if (heightType == BrushToolSettings.HeightType.RADIUS)
                                    BrushManager.settings.maxHeightFromCenter = BrushManager.settings.radius;
                                SceneView.RepaintAll();
                            }
                        }
                        using (new EditorGUI.DisabledGroupScope(
                            BrushManager.settings.heightType == BrushToolSettings.HeightType.RADIUS))
                        {
                            using (var check = new EditorGUI.ChangeCheckScope())
                            {
                                var maxHeightFromCenter = Mathf.Abs(EditorGUILayout.FloatField("Value:",
                                    BrushManager.settings.maxHeightFromCenter));
                                if (check.changed)
                                {
                                    Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                                    BrushManager.settings.maxHeightFromCenter = maxHeightFromCenter;
                                    SceneView.RepaintAll();
                                }
                            }
                        }
                    }
                }
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Surface Filters", EditorStyles.boldLabel);
                EditorGUIUtility.labelWidth = 110;
                using (new GUILayout.HorizontalScope())
                {
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var minSlope = BrushManager.settings.slopeFilter.min;
                        var maxSlope = BrushManager.settings.slopeFilter.max;
                        EditorGUILayout.MinMaxSlider("Slope Angle:", ref minSlope, ref maxSlope, 0, 90);
                        minSlope = Mathf.Round(minSlope);
                        maxSlope = Mathf.Round(maxSlope);
                        GUILayout.Label("[" + minSlope.ToString("00") + "°," + maxSlope.ToString("00") + "°]");
                        if (check.changed)
                        {
                            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                            BrushManager.settings.slopeFilter.v1 = minSlope;
                            BrushManager.settings.slopeFilter.v2 = maxSlope;
                            SceneView.RepaintAll();
                        }
                    }
                }

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var mask = EditorGUILayout.MaskField("Layers:",
                        EditorGUIUtils.LayerMaskToField(BrushManager.settings.layerFilter), InternalEditorUtility.layers);
                    if (check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        BrushManager.settings.layerFilter = EditorGUIUtils.FieldToLayerMask(mask);
                        SceneView.RepaintAll();
                    }
                }

                EditorGUIUtility.labelWidth = 108;
                var field = EditorGUIUtils.MultiTagField.Instantiate("Tags:", BrushManager.settings.tagFilter, null);
                field.OnChange += OnTagFilterChanged;
            }

            PaintSettingsGUI(BrushManager.settings, BrushManager.settings);
            OverwriteBrushPropertiesGUI(BrushManager.settings, ref _brushOverwriteGroupState);
        }

        private void OnTagFilterChanged(List<string> prevFilter, List<string> newFilter, string key)
        {
            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
            BrushManager.settings.tagFilter = newFilter;
        }
        #endregion

        #region ERASER
        private void EraserGroup()
        {
            EditorGUIUtility.labelWidth = 60;
            var settings = EraserManager.settings;
            using (new GUILayout.VerticalScope(EditorStyles.helpBox)) RadiusSlider(settings);
            ModifierGroup(settings);

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var outermostFilter = EditorGUILayout.ToggleLeft("Outermost prefab filter",
                        settings.outermostPrefabFilter);
                    if (check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        settings.outermostPrefabFilter = outermostFilter;
                    }
                }
                if (!settings.outermostPrefabFilter)
                    GUILayout.Label("When you delete a child of a prefab, the prefab will be unpacked.",
                        EditorStyles.helpBox);
            }
        }
        #endregion

        #region GRAVITY
        private static BrushPropertiesGroupState _gravityOverwriteGroupState;
        private void GravityGroup()
        {
            ToolProfileGUI(GravityToolManager.instance);
            BrushToolBaseSettingsGUI(GravityToolManager.settings);
            EditorGUIUtility.labelWidth = 120;
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var settings = GravityToolManager.settings.Clone();
                    var data = settings.simData;
                    settings.height = EditorGUILayout.FloatField("Height:", settings.height);
                    data.maxIterations = EditorGUILayout.IntField("Max Iterations:", data.maxIterations);
                    data.maxSpeed = EditorGUILayout.FloatField("Max Speed:", data.maxSpeed);
                    data.maxAngularSpeed = EditorGUILayout.FloatField("Max Angular Speed:", data.maxAngularSpeed);
                    data.mass = EditorGUILayout.FloatField("Mass:", data.mass);
                    data.drag = EditorGUILayout.FloatField("Drag:", data.drag);
                    data.angularDrag = EditorGUILayout.FloatField("Angular Drag:", data.angularDrag);
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        data.changeLayer = EditorGUILayout.ToggleLeft("Change Layer Temporarily", data.changeLayer);
                        if (data.changeLayer)
                            data.tempLayer = EditorGUILayout.LayerField("Temp layer:", data.tempLayer);
                    }
                    if (check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        GravityToolManager.settings.Copy(settings);
                        SceneView.RepaintAll();
                    }
                }
            }
            PaintToolSettingsGUI(GravityToolManager.settings);
            OverwriteBrushPropertiesGUI(GravityToolManager.settings, ref _gravityOverwriteGroupState);
        }
        #endregion

        #region LINE
        private static readonly string[] _lineModeNames = { "Auto", "Paint on surface", "Paint on the line" };
        private static readonly string[] _lineSpacingNames = { "Bounds", "Constant" };
        private static readonly string[] _lineAxesAlongTheLineNames = { "X", "Z" };
        private static string[] _shapeProjDirNames = new string[] { "+X", "-X", "+Y", "-Y", "+Z", "-Z", "Plane Axis" };

        private static int _lineProjDirIdx = 6;
        private static BrushPropertiesGroupState _lineOverwriteGroupState;
        private void LineBaseGUI(LineSettings lineSettings)
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var settings = lineSettings.Clone();
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    settings.mode = (LineSettings.PaintMode)
                    EditorGUILayout.Popup("Paint Mode:", (int)settings.mode, _lineModeNames);
                    var dirNames = lineSettings is ShapeSettings ? _shapeProjDirNames : _dirNames;

                    var shapeSettings = settings as ShapeSettings;
                    if (shapeSettings != null)
                    {
                        _lineProjDirIdx = shapeSettings.projectInNormalDir ? _lineProjDirIdx = 6
                            : Array.IndexOf(_dir, settings.projectionDirection);
                    }
                    else _lineProjDirIdx = Array.IndexOf(_dir, settings.projectionDirection);
                    if (_lineProjDirIdx == -1) _lineProjDirIdx = 3;

                    _lineProjDirIdx = EditorGUILayout.Popup("Pojection Direction:", _lineProjDirIdx, dirNames);


                    if (shapeSettings != null) shapeSettings.projectInNormalDir = _lineProjDirIdx == 6;
                    settings.projectionDirection = _lineProjDirIdx == 6
                        ? -ShapeData.instance.normal : _dir[_lineProjDirIdx];
                }
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    settings.objectsOrientedAlongTheLine
                        = EditorGUILayout.ToggleLeft("Orient Along the Line", settings.objectsOrientedAlongTheLine);
                    if (settings.objectsOrientedAlongTheLine)
                    {
                        EditorGUIUtility.labelWidth = 170;
                        settings.axisOrientedAlongTheLine
                            = EditorGUILayout.Popup("Axis Oriented Along the Line:",
                            settings.axisOrientedAlongTheLine == AxesUtils.Axis.X ? 0 : 1,
                            _lineAxesAlongTheLineNames) == 0 ? AxesUtils.Axis.X : AxesUtils.Axis.Z;
                    }
                }
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUIUtility.labelWidth = 120;
                    settings.spacingType = (LineSettings.SpacingType)
                        EditorGUILayout.Popup("Spacing:", (int)settings.spacingType, _lineSpacingNames);
                    if (settings.spacingType == LineSettings.SpacingType.CONSTANT)
                        settings.spacing = EditorGUILayout.FloatField("Value:", settings.spacing);
                    settings.gapSize = EditorGUILayout.FloatField("Gap Size:", settings.gapSize);
                    if (PaletteManager.selectedBrushIdx >= 0 && PaletteManager.selectedBrush != null)
                    {
                        var spacing = settings.spacingType == LineSettings.SpacingType.CONSTANT
                            ? settings.spacing : PaletteManager.selectedBrush.minBrushMagnitude;
                        var min = Mathf.Min(0, 0.05f - spacing);
                        settings.gapSize = Mathf.Max(min, settings.gapSize);
                    }
                }
                if (check.changed)
                {
                    Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                    lineSettings.Copy(settings);
                    PWBIO.UpdateStroke();
                    SceneView.RepaintAll();
                }
            }
        }

        private void LineGroup()
        {
            ToolProfileGUI(LineManager.instance);
            EditorGUIUtility.labelWidth = 120;
            LineBaseGUI(LineManager.settings);
            PaintSettingsGUI(LineManager.settings, LineManager.settings);
            OverwriteBrushPropertiesGUI(LineManager.settings, ref _lineOverwriteGroupState);
        }
        #endregion

        #region SHAPE
        private static readonly string[] _shapeTypeNames = { "Circle", "Polygon" };
        private static BrushPropertiesGroupState _shapeOverwriteGroupState;
        private static string[] _shapeDirNames = new string[] { "+X", "-X", "+Y", "-Y", "+Z", "-Z", "Normal to surface" };
        private void ShapeGroup()
        {
            EditorGUIUtility.labelWidth = 100;
            ToolProfileGUI(ShapeManager.instance);
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var shapeType = (ShapeSettings.ShapeType)EditorGUILayout.Popup("Shape:",
                        (int)ShapeManager.settings.shapeType, _shapeTypeNames);
                    if (check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        ShapeManager.settings.shapeType = shapeType;
                        if (shapeType == ShapeSettings.ShapeType.CIRCLE)
                        {
                            ShapeData.instance.UpdateCircleSideCount();
                            ShapeData.instance.Update(false);
                        }
                        PWBIO.UpdateStroke();
                        SceneView.RepaintAll();
                    }
                }
                if (ShapeManager.settings.shapeType == ShapeSettings.ShapeType.POLYGON)
                {
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var sideCount = EditorGUILayout.IntSlider("Number of sides:",
                            ShapeManager.settings.sidesCount, 3, 12);
                        if (check.changed)
                        {
                            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                            ShapeManager.settings.sidesCount = sideCount;
                            ShapeData.instance.UpdateIntersections();
                            PWBIO.UpdateStroke();
                            SceneView.RepaintAll();
                        }
                    }
                }

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var normalDirIdx = ShapeManager.settings.axisNormalToSurface
                        ? 6 : Array.IndexOf(_dir, ShapeManager.settings.normal);
                    EditorGUIUtility.labelWidth = 120;
                    normalDirIdx = EditorGUILayout.Popup("Initial axis direction:", normalDirIdx, _shapeDirNames);
                    var axisNormalToSurface = normalDirIdx == 6;
                    if (check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        ShapeManager.settings.axisNormalToSurface = axisNormalToSurface;
                        ShapeManager.settings.normal = normalDirIdx == 6 ? Vector3.up : _dir[normalDirIdx];
                        PWBIO.UpdateStroke();
                        SceneView.RepaintAll();
                    }
                }
            }
            EditorGUIUtility.labelWidth = 120;
            LineBaseGUI(ShapeManager.settings);
            PaintSettingsGUI(ShapeManager.settings, ShapeManager.settings);
            OverwriteBrushPropertiesGUI(ShapeManager.settings, ref _shapeOverwriteGroupState);
        }
        #endregion

        #region TILING
        private static readonly string[] _tilingModeNames = { "Auto", "Paint on surface", "Paint on the plane" };
        private static readonly string[] _tilingCellTypeNames = { "Smallest object", "Biggest object", "Custom" };
        private static BrushPropertiesGroupState _tilingOverwriteGroupState;
        private void TilingGroup()
        {
            ToolProfileGUI(TilingManager.instance);
            EditorGUIUtility.labelWidth = 180;
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var settings = TilingManager.settings.Clone();
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    settings.mode = (TilingSettings.PaintMode)EditorGUILayout.Popup("Paint mode:",
                    (int)settings.mode, _tilingModeNames);
                    using (var angleCheck = new EditorGUI.ChangeCheckScope())
                    {
                        var eulerAngles = settings.rotation.eulerAngles;
                        eulerAngles = EditorGUILayout.Vector3Field("Plane Rotation:", eulerAngles);
                        if (angleCheck.changed)
                        {
                            var newRotation = Quaternion.Euler(eulerAngles);
                            var delta = newRotation * Quaternion.Inverse(settings.rotation);
                            PWBIO.UpdateTilingRotation(delta);
                            settings.rotation = newRotation;
                        }
                    }
                    var axisIdx = EditorGUILayout.Popup("Axis aligned with plane normal: ",
                        settings.axisAlignedWithNormal, _dirNames);
                    settings.axisAlignedWithNormal = axisIdx;
                }
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUIUtility.labelWidth = 76;
                    settings.cellSizeType = (TilingSettings.CellSizeType)
                        EditorGUILayout.Popup("Cell size:", (int)settings.cellSizeType, _tilingCellTypeNames);
                    using (new EditorGUI.DisabledGroupScope(
                        settings.cellSizeType != TilingSettings.CellSizeType.CUSTOM))
                    {
                        settings.cellSize = EditorGUILayout.Vector2Field("", settings.cellSize);
                    }
                }
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    settings.spacing = EditorGUILayout.Vector2Field("Spacing", settings.spacing);
                }
                if (check.changed)
                {
                    Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                    TilingManager.settings.Copy(settings);
                    PWBIO.UpdateStroke();
                    SceneView.RepaintAll();
                }
            }
            PaintSettingsGUI(TilingManager.settings, TilingManager.settings);
            OverwriteBrushPropertiesGUI(TilingManager.settings, ref _tilingOverwriteGroupState);
        }
        #endregion

        #region EXTRUDE
        private static readonly string[] _spaceOptions = { "Global", "Local" };
        private static readonly string[] _rotationOptions = { "First Object Selected", "Last Object Selected" };
        private static readonly string[] _extrudeSpacingOptions = { "Box Size", "Custom" };
        private void ExtrudeGroup()
        {
            ToolProfileGUI(ExtrudeManager.instance);
            EditorGUIUtility.labelWidth = 60;
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var extrudeSettings = ExtrudeManager.settings.Clone();
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    extrudeSettings.space = (Space)(EditorGUILayout.Popup("Space:",
                        (int)extrudeSettings.space, _spaceOptions));
                    if (extrudeSettings.space == Space.Self)
                    {
                        EditorGUIUtility.labelWidth = 150;
                        extrudeSettings.rotationAccordingTo =
                            (ExtrudeSettings.RotationAccordingTo)EditorGUILayout.Popup("Set rotation according to:",
                            (int)extrudeSettings.rotationAccordingTo, _rotationOptions);
                    }
                }
                EditorGUIUtility.labelWidth = 60;
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    extrudeSettings.spacingType = (ExtrudeSettings.SpacingType)EditorGUILayout.Popup("Spacing:",
                        (int)extrudeSettings.spacingType, _extrudeSpacingOptions);
                    if (extrudeSettings.spacingType == ExtrudeSettings.SpacingType.BOX_SIZE)
                        extrudeSettings.multiplier
                            = EditorGUILayout.Vector3Field("Multiplier:", extrudeSettings.multiplier);
                    else extrudeSettings.spacing
                            = EditorGUILayout.Vector3Field("Value:", extrudeSettings.spacing);
                }
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    extrudeSettings.sameParentAsSource
                        = EditorGUILayout.ToggleLeft("Same parent as source", extrudeSettings.sameParentAsSource);
                    if (!extrudeSettings.sameParentAsSource)
                    {
                        extrudeSettings.autoCreateParent
                            = EditorGUILayout.ToggleLeft("Create parent", extrudeSettings.autoCreateParent);
                        if (extrudeSettings.autoCreateParent) extrudeSettings.createSubparent
                                = EditorGUILayout.ToggleLeft("Create sub-parent per prefab", extrudeSettings.createSubparent);
                        else extrudeSettings.parent = (Transform)EditorGUILayout.ObjectField("Parent Transform:",
                                extrudeSettings.parent, typeof(Transform), true);
                    }
                }
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    extrudeSettings.overwritePrefabLayer
                        = EditorGUILayout.ToggleLeft("Overwrite prefab layer", extrudeSettings.overwritePrefabLayer);
                    if (extrudeSettings.overwritePrefabLayer)
                        extrudeSettings.layer = EditorGUILayout.LayerField("Layer:", extrudeSettings.layer);
                }

                if (check.changed)
                {
                    Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                    ExtrudeManager.settings.Copy(extrudeSettings);
                    PWBIO.UpdateSelection();
                    SceneView.RepaintAll();
                }
            }
            EmbedInSurfaceSettingsGUI(ExtrudeManager.settings);
        }
        #endregion

        #region SELECTION TOOL
        private void SelectionGroup()
        {
            ToolProfileGUI(SelectionToolManager.instance);
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUIUtility.labelWidth = 90;
                    var handleSpace = (Space)(EditorGUILayout.Popup("Handle Space:",
                        (int)SelectionToolManager.settings.handleSpace, _spaceOptions));
                    if (SelectionManager.topLevelSelection.Length > 1) SelectionToolManager.settings.boxSpace = Space.World;
                    var boxSpace = SelectionToolManager.settings.boxSpace;
                    using (new EditorGUI.DisabledGroupScope(SelectionManager.topLevelSelection.Length > 1))
                    {
                        boxSpace = (Space)(EditorGUILayout.Popup("Box Space:",
                            (int)SelectionToolManager.settings.boxSpace, _spaceOptions));
                    }
                    if (check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        SelectionToolManager.settings.handleSpace = handleSpace;
                        SelectionToolManager.settings.boxSpace = boxSpace;
                        PWBIO.ResetSelectionRotation();
                        SceneView.RepaintAll();
                    }
                }
            }
            EmbedInSurfaceSettingsGUI(SelectionToolManager.settings);
        }
        #endregion

        #region MIRROR
        private static readonly string[] _mirrorActionNames = { "Transform", "Create" };
        private void MirrorGroup()
        {
            ToolProfileGUI(MirrorManager.instance);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var mirrorSettings = MirrorManager.settings.Clone();
                using (var mirrorCheck = new EditorGUI.ChangeCheckScope())
                {
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUIUtility.labelWidth = 80;
                        mirrorSettings.mirrorPosition = EditorGUILayout.Vector3Field("Position:",
                            mirrorSettings.mirrorPosition);
                        mirrorSettings.mirrorRotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation:",
                            mirrorSettings.mirrorRotation.eulerAngles));
                    }
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUIUtility.labelWidth = 110;
                        mirrorSettings.invertScale
                            = EditorGUILayout.ToggleLeft("Invert scale", mirrorSettings.invertScale);
                        mirrorSettings.reflectRotation
                            = EditorGUILayout.ToggleLeft("Reflect rotation", mirrorSettings.reflectRotation);
                        mirrorSettings.action = (MirrorSettings.MirrorAction)EditorGUILayout.Popup("Action:",
                            (int)mirrorSettings.action, _mirrorActionNames);
                    }
                    if (mirrorCheck.changed) SceneView.RepaintAll();
                }

                if (mirrorSettings.action == MirrorSettings.MirrorAction.CREATE)
                {
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        mirrorSettings.sameParentAsSource
                            = EditorGUILayout.ToggleLeft("Same parent as source", mirrorSettings.sameParentAsSource);
                        if (!mirrorSettings.sameParentAsSource)
                        {
                            mirrorSettings.autoCreateParent
                                = EditorGUILayout.ToggleLeft("Create parent", mirrorSettings.autoCreateParent);
                            if (mirrorSettings.autoCreateParent)
                                mirrorSettings.createSubparent = EditorGUILayout.ToggleLeft("Create sub-parent per prefab",
                                    mirrorSettings.createSubparent);
                            else mirrorSettings.parent = (Transform)EditorGUILayout.ObjectField("Parent Transform:",
                                    mirrorSettings.parent, typeof(Transform), true);
                        }
                    }
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        mirrorSettings.overwritePrefabLayer = EditorGUILayout.ToggleLeft("Overwrite prefab layer",
                            mirrorSettings.overwritePrefabLayer);
                        if (mirrorSettings.overwritePrefabLayer)
                            mirrorSettings.layer = EditorGUILayout.LayerField("Layer:", mirrorSettings.layer);
                    }
                }
                if (check.changed)
                {
                    Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                    MirrorManager.settings.Copy(mirrorSettings);
                    SceneView.RepaintAll();
                }
            }
            EmbedInSurfaceSettingsGUI(MirrorManager.settings);
        }
        #endregion

        #region REPLACER
        private void ReplacerGroup()
        {
            EditorGUIUtility.labelWidth = 60;
            var settings = ReplacerManager.settings;
            using (new GUILayout.VerticalScope(EditorStyles.helpBox)) RadiusSlider(settings);
            ModifierGroup(settings);
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                var keepTargetSize = settings.keepTargetSize;
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    keepTargetSize = EditorGUILayout.ToggleLeft("Keep target size", settings.keepTargetSize);

                    if (check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        settings.keepTargetSize = keepTargetSize;
                    }
                }
                if (keepTargetSize)
                {
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var maintainProportions = EditorGUILayout.ToggleLeft("Maintain proportions",
                            settings.maintainProportions);
                        if (check.changed)
                        {
                            Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                            settings.maintainProportions = maintainProportions;
                        }
                    }
                }
            }
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var outermostFilter = EditorGUILayout.ToggleLeft("Outermost prefab filter",
                        settings.outermostPrefabFilter);
                    if (check.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(this, UNDO_MSG);
                        settings.outermostPrefabFilter = outermostFilter;
                    }
                }
                if (!settings.outermostPrefabFilter)
                    GUILayout.Label("When you replace a child of a prefab, the prefab will be unpacked.",
                        EditorStyles.helpBox);
            }

            using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Replace all selected"))
                {
                    PWBIO.ReplaceAllSelected();
                    SceneView.RepaintAll();
                }
                GUILayout.FlexibleSpace();
            }
        }
        #endregion
    }
}