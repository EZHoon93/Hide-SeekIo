using System.Collections;

using Photon.Pun;
using System;

using UnityEngine;

public class InstantItem_Speed : InstantItem_Base
{
    public override Define.InGameItem InGameItemType => Define.InGameItem.Speed;


    public override void UseEffect()
    {
        //Managers.effectManager.EffectOnLocal(Define.EffectType.CloudBurst, hasPlayerController.transform.position, 1);
    }

    protected override void UsePorecess(PlayerController usePlayer)
    {
        if (photonView.IsMine)
        {
            //BuffManager.Instance.CheckBuffController(usePlayer.playerHealth, Define.BuffType.B_Speed);
        }
    }


}
