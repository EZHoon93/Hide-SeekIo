using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : InputBase
{
    public override void OnPhotonInstantiate()
    {
        base.OnPhotonInstantiate();
        if (this.photonView.IsMine)
        {
            InputManager.Instance.SetActiveSeekerController(true);
        }
    }

    private void Update()
    {
        if (photonView.IsMine == false) return;
        MoveVector = InputManager.Instance.MoveVector;
    }
}
