using System.Collections;

using Photon.Pun;
using System;

using UnityEngine;

public class Item_Vaccine: Item_Base
{
    protected override void Awake()
    {
        base.Awake();
        InGameItemType = Define.InGameItem.Vaccine;
    }

    public override Enum GetEnum()
    {
        return Define.InGameItem.Dynamite;
    }
    protected override void UsePorecess(PlayerController usePlayer)
    {
        //EffectManager.Instance.EffectOnLocal(Define.EffectType.BuffEffect, usePlayer.transform.position, 1);
        //if (photonView.IsMine)
        //{
        //    Destroy();


        //    var nuffControllerList = usePlayer.GetLivingEntity().BuffControllerList.FindAll(s => s.IsNuff == true); //부정적 버프리스트들을 갖고온다.

        //    foreach (var nuff in nuffControllerList)
        //    {
        //        nuff.End(); //부정적너프들 제거
        //    }

        //}

    }


}
