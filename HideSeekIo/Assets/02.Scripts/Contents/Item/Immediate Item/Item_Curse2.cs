using System.Collections;

using Photon.Pun;

using UnityEngine;

public class Item_Curse2 : Item_Base
{

    public override void Use(PlayerController usePlayer)
    {
        photonView.RPC("UseUseOnOtherClinets", RpcTarget.All, usePlayer.ViewID());
    }

    [PunRPC]
    public void UseUseOnOtherClinets(int useViewID)
    {
        var usePlayer = Managers.Game.GetLivingEntity(useViewID);
        if (usePlayer == null) return;

        EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, usePlayer.transform.position, 1);
        if (photonView.IsMine)
        {
            BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.Speed, usePlayer);
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

}
