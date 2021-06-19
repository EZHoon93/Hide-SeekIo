
using FoW;

using Photon.Pun;

using UnityEngine;


[RequireComponent(typeof(HiderController))]
[RequireComponent(typeof(HiderMove))]
[RequireComponent(typeof(HiderHealth))]
[RequireComponent(typeof(Poolable))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(CharacterController))]


public class HiderSetup : PlayerSetup
{

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);
        GetComponentInChildren<FogOfWarTeam>().team = this.ViewID();
        GetComponentInChildren<FogOfWarUnit>().team = this.ViewID();

    }

    protected override void LayerChange(GameObject gameObject)
    {
        Util.SetLayerRecursively(gameObject,  (int)Define.Layer.Hider);
    }
}
