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
    }
}
