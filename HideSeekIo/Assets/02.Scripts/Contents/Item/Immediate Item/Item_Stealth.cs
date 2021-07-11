
using Photon.Pun;
public class Item_Stealth : Item_Base
{

    public override void Use(PlayerController usePlayer)
    {
        photonView.RPC("UseUseOnOtherClinets", RpcTarget.All, usePlayer.ViewID());
    }

    [PunRPC]
    public void UseUseOnOtherClinets(int useViewID)
    {
        var usePlayer = Managers.Game.GetLivingEntity(useViewID);
        if (usePlayer == null) return;

        EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, usePlayer.transform.position, 1);
        if (photonView.IsMine)
        {
            BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.Stealth, usePlayer);
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

}
