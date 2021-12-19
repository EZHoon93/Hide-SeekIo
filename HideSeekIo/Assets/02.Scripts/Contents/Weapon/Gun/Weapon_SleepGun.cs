using System.Collections;

using UnityEngine;
using Photon.Pun;

public class Weapon_SleepGun : Weapon_Gun
{
    private void Start()
    {
        AttackAnim = "Attack";
        AttackDelay = 0.25f;
        AfaterAttackDelay = 0.25f;
        AttackDistance = 5;
        inputControllerObject.InitCoolTime = 0;
        _damage = 10;
    }


}
