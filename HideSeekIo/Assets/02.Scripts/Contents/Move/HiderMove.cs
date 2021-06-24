using System.Collections;

using UnityEngine;

public class HiderMove : MoveBase , IMakeRunEffect
{
    public Define.MoveHearState HearState { get; set; }
    HiderInput _hiderInput => _inputBase as HiderInput;

    //------------------함수---------------------/
    [SerializeField] float _testSpeed;

    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        //MoveSpeed = 1;
        HearState = Define.MoveHearState.NoEffect;

    }
    public bool IsLocal()
    {
        return photonView.IsMine;
    }

   

    void OnUpdate()
    {
        if (photonView.IsMine == false) return;
        MoveSpeed = _testSpeed;
        switch (_attackBase.State)
        {
            case AttackBase.state.Idle:
                UpdateSmoothRotate(_hiderInput.MoveVector);
                UpdateMove(_hiderInput.MoveVector, _hiderInput.IsRun);
                UpdateMoveAnimation(State);
                break;
            case AttackBase.state.Attack:
                UpdateImmediateRotate(_attackBase.weapon.LastAttackInput);
                UpdateMoveAnimation(MoveState.Stun);
                break;
        }

    }
    protected void FixedUpdate()
    {
        OnUpdate();
    }

    //protected virtual void UpdateMove(Vector2 inputMoveVector2, bool isRun)
    //{
    //    float resultSpeed = 0;
    //    print(inputMoveVector2 + "무브");
    //    //조이스틱 입력안할시
    //    if (inputMoveVector2.sqrMagnitude == 0)
    //    {
    //        HearState = Define.MoveHearState.NoEffect;
    //        State = MoveState.idle;
    //        resultSpeed = 0;
    //        //return;
    //    }
    //    //뛰기 버튼 시 
    //    else if (isRun)
    //    {
    //        HearState = Define.MoveHearState.Effect;
    //        State = MoveState.Run;
    //        resultSpeed = MoveSpeed * 1f;
    //    }
    //    //뛰기버튼X 걷기 
    //    else
    //    {
    //        HearState = Define.MoveHearState.NoEffect;
    //        State = MoveState.Walk;
    //        resultSpeed = MoveSpeed * 0.5f;
    //    }

    //    resultSpeed = resultSpeed + (_totRatio * resultSpeed);
    //    Vector3 moveDistance = this.transform.forward * resultSpeed * Time.deltaTime;
    //    if (!_characterController.isGrounded)
    //    {
    //        moveDistance.y -= 9.8f * Time.deltaTime;
    //    }
    //    _characterController.Move(moveDistance);
    //}

    //protected virtual void UpdateRotate(Vector2 inputMoveVector2)
    //{
    //    if (inputMoveVector2.normalized.sqrMagnitude == 0) return;
    //    var quaternion = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
    //    var temp = new Vector3(inputMoveVector2.x, 0, inputMoveVector2.y).normalized;
    //    var newDirection = quaternion * temp;
    //    Quaternion newRotation = Quaternion.LookRotation(newDirection);
    //    this.transform.rotation = Quaternion.Slerp(this.transform.localRotation, newRotation, RotationSpeed * Time.deltaTime);    //즉시변환
    //}


    protected virtual void UpdateAnimation()
    {
        //switch (State)
        //{
        //    case MoveState.idle:
        //        _animationValue = Mathf.Lerp(_animationValue, 0, Time.deltaTime * 3);
        //        break;
        //    case MoveState.Walk:
        //        _animationValue = Mathf.Lerp(_animationValue, 0.3f, Time.deltaTime * 3);
        //        break;
        //    case MoveState.Run:
        //        _animationValue = Mathf.Lerp(_animationValue, 1.0f, Time.deltaTime * 3);
        //        break;
        //}

        //_animator.SetFloat("Speed", _animationValue);
    }



    private void OnTriggerEnter(Collider other)
    {
        //var enterTrigger = other.GetComponent<IEnterTrigger>();
        //if (enterTrigger != null)
        //{
        //    enterTrigger.EnterGameObject(this.gameObject);
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        //var exitTrigger = other.GetComponent<IExitTrigger>();
        //if (exitTrigger != null)
        //{
        //    exitTrigger.ExitGameObject(this.gameObject);
        //}
    }


    public virtual void Stop()
    {
        //_animationValue = 0.0f;
        _animator.SetFloat("Speed", 0.0f);
    }

}
