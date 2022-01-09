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
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace PluginMaster
{
    [Serializable]
    public class SimulateGravityData
    {
        [SerializeField] private int _maxIterations = 1000;
        [SerializeField] private Vector3 _gravity = Physics.gravity;
        [SerializeField] private float _drag = 0f;
        [SerializeField] private float _angularDrag = 0.05f;
        [SerializeField] private float _maxSpeed = 100;
        private float _maxSpeedSquared = 10000;
        [SerializeField] private float _maxAngularSpeed = 10;
        private float _maxAngularSpeedSquared = 100;
        [SerializeField] private float _mass = 1f;
        [SerializeField] private bool _changeLayer = false;
        [SerializeField] private int _tempLayer = 0;

        public int maxIterations
        {
            get => _maxIterations;
            set
            {
                value = Mathf.Clamp(value, 1, 100000);
                if (_maxIterations == value) return;
                _maxIterations = value;
            }
        }
        public Vector3 gravity
        {
            get => _gravity;
            set
            {
                if (_gravity == value) return;
                _gravity = value;
            }
        }
        public float drag
        {
            get => _drag;
            set
            {
                value = Mathf.Max(value, 0f);
                if (_drag == value) return;
                _drag = value;
            }
        }
        public float angularDrag
        {
            get => _angularDrag;
            set
            {
                value = Mathf.Max(value, 0f);
                if (_angularDrag == value) return;
                _angularDrag = value;
            }
        }
        public float maxSpeed
        {
            get => _maxSpeed;
            set
            {
                value = Mathf.Max(value, 0f);
                if (_maxSpeed == value) return;
                _maxSpeed = value;
                _maxSpeedSquared = _maxSpeed * _maxSpeed;
            }
        }
        public float maxAngularSpeed
        {
            get => _maxAngularSpeed;
            set
            {
                value = Mathf.Max(value, 0f);
                if (_maxAngularSpeed == value) return;
                _maxAngularSpeed = value;
                _maxAngularSpeedSquared = _maxAngularSpeed * _maxAngularSpeed;
            }
        }
        public float maxSpeedSquared => _maxSpeedSquared;
        public float maxAngularSpeedSquared => _maxAngularSpeedSquared;
        public float mass
        {
            get => _mass;
            set
            {
                value = Mathf.Max(value, 1e-7f);
                if (_mass == value) return;
                _mass = value;
            }
        }
        public bool changeLayer
        {
            get => _changeLayer;
            set
            {
                if (_changeLayer == value) return;
                _changeLayer = value;
            }
        }
        public int tempLayer
        {
            get => _tempLayer;
            set
            {
                if (_tempLayer == value) return;
                _tempLayer = value;
            }
        }

        public void Copy(SimulateGravityData other)
        {
            _maxIterations = other._maxIterations;
            _gravity = other._gravity;
            _drag = other._drag;
            _angularDrag = other._angularDrag;
            _maxSpeed = other._maxSpeed;
            _maxSpeedSquared = other._maxSpeedSquared;
            _maxAngularSpeed = other._maxAngularSpeed;
            _maxAngularSpeedSquared = other._maxAngularSpeedSquared;
            _mass = other._mass;
            _changeLayer = other._changeLayer;
            _tempLayer = other._tempLayer;
        }
    }



    public static class GravityUtils
    {
        private static bool _isPlaying = false;
        private static bool _stop = false;

        private static void AddCollider(GameObject obj, Mesh mesh, SimulateGravityData data)
        {
            var assetPath = AssetDatabase.GetAssetPath(mesh);
            bool isPrimitive = assetPath.Length < 6 ? false : assetPath.Substring(0, 6) != "Assets";
            if (isPrimitive)
            {
                if (mesh.name == "Sphere") obj.AddComponent<SphereCollider>();
                else if (mesh.name == "Capsule") obj.AddComponent<CapsuleCollider>();
                else if (mesh.name == "Cube") obj.AddComponent<BoxCollider>();
            }
            else
            {
                var collider = obj.AddComponent<MeshCollider>();
                collider.sharedMesh = mesh;
                collider.convex = true;
            }
            if (data.changeLayer) obj.layer = data.tempLayer;
        }

        public static void SimulateGravity(GameObject[] selection, SimulateGravityData simData, bool recordAction)
        {
            if (_isPlaying && recordAction) return;
            var originalGravity = Physics.gravity;
            Physics.gravity = simData.gravity;
            var allBodies = GameObject.FindObjectsOfType<Rigidbody>();
            var originalBodies = new List<(Rigidbody body, bool useGravity, bool isKinematic,
                float drag, float angularDrag, float mass, RigidbodyConstraints constraints,
                RigidbodyInterpolation interpolation, CollisionDetectionMode collisionDetectionMode)>();
            foreach (var rigidBody in allBodies)
            {
                originalBodies.Add((rigidBody, rigidBody.useGravity, rigidBody.isKinematic,
                    rigidBody.drag, rigidBody.angularDrag, rigidBody.mass, rigidBody.constraints,
                    rigidBody.interpolation, rigidBody.collisionDetectionMode));
                rigidBody.useGravity = false;
                rigidBody.isKinematic = true;
                rigidBody.drag = simData.drag;
                rigidBody.angularDrag = simData.angularDrag;
                rigidBody.mass = simData.mass;
                rigidBody.constraints = RigidbodyConstraints.None;
                rigidBody.interpolation = RigidbodyInterpolation.None;
                rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            }

            var simBodies = new List<Rigidbody>();

            var clones = new GameObject[selection.Length];

            var animData = new List<AnimData>();
            for (int i = 0; i < selection.Length; ++i)
            {
                animData.Add(new AnimData(selection[i]));
                clones[i] = GameObject.Instantiate(selection[i]);
                clones[i].transform.position = selection[i].transform.position;
                clones[i].transform.rotation = selection[i].transform.rotation;
                clones[i].transform.localScale = selection[i].transform.lossyScale;
                selection[i].SetActive(false);
                var obj = clones[i];
                var colliders = obj.GetComponentsInChildren<Collider>();
                foreach (var collider in colliders) UnityEngine.Object.DestroyImmediate(collider);
                var rigidBodies = obj.GetComponentsInChildren<Rigidbody>();
                foreach (var body in rigidBodies) UnityEngine.Object.DestroyImmediate(body);
                var meshFilters = obj.GetComponentsInChildren<MeshFilter>();
                foreach (var meshFilter in meshFilters)
                {
                    var mesh = meshFilter.sharedMesh;
                    if (mesh == null) continue;
                    AddCollider(meshFilter.gameObject, mesh, simData);
                }
                var skinnedMeshRenderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var renderer in skinnedMeshRenderers)
                {
                    var mesh = renderer.sharedMesh;
                    if (mesh == null) continue;
                    var meshFilter = renderer.gameObject.AddComponent<MeshFilter>();
                    meshFilter.sharedMesh = mesh;
                    AddCollider(renderer.gameObject, mesh, simData);
                }
                var rigidBody = obj.AddComponent<Rigidbody>();
                if (simData.changeLayer) obj.layer = simData.tempLayer;
                simBodies.Add(rigidBody);
                rigidBody.useGravity = true;
                rigidBody.isKinematic = false;
            }

            Physics.autoSimulation = false;


            for (int i = 0; i < simData.maxIterations; ++i)
            {
                Physics.Simulate(Time.fixedDeltaTime);
                for (int objIdx = 0; objIdx < selection.Length; ++objIdx)
                {
                    var body = simBodies[objIdx];
                    if (body.velocity.sqrMagnitude > simData.maxSpeedSquared)
                        body.velocity = body.velocity.normalized * simData.maxSpeed;
                    if (body.angularVelocity.sqrMagnitude > simData.maxAngularSpeedSquared)
                        body.angularVelocity = body.angularVelocity.normalized * simData.maxAngularSpeed;
                    if (i % 10 == 0) animData[objIdx].poses.Add(new Pose(body.position, body.rotation));
                }

                if (simBodies.All(rb => rb.IsSleeping())) break;
            }
            Physics.autoSimulation = true;

            for (int i = 0; i < selection.Length; ++i)
            {
                selection[i].SetActive(true);
                selection[i].transform.position = clones[i].transform.position;
                selection[i].transform.rotation = clones[i].transform.rotation;
                animData[i].poses.Add(new Pose(selection[i].transform.position, selection[i].transform.rotation));
                UnityEngine.Object.DestroyImmediate(clones[i]);
            }

            foreach (var item in originalBodies)
            {
                if (item.body == null) continue;
                item.body.useGravity = item.useGravity;
                item.body.isKinematic = item.isKinematic;
                item.body.drag = item.drag;
                item.body.angularDrag = item.angularDrag;
                item.body.mass = item.mass;
                item.body.constraints = item.constraints;
                item.body.interpolation = item.interpolation;
                item.body.collisionDetectionMode = item.collisionDetectionMode;
            }

            Physics.gravity = originalGravity;
            Animate(animData, simData, recordAction);
        }
        private class AnimData
        {
            public GameObject obj = null;
            public List<Pose> poses = new List<Pose>();
            public List<Collider> enabledColliders = new List<Collider>();
            public List<GameObject> tempColliders = new List<GameObject>();
            public AnimData(GameObject obj) => this.obj = obj;
        }

        private static void Animate(List<AnimData> animData, SimulateGravityData simData, bool recordAction)
        {
            _stop = false;
            _isPlaying = true;
            foreach (var item in animData)
            {
                item.enabledColliders = item.obj.GetComponentsInChildren<Collider>()
                    .Where(collider => collider.enabled).ToList();
                item.tempColliders.Clear();
                var temp = new GameObject();
                temp.hideFlags = HideFlags.HideAndDontSave;
                var lastPose = item.poses.Last();
                temp.transform.position = lastPose.position;
                temp.transform.rotation = lastPose.rotation;
                temp.transform.localScale = item.obj.transform.lossyScale;

                var meshFilters = item.obj.GetComponentsInChildren<MeshFilter>();
                foreach (var meshFilter in meshFilters)
                {
                    var mesh = meshFilter.sharedMesh;
                    if (mesh == null) continue;
                    AddCollider(temp, mesh, simData);
                }
                var skinnedMeshRenderers = item.obj.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var renderer in skinnedMeshRenderers)
                {
                    var mesh = renderer.sharedMesh;
                    if (mesh == null) continue;
                    var meshFilter = renderer.gameObject.AddComponent<MeshFilter>();
                    meshFilter.sharedMesh = mesh;
                    AddCollider(temp, mesh, simData);
                }
                item.tempColliders.Add(temp);

                foreach (var collider in item.enabledColliders) collider.enabled = false;
            }
            Animate(animData, 0, recordAction);
        }


        private async static void Animate(List<AnimData> data, int frame, bool recordAction)
        {
            void EnableColliders(AnimData item)
            {
                foreach (var collider in item.enabledColliders) collider.enabled = true;
            }
            void DestroyTempColliders(AnimData item)
            {
                foreach(var temp in item.tempColliders) UnityEngine.Object.DestroyImmediate(temp);
            }
            void Record()
            {
                foreach (var item in data)
                {
                    if (item.obj == null) continue;
                    EnableColliders(item);
                    DestroyTempColliders(item);
                    item.obj.transform.position = item.poses.First().position;
                    item.obj.transform.rotation = item.poses.First().rotation;
                    Undo.RecordObject(item.obj.transform, "Simulate Gravity");
                    item.obj.transform.position = item.poses.Last().position;
                    item.obj.transform.rotation = item.poses.Last().rotation;
                }
            }
            var isPlaying = false;
            if (_stop)
            {
                if (recordAction) Record();
                return;
            }
            foreach (var item in data)
            {
                if (item.obj == null) break;
                if (frame >= item.poses.Count)
                {
                    item.obj.transform.position = item.poses.Last().position;
                    item.obj.transform.rotation = item.poses.Last().rotation;
                    continue;
                }
                isPlaying = true;
                item.obj.transform.position = item.poses[frame].position;
                item.obj.transform.rotation = item.poses[frame].rotation;
            }
            if (isPlaying)
            {
                await Task.Delay((int)(Time.fixedDeltaTime * 1000));
                Animate(data, frame + 1, recordAction);
            }
            else
            {
                if (recordAction) Record();
                else
                {
                    foreach (var item in data)
                    {
                        if (item.obj == null) continue;
                        EnableColliders(item);
                        DestroyTempColliders(item);
                    }
                }
                _isPlaying = false;
            }
        }


        [InitializeOnLoad]
        private static class UndoEventHandler
        {
            static UndoEventHandler() => Undo.undoRedoPerformed += OnUndoRedoPerformed;
            private static void OnUndoRedoPerformed()
            {
                _isPlaying = false;
                _stop = true;
            }
        }
    }
}