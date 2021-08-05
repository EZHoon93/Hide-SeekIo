using System.Collections;

using Photon.Pun;
using System;

using UnityEngine;

public class Item_Trap : Item_Base
{
    public override Enum GetEnum()
    {
        return Define.InGameItem.Dynamite;
    }
    protected override void Awake()
    {
        base.Awake();
        InGameItemType = Define.InGameItem.Trap;
    }

    protected override void UsePorecess(PlayerController usePlayer)
    {
        if (photonView.IsMine)
        {
            EffectManager.Instance.EffectToServer(Define.EffectType.CloudBurst, usePlayer.transform.position, 0);
            Managers.Spawn.WorldItemSpawn(Define.WorldItem.Trap, usePlayer.transform.position);
            Destroy();

        }
    }

}
