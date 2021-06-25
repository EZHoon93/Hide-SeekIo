
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class GameState_Count : GameState_Base
{
    readonly int _totZombieCount = 1;   //총 숙주 좀비 수 
    

    int _initCountTime = 3;

    bool _isCreateCharacter;



    protected override void Setup()
    {
         uI_Main.UpdateNoticeText("잠시 후 게임이 시작됩니다.");
        _initRemainTime = _initCountTime;
    }
  

    protected override void ChangeRemainTime()
    {
        uI_Main.UpdateCountText(RemainTime);
    }

    protected override void EndRemainTime()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            List<int> playerList = GetJoinUserList();   //참가한 유저수 채워넣음.
            int aiNumber = -1;
            for(int i = playerList.Count ; i <10; i++)
            {
                playerList.Add(aiNumber); // -1은 AI수 
                aiNumber--; 
            }
            //var playerSelectedDataDic = Select(playerList); //좀비 및 휴먼 선택및 위치 
            photonView.RPC("CreatePlayer", RpcTarget.AllViaServer);
        }
    }

    List<int> GetJoinUserList()
    {
        var playerList = PhotonNetwork.CurrentRoom.Players.Values.Where(s => (bool)s.CustomProperties["jn"] == true).ToList();
        List<int> result = new List<int>();
        foreach (var p in playerList)
            result.Add(p.ActorNumber);

        return result;
    }


    //Value => 0, 1 은 좀비.
    Dictionary<int, int > Select(List<int> playerNumberList)
    {
        Dictionary<int, int> result = new Dictionary<int, int>();
        int userZombieCount = GetZombieUserCount();
        int aiZombieCount = _totZombieCount - userZombieCount; //선택될 AI 좀비 수 

        List<int> zombieSpawnIndexList = new List<int>() { 0  }; //좀비 위치
        List<int> humanSpawnIndexList = new List<int>() { 2,3}; //좀비 위치


        foreach (var p in playerNumberList)
        {
            int selectSpawnIndex; //선택된 위치
            if(p < 0 )
            {
                //AI
                if(aiZombieCount > 0)
                {
                    aiZombieCount--;
                    var ranIndex = Random.Range(0, zombieSpawnIndexList.Count);
                    selectSpawnIndex = zombieSpawnIndexList[ranIndex];
                    zombieSpawnIndexList.RemoveAt(ranIndex);
                }
                else
                {
                    var ranIndex = Random.Range(0, humanSpawnIndexList.Count);
                    selectSpawnIndex = humanSpawnIndexList[ranIndex];
                    humanSpawnIndexList.RemoveAt(ranIndex);
                }
            }

            else
            {
                //AI
                if (userZombieCount > 0)
                {
                    userZombieCount--;
                    var ranIndex = Random.Range(0, zombieSpawnIndexList.Count);
                    selectSpawnIndex = zombieSpawnIndexList[ranIndex];
                    zombieSpawnIndexList.RemoveAt(ranIndex);
                }
                else
                {
                    var ranIndex = Random.Range(0, humanSpawnIndexList.Count);
                    selectSpawnIndex = humanSpawnIndexList[ranIndex];
                    humanSpawnIndexList.RemoveAt(ranIndex);
                }

            }


            result.Add(p, selectSpawnIndex);
        }

        return result;
    }

   
    //캐릭생성
    [PunRPC]
    void CreatePlayer()
    {
       
        Local_MyPlayerCharacter(0);
        Master_Creat_AIPlayer(0);


        Master_ChangeState(Define.GameState.GameReady);
    }

    void Local_MyPlayerCharacter(int index)
    {
        object[] userData = { "User1", PlayerInfo.CurrentAvater };
        if (PhotonNetwork.IsMasterClient)
        {
            var pos = Managers.Game.CurrentGameScene.GetComponent<GameMainScene>().GetSeekerPosition(index);
            Managers.Spawn.PlayerSpawn(Define.Team.Hide, pos);
        }
        else
        {
            var pos = Managers.Game.CurrentGameScene.GetComponent<GameMainScene>().GetSeekerPosition(index);
            Managers.Spawn.PlayerSpawn(Define.Team.Hide, pos);
        }
        //index = 0,1은 좀비 
        //if (index <= 1)
        //{
        //    var pos = GameManager.Instance.CurrentGameScene.GetSeekrPosition(index);
        //    GameManager.Instance.SpawnManager.PhotonSpawn(Define.PhotonObject.UserSeeker, pos);
        //}
        //else
        //{
        //    var pos = GameManager.Instance.CurrentGameScene.GetSeekrPosition(index);
        //    GameManager.Instance.SpawnManager.PhotonSpawn(Define.PhotonObject.UserHider, pos);
        //    //var pos = GameManager.Instance.CurrentGameScene.GetHiderPosition(index);
        //    //GameManager.Instance.SpawnManager.PhotonSpawn(Define.PhotonObject.UserHider, pos);
        //}
    }
 
    void Master_Creat_AIPlayer(int index)
    {
        //object[] userData = { "User1", PlayerInfo.GetUsingCharacater() };

        //if (index <= 1)
        //{
        //    _characterManager.CreatePlayer(Define.Team.Zombie, true, _playerSpawnPoints.spawnPointList[index].position);
        //}

        //else
        //{
        //    _characterManager.CreatePlayer(Define.Team.Human, true, _playerSpawnPoints.spawnPointList[index].position);
        //}
        //for(int i = 1; i < 5; i++)
        //{
        //    var pos = GameManager.Instance.CurrentGameScene.GetSeekrPosition(i);
        //    GameManager.Instance.SpawnManager.PhotonSpawn(Define.PhotonObject.AIHider, pos);
        //}

    }





    /// <summary>
    /// 좀비로 선택될유저의 수 
    /// </summary>
    /// <returns></returns>
    int GetZombieUserCount()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            //싱글모드 랜덤으로
            return Random.Range(0, 2);
        }
        else if(PhotonNetwork.CurrentRoom.PlayerCount >=2 && PhotonNetwork.CurrentRoom.PlayerCount <=6)
        {
            return 1;
        }
        else
        {
            return 2;
        }

    }

    

}
