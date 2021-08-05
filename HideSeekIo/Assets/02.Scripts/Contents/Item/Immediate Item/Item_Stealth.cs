
using System;
using Photon.Pun;
public class Item_Stealth : Item_Base
{
    protected override void Awake()
    {
        base.Awake();
        InGameItemType = Define.InGameItem.Stealth;
    }

    public override Enum GetEnum()
    {
        return Define.InGameItem.Dynamite;
    }
    protected override void UsePorecess(PlayerController usePlayer)
    {
        EffectManager.Instance.EffectOnLocal(Define.EffectType.DarkExp, usePlayer.transform.position, 1);
        if (photonView.IsMine)
        {
            BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.B_Stealth, usePlayer.GetLivingEntity());
            Destroy();

        }
    }


}
