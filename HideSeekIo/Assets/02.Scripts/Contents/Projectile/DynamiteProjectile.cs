using System.Collections;

using UnityEngine;

public class DynamiteProjectile : ThrowProjectileObject
{

    public override void Play(Vector3 startPoint, Vector3 endPoint)
    {
        base.Play(startPoint, endPoint);
        _modelObject.SetActive(true);
    }
    protected override void Explosion()
    {
        _modelObject.SetActive(false);
        EffectManager.Instance.EffectOnLocal(Define.EffectType.GrenadeEffect, this.transform.position, 0);
    }
}
