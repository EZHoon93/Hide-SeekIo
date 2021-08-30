
using System.Collections.Generic;
using System.Linq;


using Photon.Pun;
using UnityEngine;
using Data;

public class GameState_Count : GameState_Base
{
    int _totSeekerCount = 2;   //총 숙주 좀비 수 
    int _initCountTime = 3;
    bool _isCreateCharacter;
    bool _isAiCreater;
    GameMainScene _gameMainScene;

    protected override void Setup()
    {
        uI_Main.UpdateNoticeText("잠시 후 게임이 시작됩니다.");
        _initRemainTime = _initCountTime;

        _gameMainScene = Managers.Game.CurrentGameScene as GameMainScene;

        _totSeekerCount = _gameMainScene.mainSpawnPoints.SeekerSpawnPoints.Length;
    }


    protected override void ChangeRemainTime()
    {
        uI_Main.UpdateCountText(RemainTime);
        Managers.Sound.Play("TimeCount", Define.Sound.Effect);
    }

    protected override void EndRemainTime()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            List<SendAllSkinInfo> playerDataInfoList = GetJoinUserList();   //참가한 유저수 채워넣음.
            //for (int i = playerDataInfoList.Count; i < 8; i++)
            //{
            //    SendAllSkinInfo sendAllSkinInfo = UtillGame.MakeRandomAllSkin();
            //    playerDataInfoList.Add(sendAllSkinInfo); // -1은 AI수 
            //}
            //var playerSelectedDataDic = Select(playerList); //좀비 및 휴먼 선택및 위치 
            //photonView.RPC("CreatePlayer", RpcTarget.AllViaServer, playerSelectedDataDic);
            //photonView.RPC("CreatePlayer", RpcTarget.AllViaServer);
            foreach(var p in playerDataInfoList)
            {
                Managers.Spawn.PlayerSpawn(p, Vector3.zero, false);
            }
        }
    }

    /// <summary>
    /// 참여한 유저 리스트를 받아옴.
    /// </summary>
    /// <returns></returns>
    List<SendAllSkinInfo> GetJoinUserList()
    {
        var playerList = PhotonNetwork.CurrentRoom.Players.Values.Where(s => (bool)s.CustomProperties["jn"] == true).ToList();
        List<SendAllSkinInfo> result = new List<SendAllSkinInfo>();
        foreach (var p in playerList)
        {
            SendAllSkinInfo sendAllSkinInfo;
            sendAllSkinInfo.autoNumber = p.ActorNumber;
            sendAllSkinInfo.chacterType = (Define.CharacterType)p.CustomProperties["ch"];
            sendAllSkinInfo.avaterSkinID = p.CustomProperties["as"].ToString();
            result.Add(sendAllSkinInfo);
        }

        return result;
    }

    ////Value => 0, 1 은 좀비.
    //Dictionary<int, int> Select(List<int> playerNumberList)
    //{
    //    Dictionary<int, int> result = new Dictionary<int, int>();
    //    int userSeekerCount = GeeSeekerUserCount();
    //    int AISeekerCount = _totSeekerCount - userSeekerCount; //선택될 AI 좀비 수 

    //    List<int> seekerSpawnIndexList = UserPlayerSpawnSetup(Define.Team.Seek);
    //    List<int> hiderSpawnIndexList = UserPlayerSpawnSetup(Define.Team.Hide);
    //    foreach (var p in playerNumberList)
    //    {
    //        int selectSpawnIndex; //선택된 위치
    //        //p <0은 AI
    //        if (p < 0)
    //        {

    //            //술래팀 뽑아야한다면 술래팀 선정
    //            if (AISeekerCount > 0)
    //            {
    //                AISeekerCount--;
    //                var ranIndex = Random.Range(0, seekerSpawnIndexList.Count);
    //                selectSpawnIndex = seekerSpawnIndexList[ranIndex];
    //                seekerSpawnIndexList.RemoveAt(ranIndex);
    //            }
    //            //뽑아야할 술래팀없다면 나머진 숨는팀
    //            else
    //            {
    //                var ranIndex = Random.Range(0, hiderSpawnIndexList.Count);
    //                selectSpawnIndex = hiderSpawnIndexList[ranIndex];
    //                hiderSpawnIndexList.RemoveAt(ranIndex);
    //            }
    //        }

    //        //p >0 , photonView Controller Number, 즉 0보다크면 플레이어
    //        else
    //        {
    //            //AI
    //            if (userSeekerCount > 0)
    //            {
    //                userSeekerCount--;
    //                var ranIndex = Random.Range(0, seekerSpawnIndexList.Count);
    //                selectSpawnIndex = seekerSpawnIndexList[ranIndex];
    //                seekerSpawnIndexList.RemoveAt(ranIndex);
    //            }
    //            else
    //            {
    //                var ranIndex = Random.Range(0, hiderSpawnIndexList.Count);
    //                selectSpawnIndex = hiderSpawnIndexList[ranIndex];
    //                hiderSpawnIndexList.RemoveAt(ranIndex);
    //            }

    //        }


    //        result.Add(p, selectSpawnIndex);
    //    }

    //    return result;
    //}

    List<int> UserPlayerSpawnSetup(Define.Team team)
    {
        List<int> result = new List<int>();

        switch (team)
        {
            case Define.Team.Hide:
                for (int i = 0; i < _gameMainScene.mainSpawnPoints.HiderSpawnPoints.Length; i++)
                {
                    result.Add(i);
                }
                break;
            case Define.Team.Seek:
                for (int i = 0; i < _gameMainScene.mainSpawnPoints.SeekerSpawnPoints.Length; i++)
                {
                    result.Add(i);
                }
                break;
        }
        return result;
    }




    ////캐릭생성
    //[PunRPC]
    //void CreatePlayer(Dictionary<int, int> spawnPosDic)
    //{
    //    foreach (var s in spawnPosDic)
    //    {
    //        if (s.Key == PhotonNetwork.LocalPlayer.ActorNumber)
    //        {
    //            Local_MyPlayerCharacter(s.Value);
    //        }
    //        //방장만 AI생성
    //        if (PhotonNetwork.IsMasterClient == false) continue;
    //        if (s.Key < 0)
    //        {
    //        }
    //    }

    //    Master_ChangeState(Define.GameState.GameReady);
    //}

    [PunRPC]
    public void CreatePlayer()
    {
        //Managers.Spawn.PlayerSpawn( Vector3.zero, false);
    }

    void Local_MyPlayerCharacter(int index)
    {
       
    }

  



    /// <summary>
    /// 좀비로 선택될유저의 수 
    /// </summary>
    /// <returns></returns>
    int GeeSeekerUserCount()
    {
        //싱글플레이어면 .. 50:50 확률로 술래/숨는팀  0 => 술래 , 1 숨는팀
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            //싱글모드 랜덤으로
            return Random.Range(0, 2);
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount >= 2 && PhotonNetwork.CurrentRoom.PlayerCount <= 6)
        {
            return 1;
        }
        else
        {
            return 2;
        }

    }



}
