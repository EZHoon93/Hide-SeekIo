using System.Collections;

using UnityEngine;
using Photon.Pun;

public class Weapon_Grenade : Weapon_Throw
{
    //private void Start()
    //{
    //    Setup("Throw", .2f, .3f, 5f , 4);
    //}
    protected override void Awake()
    {
        base.Awake();
        Setup("Throw", .2f, .2f, 5.0f, 1);
        weaponServerKey = Define.Weapon.Grenade;
    }
}
