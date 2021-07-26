using System.Collections;
using UnityEngine;
using Photon.Pun;

public class HiderAttack : AttackBase
{


    HiderInput _hiderInput;

   
    private void Awake()
    {
        _hiderInput = GetComponent<HiderInput>();
        _hiderInput.AttackEventCallBack += UpdateBaseAttack;
    }
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        if (this.IsMyCharacter())
        {
            Managers.Spawn.WeaponSpawn(Define.Weapon.Stone, this);
            

        }

        
    }

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
    //    SetupAnimation();
    //}
    public void OnUpdate()
    {
        if (this.photonView.IsMine == false ) return;
        //UpdateAttackCoolTime();
        //UpdateAttack(_hiderInput.LastAttackVector);
    }
    public void Update()
    {
        OnUpdate();
    }

    //private void LateUpdate()
    //{
    //    if (this.IsMyCharacter() == false || currentWeapon == null) return;
    //    UpdateZoom(_hiderInput.AttackVector);
    //}
    private void LateUpdate()
    {
        if (this.IsMyCharacter() == false) return;
        UpdateBaseZoom(_hiderInput.AttackVector);
        //if(itemWeapons[0] != null)
        //{
        //    UpdateItemZoom(0, _hiderInput.ItemVector1);
        //}

    }

}
