using System.Collections;

using UnityEngine;
using Photon.Pun;
public class Weapon_Stone : Weapon_Throw
{
    public override System.Enum GetEnum() => Define.ThrowItem.Stone;

    protected override  void Awake()
    {
        base.Awake();
        type = Type.Permanent;
        throwType = Define.ThrowItem.Stone;

    }
    private void Start()
    {
        Setup("Throw", .2f, .3f, 6.0f, 1);
    }

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);
        //attackPlayer.UseWeapon(this);    //무기 사용상태로 전환
    }

    public override void Use(PlayerController usePlayerController)
    {
        
    }
}
