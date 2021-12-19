using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Instant_OverSee : InstantItem_Base
{


    public override void UseEffect()
    {
        Managers.effectManager.EffectOnLocal(Define.EffectType.CloudBurst, hasPlayerController.transform.position, 1);
    }

    protected override void UsePorecess(PlayerController usePlayer)
    {
        if (photonView.IsMine)
        {
            //BuffManager.Instance.CheckBuffController(usePlayer.playerHealth, Define.BuffType.B_OverSee);
        }
    }
}
