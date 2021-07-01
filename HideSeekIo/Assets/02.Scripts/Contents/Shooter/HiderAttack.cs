using System.Collections;
using UnityEngine;
using Photon.Pun;

public class HiderAttack : AttackBase
{


    HiderInput _hiderInput;

   
    private void Awake()
    {
        _hiderInput = GetComponent<HiderInput>();
    }
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        if (this.IsMyCharacter())
        {
            Managers.Spawn.WeaponSpawn(Define.Weapon.Stone, this);
        }
    }

    public void OnUpdate()
    {
        if (this.photonView.IsMine == false || weapon == null) return;
        UpdateAttackCoolTime();
        UpdateAttack(_hiderInput.LastAttackVector);
        weapon.Zoom(_hiderInput.AttackVector);
    }
    public void Update()
    {
        OnUpdate();
    }

    //private void LateUpdate()
    //{
    //    if (this.IsMyCharacter() == false || weapon == null) return;
    //    weapon.Zoom(_hiderInput.AttackVector);
    //}


}
