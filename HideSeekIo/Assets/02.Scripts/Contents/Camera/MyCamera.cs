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
            Managers.CameraManager.SetupTargetPlayerController(myPlayer);
            //myPlayer.playerInput.SetActiveUserControllerJoystick(true);
            OnCallBack_ChangeTeam(myPlayer.Team);
            myPlayer.playerHealth.onChangeTeamEvent += OnCallBack_ChangeTeam;
        }
    }

    private void OnDisable()
    {
        var myPlayer = Managers.Game.myPlayer;
        if (myPlayer)
        {
            var worldUICamera = Managers.CameraManager.worldUICamera;
            //worldUICamera.cullingMask = UtillLayer.GetUILayerByTeam(myPlayer.Team);
            myPlayer.playerHealth.onChangeTeamEvent -= OnCallBack_ChangeTeam;
        }
    }

    void OnCallBack_ChangeTeam(Define.Team team)
    {
        var worldUICamera = Managers.CameraManager.worldUICamera;
        worldUICamera.cullingMask = 1 << UtillLayer.GetUILayerByTeam(team);
        worldUICamera.gameObject.SetActive(true);
    }
}
