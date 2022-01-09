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
using static PluginMaster.DropUtils;

namespace PluginMaster
{
    public class PrefabPalette : EditorWindow, ISerializationCallbackReceiver
    {
        #region COMMON
        private GUISkin _skin = null;

        [SerializeField] private PWBData _data = null;
        private bool _loadFromFile = false;
        private bool _undoRegistered = false;

        private static PrefabPalette _instance = null;
        public static PrefabPalette instance => _instance;
        [MenuItem("Tools/Plugin Master/Prefab World Builder/Palette", false, 1110)]
        public static void ShowWindow() => _instance = GetWindow<PrefabPalette>("Palette");
        private static bool _repaint = false;
        public static void RepainWindow()
        {
            if (_instance != null) _instance.Repaint();
            _repaint = true;

        }

        private void OnEnable()
        {
            _instance = this;
            _data = PWBCore.staticData;
            _skin = Resources.Load<GUISkin>("PWBSkin");
            if(_skin == null)
            {
                Close();
                return;
            }
            _toggleStyle = _skin.GetStyle("PaletteToggle");
            _loadingIcon = Resources.Load<Texture2D>("Sprites/Loading");
            _toggleStyle.margin = new RectOffset(4, 4, 4, 4);
            _dropdownIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/DropdownArrow"));
            _labelIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Label"), "Filter by label");
            _selectionFilterIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/SelectionFilter"),
                "Filter by selection");
            _newBrushIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/New"), "New Brush");
            _deleteBrushIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Delete"), "Delete Brush");
            _pickerIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Picker"), "Brush Picker");
            _clearFilterIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Clear"));
            _cursorStyle = _skin.GetStyle("Cursor");
            wantsMouseMove = true;
            wantsMouseEnterLeaveWindow = true;
            _visibleTabCount = PaletteManager.paletteNames.Length;
            autoRepaintOnSceneChange = true;
            UpdateLabelFilter();
            UpdateFilteredList(false);
            PaletteManager.ClearSelection(false);
            Undo.undoRedoPerformed += OnPaletteChange;
        }

        private void OnDisable() => Undo.undoRedoPerformed -= OnPaletteChange;

        private void OnDestroy() => ToolManager.OnPaletteClosed();

        private void OnGUI()
        {
            if (_skin == null)
            {
                Close();
                return;
            }
            if (_loadFromFile && Event.current.type == EventType.Repaint)
            {
                _loadFromFile = false;
                PWBCore.LoadFromFile();
                UpdateFilteredList(false);
                return;
            }
            if (_contextBrushAdded)
            {
                RegisterUndo("Add Brush");
                PaletteManager.selectedPalette.AddBrush(_newContextBrush);
                _newContextBrush = null;
                PaletteManager.selectedBrushIdx = PaletteManager.selectedPalette.brushes.Length - 1;
                _contextBrushAdded = false;
                OnPaletteChange();
                return;
            }
            TabBar();
            if (PaletteManager.paletteData.Length == 0) return;
            SearchBar();
            Palette();

            var eventType = Event.current.rawType;
            if (eventType == EventType.MouseMove || eventType == EventType.MouseUp)
            {
                _moveBrush.to = -1;
                _draggingBrush = false;
                _showCursor = false;
            }
            else if (Event.current.control && Event.current.type == EventType.KeyDown
                && Event.current.keyCode == KeyCode.Delete) OnDelete();
        }

        private void Update()
        {
            if (mouseOverWindow != this)
            {
                _moveBrush.to = -1;
                _showCursor = false;
            }
            else if (_draggingBrush) _showCursor = true;
            if(_repaint)
            {
                _repaint = false;
                Repaint();
            }
            if (_frameSelectedBrush && _newSelectedPositionSet) DoFrameSelectedBrush();
        }

        private void RegisterUndo(string name)
        {
            _undoRegistered = true;
            Undo.RegisterCompleteObjectUndo(this, name);
        }

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            _repaint = true;
            if (!_undoRegistered) _loadFromFile = true;
            PaletteManager.ClearSelection(false);
        }
        #endregion

        #region PALETTE
        private Vector2 _scrollPosition;
        private Rect _scrollViewRect;
        private Vector2 _prevSize;
        private int _columnCount = 1;
        private GUIStyle _toggleStyle = null;
        private const int MIN_ICON_SIZE = 24;
        private const int MAX_ICON_SIZE = 256;
        private const int DEFAULT_ICON_SIZE = 64;
        private int _iconSize = DEFAULT_ICON_SIZE;
        private int _prevIconSize = DEFAULT_ICON_SIZE;

        private GUIContent _dropdownIcon = null;
        private bool _draggingBrush = false;
        private bool _showCursor = false;
        private Rect _cursorRect;
        private GUIStyle _cursorStyle = null;
        private (int from, int to, bool perform) _moveBrush = (0, 0, false);

        private void Palette()
        {
            UpdateColumnCount();

            _prevIconSize = _iconSize;

            if (_moveBrush.perform)
            {
                RegisterUndo("Change Brush Order");
                var selection = PaletteManager.idxSelection;
                PaletteManager.selectedPalette.Swap(_moveBrush.from, _moveBrush.to, ref selection);
                PaletteManager.idxSelection = selection;
                if (selection.Length == 1) PaletteManager.selectedBrushIdx = selection[0];
                _moveBrush.perform = false;
                UpdateFilteredList(false);
            }
            BrushInputData toggleData = null;
            using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosition, false, false,
                GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, _skin.box))
            {
                _scrollPosition = scrollView.scrollPosition;
                Brushes(ref toggleData);
                if (_showCursor) GUI.Box(_cursorRect, string.Empty, _cursorStyle);
            }
            _scrollViewRect = GUILayoutUtility.GetLastRect();
            if (PaletteManager.selectedPalette.brushCount == 0) DropBox();
            Bottom();

            BrushMouseEventHandler(toggleData);
            PaletteContext();
            DropPrefab();
        }

        private void UpdateColumnCount()
        {
            if (PaletteManager.paletteCount == 0) return;
            var paletteData = PaletteManager.selectedPalette;
            var brushes = paletteData.brushes;
            if (_scrollViewRect.width > MIN_ICON_SIZE)
            {
                if (_prevSize != position.size || _prevIconSize != _iconSize)
                {
                    var iconW = (float)((_iconSize + 4) * brushes.Length + 6) / brushes.Length;
                    _columnCount = Mathf.Max((int)(_scrollViewRect.width / iconW), 1);
                    var rowCount = Mathf.CeilToInt((float)brushes.Length / _columnCount);
                    var h = rowCount * (_iconSize + 4) + 42;

                    if (h > _scrollViewRect.height)
                    {
                        iconW = (float)((_iconSize + 4) * brushes.Length + 17) / brushes.Length;
                        _columnCount = Mathf.Max((int)(_scrollViewRect.width / iconW), 1);
                    }
                }
                _prevSize = position.size;
            }
        }

        public void OnPaletteChange()
        {
            UpdateLabelFilter();
            UpdateFilteredList(false);
            Repaint();
        }
        #endregion

        #region BOTTOM
        private GUIContent _newBrushIcon = null;
        private GUIContent _deleteBrushIcon = null;
        private GUIContent _pickerIcon = null;
        private void Bottom()
        {
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.Height(18)))
            {
                if (PaletteManager.selectedPalette.brushCount > 0)
                {
                    var sliderStyle = new GUIStyle(GUI.skin.horizontalSlider);
                    sliderStyle.margin.top = 0;
                    _iconSize = (int)GUILayout.HorizontalSlider(
                        (float)_iconSize,
                        (float)MIN_ICON_SIZE,
                        (float)MAX_ICON_SIZE,
                        sliderStyle,
                        GUI.skin.horizontalSliderThumb,
                        GUILayout.MinWidth(Mathf.Max(position.width - 66, MIN_ICON_SIZE)),
                        GUILayout.MaxWidth(128));
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(_newBrushIcon, EditorStyles.toolbarButton)) PaletteContextMenu();
                using (new EditorGUI.DisabledGroupScope(PaletteManager.selectionCount == 0))
                {
                    if (GUILayout.Button(_deleteBrushIcon, EditorStyles.toolbarButton)) OnDelete();
                }
                PaletteManager.pickingBrushes = GUILayout.Toggle(PaletteManager.pickingBrushes,
                    _pickerIcon, EditorStyles.toolbarButton);
            }
            var rect = GUILayoutUtility.GetLastRect();
            if (rect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.DragUpdated
                    || Event.current.type == EventType.MouseDrag || Event.current.type == EventType.DragPerform)
                    Event.current.Use();
            }
        }

        private void OnDelete()
        {
            RegisterUndo("Delete Brush");
            DeleteBrushSelection();
            PaletteManager.ClearSelection();
            OnPaletteChange();
        }

        #endregion

        #region BRUSHES
        private Vector3 _selectedBrushPosition = Vector3.zero;
        private bool _frameSelectedBrush = false;
        private bool _newSelectedPositionSet = false;
        private Texture2D _loadingIcon = null;
        public void FrameSelectedBrush()
        {
            _frameSelectedBrush = true;
            _newSelectedPositionSet = false;
        }

        private void DoFrameSelectedBrush()
        {
            _frameSelectedBrush = false;
            if (_scrollPosition.y > _selectedBrushPosition.y
                || _scrollPosition.y + _scrollViewRect.height < _selectedBrushPosition.y)
                _scrollPosition.y = _selectedBrushPosition.y - 4;
            RepainWindow();
        }
        private void Brushes(ref BrushInputData toggleData)
        {
            if (Event.current.control && Event.current.keyCode == KeyCode.A && _filteredBrushList.Count > 0)
            {
                PaletteManager.ClearSelection();
                foreach (var brush in _filteredBrushList) PaletteManager.AddToSelection(brush.index);
                PaletteManager.selectedBrushIdx = _filteredBrushList[0].index;
                Repaint();
            }
            if (PaletteManager.selectedPalette.brushCount == 0) return;
            if (filteredBrushListCount == 0) return;
            var filteredBrushes = filteredBrushList.ToArray();
            int filterBrushIdx = 0;
            while (filterBrushIdx < filteredBrushes.Length)
            {
                using (new GUILayout.HorizontalScope())
                {
                    for (int col = 0; col < _columnCount && filterBrushIdx < filteredBrushes.Length; ++col)
                    {
                        var brush = filteredBrushes[filterBrushIdx].brush;
                        var brushIdx = filteredBrushes[filterBrushIdx].index;
                        var style = new GUIStyle(_toggleStyle);
                        if (PaletteManager.SelectionContains(brushIdx))
                            style.normal = _toggleStyle.onNormal;
                        var icon = brush.thumbnail;
                        if(icon == null) icon = _loadingIcon;
                        GUILayout.Box(new GUIContent(icon, brush.name), style, GUILayout.Width(_iconSize),
                            GUILayout.Height(_iconSize));

                        var rect = GUILayoutUtility.GetLastRect();
                        if (rect.Contains(Event.current.mousePosition))
                            toggleData = new BrushInputData(brushIdx, rect, Event.current.type,
                                Event.current.control, Event.current.shift, Event.current.mousePosition.x);
                        if (Event.current.type != EventType.Layout && PaletteManager.selectedBrushIdx == brushIdx)
                        {
                            _selectedBrushPosition = rect.position;
                            _newSelectedPositionSet = true;
                        }
                        ++filterBrushIdx;
                    }
                }
            }
        }

        public void DeselectAllButThis(int index)
        {
            if (PaletteManager.selectedBrushIdx == index && PaletteManager.selectionCount == 1) return;
            PaletteManager.ClearSelection();
            if (index < 0) return;
            PaletteManager.AddToSelection(index);
            PaletteManager.selectedBrushIdx = index;
        }

        private void DeleteBrushSelection()
        {
            var descendingSelection = PaletteManager.idxSelection;
            Array.Sort<int>(descendingSelection, new Comparison<int>((i1, i2) => i2.CompareTo(i1)));
            foreach (var i in descendingSelection) PaletteManager.selectedPalette.RemoveBrushAt(i);
        }
        private void DeleteBrush(object idx)
        {
            RegisterUndo("Delete Brush");
            if (PaletteManager.SelectionContains((int)idx)) DeleteBrushSelection();
            else PaletteManager.selectedPalette.RemoveBrushAt((int)idx);
            PaletteManager.ClearSelection();
            OnPaletteChange();
        }

        private void CopyBrushSettings(object idx)
            => PaletteManager.clipboardSetting = PaletteManager.selectedPalette.brushes[(int)idx].CloneMainSettings();

        private void PasteBrushSettings(object idx)
        {
            RegisterUndo("Paste Brush Settings");
            PaletteManager.selectedPalette.brushes[(int)idx].Copy(PaletteManager.clipboardSetting);
            if (BrushProperties.instance != null) BrushProperties.instance.Repaint();
            PWBCore.SetSavePending();
        }

        private void DuplicateBrush(object idx)
        {
            RegisterUndo("Duplicate Brush");
            if (PaletteManager.SelectionContains((int)idx))
            {
                var descendingSelection = PaletteManager.idxSelection;
                Array.Sort<int>(descendingSelection, new Comparison<int>((i1, i2) => i2.CompareTo(i1)));
                for (int i = 0; i < descendingSelection.Length; ++i)
                {
                    PaletteManager.selectedPalette.DuplicateBrush(descendingSelection[i]);
                    descendingSelection[i] += descendingSelection.Length - 1 - i;
                }
                PaletteManager.idxSelection = descendingSelection;
            }
            else PaletteManager.selectedPalette.DuplicateBrush((int)idx);
            OnPaletteChange();
        }

        private void MergeBrushes()
        {
            RegisterUndo("Merge Brushes");
            var selection = new List<int>(PaletteManager.idxSelection);
            selection.Sort();
            var resultIdx = selection[0];
            selection.RemoveAt(0);
            selection.Reverse();
            var result = PaletteManager.selectedPalette.GetBrush(resultIdx);
            for (int i = 0; i < selection.Count; ++i)
            {
                var idx = selection[i];
                var other = PaletteManager.selectedPalette.GetBrush(idx);
                var otherItems = other.items;
                foreach (var item in otherItems) result.AddItem(item);
                PaletteManager.selectedPalette.RemoveBrushAt(idx);
            }
            PaletteManager.ClearSelection();
            PaletteManager.AddToSelection(resultIdx);
            PaletteManager.selectedBrushIdx = resultIdx;
            OnPaletteChange();
        }

        private void SelectPrefabs(object idx)
        {
            var prefabs = new List<GameObject>();
            if (PaletteManager.SelectionContains((int)idx))
            {
                foreach (int selectedIdx in PaletteManager.idxSelection)
                {
                    var brush = PaletteManager.selectedPalette.GetBrush(selectedIdx);
                    foreach (var item in brush.items) prefabs.Add(item.prefab);
                }
            }
            else
            {
                var brush = PaletteManager.selectedPalette.GetBrush((int)idx);
                foreach (var item in brush.items) prefabs.Add(item.prefab);
            }
            Selection.objects = prefabs.ToArray();
        }

        private void OpenPrefab(object idx)
            => AssetDatabase.OpenAsset(PaletteManager.selectedPalette.GetBrush((int)idx).items[0].prefab);

        private void SelectReferences(object idx)
        {
            var items = PaletteManager.selectedPalette.GetBrush((int)idx).items;
            var itemsprefabIds = new List<int>();
            foreach (var item in items) itemsprefabIds.Add(item.prefab.GetInstanceID());
            var selection = new List<GameObject>();
            var objects = GameObject.FindObjectsOfType<Transform>();
            foreach (var obj in objects)
            {
                var source = PrefabUtility.GetCorrespondingObjectFromSource(obj);
                if (source == null) continue;
                var sourceIdx = source.gameObject.GetInstanceID();
                if (itemsprefabIds.Contains(sourceIdx)) selection.Add(obj.gameObject);
            }
            Selection.objects = selection.ToArray();
        }

        private void UpdateThumbnail(object idx)
        {
            var brush = PaletteManager.selectedPalette.GetBrush((int)idx);
            brush.UpdateThumbnail();
        }

        private void EditThumbnail(object idx)
        {
            var brushIdx = (int)idx;
            var brush = PaletteManager.selectedPalette.GetBrush(brushIdx);
            ThumbnailEditorWindow.ShowWindow(brush, brushIdx);
        }

        private void CopyThumbnailSettings(object idx)
        {
            var brush = PaletteManager.selectedPalette.brushes[(int)idx];
            PaletteManager.clipboardThumbnailSettings = brush.thumbnailSettings.Clone();
            PaletteManager.clipboardOverwriteThumbnailSettings = PaletteManager.Trit.SAME;
        }
        private void PasteThumbnailSettings(object idx)
        {
            if (PaletteManager.clipboardThumbnailSettings == null) return;
            RegisterUndo("Paste Thumbnail Settings");
            void Paste(MultibrushSettings brush)
            {
                brush.thumbnailSettings.Copy(PaletteManager.clipboardThumbnailSettings);
                ThumbnailUtils.UpdateThumbnail(brush);
            }
            if (PaletteManager.SelectionContains((int)idx))
            {
                foreach(var i in PaletteManager.idxSelection) Paste(PaletteManager.selectedPalette.brushes[i]);
            }
            else Paste(PaletteManager.selectedPalette.brushes[(int)idx]);
            PWBCore.SetSavePending();
        }

        private void BrushContext(int idx)
        {
            var menu = new GenericMenu();
            var brush = PaletteManager.selectedPalette.GetBrush(idx);
            menu.AddItem(new GUIContent("Select Prefab" + (PaletteManager.selectionCount > 1
                || brush.itemCount > 1 ? "s" : "")), false, SelectPrefabs, idx);
            if (brush.itemCount == 1) menu.AddItem(new GUIContent("Open Prefab"), false, OpenPrefab, idx);
            menu.AddItem(new GUIContent("Select References In Scene"), false, SelectReferences, idx);
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Update Thumbnail"), false, UpdateThumbnail, idx);
            menu.AddItem(new GUIContent("Edit Thumbnail"), false, EditThumbnail, idx);
            menu.AddItem(new GUIContent("Copy Thumbnail Settings"), false, CopyThumbnailSettings, idx);
            if (PaletteManager.clipboardThumbnailSettings != null)
                menu.AddItem(new GUIContent("Paste Thumbnail Settings"), false, PasteThumbnailSettings, idx);
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Delete"), false, DeleteBrush, idx);
            menu.AddItem(new GUIContent("Duplicate"), false, DuplicateBrush, idx);
            if (PaletteManager.selectionCount > 1) menu.AddItem(new GUIContent("Merge"), false, MergeBrushes);
            if (PaletteManager.selectionCount == 1)
                menu.AddItem(new GUIContent("Copy Brush Settings"), false, CopyBrushSettings, idx);
            if (PaletteManager.clipboardSetting != null)
                menu.AddItem(new GUIContent("Paste Brush Settings"), false, PasteBrushSettings, idx);
            menu.AddSeparator(string.Empty);
            PaletteContextAddMenuItems(menu);
            menu.ShowAsContext();
        }

        private void BrushMouseEventHandler(BrushInputData data)
        {
            void DeselectAllButCurrent()
            {
                PaletteManager.ClearSelection();
                PaletteManager.selectedBrushIdx = data.index;
                PaletteManager.AddToSelection(data.index);
            }
            if (data == null) return;
            if (data.eventType == EventType.MouseMove) Event.current.Use();
            if (data.eventType == EventType.MouseDown && Event.current.button == 0)
            {
                void DeselectAll() => PaletteManager.ClearSelection();
                void ToggleCurrent()
                {
                    if (PaletteManager.SelectionContains(data.index)) PaletteManager.RemoveFromSelection(data.index);
                    else PaletteManager.AddToSelection(data.index);
                    PaletteManager.selectedBrushIdx = PaletteManager.selectionCount == 1
                        ? PaletteManager.idxSelection[0] : -1;
                }
                if (data.shift)
                {
                    var selectedIdx = PaletteManager.selectedBrushIdx;
                    var sign = (int)Mathf.Sign(data.index - selectedIdx);
                    if (sign != 0)
                    {
                        PaletteManager.ClearSelection();
                        for (int i = selectedIdx; i != data.index; i += sign)
                        {
                            if (FilteredListContains(i)) PaletteManager.AddToSelection(i);
                        }
                        PaletteManager.AddToSelection(data.index);
                        PaletteManager.selectedBrushIdx = selectedIdx;
                    }
                    else DeselectAllButCurrent();
                }
                else
                {
                    if (data.control && PaletteManager.selectionCount < 2)
                    {
                        if (PaletteManager.selectedBrushIdx == data.index) DeselectAll();
                        else ToggleCurrent();
                    }
                    else if (data.control && PaletteManager.selectionCount > 1) ToggleCurrent();
                    else if (!data.control && PaletteManager.selectionCount < 2)
                    {
                        if (PaletteManager.selectedBrushIdx == data.index) DeselectAll();
                        else DeselectAllButCurrent();
                    }
                    else if (!data.control && PaletteManager.selectionCount > 1) DeselectAllButCurrent();
                }
                Event.current.Use();
                Repaint();
            }
            else if (data.eventType == EventType.ContextClick)
            {
                BrushContext(data.index);
                Event.current.Use();
            }
            else if (Event.current.type == EventType.MouseDrag)
            {
                if (!PaletteManager.SelectionContains(data.index)) DeselectAllButCurrent();
                DragAndDrop.PrepareStartDrag();
                PWBIO.sceneDragReceiver.brushId = data.index;
                SceneDragAndDrop.StartDrag(PWBIO.sceneDragReceiver, "Dragging brush");
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                _draggingBrush = true;
                _moveBrush.from = data.index;
                _moveBrush.perform = false;
                _moveBrush.to = -1;
            }
            else if (data.eventType == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                var size = new Vector2(4, _iconSize);
                var min = data.rect.min;
                bool toTheRight = data.mouseX - data.rect.center.x > 0;
                min.x = toTheRight ? data.rect.max.x : min.x - size.x;
                _cursorRect = new Rect(min, size);
                _showCursor = true;
                _moveBrush.to = data.index;
                if (toTheRight) ++_moveBrush.to;
            }
            else if (data.eventType == EventType.DragPerform)
            {
                _moveBrush.to = data.index;
                bool toTheRight = data.mouseX - data.rect.center.x > 0;
                if (toTheRight) ++_moveBrush.to;
                if (_draggingBrush)
                {
                    _moveBrush.perform = _moveBrush.from != _moveBrush.to;
                    _draggingBrush = false;
                }
                _showCursor = false;
            }
            else if (data.eventType == EventType.DragExited)
            {
                _showCursor = false;
                _draggingBrush = false;
                _moveBrush.to = -1;
            }
        }
        #endregion

        #region PALETTE CONTEXT
        private int _currentPickerId = -1;
        private bool _contextBrushAdded = false;
        private MultibrushSettings _newContextBrush = null;
        
        private void PaletteContext()
        {
            if (_scrollViewRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.ContextClick)
                {
                    PaletteContextMenu();
                    Event.current.Use();
                }
                else if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    PaletteManager.ClearSelection();
                    Repaint();
                }
            }

            if (Event.current.commandName == "ObjectSelectorClosed"
                && EditorGUIUtility.GetObjectPickerControlID() == _currentPickerId)
            {
                var obj = EditorGUIUtility.GetObjectPickerObject();
                if (obj != null)
                {
                    var prefabType = PrefabUtility.GetPrefabAssetType(obj);
                    if (prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant)
                    {
                        _contextBrushAdded = true;
                        var gameObj = obj as GameObject;
                        AddLabels(gameObj, GetPrefabFolder(gameObj));
                        _newContextBrush = new MultibrushSettings(gameObj);
                    }
                }
                _currentPickerId = -1;
            }
        }

        private void PaletteContextAddMenuItems(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("New Brush From Prefab"), false, CreateBrushFromPrefab);
            menu.AddItem(new GUIContent("New MultiBrush From Folder"), false, CreateBrushFromFolder);
            menu.AddItem(new GUIContent("New Brush From Each Prefab In Folder"), false,
                CreateBrushFromEachPrefabInFolder);
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("New MultiBrush From Selection"), false, CreateBrushFromSelection);
            menu.AddItem(new GUIContent("New Brush From Each Prefab Selected"), false,
                CreateBushFromEachPrefabSelected);
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Brush Creation And Drop Settings"), false,
                BrushCreationSettingsWindow.ShowWindow);
        }
        private void PaletteContextMenu()
        {
            var menu = new GenericMenu();
            PaletteContextAddMenuItems(menu);
            menu.ShowAsContext();
        }

        private void CreateBrushFromPrefab()
        {
            _currentPickerId = GUIUtility.GetControlID(FocusType.Passive) + 100;
            EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "t:Prefab", _currentPickerId);
        }

        private void CreateBrushFromFolder()
        {
            var items = GetFolderItems();
            if (items == null) return;
            RegisterUndo("Add Brush");
            var brush = new MultibrushSettings(items[0].obj);
            AddLabels(items[0].obj, items[0].subfolder);
            PaletteManager.selectedPalette.AddBrush(brush);
            DeselectAllButThis(PaletteManager.selectedPalette.brushes.Length - 1);
            for (int i = 1; i < items.Length; ++i)
            {
                var item = new MultibrushItemSettings(items[i].obj, brush);
                AddLabels(items[i].obj, items[i].subfolder);
                brush.AddItem(item);
            }
            OnPaletteChange();
        }

        private void CreateBrushFromEachPrefabInFolder()
        {
            var items = GetFolderItems();
            if (items == null) return;
            foreach (var item in items)
            {
                if (item.obj == null) continue;
                RegisterUndo("Add Brush");
                AddLabels(item.obj, item.subfolder);
                var brush = new MultibrushSettings(item.obj);
                PaletteManager.selectedPalette.AddBrush(brush);
            }
            DeselectAllButThis(PaletteManager.selectedPalette.brushes.Length - 1);
            OnPaletteChange();
        }

        private string GetPrefabFolder(GameObject obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            var folders = path.Split(new char[] { '\\', '/' });
            var subFolder = folders[folders.Length - 2];
            return subFolder;
        }

        public void CreateBrushFromSelection()
        {
            var selectionPrefabs = SelectionManager.GetSelectionPrefabs();
            if (selectionPrefabs.Length == 0) return;

            RegisterUndo("Add Brush");
            AddLabels(selectionPrefabs[0], GetPrefabFolder(selectionPrefabs[0]));
            var brush = new MultibrushSettings(selectionPrefabs[0]);
            PaletteManager.selectedPalette.AddBrush(brush);
            DeselectAllButThis(PaletteManager.selectedPalette.brushes.Length - 1);
            for (int i = 1; i < selectionPrefabs.Length; ++i)
            {
                AddLabels(selectionPrefabs[i], GetPrefabFolder(selectionPrefabs[i]));
                brush.AddItem(new MultibrushItemSettings(selectionPrefabs[i], brush));
            }
            OnPaletteChange();
        }

        public void CreateBushFromEachPrefabSelected()
        {
            var selectionPrefabs = SelectionManager.GetSelectionPrefabs();
            if (selectionPrefabs.Length == 0) return;
            foreach (var obj in selectionPrefabs)
            {
                if (obj == null) continue;
                RegisterUndo("Add Brush");
                var brush = new MultibrushSettings(obj);
                AddLabels(obj, GetPrefabFolder(obj));
                PaletteManager.selectedPalette.AddBrush(brush);
            }
            DeselectAllButThis(PaletteManager.selectedPalette.brushes.Length - 1);
            OnPaletteChange();
        }
        #endregion

        #region DROPBOX
        private void DropBox()
        {
            GUIStyle dragAndDropBoxStyle = new GUIStyle();
            dragAndDropBoxStyle.alignment = TextAnchor.MiddleCenter;
            dragAndDropBoxStyle.fontStyle = FontStyle.Italic;
            dragAndDropBoxStyle.fontSize = 12;
            dragAndDropBoxStyle.normal.textColor = Color.white;
            dragAndDropBoxStyle.wordWrap = true;
            GUI.Box(_scrollViewRect, "Drag and Drop Prefabs Or Folders Here", dragAndDropBoxStyle);
        }

        private void AddLabels(GameObject obj, string subfolder)
        {
            if (!PaletteManager.selectedPalette.brushCreationSettings.addLabelsToDroppedPrefabs
                && (!PaletteManager.selectedPalette.brushCreationSettings.createLablesForEachDroppedFolder
                || subfolder == null)) return;
            var labels = new HashSet<string>(AssetDatabase.GetLabels(obj));
            int labelCount = labels.Count;
            if (PaletteManager.selectedPalette.brushCreationSettings.addLabelsToDroppedPrefabs)
                labels.UnionWith(PaletteManager.selectedPalette.brushCreationSettings.labels);
            if (PaletteManager.selectedPalette.brushCreationSettings.createLablesForEachDroppedFolder
                && subfolder != null) labels.Add(subfolder);
            if (labelCount != labels.Count) AssetDatabase.SetLabels(obj, labels.ToArray());
        }

        private void DropPrefab()
        {
            if (_scrollViewRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Event.current.Use();
                }
                else if (Event.current.type == EventType.DragPerform)
                {
                    bool paletteChanged = false;
                    var items = GetDroppedPrefabs();
                    if(items.Length > 0) PaletteManager.ClearSelection();
                    foreach (var item in items)
                    {
                        AddLabels(item.obj, item.subfolder);
                        var brush = new MultibrushSettings(item.obj);
                        RegisterUndo("Add Brush");
                        if (_moveBrush.to < 0)
                        {
                            PaletteManager.selectedPalette.AddBrush(brush);
                            PaletteManager.selectedBrushIdx = PaletteManager.selectedPalette.brushes.Length - 1;
                        }
                        else
                        {
                            PaletteManager.selectedPalette.InsertBrushAt(brush, _moveBrush.to);
                            PaletteManager.selectedBrushIdx = _moveBrush.to;
                        }
                        paletteChanged = true;
                    }
                    if (paletteChanged) OnPaletteChange();
                    if (_draggingBrush && _moveBrush.to >= 0)
                    {
                        _moveBrush.perform = _moveBrush.from != _moveBrush.to;
                        _draggingBrush = false;
                    }
                    _showCursor = false;
                    Event.current.Use();
                }
                else if (Event.current.type == EventType.DragExited) _showCursor = false;
            }
        }
        #endregion

        #region TAB BAR
        #region RENAME
        private class RenamePaletteWindow : EditorWindow
        {
            private string _currentName = string.Empty;
            private int _paletteIdx = -1;
            private Action<string, int> _onDone;
            public static void ShowWindow(RenameData data, Action<string, int> onDone)
            {
                var window = GetWindow<RenamePaletteWindow>(true, "Rename Palette");
                window._currentName = data.currentName;
                window._paletteIdx = data.paletteIdx;
                window._onDone = onDone;
                window.position = new Rect(data.mousePosition.x + 50, data.mousePosition.y + 50, 160, 50);
            }

            private void OnGUI()
            {
                EditorGUIUtility.labelWidth = 70;
                EditorGUIUtility.fieldWidth = 70;
                using (new GUILayout.HorizontalScope())
                {
                    _currentName = EditorGUILayout.TextField("New Name:", _currentName);
                }
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Apply", GUILayout.Width(50)))
                    {
                        _onDone(_currentName, _paletteIdx);
                        Close();
                    }
                }
            }
        }

        private struct RenameData
        {
            public readonly int paletteIdx;
            public readonly string currentName;
            public readonly Vector2 mousePosition;

            public RenameData(int paletteIdx, string currentName, Vector2 mousePosition)
                => (this.paletteIdx, this.currentName, this.mousePosition) = (paletteIdx, currentName, mousePosition);
        }
        private void ShowRenamePaletteWindow(object obj)
        {
            var data = (RenameData)obj;
            RenamePaletteWindow.ShowWindow(data, RenamePalette);
        }

        private void RenamePalette(string paletteName, int paletteIdx)
        {
            RegisterUndo("Rename Palette");
            PaletteManager.paletteData[paletteIdx].name = paletteName;
            _updateTabBarWidth = true;
            Repaint();
        }
        #endregion


        private void ShowDeleteConfirmation(object obj)
        {
            if (EditorUtility.DisplayDialog("Delete Palette", "Are you sure you want to delete this palette?",
                "Delete", "Cancel"))
            {
                int paletteIdx = (int)obj;
                RegisterUndo("Remove Palette");
                PaletteManager.RemovePaletteAt(paletteIdx);
                if (PaletteManager.paletteCount == 0) CreatePalette();
                else if (paletteIdx == PaletteManager.selectedPaletteIdx)
                {
                    SelectPalette(PaletteManager.selectedPaletteIdx == 0 ? 1 : PaletteManager.selectedPaletteIdx - 1);
                }
                --_visibleTabCount;
                if (_lastVisibleIdx >= _visibleTabCount)
                {
                    _lastVisibleIdx = _visibleTabCount - 1;
                }
                PaletteManager.selectedBrushIdx = -1;
                _updateTabBarWidth = true;
                _updateTabBar = true;
                UpdateFilteredList(false);
                Repaint();
            }
        }

        #region TAB BUTTONS
        private float _prevWidth = 0f;
        private bool _updateTabBarWidth = true;
        private bool _updateTabBar = false;
        private int _lastVisibleIdx = 0;
        private int _visibleTabCount = 0;
        private Rect _dropdownRect;

        private void SelectPalette(int idx)
        {
            if (PaletteManager.selectedPaletteIdx == idx) return;
            PaletteManager.selectedPaletteIdx = idx;
            PaletteManager.selectedBrushIdx = -1;
            PaletteManager.ClearSelection();
            OnPaletteChange();
        }
        private void SelectPalette(object obj) => SelectPalette((int)obj);

        private void CreatePalette()
        {
            _lastVisibleIdx = PaletteManager.paletteCount;
            RegisterUndo("Add Palette");
            PaletteManager.AddPalette(new PaletteData("Palette" + (PaletteManager.paletteCount + 1),
                DateTime.Now.ToBinary()));
            SelectPalette(_lastVisibleIdx);
            _updateTabBarWidth = true;
            _updateTabBar = true;
        }

        private void TabBar()
        {
            float visibleW = 0;
            int lastVisibleIdx = 0;
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button(_dropdownIcon, EditorStyles.toolbarButton))
                {
                    var menu = new GenericMenu();
                    for (int i = 0; i < PaletteManager.paletteNames.Length; ++i)
                    {
                        var name = (i + 1).ToString() + ". " + PaletteManager.paletteNames[i];
                        menu.AddItem(new GUIContent(name), PaletteManager.selectedPaletteIdx == i, SelectPalette, i);
                    }
                    menu.AddSeparator(string.Empty);
                    menu.AddItem(new GUIContent("New Palette"), false, CreatePalette);
                    menu.AddSeparator(string.Empty);
                    menu.AddItem(new GUIContent("Brush Creation Settings"), false,
                        BrushCreationSettingsWindow.ShowWindow);
                    menu.ShowAsContext();
                }
                if (PaletteManager.paletteCount == 0) return;
                for (int i = 0; i <= _lastVisibleIdx; ++i)
                {
                    var isSelected = PaletteManager.selectedPaletteIdx == i;
                    var name = PaletteManager.paletteNames[i];

                    if (GUILayout.Toggle(isSelected, name, EditorStyles.toolbarButton) && Event.current.button == 0)
                    {
                        if (!isSelected)
                        {
                            SelectPalette(i);
                        }
                        isSelected = true;
                    }

                    var toggleRect = GUILayoutUtility.GetLastRect();
                    if (Event.current.type == EventType.Repaint && toggleRect.xMax < position.width)
                    {
                        lastVisibleIdx = i;
                        visibleW = toggleRect.xMax;
                    }

                    if (toggleRect.Contains(Event.current.mousePosition) && Event.current.button == 1)
                    {
                        var menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Rename"), false, ShowRenamePaletteWindow,
                            new RenameData(i, name, position.position + Event.current.mousePosition));
                        menu.AddItem(new GUIContent("Delete"), false, ShowDeleteConfirmation, i);
                        menu.ShowAsContext();
                    }
                }
                GUILayout.FlexibleSpace();

                if (Event.current.type == EventType.Repaint)
                {
                    if (_updateTabBarWidth && _visibleTabCount == PaletteManager.paletteCount)
                    {
                        _updateTabBarWidth = false;
                        _lastVisibleIdx = lastVisibleIdx;
                        _updateTabBar = true;
                    }
                    else if (_updateTabBarWidth && _visibleTabCount != PaletteManager.paletteCount)
                    {
                        _lastVisibleIdx = PaletteManager.paletteCount - 1;
                        _updateTabBar = true;
                    }
                    if (_prevWidth != position.width)
                    {
                        if (_prevWidth < position.width) _updateTabBarWidth = true;

                        _lastVisibleIdx = lastVisibleIdx;
                        _prevWidth = position.width;
                        _updateTabBar = true;
                    }
                }
            }

            if (_updateTabBar && PaletteManager.paletteCount > 1)
            {
                if (PaletteManager.selectedPaletteIdx > _lastVisibleIdx)
                {
                    PaletteManager.SwapPalette(PaletteManager.selectedPaletteIdx, _lastVisibleIdx);
                    PaletteManager.selectedPaletteIdx = _lastVisibleIdx;
                }
                _visibleTabCount = _lastVisibleIdx + 1;
                _updateTabBar = false;
                Repaint();
            }
        }
        #endregion
        #endregion

        #region SEARCH BAR
        private string _filterText = string.Empty;
        private GUIContent _labelIcon = null;
        private GUIContent _selectionFilterIcon = null;
        private GUIContent _clearFilterIcon = null;

        private struct FilteredBrush
        {
            public readonly MultibrushSettings brush;
            public readonly int index;
            public FilteredBrush(MultibrushSettings brush, int index) => (this.brush, this.index) = (brush, index);
        }
        private List<FilteredBrush> _filteredBrushList = new List<FilteredBrush>();
        private List<FilteredBrush> filteredBrushList
        {
            get
            {
                if (_filteredBrushList == null) _filteredBrushList = new List<FilteredBrush>();
                return _filteredBrushList;
            }
        }
        public bool FilteredBrushListContains(int index) => _filteredBrushList.Exists(brush => brush.index == index);
        private Dictionary<string, bool> _labelFilter = new Dictionary<string, bool>();
        public Dictionary<string, bool> labelFilter
        {
            get
            {
                if (_labelFilter == null) _labelFilter = new Dictionary<string, bool>();
                return _labelFilter;
            }
            set => _labelFilter = value;
        }

        private bool _updateLabelFilter = true;
        public int filteredBrushListCount => filteredBrushList.Count;

        public string filterText
        {
            get
            {
                if (_filterText == null) _filterText = string.Empty;
                return _filterText;
            }
            set => _filterText = value;
        }

        private void ClearLabelFilter()
        {
            foreach (var key in labelFilter.Keys.ToArray()) labelFilter[key] = false;
        }

        private void SearchBar()
        {
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.FlexibleSpace();

                using (var check = new EditorGUI.ChangeCheckScope())
                {
#if UNITY_2019_1_OR_NEWER
                    var searchFieldStyle = EditorStyles.toolbarSearchField;
#else
                    var searchFieldStyle = EditorStyles.toolbarTextField;
#endif
                    GUILayout.Space(2);
                    filterText = EditorGUILayout.TextField(filterText, searchFieldStyle).Trim();
                    if (check.changed) UpdateFilteredList(true);
                }
                if (filterText != string.Empty)
                {
                    if (GUILayout.Button(_clearFilterIcon, EditorStyles.toolbarButton))
                    {
                        filterText = string.Empty;
                        ClearLabelFilter();
                        UpdateFilteredList(true);
                        GUI.FocusControl(null);
                    }
                }

                if (GUILayout.Button(_labelIcon, EditorStyles.toolbarButton))
                {
                    GUI.FocusControl(null);
                    UpdateLabelFilter();
                    var menu = new GenericMenu();
                    if (labelFilter.Count == 0)
                        menu.AddItem(new GUIContent("No labels Found"), false, null);
                    else
                        foreach (var labelItem in labelFilter.OrderBy(item => item.Key))
                            menu.AddItem(new GUIContent(labelItem.Key), labelItem.Value,
                                SelectLabelFilter, labelItem.Key);
                    menu.ShowAsContext();
                }

                if (GUILayout.Button(_selectionFilterIcon, EditorStyles.toolbarButton))
                {
                    GUI.FocusControl(null);
                    FilterBySelection();
                }
            }
            if (_updateLabelFilter)
            {
                _updateLabelFilter = false;
                UpdateLabelFilter();
            }
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                GUI.FocusControl(null);
                Repaint();
            }
        }

        private bool FilteredListContains(int index)
        {
            foreach (var filtered in filteredBrushList)
            {
                if (filtered.index == index) return true;
            }
            return false;
        }

        private void UpdateFilteredList(bool textCanged)
        {
            filteredBrushList.Clear();
            void RemoveFromSelection(int index)
            {
                PaletteManager.RemoveFromSelection(index);
                if (PaletteManager.selectedBrushIdx == index) PaletteManager.selectedBrushIdx = -1;
                if (PaletteManager.selectionCount == 1)
                    PaletteManager.selectedBrushIdx = PaletteManager.idxSelection[0];
            }

            //filter by label
            var filterTextArray = filterText.Split(',');
            var filterTextList = new List<string>();
            ClearLabelFilter();
            for (int i = 0; i < filterTextArray.Length; ++i)
            {
                var filterText = filterTextArray[i].Trim();
                if (filterText.Length >= 2 && filterText.Substring(0, 2) == "l:")
                {
                    filterText = filterText.Substring(2);
                    if (labelFilter.ContainsKey(filterText)) labelFilter[filterText] = true;
                    else return;
                    continue;
                }
                filterTextList.Add(filterText);
            }
            var tempFilteredBrushList = new List<FilteredBrush>();
            if (!labelFilter.ContainsValue(true))
                for (int i = 0; i < PaletteManager.selectedPalette.brushes.Length; ++i)
                    tempFilteredBrushList.Add(new FilteredBrush(PaletteManager.selectedPalette.brushes[i], i));
            else
            {
                for (int i = 0; i < PaletteManager.selectedPalette.brushes.Length; ++i)
                {
                    var brush = PaletteManager.selectedPalette.brushes[i];
                    bool itemContainsFilter = false;
                    foreach (var item in brush.items)
                    {
                        var labels = AssetDatabase.GetLabels(item.prefab);
                        foreach (var label in labels)
                        {
                            if (labelFilter[label])
                            {
                                itemContainsFilter = true;
                                break;
                            }
                        }
                        if (itemContainsFilter) break;
                    }
                    if (itemContainsFilter) tempFilteredBrushList.Add(new FilteredBrush(brush, i));
                    else RemoveFromSelection(i);
                }
            }
            //filter by name
            var listIsEmpty = filterTextList.Count == 0;
            if (!listIsEmpty)
            {
                listIsEmpty = true;
                foreach (var filter in filterTextList)
                {
                    if (filter != String.Empty)
                    {
                        listIsEmpty = false;
                        break;
                    }
                }
            }
            if (listIsEmpty)
            {
                filteredBrushList.AddRange(tempFilteredBrushList);
                return;
            }

            foreach (var filteredItem in tempFilteredBrushList.ToArray())
            {
                for (int i = 0; i < filterTextList.Count; ++i)
                {
                    var filterText = filterTextList[i].Trim();
                    bool wholeWordOnly = false;
                    if (filterText == string.Empty) continue;
                    if (filterText.Length >= 2 && filterText.Substring(0, 2) == "w:")
                    {
                        wholeWordOnly = true;
                        filterText = filterText.Substring(2);
                    }
                    if (filterText == string.Empty) continue;
                    filterText = filterText.ToLower();
                    var brush = filteredItem.brush;
                    if ((!wholeWordOnly && brush.name.ToLower().Contains(filterText))
                        || (wholeWordOnly && brush.name.ToLower() == filterText))
                    {
                        filteredBrushList.Add(filteredItem);
                        break;
                    }
                    bool itemContainsFilter = false;
                    foreach (var item in brush.items)
                    {
                        if ((!wholeWordOnly && item.prefab.name.ToLower().Contains(filterText))
                            || (wholeWordOnly && item.prefab.name.ToLower() == filterText))
                        {
                            itemContainsFilter = true;
                            break;
                        }
                    }
                    if (itemContainsFilter)
                    {
                        filteredBrushList.Add(filteredItem);
                        break;
                    }
                    else RemoveFromSelection(filteredItem.index);
                }
            }
        }

        private void UpdateLabelFilter()
        {
            foreach (var brush in PaletteManager.selectedPalette.brushes)
            {
                foreach (var item in brush.items)
                {
                    var labels = AssetDatabase.GetLabels(item.prefab);
                    foreach (var label in labels)
                    {
                        if (labelFilter.ContainsKey(label)) continue;
                        labelFilter.Add(label, false);
                    }
                }
            }
        }

        private void SelectLabelFilter(object key)
        {
            labelFilter[(string)key] = !labelFilter[(string)key];
            foreach (var pair in labelFilter)
            {
                if (!pair.Value) continue;
                var labelFilter = "l:" + pair.Key;
                if (filterText.Contains(labelFilter)) continue;
                if (filterText.Length > 0) filterText += ", ";
                filterText += labelFilter;
            }
            var filterTextArray = filterText.Split(',');
            filterText = string.Empty;
            for (int i = 0; i < filterTextArray.Length; ++i)
            {
                var filter = filterTextArray[i].Trim();
                if (filter.Length >= 2 && filter.Substring(0, 2) == "l:")
                {
                    var label = filter.Substring(2);
                    if (!labelFilter.ContainsKey(label)) continue;
                    if (!labelFilter[label]) continue;
                    if (filterText.Contains(filter)) continue;
                }
                if (filter == string.Empty) continue;
                filterText += filter + ", ";
            }
            if (filterText != string.Empty) filterText = filterText.Substring(0, filterText.Length - 2);
            UpdateFilteredList(false);
            Repaint();
        }

        public int FilterBySelection()
        {
            var selection = SelectionManager.GetSelectionPrefabs();
            filterText = string.Empty;
            for (int i = 0; i < selection.Length; ++i)
            {
                filterText += "w:" + selection[i].name;
                if (i < selection.Length - 1) filterText += ", ";
            }
            UpdateFilteredList(false);
            return filteredBrushListCount;
        }

        public void SelectFirstBrush()
        {
            if (filteredBrushListCount == 0) return;
            DeselectAllButThis(filteredBrushList[0].index);
        }
        #endregion
    }
}
