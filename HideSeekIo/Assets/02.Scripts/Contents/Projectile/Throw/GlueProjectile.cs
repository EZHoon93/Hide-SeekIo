
using Photon.Pun;

using UnityEngine;

public class GlueProjectile : ThrowProjectileObject
{

    protected override void Explosion()
    {
        Managers.effectManager.EffectOnLocal(Define.EffectType.AcidExp, this.transform.position + new Vector3(0, 0.5f, 0), 0);

        PhotonNetwork.InstantiateRoomObject("TimerItem/T_Glue", this.transform.position, this.transform.rotation, 0,
       new object[] { _attackViewID, 10.0f });

        //_modelObject.SetActive(false);
        Invoke("Push", 1.0f);
    }



}
