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
using UnityEngine;
using static PluginMaster.DropUtils;

namespace PluginMaster
{
    public class BrushProperties : EditorWindow, ISerializationCallbackReceiver
    {
        #region COMMON
        [SerializeField] PWBData _data = null;

        private GUISkin _skin = null;
        private GUIStyle _itemStyle = null;
        private GUIStyle _cursorStyle = null;
        private GUIStyle _thumbnailToggleStyle = null;
        private Vector2 _mainScrollPosition = Vector2.zero;

        private bool _repaint = false;
        private bool _updateBrushStroke = false;
        private static BrushProperties _instance = null;
        public static BrushProperties instance => _instance;
        [MenuItem("Tools/Plugin Master/Prefab World Builder/Brush Properties", false, 1120)]
        public static void ShowWindow() => _instance = GetWindow<BrushProperties>("Brush Properties");
        public static void RepaintWindow()
        {
            if (_instance != null)
            {
                _instance.Repaint();
                _instance._repaint = true;
            }
        }
        private void OnEnable()
        {
            _instance = this;
            _data = PWBCore.staticData;
            PaletteManager.OnBrushChanged += OnBrushChanged;
            _skin = Resources.Load<GUISkin>("PWBSkin");
            if(_skin == null)
            {
                Close();
                return;
            }
            _itemStyle = _skin.GetStyle("PaletteToggle");
            _cursorStyle = _skin.GetStyle("Cursor");
            _thumbnailToggleStyle = _skin.GetStyle("ThumbnailToggle");
            wantsMouseMove = true;
            wantsMouseEnterLeaveWindow = true;
            PaletteManager.OnSelectionChanged += UpdateBrushSelectionSettings;

            _sameStateIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Same"),
                "All selected brushes define the same value for this element");
            _mixedStateIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Mixed"),
                "The Selection contains different values for this element");
            _changedStateIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Edited"),
                "This value has changed");
            Undo.undoRedoPerformed += Repaint;
        }

        private void OnDisable()
        {
            PaletteManager.OnBrushChanged -= OnBrushChanged;
            PaletteManager.OnSelectionChanged -= UpdateBrushSelectionSettings;
            Undo.undoRedoPerformed -= Repaint;
        }

        private void OnGUI()
        {
            if (_skin == null)
            {
                Close();
                return;
            }
            if (_itemAdded)
            {
                Undo.RegisterCompleteObjectUndo(this, "Add Brush Item");
                PaletteManager.selectedBrush.InsertItemAt(_newItem, _newItemIdx);
                _newItem = null;
                _selectedItemIdx = _newItemIdx;
                _itemAdded = false;
                OnMultiBrushChanged();
                return;
            }
            BrushInputData toggleData = null;
            using (var scrollView = new EditorGUILayout.ScrollViewScope(_mainScrollPosition,
                false, false, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUIStyle.none))
            {
                _mainScrollPosition = scrollView.scrollPosition;

                if (PaletteManager.selectionCount > 1)
                {
                    BrushSelectionFields(ref _brushPosGroupOpen, ref _brushRotGroupOpen,
                        ref _brushScaleGroupOpen, BRUSH_SETTINGS_UNDO_MSG, false, true,
                        PaletteManager.selectedPalette.brushes, PaletteManager.idxSelection,
                        _brushSelectionSettings, _brushSelectionState);
                    return;
                }
                if (PaletteManager.selectedBrushIdx == -1) return;
                bool showBrushGroup = PaletteManager.selectedBrush != null;
                if (showBrushGroup)
                {
                    if (PaletteManager.selectedBrush.items.Length == 0)
                    {
                        showBrushGroup = false;
                        Undo.RegisterCompleteObjectUndo(this, "Delete Brush");
                        PaletteManager.selectedPalette.RemoveBrushAt(PaletteManager.selectedBrushIdx);
                    }
                }
                if (showBrushGroup)
                {
#if UNITY_2019_1_OR_NEWER
                    _brushGroupOpen = EditorGUILayout.BeginFoldoutHeaderGroup(_brushGroupOpen, "Brush Settings");
#else
                    _brushGroupOpen = EditorGUILayout.Foldout(_brushGroupOpen, "Brush Settings");
#endif
                    if (_brushGroupOpen) BrushGroup();
#if UNITY_2019_1_OR_NEWER
                    EditorGUILayout.EndFoldoutHeaderGroup();
#endif
#if UNITY_2019_1_OR_NEWER
                    _multiBrushGroupOpen = EditorGUILayout.BeginFoldoutHeaderGroup(_multiBrushGroupOpen,
                        "Multi Brush Settings");
#else
                    _multiBrushGroupOpen = EditorGUILayout.Foldout(_multiBrushGroupOpen, "Multi Brush Settings");
#endif
                    if (_multiBrushGroupOpen) MultiBrushGroup(ref toggleData);
#if UNITY_2019_1_OR_NEWER
                    EditorGUILayout.EndFoldoutHeaderGroup();
#endif
                }

            }
            OnObjectSelectorClosed();
            ItemMouseEventHandler(toggleData);
            var eventType = Event.current.rawType;
            if (eventType == EventType.MouseMove || eventType == EventType.MouseUp)
            {
                _moveItem.to = -1;
                _draggingItem = false;
                _showCursor = false;
            }
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                GUI.FocusControl(null);
                Repaint();
            }
        }

        private void Update()
        {
            if (mouseOverWindow != this)
            {
                _moveItem.to = -1;
                _showCursor = false;
            }
            else if (_draggingItem) _showCursor = true;
            if (_repaint)
            {
                _repaint = false;
                Repaint();
            }
            if (_updateBrushStroke)
            {
                _updateBrushStroke = false;
                BrushstrokeManager.UpdateBrushstroke();
            }
        }

        private void OnBrushChanged()
        {
            _selectedItemIdx = 0;
            _repaint = true;
        }

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            _repaint = true;
            _updateBrushStroke = true;
        }
        #endregion

        #region BRUSH SELECTION
        private GUIContent _sameStateIcon = null;
        private GUIContent _mixedStateIcon = null;
        private GUIContent _changedStateIcon = null;

        private enum SelectionFieldState { SAME, MIXED, CHANGED }
        private class BrushSelectionState
        {
            public SelectionFieldState surfaceDistance = SelectionFieldState.SAME;
            public SelectionFieldState embedInSurface = SelectionFieldState.SAME;
            public SelectionFieldState embedAtPivotHeight = SelectionFieldState.SAME;
            public SelectionFieldState localPositionOffset = SelectionFieldState.SAME;
            public SelectionFieldState rotateToTheSurface = SelectionFieldState.SAME;
            public SelectionFieldState eulerOffset = SelectionFieldState.SAME;
            public SelectionFieldState addRandomRotation = SelectionFieldState.SAME;
            public SelectionFieldState randomEulerOffset = SelectionFieldState.SAME;
            public SelectionFieldState separateScaleAxes = SelectionFieldState.SAME;
            public SelectionFieldState scaleMultiplier = SelectionFieldState.SAME;
            public SelectionFieldState randomScaleMultiplier = SelectionFieldState.SAME;
            public SelectionFieldState randomScaleMultiplierRange = SelectionFieldState.SAME;
            public virtual bool changed
                => surfaceDistance == SelectionFieldState.CHANGED
                || embedInSurface == SelectionFieldState.CHANGED
                || embedAtPivotHeight == SelectionFieldState.CHANGED
                || localPositionOffset == SelectionFieldState.CHANGED
                || rotateToTheSurface == SelectionFieldState.CHANGED
                || eulerOffset == SelectionFieldState.CHANGED
                || addRandomRotation == SelectionFieldState.CHANGED
                || randomEulerOffset == SelectionFieldState.CHANGED
                || separateScaleAxes == SelectionFieldState.CHANGED
                || scaleMultiplier == SelectionFieldState.CHANGED
                || randomScaleMultiplier == SelectionFieldState.CHANGED
                || randomScaleMultiplierRange == SelectionFieldState.CHANGED;
            public virtual void Reset()
            {
                surfaceDistance = SelectionFieldState.SAME;
                embedInSurface = SelectionFieldState.SAME;
                embedAtPivotHeight = SelectionFieldState.SAME;
                localPositionOffset = SelectionFieldState.SAME;
                rotateToTheSurface = SelectionFieldState.SAME;
                eulerOffset = SelectionFieldState.SAME;
                addRandomRotation = SelectionFieldState.SAME;
                randomEulerOffset = SelectionFieldState.SAME;
                separateScaleAxes = SelectionFieldState.SAME;
                scaleMultiplier = SelectionFieldState.SAME;
                randomScaleMultiplier = SelectionFieldState.SAME;
                randomScaleMultiplierRange = SelectionFieldState.SAME;
            }
        }

        private void UpdateBrushSelectionSettings(int[] selection, BrushSettings[] settingsArray,
            BrushSelectionState brushSelectionState, BrushSettings brushSelectionSettings)
        {
            if(brushSelectionSettings == null) brushSelectionSettings = settingsArray[selection[0]].Clone();
            if (selection.Length == 0) return;
            if (settingsArray.Length <= selection[0]) return;
            brushSelectionState.Reset();
            if (selection.Length > 0) brushSelectionSettings.Copy(settingsArray[selection[0]]);
            GUI.FocusControl(null);
            _repaint = true;
        }

        private GUIContent GetStateGUIContent(SelectionFieldState state)
                => state == SelectionFieldState.SAME ? _sameStateIcon : state == SelectionFieldState.MIXED
                ? _mixedStateIcon : _changedStateIcon;

        private void UpdateSelectionState(BrushSettings[] settingsArray, int[] selection, BrushSelectionState brushSelectionState)
        {
            for (int i = 0; i < selection.Length - 1; ++i)
            {
                var brush = settingsArray[selection[i]];
                var nextBrush = settingsArray[selection[i + 1]];
                if (brushSelectionState.embedInSurface != SelectionFieldState.CHANGED
                    && brush.embedInSurface != nextBrush.embedInSurface)
                    brushSelectionState.embedInSurface = SelectionFieldState.MIXED;
                if (brushSelectionState.embedAtPivotHeight != SelectionFieldState.CHANGED
                    && brush.embedAtPivotHeight != nextBrush.embedAtPivotHeight)
                    brushSelectionState.embedInSurface = SelectionFieldState.MIXED;
                if (brushSelectionState.surfaceDistance != SelectionFieldState.CHANGED
                    && brush.surfaceDistance != nextBrush.surfaceDistance)
                    brushSelectionState.surfaceDistance = SelectionFieldState.MIXED;
                if (brushSelectionState.localPositionOffset != SelectionFieldState.CHANGED
                    && brush.localPositionOffset != nextBrush.localPositionOffset)
                    brushSelectionState.localPositionOffset = SelectionFieldState.MIXED;
                if (brushSelectionState.rotateToTheSurface != SelectionFieldState.CHANGED
                    && brush.rotateToTheSurface != nextBrush.rotateToTheSurface)
                    brushSelectionState.rotateToTheSurface = SelectionFieldState.MIXED;
                if (brushSelectionState.addRandomRotation != SelectionFieldState.CHANGED
                    && brush.addRandomRotation != nextBrush.addRandomRotation)
                    brushSelectionState.addRandomRotation = SelectionFieldState.MIXED;
                if (brushSelectionState.eulerOffset != SelectionFieldState.CHANGED
                    && brush.eulerOffset != nextBrush.eulerOffset)
                    brushSelectionState.eulerOffset = SelectionFieldState.MIXED;
                if (brushSelectionState.randomEulerOffset != SelectionFieldState.CHANGED
                    && brush.randomEulerOffset != nextBrush.randomEulerOffset)
                    brushSelectionState.randomEulerOffset = SelectionFieldState.MIXED;
                if (brushSelectionState.randomScaleMultiplier != SelectionFieldState.CHANGED
                    && brush.randomScaleMultiplier != nextBrush.randomScaleMultiplier)
                    brushSelectionState.randomScaleMultiplier = SelectionFieldState.MIXED;
                if (brushSelectionState.separateScaleAxes != SelectionFieldState.CHANGED
                    && brush.separateScaleAxes != nextBrush.separateScaleAxes)
                    brushSelectionState.separateScaleAxes = SelectionFieldState.MIXED;
                if (brushSelectionState.scaleMultiplier != SelectionFieldState.CHANGED
                    && brush.scaleMultiplier != nextBrush.scaleMultiplier)
                    brushSelectionState.scaleMultiplier = SelectionFieldState.MIXED;
                if (brushSelectionState.randomScaleMultiplierRange != SelectionFieldState.CHANGED
                    && brush.randomScaleMultiplierRange != nextBrush.randomScaleMultiplierRange)
                    brushSelectionState.randomScaleMultiplierRange = SelectionFieldState.MIXED;
            }
        }

        private bool BrushSelectionFields(ref bool brushPosGroupOpen, ref bool brushRotGroupOpen,
            ref bool brushScaleGroupOpen, string undoMsg, bool isItem, bool showApplyAndDiscard,
            BrushSettings[] settingsArray, int[] selection,
            BrushSettings brushSelectionSettings, BrushSelectionState brushSelectionState)
        {
            if (brushSelectionSettings == null)
                UpdateBrushSelectionSettings(selection, settingsArray, brushSelectionState, brushSelectionSettings);
            UpdateSelectionState(settingsArray, selection, brushSelectionState);

            brushPosGroupOpen = EditorGUILayout.Foldout(brushPosGroupOpen, "Position");
            EditorGUIUtility.labelWidth = 110;
            if (brushPosGroupOpen)
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Box(GetStateGUIContent(brushSelectionState.embedInSurface), EditorStyles.label);

                            using (var check = new EditorGUI.ChangeCheckScope())
                            {
                                brushSelectionSettings.embedInSurface
                                    = EditorGUILayout.ToggleLeft("Embed On the Surface",
                                    brushSelectionSettings.embedInSurface);
                                if (check.changed) brushSelectionState.embedInSurface = SelectionFieldState.CHANGED;
                            }
                            GUILayout.FlexibleSpace();
                        }
                        if (brushSelectionSettings.embedAtPivotHeight)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Box(GetStateGUIContent(brushSelectionState.embedAtPivotHeight), EditorStyles.label);

                                using (var check = new EditorGUI.ChangeCheckScope())
                                {
                                    brushSelectionSettings.embedAtPivotHeight
                                        = EditorGUILayout.ToggleLeft("Embed At Pivot Height",
                                        brushSelectionSettings.embedAtPivotHeight);
                                    if (check.changed) brushSelectionState.embedAtPivotHeight = SelectionFieldState.CHANGED;
                                }
                                GUILayout.FlexibleSpace();
                            }
                        }
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Box(GetStateGUIContent(brushSelectionState.surfaceDistance), EditorStyles.label);

                        using (var check = new EditorGUI.ChangeCheckScope())
                        {
                            brushSelectionSettings.surfaceDistance
                                = EditorGUILayout.FloatField("Surface Distance:",
                                brushSelectionSettings.surfaceDistance);
                            if (check.changed)
                                brushSelectionState.surfaceDistance = SelectionFieldState.CHANGED;
                        }
                        GUILayout.FlexibleSpace();
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Box(GetStateGUIContent(brushSelectionState.localPositionOffset),
                            EditorStyles.label);

                        using (var check = new EditorGUI.ChangeCheckScope())
                        {
                            brushSelectionSettings.localPositionOffset
                                = EditorGUILayout.Vector3Field("Local Offset:",
                                brushSelectionSettings.localPositionOffset);
                            if (check.changed) brushSelectionState.localPositionOffset = SelectionFieldState.CHANGED;
                        }
                        GUILayout.FlexibleSpace();
                    }
                }
            }

            brushRotGroupOpen = EditorGUILayout.Foldout(brushRotGroupOpen, "Rotation");
            if (brushRotGroupOpen)
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Box(GetStateGUIContent(brushSelectionState.rotateToTheSurface), EditorStyles.label);

                        using (var check = new EditorGUI.ChangeCheckScope())
                        {
                            brushSelectionSettings.rotateToTheSurface
                                = EditorGUILayout.ToggleLeft("Rotate to the Surface",
                                brushSelectionSettings.rotateToTheSurface);
                            if (check.changed) brushSelectionState.rotateToTheSurface = SelectionFieldState.CHANGED;
                        }
                        GUILayout.FlexibleSpace();
                    }
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Box(GetStateGUIContent(brushSelectionState.addRandomRotation),
                                EditorStyles.label);

                            using (var check = new EditorGUI.ChangeCheckScope())
                            {
                                brushSelectionSettings.addRandomRotation
                                    = EditorGUILayout.Popup("Add Rotation:",
                                    brushSelectionSettings.addRandomRotation ? 1 : 0,
                                    new string[] { "Constant", "Random" }) == 1;
                                if (check.changed)
                                    brushSelectionState.addRandomRotation = SelectionFieldState.CHANGED;
                            }
                            GUILayout.FlexibleSpace();
                        }
                        if (brushSelectionSettings.addRandomRotation)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Box(GetStateGUIContent(brushSelectionState.randomEulerOffset),
                                    EditorStyles.label);

                                using (var check = new EditorGUI.ChangeCheckScope())
                                {
                                    brushSelectionSettings.randomEulerOffset
                                        = EditorGUIUtils.Range3Field(string.Empty,
                                        brushSelectionSettings.randomEulerOffset);
                                    if (check.changed)
                                        brushSelectionState.randomEulerOffset = SelectionFieldState.CHANGED;
                                }
                                GUILayout.FlexibleSpace();
                            }
                        }
                        else
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Box(GetStateGUIContent(brushSelectionState.eulerOffset),
                                    EditorStyles.label);

                                using (var check = new EditorGUI.ChangeCheckScope())
                                {
                                    brushSelectionSettings.eulerOffset = EditorGUILayout.Vector3Field(string.Empty,
                                        brushSelectionSettings.eulerOffset);
                                    if (check.changed) brushSelectionState.eulerOffset = SelectionFieldState.CHANGED;
                                }
                                GUILayout.FlexibleSpace();
                            }
                        }
                    }

                }
            }

            brushScaleGroupOpen = EditorGUILayout.Foldout(brushScaleGroupOpen, "Scale");
            if (brushScaleGroupOpen)
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Box(GetStateGUIContent(brushSelectionState.randomScaleMultiplier),
                            EditorStyles.label);

                        using (var check = new EditorGUI.ChangeCheckScope())
                        {
                            brushSelectionSettings.randomScaleMultiplier = EditorGUILayout.Popup("Multiplier:",
                                brushSelectionSettings.randomScaleMultiplier ? 1
                                : 0, new string[] { "Constant", "Random" }) == 1;
                            if (check.changed)
                                brushSelectionState.randomScaleMultiplier = SelectionFieldState.CHANGED;
                        }
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.Space(4);
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Box(GetStateGUIContent(brushSelectionState.separateScaleAxes), EditorStyles.label);

                        using (var check = new EditorGUI.ChangeCheckScope())
                        {
                            brushSelectionSettings.separateScaleAxes = EditorGUILayout.ToggleLeft("Separate Axes",
                                brushSelectionSettings.separateScaleAxes);
                            if (check.changed) brushSelectionState.separateScaleAxes = SelectionFieldState.CHANGED;
                        }
                        GUILayout.FlexibleSpace();
                    }

                    if (brushSelectionSettings.separateScaleAxes)
                    {
                        if (brushSelectionSettings.randomScaleMultiplier)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Box(GetStateGUIContent(brushSelectionState.randomScaleMultiplierRange),
                                    EditorStyles.label);

                                using (var check = new EditorGUI.ChangeCheckScope())
                                {
                                    brushSelectionSettings.randomScaleMultiplierRange
                                        = EditorGUIUtils.Range3Field(string.Empty,
                                        brushSelectionSettings.randomScaleMultiplierRange);
                                    if (check.changed)
                                        brushSelectionState.randomScaleMultiplierRange = SelectionFieldState.CHANGED;
                                }
                                GUILayout.FlexibleSpace();
                            }
                        }
                        else
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Box(GetStateGUIContent(brushSelectionState.scaleMultiplier),
                                    EditorStyles.label);

                                using (var check = new EditorGUI.ChangeCheckScope())
                                {
                                    brushSelectionSettings.scaleMultiplier
                                        = EditorGUILayout.Vector3Field(string.Empty,
                                        brushSelectionSettings.scaleMultiplier);
                                    if (check.changed)
                                        brushSelectionState.scaleMultiplier = SelectionFieldState.CHANGED;
                                }
                                GUILayout.FlexibleSpace();
                            }
                        }
                    }
                    else
                    {
                        if (brushSelectionSettings.randomScaleMultiplier)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Box(GetStateGUIContent(brushSelectionState.randomScaleMultiplierRange),
                                    EditorStyles.label);

                                using (var check = new EditorGUI.ChangeCheckScope())
                                {
                                    brushSelectionSettings.randomScaleMultiplierRange.z
                                        = brushSelectionSettings.randomScaleMultiplierRange.y
                                        = brushSelectionSettings.randomScaleMultiplierRange.x
                                        = EditorGUIUtils.RangeField(string.Empty,
                                        brushSelectionSettings.randomScaleMultiplierRange.x);
                                    if (check.changed)
                                        brushSelectionState.randomScaleMultiplierRange = SelectionFieldState.CHANGED;
                                }
                                GUILayout.FlexibleSpace();
                            }
                        }
                        else
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Box(GetStateGUIContent(brushSelectionState.scaleMultiplier),
                                    EditorStyles.label);

                                using (var check = new EditorGUI.ChangeCheckScope())
                                {
                                    brushSelectionSettings.scaleMultiplier = Vector3.one
                                        * EditorGUILayout.FloatField(brushSelectionSettings.scaleMultiplier.x);
                                    if (check.changed)
                                        brushSelectionState.scaleMultiplier = SelectionFieldState.CHANGED;
                                }
                                GUILayout.FlexibleSpace();
                            }
                        }
                    }
                }
            }
            if (showApplyAndDiscard)
                return ApplyDiscardButtons(undoMsg, isItem, settingsArray, selection,
                    brushSelectionSettings, brushSelectionState);
            return false;
        }

        private bool ApplyDiscardButtons(string undoMsg, bool isItem,
            BrushSettings[] settingsArray, int[] selection,
            BrushSettings brushSelectionSettings, BrushSelectionState brushSelectionState)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                using (new EditorGUI.DisabledGroupScope(!brushSelectionState.changed))
                {
                    if (GUILayout.Button("Discard")) UpdateBrushSelectionSettings(selection, settingsArray,
                        brushSelectionState, brushSelectionSettings);
                }
                if (GUILayout.Button("Apply"))
                {
                    foreach (var idx in selection)
                    {
                        Undo.RegisterCompleteObjectUndo(this, undoMsg);
                        var brush = isItem ? (BrushSettings)PaletteManager.selectedBrush.GetItemAt(idx)
                            : PaletteManager.selectedPalette.GetBrush(idx);
                        brush.embedInSurface = brushSelectionSettings.embedInSurface;
                        brush.embedAtPivotHeight = brushSelectionSettings.embedAtPivotHeight;
                        brush.surfaceDistance = brushSelectionSettings.surfaceDistance;
                        brush.localPositionOffset = brushSelectionSettings.localPositionOffset;
                        brush.rotateToTheSurface = brushSelectionSettings.rotateToTheSurface;
                        brush.addRandomRotation = brushSelectionSettings.addRandomRotation;
                        brush.eulerOffset = brushSelectionSettings.eulerOffset;
                        brush.randomEulerOffset = brushSelectionSettings.randomEulerOffset;
                        brush.randomScaleMultiplier = brushSelectionSettings.randomScaleMultiplier;
                        brush.separateScaleAxes = brushSelectionSettings.separateScaleAxes;
                        brush.scaleMultiplier = brushSelectionSettings.scaleMultiplier;
                        brush.randomScaleMultiplier = brushSelectionSettings.randomScaleMultiplier;
                        if (ToolManager.tool == ToolManager.PaintTool.PIN
                            && (brushSelectionState.embedInSurface == SelectionFieldState.CHANGED
                            || brushSelectionState.embedAtPivotHeight == SelectionFieldState.CHANGED)) PWBIO.ResetPinValues();
                    }
                    PWBCore.SetSavePending();
                    UpdateBrushSelectionSettings(selection, settingsArray, brushSelectionState, brushSelectionSettings);
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region BRUSH SETTINGS
        private const string BRUSH_SETTINGS_UNDO_MSG = "Brush Settings";

        private bool _brushGroupOpen = true;
        private bool _brushPosGroupOpen = false;
        private bool _brushRotGroupOpen = false;
        private bool _brushScaleGroupOpen = false;

        private BrushSelectionState _brushSelectionState = new BrushSelectionState();
        private BrushSettings _brushSelectionSettings = new BrushSettings();
        private void UpdateBrushSelectionSettings()
        {
            if (PaletteManager.selectedBrushIdx == -1) return;
            UpdateBrushSelectionSettings(PaletteManager.idxSelection, PaletteManager.selectedPalette.brushes,
                _brushSelectionState, _brushSelectionSettings);
            _selection.Clear();
            _selection.Add(0);
            _selectedItemIdx = 0;
            if (PaletteManager.selectedBrush == null)
            {
                PaletteManager.ClearSelection();
                return;
            }
            UpdateBrushSelectionSettings(_selection.ToArray(), PaletteManager.selectedBrush.items,
                _itemSelectionState, _itemSelectionSettings);
        }

        public static bool BrushFields(BrushSettings brush,
            ref bool brushPosGroupOpen, ref bool brushRotGroupOpen, ref bool brushScaleGroupOpen,
            UnityEngine.Object objToUndo, string undoMsg)
        {
            bool changed = false;
            var tempBrush = new BrushSettings(brush);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                brushPosGroupOpen = EditorGUILayout.Foldout(brushPosGroupOpen, "Position");
                EditorGUIUtility.labelWidth = 110;
                bool resetPinValues = false;
                if (brushPosGroupOpen)
                {
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            using (var checkEmbedSettings = new EditorGUI.ChangeCheckScope())
                            {
                                tempBrush.embedInSurface = EditorGUILayout.ToggleLeft("Embed On the Surface",
                                tempBrush.embedInSurface);
                                if (tempBrush.embedInSurface)
                                    tempBrush.embedAtPivotHeight = EditorGUILayout.ToggleLeft("Embed At Pivot Height",
                                        tempBrush.embedAtPivotHeight);
                                if (checkEmbedSettings.changed && ToolManager.tool == ToolManager.PaintTool.PIN
                                    && !(objToUndo is BrushCreationSettingsWindow)) resetPinValues = true;
                            }
                        }
                        tempBrush.surfaceDistance = EditorGUILayout.FloatField("Surface Distance:",
                            tempBrush.surfaceDistance);
                        tempBrush.localPositionOffset = EditorGUILayout.Vector3Field("Local Offset:",
                            tempBrush.localPositionOffset);
                    }
                }

                brushRotGroupOpen = EditorGUILayout.Foldout(brushRotGroupOpen, "Rotation");
                if (brushRotGroupOpen)
                {
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        tempBrush.rotateToTheSurface = EditorGUILayout.ToggleLeft("Rotate to the Surface",
                            tempBrush.rotateToTheSurface);

                        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            tempBrush.addRandomRotation = EditorGUILayout.Popup("Add Rotation:",
                                tempBrush.addRandomRotation ? 1 : 0, new string[] { "Constant", "Random" }) == 1;
                            if (tempBrush.addRandomRotation)
                                tempBrush.randomEulerOffset = EditorGUIUtils.Range3Field(string.Empty,
                                    tempBrush.randomEulerOffset);
                            else tempBrush.eulerOffset = EditorGUILayout.Vector3Field(string.Empty,
                                tempBrush.eulerOffset);
                        }
                    }
                }

                EditorGUIUtility.labelWidth = 65;
                brushScaleGroupOpen = EditorGUILayout.Foldout(brushScaleGroupOpen, "Scale");
                if (brushScaleGroupOpen)
                {
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            tempBrush.randomScaleMultiplier = EditorGUILayout.Popup("Multiplier:",
                                tempBrush.randomScaleMultiplier ? 1 : 0, new string[] { "Constant", "Random" }) == 1;
                            GUILayout.Space(4);
                            tempBrush.separateScaleAxes = EditorGUILayout.ToggleLeft("Separate Axes",
                                tempBrush.separateScaleAxes, GUILayout.Width(102));
                        }
                        if (tempBrush.separateScaleAxes)
                        {
                            if (tempBrush.randomScaleMultiplier)
                                tempBrush.randomScaleMultiplierRange = EditorGUIUtils.Range3Field(string.Empty,
                                    tempBrush.randomScaleMultiplierRange);
                            else tempBrush.scaleMultiplier = EditorGUILayout.Vector3Field(string.Empty,
                                tempBrush.scaleMultiplier);
                        }
                        else
                        {
                            if (tempBrush.randomScaleMultiplier)
                            {
                                 tempBrush.randomScaleMultiplierRange.z
                                     = tempBrush.randomScaleMultiplierRange.y
                                     = tempBrush.randomScaleMultiplierRange.x
                                     = EditorGUIUtils.RangeField(string.Empty, tempBrush.randomScaleMultiplierRange.x);
                            }
                            else
                            {
                                var value = EditorGUILayout.FloatField(tempBrush.scaleMultiplier.x);
                                tempBrush.scaleMultiplier = new Vector3(value, value, value);
                            }
                        }
                    }
                }

                if (check.changed)
                {
                    changed = true;
                    Undo.RegisterCompleteObjectUndo(objToUndo, undoMsg);
                    brush.Copy(tempBrush);
                    PWBCore.SetSavePending();
                    BrushstrokeManager.UpdateBrushstroke();
                    if (resetPinValues) PWBIO.ResetPinValues();
                }
            }
            return changed;
        }

        private void BrushGroup()
        {
            var brush = PaletteManager.selectedBrush;
            if (brush == null) return;
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUIUtility.labelWidth = 50;
                brush.name = EditorGUILayout.DelayedTextField("Name:", brush.name);
                BrushFields(brush, ref _brushPosGroupOpen, ref _brushRotGroupOpen, ref _brushScaleGroupOpen,
                    this, BRUSH_SETTINGS_UNDO_MSG);
            }
        }
        #endregion

        #region MULTIBRUSH SETTINGS
        private const string MULTIBRUSH_SETTINGS_UNDO_MSG = "Multibrush Settings";
        private bool _multiBrushGroupOpen = false;
        private Vector2 _multiBrushScrollPosition = Vector2.zero;
        private bool _multiBrushClipped = false;

        private bool _itemPosGroupOpen = false;
        private bool _itemRotGroupOpen = false;
        private bool _itemScaleGroupOpen = false;
        private bool _frequencyGroupOpen = false;


        private void MultiBrushGroup(ref BrushInputData toggleData)
        {
            if (Event.current.control && Event.current.keyCode == KeyCode.A)
            {
                _selection.Clear();
                for(int i = 0; i < PaletteManager.selectedBrush.itemCount; ++i) _selection.Add(i);
                Repaint();
            }

            if (_moveItem.perform)
            {
                Undo.RegisterCompleteObjectUndo(this, "Change Multi Brush Item Order");
                var selection = _selection.ToArray();
                PaletteManager.selectedBrush.Swap(_moveItem.from, _moveItem.to, ref selection);
                _selection = new List<int>(selection);
                if (selection.Length == 1) _selectedItemIdx = selection[0];
                _moveItem.perform = false;
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                var brushesRect = new Rect();

                using (var scrollView = new EditorGUILayout.ScrollViewScope(
                    _multiBrushScrollPosition, false, false,
                    GUI.skin.horizontalScrollbar, GUIStyle.none, _skin.box,
                    GUILayout.Height(_multiBrushClipped ? 102 : 87)))
                {
                    _multiBrushScrollPosition = scrollView.scrollPosition;
                    using (new GUILayout.HorizontalScope())
                    {
                        BrushItems(ref toggleData);
                        GUILayout.FlexibleSpace();
                    }
                    brushesRect = GUILayoutUtility.GetLastRect();
                }
                var scrollViewRect = GUILayoutUtility.GetLastRect();
                if (Event.current.type == EventType.Repaint)
                {
                    var prev = _multiBrushClipped;
                    _multiBrushClipped = (scrollViewRect.width - 8) < brushesRect.width;
                    if (prev != _multiBrushClipped) Repaint();
                }
                if (scrollViewRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.ContextClick)
                    {
                        var menu = new GenericMenu();
                        menu.AddItem(new GUIContent("New Item..."), false, AddItemAt,
                            PaletteManager.selectedBrush.items.Length);
                        menu.AddItem(new GUIContent("New Items From Folder..."), false,
                            CreateItemsFromEachPrefabInFolder, PaletteManager.selectedBrush.items.Length - 1);
                        menu.AddItem(new GUIContent("New Items From Selection"), false,
                            CreateItemsFromEachPrefabSelected, PaletteManager.selectedBrush.items.Length - 1);
                        menu.ShowAsContext();
                        Event.current.Use();
                    }
                    else if (Event.current.type == EventType.DragUpdated)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        Event.current.Use();
                    }
                    else if (Event.current.type == EventType.DragPerform)
                    {
                        bool multiBrushChanged = false;
                        var droppedItems = GetDroppedPrefabs();
                        foreach (var droppedItem in droppedItems)
                        {
                            var item = new MultibrushItemSettings(droppedItem.obj, PaletteManager.selectedBrush);
                            Undo.RegisterCompleteObjectUndo(this, "Add Brush Item");
                            if (_moveItem.to == -1)
                            {
                                PaletteManager.selectedBrush.AddItem(item);
                                _selectedItemIdx = PaletteManager.selectedBrush.items.Length - 1;
                            }
                            else
                            {
                                PaletteManager.selectedBrush.InsertItemAt(item, _moveItem.to);
                                _selectedItemIdx = _moveItem.to;
                            }
                            multiBrushChanged = true;
                        }
                        if (multiBrushChanged) OnMultiBrushChanged();
                        Event.current.Use();
                    }
                }

                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    if (PaletteManager.selectedBrush == null) return;
                    var selectedItem = GetSelectedItem(PaletteManager.selectedBrush);
                    if (selectedItem.prefab == null) return;
                    var itemName = selectedItem.prefab.name;
                    var itemNameStyle = new GUIStyle(EditorStyles.boldLabel);
                    itemNameStyle.alignment = TextAnchor.MiddleCenter;
                    GUILayout.Label((_selectedItemIdx + 1) + ". " + itemName, itemNameStyle);
                    var separatorStyle = new GUIStyle(EditorStyles.toolbarButton);
                    separatorStyle.fixedHeight = 1;
                    GUILayout.Box(GUIContent.none, separatorStyle);
                    _frequencyGroupOpen = EditorGUILayout.Foldout(_frequencyGroupOpen, "Frequency");
                    if (_frequencyGroupOpen) FrequencyGroup();
                    EditorGUIUtility.labelWidth = 150;
                    if (_selection.Count <= 1)
                    {
                        using (var check = new EditorGUI.ChangeCheckScope())
                        {
                            bool overwriteSettings = EditorGUILayout.ToggleLeft("Overwrite Brush Settings",
                                selectedItem.overwriteSettings);
                            if (check.changed)
                            {
                                Undo.RegisterCompleteObjectUndo(this, MULTIBRUSH_SETTINGS_UNDO_MSG);
                                selectedItem.overwriteSettings = overwriteSettings;
                                if (selectedItem.overwriteSettings) selectedItem.Copy(PaletteManager.selectedBrush);
                            }
                        }
                    }
                    else
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Box(GetStateGUIContent(_itemSelectionState.overwriteSettings), EditorStyles.label);
                            using (var check = new EditorGUI.ChangeCheckScope())
                            {
                                _itemSelectionSettings.overwriteSettings
                                    = EditorGUILayout.ToggleLeft("Overwrite Brush Settings",
                                    _itemSelectionSettings.overwriteSettings);
                                if (check.changed) _itemSelectionState.overwriteSettings = SelectionFieldState.CHANGED;
                            }
                            GUILayout.FlexibleSpace();
                        }
                    }
                    if ((_selection.Count > 1 && (_itemSelectionState.overwriteSettings == SelectionFieldState.MIXED
                        || (_itemSelectionState.overwriteSettings != SelectionFieldState.MIXED
                        && _itemSelectionSettings.overwriteSettings)))
                        || (_selection.Count <= 1 && selectedItem.overwriteSettings)) ItemSettingsGroup();
                    if (_selection.Count > 1)
                    {
                        var selection = _selection.ToArray();
                        var settingsArray = PaletteManager.selectedBrush.items;
                        var apply = ApplyDiscardButtons(MULTIBRUSH_SETTINGS_UNDO_MSG, true, settingsArray, selection,
                            _itemSelectionSettings, _itemSelectionState);
                        if (apply)
                        {
                            foreach (var idx in selection)
                            {
                                var brush = PaletteManager.selectedBrush.GetItemAt(idx);
                                brush.overwriteSettings = _itemSelectionSettings.overwriteSettings;
                                brush.frequency = _itemSelectionSettings.frequency;
                            }
                            if (_itemSelectionState.overwriteSettings == SelectionFieldState.CHANGED)
                                _itemSelectionState.overwriteSettings = SelectionFieldState.SAME;
                            if (_itemSelectionState.frecuency == SelectionFieldState.CHANGED)
                                _itemSelectionState.frecuency = SelectionFieldState.SAME;
                        }
                    }

                }
            }
        }

        #region ITEMS
        private int _selectedItemIdx = 0;
        private int _currentPickerId = -1;
        private bool _itemAdded = false;
        private MultibrushItemSettings _newItem = null;
        private int _newItemIdx = -1;

        [SerializeField] private List<int> _selection = new List<int>() { 0 };

        private (int from, int to, bool perform) _moveItem = (0, 0, false);
        private bool _draggingItem = false;
        private Rect _cursorRect = Rect.zero;
        private bool _showCursor = false;

        private class ItemSelectionState : BrushSelectionState
        {
            public SelectionFieldState overwriteSettings = SelectionFieldState.SAME;
            public SelectionFieldState frecuency = SelectionFieldState.SAME;
            public override bool changed => base.changed || embedInSurface == SelectionFieldState.CHANGED
                || frecuency == SelectionFieldState.CHANGED;
            public override void Reset()
            {
                base.Reset();
                overwriteSettings = SelectionFieldState.SAME;
                frecuency = SelectionFieldState.SAME;
            }
        }

        private ItemSelectionState _itemSelectionState = new ItemSelectionState();
        private MultibrushItemSettings _itemSelectionSettings = new MultibrushItemSettings();


        private void ItemSelectionFields(bool checkSelectionIndexes = true)
        {
            var selection = _selection.ToArray();
            var settingsArray = PaletteManager.selectedBrush.items;

            if (checkSelectionIndexes)
            {
                for (int i = 0; i < selection.Length - 1; ++i)
                {
                    var brushIdx = selection[i];
                    var nextBrushIdx = selection[i + 1];
                    if (brushIdx >= settingsArray.Length || nextBrushIdx >= settingsArray.Length)
                    {
                        _selection.Clear();
                        _selection.Add(0);
                        _selectedItemIdx = 0;
                        UpdateBrushSelectionSettings(_selection.ToArray(), settingsArray,
                            _itemSelectionState, _itemSelectionSettings);
                        ItemSelectionFields(false);
                        Repaint();
                        return;
                    }
                }
            }
            UpdateSelectionState(settingsArray, selection, _itemSelectionState);
            _itemSelectionState.overwriteSettings = SelectionFieldState.SAME;
            _itemSelectionState.frecuency = SelectionFieldState.SAME;
            for (int i = 0; i < selection.Length - 1; ++i)
            {
                var brushIdx = selection[i];
                var nextBrushIdx = selection[i + 1];
                var brush = settingsArray[brushIdx];
                var nextBrush = settingsArray[nextBrushIdx];
                if (_itemSelectionState.overwriteSettings != SelectionFieldState.CHANGED
                   && brush.overwriteSettings != nextBrush.overwriteSettings)
                    _itemSelectionState.overwriteSettings = SelectionFieldState.MIXED;
                if (_itemSelectionState.frecuency != SelectionFieldState.CHANGED
                   && brush.frequency != nextBrush.frequency)
                    _itemSelectionState.frecuency = SelectionFieldState.MIXED;
            }

            BrushSelectionFields(ref _itemPosGroupOpen, ref _itemRotGroupOpen, ref _itemScaleGroupOpen,
                    MULTIBRUSH_SETTINGS_UNDO_MSG, true, false, settingsArray, selection,
                    _itemSelectionSettings, _itemSelectionState);
        }
        private void BrushItems(ref BrushInputData toggleData)
        {
            var brush = PaletteManager.selectedBrush;
            var items = brush.items;
            for (int i = 0; i < items.Length; ++i)
            {
                var item = items[i];
                BrushItem(item, i, ref toggleData);
            }
            if (_showCursor) GUI.Box(_cursorRect, string.Empty, _cursorStyle);
        }

        private void SelectPrefabs(object idx)
        {
            var prefabs = new List<GameObject>();
            if (_selection.Contains((int)idx))
                foreach (int selectedIdx in _selection)
                    prefabs.Add(PaletteManager.selectedBrush.GetItemAt(selectedIdx).prefab);
            else prefabs.Add(PaletteManager.selectedBrush.GetItemAt((int)idx).prefab);
            Selection.objects = prefabs.ToArray();
        }

        private void OpenPrefab(object idx)
            => AssetDatabase.OpenAsset(PaletteManager.selectedBrush.GetItemAt((int)idx).prefab);

        private void UpdateThumbnail(object idx)
        {
            var item = PaletteManager.selectedBrush.GetItemAt((int)idx);
            item.UpdateThumbnail();
        }

        private void EditThumbnail(object idx)
        {
            var itemIdx = (int)idx;
            var item = PaletteManager.selectedBrush.GetItemAt(itemIdx);
            ThumbnailEditorWindow.ShowWindow(item, itemIdx);
        }

        private void CopyThumbnailSettings(object idx)
        {
            var item = PaletteManager.selectedBrush.GetItemAt((int)idx);
            PaletteManager.clipboardThumbnailSettings = item.thumbnailSettings.Clone();
            PaletteManager.clipboardOverwriteThumbnailSettings = item.overwriteThumbnailSettings
                ? PaletteManager.Trit.TRUE: PaletteManager.Trit.FALSE;
        }

        private void PasteThumbnailSettings(object idx)
        {
            if (PaletteManager.clipboardThumbnailSettings == null) return;
            Undo.RegisterCompleteObjectUndo(this, "Paste Thumbnail Settings");
            void Paste(MultibrushItemSettings item)
            {
                if (PaletteManager.clipboardOverwriteThumbnailSettings != PaletteManager.Trit.SAME)
                {
                    item.overwriteThumbnailSettings
                        = PaletteManager.clipboardOverwriteThumbnailSettings == PaletteManager.Trit.TRUE;
                }
                item.thumbnailSettings.Copy(PaletteManager.clipboardThumbnailSettings);
                ThumbnailUtils.UpdateThumbnail(item);
                ThumbnailUtils.UpdateThumbnail(item.parentSettings);
            }
            if (_selection.Contains((int)idx))
            {
                foreach (var i in _selection) Paste(PaletteManager.selectedBrush.GetItemAt(i));
            }
            else Paste(PaletteManager.selectedBrush.GetItemAt((int)idx));
            PWBCore.SetSavePending();
        }

        private void DeleteItem(object obj)
        {
            var idx = (int)obj;
            Undo.RegisterCompleteObjectUndo(this, "Delete Brush Item");
            if (_selection.Contains(idx))
            {
                var descendingSelection = _selection.ToArray();
                Array.Sort<int>(descendingSelection, new Comparison<int>((i1, i2) => i2.CompareTo(i1)));
                foreach (var i in descendingSelection) PaletteManager.selectedBrush.RemoveItemAt(i);
            }
            else PaletteManager.selectedBrush.RemoveItemAt(idx);
            _selectedItemIdx = Mathf.Clamp(_selectedItemIdx, 0, PaletteManager.selectedBrush.itemCount - 1);
            _selection.Clear();
            _selection.Add(_selectedItemIdx);
            OnMultiBrushChanged();
        }

        private void AddItemAt(object obj)
        {
            _newItemIdx = (int)obj;
            _currentPickerId = EditorGUIUtility.GetControlID(FocusType.Passive) + 100;
            EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "t:Prefab", _currentPickerId);
        }

        private void CreateItemsFromEachPrefabInFolder(object obj)
        {
            _newItemIdx = (int)obj;
            var items = GetFolderItems();
            if (items == null) return;
            for (int i = 0; i < items.Length; ++i)
            {
                var item = items[i];
                if (item.obj == null) continue;
                Undo.RegisterCompleteObjectUndo(this, "Add Brush Item");
                _newItem = new MultibrushItemSettings(item.obj, PaletteManager.selectedBrush);
                PaletteManager.selectedBrush.InsertItemAt(_newItem, _newItemIdx + 1 + i);
            }
            OnMultiBrushChanged();
        }

        public void CreateItemsFromEachPrefabSelected(object obj)
        {
            _newItemIdx = (int)obj;
            var selectionPrefabs = SelectionManager.GetSelectionPrefabs();
            if (selectionPrefabs.Length == 0) return;
            for (int i = 0; i < selectionPrefabs.Length; ++i)
            {
                var selectedObj = selectionPrefabs[i];
                if (selectedObj == null) continue;
                Undo.RegisterCompleteObjectUndo(this, "Add Brush Item");
                _newItem = new MultibrushItemSettings(selectedObj, PaletteManager.selectedBrush);
                PaletteManager.selectedBrush.InsertItemAt(_newItem, _newItemIdx + 1 + i);
            }
            OnMultiBrushChanged();
        }

        private void BrushItem(MultibrushItemSettings item, int index, ref BrushInputData data)
        {
            var style = new GUIStyle(_itemStyle);
            var selection = _selection.ToArray();
            var settingsArray = PaletteManager.selectedBrush.items;

            for (int i = 0; i < selection.Length; ++i)
            {
                if (selection[i] >= settingsArray.Length)
                {
                    _selection.Clear();
                    _selection.Add(0);
                    _selectedItemIdx = 0;
                    UpdateBrushSelectionSettings(_selection.ToArray(), settingsArray,
                        _itemSelectionState, _itemSelectionSettings);
                    break;
                }
            }

            if (_selection.Contains(index)) style.normal = _itemStyle.onNormal;
            using (new GUILayout.VerticalScope(style))
            {
                var nameStyle = GUIStyle.none;
                nameStyle.margin = new RectOffset(2, 2, 0, 1);
                nameStyle.clipping = TextClipping.Clip;
                nameStyle.fontSize = 8;
                if (item.prefab == null) return;
                GUILayout.Box(new GUIContent((index + 1).ToString() + ". " + item.prefab.name, item.prefab.name),
                    nameStyle, GUILayout.Width(56));
                GUILayout.Box(new GUIContent(item.thumbnail, item.prefab.name), GUIStyle.none,
                    GUILayout.Width(64), GUILayout.Height(64));
            }

            var rect = GUILayoutUtility.GetLastRect();
            var toggleRect = new Rect(rect.xMax - 16, rect.yMax - 16, 14, 14);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var include = GUI.Toggle(toggleRect, item.includeInThumbnail, GUIContent.none, _thumbnailToggleStyle);
                if(check.changed)
                {
                    Undo.RegisterCompleteObjectUndo(this, "Edit thumbnail settings");
                    item.includeInThumbnail = include;
                    ThumbnailUtils.UpdateThumbnail(item.parentSettings);
                    PWBCore.SetSavePending();
                }
            }
            if (rect.Contains(Event.current.mousePosition))
                data = new BrushInputData(index, rect, Event.current.type, Event.current.control,
                    Event.current.shift, Event.current.mousePosition.x);
        }
        private void CopyItemSettings(object idx)
            => PaletteManager.clipboardSetting = PaletteManager.selectedBrush.items[(int)idx].Clone();

        private void PasteItemSettings(object idx)
        {
            Undo.RegisterCompleteObjectUndo(this, "Paste Brush Item Settings");
            PaletteManager.selectedBrush.items[(int)idx].Copy(PaletteManager.clipboardSetting);
            PWBCore.SetSavePending();
        }

        private void DuplicateItem(object obj)
        {
            var idx = (int)obj;
            Undo.RegisterCompleteObjectUndo(this, "Duplicate Brush Item");
            if (_selection.Contains(idx))
            {
                var descendingSelection = _selection.ToArray();
                Array.Sort<int>(descendingSelection, new Comparison<int>((i1, i2) => i2.CompareTo(i1)));
                for (int i = 0; i < descendingSelection.Length; ++i)
                {
                    PaletteManager.selectedBrush.Duplicate(descendingSelection[i]);
                    descendingSelection[i] += descendingSelection.Length - 1 - i;
                }
                _selection.Clear();
                _selection.AddRange(descendingSelection);
            }
            else PaletteManager.selectedBrush.Duplicate(idx);
            OnMultiBrushChanged();
            BrushstrokeManager.UpdateBrushstroke();
        }

        private void ItemMouseEventHandler(BrushInputData data)
        {
            if (data == null) return;
            if (data.eventType == EventType.MouseUp && Event.current.button == 0)
            {
                void SelectionChanged()
                {
                    var selection = _selection.ToArray();
                    var settingsArray = PaletteManager.selectedBrush.items;
                    UpdateBrushSelectionSettings(_selection.ToArray(), settingsArray,
                            _itemSelectionState, _itemSelectionSettings);
                    _itemSelectionState.overwriteSettings = SelectionFieldState.SAME;

                    for (int i = 0; i < selection.Length - 1; ++i)
                    {
                        var brushIdx = selection[i];
                        var nextBrushIdx = selection[i + 1];
                        var brush = settingsArray[brushIdx];
                        var nextBrush = settingsArray[nextBrushIdx];
                        if (brush.overwriteSettings != nextBrush.overwriteSettings)
                        {
                            _itemSelectionState.overwriteSettings = SelectionFieldState.MIXED;
                            _itemSelectionSettings.overwriteSettings = true;
                        }
                    }
                    if (_itemSelectionState.overwriteSettings == SelectionFieldState.SAME)
                        _itemSelectionSettings.overwriteSettings = settingsArray[selection[0]].overwriteSettings;
                    _itemSelectionSettings.frequency = settingsArray[selection[0]].frequency;
                }
                void DeselectAllButCurrent()
                {
                    _selection.Clear();
                    _selection.Add(data.index);
                    _selectedItemIdx = data.index;
                    SelectionChanged();
                }
                void ToggleCurrent()
                {
                    if (_selection.Contains(data.index))
                    {
                        if (_selection.Count <= 1) return;
                        _selectedItemIdx = Mathf.Clamp(_selection.IndexOf(data.index), 0,
                            PaletteManager.selectedBrush.itemCount - 2);
                        _selection.Remove(data.index);
                    }
                    else
                    {
                        _selection.Add(data.index);
                        _selectedItemIdx = data.index;
                    }
                    SelectionChanged();
                }
                if (data.shift)
                {
                    var sign = (int)Mathf.Sign(data.index - _selectedItemIdx);
                    if (sign != 0)
                    {
                        _selection.Clear();
                        for (int i = _selectedItemIdx; i != data.index; i += sign) _selection.Add(i);
                        _selection.Add(data.index);
                        SelectionChanged();
                    }
                    else DeselectAllButCurrent();
                }
                else if (data.control) ToggleCurrent();
                else DeselectAllButCurrent();

                Repaint();
                Event.current.Use();
            }
            else if (data.eventType == EventType.ContextClick)
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Select Prefab" + (_selection.Count > 1 ? "s" : "")),
                    false, SelectPrefabs, data.index);
                if (_selection.Count == 1)
                    menu.AddItem(new GUIContent("Open Prefab"), false, OpenPrefab, data.index);
                menu.AddSeparator(string.Empty);
                menu.AddItem(new GUIContent("Update Thumbnail"), false, UpdateThumbnail, data.index);
                menu.AddItem(new GUIContent("Edit Thumbnail"), false, EditThumbnail, data.index);
                menu.AddItem(new GUIContent("Copy Thumbnail Settings"), false, CopyThumbnailSettings, data.index);
                if (PaletteManager.clipboardThumbnailSettings != null)
                    menu.AddItem(new GUIContent("Paste Thumbnail Settings"), false, PasteThumbnailSettings, data.index);
                menu.AddSeparator(string.Empty);
                if (PaletteManager.selectedBrush.items.Length > 1
                    && _selection.Count < PaletteManager.selectedBrush.items.Length)
                    menu.AddItem(new GUIContent("Delete"), false, DeleteItem, data.index);
                menu.AddItem(new GUIContent("Duplicate"), false, DuplicateItem, data.index);
                if (_selection.Count == 1)
                    menu.AddItem(new GUIContent("Copy Brush Settings"), false, CopyItemSettings, data.index);
                if (PaletteManager.clipboardSetting != null)
                    menu.AddItem(new GUIContent("Paste Brush Settings"), false, PasteItemSettings, data.index);
                menu.AddSeparator(string.Empty);
                menu.AddItem(new GUIContent("New Item..."), false, AddItemAt, data.index);
                menu.AddItem(new GUIContent("New Items From Folder..."),
                    false, CreateItemsFromEachPrefabInFolder, data.index);
                menu.AddItem(new GUIContent("New Items From Selection"),
                    false, CreateItemsFromEachPrefabSelected, data.index);
                menu.ShowAsContext();
                Event.current.Use();
            }
            else if (data.eventType == EventType.MouseDrag)
            {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.StartDrag("Dragging brush");
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                _draggingItem = true;
                _moveItem.from = data.index;
                _moveItem.perform = false;
                _moveItem.to = -1;
                Event.current.Use();
            }
            else if (data.eventType == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                var size = new Vector2(4, data.rect.height);
                var min = data.rect.min;
                var toTheRight = data.mouseX - data.rect.center.x > 0;
                min.x = toTheRight ? data.rect.max.x : min.x - size.x;
                _cursorRect = new Rect(min, size);
                _showCursor = true;
                _moveItem.to = data.index;
                if (toTheRight) ++_moveItem.to;
                Event.current.Use();
            }
            else if (data.eventType == EventType.DragPerform)
            {
                var toTheRight = data.mouseX - data.rect.center.x > 0;
                _moveItem.to = data.index;
                if (toTheRight) ++_moveItem.to;
                if (_draggingItem)
                {
                    _moveItem.perform = _moveItem.from != _moveItem.to;
                    _draggingItem = false;
                }
                _showCursor = false;
                Event.current.Use();
            }
            else if (data.eventType == EventType.DragExited)
            {
                _showCursor = false;
                _draggingItem = false;
                _moveItem.to = -1;
            }
        }

        private void OnObjectSelectorClosed()
        {
            if (Event.current.commandName == "ObjectSelectorClosed"
                && EditorGUIUtility.GetObjectPickerControlID() == _currentPickerId)
            {
                var obj = EditorGUIUtility.GetObjectPickerObject();
                if (obj != null)
                {
                    var prefabType = PrefabUtility.GetPrefabAssetType(obj);
                    if (prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant)
                    {
                        _itemAdded = true;
                        _newItem = new MultibrushItemSettings(obj as GameObject, PaletteManager.selectedBrush);
                    }
                }
                _currentPickerId = -1;
            }
        }

        private void OnMultiBrushChanged()
        {
            if (PrefabPalette.instance != null) PrefabPalette.instance.OnPaletteChange();
        }

        private void ItemSettingsGroup()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                var item = GetSelectedItem(PaletteManager.selectedBrush);
                if (_selection.Count <= 1)
                    BrushFields(item, ref _itemPosGroupOpen, ref _itemRotGroupOpen, ref _itemScaleGroupOpen,
                        this, MULTIBRUSH_SETTINGS_UNDO_MSG);
                else ItemSelectionFields();
            }
        }

        private MultibrushItemSettings GetSelectedItem(MultibrushSettings brush)
        {
            if (brush == null) return null;
            var item = brush.GetItemAt(_selectedItemIdx);
            if (item == null)
            {
                _selectedItemIdx = 0;
                item = brush.GetItemAt(_selectedItemIdx);
            }
            return item;
        }
        #endregion

        #region FRECUENCY
        private readonly string[] FRECUENCY_MODES = new string[] { "Random", "Pattern" };

        private Texture2D _warningTexture = null;
        private string _patternWarningMsg = null;
        private PatternMachine _previewPatternMachine = null;
        private void FrequencyGroup()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var brushCheck = new EditorGUI.ChangeCheckScope())
                {
                    var brush = PaletteManager.selectedBrush;
                    var tempBrush = brush.Clone() as MultibrushSettings;
                    bool selectionFrecuencyChanged = false;
                    tempBrush.frequencyMode = (MultibrushSettings.FrecuencyMode)
                        EditorGUILayout.Popup("Frequency Mode:", (int)tempBrush.frequencyMode, FRECUENCY_MODES);
                    var tempItem = GetSelectedItem(tempBrush);
                    if (tempBrush.frequencyMode == MultibrushSettings.FrecuencyMode.RANDOM)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            if (_selection.Count <= 1)
                            {
                                tempItem.frequency = EditorGUILayout.IntField("Frecuency:", tempItem.frequency);
                                GUILayout.Label("in " + tempBrush.totalFrecuency);
                            }
                            else
                            {
                                GUILayout.Box(GetStateGUIContent(_itemSelectionState.frecuency), EditorStyles.label);
                                using (var check = new EditorGUI.ChangeCheckScope())
                                {
                                    _itemSelectionSettings.frequency = EditorGUILayout.IntField("Frecuency:",
                                        _itemSelectionSettings.frequency);
                                    int totalFrecuency = _itemSelectionSettings.frequency * _selection.Count;
                                    for (int i = 0; i < PaletteManager.selectedBrush.itemCount; ++i)
                                    {
                                        if (_selection.Contains(i)) continue;
                                        totalFrecuency += PaletteManager.selectedBrush.GetItemAt(i).frequency;
                                    }
                                    GUILayout.Label("in " + totalFrecuency);
                                    if (check.changed)
                                    {
                                        _itemSelectionState.frecuency = SelectionFieldState.CHANGED;
                                        selectionFrecuencyChanged = true;
                                    }
                                }
                                GUILayout.FlexibleSpace();
                            }
                        }
                    }
                    else
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            using (var patternCheck = new EditorGUI.ChangeCheckScope())
                            {
                                tempBrush.pattern = EditorGUILayout.TextField("Pattern:", tempBrush.pattern);
                                if (patternCheck.changed || tempBrush.patternMachine != _previewPatternMachine
                                    || tempBrush.patternMachine == null)
                                {
                                    _patternWarningMsg = null;
                                    tempBrush.patternMachine = null;
                                    switch (PatternMachine.Validate(tempBrush.pattern, tempBrush.items.Length,
                                        out PatternMachine.Token[] tokens))
                                    {
                                        case PatternMachine.ValidationResult.EMPTY:
                                            _patternWarningMsg = "Empty pattern"; break;
                                        case PatternMachine.ValidationResult.INDEX_OUT_OF_RANGE:
                                            _patternWarningMsg = "Index out of range"; break;
                                        case PatternMachine.ValidationResult.MISPLACED_PERIOD:
                                            _patternWarningMsg = "Misplaced Period"; break;
                                        case PatternMachine.ValidationResult.MISPLACED_ASTERISK:
                                            _patternWarningMsg = "Misplaced asterisk"; break;
                                        case PatternMachine.ValidationResult.MISPLACED_COMMA:
                                            _patternWarningMsg = "Mispalced comma"; break;
                                        case PatternMachine.ValidationResult.UNPAIRED_PARENTHESIS:
                                            _patternWarningMsg = "Unpaired parenthesis"; break;
                                        case PatternMachine.ValidationResult.EMPTY_PARENTHESIS:
                                            _patternWarningMsg = "Empty parenthesis"; break;
                                        case PatternMachine.ValidationResult.INVALID_MULTIPLIER:
                                            _patternWarningMsg = "The multiplier must be greater than one"; break;
                                        case PatternMachine.ValidationResult.INVALID_CHARACTER:
                                            _patternWarningMsg = "Invalid character"; break;
                                        default:
                                            if (tempBrush.patternMachine == null)
                                                tempBrush.patternMachine = new PatternMachine(tokens);
                                            else tempBrush.patternMachine.SetTokens(tokens);
                                            brush.patternMachine = tempBrush.patternMachine;
                                            break;
                                    }
                                    _previewPatternMachine = tempBrush.patternMachine;
                                }
                                if (_patternWarningMsg != null && _patternWarningMsg != string.Empty)
                                {
                                    var style = new GUIStyle();
                                    style.margin.top = 4;
                                    if (_warningTexture == null)
                                        _warningTexture = Resources.Load<Texture2D>("Sprites/Warning");
                                    GUILayout.Box(new GUIContent(_warningTexture, _patternWarningMsg), style,
                                        GUILayout.Width(14), GUILayout.Height(14));
                                }
                            }
                        }

                        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            tempBrush.restartPatternForEachStroke
                                = EditorGUILayout.ToggleLeft("Restart the pattern for each stroke",
                                tempBrush.restartPatternForEachStroke, GUILayout.Width(220));
                            if (!tempBrush.restartPatternForEachStroke)
                            {
                                if (GUILayout.Button("Restart Pattern"))
                                {
                                    brush.patternMachine.Reset();
                                    BrushstrokeManager.UpdateBrushstroke();
                                }
                            }
                        }
                    }
                    if (brushCheck.changed && !selectionFrecuencyChanged)
                    {
                        Undo.RegisterCompleteObjectUndo(this, MULTIBRUSH_SETTINGS_UNDO_MSG);
                        brush.Copy(tempBrush);
                        var selectedItem = GetSelectedItem(PaletteManager.selectedBrush);
                        selectedItem.Copy(tempItem);
                        brush.UpdateTotalFrequency();
                        BrushstrokeManager.UpdateBrushstroke(false);
                        PWBCore.SetSavePending();
                        SceneView.RepaintAll();
                    }
                }
            }
        }
        #endregion
        #endregion
    }
}
