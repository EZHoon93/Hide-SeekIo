/*
Copyright (c) 2021 Omar Duarte
Unauthorized copying of this file, via any medium is strictly prohibited.
Writen by Omar Duarte, 2021.

This class demonstrates the code discussed in these two articles:
http://devmag.org.za/2011/04/05/bzier-curves-a-tutorial/
http://devmag.org.za/2011/06/23/bzier-path-algorithms/

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using UnityEngine;
using System.Collections.Generic;

namespace PluginMaster
{
    public static class BezierPath
    {
        private const float MINIMUM_SQR_DISTANCE = 0.01f;
        private const float DIVISION_THRESHOLD = -0.99f;

        public static Vector3[] GetBezierPoints(Vector3[] segmentPoints)
        {
            var controlPoints = Interpolate(segmentPoints, 0.25f);
            var curveCount = (controlPoints.Length - 1) / 3;
            var pathPoints = GetDrawingPoints(controlPoints, curveCount);
            return pathPoints;
        }

        private static Vector3[] Interpolate(Vector3[] segmentPoints, float scale)
        {
            var controlPoints = new List<Vector3>();
            if (segmentPoints.Length < 2) return segmentPoints;

            for (int i = 0; i < segmentPoints.Length; i++)
            {
                if (i == 0)
                {
                    var p1 = segmentPoints[i];
                    var p2 = segmentPoints[i + 1];
                    var tangent = (p2 - p1);
                    var q1 = p1 + scale * tangent;
                    controlPoints.Add(p1);
                    controlPoints.Add(q1);
                }
                else if (i == segmentPoints.Length - 1)
                {
                    var p0 = segmentPoints[i - 1];
                    var p1 = segmentPoints[i];
                    var tangent = (p1 - p0);
                    var q0 = p1 - scale * tangent;
                    controlPoints.Add(q0);
                    controlPoints.Add(p1);
                }
                else
                {
                    var p0 = segmentPoints[i - 1];
                    var p1 = segmentPoints[i];
                    var p2 = segmentPoints[i + 1];
                    var tangent = (p2 - p0).normalized;
                    var q0 = p1 - scale * tangent * (p1 - p0).magnitude;
                    var q1 = p1 + scale * tangent * (p2 - p1).magnitude;
                    controlPoints.Add(q0);
                    controlPoints.Add(p1);
                    controlPoints.Add(q1);
                }
            }
            return controlPoints.ToArray();
        }

        private static Vector3 CalculateBezierPoint(int curveIndex, Vector3[] controlPoints, float t)
        {
            var nodeIndex = curveIndex * 3;
            var p0 = controlPoints[nodeIndex];
            var p1 = controlPoints[nodeIndex + 1];
            var p2 = controlPoints[nodeIndex + 2];
            var p3 = controlPoints[nodeIndex + 3];
            return CalculateBezierPoint(t, p0, p1, p2, p3);
        }

        public static Vector3[] GetDrawingPoints(Vector3[] controlPoints, int curveCount)
        {
            var drawingPoints = new List<Vector3>();
            for (int curveIndex = 0; curveIndex < curveCount; ++curveIndex)
            {
                var bezierCurveDrawingPoints = FindDrawingPoints(curveIndex, controlPoints);
                if (curveIndex != 0) bezierCurveDrawingPoints.RemoveAt(0);
                drawingPoints.AddRange(bezierCurveDrawingPoints);
            }
            return drawingPoints.ToArray();
        }

        private static List<Vector3> FindDrawingPoints(int curveIndex, Vector3[] controlPoints)
        {
            var pointList = new List<Vector3>();
            var left = CalculateBezierPoint(curveIndex, controlPoints, 0);
            var right = CalculateBezierPoint(curveIndex, controlPoints, 1);
            pointList.Add(left);
            pointList.Add(right);
            FindDrawingPoints(curveIndex, 0, 1, pointList, controlPoints, 1);
            return pointList;
        }

        private static int FindDrawingPoints(int curveIndex, float t0, float t1,
            List<Vector3> pointList, Vector3[] controlPoints, int insertionIndex)
        {
            var left = CalculateBezierPoint(curveIndex, controlPoints, t0);
            var right = CalculateBezierPoint(curveIndex, controlPoints, t1);
            if ((left - right).sqrMagnitude < MINIMUM_SQR_DISTANCE) return 0;
            var tMid = (t0 + t1) / 2;
            var mid = CalculateBezierPoint(curveIndex, controlPoints, tMid);
            var leftDirection = (left - mid).normalized;
            var rightDirection = (right - mid).normalized;
            if (Vector3.Dot(leftDirection, rightDirection) < DIVISION_THRESHOLD && Mathf.Abs(tMid - 0.5f) > 0.0001f) return 0;
            int pointsAddedCount = 0;
            pointsAddedCount += FindDrawingPoints(curveIndex, t0, tMid, pointList, controlPoints, insertionIndex);
            pointList.Insert(insertionIndex + pointsAddedCount, mid);
            pointsAddedCount++;
            pointsAddedCount += FindDrawingPoints(curveIndex, tMid, t1, pointList, controlPoints, insertionIndex + pointsAddedCount);
            return pointsAddedCount;
        }

        private static Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var u = 1 - t;
            var tt = t * t;
            var uu = u * u;
            var uuu = uu * u;
            var ttt = tt * t;
            var p = uuu * p0; //first term
            p += 3 * uu * t * p1; //second term
            p += 3 * u * tt * p2; //third term
            p += ttt * p3; //fourth term
            return p;
        }
    }
}