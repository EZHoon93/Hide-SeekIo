using System.Collections;

using UnityEngine;
using Photon.Pun;
public class Weapon_TNT : Weapon_Throw
{
    public override System.Enum GetEnum() => Define.ThrowItem.TNT;

    protected override void Awake()
    {
        base.Awake();
        type = Type.Disposable;
        throwType = Define.ThrowItem.TNT;

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
}
