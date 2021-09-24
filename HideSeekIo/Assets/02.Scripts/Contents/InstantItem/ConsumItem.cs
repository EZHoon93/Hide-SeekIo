using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ConsumItem : MonoBehaviourPun
{
    public void OnPhotonInstantiate(PlayerController playerController)
    {
        Managers.InputItemManager.SetupConsumItem(playerController, this);
    }

    public void Use(PlayerController playerController , InputType inputType)
    {
        //이벤트 제거
        //var inputType = inputControllerObject.inputType;
        playerController.playerShooter.consumItem = null;
        playerController.playerInput.RemoveInputEvent(inputType);
    }
}
