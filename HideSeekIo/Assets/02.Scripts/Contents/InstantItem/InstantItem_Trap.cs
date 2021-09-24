using System.Collections;

using Photon.Pun;
using System;

using UnityEngine;

public class InstantItem_Trap : InstantItem_Base
{
    public override Define.InGameItem InGameItemType => Define.InGameItem.Speed;


    public override void UseEffect()
    {
        EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, hasPlayerController.transform.position, 1);
    }

    protected override void UsePorecess(PlayerController usePlayer)
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.InstantiateRoomObject("Trap", usePlayer.transform.position, Quaternion.identity);
        }
    }


}
