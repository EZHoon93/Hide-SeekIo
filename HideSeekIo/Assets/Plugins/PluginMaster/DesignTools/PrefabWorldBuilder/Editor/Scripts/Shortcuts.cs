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
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace PluginMaster
{
    public static partial class Shortcuts
    {
#if UNITY_2019_1_OR_NEWER
        #region TOGGLE TOOLS
        private static void ToogleTool(ToolManager.PaintTool tool)
        {
#if UNITY_2021_2_OR_NEWER
#else
            if (PWBToolbar.instance == null) PWBToolbar.ShowWindow();
#endif
            ToolManager.tool = ToolManager.tool == tool ? ToolManager.PaintTool.NONE : tool;
        }

        public const string PWB_TOGGLE_PIN_SHORTCUT_ID = "Prefab World Builder/Tools - Toggle Pin Tool";
        [Shortcut(PWB_TOGGLE_PIN_SHORTCUT_ID, KeyCode.Alpha1, ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        private static void TogglePin() => ToogleTool(ToolManager.PaintTool.PIN);

        public const string PWB_TOGGLE_BRUSH_SHORTCUT_ID = "Prefab World Builder/Tools - Toggle Brush Tool";
        [Shortcut(PWB_TOGGLE_BRUSH_SHORTCUT_ID, KeyCode.Alpha2, ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        private static void ToggleBrush() => ToogleTool(ToolManager.PaintTool.BRUSH);

        public const string PWB_TOGGLE_GRAVITY_SHORTCUT_ID = "Prefab World Builder/Tools - Toggle Gravity Tool";
        [Shortcut(PWB_TOGGLE_GRAVITY_SHORTCUT_ID, KeyCode.Alpha3, ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        private static void ToggleGravity() => ToogleTool(ToolManager.PaintTool.GRAVITY);

        public const string PWB_TOGGLE_LINE_SHORTCUT_ID = "Prefab World Builder/Tools - Toggle Line Tool";
        [Shortcut(PWB_TOGGLE_LINE_SHORTCUT_ID, KeyCode.Alpha4, ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        private static void ToggleLine() => ToogleTool(ToolManager.PaintTool.LINE);

        public const string PWB_TOGGLE_SHAPE_SHORTCUT_ID = "Prefab World Builder/Tools - Toggle Shape Tool";
        [Shortcut(PWB_TOGGLE_SHAPE_SHORTCUT_ID, KeyCode.Alpha5, ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        private static void ToggleShape() => ToogleTool(ToolManager.PaintTool.SHAPE);

        public const string PWB_TOGGLE_TILING_SHORTCUT_ID = "Prefab World Builder/Tools - Toggle Tiling Tool";
        [Shortcut(PWB_TOGGLE_TILING_SHORTCUT_ID, KeyCode.Alpha6, ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        private static void ToggleTiling() => ToogleTool(ToolManager.PaintTool.TILING);

        public const string PWB_TOGGLE_REPLACER_SHORTCUT_ID = "Prefab World Builder/Tools - Toggle Replacer Tool";
        [Shortcut(PWB_TOGGLE_REPLACER_SHORTCUT_ID, KeyCode.Alpha7, ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        private static void ToggleReplacer() => ToogleTool(ToolManager.PaintTool.REPLACER);

        public const string PWB_TOGGLE_ERASER_SHORTCUT_ID = "Prefab World Builder/Tools - Toggle Eraser Tool";
        [Shortcut(PWB_TOGGLE_ERASER_SHORTCUT_ID, KeyCode.Alpha8, ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        private static void ToggleEraser() => ToogleTool(ToolManager.PaintTool.ERASER);

        public const string PWB_TOGGLE_SELECTION_SHORTCUT_ID = "Prefab World Builder/Tools - Toggle Selection Tool";
        [Shortcut(PWB_TOGGLE_SELECTION_SHORTCUT_ID, KeyCode.Alpha9, ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        private static void ToggleSelection() => ToogleTool(ToolManager.PaintTool.SELECTION);

        public const string PWB_TOGGLE_EXTRUDE_SHORTCUT_ID = "Prefab World Builder/Tools - Toggle Extrude Tool";
        [Shortcut(PWB_TOGGLE_EXTRUDE_SHORTCUT_ID, KeyCode.X, ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        private static void ToggleExtrude() => ToogleTool(ToolManager.PaintTool.EXTRUDE);

        public const string PWB_TOGGLE_MIRROR_SHORTCUT_ID = "Prefab World Builder/Tools - Toggle Mirror Tool";
        [Shortcut(PWB_TOGGLE_MIRROR_SHORTCUT_ID, KeyCode.M, ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        private static void ToggleMirror() => ToogleTool(ToolManager.PaintTool.MIRROR);
#endregion

            #region GRID AND SNAPPING
        public const string PWB_TOGGLE_GRID_LOCK_SHORTCUT_ID = "Prefab World Builder/Grid - Toggle Grid Lock";
        [Shortcut(PWB_TOGGLE_GRID_LOCK_SHORTCUT_ID)]
        private static void ToggleGridLock()
        {
            SnapManager.settings.lockedGrid = !SnapManager.settings.lockedGrid;
            PWBToolbar.instance.Repaint();
        }
        public const string PWB_FRAME_GRID_ORIGIN_SHORTCUT_ID
            = "Prefab World Builder/Grid - Frame Origin";
        [Shortcut(PWB_FRAME_GRID_ORIGIN_SHORTCUT_ID, KeyCode.Q,
            ShortcutModifiers.Action | ShortcutModifiers.Alt)]
        private static void FrameGridOrigin() => SnapManager.FrameGridOrigin();

        public const string PWB_TOGGLE_GRID_POSITION_HANDLE_SHORTCUT_ID 
            = "Prefab World Builder/Grid - Toggle Postion Handle";
        [Shortcut(PWB_TOGGLE_GRID_POSITION_HANDLE_SHORTCUT_ID, KeyCode.W,
            ShortcutModifiers.Action | ShortcutModifiers.Alt)]
        private static void ToggleGridPositionHandle() => SnapManager.ToggleGridPositionHandle();

        public const string PWB_TOGGLE_GRID_ROTATION_HANDLE_SHORTCUT_ID
            = "Prefab World Builder/Grid - Toggle Rotation Handle";
        [Shortcut(PWB_TOGGLE_GRID_ROTATION_HANDLE_SHORTCUT_ID, KeyCode.E,
            ShortcutModifiers.Action | ShortcutModifiers.Alt)]
        private static void ToggleGridRotationHandle() => SnapManager.ToggleGridRotationHandle();

        public const string PWB_TOGGLE_GRID_SCALE_HANDLE_SHORTCUT_ID
            = "Prefab World Builder/Grid - Toggle Spacing Handle";
        [Shortcut(PWB_TOGGLE_GRID_SCALE_HANDLE_SHORTCUT_ID, KeyCode.R,
            ShortcutModifiers.Action | ShortcutModifiers.Alt)]
        private static void ToggleGridScaleHandle() => SnapManager.ToggleGridScaleHandle();
            #endregion

            #region PALETTE
        public const string PWB_TOGGLE_PICKER_SHORTCUT_ID = "Prefab World Builder/Palette - Toggle Brush Picker";
        [Shortcut(PWB_TOGGLE_PICKER_SHORTCUT_ID, KeyCode.P, ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        private static void TogglePicker()
        {
            PaletteManager.pickingBrushes = !PaletteManager.pickingBrushes;
            PrefabPalette.RepainWindow();
        }

        public const string PWB_NEW_MULTIBRUSH_FROM_SELECTION_SHORTCUT_ID
            = "Prefab World Builder/Palette - New MultiBrush From Selection";
        [Shortcut(PWB_NEW_MULTIBRUSH_FROM_SELECTION_SHORTCUT_ID)]
        private static void NewMultiBrushFromSelection()
        {
            if (PrefabPalette.instance == null) PrefabPalette.ShowWindow();
            PrefabPalette.instance.CreateBrushFromSelection();
        }

        public const string PWB_NEW_BRUSH_FROM_EACH_PREFAB_SELECTED_SHORTCUT_ID
            = "Prefab World Builder/Palette - New Brush From Each Prefab Selected";
        [Shortcut(PWB_NEW_BRUSH_FROM_EACH_PREFAB_SELECTED_SHORTCUT_ID)]
        private static void NewBushFromEachPrefabSelected()
        {
            if (PrefabPalette.instance == null) PrefabPalette.ShowWindow();
            PrefabPalette.instance.CreateBushFromEachPrefabSelected();
        }

        public const string PWB_FILTER_BRUSH_BY_SELECTION_SHORTCUT_ID = "Prefab World Builder/Palette - Filter by selection";
        [Shortcut(PWB_FILTER_BRUSH_BY_SELECTION_SHORTCUT_ID)]
        private static void FilterBySelection()
        {
            if (PrefabPalette.instance == null) PrefabPalette.ShowWindow();
            if (PrefabPalette.instance.FilterBySelection() == 1) PrefabPalette.instance.SelectFirstBrush();
        }
            #endregion
#endif
        }
}
