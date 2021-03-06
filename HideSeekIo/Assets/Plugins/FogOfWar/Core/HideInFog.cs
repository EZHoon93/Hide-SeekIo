using System;
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
        [Range(0.0f, 1.0f)]
        public float minFogStrength = 0.2f;

        Transform _transform;
        [SerializeField] Canvas _canvas1;

        [SerializeField] List<Renderer> _renderers = new List<Renderer>();


        public bool isInGrass { get; set; } = false;
        public bool isGrassDetected { get; set; } = false;  //부쉬안에있는걸 들킨 리모트 오브젝트
        public bool istransSkill { get; set; } = false;

        public bool isAttack { get; set; } = false;


        private void Reset()
        {
            minFogStrength = 0.8f;
            _renderers = GetComponentsInChildren<Renderer>().ToList();

            
        }
        
        //private void OnEnable()
        //{
        //    if (Managers.cameraManager.cameraTagerPlayer)
        //    {
        //        team = Managers.cameraManager.cameraTagerPlayer.ViewID();
        //    }
        //    Managers.cameraManager.fogChangeEvent += ChangeCameraTarget;
        //    SetActiveRender(false);
        //    isAttack = false;
        //    isInGrass = false;
        //    isGrassDetected = false;
        //    istransSkill = false;
        //}

        //private void OnDisable()
        //{
        //    if (Managers.Game == null) return;
        //    if (Managers.cameraManager == null) return;
        //    Managers.cameraManager.fogChangeEvent -= ChangeCameraTarget;
        //}
        void Start()
        {
            _transform = transform;
        }

        public void UpdateInFog()
        {
            FogOfWarTeam fow = FogOfWarTeam.GetTeam(team);
            if (fow == null)
            {
                Debug.LogWarning("There is no Fog Of War team for team #" + team.ToString());
                return;
            }

            bool visible = fow.GetFogValue(_transform.position) < minFogStrength * 255;

            

            SetActiveRender(visible);


        }

        void ChangeCameraTarget(int cameraViewID)
        {
            team = cameraViewID;
            
        }

        public void ClearRenders()
        {
            _renderers.Clear();
        }
        //public void AddRenderer(RenderController renderController)
        //{
        //    foreach(var r in renderController.renderers)
        //    {
        //        if(_renderers.Contains(r) == false)
        //        {
        //            _renderers.Add(r);
        //        }
        //    }
        //}

        //public void RemoveRenderer(RenderController renderController)
        //{
        //    foreach (var r in renderController.renderers)
        //    {
        //        if (_renderers.Contains(r) )
        //        {
        //            _renderers.Remove(r);
        //        }
        //    }
        //}

        public void SetActiveRender(bool visible)
        {

            if (_canvas1 != null)
                _canvas1.enabled = visible;

            if (_renderers.Count > 0)
            {
                foreach (var r in _renderers)
                {
                    r.enabled = visible;
                }

            }
        }


        //public void ChangeTransParentBySkill(bool isTransParent , Define.Team team)
        //{
        //    istransSkill = isTransParent;
        //    if (istransSkill)
        //    {
        //        //if (this.enabled) return;
        //        foreach (var r in _renderers)
        //        {
        //            Color color = r.material.color;
                   
        //            if (Managers.cameraManager.cameraTagerPlayer)
        //            {
        //                var viewTeam = Managers.cameraManager.cameraTagerPlayer.Team;
        //                if (viewTeam == team)
        //                {
        //                    if (color != null)
        //                    {
        //                        color.a = 0.5f;
        //                        r.material.color = color;
        //                    }
        //                }
        //                else
        //                {
        //                    if (color != null)
        //                    {
        //                        color.a = 0.0f;
        //                        r.material.color = color;
        //                    }
        //                }   
        //            }
        //            //if (team == Define.Team.Hide)
        //            //{
        //            //    r.gameObject.layer = (int)Define.Layer.TransparentHider;
        //            //}
        //            //else
        //            //{
        //            //    r.gameObject.layer = (int)Define.Layer.SeekerTransCollider;
        //            //}
        //        }
        //    }
        //    else
        //    {
        //        foreach (var r in _renderers)
        //        {
        //            Color color = r.material.color;
        //            if (color != null)
        //            {
        //                color.a = 1;
        //                r.material.color = color;
        //            }
        //            //if (team == Define.Team.Hide)
        //            //{
        //            //    r.gameObject.layer = (int)Define.Layer.Hider;
        //            //}
        //            //else
        //            //{
        //            //    r.gameObject.layer = (int)Define.Layer.Seeker;
        //            //}
        //        }
        //    }
        //}

    }
}
