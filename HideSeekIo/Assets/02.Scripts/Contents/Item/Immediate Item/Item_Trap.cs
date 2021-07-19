using System.Collections;

using Photon.Pun;

using UnityEngine;

public class Item_Trap : Item_Base
{
    protected override void Start()
    {
        State = UseState.Local;
    }
    public override void Use(PlayerController usePlayer)
    {
        EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, usePlayer.transform.position, 1);
        if (photonView.IsMine)
        {
            Managers.Spawn.WorldItemSpawn(Define.WorldItem.Trap, usePlayer.transform.position);
            Destroy();

        }
    }

}
