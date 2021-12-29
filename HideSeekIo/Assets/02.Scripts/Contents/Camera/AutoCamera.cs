using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCamera : CameraBase
{
    public override void Init()
    {
        var currentTargetPlayer = Managers.cameraManager.cameraTagerPlayer;
        switch (Managers.Game.gameStateType)
        {

            case Define.GameState.Wait:
            case Define.GameState.CountDown:
                Camera.main.cullingMask = Managers.cameraManager._initObjectModeLayer;
                Managers.cameraManager.worldUICamera.gameObject.SetActive(false);
                break;
            case Define.GameState.Gameing:
            case Define.GameState.GameReady:
            case Define.GameState.End:
                Managers.cameraManager.FindNextPlayer();
                break;
        }

        Managers.Input.SetActiveController(false);
    }

   
}
