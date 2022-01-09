using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class UI_FindCamera : UI_Button
{
    public override void Init()
    {
        base.Init();
        //Managers.CameraManager.cameraStateChangeEvent += ChangeCameraState;

        //Managers.Game.AddListenrOnGameEvent(Define.GameEvent.MyPlayerActive, CallBack_MyPlayerSetActive);
        //Managers.Game.AddListenrOnGameEvent(Define.GameEvent.ChangeState,
          //CallBack_ChangeGameState);
    }



    void CallBack_MyPlayerSetActive(object isJoin)
    {
        this.gameObject.SetActive(!(bool)isJoin);
    }

    void CallBack_ChangeGameState(object gameState)
    {
        switch((Define.GameState)gameState)
        {
            case Define.GameState.Wait:
            case Define.GameState.CountDown:
                this.gameObject.SetActive(false);
                break;
        }
    }
    //void ChangeCameraState(Define.CameraState cameraState)
    //{
    //    switch (cameraState)
    //    {
    //        case Define.CameraState.Auto:
    //        case Define.CameraState.Observer:
    //            bool active = Managers.Game.gameStateType > Define.GameState.GameReady ? true : false;
    //            this.gameObject.SetActive(active);
    //            break;
    //        case Define.CameraState.MyPlayer:
    //            this.gameObject.SetActive(false);
    //            break;
    //    }
    //}

    protected override void OnClickEvent()
    {
        Managers.CameraManager.FindNextPlayer();
    }
}
