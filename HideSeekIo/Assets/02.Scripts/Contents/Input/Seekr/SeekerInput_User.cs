using System.Collections;

using UnityEngine;

public class SeekerInput_User : SeekerInput
{

    private void Update()
    {
        if (!photonView.IsMine) return;
        OnUpdate();
    }
    public override void OnPhotonInstantiate()
    {
        if (this.IsMyCharacter())
        {
            InputManager.Instance.SetActiveSeekerController(true);
            var uiMain = Managers.UI.SceneUI as UI_Main;
            uiMain.FindButton.gameObject.SetActive(false);
        }
    }

    protected override void HandleDeath()
    {
        if (this.IsMyCharacter())
        {
            InputManager.Instance.SetActiveSeekerController(false);
        }
    }

    public void OnUpdate()
    {
        if (IsStop)
        {
            UpdateStopState();
            return;
        }
        MoveVector = InputManager.Instance.MoveVector;
        //UtillGame.UpdateUserAttackInput(ref _attackVector, ref _lastAttackVector, ref _isAttack);
    }

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
