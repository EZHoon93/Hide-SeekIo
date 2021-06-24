using System.Collections;
using System.IO.Pipes;

using Photon.Pun;

using UnityEngine;

public class SeekerMove : MoveBase
{
    CharacterController _characterController;
    SeekerAttack _seekerAttack;
    SeekerInput _seekerInput;
    protected Animator _animator;


    [SerializeField] float _testSpeed;


    //public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    base.OnPhotonSerializeView(stream, info);
    //    switch (_seekerAttack.State)
    //    {
    //        case SeekerAttack.state.Attack:
    //            this.transform.rotation = UtillGame.GetWorldRotation_ByInputVector(_seekerAttack.weapon.LastAttackInput);
    //            print("씨꺼");
    //            break;
    //    }

    //    //if (stream.IsWriting)
    //    //{
    //    //    stream.SendNext(_seekerAttack.State);
    //    //}
    //    //else
    //    //{
    //    //    var reciveState = (SeekerAttack.state)stream.ReceiveNext();
    //    //    if (reciveState == _seekerAttack.State) return;
    //    //    switch (reciveState)
    //    //    {
    //    //        case SeekerAttack.state.Attack:
    //    //            _animator.SetTrigger("Attack");
    //    //            this.transform.rotation = UtillGame.GetWorldRotation_ByInputVector(_seekerAttack.weapon.LastAttackInput);
    //    //            break;
    //    //    }

    //    //}
    //}
    protected override void Awake()
    {
        base.Awake();
        _seekerAttack = GetComponent<SeekerAttack>();
        _seekerInput = GetComponent<SeekerInput>();
        _characterController = GetComponent<CharacterController>();
    }

    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        _animator = GetComponentInChildren<Animator>();

    }

    protected override void Update()
    {
        base.Update();
        //if (photonView.IsMine == false)
        //{
        //    switch (_seekerAttack.State)
        //    {
        //        case SeekerAttack.state.Attack:
        //            this.transform.rotation = UtillGame.GetWorldRotation_ByInputVector(_seekerAttack.weapon.LastAttackInput);
        //            break;
        //    }
        //}
    }


    protected void FixedUpdate()
    {
        print("이동" + _seekerAttack.State);
        MoveSpeed = _testSpeed;
        if (!photonView.IsMine) return;
        switch (_seekerAttack.State)
        {
            case SeekerAttack.state.Idle:
                UpdateMove(_seekerInput.MoveVector, MoveSpeed);
                UpdateRotate(_seekerInput.MoveVector);
                UpdateAnimation(ResultSpeed);
                break;
            case SeekerAttack.state.Attack:
                //var quaternion = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
                //var temp = new Vector3(_seekerAttack.AttackTargetDirection.x, 0, _seekerAttack.AttackTargetDirection.z).normalized;
                //var temp = new Vector3(_seekerAttack.weapon.AttackDirecition.x, 0, _seekerAttack.weapon.AttackDirecition.z).normalized;
                //var newDirection = quaternion * temp;
                //Quaternion newRotation = Quaternion.LookRotation(newDirection);
                //this.transform.rotation = newRotation;
                this.transform.rotation = UtillGame.GetWorldRotation_ByInputVector(_seekerAttack.weapon.LastAttackInput);
                UpdateAnimation(0);

                break;

        }
    }

    protected virtual void UpdateMove(Vector2 inputMoveVector2, float moveSpeed)
    {
        float resultSpeed = 0;
        if (inputMoveVector2.sqrMagnitude == 0)
        {
            resultSpeed = 0;
        }
        else
        {
            resultSpeed = moveSpeed;
        }
        ResultSpeed = resultSpeed + (_totRatio * resultSpeed);
        //Vector3 moveDistance = this.transform.forward * ResultSpeed * Time.deltaTime;
        var quaternion = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
        var temp = new Vector3(inputMoveVector2.x, 0, inputMoveVector2.y).normalized;
        var newDirection = quaternion * temp;



        Vector3 moveDistance = newDirection * ResultSpeed * Time.deltaTime;
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


    protected virtual void UpdateAnimation(float newAnimationValue)
    {
        //_animationValue = Mathf.Lerp(_animationValue, newAnimationValue, Time.deltaTime * 3);
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

}
