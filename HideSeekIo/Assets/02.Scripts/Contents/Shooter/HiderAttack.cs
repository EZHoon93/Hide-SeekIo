using System.Collections;
using UnityEngine;
using Photon.Pun;

public class HiderAttack : AttackBase
{
    public enum state
    {
        Idle,
        Attack,
        Skill
    }


    HiderInput _hiderInput;
    public state State { get; private set; }
    public Vector3 AttackTargetDirection { get; protected set; }

   
    private void Awake()
    {
        _hiderInput = GetComponent<HiderInput>();
    }
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        State = state.Idle;
        if (this.IsMyCharacter())
        {
            GameManager.Instance.SpawnManager.WeaponSpawn(Define.Weapon.Stone, this);
        }
    }

    public void Update()
    {
        if (this.photonView.IsMine == false || weapon == null) return;
        UpdateAttackCoolTime();
        UpdateAttack();
      
    }

    private void LateUpdate()
    {
        if (this.IsMyCharacter() == false || weapon == null) return;
        weapon.Zoom(_hiderInput.AttackVector);
    }

    public void UpdateAttack()
    {
        if (_hiderInput.LastAttackVector != Vector2.zero)
        {
            if (State != state.Idle) return;
            weapon.AttackCheck(_hiderInput.LastAttackVector);

        }
    }

    protected override void AttackSucess()
    {
        base.AttackSucess();
        State = state.Attack;
    }

    protected override void AttackEnd()
    {
        base.AttackEnd();
        State = state.Idle;
    }
}
