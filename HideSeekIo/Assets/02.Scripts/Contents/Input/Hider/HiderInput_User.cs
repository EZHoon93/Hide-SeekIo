using System.Collections;

using UnityEngine;

public class HiderInput_User : HiderInput
{
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        if (this.photonView.IsMine)
        {
            InputManager.Instance.SetActiveHiderController(true);
            InputManager.Instance.baseAttackJoystick.onAttackEventCallBack = Call_AttackCallBackEvent;
            InputManager.Instance.itemControllerJoysticks[0].onAttackEventCallBack = CallBackItem1;
            IsRun = true;

        }
    }

    protected override void HandleDeath()
    {
        if (photonView.IsMine)
        {
            InputManager.Instance.SetActiveHiderController(false);

        }
    }
    private void Update()
    {
        OnUpdate();
    }
    public void OnUpdate()
    {
        if (IsStop)
        {
            UpdateStopState();
            return;
        }
        MoveVector = InputManager.Instance.MoveVector;
        IsRun = InputManager.Instance.IsRun;
        AttackVector = InputManager.Instance.baseAttackJoystick.InputVector2;
        ItemVector1 = InputManager.Instance.itemControllerJoysticks[0].InputVector2;
        //UtillGame.UpdateUserAttackInput(ref _attackVector, ref _lastAttackVector, ref _isAttack);
    }

   

    public override void EnegyZero()
    {
        InputManager.Instance.IsRun = false;
    }
    //스탑 상태에서 발생
    protected void UpdateStopState()
    {
        if (_stopTime > 0)
        {
            _stopTime -= Time.deltaTime;
            MoveVector = Vector2.zero;
        }
        else
        {
            IsStop = false;
        }
    }
}
