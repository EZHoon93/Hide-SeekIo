using System.Collections;
using UnityEngine;
using Photon.Pun;

public class HiderAttack : AttackBase
{




    protected override void Awake()
    {
        base.Awake();
    }
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        if (this.photonView.IsMine)
        {
            //Managers.Spawn.WeaponSpawn(Define.Weapon.Stone, this);
        }
        Managers.Spawn.WeaponSpawn(Define.Weapon.Grenade, this.GetComponent<AttackBase>());

    }




}
