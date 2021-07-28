using System;

using Photon.Pun;

public abstract class Item_Weapon : Item_Base
{
    protected Weapon_Throw _weapon_Throw;

    protected Define.Weapon _weaponType;

    protected override void Awake()
    {
        State = UseState.Local;
        useType = UseType.Weapon;
        SetupWeaponType();
    }

    protected abstract void SetupWeaponType();

    public override void OnPhotonInstantiate(PlayerController hasPlayerController)
    {
        //다른 무기오브젝트가 설정된상태였다면.. 
        if(_weapon_Throw != null)
        {
            Managers.Resource.Destroy(_weapon_Throw.gameObject);
        }
        if (_weapon_Throw == null)
        {
            print("생성!!");
            //_weapon_Throw = Managers.Spawn.WeaponSpawn(_weaponType, hasPlayerController.GetAttackBase()).GetComponent<Weapon_Throw>();
            //_weapon_Throw.AttackSucessEvent += () => Destroy();

        }
        hasPlayerController.GetAttackBase().UseWeapon(_weapon_Throw);

    }

    public override void Use(PlayerController usePlayer)
    {
        if (_weapon_Throw == null)
        {
            //_weapon_Throw = Managers.Spawn.WeaponSpawn(_weaponType, usePlayer.GetAttackBase() ).GetComponent<Weapon_Throw>();
            _weapon_Throw.AttackSucessEvent += () => Destroy();

        }
        usePlayer.GetAttackBase().UseWeapon(_weapon_Throw);
    }

}




