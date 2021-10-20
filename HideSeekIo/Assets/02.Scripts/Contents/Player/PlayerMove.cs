using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Smooth;
using UnityEngine.AI;
using System;

public class PlayerMove : PhotonMove 
{
    public enum MoveState
    {
        Idle,
        Walk,
        Run,
        Stun,
    }
    public MoveState State { get; set; }

    protected override Transform target => _playerCharacter.characterAvater.transform;
    CharacterController _characterController;
    PlayerShooter _playerShooter;
    PlayerStat _playerStat;
    PlayerInput _playerInput;
    PlayerCharacter _playerCharacter;
    NavMeshAgent agent;
    Animator _animator => _playerCharacter.animator;

    float _animationValue;
    bool _run;
    List<float> _moveBuffRatioList = new List<float>(); //캐릭에 슬로우및이속증가 버퍼리스트
    protected float _totRatio;    //버퍼리스트 합계산한 최종 이속 증/감소율
    
    public float ResultSpeed { get; set; }
    public bool Run { 
        get => _run; 
        set
        {
            if (_run == value) return;
            _run = value;
            if (_run)
            {
                startRunTime = 0;
            }
            else
            {
                startRunTime = 0;
            }
        }
    }
    float startRunTime;
    public bool isInvinc { get; set; }  //무적

    public event Action<MoveState> changeMoveStateEvent;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        _playerShooter = GetComponent<PlayerShooter>();
        _playerInput = GetComponent<PlayerInput>();
        _playerStat = GetComponent<PlayerStat>();
        _playerCharacter = GetComponent<PlayerCharacter>();

        //GetComponent<PlayerHealth>().onDamageEventPoster += OnDamageListen; //대미지 입으면 일시멈춤.
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        State = MoveState.Idle;
        _animationValue = 0;
        Run = false;
        startRunTime = 0;
        //_remainStunTime = 0;
    }
    public virtual void OnPhotonInstantiate()
    {
        isInvinc = false;
    }

    public void ChangeOwnerShip()
    {
        //_playerInput.AddInputEvent(Define.AttackType.Button, ControllerInputType.Down, InputType.Main, (vector2) => { Run = !Run;  } );
        //_playerInput.AddInputEvent(Define.AttackType.Button, ControllerInputType.Up, InputType.Main, (vector2) => { Run = false; } );
    }
   

    protected override void WriteData(PhotonStream stream, PhotonMessageInfo info)
    {
        base.WriteData(stream, info);
        stream.SendNext(State);
    }

    protected override void ReadData(PhotonStream stream, PhotonMessageInfo info)
    {
        base.ReadData(stream, info);
        State =(MoveState)stream.ReceiveNext();
    }
    protected override void UpdateLocal()
    {
        var inputVector = _playerInput.controllerInputDic[InputType.Move].inputVector2;

        switch (_playerShooter.State)
        {
            case PlayerShooter.state.Idle:
                dataState = DataState.SerializeView;
                UpdateSmoothRotate(new Vector3(inputVector.x, 0, inputVector.y));
                UpdateMove(inputVector, true);
                UpdateMoveAnimation(State);
                break;
            case PlayerShooter.state.Skill:
                dataState = DataState.ServerView;
                UpdateMoveAnimation(  MoveState.Idle);
                break;
            case PlayerShooter.state.MoveAttack:
                dataState = DataState.SerializeView;
                UpdateMove(inputVector, true);
                UpdateSmoothRotate(new Vector3(inputVector.x, 0, inputVector.y));
                UpdateMoveAnimation(State);
                break;
            //case PlayerShooter.state.No:
            //    UpdateMoveAnimation(MoveState.Idle);
                //break;
        }
    }
    protected override void UpdateRemote()
    {
        switch (_playerShooter.State)
        {
            //case PlayerShooter.state.Idle:
            //case PlayerShooter.state.NoMove:
            case PlayerShooter.state.MoveAttack:
                UpdateMoveAnimation(State);
                break;
        }
        UpdateMoveAnimation(State);

    }
    protected virtual void UpdateMove(Vector2 inputVector2, bool isRun)
    {
        //조이스틱 입력안할시
        if (inputVector2.sqrMagnitude == 0)
        {
            State = MoveState.Idle;
            ResultSpeed = 0;
        }
        //뛰기 버튼 시 
        else if (isRun)
        {
            State = MoveState.Run;
            ResultSpeed = _playerStat.moveSpeed * 1f;
        }
        //뛰기버튼X 걷기 
        else
        {
            State = MoveState.Walk;
            ResultSpeed = _playerStat.moveSpeed* 0.7f;
        }
        ResultSpeed = ResultSpeed + (_totRatio * ResultSpeed) + (startRunTime* _playerStat.moveSpeed*0.0f)  ;
        Vector3 moveDistance = UtillGame.ConventToVector3(inputVector2.normalized) * ResultSpeed * Time.deltaTime;
        if (!_characterController.isGrounded)
        {
            moveDistance.y -= 9.8f * Time.deltaTime;
        }
        _characterController.Move(moveDistance);
    }

    protected void UpdateMoveAnimation(MoveState moveState)
    {
        switch (moveState)
        {
            case MoveState.Idle:
                _animationValue = Mathf.Clamp(Mathf.Lerp(_animationValue, 0, Time.deltaTime * 3), 0.2f, 2.5f);
                break;
            case MoveState.Walk:
                _animationValue = Mathf.Clamp(Mathf.Lerp(_animationValue, _playerStat.moveSpeed * 0.7f, Time.deltaTime * 3), 0, _playerStat.moveSpeed * 0.5f);
                break;
            case MoveState.Run:
                _animationValue = Mathf.Clamp(Mathf.Lerp(_animationValue, _playerStat.moveSpeed * 1, Time.deltaTime * 3), 0, _playerStat.moveSpeed);
                break;
            case MoveState.Stun:
                _animationValue = -0.1f;
                break;
        }
        _animator.SetFloat("Speed", _animationValue);
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

   
 

    // 목표지점 이동이아닌 초기 target의 방향으로 ,직진
    public void MoveToTarget(Vector3 target , float time)
    {
        StartCoroutine(ProcessMoveToTarget(target, time));
    }

    IEnumerator ProcessMoveToTarget(Vector3 targetPoint , float duration )
    {
        float currTime = 0;
        Vector3 dir = (targetPoint - this.transform.position);
        dir.y = this.transform.position.y;
        Vector3 moveDistance = Vector3.zero;
        Quaternion newRotation = Quaternion.LookRotation(dir);
        target.transform.rotation = newRotation;
        n_direction = target.transform.forward;
        while (currTime <= duration)
        {
            dir.y -= 9.8f * Time.deltaTime;
            //moveDistance = Vector3.Lerp(moveDistance, 2 * dir, Time.deltaTime * (1 / duration));
            _characterController.Move(dir * Time.deltaTime * (1 / duration));
            yield return null;
            currTime += Time.deltaTime;
        }
        //n_direction
        //float currTime = 0;
        //Vector3 dir = (target - this.transform.position);
        //Vector3 moveDistance = Vector3.zero;
        //while (currTime <= duration)
        //{
        //    if(Vector3.Distance(this.transform.position ,dir) >1.5f )
        //    {
        //        dir = (target - this.transform.position);
        //        dir.y -= 9.8f * Time.deltaTime;
        //        //moveDistance = Vector3.Slerp(moveDistance, 3 * dir, Time.deltaTime * (1 / duration));
        //        _characterController.Move(dir.normalized * Time.deltaTime * 8);
        //    }
        //    else
        //    {

        //        _characterController.Move(Vector3.zero * Time.deltaTime );

        //    }

        //    yield return null;
        //    currTime += Time.deltaTime;
        //}
    }

}
