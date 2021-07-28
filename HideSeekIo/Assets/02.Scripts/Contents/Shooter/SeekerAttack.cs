using System.Collections;

using UnityEngine;
using Photon.Pun;
public class SeekerAttack : AttackBase
{

    SeekerInput _seekerInput;
    public override InputBase GetInputBase() => _seekerInput;

    private void Awake()
    {
        _seekerInput = GetComponent<SeekerInput>();
    }
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        if (this.IsMyCharacter())
        {
            Managers.Spawn.WeaponSpawn(Define.Weapon.Melee2, this , true);
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
