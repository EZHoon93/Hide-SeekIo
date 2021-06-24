using System.Collections;

using UnityEngine;
using Photon.Pun;
public class SeekerAttack : AttackBase
{

    SeekerInput _seekerInput;
    private void Awake()
    {
        _seekerInput = GetComponent<SeekerInput>();
    }
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        print("#3333333333333333");
        if (this.IsMyCharacter())
        {
            Managers.Spawn.WeaponSpawn(Define.Weapon.Melee2, this);
        }
    }
    public void Update()
    {
        OnUpdate();
    }

    public void OnUpdate()
    {
        if (this.photonView.IsMine == false || weapon == null) return;
        UpdateAttackCoolTime();
        UpdateAttack(_seekerInput.LastAttackVector);
        weapon.Zoom(_seekerInput.AttackVector);
    }

}
