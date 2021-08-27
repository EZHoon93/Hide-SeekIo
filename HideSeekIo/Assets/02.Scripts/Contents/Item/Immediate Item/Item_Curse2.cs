using System.Collections;

using Photon.Pun;
using System;

using UnityEngine;

public class Item_Curse2 : Item_Base
{

    public override Enum GetEnum()
    {
        return Define.InGameItem.Dynamite;
    }
    protected override void UsePorecess(PlayerController usePlayer)
    {

        //EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, usePlayer.transform.position, 1);
        //if (photonView.IsMine)
        //{
        //    BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.B_Speed, usePlayer.GetLivingEntity());
        //    Destroy();

        //}
    }

}
