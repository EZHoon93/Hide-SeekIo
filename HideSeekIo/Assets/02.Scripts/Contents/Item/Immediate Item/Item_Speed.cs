using System.Collections;

using Photon.Pun;

using UnityEngine;

public class Item_Speed : Item_Base
{
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
