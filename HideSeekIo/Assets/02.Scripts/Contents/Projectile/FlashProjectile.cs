using System.Collections;

using UnityEngine;
public class FlashProjectile : ThrowProjectileObject
{
    [SerializeField] float _range;

    public override void Play(int useViewID , Vector3 startPoint, Vector3 endPoint)
    {
        base.Play(useViewID, startPoint, endPoint);
        _modelObject.SetActive(true);
    }
    protected override void Explosion()
    {
        _modelObject.SetActive(false);
        EffectManager.Instance.EffectOnLocal(Define.EffectType.GrenadeEffect, this.transform.position, 0);
        _fogOfWarUnit.enabled = true;
        UtillGame.BuffInRange(this.transform, _range, Define.BuffType.Stun, _useViewID, UtillLayer.seekerToHiderAttack);

        Invoke("Push", 1.0f);
    }
}
