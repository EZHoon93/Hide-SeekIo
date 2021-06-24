using System.Collections;
using System.IO.Pipes;

using Photon.Pun;

using UnityEngine;

public class SeekerMove : MoveBase
{
    SeekerInput _seekerInput => _inputBase as SeekerInput;


    [SerializeField] float _testSpeed;


  

    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
    }


    void OnUpdate()
    {
        if (photonView.IsMine == false) return;
        MoveSpeed = _testSpeed;
        switch (_attackBase.State)
        {
            case AttackBase.state.Idle:
                UpdateSmoothRotate(_seekerInput.MoveVector);
                UpdateMove(_seekerInput.MoveVector, true);
                UpdateMoveAnimation(State);
                break;
            case AttackBase.state.Attack:
                UpdateImmediateRotate(_attackBase.weapon.LastAttackInput);
                UpdateMoveAnimation(MoveState.Stun);
                break;
        }

    }

    protected void FixedUpdate()
    {
        OnUpdate();
    }

    //protected virtual void UpdateMove(Vector2 inputMoveVector2, float moveSpeed)
    //{
    //    float resultSpeed = 0;
    //    if (inputMoveVector2.sqrMagnitude == 0)
    //    {
    //        resultSpeed = 0;
    //    }
    //    else
    //    {
    //        resultSpeed = moveSpeed;
    //    }
    //    ResultSpeed = resultSpeed + (_totRatio * resultSpeed);
    //    //Vector3 moveDistance = this.transform.forward * ResultSpeed * Time.deltaTime;
    //    var quaternion = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
    //    var temp = new Vector3(inputMoveVector2.x, 0, inputMoveVector2.y).normalized;
    //    var newDirection = quaternion * temp;



    //    Vector3 moveDistance = newDirection * ResultSpeed * Time.deltaTime;
    //    if (!_characterController.isGrounded)
    //    {
    //        moveDistance.y -= 9.8f * Time.deltaTime;
    //    }
    //    _characterController.Move(moveDistance);
    //}
    //protected virtual void UpdateRotate(Vector2 inputMoveVector2)
    //{
    //    if (inputMoveVector2.normalized.sqrMagnitude == 0) return;
    //    var quaternion = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
    //    var temp = new Vector3(inputMoveVector2.x, 0, inputMoveVector2.y).normalized;
    //    var newDirection = quaternion * temp;
    //    Quaternion newRotation = Quaternion.LookRotation(newDirection);
    //    this.transform.rotation = Quaternion.Slerp(this.transform.localRotation, newRotation, RotationSpeed * Time.deltaTime);    //즉시변환
    //}


    //protected virtual void UpdateAnimation(float newAnimationValue)
    //{
    //    //_animationValue = Mathf.Lerp(_animationValue, newAnimationValue, Time.deltaTime * 3);
    //    //_animator.SetFloat("Speed", _animationValue);
    //}


}
