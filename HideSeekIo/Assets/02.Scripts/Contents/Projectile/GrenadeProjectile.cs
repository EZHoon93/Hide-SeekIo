﻿using System.Collections;

using UnityEngine;

public class GrenadeProjectile : ThrowProjectileObject
{
    int _attackLayer = (1 << (int)Define.Layer.Seeker);
    [SerializeField] float _range = 2;
    public override void Play(AttackBase attackPlayer, Vector3 startPoint, Vector3 endPoint)
    {
        base.Play(attackPlayer, startPoint, endPoint);
        _modelObject.SetActive(true);
    }
    protected override void Explosion()
    {
        _modelObject.SetActive(false);
        EffectManager.Instance.EffectOnLocal(Define.EffectType.GrenadeEffect, this.transform.position, 0 );

        UtillGame.BuffInRange(this.transform, _range, Define.BuffType.B_Stun  ,attackPlayer.ViewID(), UtillLayer.hiderToSeekerAttack);

    }
}
