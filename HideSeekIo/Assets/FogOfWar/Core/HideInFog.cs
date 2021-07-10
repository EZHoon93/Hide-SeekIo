using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
namespace FoW
{
    [AddComponentMenu("FogOfWar/HideInFog")]
    public class HideInFog : MonoBehaviour
    {
        public int team = 0;

        [Range(0.0f, 1.0f)]
        public float minFogStrength = 0.2f;

        Transform _transform;
        Canvas _canvas;
        [SerializeField] List<Renderer> _renderers;

      
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

        public void AddRenderer(Renderer renderer)
        {
            _renderers.Add(renderer);
        }

        public void RemoveRenderer(Renderer renderer)
        {
            _renderers.Remove(renderer);
        }
      
    }
}
