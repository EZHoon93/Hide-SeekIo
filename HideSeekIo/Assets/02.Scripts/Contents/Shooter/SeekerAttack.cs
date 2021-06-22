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
        UpdateAttack();

        //CenterPivot.rotation = UtillGame.WorldRotationByInput(_seekerInput.AttackVector);
    }
    private void LateUpdate()
    {
        if (this.IsMyCharacter() == false) return;
        weapon.Zoom(_seekerInput.AttackVector);
    }
    public void UpdateAttack()
    {
        if (_seekerInput.LastAttackVector != Vector2.zero)
        {
            if (State != state.Idle) return;
            //AttackTargetDirection = _seekerInput.LastAttackVector;
            //photonView.RPC("AttackToServer", RpcTarget.All, _seekerInput.LastAttackVector);
            Util.StartCoroutine(this, ref _attackEnumerator, Attack(_seekerInput.LastAttackVector ));

        }
    }

    #region Attack
    //[PunRPC]
    //public void AttackToServer(Vector2 targetPoint)
    //{
    //    StartCoroutine(ProcessAttackOnClients(targetPoint));

    //}

    //IEnumerator ProcessAttackOnClients(Vector3 targetPoint)
    //{
    //    State = state.Attack;
    //    AttackTargetDirection = UtillGame.ConventToVector3(targetPoint);
    //    _animator.SetTrigger(weapon.AttackAnim);
    //    yield return new WaitForSeconds(weapon.AttackDelay);   //대미지 주기전까지 시간
    //    if (this.IsMyCharacter())
    //    {
    //        CameraManager.Instance.ShakeCameraByPosition(this.transform.position, 0.3f, 1.0f, 1.0f);
    //    }
    //    weapon.Attack(targetPoint);    //대미지 줌

    //    yield return new WaitForSeconds(weapon.AfaterAttackDelay); //끝날때까지 못움직임
    //    State = state.Idle;
    //}

    IEnumerator Attack(Vector2 inputVector2)
    {
        if (weapon.Attack(inputVector2) == false)
        {
            StopCoroutine(_attackEnumerator);
        }
        State = state.Attack;
        _animator.SetTrigger(weapon.AttackAnim);
        while ( weapon.state == Weapon.State.Delay)
        {
            yield return null;
        }
        State = state.Idle;
    }

    #endregion




}
