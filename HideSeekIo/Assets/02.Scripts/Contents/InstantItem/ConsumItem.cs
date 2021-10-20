using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ConsumItem : MonoBehaviourPun
{
    PlayerController _playerController;
    public void OnPhotonInstantiate(PlayerController playerController)
    {
        _playerController = playerController;
        //Managers.InputItemManager.SetupConsumItem(playerController, this);
    }

    public void Use()
    {
        //이벤트 제거
        _playerController.playerShooter.consumItem = null;

        //var inputType = inputControllerObject.inputType;
        //playerController.playerShooter.consumItem = null;
        //playerController.playerInput.RemoveInputEvent(inputType);
    }
}
