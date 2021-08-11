
using Photon.Pun;

using UnityEngine;
public class HiderMove : MoveBase , IMakeRunEffect
{
    public Define.MoveHearState HearState { get; set; }
    public float MaxEnergy { get; private set; } = 16;
    public float CurrentEnergy { get; set; }
    public bool Run { get; set; }
    float lastTime;
    [SerializeField] float timeBiet = 1.5f;
    protected override void Awake()
    {
        base.Awake();
    }
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        HearState = Define.MoveHearState.NoEffect;
        CurrentEnergy = MaxEnergy;
        lastTime = 0;

        _inputBase = GetComponent<InputBase>();

        _inputBase.AddInputEvent(InputType.Main, ControllerInputType.Down, (v) => { Run = true; });

    }
    public bool IsLocal()
    {
        return photonView.IsMine;
    }
    private void LateUpdate()
    {
        if (photonView.IsMine == false) return;
        UpdateEnergy();
        Run = false;
    }
    //protected void FixedUpdate()
    //{
    //    if (photonView.IsMine == false) return;
    //    //OnUpdate(_hiderInput.MoveVector, _hiderInput.IsRun);
    //    UpdateMoveEffect();
    //    UpdateEnergy();
    //}

    protected void FixedUpdate()
    {
        if (photonView.IsMine == false) return;
        OnUpdate(_inputBase.controllerInputDic[InputType.Move].inputVector2, Run);
        //UpdateStepSound();
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        var move = new Vector2(h, v);
#if UNITY_EDITOR
        //if (this.IsMyCharacter())
        //{
        //    OnUpdate(move, Run);
        //}

#endif

    }

    void UpdateEnergy()
    {
        switch (State)
        {
            case MoveState.Run:
                CurrentEnergy = Mathf.Clamp(CurrentEnergy -  1* Time.deltaTime, 0, MaxEnergy);
                break;
            case MoveState.Idle:
            case MoveState.Walk:
                CurrentEnergy = Mathf.Clamp(CurrentEnergy + 0.5f* Time.deltaTime, 0, MaxEnergy);
                break;
        }
    }
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
                if (Time.time >= lastTime + timeBiet)
                {
                    lastTime = Time.time;
                    CreateEffect();
                }
                break;
        }
    }

    public override void OnUpdate(Vector2 inputVector2, bool isRun)
    {
        MoveSpeed = _testSpeed;
        switch (_attackBase.State)
        {
            case AttackBase.state.Idle:
                if (CurrentEnergy > 0)
                {
                    print(inputVector2);
                    UpdateSmoothRotate(inputVector2);
                    UpdateMove(inputVector2, isRun);
                }
                else
                {
                    //_inputBase.EnegyZero();
                    UpdateSmoothRotate(Vector2.zero);
                    UpdateMove(Vector2.zero, isRun);
                }
                UpdateMoveAnimation(State);
                break;
            case AttackBase.state.Attack:
                //UpdateImmediateRotate(_attackBase.AttackDirection);
                //UpdateMoveAnimation(MoveState.Idle);
                break;
        }

    }

    public virtual void Stop()
    {
        //_animationValue = 0.0f;
        _animator.SetFloat("Speed", 0.0f);
    }


    void CreateEffect()
    {
        photonView.RPC("CreateEffectOnLocal", RpcTarget.All);
    }

    [PunRPC]
    void CreateEffectOnLocal()
    {
        var pos = this.transform.position;
        pos.y = 0;
        if (CameraManager.Instance.Target == null)
        {
            EffectManager.Instance.EffectOnLocal(Define.EffectType.Ripple, pos, 0);

            EffectManager.Instance.EffectToServer(Define.EffectType.Dust, pos, 0);
        }
        else if (CameraManager.Instance.Target.Team == Define.Team.Hide)
        {
            EffectManager.Instance.EffectOnLocal(Define.EffectType.Ripple, pos, 0);

            //EffectManager.Instance.EffectToServer(Define.EffectType.Dust, this.transform.position, 0);
        }
        else
        {
            if(Vector3.Distance(this.transform.position , CameraManager.Instance.Target.transform.position) <= 4)
            {
                EffectManager.Instance.EffectToServer(Define.EffectType.Ripple, pos, 0);
            }
        }
    }

}
