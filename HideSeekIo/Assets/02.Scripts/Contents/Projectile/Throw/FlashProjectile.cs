
using UnityEngine;
public class FlashProjectile : ThrowProjectileObject
{
    [SerializeField] float _range;

    protected override void Explosion()
    {
        _modelObject.SetActive(false);
        EffectManager.Instance.EffectOnLocal(Define.EffectType.FlashEffect, this.transform.position, 0,_range);
        _fogOfWarUnit.enabled = true;
        LayerMask attackLayer = _attackPlayer.Team == Define.Team.Hide ? UtillLayer.hiderToSeekerAttack : UtillLayer.seekerToHiderAttack;
        UtillGame.BuffInRange(this.transform, _range, Define.BuffType.B_SightCurse, _attackViewID, attackLayer);

        Invoke("Push", 1.0f);
    }
}
