
using FoW;

using Photon.Pun;

using UnityEngine;
public class Timer_TNT : TimerItem
{

    [SerializeField] ParticleSystem _effect;
    [SerializeField] float _damageRange;
    [SerializeField] int _damage = 10;

    public override void EndTime()
    {
        //UtillGame.DamageInRange(this.transform, _damageRange, _damage, _timerItemController.usePlayer.ViewID(), UtillLayer.seekerToHiderAttack);
        Managers.effectManager.EffectOnLocal(Define.EffectType.GrenadeEffect, this.transform.position, 0);
            
    }

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);
        print("TNT Effect Play");
        _effect.Play();
    }

}
