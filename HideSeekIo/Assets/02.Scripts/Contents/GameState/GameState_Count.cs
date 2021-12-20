
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Data;
using ExitGames.Client.Photon;
using System;

public class GameState_Count : GameState_Base
{
    public override float remainTime => 10;

    public override void OnPhotonInstantiate(PhotonMessageInfo info, float createServerTime)
    {
        uI_Main = Managers.UI.SceneUI as UI_Main;
        uI_Main.UpdateNoticeText("잠시 후 게임이 시작됩니다.");
    }
  
    public override void OnUpdate(int remainTime)
    {
        uI_Main.UpdateCountText(remainTime);
        Managers.Sound.Play("TimeCount", Define.Sound.Effect);
    }
  
    /// <summary>
    /// 카운트가 끝나면 술래 미리 정해주고 각 플레이어 캐릭터생성 
    /// 다음 상태로 넘어감.
    /// </summary>
    public override void OnTimeEnd()
    {
        //방장만 실행.. 캐릭터 생성.
        if (PhotonNetwork.IsMasterClient)
        {
            var gameScene = Managers.Scene.currentGameScene;
            var maxPlayerCount = gameScene.maxPlayerCount;
            var seekerCount = gameScene.totSeekerCount;
            //Dictionary<int, object[]> playerTeamDic = new Dictionary<int, object[]>();
            
            Dictionary<int, Dictionary<string, object>> playerDataTable = new Dictionary<int, Dictionary<string, object>>();
    
            
            //참여한 유저 추가
            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values.Where(s => (bool)s.CustomProperties["jn"] == true).ToList())
            {
                var HT = player.CustomProperties;
                playerDataTable.Add(player.ActorNumber, new Dictionary<string, object>()
                {
                    //["nu"] = player.ActorNumber,        //넘버
                    ["nn"] = player.NickName ,          //닉네임
                    ["te"] = Define.Team.Hide,           //팀
                    ["ch"] = HT["ch"],                          //캐릭스킨
                    ["we"] = HT["we"],                        //무기스킨
                    ["ac"] = HT["ac"],                        //악세스킨
                });
            }


            //AI 추가
            Managers.aIManager.SetupRandomSkinnfo(playerDataTable, maxPlayerCount);

            //var s = from data in playerDataTable where( )

            //술래 정함
            var seekrDic = playerDataTable.Keys.OrderBy(g => Guid.NewGuid()).Take(seekerCount);
            //술래 데이터로 변경
            foreach(var seekerKey in seekrDic)
            {
                if (playerDataTable.ContainsKey(seekerKey))
                {
                    playerDataTable[seekerKey]["te"] = Define.Team.Seek;
                }
            }

            //playerDataTable[PhotonNetwork.LocalPlayer.ActorNumber]["te"] = Define.Team.Seek;    //테스트 로컬 술래

            NextScene(Define.GameState.GameReady , playerDataTable);   //다음 게임 단계로 진행
        }
    }


    /// <summary>
    /// 참여한 유저 리스트를 받아옴.
    /// </summary>
    /// <returns></returns>
    void AddJoinUserSkinInfo(ref List<SendAllSkinInfo> sendAllSkinInfosList)
    {
        var playerList = PhotonNetwork.CurrentRoom.Players.Values.Where(s => (bool)s.CustomProperties["jn"] == true).ToList();
        foreach (var p in playerList)
        {
            SendAllSkinInfo sendAllSkinInfo = new SendAllSkinInfo();
            sendAllSkinInfo.autoNumber = p.ActorNumber;
            sendAllSkinInfo.avaterKey =(int)p.CustomProperties["ch"];
            sendAllSkinInfo.nickName = p.NickName;
            sendAllSkinInfo.team = Define.Team.Hide;    
            sendAllSkinInfosList.Add(sendAllSkinInfo);
        }
    }

    /// <summary>
    /// AI 추가
    /// </summary>
    void AddAISkinInfo(ref List<SendAllSkinInfo> sendAllSkinInfosList)
    {
        //AI로 채움
        var maxPlayerCount = _gameScene.maxPlayerCount;
        for (int i = sendAllSkinInfosList.Count; i < maxPlayerCount; i++)
        {
            //SendAllSkinInfo sendAllSkinInfo = AIManager.Instance.GetSendAllSkinInfo();
            //sendAllSkinInfosList.Add(sendAllSkinInfo);
        }
    }

    void SelectSeeker(List<SendAllSkinInfo> sendAllSkinInfosList)
    {
        //술래 정해줌
        var seekerCount = _gameScene.totSeekerCount;
        //var seekrList = sendAllSkinInfosList.OrderBy(g => Guid.NewGuid()).Take(seekerCount);
        //foreach (var skinInfo in seekrList)
        //{
        //    skinInfo.team = Define.Team.Seek;
        //}
        sendAllSkinInfosList[0].team = Define.Team.Seek;
    }

    void PlayerSapwn(List<SendAllSkinInfo> sendAllSkinInfosList)
    {
        Hashtable HT = new Hashtable();
        foreach (var p in sendAllSkinInfosList)
        {
            var spawnPointController = _gameScene.mainSpawnPoints;
            if(p.team== Define.Team.Seek)
            {

            }
            else
            {
                //Managers.Spawn.PlayerSpawn(p, spawnPointController.GetSpawnPos());
                //                PhotonNetwork.SetPlayerCustomProperties
            }
            
        }
        NextScene(Define.GameState.GameReady);   //다음 상태로 이동..
    }
 
}
