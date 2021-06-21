using System.Collections;

using UnityEngine;
using Photon.Pun;
public class Weapon_Stone : Weapon_Throw
{
    protected override  void Awake()
    {
        base.Awake();
        Setup("Throw", .2f, .2f, 5.0f, 1);
        weaponServerKey = Define.Weapon.Stone;
    }
}
