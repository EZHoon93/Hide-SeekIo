
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameState_End : GameState_Base, IPunObservable
{
    int _initSceneWaitTime = 5;  //다음씬으로 넘어가기위한 대기시간
    protected override void Setup()
    {
        _initRemainTime = _initSceneWaitTime;
    }
    protected override void ChangeRemainTime()
    {
        uI_Main.UpdateCountText(RemainTime);
    }

    protected override void EndRemainTime()
    {
        
        Managers.Scene.MasterSelectNextMainScene(Managers.Game.CurrentGameScene.SceneType);
    }


   
}
