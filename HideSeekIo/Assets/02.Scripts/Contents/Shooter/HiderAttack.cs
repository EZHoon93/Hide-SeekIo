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
    public state State { get; set; }
    public Vector3 AttackTargetDirection { get; protected set; }

   
    private void Awake()
    {
        _hiderInput = GetComponent<HiderInput>();
    }
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        State = state.Idle;
        GameManager.Instance.SpawnManager.WeaponSpawn(Define.Weapon.Stone, this);
        if (this.IsMyCharacter())
        {

        }
    }

    public void Update()
    {
        if (this.photonView.IsMine == false|| weapon == null) return;
        UpdateAttack();

        if (Input.GetKeyDown(KeyCode.O))
        {
            _animator.CrossFade("Gun", 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            _animator.CrossFade("Melee", 0.1f);
        }
    }

    private void LateUpdate()
    {
        if (this.IsMyCharacter() == false) return;
        weapon.Zoom(_hiderInput.AttackVector);
    }

    public void UpdateAttack()
    {
        if (_hiderInput.LastAttackVector != Vector2.zero)
        {
            if (State != state.Idle) return;
            State = state.Attack;
            AttackTargetDirection = _hiderInput.LastAttackVector;
            photonView.RPC("AttackToServer", RpcTarget.All, _hiderInput.LastAttackVector);
        }
    }
    #region Attack
    [PunRPC]
    public void AttackToServer(Vector2 targetPoint)
    {
        StartCoroutine(ProcessAttackOnClients(targetPoint));
    }

    IEnumerator ProcessAttackOnClients(Vector3 targetPoint)
    {
        State = state.Attack;
        AttackTargetDirection = UtillGame.ConventToVector3(targetPoint);
        _animator.SetTrigger(weapon.AttackAnim);
        yield return new WaitForSeconds(weapon.AttackDelay);   //대미지 주기전까지 시간
        weapon.Attack(targetPoint);    //대미지 줌
        yield return new WaitForSeconds(weapon.AfaterAttackDelay); //끝날때까지 못움직임
        State = state.Idle;
    }

    #endregion

}
