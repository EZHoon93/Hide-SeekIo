
using UnityEngine;
using FoW;
using Photon.Pun;
[RequireComponent(typeof(SeekerInput))]
[RequireComponent(typeof(SeekerController))]
[RequireComponent(typeof(SeekerMove))]
[RequireComponent(typeof(SeekerAttack))]

[RequireComponent(typeof(Poolable))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(CharacterController))]

public class SeekerSetup : PlayerSetup
{
    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);
        //var seekerController =  GetComponent<SeekerController>();
        //var weaponId = (string)info.photonView.InstantiationData[2]; //무기 스킨
        GetComponentInChildren<FogOfWarTeam>().team = this.ViewID();
        GetComponentInChildren<FogOfWarUnit>().team = this.ViewID();

    }

    protected override void LayerChange(GameObject gameObject)
    {
        Util.SetLayerRecursively(gameObject, (int)Define.Layer.Seeker);
    }
}
