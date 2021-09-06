using System.Collections;

using UnityEngine;

public class StoneProjectile : BulletProjectile
{

    public override void Enter(GameObject Gettingobject)
    {
        if (isPlay == false) return;
        var livingEntity = Gettingobject.GetComponent<LivingEntity>();
        if (livingEntity != null)
        {
            Expolosion();
            if (livingEntity.photonView.IsMine == false) return;
            BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.B_Stun, livingEntity);
        }
    }
    protected override void Expolosion()
    {
        base.Expolosion();
        EffectManager.Instance.EffectOnLocal(Define.EffectType.Dust, this.transform.position, 0);
    }

}
