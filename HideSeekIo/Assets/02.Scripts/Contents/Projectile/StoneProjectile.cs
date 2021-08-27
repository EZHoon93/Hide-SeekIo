using System.Collections;

using UnityEngine;

public class StoneProjectile : ThrowProjectileObject
{


    //public override void Play(AttackBase attackPlayer, Vector3 startPoint, Vector3 endPoint)
    //{
    //    base.Play(attackPlayer, startPoint, endPoint);
    //    _modelObject.SetActive(true);
    //}
    //protected override void Explosion()
    //{
    //    _modelObject.SetActive(false);
    //    //EffectManager.Instance.EffectOnLocal(Define.EffectType.Dust, this.transform.position, 0);
    //    if (CameraManager.Instance.Target == null)
    //    {
    //        EffectManager.Instance.EffectOnLocal(Define.EffectType.Dust, this.transform.position, 0);
    //    }
    //    else if (CameraManager.Instance.Target.Team == Define.Team.Hide)
    //    {
    //        EffectManager.Instance.EffectOnLocal(Define.EffectType.Dust, this.transform.position, 0);
    //    }
    //    else
    //    {
    //        if (Vector3.Distance(this.transform.position, CameraManager.Instance.Target.transform.position) <= 4)
    //        {
    //            EffectManager.Instance.EffectOnLocal(Define.EffectType.Ripple, this.transform.position, 0);
    //        }
    //    }
    //}

}
