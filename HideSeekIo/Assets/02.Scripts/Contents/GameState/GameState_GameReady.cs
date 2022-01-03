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
        //GetComponent<GameStateController>().ChangeInitTime(_gameScene.initReadyTime);
    }
    public override void OnPhotonInstantiate(PhotonMessageInfo info, float createServerTime)
    {
        _playerDataTable = (Dictionary<int, Dictionary<string, object>>)info.photonView.InstantiationData[1];
        var localActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        switch (_gameScene.gameMode)
        {
            case Define.GameMode.Object:

                if (_playerDataTable.ContainsKey(localActorNumber))
                {
                    var team = (Define.Team)_playerDataTable[localActorNumber]["te"];
                    if(team == Define.Team.Hide)
                    {
                        uI_Main.UpdateNoticeText(Util.GetColorContent(Color.white, "잠시 후 술래가 등장합니다 "));
                    }
                    else
                    {
                        uI_Main.UpdateNoticeText(Util.GetColorContent(Color.white, "당신은 술래입니다."));
                    }
                }

                break;
            case Define.GameMode.Item:
                break;
        }
        
        //캐릭터 생성 방장만 실행..
        Managers.Scene.currentGameScene.PlayerSpawnOnGameReady(_playerDataTable);

    }
    public override void OnUpdate(int newTime)
    {
        uI_Main.UpdateCountText(newTime);
        Managers.Game.GetHiderCount();
        Managers.Sound.Play("TimeCount", Define.Sound.Effect);

    }
    public override void OnTimeEnd()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            NextScene(Define.GameState.Gameing, _playerDataTable);    //게임 상태바꿈
        }
    }


}
