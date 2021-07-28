using System.Collections;
using UnityEngine;
using Photon.Pun;

public class HiderAttack : AttackBase
{


    HiderInput _hiderInput;

    public override InputBase GetInputBase() => _hiderInput;

    private void Awake()
    {
        _hiderInput = GetComponent<HiderInput>();
    }
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        if (this.IsMyCharacter())
        {
            Managers.Spawn.WeaponSpawn(Define.Weapon.Stone, this, true);
        }
    }
    

    

}
