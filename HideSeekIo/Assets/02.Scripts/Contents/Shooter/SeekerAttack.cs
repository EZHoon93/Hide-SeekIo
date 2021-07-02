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
        if (this.IsMyCharacter())
        {
            Managers.Spawn.WeaponSpawn(Define.Weapon.Melee2, this);
        }
    }

    public void OnUpdate()
    {
        if (this.photonView.IsMine == false || weapon == null) return;
        UpdateAttackCoolTime();
        UpdateAttack(_seekerInput.LastAttackVector);
    }


   
    public void Update()
    {
        OnUpdate();
    }

    private void LateUpdate()
    {
        if (this.IsMyCharacter() == false || weapon == null) return;
        UpdateZoom(_seekerInput.AttackVector);
    }

}
