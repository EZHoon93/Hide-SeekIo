using System.Collections;

using UnityEngine;

public class StoneProjectile : ThrowProjectileObject
{


    public override void Play(int useViewID, Vector3 startPoint, Vector3 endPoint)
    {
        base.Play(useViewID , startPoint, endPoint);
        _modelObject.SetActive(true);
    }
    protected override void Explosion()
    {
        _modelObject.SetActive(false);
        EffectManager.Instance.EffectOnLocal(Define.EffectType.Dust, this.transform.position, 0);
    }

}
