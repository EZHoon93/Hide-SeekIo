using System.Collections;

using UnityEngine;
using Photon.Pun;
public class GameState_End : GameState_Base, IPunObservable
{
    int _initSceneWaitTime = 10;  //다음씬으로 넘어가기위한 대기시간


    int _zombiedefatExp = 10;
    int _zombiedefatGem = 10;
    int _zombieWinAddExp = 20;   //좀비가 이겼을시 기본 경험치 추가량
    int _zombieWinAddGem = 50;   //기본 젬 추가량
    int _addGemByKill = 10;  //킬당 추가 돈
    int _addExpByKill = 5;  //킬당 추가 경험치

    int _humanWinExp = 15;
    int _humanWinGem  = 15;
    int _humanDefeatExp = 5;
    int _humanDefeatGem = 5;


    //UI_Text_Menu _timeText;
    //UI_Text_Menu _noticeText;


    protected override void Setup()
    {
        _initRemainTime = _initSceneWaitTime;
        //_timeText = UIManager.instance.GetMenuText(UI_Text_Menu.Type.CountDown);
        //_noticeText = UIManager.instance.GetMenuText(UI_Text_Menu.Type.Notice);

        
        //Master_ProceesWin();
    }
    protected override void ChangeRemainTime()
    {
        //_timeText.UpdateText(RemainTime.ToString());
        //if(RemainTime < 5)
        //{
        //    _noticeText.UpdateText("잠시 후 다른 맵으로 이동합니다");
        //}
    }

    protected override void EndRemainTime()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("NextSceneLoad", RpcTarget.AllViaServer);
        }
    }

    ////다음맵으로이동
    //[PunRPC]
    //public void NextSceneLoad()
    //{
    //    print("NextSceneLoad");
    //    var lists = FindObjectsOfType<Photon.Pun.PhotonView>();
    //    foreach(var l in lists)
    //    {
    //        if (l.IsMine)
    //        {
    //            PhotonNetwork.Destroy(l);
    //        }
    //    }
    //    ObjectPoolManager.instance.Clear();
    //    PhotonNetwork.LoadLevel("Main");
    //    //PhotonNetwork.LeaveRoom();
    //}

    ////방장은 누가이겼는지 판별
    //void Master_ProceesWin()
    //{
    //    if (PhotonNetwork.IsMasterClient == false) return;
    //    Define.Team winTeam;
    //    //if (GameManager.instance.HumanCount == 0)
    //    //{
    //    //    //좀비 승리
    //    //    winTeam = Define.Team.Zombie;
    //    //}
    //    //else
    //    //{
    //    //    //인간 승리
    //    //    winTeam = Define.Team.Human;
    //    //}

    //    photonView.RPC("OnEventWin", RpcTarget.AllViaServer, winTeam);

    //}

    ////누가이겼는지
    //[PunRPC]
    //public void OnEventWin(Define.Team winTeam)
    //{

    //    Process_User(winTeam);

    //    switch (winTeam)
    //    {
    //        case Define.Team.Human:
    //            _noticeText.UpdateFadeText("인간 팀 승리", 0.01f);
    //            break;
    //        case Define.Team.Zombie:
    //            _noticeText.UpdateFadeText("좀비 팀 승리", 0.01f);
    //            break;
    //    }
    //}


    void Process_User(Define.Team winTeam)
    {
        //var myPlayer = GameManager.instance.myPlayer;
        //EndGameInfo endGameInfo;
        //endGameInfo.exp = 0;
        //endGameInfo.gem = 0;

        //switch (myPlayer.Team)
        //{
        //    case Define.Team.Human:
        //        endGameInfo = Process_HumanUser(winTeam);
        //        break;
        //    case Define.Team.Zombie:
        //        endGameInfo = Process_ZombieUser(winTeam);
        //        break;
        //}
        //Util.CallBackFunction(this, 3.0f, () =>{
        //    PlayerInfo.EndGame(endGameInfo.exp, endGameInfo.gem);
        //});
       
    }

    ////자기자신이 좀비팀일경우
    //EndGameInfo Process_ZombieUser(Define.Team winTeam  )
    //{
    //    EndGameInfo endGameInfo;
    //    endGameInfo.exp = 0;
    //    endGameInfo.gem = 0;
    //    var zombieController = GameManager.instance.myPlayer as ZombieController;
    //    switch (winTeam)
    //    {
    //        case Define.Team.Human:
    //            endGameInfo.gem = _zombiedefatGem;
    //            endGameInfo.exp = _zombiedefatExp;
    //            break;
    //        case Define.Team.Zombie:
    //            endGameInfo.gem = _zombieWinAddGem + zombieController.KillCount * _addGemByKill;
    //            endGameInfo.exp = _zombieWinAddExp + zombieController.KillCount * _addExpByKill;

    //            break;
    //    }

    //    return endGameInfo;
    //}



    ////자가 자신이 휴먼일경우
    //EndGameInfo Process_HumanUser(Define.Team winTeam)
    //{
    //    EndGameInfo endGameInfo;
    //    endGameInfo.exp = 0;
    //    endGameInfo.gem = 0;
    //    switch (winTeam)
    //    {
    //        case Define.Team.Human:
    //            endGameInfo.gem = _humanWinGem;
    //            endGameInfo.exp = _humanWinExp;
    //            break;
    //        case Define.Team.Zombie:
    //            endGameInfo.gem = _humanDefeatGem;
    //            endGameInfo.exp = _humanDefeatExp;

    //            break;
    //    }

    //    return endGameInfo;
    //}
}
