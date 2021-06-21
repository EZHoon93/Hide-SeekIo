using System.Collections;

using UnityEngine;

public class Weapon_Sniper : Weapon_Gun
{
    protected override void Awake()
    {
        base.Awake();
        weaponServerKey = Define.Weapon.Sniper;
    }
    protected override void Start()
    {
        base.Start();
    }
}
