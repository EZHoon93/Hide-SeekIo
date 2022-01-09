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
#if !UNITY_2020_2_OR_NEWER
using System.Linq;
using System.Reflection;
#endif
using UnityEditor;
using UnityEngine;

namespace PluginMaster
{
    public static class DistanceUtils
    {
#if !UNITY_2020_2_OR_NEWER
        private static MethodInfo findNearestVertex = null;
#endif
        public static bool FindNearestVertexToMouse(out RaycastHit hit, Transform transform = null)
        {
            var mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            hit = new RaycastHit();
            hit.point = mouseRay.origin;
            hit.normal = Vector3.up;
            hit.distance = 0f;

            bool result = false;
            var vertex = Vector3.zero;
#if UNITY_2020_2_OR_NEWER
            if (transform == null) result = HandleUtility.FindNearestVertex(Event.current.mousePosition, out vertex);
            else
                result = HandleUtility.FindNearestVertex(Event.current.mousePosition,
                new Transform[] { transform }, out vertex);
#else
            Transform[] selection = { transform };
            if (findNearestVertex == null)
            {
                var editorTypes = typeof(Editor).Assembly.GetTypes();
                var type_HandleUtility = editorTypes.FirstOrDefault(t => t.Name == "HandleUtility");
                findNearestVertex = type_HandleUtility.GetMethod("FindNearestVertex",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            }
            var parameters = new object[] { Event.current.mousePosition, selection, null };
            result = (bool)findNearestVertex.Invoke(null, parameters);
            if (result) vertex = (Vector3)parameters[2];
#endif
            if (result)
            {
                var vertexRay = new Ray(vertex - mouseRay.direction, mouseRay.direction);
                if (Physics.Raycast(vertexRay, out RaycastHit rayHit))
                {
                    if (transform == null) transform = rayHit.collider.transform;
                    hit.normal = rayHit.normal;
                }
                else if (transform != null)
                {
                    var normal = GetNormal(transform, vertex, Vector3.zero);
                    if (normal != Vector3.zero) hit.normal = normal;
                }
                hit.point = vertex;
            }
            return result;
        }

        private static Vector3 GetNormal(Transform transform, Vector3 vertex, Vector3 defaultValue)
        {
            var result = defaultValue;
            var colliders = transform.GetComponentsInChildren<Collider>();

            int GetVertexIdx(Mesh mesh, Vector3 v)
            {
                if (mesh == null) return -1;
                var idx = Array.IndexOf(mesh.vertices, v);
                if (idx < 0) return idx;
                return idx;
            }
            foreach (var collider in colliders)
            {
                if (collider is MeshCollider)
                {
                    var meshCollider = collider as MeshCollider;
                    var idx = GetVertexIdx(meshCollider.sharedMesh, vertex);
                    if (idx < 0) continue;
                    return meshCollider.sharedMesh.normals[idx];
                }
                else
                {
                    var center = BoundsUtils.GetBoundsRecursive(transform).center;
                    result = (vertex - center).normalized;
                    return result;
                }
            }
            var meshFilters = transform.GetComponentsInChildren<MeshFilter>();
            foreach (var filter in meshFilters)
            {
                var mesh = filter.sharedMesh;
                var idx = GetVertexIdx(mesh, vertex);
                if (idx < 0) continue;
                return mesh.normals[idx];
            }
            var renderers = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in renderers)
            {
                var mesh = renderer.sharedMesh;
                var idx = GetVertexIdx(mesh, vertex);
                if (idx < 0) continue;
                return mesh.normals[idx];
            }
            return result;
        }

    }
}
