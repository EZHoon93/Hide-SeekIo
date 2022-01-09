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
#if UNITY_2021_2_OR_NEWER
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.ShortcutManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace PluginMaster
{
    #region TOGGLE MANAGER
    public static class ToggleManager
    {
        private static Dictionary<ToolManager.PaintTool, IPWBToogle> _toggles = null;
        private static Dictionary<ToolManager.PaintTool, IPWBToogle> toggles
        {
            get
            {
                if (_toggles == null)
                {
                    _toggles = new Dictionary<ToolManager.PaintTool, IPWBToogle>()
                    {
                        {ToolManager.PaintTool.PIN,  PinToggle.instance },
                        {ToolManager.PaintTool.BRUSH, BrushToggle.instance},
                        {ToolManager.PaintTool.GRAVITY, GravityToggle.instance},
                        {ToolManager.PaintTool.LINE, LineToggle.instance},
                        {ToolManager.PaintTool.SHAPE, ShapeToggle.instance},
                        {ToolManager.PaintTool.TILING, TilingToggle.instance},
                        {ToolManager.PaintTool.REPLACER, ReplacerToggle.instance},
                        {ToolManager.PaintTool.ERASER, EraserToggle.instance},
                        {ToolManager.PaintTool.SELECTION, SelectionToggle.instance},
                        {ToolManager.PaintTool.EXTRUDE, ExtrudeToggle.instance},
                        {ToolManager.PaintTool.MIRROR, MirrorToggle.instance}
                    };
                }
                return _toggles;
            }
        }

        public static void DeselectOthers(string id)
        {
            foreach (var toggle in toggles.Values)
            {
                if (toggle == null) continue;
                if (id != toggle.id && toggle.value) toggle.value = false;
            }
        }

        public static string GetTooltip(string tooltip, string shortcutId)
        => tooltip + " ... " + ShortcutManager.instance.GetShortcutBinding(shortcutId).ToString();

        public static string iconPath => EditorGUIUtility.isProSkin ? "Sprites/" : "Sprites/LightTheme/";
    }
    #endregion
    #region TOGGLE BASE
    interface IPWBToogle
    {
        public string id { get; }
        public ToolManager.PaintTool tool { get; }
        public bool value { get; set; }
    }
    public abstract class ToolToggleBase<T> : EditorToolbarToggle, IPWBToogle where T : EditorToolbarToggle, new()
    {
        private static ToolToggleBase<T> _instance = null;
        public static ToolToggleBase<T> instance => _instance;
        public abstract string id { get; }
        public abstract ToolManager.PaintTool tool { get; }
        public ToolToggleBase()
        {
            _instance = this;
            this.RegisterValueChangedCallback(OnValueChange);
            ToolManager.OnToolChange += OnToolChange;
        }

        private void OnToolChange(ToolManager.PaintTool prevTool)
        {
            if (tool == prevTool && tool != ToolManager.tool && value) value = false;
            if (tool == ToolManager.tool && !value) value = true;
        }

        private void OnValueChange(ChangeEvent<bool> evt)
        {
            if (evt.newValue)
            {
                ToolManager.tool = tool;
                ToggleManager.DeselectOthers(id);
            }
            else if (tool == ToolManager.tool) ToolManager.DeselectTool();
        }
    }
    #endregion
    #region PROP PLACEMENT TOOLS
    [EditorToolbarElement(ID, typeof(SceneView))]
    public class PinToggle : ToolToggleBase<PinToggle>
    {
        public const string ID = "PWB/PinToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.PIN;
        public PinToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Pin");
            tooltip = ToggleManager.GetTooltip("Pin", Shortcuts.PWB_TOGGLE_PIN_SHORTCUT_ID);
        }
    }

    [EditorToolbarElement(ID, typeof(SceneView))]
    public class BrushToggle : ToolToggleBase<BrushToggle>
    {
        public const string ID = "PWB/BrushToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.BRUSH;
        public BrushToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Brush");
            tooltip = ToggleManager.GetTooltip("Brush", Shortcuts.PWB_TOGGLE_BRUSH_SHORTCUT_ID);
        }
    }

    [EditorToolbarElement(ID, typeof(SceneView))]
    public class GravityToggle : ToolToggleBase<GravityToggle>
    {
        public const string ID = "PWB/GravityToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.GRAVITY;
        public GravityToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "GravityTool");
            tooltip = ToggleManager.GetTooltip("Gravity Brush", Shortcuts.PWB_TOGGLE_GRAVITY_SHORTCUT_ID);
        }
    }

    [EditorToolbarElement(ID, typeof(SceneView))]
    public class LineToggle : ToolToggleBase<LineToggle>
    {
        public const string ID = "PWB/LineToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.LINE;
        public LineToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Line");
            tooltip = ToggleManager.GetTooltip("Line", Shortcuts.PWB_TOGGLE_LINE_SHORTCUT_ID);
        }
    }

    [EditorToolbarElement(ID, typeof(SceneView))]
    public class ShapeToggle : ToolToggleBase<ShapeToggle>
    {
        public const string ID = "PWB/ShapeToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.SHAPE;
        public ShapeToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Shape");
            tooltip = ToggleManager.GetTooltip("Shape", Shortcuts.PWB_TOGGLE_SHAPE_SHORTCUT_ID);
        }
    }

    [EditorToolbarElement(ID, typeof(SceneView))]
    public class TilingToggle : ToolToggleBase<TilingToggle>
    {
        public const string ID = "PWB/TilingToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.TILING;
        public TilingToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Tiling");
            tooltip = ToggleManager.GetTooltip("Tiling", Shortcuts.PWB_TOGGLE_TILING_SHORTCUT_ID);
        }
    }

    [EditorToolbarElement(ID, typeof(SceneView))]
    public class ReplacerToggle : ToolToggleBase<ReplacerToggle>
    {
        public const string ID = "PWB/ReplacerToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.REPLACER;
        public ReplacerToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Replace");
            tooltip = ToggleManager.GetTooltip("Replacer", Shortcuts.PWB_TOGGLE_REPLACER_SHORTCUT_ID);
        }
    }

    [EditorToolbarElement(ID, typeof(SceneView))]
    public class EraserToggle : ToolToggleBase<EraserToggle>
    {
        public const string ID = "PWB/EraserToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.ERASER;
        public EraserToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Eraser");
            tooltip = ToggleManager.GetTooltip("Eraser", Shortcuts.PWB_TOGGLE_ERASER_SHORTCUT_ID);
        }
    }

    [EditorToolbarElement(ID, typeof(SceneView))]
    public class HelpButton : EditorToolbarButton
    {
        public const string ID = "PWB/HelpButton";
        private UnityEngine.Object _documentationPdf = null;
        public HelpButton()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Help");
            tooltip = "Documentation";
            clicked += OpenDocumentation;
        }

        private void OpenDocumentation()
        {
            if (_documentationPdf == null)
                _documentationPdf = AssetDatabase.LoadMainAssetAtPath(PWBCore.staticData.documentationPath);
            if (_documentationPdf == null) Debug.LogWarning("Missing Documentation File");
            else AssetDatabase.OpenAsset(_documentationPdf);
        }
    }
    [Icon("Assets/PluginMaster/DesignTools/PrefabWorldBuilder/Editor/Resources/Sprites/Brush.png")]
    [Overlay(typeof(SceneView), "PWB Prop Placement Tools", true)]
    public class PWBPropPlacementToolbarOverlay : ToolbarOverlay
    {
        PWBPropPlacementToolbarOverlay() : base(PinToggle.ID, BrushToggle.ID, GravityToggle.ID, LineToggle.ID,
            ShapeToggle.ID, TilingToggle.ID, ReplacerToggle.ID, EraserToggle.ID, HelpButton.ID)
        {}
    }
    #endregion
    #region SELECTION TOOLS
    [EditorToolbarElement(ID, typeof(SceneView))]
    public class SelectionToggle : ToolToggleBase<SelectionToggle>
    {
        public const string ID = "PWB/SelectionToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.SELECTION;
        public SelectionToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Selection");
            tooltip = ToggleManager.GetTooltip("Selection", Shortcuts.PWB_TOGGLE_SELECTION_SHORTCUT_ID);
        }
    }

    [EditorToolbarElement(ID, typeof(SceneView))]
    public class ExtrudeToggle : ToolToggleBase<ExtrudeToggle>
    {
        public const string ID = "PWB/ExtrudeToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.EXTRUDE;
        public ExtrudeToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Extrude");
            tooltip = ToggleManager.GetTooltip("Extrude", Shortcuts.PWB_TOGGLE_EXTRUDE_SHORTCUT_ID);
        }
    }

    [EditorToolbarElement(ID, typeof(SceneView))]
    public class MirrorToggle : ToolToggleBase<MirrorToggle>
    {
        public const string ID = "PWB/MirrorToggle";
        public override string id => ID;
        public override ToolManager.PaintTool tool => ToolManager.PaintTool.MIRROR;
        public MirrorToggle() : base()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Mirror");
            tooltip = ToggleManager.GetTooltip("Mirror", Shortcuts.PWB_TOGGLE_MIRROR_SHORTCUT_ID);
        }
    }

    [Icon("Assets/PluginMaster/DesignTools/PrefabWorldBuilder/Editor/Resources/Sprites/Selection.png")]
    [Overlay(typeof(SceneView), "PWB Selection Tools", true)]
    public class PWBSelectionToolbarOverlay : ToolbarOverlay
    {
        PWBSelectionToolbarOverlay() : base(SelectionToggle.ID, ExtrudeToggle.ID, MirrorToggle.ID) { }
    }
    #endregion
    #region GRID TOOLS
    [EditorToolbarElement(ID, typeof(SceneView))]
    public class GridTypeToggle : EditorToolbarButton
    {
        public const string ID = "PWB/GridTypeToggle";
        private Texture2D _radialGridIcon = null;
        private Texture2D _rectGridIcon = null;
        public GridTypeToggle() : base()
        {
            UpdateIcon();
            clicked += OnClick;
            SnapManager.settings.OnDataChanged += UpdateIcon;
        }

        private void UpdateIcon()
        {
            if (_radialGridIcon == null) _radialGridIcon = Resources.Load<Texture2D>(ToggleManager.iconPath + "RadialGrid");
            if (_rectGridIcon == null) _rectGridIcon = Resources.Load<Texture2D>(ToggleManager.iconPath + "Grid");
            icon = SnapManager.settings.radialGridEnabled ? _rectGridIcon : _radialGridIcon;
            tooltip = SnapManager.settings.radialGridEnabled ? "Grid" : "Radial Grid";
        }

        private void OnClick()
        {
            SnapManager.settings.radialGridEnabled = !SnapManager.settings.radialGridEnabled;
            UpdateIcon();
            SnapSettingsWindow.RepaintWindow();
        }
    }

    [EditorToolbarElement(ID, typeof(SceneView))]
    public class SnapToggle : EditorToolbarDropdownToggle, IAccessContainerWindow
    {
        public const string ID = "PWB/SnapToggle";
        public EditorWindow containerWindow { get; set; }
        public SnapToggle()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "SnapOn");
            tooltip = "Enable snapping";
            dropdownClicked += ShowSnapWindow;
            this.RegisterValueChangedCallback(OnValueChange);
            SnapManager.settings.OnDataChanged += () => value = SnapManager.settings.snappingEnabled;
        }

        private void OnValueChange(ChangeEvent<bool> evt)
        {
            SnapManager.settings.snappingEnabled = evt.newValue;
            SnapSettingsWindow.RepaintWindow();
        }

        private void ShowSnapWindow()
        {
            var settings = SnapManager.settings;
            var menu = new GenericMenu();
            if (settings.radialGridEnabled)
            {
                menu.AddItem(new GUIContent("Snap To Radius"), settings.snapToRadius,
                    () => settings.snapToRadius = !settings.snapToRadius);
                menu.AddItem(new GUIContent("Snap To Circunference"), settings.snapToCircunference,
                    () => settings.snapToCircunference = !settings.snapToCircunference);
            }
            else
            {
                menu.AddItem(new GUIContent("X"), settings.snappingOnX, () => settings.snappingOnX = !settings.snappingOnX);
                menu.AddItem(new GUIContent("Y"), settings.snappingOnY, () => settings.snappingOnY = !settings.snappingOnY);
                menu.AddItem(new GUIContent("Z"), settings.snappingOnZ, () => settings.snappingOnZ = !settings.snappingOnZ);
            }
            menu.ShowAsContext();
            SnapSettingsWindow.RepaintWindow();
        }
    }

    [EditorToolbarElement(ID, typeof(SceneView))]
    public class GridToggle : EditorToolbarDropdownToggle, IAccessContainerWindow
    {
        public const string ID = "PWB/GridToggle";
        public EditorWindow containerWindow { get; set; }
        public GridToggle()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "ShowGrid");
            tooltip = "Show grid";
            dropdownClicked += ShowGridWindow;
            this.RegisterValueChangedCallback(OnValueChange);
            SnapManager.settings.OnDataChanged += () => value = SnapManager.settings.visibleGrid;
        }

        private void OnValueChange(ChangeEvent<bool> evt) => SnapManager.settings.visibleGrid = evt.newValue;

        private void ShowGridWindow()
        {
            var settings = SnapManager.settings;
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("X"), settings.gridOnX, () => settings.gridOnX = !settings.gridOnX);
            menu.AddItem(new GUIContent("Y"), settings.gridOnY, () => settings.gridOnY = !settings.gridOnY);
            menu.AddItem(new GUIContent("Z"), settings.gridOnZ, () => settings.gridOnZ = !settings.gridOnZ);
            menu.ShowAsContext();
        }
    }

    [EditorToolbarElement(ID, typeof(SceneView))]
    public class LockGridToggle : EditorToolbarToggle
    {
        public const string ID = "PWB/LockGridToggle";
        public LockGridToggle()
        {
            UpdteIcon();
            this.RegisterValueChangedCallback(OnValueChange);
            SnapManager.settings.OnDataChanged += () => value = SnapManager.settings.lockedGrid;
        }

        private void UpdteIcon()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath
            + (SnapManager.settings.lockedGrid ? "LockGrid" : "UnlockGrid"));
            tooltip = SnapManager.settings.lockedGrid ? "Lock the grid origin in place" : "Unlock the grid origin";
        }

        private void OnValueChange(ChangeEvent<bool> evt)
        {
            SnapManager.settings.lockedGrid = evt.newValue;
            UpdteIcon();
        }
    }

    [EditorToolbarElement(ID, typeof(SceneView))]
    public class GridSettingsButton : EditorToolbarButton
    {
        public const string ID = "PWB/GridSettingsButton";
        public GridSettingsButton()
        {
            icon = Resources.Load<Texture2D>(ToggleManager.iconPath + "SnapSettings");
            tooltip = "Grid & Snapping Settings";
            clicked += SnapSettingsWindow.ShowWindow;
        }
    }

    [Icon("Assets/PluginMaster/DesignTools/PrefabWorldBuilder/Editor/Resources/Sprites/Grid.png")]
    [Overlay(typeof(SceneView), "PWB Grid Tools", true)]
    public class PWBGridToolbarOverlay : ToolbarOverlay
    {
        PWBGridToolbarOverlay() : base(GridTypeToggle.ID, SnapToggle.ID,
            GridToggle.ID, LockGridToggle.ID, GridSettingsButton.ID)
        { }
    }
    #endregion
}
#endif