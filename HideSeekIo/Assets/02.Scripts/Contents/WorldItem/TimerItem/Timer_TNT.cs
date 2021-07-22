
using FoW;

using Photon.Pun;

using UnityEngine;
public class Timer_TNT : TimerItem
{
    int _viewID;

    FogOfWarUnit _fogOfWarUnit;
    [SerializeField] ParticleSystem _effect;

    private void Awake()
    {
        _fogOfWarUnit = GetComponent<FogOfWarUnit>();
    }

    private void OnEnable()
    {
        _effect.Play();
    }
    public override void EndTime()
    {
        
    }

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var userViewID = (int)info.photonView.InstantiationData[0];
        _viewID = userViewID;
        _fogOfWarUnit.team = userViewID;
        if (PhotonNetwork.Time - info.SentServerTime < 1)
        {
            EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, this.transform.position, 0);
        }
    }
  
}
