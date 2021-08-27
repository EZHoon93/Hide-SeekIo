using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerMove : MonoBehaviourPun
{
    public enum MoveState
    {
        Idle,
        Walk,
        Run,
        Stun,
    }

    public MoveState State { get; set; }

    CharacterController _characterController;
    PlayerShooter _playerShooter;
    PlayerInput _playerInput;
    Character_Base _character_Base;
    Animator _animator => _character_Base.animator;


    float _animationValue;
    List<float> _moveBuffRatioList = new List<float>(); //캐릭에 슬로우및이속증가 버퍼리스트

    protected float _totRatio;    //버퍼리스트 합계산한 최종 이속 증/감소율
    public Vector2 moveInputVector2 { get; set; }

    public int RotationSpeed { get; protected set; } = 15;
    public float ResultSpeed { get; set; }
    public bool Run { get; set; }


    float _lastTime;
    float _stepTimeBet = 0.5f;


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_animationValue);
        }
        else
        {
            _animationValue = (float)stream.ReceiveNext();
            _animator.SetFloat("Speed", _animationValue);

        }
    }
    protected virtual void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerShooter = GetComponent<PlayerShooter>();
        _playerInput = GetComponent<PlayerInput>();
        //_stepClip = Managers.Resource.Load<AudioClip>("Sounds/Step1");ㅌ

      

    }
    private void OnEnable()
    {
        State = MoveState.Idle;
        _lastTime = 0;
        Run = false;
    }
    public virtual void OnPhotonInstantiate()
    {
        _character_Base = GetComponent<Character_Base>();
        _playerInput.AddInputEvent(Define.AttackType.Button, ControllerInputType.Down, InputType.Main, (vector2) => { Run = true; print("ㄹㅓㄴ온"); });
        _playerInput.AddInputEvent(Define.AttackType.Button, ControllerInputType.Up, InputType.Main, (vector2) => { Run = false; print("ㄹㅓㄴ오프"); });
    }
    private void Update()
    {
        OnUpdateOtherClients();
    }



    protected void FixedUpdate()
    {
        if (photonView.IsMine == false) return;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        var move = new Vector2(h, v);
        move = UtillGame.GetInputVector2_ByCamera(move);
#if UNITY_EDITOR
        OnUpdate(move, Run);
#endif
    }



    protected void OnUpdateOtherClients()
    {
        if (photonView.IsMine) return;
        switch (_playerShooter.State)
        {
            case PlayerShooter.state.Idle:
                UpdateMoveAnimation(State);
                break;
            case PlayerShooter.state.Attack:
                UpdateImmediateRotate(_playerShooter.AttackDirection);
                UpdateMoveAnimation(MoveState.Idle);
                break;
        }
    }
    //public void OnUpdate(Vector2 inputVector2, bool isRun)
    //{
    //    _character_Base.MoveSpeed = _testSpeed;
    //    switch (_playerShooter.State)
    //    {
    //        case PlayerShooter.state.Idle:
    //            UpdateSmoothRotate(inputVector2);
    //            UpdateMove(inputVector2, isRun);
    //            UpdateMoveAnimation(State);
    //            break;
    //        case PlayerShooter.state.Attack:
    //            //UpdateImmediateRotate(_playerShooter.AttackDirection);
    //            UpdateMoveAnimation(MoveState.Idle);
    //            break;
    //        case PlayerShooter.state.Throw:
    //            UpdateMove(inputVector2, isRun);
    //            UpdateImmediateRotate(_playerShooter.AttackDirection);
    //            UpdateMoveAnimation(MoveState.Idle);

    //            break;
    //        //case AttackBase.state.Dash:
    //        //    UpdateMove(_attackBase.AttackDirection, isRun);
    //        //    UpdateMoveAnimation(MoveState.Run);
    //        //break;
    //        case PlayerShooter.state.Jump:
    //            break;
    //    }
    //    //UpdateStepSound();

    //}

    public void OnUpdate(Vector2 inputVector2, bool isRun)
    {
        switch (_playerShooter.State)
        {
            case PlayerShooter.state.Idle:
                if (_character_Base.CurrentEnergy > 0)
                {
                    UpdateSmoothRotate(inputVector2);
                    UpdateMove(inputVector2, isRun);
                    UpdateMoveAnimation(State);
                }
                else
                {
                    UpdateSmoothRotate(Vector2.zero);
                    UpdateMove(Vector2.zero, isRun);
                }
                UpdateMoveAnimation(State);
                break;
            case PlayerShooter.state.Attack:
                UpdateMoveAnimation(MoveState.Idle);
                break;
            case PlayerShooter.state.Throw:
                UpdateMove(inputVector2, isRun);
                UpdateImmediateRotate(_playerShooter.AttackDirection);
                UpdateMoveAnimation(MoveState.Idle);
                break;
        }
        UpdateEnergy();
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
            ResultSpeed = _character_Base.MoveSpeed * 1f;
        }
        //뛰기버튼X 걷기 
        else
        {
            State = MoveState.Walk;
            ResultSpeed = _character_Base.MoveSpeed * 0.7f;
        }

        ResultSpeed = ResultSpeed + (_totRatio * ResultSpeed);
        if (_playerShooter.State == PlayerShooter.state.Dash)
        {
            ResultSpeed *= 1.5f;
        }
        Vector3 moveDistance = UtillGame.ConventToVector3(inputMoveVector2.normalized) * ResultSpeed * Time.deltaTime;
        if (!_characterController.isGrounded)
        {
            moveDistance.y -= 9.8f * Time.deltaTime;
        }
        _characterController.Move(moveDistance);
    }


    //protected void UpdateStepSound()
    //{

    //    if (_lastTime < 0)
    //    {
    //        if (State == MoveState.Run)
    //        {
    //            //Managers.Sound.Play(_stepClip, Define.Sound.Effect, 0.3f);
    //            _lastTime = _stepTimeBet;
    //        }

    //    }
    //    _lastTime -= Time.deltaTime;
    //}

    ////해당지점까지 점프.
    //public void Jump(Vector3 targetPoint, float durationTime)
    //{
    //    //this.transform.DOMoveX(targetPoint.x, durationTime);
    //    //this.transform.DOMoveZ(targetPoint.z, durationTime);
    //    //this.transform.DOMoveY(0.5f, durationTime);
    //    //_animator.SetFloat("Jump", durationTime);
    //    //this.transform.DOJump(targetPoint, 0.5f,1, durationTime);

    //    //_attackBase.state
    //}



    protected void UpdateSmoothRotate(Vector2 inputVector2)
    {
        if (inputVector2.normalized.sqrMagnitude == 0) return;
        Quaternion quaternion = Quaternion.Euler(0, 0, 0);
        var converVector3 = UtillGame.ConventToVector3(inputVector2.normalized);
        var newDirection = quaternion * converVector3;
        Quaternion newRotation = Quaternion.LookRotation(newDirection);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, RotationSpeed * Time.deltaTime);
    }
    protected void UpdateImmediateRotate(Vector2 inputVector2)
    {
        var converVector3 = UtillGame.ConventToVector3(inputVector2.normalized);
        Quaternion newRotation = Quaternion.LookRotation(converVector3);
        this.transform.rotation = newRotation;
    }

    protected void UpdateMoveAnimation(MoveState moveState)
    {
        switch (moveState)
        {
            case MoveState.Idle:
                _animationValue = Mathf.Clamp(Mathf.Lerp(_animationValue, 0, Time.deltaTime * 3), 0, _character_Base.MoveSpeed);
                break;
            case MoveState.Walk:
                _animationValue = Mathf.Clamp(Mathf.Lerp(_animationValue, _character_Base.MoveSpeed * 0.7f, Time.deltaTime * 3), 0, _character_Base.MoveSpeed);
                break;
            case MoveState.Run:
                _animationValue = Mathf.Clamp(Mathf.Lerp(_animationValue, _character_Base.MoveSpeed * 1, Time.deltaTime * 3), 0, _character_Base.MoveSpeed);
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
                _character_Base.CurrentEnergy = Mathf.Clamp(_character_Base.CurrentEnergy - 1 * Time.deltaTime, 0, _character_Base.MaxEnergy);
                break;
            case MoveState.Idle:
            case MoveState.Walk:
                _character_Base.CurrentEnergy = Mathf.Clamp(_character_Base.CurrentEnergy + 0.5f * Time.deltaTime, 0, _character_Base.MaxEnergy);
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
