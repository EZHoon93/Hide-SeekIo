
using UnityEngine;
using Photon.Pun;
using FoW;

public class Timer_Dynamite : TimerItem
{
    
    [SerializeField] GameObject _dmageUI;
    float _damageRange = 2;
    int _damage = 5;

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);
        
        
    }
    public override void EndTime()
    {
        EffectManager.Instance.EffectOnLocal(Define.EffectType.GrenadeEffect, this.transform.position, 0);
        UtillGame.BuffInRange(this.transform, _damageRange, Define.BuffType.B_Stun, _timerItemController.usePlayer.ViewID(), UtillLayer.seekerToHiderAttack);

    }


}
