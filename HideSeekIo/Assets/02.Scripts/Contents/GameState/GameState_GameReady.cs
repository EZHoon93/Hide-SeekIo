using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

//바로 바뀌는거 방지를 위한
public class GameState_GameReady : GameState_Base
{

    public override float remainTime => _gameScene.initReadyTime;
    int _totSeekerCount => _gameScene.totSeekerCount;

    Dictionary<int, Dictionary<string, object>> _playerDataTable;



    /// <summary>
    /// GameReady,Start는 게임씬 데이터 이용
    /// /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        var gameScene = _gameScene;
        GetComponent<GameStateController>().ChangeInitTime(gameScene.initReadyTime);
    }
    public override void OnPhotonInstantiate(PhotonMessageInfo info, float createServerTime)
    {
        var inGameTime = _gameScene.initGameTime;
        switch (_gameScene.gameMode)
        {
            case Define.GameMode.Object:
                uI_Main.UpdateNoticeText(Util.GetColorContent(Color.white, "잠시 후 술래가 등장합니다 "));

                break;
            case Define.GameMode.Item:
                break;
        }
        _playerDataTable = (Dictionary<int, Dictionary<string, object>>)info.photonView.InstantiationData[1];
        
        //캐릭터 생성 
        Managers.Scene.currentGameScene.PlayerSpawnOnGameReady(_playerDataTable);
    }
    public override void OnUpdate(int newTime)
    {
        uI_Main.UpdateCountText(newTime);
        Managers.Sound.Play("TimeCount", Define.Sound.Effect);
    }
    public override void OnTimeEnd()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            NextScene(Define.GameState.Gameing, _playerDataTable);    //게임 상태바꿈
        }
    }

    //void MakeTeamList(int totSeekerCount)
    //{
    //    var allHiderList = Managers.Game.GetAllHiderList().ToList();
    //    List<int> selectSeekerViewIDLIst = new List<int>(10);

    //    for (int i = 0; i < totSeekerCount; i++)
    //    {
    //        int ran = Random.Range(0, allHiderList.Count);
    //        selectSeekerViewIDLIst.Add(allHiderList[ran].ViewID()); //술래로 등록
    //        allHiderList.RemoveAt(ran); //숨는팀 목록에서 삭제

    //    }
    //    Hashtable sendSeekrHashData = new Hashtable()
    //        {
    //            { "se", selectSeekerViewIDLIst.ToArray()},
    //        };
    //    Managers.photonGameManager.SendEvent(Define.PhotonOnEventCode.TeamSelect, Photon.Realtime.EventCaching.AddToRoomCacheGlobal, sendSeekrHashData);
    //}



}
