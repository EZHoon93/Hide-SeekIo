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
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace PluginMaster
{
    [InitializeOnLoad]
    public static class ToolManager
    {
        public enum PaintTool
        {
            NONE,
            PIN,
            BRUSH,
            GRAVITY,
            LINE,
            SHAPE,
            TILING,
            REPLACER,
            ERASER,
            SELECTION,
            EXTRUDE,
            MIRROR
        }
        private static PaintTool _tool = ToolManager.PaintTool.NONE;
        public static Action<PaintTool> OnToolChange;
        static ToolManager()
        {
            EditorSceneManager.activeSceneChangedInEditMode += OnSceneChange;
            PaletteManager.OnBrushChanged += TilingManager.settings.UpdateCellSize;
        }

        public static ToolManager.PaintTool tool
        {
            get => _tool;
            set
            {
                if (_tool == value) return;
                var prevTool = _tool;
                _tool = value;
                switch (_tool)
                {
                    case PaintTool.PIN:
                        PWBCore.UpdateTempColliders();
                        PWBIO.ResetPinValues();
                        break;
                    case PaintTool.BRUSH:
                        PWBCore.UpdateTempColliders();
                        break;
                    case PaintTool.GRAVITY:
                        PWBCore.DestroyTempColliders();
                        break;
                    case PaintTool.REPLACER:
                        PWBIO.UpdateOctree();
                        PWBCore.UpdateTempColliders();
                        PWBIO.ResetReplacer();
                        break;
                    case PaintTool.ERASER:
                        PWBIO.UpdateOctree();
                        break;
                    case PaintTool.EXTRUDE:
                        SelectionManager.UpdateSelection();
                        PWBIO.ResetUnityCurrentTool();
                        break;
                    case PaintTool.LINE:
                        PWBCore.UpdateTempColliders();
                        PWBIO.ResetLineState();
                        break;
                    case PaintTool.SHAPE:
                        PWBIO.ResetShapeState();
                        break;
                    case PaintTool.TILING:
                        PWBCore.UpdateTempColliders();
                        PWBIO.ResetTilingState();
                        break;
                    case PaintTool.SELECTION:
                        PWBCore.UpdateTempColliders();
                        SelectionManager.UpdateSelection();
                        PWBIO.ResetUnityCurrentTool();
                        break;
                    case PaintTool.MIRROR:
                        SelectionManager.UpdateSelection();
                        PWBIO.InitializeMirrorPose();
                        break;
                    case PaintTool.NONE:
                        PWBIO.ResetUnityCurrentTool();
                        PWBIO.ResetReplacer();
                        break;
                    default: break;
                }

                if (_tool != PaintTool.NONE)
                {
                    PWBIO.SaveUnityCurrentTool();
                    ToolProperties.ShowWindow();
                    PaletteManager.pickingBrushes = false;
                }

                if (_tool == PaintTool.BRUSH || _tool == PaintTool.PIN || _tool == PaintTool.GRAVITY
                    || _tool == PaintTool.REPLACER || _tool == PaintTool.ERASER || _tool == PaintTool.LINE
                    || _tool == PaintTool.SHAPE || _tool == PaintTool.TILING)
                {
                    PrefabPalette.ShowWindow();
                    BrushProperties.ShowWindow();
                    SelectionManager.UpdateSelection();
                    BrushstrokeManager.UpdateBrushstroke();
                    PWBIO.ResetAutoParent();
                }
                ToolProperties.RepainWindow();
                if (BrushProperties.instance != null) BrushProperties.instance.Repaint();
                if (SceneView.sceneViews.Count > 0) ((SceneView)SceneView.sceneViews[0]).Focus();
                if(_tool != prevTool) OnToolChange(prevTool);
            }
        }

        public static void DeselectTool()
        {
            if (tool == ToolManager.PaintTool.REPLACER) PWBIO.ResetReplacer();
            tool = ToolManager.PaintTool.NONE;
            PWBIO.ResetUnityCurrentTool();
        }

        private static void OnSceneChange(Scene previous, Scene current) => DeselectTool();

        public static void OnPaletteClosed()
        {
            if (tool != ToolManager.PaintTool.ERASER && tool != ToolManager.PaintTool.EXTRUDE)
                tool = ToolManager.PaintTool.NONE;
        }
    }
}