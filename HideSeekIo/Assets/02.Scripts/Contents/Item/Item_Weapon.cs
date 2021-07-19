using System;

using Photon.Pun;

public abstract class Item_Weapon : Item_Base
{
    protected Weapon_Throw _weapon_Throw;

    protected Define.Weapon _weaponType;

    protected override void Start()
    {
        State = UseState.Local;
        useType = UseType.Weapon;
        SetupWeaponType();
    }

    protected abstract void SetupWeaponType();

    public override void OnPhotonInstantiate()
    {
        if(_weapon_Throw != null)
        {
            Managers.Resource.Destroy(_weapon_Throw.gameObject);
        }
    }

    public override void Use(PlayerController usePlayer)
    {
        if (_weapon_Throw == null)
        {
            _weapon_Throw = Managers.Spawn.WeaponSpawn(_weaponType, usePlayer.GetComponent<AttackBase>()).GetComponent<Weapon_Throw>();
            _weapon_Throw.AttackSucessEvent += () => Destroy();

        }
        _weapon_Throw.UseToPlayerToServer();
    }

}




