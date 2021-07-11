using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
namespace FoW
{
    [AddComponentMenu("FogOfWar/HideInFog")]
    public class HideInFog : MonoBehaviour
    {
        public int team = 0;
        bool _isTransParent;
        [Range(0.0f, 1.0f)]
        public float minFogStrength = 0.2f;

        Transform _transform;
        Canvas _canvas;

        [SerializeField] List<Renderer> _renderers;

        Renderer characterRenderer;
        Renderer WeaponRenderer;

        private void Reset()
        {
            minFogStrength = 0.8f;
            _renderers = GetComponentsInChildren<Renderer>().ToList();
        }
        private void OnEnable()
        {
            if (CameraManager.Instance.Target)
            {
                team = CameraManager.Instance.Target.ViewID();
            }
            CameraManager.Instance.fogChangeEvent += ChangeCameraTarget;
        }

        private void OnDisable()
        {
            if (CameraManager.Instance.Target == null) return;

            CameraManager.Instance.fogChangeEvent -= ChangeCameraTarget;
        }
        void Start()
        {
            _transform = transform;
            _canvas = GetComponentInChildren<Canvas>();

        }

        void Update()
        {
            FogOfWarTeam fow = FogOfWarTeam.GetTeam(team);
            if (fow == null)
            {
                Debug.LogWarning("There is no Fog Of War team for team #" + team.ToString());
                return;
            }

            bool visible = fow.GetFogValue(_transform.position) < minFogStrength * 255;

            if (_isTransParent)
            {
                visible = false;
            }

            if (_canvas != null)
                _canvas.enabled = visible;

            if (_renderers.Count > 0)
            {
                foreach (var r in _renderers)
                {
                    r.enabled = visible;
                }

            }


        }

        void ChangeCameraTarget(int cameraViewID)
        {
            team = cameraViewID;
        }
        public void AddRenderer(Renderer renderer)
        {
            _renderers.Add(renderer);
        }

        public void RemoveRenderer(Renderer renderer)
        {
            _renderers.Remove(renderer);
        }

        public void ChangeTransParent(bool isTransParent)
        {
            _isTransParent = isTransParent;
            if (isTransParent)
            {
                if (this.enabled) return;
                foreach (var r in _renderers)
                {
                    Color color = r.material.color;
                    color.a = 0.5f;
                    r.material.color = color;
                }
            }
            else
            {
                foreach (var r in _renderers)
                {
                    Color color = r.material.color;
                    color.a = 1;
                    r.material.color = color;
                }
            }
        }

    }
}
