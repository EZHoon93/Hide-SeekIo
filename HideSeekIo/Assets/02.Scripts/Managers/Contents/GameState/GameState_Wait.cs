using Photon.Pun;
using UnityEngine;
using TMPro;

//바로 바뀌는거 방지를 위한
public class GameState_Wait : GameState_Base
{
    TextMeshProUGUI _countDownText;
    TextMeshProUGUI _noticeText;
    TextMeshProUGUI _DeathInfoText;


    /// <summary>
    /// 리셋
    /// </summary>
    protected override void Setup()
    {
        var mainSceneUI = Managers.UI.SceneUI as UI_Main;
        mainSceneUI.ResetTexts();

        _initRemainTime = 1;

        CameraManager.Instance.SetupTarget(Managers.Game.CurrentGameScene.CameraView);
    }



    protected override void ChangeRemainTime()
    {

        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2 && RemainTime <= 0)
        {
            Master_ChangeState(Define.GameState.CountDown);
        }
    }

    //아무것도안함.
    protected override void EndRemainTime()
    {
    }

    //테스트
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Master_ChangeState(Define.GameState.CountDown);
        }
    }

    public void Test()
    {
        Master_ChangeState(Define.GameState.CountDown);
    }

}
