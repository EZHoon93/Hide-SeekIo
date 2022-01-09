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
using System.Collections.Generic;
using UnityEngine;

namespace PluginMaster
{
    public static class BoundsUtils
    {
        public static readonly Vector3 MIN_VECTOR3 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        public static readonly Vector3 MAX_VECTOR3 = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

        public enum ObjectProperty
        {
            BOUNDING_BOX,
            CENTER,
            PIVOT
        }

        public static Bounds GetBounds(Transform transform, ObjectProperty property = ObjectProperty.BOUNDING_BOX)
        {
            var renderer = transform.GetComponent<Renderer>();
            var rectTransform = transform.GetComponent<RectTransform>();

            if (rectTransform == null)
            {
                if (renderer == null || !renderer.enabled || property == ObjectProperty.PIVOT)
                    return new Bounds(transform.position, Vector3.zero);
                if (property == ObjectProperty.CENTER) return new Bounds(renderer.bounds.center, Vector3.zero);
                return renderer.bounds;
            }
            else
            {
                if (property == ObjectProperty.PIVOT) return new Bounds(rectTransform.position, Vector3.zero);
                return new Bounds(rectTransform.TransformPoint(rectTransform.rect.center),
                    rectTransform.TransformVector(rectTransform.rect.size));
            }
        }

        public static Bounds GetBoundsRecursive(Transform transform, bool recursive = true,
            ObjectProperty property = ObjectProperty.BOUNDING_BOX)
        {

            if (!recursive) return GetBounds(transform, property);
            var children = transform.GetComponentsInChildren<Transform>(true);
            var min = MAX_VECTOR3;
            var max = MIN_VECTOR3;
            var emptyHierarchy = true;
            bool IsActiveInHierarchy(Transform obj)
            {
                var parent = obj;
                do
                {
                    if (!parent.gameObject.activeSelf) return false;
                    parent = parent.parent;
                }
                while (parent != null);
                return true;
            }
            foreach (var child in children)
            {
                var notActive = !IsActiveInHierarchy(child);
                if (notActive) continue;
                var renderer = child.GetComponent<Renderer>();
                var rectTransform = child.GetComponent<RectTransform>();
                if ((renderer == null || !renderer.enabled) && rectTransform == null) continue;
                var bounds = GetBounds(child, property);
                if (bounds.size == Vector3.zero) continue;
                emptyHierarchy = false;
                min = Vector3.Min(bounds.min, min);
                max = Vector3.Max(bounds.max, max);
            }
            if (emptyHierarchy) return new Bounds(transform.position, Vector3.zero);
            var size = max - min;
            var center = min + size / 2f;
            return new Bounds(center, size);
        }

        public static Bounds GetSelectionBounds(GameObject[] selection, bool recursive = true,
            BoundsUtils.ObjectProperty property = BoundsUtils.ObjectProperty.BOUNDING_BOX)
        {
            var max = MIN_VECTOR3;
            var min = MAX_VECTOR3;
            foreach (var obj in selection)
            {
                if (obj == null) continue;
                var bounds = GetBoundsRecursive(obj.transform, recursive, property);
                max = Vector3.Max(bounds.max, max);
                min = Vector3.Min(bounds.min, min);
            }
            var size = max - min;
            var center = min + size / 2f;
            return new Bounds(center, size);
        }

        public static Bounds GetBounds(Transform transform, Quaternion rotation)
        {
            var rectTransform = transform.GetComponent<RectTransform>();
            if (rectTransform != null)
                return new Bounds(rectTransform.TransformPoint(rectTransform.rect.center),
                    rectTransform.TransformVector(rectTransform.rect.size));
            var renderer = transform.GetComponent<Renderer>();
            var meshFilter = transform.GetComponent<MeshFilter>();
            if (renderer == null || meshFilter == null || meshFilter.sharedMesh == null || !renderer.enabled)
                return new Bounds(transform.position, Vector3.zero);
            var maxSqrDistance = MIN_VECTOR3;
            var minSqrDistance = MAX_VECTOR3;
            var center = GetBounds(transform).center;
            var right = rotation * Vector3.right;
            var up = rotation * Vector3.up;
            var forward = rotation * Vector3.forward;
            foreach (var vertex in meshFilter.sharedMesh.vertices)
            {
                var centerToVertex = transform.TransformPoint(vertex) - center;
                var rightProjection = Vector3.Project(centerToVertex, right);
                var upProjection = Vector3.Project(centerToVertex, up);
                var forwardProjection = Vector3.Project(centerToVertex, forward);
                var rightSqrDistance = rightProjection.sqrMagnitude * (rightProjection.normalized != right ? -1 : 1);
                var upSqrDistance = upProjection.sqrMagnitude * (upProjection.normalized != up ? -1 : 1);
                var forwardSqrDistance = forwardProjection.sqrMagnitude
                    * (forwardProjection.normalized != forward ? -1 : 1);
                maxSqrDistance.x = Mathf.Max(maxSqrDistance.x, rightSqrDistance);
                maxSqrDistance.y = Mathf.Max(maxSqrDistance.y, upSqrDistance);
                maxSqrDistance.z = Mathf.Max(maxSqrDistance.z, forwardSqrDistance);
                minSqrDistance.x = Mathf.Min(minSqrDistance.x, rightSqrDistance);
                minSqrDistance.y = Mathf.Min(minSqrDistance.y, upSqrDistance);
                minSqrDistance.z = Mathf.Min(minSqrDistance.z, forwardSqrDistance);
            }
            var size = new Vector3(
                Mathf.Sqrt(Mathf.Abs(maxSqrDistance.x)) * Mathf.Sign(maxSqrDistance.x)
                - Mathf.Sqrt(Mathf.Abs(minSqrDistance.x)) * Mathf.Sign(minSqrDistance.x),
                Mathf.Sqrt(Mathf.Abs(maxSqrDistance.y)) * Mathf.Sign(maxSqrDistance.y)
                - Mathf.Sqrt(Mathf.Abs(minSqrDistance.y)) * Mathf.Sign(minSqrDistance.y),
                Mathf.Sqrt(Mathf.Abs(maxSqrDistance.z)) * Mathf.Sign(maxSqrDistance.z)
                - Mathf.Sqrt(Mathf.Abs(minSqrDistance.z)) * Mathf.Sign(minSqrDistance.z));
            return new Bounds(center, size);
        }

        private static void GetDistanceFromCenter(Transform transform, Quaternion rotation,
            Vector3 center, out Vector3 min, out Vector3 max)
        {
            min = max = Vector3.zero;
            if (!transform.gameObject.activeSelf) return;
            var vertices = new List<Vector3>();
            var rectTransform = transform.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                vertices.Add(rectTransform.rect.min);
                vertices.Add(rectTransform.rect.max);
                vertices.Add(new Vector2(rectTransform.rect.min.x, rectTransform.rect.max.y));
                vertices.Add(new Vector2(rectTransform.rect.max.x, rectTransform.rect.min.y));
            }
            else
            {
                var renderer = transform.GetComponent<Renderer>();
                if (renderer == null || !renderer.enabled) return;
                if (renderer is SpriteRenderer)
                {
                    var sprite = (renderer as SpriteRenderer).sprite;
                    if (sprite == null) return;
                    var spriteSize = sprite.rect.size / sprite.pixelsPerUnit;
                    vertices.Add(Vector3.Scale(spriteSize, new Vector3(-0.5f, -0.5f, 0f)));
                    vertices.Add(Vector3.Scale(spriteSize, new Vector3(-0.5f, 0.5f, 0f)));
                    vertices.Add(Vector3.Scale(spriteSize, new Vector3(0.5f, -0.5f, 0f)));
                    vertices.Add(Vector3.Scale(spriteSize, new Vector3(0.5f, 0.5f, 0f)));
                }
                else if (renderer is MeshRenderer)
                {
                    var meshFilter = transform.GetComponent<MeshFilter>();
                    if (meshFilter == null || meshFilter.sharedMesh == null) return;
                    vertices.AddRange(meshFilter.sharedMesh.vertices);
                }
                else if (renderer is SkinnedMeshRenderer)
                {
                    var mesh = (renderer as SkinnedMeshRenderer).sharedMesh;
                    if (mesh == null) return;
                    vertices.AddRange(mesh.vertices);
                }
            }
            var maxSqrDistance = MIN_VECTOR3;
            var minSqrDistance = MAX_VECTOR3;
            var right = rotation * Vector3.right;
            var up = rotation * Vector3.up;
            var forward = rotation * Vector3.forward;

            foreach (var vertex in vertices)
            {
                var centerToVertex = transform.TransformPoint(vertex) - center;
                var rightProjection = Vector3.Project(centerToVertex, right);
                var upProjection = Vector3.Project(centerToVertex, up);
                var forwardProjection = Vector3.Project(centerToVertex, forward);
                var rightSqrDistance = rightProjection.sqrMagnitude * (rightProjection.normalized != right ? -1 : 1);
                var upSqrDistance = upProjection.sqrMagnitude * (upProjection.normalized != up ? -1 : 1);
                var forwardSqrDistance = forwardProjection.sqrMagnitude
                    * (forwardProjection.normalized != forward ? -1 : 1);

                maxSqrDistance.x = Mathf.Max(maxSqrDistance.x, rightSqrDistance);
                maxSqrDistance.y = Mathf.Max(maxSqrDistance.y, upSqrDistance);
                maxSqrDistance.z = Mathf.Max(maxSqrDistance.z, forwardSqrDistance);

                minSqrDistance.x = Mathf.Min(minSqrDistance.x, rightSqrDistance);
                minSqrDistance.y = Mathf.Min(minSqrDistance.y, upSqrDistance);
                minSqrDistance.z = Mathf.Min(minSqrDistance.z, forwardSqrDistance);
            }

            min = new Vector3(
                Mathf.Sqrt(Mathf.Abs(minSqrDistance.x)) * Mathf.Sign(minSqrDistance.x),
                Mathf.Sqrt(Mathf.Abs(minSqrDistance.y)) * Mathf.Sign(minSqrDistance.y),
                Mathf.Sqrt(Mathf.Abs(minSqrDistance.z)) * Mathf.Sign(minSqrDistance.z));
            max = new Vector3(
               Mathf.Sqrt(Mathf.Abs(maxSqrDistance.x)) * Mathf.Sign(maxSqrDistance.x),
               Mathf.Sqrt(Mathf.Abs(maxSqrDistance.y)) * Mathf.Sign(maxSqrDistance.y),
               Mathf.Sqrt(Mathf.Abs(maxSqrDistance.z)) * Mathf.Sign(maxSqrDistance.z));
        }


        private static void GetDistanceFromCenterRecursive(Transform transform, Quaternion rotation,
            Vector3 center, out Vector3 minDistance, out Vector3 maxDistance)
        {
            var children = transform.GetComponentsInChildren<Transform>(true);
            var emptyHierarchy = true;
            maxDistance = MIN_VECTOR3;
            minDistance = MAX_VECTOR3;
            foreach (var child in children)
            {
                var renderer = child.GetComponent<Renderer>();
                var rectTransform = child.GetComponent<RectTransform>();
                if ((renderer == null || !renderer.enabled) && rectTransform == null) continue;
                emptyHierarchy = false;

                Vector3 min, max;
                GetDistanceFromCenter(child, rotation, center, out min, out max);
                minDistance = Vector3.Min(min, minDistance);
                maxDistance = Vector3.Max(max, maxDistance);
            }
            if (emptyHierarchy) minDistance = maxDistance = Vector3.zero;
        }

        public static Bounds GetBoundsRecursive(Transform transform, Quaternion rotation)
        {
            var center = GetBoundsRecursive(transform).center;
            Vector3 maxDistance, minDistance;
            GetDistanceFromCenterRecursive(transform, rotation, center, out minDistance, out maxDistance);
            var size = maxDistance - minDistance;
            center += rotation * (minDistance + size / 2);
            return new Bounds(center, size);
        }

        public static Bounds GetBoundsRecursive(Transform transform, Quaternion rotation, Vector3 scale)
        {
            var obj = Object.Instantiate(transform.gameObject);
            obj.transform.localScale = Vector3.Scale(obj.transform.localScale, scale);
            var bounds = GetBoundsRecursive(obj.transform, rotation);
            Object.DestroyImmediate(obj);
            return bounds;
        }

        public static Bounds GetSelectionBounds(GameObject[] selection, Quaternion rotation)
        {
            var max = MIN_VECTOR3;
            var min = MAX_VECTOR3;
            var center = GetSelectionBounds(selection).center;
            bool empty = true;
            foreach (var obj in selection)
            {
                if (obj == null) continue;
                var objMagnitude = GetMagnitude(obj.transform);
                if (objMagnitude == 0) continue;
                Vector3 minDistance, maxDistance;
                GetDistanceFromCenterRecursive(obj.transform, rotation, center, out minDistance, out maxDistance);
                max = Vector3.Max(maxDistance, max);
                min = Vector3.Min(minDistance, min);
                empty = false;
            }
            if (empty) return new Bounds(center, Vector3.zero);
            var size = max - min;
            center += rotation * (min + size / 2);
            return new Bounds(center, size);
        }

        public static Vector3[] GetVertices(Transform transform)
        {
            var vertices = new List<Vector3>();
            var meshFilters = transform.GetComponentsInChildren<MeshFilter>();
            foreach (var filter in meshFilters)
            {
                if (filter.sharedMesh == null) continue;
                vertices.AddRange(filter.sharedMesh.vertices);
            }
            var skinnedMeshRenderers = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in skinnedMeshRenderers)
            {
                if (renderer.sharedMesh == null) continue;
                vertices.AddRange(renderer.sharedMesh.vertices);
            }
            return vertices.ToArray();
        }

        public static Vector3[] GetBottomVertices(Transform transform, Space space = Space.Self)
        {
            var vertices = new List<Vector3>();
            var allLocalVertices = new List<Vector3>();
            var minY = float.MaxValue;
            var meshFilters = transform.GetComponentsInChildren<MeshFilter>();
            void UpdateMinVertex(Vector3 vertex, Transform child)
            {
                var worldVertex = child.TransformPoint(vertex);
                var localVertex = space == Space.Self ? transform.InverseTransformPoint(worldVertex) : worldVertex;
                allLocalVertices.Add(localVertex);
                minY = Mathf.Min(localVertex.y, minY);
            }
            foreach (var filter in meshFilters)
            {
                if (filter.sharedMesh == null) continue;
                foreach (var vertex in filter.sharedMesh.vertices) UpdateMinVertex(vertex, filter.transform);
            }
            var skinnedMeshRenderers = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in skinnedMeshRenderers)
            {
                if (renderer.sharedMesh == null) continue;
                foreach (var vertex in renderer.sharedMesh.vertices) UpdateMinVertex(vertex, renderer.transform);
            }

            var threshold = 0.01f;
            foreach (var vertex in allLocalVertices)
                if (vertex.y < minY + threshold)
                {
                    var localVertex = space == Space.Self ? vertex : transform.InverseTransformPoint(vertex);
                    vertices.Add(localVertex);
                }
            return vertices.ToArray();
        }

        public static float GetBottomMagnitude(Transform transform)
        {
            var vertices = GetBottomVertices(transform);
            var magnitude = 0f;
            foreach (var vertex in vertices) magnitude = Mathf.Max(magnitude, Mathf.Abs(vertex.y));
            return magnitude;
        }
        public static float GetMagnitude(Transform transform)
        {
            var size = GetBoundsRecursive(transform).size;
            return Mathf.Max(size.x, size.y, size.z);
        }

        public static float GetAverageMagnitude(Transform transform)
        {
            var size = GetBoundsRecursive(transform).size;
            return (size.x + size.y + size.z) / 3;
        }
    }
}