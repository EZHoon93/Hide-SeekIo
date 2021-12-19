using System.Collections.Generic;

using Photon.Pun;
using UnityEngine;

public class ObjectGameScene : GameScene
{
    public override Define.GameMode gameMode => Define.GameMode.Object;
    public override int initReadyTime => 5;
    public override int initGameTime =>10;
    public override int totSeekerCount => 1;
    public override int maxPlayerCount => 8;



    protected override void Init()
    {
        base.Init();
        Managers.Game.gameMode = Define.GameMode.Object;
    }

    /// <summary>
    /// 숨는 유저만 생성
    /// </summary>
    public override void PlayerSpawnOnGameReady(Dictionary<int, Dictionary<string, object>> playerDataTable)
    {
        if (playerDataTable.ContainsKey(PhotonNetwork.LocalPlayer.ActorNumber))
        {

        }


        if (PhotonNetwork.IsMasterClient == false) return;
        var mapSpawnPoint = mainSpawnPoints;
        foreach (var data in playerDataTable)
        {
            if ((Define.Team)data.Value["te"] == Define.Team.Hide)
            {
                Managers.Spawn.PlayerSpawn(data.Key, data.Value, mapSpawnPoint.GetHiderPosition_Random());
            }
        }
    }

    //술래 유저만 생성
    public override void PlayerSpawnOnGameStart(Dictionary<int, Dictionary<string, object>> playerDataTable)
    {
        if (PhotonNetwork.IsMasterClient == false) return;
        var mapSpawnPoint = mainSpawnPoints;
        foreach (var data in playerDataTable)
        {
            if ((Define.Team)data.Value["te"] == Define.Team.Seek)
            {
                var player = Managers.Spawn.PlayerSpawn(data.Key, data.Value, mapSpawnPoint.GetSeekerPosition_Random());
                Managers.Spawn.WeaponSpawn(Define.Weapon.Hammer, player);
            }
        }
    }

    //public override void OnGameStart()
    //{
    //    ////내캐릭이 있고 술래라면..
    //    //var myPlayer = Managers.Game.myPlayer;
    //    //if (myPlayer && myPlayer.Team == Define.Team.Seek)
    //    //{
    //    //    Managers.Spawn.WeaponSpawn(Define.Weapon.Hammer, myPlayer); //술래 무기 생성
    //    //}

    //    ////방장은 AI술래에게 무기지급
    //    //if (PhotonNetwork.IsMasterClient)
    //    //{
    //    //    var seekerList =  Managers.Game.GetAllSeekerList();
    //    //    foreach(var seeker in seekerList)
    //    //    {
    //    //        if (seeker.gameObject.IsValidAI())
    //    //        {
    //    //            Managers.Spawn.WeaponSpawn(Define.Weapon.Hammer, seeker.GetComponent<PlayerController>() ); //술래 무기 생성
    //    //        }
    //    //    }
    //    //}
    //}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("생성");
            Managers.Spawn.WeaponSpawn(Define.Weapon.Hammer, Managers.Game.myPlayer);
        }
    }


}
