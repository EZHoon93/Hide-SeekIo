using System.Collections;

using UnityEngine;

public class HiderMove : MoveBase , IMakeRunEffect
{
    public Define.MoveHearState HearState { get; set; }
    HiderInput _hiderInput;


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
        OnUpdate(_hiderInput.MoveVector, _hiderInput.IsRun);
    }

    private void Update()
    {
        if(photonView.IsMine == false)
        {
            UpdateMoveAnimation(State);
        }
    }



    public virtual void Stop()
    {
        //_animationValue = 0.0f;
        _animator.SetFloat("Speed", 0.0f);
    }

}
