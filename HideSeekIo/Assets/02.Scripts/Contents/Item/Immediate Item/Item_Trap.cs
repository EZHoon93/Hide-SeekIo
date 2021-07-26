using System.Collections;

using Photon.Pun;

using UnityEngine;

public class Item_Trap : Item_Base
{
    protected override void Awake()
    {
        State = UseState.Local;
    }
    public override void Use(PlayerController usePlayer)
    {
        if (photonView.IsMine)
        {
            EffectManager.Instance.EffectToServer(Define.EffectType.CloudBurst, usePlayer.transform.position, 0);
            Managers.Spawn.WorldItemSpawn(Define.WorldItem.Trap, usePlayer.transform.position);
            Destroy();

        }
    }

}
