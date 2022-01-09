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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PluginMaster.MultibrushSettings;

namespace PluginMaster
{
    public class BrushstrokeItem
    {
        public readonly MultibrushItemSettings settings = null;
        public Vector3 tangentPosition = Vector3.zero;
        public readonly Vector3 additionalAngle = Vector3.zero;
        public readonly Vector3 scaleMultiplier = Vector3.zero;
        public BrushstrokeItem(MultibrushItemSettings settings, Vector3 tangentPosition,
            Vector3 additionalAngle, Vector3 scaleMultiplier)
        {
            this.settings = settings;
            this.tangentPosition = tangentPosition;
            this.additionalAngle = additionalAngle;
            this.scaleMultiplier = scaleMultiplier;
        }
    }

    public static class BrushstrokeManager
    {
        private static List<BrushstrokeItem> _brushstroke = new List<BrushstrokeItem>();
        public static BrushstrokeItem[] brushstroke => _brushstroke.ToArray();        

        private static void AddBrushstrokeItem(int index, Vector3 tangentPosition, Vector3 angle,
            IPaintToolSettings paintToolSettings)
        {
            if (index < 0 || index >= PaletteManager.selectedBrush.itemCount) return;

            BrushSettings brushSettings = PaletteManager.selectedBrush.items[index];
            if (paintToolSettings != null &&paintToolSettings.overwriteBrushProperties)
                brushSettings = paintToolSettings.brushSettings;
            var additonalAngle = angle + (brushSettings.addRandomRotation
                ? brushSettings.randomEulerOffset.randomVector : brushSettings.eulerOffset);
            var scale = brushSettings.randomScaleMultiplier
                ? brushSettings.randomScaleMultiplierRange.randomVector : brushSettings.scaleMultiplier;
            if (!brushSettings.separateScaleAxes) scale.z = scale.y = scale.x;
            _brushstroke.Add(new BrushstrokeItem(PaletteManager.selectedBrush.items[index],
                tangentPosition, additonalAngle, scale));
        }

        public static float GetLineSpacing(int itemIdx, LineSettings settings)
        {
            float spacing = settings.spacing;
            if (settings.spacingType == LineSettings.SpacingType.BOUNDS && itemIdx >= 0)
            {
                var item = PaletteManager.selectedBrush.items[itemIdx];
                if (item.prefab == null) return spacing;
                var bounds = BoundsUtils.GetBoundsRecursive(item.prefab.transform);
                var size = Vector3.Scale(bounds.size, item.scaleMultiplier);
                spacing = AxesUtils.GetAxisValue(size, settings.axisOrientedAlongTheLine);
                if (spacing <= 0.0001) spacing = 0.5f;
            }
            spacing += settings.gapSize;
            return spacing;
        }

        private static void UpdateLineBrushstroke(Vector3[] points, LineSettings settings)
        {
            _brushstroke.Clear();
            if (PaletteManager.selectedBrush == null) return;

            float lineLength = 0f;
            var lengthFromFirstPoint = new float[points.Length];
            var segmentLength = new float[points.Length];
            lengthFromFirstPoint[0] = 0f;
            for (int i = 1; i < points.Length; ++i)
            {
                segmentLength[i - 1] = (points[i] - points[i - 1]).magnitude;
                lineLength += segmentLength[i - 1];
                lengthFromFirstPoint[i] = lineLength;
            }

            float length = 0f;
            int segment = 0;
            float minSpace = lineLength / 1000f;
            if (PaletteManager.selectedBrush.patternMachine != null)
                PaletteManager.selectedBrush.patternMachine.Reset();
            do
            {
                var nextIdx = PaletteManager.selectedBrush.nextItemIndex;
                while (lengthFromFirstPoint[segment + 1] < length)
                {
                    ++segment;
                    if (segment >= points.Length - 1) break;
                }
                if (segment >= points.Length - 1) break;
                var segmentDirection = (points[segment + 1] - points[segment]).normalized;
                var distance = length - lengthFromFirstPoint[segment];
                var position = points[segment] + segmentDirection * distance;
                float spacing = GetLineSpacing(nextIdx, settings);
                length += Mathf.Max(spacing, minSpace);
                if (length > lineLength) break;
                AddBrushstrokeItem(nextIdx, position, Vector3.zero, settings);
            } while (length < lineLength);
        }

        public static void UpdateLineBrushstroke(Vector3[] bezierPoints)
            => UpdateLineBrushstroke(bezierPoints, LineManager.settings);

        public static void UpdateShapeBrushstroke()
        {
            _brushstroke.Clear();
            if (PaletteManager.selectedBrush == null) return;
            var settings = ShapeManager.settings;
            var points = new List<Vector3>();
            var firstVertexIdx = ShapeData.instance.firstVertexIdxAfterIntersection;
            var lastVertexIdx = ShapeData.instance.lastVertexIdxBeforeIntersection;
            int sidesCount = settings.shapeType == ShapeSettings.ShapeType.POLYGON ? settings.sidesCount
                : ShapeData.instance.circleSideCount;
            int GetNextVertexIdx(int currentIdx) => currentIdx == sidesCount ? 1 : currentIdx + 1;
            int GetPrevVertexIdx(int currentIdx) => currentIdx == 1 ? sidesCount : currentIdx - 1;
            var firstPrev = GetPrevVertexIdx(firstVertexIdx);
            points.Add(ShapeData.instance.GetArcIntersection(0));
            if (lastVertexIdx != firstPrev || (lastVertexIdx == firstPrev && ShapeData.instance.arcAngle > 120))
            {
                var vertexIdx = firstVertexIdx;
                var nextVertexIdx = firstVertexIdx;
                do
                {
                    vertexIdx = nextVertexIdx;
                    points.Add(ShapeData.instance.GetPoint(vertexIdx));
                    nextVertexIdx = GetNextVertexIdx(nextVertexIdx);
                } while (vertexIdx != lastVertexIdx);
            }
            var lastPoint = ShapeData.instance.GetArcIntersection(1);
            if (points.Last() != lastPoint) points.Add(lastPoint);

            void AddItemsToLine(Vector3 start, Vector3 end, ref int nextIdx)
            {
                if (nextIdx < 0) nextIdx = PaletteManager.selectedBrush.nextItemIndex;
                var startToEnd = end - start;
                var lineLegth = startToEnd.magnitude;
                float itemsSize = 0f;
                var items = new List<(int idx, float size)>();
                do
                {
                    var itemSize = GetLineSpacing(nextIdx, settings);
                    if (itemsSize + itemSize > lineLegth) break;
                    itemsSize += itemSize;
                    items.Add((nextIdx, itemSize));
                    nextIdx = PaletteManager.selectedBrush.nextItemIndex;
                } while (itemsSize < lineLegth);
                var spacing = (lineLegth - itemsSize) / (items.Count + 1);
                var distance = spacing;
                var direction = startToEnd.normalized;
                Vector3 itemDir = settings.objectsOrientedAlongTheLine ? direction : Vector3.zero;
                var lookAt = Quaternion.LookRotation(
                    (Vector3)(AxesUtils.SignedAxis)(settings.axisOrientedAlongTheLine), Vector3.up);
                var angle = itemDir == Vector3.zero ? Vector3.zero
                    : (Quaternion.LookRotation(itemDir, -settings.projectionDirection) * lookAt).eulerAngles;
                foreach (var item in items)
                {
                    var brushItem = PaletteManager.selectedBrush.items[item.idx];
                    if (brushItem.prefab == null) continue;
                    var bounds = BoundsUtils.GetBoundsRecursive(brushItem.prefab.transform);
                    var size = bounds.size;
                    var centerToPivot = brushItem.prefab.transform.position - bounds.center;
                    var centerToPivotLocal = brushItem.prefab.transform.InverseTransformVector(centerToPivot);
                    var centerToPivotOnLine
                        = AxesUtils.GetAxisValue(centerToPivot, settings.axisOrientedAlongTheLine);
                    var position = start + direction * (distance + item.size / 2 + centerToPivotOnLine);
                    AddBrushstrokeItem(item.idx, position, angle, settings);
                    distance += item.size + spacing;
                }
            }
            int nexItemItemIdx = -1;
            for (int i = 0; i < points.Count - 1; ++i)
            {
                var start = points[i];
                var end = points[i + 1];
                AddItemsToLine(start, end, ref nexItemItemIdx);
            }
        }

        public static void UpdateRectBrushstroke(Vector3[] cellCenters)
        {
            _brushstroke.Clear();
            if (PaletteManager.selectedBrush == null) return;
            for (int i = 0; i < cellCenters.Length; ++i)
            {
                var nextIdx = PaletteManager.selectedBrush.nextItemIndex;
                AddBrushstrokeItem(nextIdx, cellCenters[i], Vector3.zero, TilingManager.settings);
            }
        }

        private static int _currentPinIdx = 0;

        public static void SetNextPinBrushstroke(int delta)
        {
            _currentPinIdx = _currentPinIdx + delta;
            var mod = _currentPinIdx % PaletteManager.selectedBrush.itemCount;
            _currentPinIdx = mod < 0 ? PaletteManager.selectedBrush.itemCount + mod : mod;
            _brushstroke.Clear();
            AddBrushstrokeItem(_currentPinIdx, Vector3.zero, Vector3.zero, PinManager.settings);
        }

        private static void UpdateBrushBaseStroke(BrushToolBase brushSettings)
        {
            if (brushSettings.spacingType == BrushToolBase.SpacingType.AUTO)
            {
                var maxSize = 0.1f;
                foreach (var item in PaletteManager.selectedBrush.items)
                {
                    if (item.prefab == null) continue;
                    var itemSize = BoundsUtils.GetBoundsRecursive(item.prefab.transform).size;
                    itemSize = Vector3.Scale(itemSize,
                        item.randomScaleMultiplier ? item.maxScaleMultiplier : item.scaleMultiplier);
                    maxSize = Mathf.Max(itemSize.x, itemSize.z, maxSize);
                }
                brushSettings.minSpacing = maxSize;
                ToolProperties.RepainWindow();
            }

            if (brushSettings.brushShape == BrushToolSettings.BrushShape.POINT)
            {
                var nextIdx = PaletteManager.selectedBrush.nextItemIndex;
                if (nextIdx == -1) return;
                if (PaletteManager.selectedBrush.frequencyMode == FrecuencyMode.PATTERN && nextIdx == -2) return;
                _brushstroke.Clear();
                
                AddBrushstrokeItem(nextIdx, Vector3.zero, Vector3.zero, brushSettings);
                _currentPinIdx = Mathf.Clamp(nextIdx, 0, PaletteManager.selectedBrush.itemCount - 1);
            }
            else
            {
                var radius = brushSettings.radius;
                var radiusSqr = radius * radius;

                var minSpacing = brushSettings.minSpacing * 100f / brushSettings.density;

                var delta = minSpacing;
                var maxRandomOffset = delta / 3f;

                int halfSize = (int)Mathf.Ceil(radius / delta) + 1;
                int size = halfSize * 2;
                float col0x = -delta * halfSize;
                float row0y = -delta * halfSize;

                for (int row = 0; row < size; ++row)
                {
                    for (int col = 0; col < size; ++col)
                    {
                        var x = col0x + col * delta;
                        var y = row0y + row * delta;
                        if(brushSettings.randomizePositions)
                        {
                            x += Random.Range(-maxRandomOffset, maxRandomOffset);
                            y += Random.Range(-maxRandomOffset, maxRandomOffset);
                        }
                        if (brushSettings.brushShape == BrushToolBase.BrushShape.CIRCLE)
                        {
                            var distanceSqr = x * x + y * y;
                            if (distanceSqr >= radiusSqr) continue;
                        }
                        else if (brushSettings.brushShape == BrushToolBase.BrushShape.SQUARE)
                        {
                            if (Mathf.Abs(x) > radius || Mathf.Abs(y) > radius) continue;
                        }
                        var nextItemIdx = PaletteManager.selectedBrush.nextItemIndex;
                        var position = new Vector3(x, y, 0f);
                        if ((PaletteManager.selectedBrush.frequencyMode == FrecuencyMode.RANDOM && nextItemIdx == -1)
                            || (PaletteManager.selectedBrush.frequencyMode == FrecuencyMode.PATTERN
                            && nextItemIdx == -2)) continue;
                        else if (nextItemIdx != -1) AddBrushstrokeItem(nextItemIdx, position, Vector3.zero, brushSettings);
                    }
                }
            }
        }
        private static void UpdatePinBrushstroke()
        {
            var nextIdx = PaletteManager.selectedBrush.nextItemIndex;
            if (nextIdx == -1) return;
            if (PaletteManager.selectedBrush.frequencyMode == FrecuencyMode.PATTERN && nextIdx == -2)
            {
                if (PaletteManager.selectedBrush.patternMachine != null) PaletteManager.selectedBrush.patternMachine.Reset();
                else return;
            }
            AddBrushstrokeItem(nextIdx, Vector3.zero, Vector3.zero, PinManager.settings);

            const int maxTries = 10;
            int tryCount = 0;
            while (_brushstroke.Count == 0 && ++tryCount < maxTries)
            {
                nextIdx = PaletteManager.selectedBrush.nextItemIndex;
                if (nextIdx >= 0)
                {
                    AddBrushstrokeItem(nextIdx, Vector3.zero, Vector3.zero, PinManager.settings);
                    break;
                }
            }
            _currentPinIdx = Mathf.Clamp(nextIdx, 0, PaletteManager.selectedBrush.itemCount - 1);
        }
        public static void UpdateBrushstroke(bool brushChange = false)
        {
            if (ToolManager.tool == ToolManager.PaintTool.SELECTION) return;
            if (ToolManager.tool == ToolManager.PaintTool.LINE
                || ToolManager.tool == ToolManager.PaintTool.SHAPE
                || ToolManager.tool == ToolManager.PaintTool.TILING)
            {
                PWBIO.UpdateStroke();
                return;
            }
            if (!brushChange && ToolManager.tool == ToolManager.PaintTool.PIN && PinManager.settings.repeat) return;
            _brushstroke.Clear();
            if (PaletteManager.selectedBrush == null) return;
            if (ToolManager.tool == ToolManager.PaintTool.BRUSH) UpdateBrushBaseStroke(BrushManager.settings);
            else if (ToolManager.tool == ToolManager.PaintTool.GRAVITY) UpdateBrushBaseStroke(GravityToolManager.settings);
            else if (ToolManager.tool == ToolManager.PaintTool.PIN) UpdatePinBrushstroke();
            else if (ToolManager.tool == ToolManager.PaintTool.REPLACER) UpdatePinBrushstroke();
        }
    }
}
