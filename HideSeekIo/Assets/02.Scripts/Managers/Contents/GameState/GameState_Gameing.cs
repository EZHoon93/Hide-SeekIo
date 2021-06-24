﻿using Photon.Pun;
using TMPro;

using UnityEngine;

public class GameState_Gameing : GameState_Base, IPunObservable
{
    int _initGameTime = 100 ;

    TextMeshProUGUI _inGametimeText;
    TextMeshProUGUI _noticeText;
    TextMeshProUGUI _countDownText;



    protected override void Setup()
    {
        
        //print("셋업 노티스,카운트");
    }

    private void Start()
    {
        var mainSceneUI = Managers.UI.SceneUI as UI_Main;
        _inGametimeText = mainSceneUI.GetText(UI_Main.TextMeshProUGUIs.InGameTime);
        _noticeText = mainSceneUI.GetText(UI_Main.TextMeshProUGUIs.Notice);
        _countDownText = mainSceneUI.GetText(UI_Main.TextMeshProUGUIs.CountDown);



        _initRemainTime = _initGameTime;
        _countDownText.text = null;
        _noticeText.text = null;
        
    }



    protected override void ChangeRemainTime()
    {
        string timeStr = Util.GetTimeFormat(RemainTime);
        _inGametimeText.text = timeStr;
    }

    protected override void EndRemainTime()
    {
        Master_ChangeState(Define.GameState.End);
    }

    //좀비팀승리시. GameManger에서 호출
    public void ZombieTeamWin()
    {
        Master_ChangeState(Define.GameState.End);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            print("End변ㄱㅇ");
            Master_ChangeState(Define.GameState.End);
        }
    }


}