
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameState_End : GameState_Base, IPunObservable
{
    int _initSceneWaitTime = 5;  //다음씬으로 넘어가기위한 대기시간


    TextMeshProUGUI _inGametimeText;
    TextMeshProUGUI _noticeText;
    TextMeshProUGUI _countDownText;

    protected override void Setup()
    {
        var mainSceneUI = Managers.UI.SceneUI as UI_Main;
        _inGametimeText = mainSceneUI.GetText(UI_Main.TextMeshProUGUIs.InGameTime);
        _noticeText = mainSceneUI.GetText(UI_Main.TextMeshProUGUIs.Notice);
        _countDownText = mainSceneUI.GetText(UI_Main.TextMeshProUGUIs.CountDown);


        _countDownText.text = null;
        _noticeText.text = null;

        _initRemainTime = _initSceneWaitTime;
  
    }
    protected override void ChangeRemainTime()
    {
        _countDownText.text = RemainTime.ToString();
    }

    protected override void EndRemainTime()
    {
        print(GameManager.Instance.CurrentGameScene.SceneType + "현재씬타입");
        Managers.Scene.MasterSelectNextMainScene(GameManager.Instance.CurrentGameScene.SceneType);
    }


   
}
