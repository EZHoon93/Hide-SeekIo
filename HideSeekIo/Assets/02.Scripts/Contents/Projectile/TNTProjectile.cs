
using Photon.Pun;

using UnityEngine;

public class TNTProjectile : ThrowProjectileObject
{

    
    protected override void Explosion()
    {
        EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, this.transform.position + new Vector3(0, 0.5f, 0), 0);
        if (attackPlayer == null) return;
        if (attackPlayer.photonView.IsMine)
        {
            //AI플레이어는 ..
            if (attackPlayer.gameObject.IsValidAI())
            {
                PhotonNetwork.InstantiateRoomObject("TimerItem/T_TNT", this.transform.position, this.transform.rotation, 0,
                new object[] { attackPlayer.ViewID(), 10.0f });
            }
            else
            {
                PhotonNetwork.Instantiate("TimerItem/T_TNT", this.transform.position, this.transform.rotation, 0,
                new object[] { attackPlayer.ViewID(), 10.0f });
            }
        }
        _modelObject.SetActive(false);
        Push();

    }
}
