using System.Collections;

using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class MoveBase : MonoBehaviourPun
{
    public enum MoveState
    {
        Idle,
        Walk,
        Run,
        Stun
    }

    public MoveState State { get; set; }

    protected CharacterController _characterController;
    protected InputBase _inputBase;
    protected Animator _animator;
    protected AttackBase _attackBase;

    float _animationValue;
    List<float> _moveBuffRatioList = new List<float>(); //캐릭에 슬로우및이속증가 버퍼리스트

    protected float _totRatio;    //버퍼리스트 합계산한 최종 이속 증/감소율
    public float MoveSpeed { get; protected set; }
    public int RotationSpeed { get; protected set; } = 15;
    public float ResultSpeed { get; protected set; }


    protected virtual void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _inputBase = GetComponent<InputBase>();
        _attackBase = GetComponent<AttackBase>();
    }
    public virtual void OnPhotonInstantiate()
    {
        _animator = GetComponentInChildren<Animator>();
    }




    protected void UpdateSmoothRotate(Vector2 inputVector2)
    {
        if (inputVector2.normalized.sqrMagnitude == 0) return;
        var newRotation = UtillGame.GetWorldRotation_ByInputVector(inputVector2);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, RotationSpeed * Time.deltaTime);
    }
    protected void UpdateImmediateRotate(Vector2 inputVector2)
    {
        this.transform.rotation = UtillGame.GetWorldRotation_ByInputVector(inputVector2);
    }

    protected void UpdateMoveAnimation(MoveState moveState)
    {
        switch (moveState)
        {
            case MoveState.Idle:
                _animationValue = Mathf.Lerp(_animationValue, 0, Time.deltaTime * 3);
                break;
            case MoveState.Walk:
                _animationValue = Mathf.Lerp(_animationValue, MoveSpeed *0.5f, Time.deltaTime * 3);
                break;
            case MoveState.Run:
                _animationValue = Mathf.Lerp(_animationValue, MoveSpeed * 1 , Time.deltaTime * 3);
                break;
            case MoveState.Stun:
                _animationValue = 0;
                break;
        }
        _animator.SetFloat("Speed", _animationValue);
    }

    protected virtual void UpdateMove(Vector2 inputMoveVector2, bool isRun)
    {
        //조이스틱 입력안할시
        if (inputMoveVector2.sqrMagnitude == 0)
        {
            State = MoveState.Idle;
            ResultSpeed = 0;
        }
        //뛰기 버튼 시 
        else if (isRun)
        {
            State = MoveState.Run;
            ResultSpeed = MoveSpeed * 1f;
        }
        //뛰기버튼X 걷기 
        else
        {
            State = MoveState.Walk;
            ResultSpeed = MoveSpeed * 0.5f;
        }

        ResultSpeed = ResultSpeed + (_totRatio * ResultSpeed);

        Vector3 moveDistance = this.transform.forward * ResultSpeed * Time.deltaTime;
        if (!_characterController.isGrounded)
        {
            moveDistance.y -= 9.8f * Time.deltaTime;
        }
        _characterController.Move(moveDistance);
    }



    public void AddMoveBuffList(float ratio, bool isAdd)
    {
        if (isAdd)
        {

            _moveBuffRatioList.Add(ratio);
        }
        else
        {
            _moveBuffRatioList.Remove(ratio);
        }

        _totRatio = 0; ;
        foreach (var v in _moveBuffRatioList)
        {
            _totRatio += v;
        }

    }

  


}
