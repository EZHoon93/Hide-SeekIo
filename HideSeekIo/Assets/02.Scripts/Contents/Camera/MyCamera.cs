using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCamera : CameraBase
{
    public override void Init()
    {
        var myPlayer = Managers.Game.myPlayer;
        if (myPlayer)
        {
            Managers.cameraManager.SetupTargetPlayerController(myPlayer);
            //myPlayer.playerInput.SetActiveUserControllerJoystick(true);
        }
    }
}
