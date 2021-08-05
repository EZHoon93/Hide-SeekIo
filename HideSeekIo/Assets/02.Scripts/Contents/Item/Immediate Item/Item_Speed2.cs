using System.Collections;

using Photon.Pun;
using System;

using UnityEngine;

public class Item_Speed2 : Item_Base
{
    protected override void Awake()
    {
        base.Awake();
        InGameItemType = Define.InGameItem.Speed2;
    }

    public override Enum GetEnum()
    {
        return Define.InGameItem.Dynamite;
    }
    protected override void UsePorecess(PlayerController usePlayer)
    {

        EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, usePlayer.transform.position, 1);
        if (photonView.IsMine)
        {
            BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.B_Speed, usePlayer.GetLivingEntity());
            Destroy();

        }
    }


}
