using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using System.Collections;
using DG.Tweening;

public class MoveBase : MonoBehaviourPun, IPunObservable
{
    public enum MoveState
    {
        Idle,
        Walk,
        Run,
        Stun,
    }

    public MoveState State { get; set; }

    protected CharacterController _characterController;
    protected Animator _animator;
    protected AttackBase _attackBase;
    protected InputBase _inputBase;
    PhotonTransformView _TransformView;
    AudioClip _stepClip;


    float _animationValue;
    List<float> _moveBuffRatioList = new List<float>(); //캐릭에 슬로우및이속증가 버퍼리스트

    protected float _totRatio;    //버퍼리스트 합계산한 최종 이속 증/감소율
    public Vector2 moveInputVector2 { get; set; }
    public float MoveSpeed { get; protected set; }
    public int RotationSpeed { get; protected set; } = 15;
    public float ResultSpeed { get; protected set; }

    [SerializeField] protected float _testSpeed;

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
        _attackBase = GetComponent<AttackBase>();
        _TransformView = GetComponent<PhotonTransformView>();
        _stepClip = Managers.Resource.Load<AudioClip>("Sounds/Step1");



    }
    private void OnEnable()
    {
        State = MoveState.Idle;
        _lastTime = 0;
    }
    public virtual void OnPhotonInstantiate()
    {
        _animator = GetComponentInChildren<Animator>();
        _inputBase = GetComponent<InputBase>();
        MoveSpeed = _testSpeed;
    }
    private void Update()
    {
        OnUpdateOtherClients();
    }

    protected void OnUpdateOtherClients()
    {
        if (photonView.IsMine) return;
        switch (_attackBase.State)
        {
            case AttackBase.state.Idle:
                    UpdateMoveAnimation(State);
                break;
            case AttackBase.state.Attack:
                UpdateImmediateRotate(_attackBase.AttackDirection);
                UpdateMoveAnimation(MoveState.Idle);
                break;
        }
    }
    public virtual void OnUpdate(Vector2 inputVector2, bool isRun)
    {
        MoveSpeed = _testSpeed;
        switch (_attackBase.State)
        {
            case AttackBase.state.Idle:
                UpdateSmoothRotate(inputVector2);
                UpdateMove(inputVector2, isRun);
                UpdateMoveAnimation(State);
                break;
            //case AttackBase.state.Attack:
            //    UpdateImmediateRotate(_attackBase.AttackDirection);
            //    UpdateMoveAnimation(MoveState.Idle);
            //    break;
            //case AttackBase.state.Dash:
            //    UpdateMove(_attackBase.AttackDirection, isRun);
            //    UpdateMoveAnimation(MoveState.Run);
                //break;
            case AttackBase.state.Jump:
                break;
        }
        UpdateStepSound();

    }

    protected void UpdateStepSound()
    {
        
        if(_lastTime < 0)
        {
            if(State == MoveState.Run)
            {
                Managers.Sound.Play(_stepClip, Define.Sound.Effect, 0.3f);
                _lastTime = _stepTimeBet;
            }
            
        }
        _lastTime -= Time.deltaTime;
    }

    //해당지점까지 점프.
    public void Jump(Vector3 targetPoint, float durationTime)
    {
        //this.transform.DOMoveX(targetPoint.x, durationTime);
        //this.transform.DOMoveZ(targetPoint.z, durationTime);
        //this.transform.DOMoveY(0.5f, durationTime);
        //_animator.SetFloat("Jump", durationTime);
        //this.transform.DOJump(targetPoint, 0.5f,1, durationTime);

        //_attackBase.state
    }



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
                _animationValue = Mathf.Clamp( Mathf.Lerp(_animationValue, 0, Time.deltaTime * 3 ) , 0, MoveSpeed);
                break;
            case MoveState.Walk:
                _animationValue =Mathf.Clamp( Mathf.Lerp(_animationValue, MoveSpeed * 0.7f, Time.deltaTime * 3) , 0 , MoveSpeed);
                break;
            case MoveState.Run:
                _animationValue = Mathf.Clamp( Mathf.Lerp(_animationValue, MoveSpeed * 1, Time.deltaTime * 3) ,0 , MoveSpeed);
                break;
            case MoveState.Stun:
                _animationValue = -0.1f;
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
            ResultSpeed = MoveSpeed * 0.7f;
        }

        ResultSpeed = ResultSpeed + (_totRatio * ResultSpeed);
        if(_attackBase.State == AttackBase.state.Dash)
        {
            ResultSpeed *= 1.5f;
        }
        Vector3 moveDistance = UtillGame.ConventToVector3(inputMoveVector2.normalized)* ResultSpeed * Time.deltaTime;
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


    public void Dash()
    {

    }

    protected  IEnumerator DashProcess()
    {
        var inputVector = _inputBase.controllerInputDic[InputType.Move].inputVector2;

        yield return new WaitForSeconds(1.0f);
    }

}
