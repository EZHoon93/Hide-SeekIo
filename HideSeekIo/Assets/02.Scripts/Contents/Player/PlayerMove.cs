using System;
using System.Collections;

using ExitGames.Client.Photon.StructWrapping;

using Photon.Pun;
using UnityEngine;

public class PlayerMove : PhotonMove 
{
    CharacterController _characterController;
    PlayerShooter _playerShooter;
    PlayerStat _playerStat;
    PlayerInput _playerInput;
    Animator _animator => _playerStat.animator;
    [SerializeField] Sprite _noFixedSprite;
    [SerializeField] Sprite _fixedSprite;
    public enum MoveState
    {
        Idle,
        Walk,
        Run,
        Stun,
    }

    MoveState _moveState;
    public MoveState State 
    {
        get => _moveState;
        set
        {
            if (_moveState == value) return;
            _moveState = value;
            onChangeMoveStateEvent?.Invoke(_moveState);
        }
    }

 

    float _animationValue;
    bool _run = true;
    public bool run {
        get => _run;
        set
        {
            _run = value;
        }
    }
    public float ResultSpeed { get; set; }

    float _moveCurrEnergy;
    public float moveCurrEnergy
    {
        get => _moveCurrEnergy;
        set
        {
            _moveCurrEnergy = value;
            onChangeMoveEnergy?.Invoke(value);
        }
    }

    float _moveMaxEnergy;
    public float moveMaxEnergy {
        get => _moveMaxEnergy;
        set
        {
            _moveMaxEnergy = value;
            onChangeMoveMaxEnergy?.Invoke(value);
        }
    } 

    public event Action<float> onChangeMoveEnergy;
    public event Action<float> onChangeMoveMaxEnergy;
    public event Action<MoveState> onChangeMoveStateEvent;


    //사물모드전용
    bool _isOnlyRotation;
    public bool isOnlyRotation
    {
        get => _isOnlyRotation;
        set
        {
            _isOnlyRotation = value;
            State = MoveState.Idle;
        }
    }
 


    int moveAnimId;



    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerShooter = GetComponent<PlayerShooter>();
        _playerInput = GetComponent<PlayerInput>();
        _playerStat = GetComponent<PlayerStat>();
        _playerInput.AddInputEvent(Define.AttackType.Joystick, ControllerInputType.Drag, InputType.Move, null);
        //_playerInput.AddInputEvent(Define.AttackType.Button, ControllerInputType.Down, InputType.Main, (vector2) => { run = true; });
        //_playerInput.AddInputEvent(Define.AttackType.Button, ControllerInputType.Up, InputType.Main, (vector2) => {
        //    run = false; });

        //_playerShooter.weaponChangeCallBack += (a) => { isOnlyRotation = false; };

    }
    protected override void OnEnable()
    {
        base.OnEnable();
        State = MoveState.Idle;
        _animationValue = 0;
        run = false;
        isOnlyRotation = false;
        moveMaxEnergy = 2;
        moveCurrEnergy = moveMaxEnergy;

        moveAnimId = Animator.StringToHash("Speed");

    }
    public override void OnPhotonInstantiate(PlayerController playerController)
    {
        //if (Managers.Scene.currentGameScene.gameMode == Define.GameMode.Object && playerController.Team == Define.Team.Hide)
        //{
        //    _playerInput.AddInputEvent(Define.AttackType.Button, ControllerInputType.Down, InputType.Main, (v) => { FixedRotationByObjectMode(); });
        //}
    }

    public override void OnPreNetDestroy(PhotonView rootView)
    {
        
    }
    //소유권이 온다면 ..
    public void ChangeOwnerShip()
    {
        
    }

    public void ChangeTeam(Define.Team team)
    {

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
        var inputVector = _playerInput.GetVector2(InputType.Move);
        switch (_playerShooter.State)
        {
            case PlayerShooter.state.CanMove:
                dataState = DataState.SerializeView;
                UpdateSmoothRotate(new Vector3(inputVector.x, 0, inputVector.y));
                UpdateMove(inputVector, run);
                UpdateMoveAnimation(State);
                break;
            case PlayerShooter.state.Skill:
                dataState = DataState.ServerView;
                UpdateMoveAnimation(  MoveState.Idle);
                break;
            case PlayerShooter.state.MoveToAttackPoint:
                dataState = DataState.SerializeView;
                UpdateMove(inputVector, run);
                UpdateSmoothRotate(new Vector3(inputVector.x, 0, inputVector.y));
                UpdateMoveAnimation(State);
                break;
            //case PlayerShooter.state.No:
            //    UpdateMoveAnimation(MoveState.Idle);
                //break;
        }

        UpdateMoveEnergy();

    }
    protected override void UpdateRemote()
    {
        switch (_playerShooter.State)
        {
            case PlayerShooter.state.MoveToAttackPoint:
                UpdateMoveAnimation(State);
                break;
        }
        UpdateMoveAnimation(State);

    }


    protected virtual void UpdateMove(Vector2 inputVector2, bool isRun)
    {
        if(isOnlyRotation)
        {
            return;
        }
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
        ResultSpeed = ResultSpeed + (_totRatio * ResultSpeed) + (_playerStat.moveSpeed*0.0f)  ;
        Vector3 moveDistance = UtillGame.ConventToVector3(inputVector2.normalized) * ResultSpeed * Time.deltaTime;
        if (!_characterController.isGrounded)
        {
            moveDistance.y -= 9.8f * Time.deltaTime;
        }
        _characterController.Move(moveDistance);
        //_rigidbody.MovePosition(_rigidbody.position+ moveDistance);
    }

    
    protected  void UpdateMoveEnergy()
    {
        if(State == MoveState.Run)
        {
            moveCurrEnergy = Mathf.Clamp(moveCurrEnergy - Time.deltaTime, 0, moveMaxEnergy);
            if(moveCurrEnergy <= 0)
            {
                State = MoveState.Walk;
                run = false;
            }
        }
        else
        {
            moveCurrEnergy = Mathf.Clamp(moveCurrEnergy + Time.deltaTime *0.3f , 0, moveMaxEnergy);
        }
    }
    protected void UpdateMoveAnimation(MoveState moveState)
    {
        switch (moveState)
        {
            case MoveState.Idle:
                //_animationValue = Mathf.Clamp(Mathf.Lerp(_animationValue, 0, Time.deltaTime * 3), 0.0f, 2.5f);
                _animationValue = 0.0f;

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


  
    /// <summary>
    /// 사물모드 전용
    /// </summary>
    //void FixedRotationByObjectMode()
    //{
    //    isOnlyRotation = !isOnlyRotation;
    //    State = MoveState.Idle; //제자리로변경. => 변신모드 hider 변신을위해 콜백호출
    //    if (this.IsMyCharacter())
    //    {
    //        Sprite sprite = isOnlyRotation ? _fixedSprite : _noFixedSprite;
    //        Managers.Input.GetControllerJoystick(InputType.Main).SetupItemImage(sprite);
    //    }

    //}


    #region 필없음
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
        _rotateTarget.transform.rotation = newRotation;
        n_direction = _rotateTarget.transform.forward;
        while (currTime <= duration)
        {
            dir.y -= 9.8f * Time.deltaTime;
            moveDistance = Vector3.Lerp(moveDistance, 2 * dir, Time.deltaTime * (1 / duration));
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
    #endregion
}
