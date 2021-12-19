
using UnityEngine;
using Photon.Pun;
using FoW;

public class Timer_Dynamite : TimerItem
{
    
    [SerializeField] GameObject _dmageUI;
    float _damageRange = 1.5f;
    int _damage = 5;

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);
        
        
    }
    public override void EndTime()
    {
        Managers.effectManager.EffectOnLocal(Define.EffectType.GrenadeEffect, this.transform.position, 0,_timerItemController.usePlayer.ViewID());
        //Managers.effectManager.EffectOnLocal(Define.EffectType.FogSight, this.transform.position, 0);
        //UtillGame.BuffInRange(this.transform, _damageRange, Define.BuffType.B_Stun, _timerItemController.usePlayer.ViewID(), UtillLayer.seekerToHiderAttack);

    }


}
