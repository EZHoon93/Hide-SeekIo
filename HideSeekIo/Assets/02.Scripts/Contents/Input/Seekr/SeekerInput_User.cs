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
            InputManager.Instacne.SetActiveSeekerController(true);
            var uiMain = Managers.UI.SceneUI as UI_Main;
            uiMain.FindButton.gameObject.SetActive(false);
        }
    }

    protected override void HandleDeath()
    {
        if (this.IsMyCharacter())
        {
            InputManager.Instacne.SetActiveSeekerController(false);
        }
    }

    public void OnUpdate()
    {
        MoveVector = InputManager.Instacne.MoveVector;
        UtillGame.UpdateUserAttackInput(ref _attackVector, ref _lastAttackVector, ref _isAttack);
    }
}
