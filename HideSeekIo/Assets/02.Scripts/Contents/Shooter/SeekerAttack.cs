﻿using System.Collections;

using UnityEngine;
using Photon.Pun;
public class SeekerAttack : AttackBase
{


    protected override void Awake()
    {
        base.Awake();
    }
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        if (this.IsMyCharacter())
        {
            Managers.Spawn.WeaponSpawn(Define.Weapon.Melee2, this );
            Managers.Spawn.WeaponSpawn(Define.Weapon.Flash, this);
        }
    }

    

    //public void OnUpdate()
    //{
    //    if (this.photonView.IsMine == false || currentWeapon == null) return;
    //    UpdateAttackCoolTime();
    //    UpdateAttack(_seekerInput.LastAttackVector);
    //}

    //public override void UseWeapon(Weapon newWeapon)
    //{
    //    if (currentWeapon != null)
    //    {
    //        currentWeapon.useState = Weapon.UseState.NoUse;
    //    }
    //    currentWeapon = newWeapon;
    //    currentWeapon.AttackSucessEvent -= AttackSucess;
    //    currentWeapon.AttackSucessEvent += AttackSucess;
    //    currentWeapon.AttackEndEvent -= AttackEnd;
    //    currentWeapon.AttackEndEvent += AttackEnd;
    //    currentWeapon.useState = Weapon.UseState.Use;
    //    if (currentWeapon.type == Weapon.Type.Permanent)
    //    {
    //        baseWeapon = currentWeapon;
    //    }
    //    else
    //    {
    //        skillWeapon = newWeapon;

    //    }
    //    SetupAnimation();
    //}



    //public void Update()
    //{
    //    OnUpdate();
    //}

    //private void LateUpdate()
    //{
    //    if (this.IsMyCharacter() == false) return;
    //    if()
    //    UpdateZoom(_seekerInput.AttackVector);
    //}

}
