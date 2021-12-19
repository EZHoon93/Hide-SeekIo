using System.Collections;

using UnityEngine;

public class UI_Select_Weapon : UI_Select_Base
{
    [SerializeField]
    Define.Weapon _weaponType;
    public Define.Weapon GetWeaponType() => _weaponType;

    protected override void Click()
    {
        base.Click();
        //var playerController = Managers.Game.myPlayer;
        //if (!playerController) return;
        //Managers.Spawn.WeaponSpawn(_weaponType, playerController);
        //Managers.Spawn.WeaponSpawn(Define.Weapon.PoisonBomb, playerController);
    }
}
