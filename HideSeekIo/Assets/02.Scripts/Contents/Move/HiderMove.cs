using System.Collections;

using UnityEngine;

public class HiderMove : MoveBase , IMakeRunEffect
{
    public Define.MoveHearState HearState { get; set; }
    HiderInput _hiderInput;

    //public float MaxEnergy { get; private set; } = 10;
    //public float CurrentEnergy { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        _hiderInput = GetComponent<HiderInput>();
    }
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        HearState = Define.MoveHearState.NoEffect;
    }
    public bool IsLocal()
    {
        return photonView.IsMine;
    }

    protected void FixedUpdate()
    {
        if (photonView.IsMine == false) return;
        OnUpdate(_hiderInput.MoveVector, _hiderInput.IsRun);
        UpdateMoveEffect();
    }

    //void UpdateEnergy()
    //{
    //    switch (State)
    //    {
    //        case MoveState.Run:
    //            CurrentEnergy = Mathf.Clamp(CurrentEnergy - 2*Time.deltaTime, 0, MaxEnergy);
    //            break;
    //        case MoveState.Idle:
    //        case MoveState.Walk:
    //            CurrentEnergy = Mathf.Clamp(CurrentEnergy + Time.deltaTime, 0, MaxEnergy);
    //            break;
    //    }
    //}
    void UpdateMoveEffect()
    {
        switch (State)
        {
            case MoveState.Stun:
            case MoveState.Idle:
            case MoveState.Walk:
                HearState = Define.MoveHearState.NoEffect;
                break;
            case MoveState.Run:
                HearState = Define.MoveHearState.Effect;
                break;
        }
    }

    //public override void OnUpdate(Vector2 inputVector2, bool isRun)
    //{
    //    switch (_attackBase.State)
    //    {
    //        case AttackBase.state.Idle:
    //            if(CurrentEnergy > 0)
    //            {
    //                UpdateSmoothRotate(inputVector2);
    //                UpdateMove(inputVector2, isRun);
    //            }
    //            else
    //            {
    //                UpdateSmoothRotate(Vector2.zero);
    //                UpdateMove(Vector2.zero, isRun);
    //            }
    //            UpdateMoveAnimation(State);
    //            break;
    //        case AttackBase.state.Attack:
    //            UpdateImmediateRotate(_attackBase.currentWeapon.LastAttackInput);
    //            UpdateMoveAnimation(MoveState.Idle);
    //            break;
    //    }

    //}

    public virtual void Stop()
    {
        //_animationValue = 0.0f;
        _animator.SetFloat("Speed", 0.0f);
    }

}
