using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCamera : CameraBase
{
    public override void Init()
    {
        var currentTargetPlayer = Managers.CameraManager.cameraTagerPlayer;
        switch (Managers.Game.gameStateType)
        {

            case Define.GameState.Wait:
            case Define.GameState.CountDown:
                Camera.main.cullingMask = Managers.CameraManager._initObjectModeLayer;
                Managers.CameraManager.worldUICamera.gameObject.SetActive(false);
                break;
            case Define.GameState.Gameing:
            case Define.GameState.GameReady:
            case Define.GameState.End:
                Managers.CameraManager.FindNextPlayer();
                break;
        }

        Managers.Input.SetActiveController(false);
    }

   
}
