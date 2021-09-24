using System.Collections;

using UnityEngine;

public class GrenadeProjectile : ThrowProjectileObject
{
    int _attackLayer = (1 << (int)Define.Layer.Seeker);
    [SerializeField] float _range = 2;



    protected override void Explosion()
    {
        _modelObject.SetActive(false);
        _fogOfWarUnit.enabled = true;
        _fogOfWarUnit.team = _attackViewID;
        EffectManager.Instance.EffectOnLocal(Define.EffectType.GrenadeEffect, this.transform.position, 0,_range);
        LayerMask attackLayer = _attackPlayer.Team == Define.Team.Hide ? UtillLayer.hiderToSeekerAttack : UtillLayer.seekerToHiderAttack;

        UtillGame.BuffInRange(this.transform, _range, Define.BuffType.B_Stun, _attackViewID, _attackLayer);
        Invoke("Push", 1.0f);
    }
}
