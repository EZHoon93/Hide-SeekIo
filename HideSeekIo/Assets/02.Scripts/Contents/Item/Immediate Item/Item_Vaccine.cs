using System.Collections;

using Photon.Pun;

using UnityEngine;

public class Item_Vaccine: Item_Base
{

    public override void Use(PlayerController usePlayer)
    {
        photonView.RPC("UseUseOnOtherClinets", RpcTarget.All, usePlayer.ViewID());
    }
    [PunRPC]
    public  void UseUseOnOtherClinets(int useViewID)
    {
        var usePlayer = Managers.Game.GetLivingEntity(useViewID);
        if (usePlayer == null) return;

        EffectManager.Instance.EffectOnLocal(Define.EffectType.BuffEffect, usePlayer.transform.position, 1);
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);

             var nuffControllerList =  usePlayer.BuffControllerList.FindAll(s => s.IsNuff == true); //부정적 버프리스트들을 갖고온다.

            foreach(var nuff in nuffControllerList)
            {
                nuff.End(); //부정적너프들 제거
            }

        }
    }

}
