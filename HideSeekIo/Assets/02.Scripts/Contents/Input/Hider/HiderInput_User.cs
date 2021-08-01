
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
            InputManager.Instance.itemControllerJoysticks[1].onAttackEventCallBack = CallBackItem2;

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
#if UNITY_ANDROID
        MoveVector = InputManager.Instance.moveJoystick.InputVector2 * RandomVector2;
#endif
        IsRun = InputManager.Instance.IsRun;
        AttackVector = InputManager.Instance.baseAttackJoystick.InputVector2;
        ItemVector1 = InputManager.Instance.itemControllerJoysticks[0].InputVector2;
        ItemVector2 = InputManager.Instance.itemControllerJoysticks[1].InputVector2;
        //UtillGame.UpdateUserAttackInput(ref _attackVector, ref _lastAttackVector, ref _isAttack);
#if UNITY_EDITOR
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        MoveVector = new Vector2(h, v) * RandomVector2;
#endif
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
