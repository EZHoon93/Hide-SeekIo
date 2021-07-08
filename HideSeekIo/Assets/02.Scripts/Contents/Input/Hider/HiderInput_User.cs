using System.Collections;

using UnityEngine;

public class HiderInput_User : HiderInput
{
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        if (this.photonView.IsMine)
        {
            InputManager.Instacne.SetActiveHiderController(true);
            var uiMain = Managers.UI.SceneUI as UI_Main;
            uiMain.FindButton.gameObject.SetActive(false);
            IsRun = true;

        }
    }

    protected override void HandleDeath()
    {
        if (photonView.IsMine)
        {
            InputManager.Instacne.SetActiveHiderController(false);

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
        MoveVector = InputManager.Instacne.MoveVector;
        IsRun = InputManager.Instacne.IsRun;
        UtillGame.UpdateUserAttackInput(ref _attackVector, ref _lastAttackVector, ref _isAttack);
    }

    public override void EnegyZero()
    {
        InputManager.Instacne.IsRun = false;
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
