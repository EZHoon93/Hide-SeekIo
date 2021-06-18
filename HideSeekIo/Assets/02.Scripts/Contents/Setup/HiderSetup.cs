
using Photon.Pun;
using UnityEngine;


[RequireComponent(typeof(HiderInput))]
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
    }
}
