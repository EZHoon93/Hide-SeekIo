using System.Collections;

using UnityEngine;

public class HiderMove : MoveBase , IMakeRunEffect
{
    public enum MoveState
    {
        idle,
        Walk,
        Run
    }

    MoveState n_moveState;




    protected HiderInput _humanInput;
    protected CharacterController _characterController;
    //============================= 변수 =============================/

  


    public MoveState State { get => n_moveState; set { n_moveState = value; } }

    public Define.MoveHearState HearState { get; set; }

    //------------------함수---------------------/

    private void Awake()
    {
        _humanInput = GetComponent<HiderInput>();
        _characterController = GetComponent<CharacterController>();
    }

    public override void OnPhotonInstantiate()
    {
        MoveSpeed = 1;
        base.OnPhotonInstantiate();
        _animator = GetComponentInChildren<Animator>();
        HearState = Define.MoveHearState.NoEffect;
    }
    public bool IsLocal()
    {
        return photonView.IsMine;
    }

    protected void FixedUpdate()
    {
        if (photonView.IsMine == false) return;
        if (_humanInput.IsStop)
        {
            _animator.SetFloat("Speed", -0.1f);
            return;
        }
        // Stop 상태가 아니라면 진행
        UpdateRotate(_humanInput.MoveVector);
        UpdateMove(_humanInput.MoveVector, _humanInput.IsRun);
        UpdateAnimation();
    }

    protected virtual void UpdateMove(Vector2 inputMoveVector2, bool isRun)
    {
        float resultSpeed = 0;
        //조이스틱 입력안할시
        if (inputMoveVector2.sqrMagnitude == 0)
        {
            HearState = Define.MoveHearState.NoEffect;
            State = MoveState.idle;
            resultSpeed = 0;
            //return;
        }
        //뛰기 버튼 시 
        else if (isRun)
        {
            HearState = Define.MoveHearState.Effect;
            State = MoveState.Run;
            resultSpeed = MoveSpeed * 1.8f;
        }
        //뛰기버튼X 걷기 
        else
        {
            HearState = Define.MoveHearState.NoEffect;
            State = MoveState.Walk;
            resultSpeed = MoveSpeed * 1;
        }

        resultSpeed = resultSpeed + (_totRatio * resultSpeed);
        Vector3 moveDistance = this.transform.forward * resultSpeed * Time.deltaTime;
        if (!_characterController.isGrounded)
        {
            moveDistance.y -= 9.8f * Time.deltaTime;
        }
        _characterController.Move(moveDistance);
    }

    protected virtual void UpdateRotate(Vector2 inputMoveVector2)
    {
        if (inputMoveVector2.normalized.sqrMagnitude == 0) return;
        var quaternion = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
        var temp = new Vector3(inputMoveVector2.x, 0, inputMoveVector2.y).normalized;
        var newDirection = quaternion * temp;
        Quaternion newRotation = Quaternion.LookRotation(newDirection);
        this.transform.rotation = Quaternion.Slerp(this.transform.localRotation, newRotation, RotationSpeed * Time.deltaTime);    //즉시변환
    }


    protected virtual void UpdateAnimation()
    {
        switch (State)
        {
            case MoveState.idle:
                _animationValue = Mathf.Lerp(_animationValue, 0, Time.deltaTime * 3);
                break;
            case MoveState.Walk:
                _animationValue = Mathf.Lerp(_animationValue, 0.3f, Time.deltaTime * 3);
                break;
            case MoveState.Run:
                _animationValue = Mathf.Lerp(_animationValue, 1.0f, Time.deltaTime * 3);
                break;
        }

        _animator.SetFloat("Speed", _animationValue);
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
        _animationValue = 0.0f;
        _animator.SetFloat("Speed", 0.0f);
    }

}
