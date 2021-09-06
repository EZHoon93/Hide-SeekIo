using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Smooth;
using UnityEngine.AI;

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

    protected override Transform target => _playerCharacter.character_Base.transform;
    CharacterController _characterController;
    PlayerShooter _playerShooter;
    PlayerStat _playerStat;
    PlayerInput _playerInput;
    PlayerCharacter _playerCharacter;
    NavMeshAgent agent;
    Animator _animator => _playerCharacter.animator;

    float _animationValue;
    List<float> _moveBuffRatioList = new List<float>(); //캐릭에 슬로우및이속증가 버퍼리스트
    protected float _totRatio;    //버퍼리스트 합계산한 최종 이속 증/감소율
    
    public float ResultSpeed { get; set; }
    public bool Run { get; set; }
    float runCoolTime = 0.0f;
 
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        agent = this.gameObject.GetOrAddComponent<NavMeshAgent>();
        _playerShooter = GetComponent<PlayerShooter>();
        _playerInput = GetComponent<PlayerInput>();
        _playerStat = GetComponent<PlayerStat>();
        _playerCharacter = GetComponent<PlayerCharacter>();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        State = MoveState.Idle;
        _animationValue = 0;
        Run = false;
    }
    public virtual void OnPhotonInstantiate()
    {
        
    }

    public void ChangeOwnerShip()
    {
        _playerInput.AddInputEvent(Define.AttackType.Button, ControllerInputType.Down, InputType.Main, (vector2) => { Run = true;  } );
        _playerInput.AddInputEvent(Define.AttackType.Button, ControllerInputType.Up, InputType.Main, (vector2) => { Run = false; } );
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
        var move = _playerInput.controllerInputDic[InputType.Move].inputVector2;
        //move = UtillGame.GetInputVector2_ByCamera(move);
        OnUpdate(move, Run);
        
    }
    protected override void UpdateRemote()
    {
        UpdateMoveAnimation(State);

    }
    public void OnUpdate(Vector2 inputVector2, bool isRun)
    {
        switch (_playerShooter.State)
        {
            case PlayerShooter.state.Idle:
                Vector3 test = UtillGame.ConventToVector3(inputVector2);
                if (_playerStat.CurrentEnergy > -900)
                {
                    UpdateSmoothRotate(test);
                    UpdateMove(inputVector2, isRun);
                    UpdateMoveAnimation(State);
                }
                else
                {
                    Run = false;
                    UpdateSmoothRotate(Vector2.zero);
                    UpdateMove(Vector2.zero, isRun);
                }
                UpdateMoveAnimation(State);
                break;
            case PlayerShooter.state.NoMove:
                UpdateImmediateRotate(_playerShooter.AttackDirection);
                UpdateMoveAnimation(MoveState.Idle);
                break;
            case PlayerShooter.state.MoveAttack:
                UpdateMove(inputVector2, isRun);
                UpdateMoveAnimation(State);
                break;
            case PlayerShooter.state.Jump:
                UpdateImmediateRotate(_playerShooter.AttackDirection);
                UpdateMoveFoward(_playerShooter.AttackDirection);
                break;
        }
        UpdateEnergy();
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
        ResultSpeed = ResultSpeed + (_totRatio * ResultSpeed);
        Vector3 moveDistance = UtillGame.ConventToVector3(inputVector2.normalized) * ResultSpeed * Time.deltaTime;
        if (!_characterController.isGrounded)
        {
            moveDistance.y -= 9.8f * Time.deltaTime;
        }
        _characterController.Move(moveDistance);
    }

    //대쉬
    void UpdateMoveFoward(Vector2 inputVector2)
    {
        var  c = 5;
        ResultSpeed = ResultSpeed + (_totRatio * ResultSpeed);
        Vector3 moveDistance = UtillGame.ConventToVector3(inputVector2.normalized) *  10 * Time.deltaTime;
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
                _animationValue = Mathf.Clamp(Mathf.Lerp(_animationValue, _playerStat.moveSpeed * 0.7f, Time.deltaTime * 3), 0, 2.5f);
                break;
            case MoveState.Run:
                _animationValue = Mathf.Clamp(Mathf.Lerp(_animationValue, _playerStat.moveSpeed * 1, Time.deltaTime * 3), 0, 2.5f);
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
    void UpdateEnergy()
    {
        switch (State)
        {
            case MoveState.Run:
                _playerStat.CurrentEnergy = Mathf.Clamp(_playerStat.CurrentEnergy - 1 * Time.deltaTime, 0, _playerStat.MaxEnergy);
                break;
            case MoveState.Idle:
            case MoveState.Walk:
                _playerStat.CurrentEnergy = Mathf.Clamp(_playerStat.CurrentEnergy + 0.5f * Time.deltaTime, 0, _playerStat.MaxEnergy);
                break;
        }
    }
    public void Dash()
    {

    }

    protected IEnumerator DashProcess()
    {
        var inputVector = _playerInput.controllerInputDic[InputType.Move].inputVector2;

        yield return new WaitForSeconds(1.0f);
    }
}
