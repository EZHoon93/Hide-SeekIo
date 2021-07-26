﻿
using UnityEngine;
public class FlashProjectile : ThrowProjectileObject
{
    [SerializeField] float _range;

    public override void Play(AttackBase attackPlayer, Vector3 startPoint, Vector3 endPoint)
    {
        base.Play(attackPlayer, startPoint, endPoint);
        _modelObject.SetActive(true);
    }
    protected override void Explosion()
    {
        _modelObject.SetActive(false);
        EffectManager.Instance.EffectOnLocal(Define.EffectType.FlashEffect, this.transform.position, 0);
        _fogOfWarUnit.enabled = true;
        UtillGame.BuffInRange(this.transform, _range, Define.BuffType.B_Stun, attackPlayer.ViewID(), UtillLayer.seekerToHiderAttack);

        Invoke("Push", 1.0f);
    }
}
