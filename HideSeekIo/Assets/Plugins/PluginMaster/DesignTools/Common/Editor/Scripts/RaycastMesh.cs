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
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PluginMaster
{
    public static class RaycastMesh
    {
        public static GameObject[] FindFilters(LayerMask mask, GameObject[] exclude = null, bool excludeColliders = true)
        {
            var objects = new HashSet<GameObject>();
            var meshFilters = GameObject.FindObjectsOfType<MeshFilter>();
            var skinnedMeshes = GameObject.FindObjectsOfType<SkinnedMeshRenderer>();
            objects.UnionWith(meshFilters.Select(comp => comp.gameObject));
            objects.UnionWith(skinnedMeshes.Select(comp => comp.gameObject));

            var filterList = new List<GameObject>(objects);
            if (exclude != null)
            {
                filterList = new List<GameObject>(objects.Except(exclude));
                objects = new HashSet<GameObject>(filterList);
            }
            if(excludeColliders)
            {
                var colliders = GameObject.FindObjectsOfType<Collider>();
                var collidersSet = new HashSet<GameObject>(colliders.Select(comp => comp.gameObject));
                filterList = new List<GameObject>(objects.Except(collidersSet));
            }
            filterList = filterList.Where(obj => (mask.value & (1 << obj.layer)) != 0).ToList();
            return filterList.ToArray();
        }


        private static MethodInfo intersectRayMesh = null;
        public static bool Raycast(Ray ray, out RaycastHit hitInfo, out GameObject collider, GameObject[] filters, float maxDistance)
        {
            collider = null;
            hitInfo = new RaycastHit();
            if (intersectRayMesh == null)
            {
                var editorTypes = typeof(Editor).Assembly.GetTypes();
                var type_HandleUtility = editorTypes.FirstOrDefault(t => t.Name == "HandleUtility");
                intersectRayMesh = type_HandleUtility.GetMethod("IntersectRayMesh", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            }
            var minDistance = float.MaxValue;
            var result = false;
            foreach (var filter in filters)
            {
                if (filter == null) continue;
                var meshFilter = filter.GetComponent<MeshFilter>();
                Mesh mesh;
                if (meshFilter == null)
                {
                    var skinnedMeshRenderer = filter.GetComponent<SkinnedMeshRenderer>();
                    if (skinnedMeshRenderer == null) continue;
                    mesh = skinnedMeshRenderer.sharedMesh;
                }
                else if (meshFilter.sharedMesh == null) continue;
                else mesh = meshFilter.sharedMesh;

                var parameters = new object[] { ray, mesh, filter.transform.localToWorldMatrix, null };
                if ((bool)intersectRayMesh.Invoke(null, parameters))
                {
                    if (hitInfo.distance > maxDistance) continue;
                    result = true;
                    var hit = (RaycastHit)parameters[3];
                    if (hit.distance < minDistance)
                    {
                        collider = filter;
                        minDistance = hit.distance;
                        hitInfo = hit;
                    }
                }
            }
            if(result)
            {
                hitInfo.normal = hitInfo.normal.normalized;
            }
            return result;
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, out GameObject collider, GameObject[]  filters, float maxDistance)
        {
            var ray = new Ray(origin, direction);
            return Raycast(ray, out hitInfo, out collider, filters, maxDistance);
        }

        public static bool RaycastAll(Ray ray, out RaycastHit[] hitInfo, out GameObject[] colliders, GameObject[] filters, float maxDistance)
        {
            List<RaycastHit> hitInfoList = new List<RaycastHit>();
            List<GameObject> colliderList = new List<GameObject>();
            if (intersectRayMesh == null)
            {
                var editorTypes = typeof(Editor).Assembly.GetTypes();
                var type_HandleUtility = editorTypes.FirstOrDefault(t => t.Name == "HandleUtility");
                intersectRayMesh = type_HandleUtility.GetMethod("IntersectRayMesh", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            }
            foreach (var filter in filters)
            {
                if (filter == null) continue;
                var meshFilter = filter.GetComponent<MeshFilter>();
                Mesh mesh;
                if (meshFilter == null)
                {
                    var skinnedMeshRenderer = filter.GetComponent<SkinnedMeshRenderer>();
                    if (skinnedMeshRenderer == null) continue;
                    mesh = skinnedMeshRenderer.sharedMesh;
                }
                else if (meshFilter.sharedMesh == null) continue;
                else mesh = meshFilter.sharedMesh;

                var parameters = new object[] { ray, mesh, filter.transform.localToWorldMatrix, null };
                if ((bool)intersectRayMesh.Invoke(null, parameters))
                {
                    var hit = (RaycastHit)parameters[3];
                    if (hit.distance > maxDistance) continue;
                    hitInfoList.Add(hit);
                    colliderList.Add(filter);
                }
            }
            hitInfo = hitInfoList.ToArray();
            colliders = colliderList.ToArray();
            return hitInfoList.Count > 0;
        }
    }
}
