using System.Collections;

using UnityEngine;
using Photon.Pun;
public class SeekerAttack : AttackBase
{
    public enum state
    {
        Idle,
        Attack,
    }

    SeekerInput _seekerInput;
    public state State { get ; private set; }
    public Vector3 AttackTargetDirection { get; set; }
    private void Awake()
    {
        _seekerInput = GetComponent<SeekerInput>();
    }
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        State = state.Idle;
        
        if (this.IsMyCharacter())
        {
            GameManager.Instance.SpawnManager.WeaponSpawn(Define.Weapon.Melee2, this);
        }
    }
    public void Update()
    {
        if (this.photonView.IsMine == false || weapon ==null) return;
        UpdateAttackCoolTime();
        UpdateAttack();
    }
    private void LateUpdate()
    {
        if (this.IsMyCharacter() == false || weapon == null ) return;
        weapon.Zoom(_seekerInput.AttackVector);
    }
    public void UpdateAttack()
    {
        if (_seekerInput.LastAttackVector != Vector2.zero)
        {
            if (State != state.Idle) return;
            //Util.StartCoroutine(this, ref _attackEnumerator, Attack(_seekerInput.LastAttackVector ));
            weapon.AttackCheck(_seekerInput.LastAttackVector);
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
    //IEnumerator Attack(Vector2 inputVector2)
    //{
    //    if (weapon.AttackCheck(inputVector2))
    //    {
    //        while (weapon.state == Weapon.State.Delay)
    //        {
    //            yield return null;
    //        }
    //        State = state.Idle;
    //    }
        
    //}

}
