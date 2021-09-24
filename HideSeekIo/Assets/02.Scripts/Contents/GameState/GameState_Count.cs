
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Data;

public class GameState_Count : GameState_Base
{
    public override float remainTime => 5;

    public override void OnPhotonInstantiate(PhotonMessageInfo info, float createServerTime)
    {
        uI_Main = Managers.UI.SceneUI as UI_Main;
        uI_Main.UpdateNoticeText("잠시 후 게임이 시작됩니다.");
        PhotonGameManager.Instacne.gameExit += GameExit;

    }
    public override void OnDestroy()
    {
        PhotonGameManager.Instacne.gameExit -= GameExit;
    }
    public override void OnUpdate(int remainTime)
    {
        uI_Main.UpdateCountText(remainTime);
        Managers.Sound.Play("TimeCount", Define.Sound.Effect);
    }
  
    public override void OnTimeEnd()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            List<SendAllSkinInfo> playerDataInfoList = GetJoinUserList();   //참가한 유저수 채워넣음.
            AIManager.Instance.CreateAI(8, ref playerDataInfoList);
            //for (int i = playerDataInfoList.Count; i < 8; i++)  //나머지 자리 AI추가.
            //{
            //    SendAllSkinInfo sendAllSkinInfo = UtillGame.MakeRandomAllSkin();
            //    playerDataInfoList.Add(sendAllSkinInfo);
            //}

            //자동으로랜덤 선정
            var playerSpawnPointList = Managers.Game.CurrentGameScene.mainSpawnPoints.playerSpawnPoints.ToList();
            foreach (var p in playerDataInfoList)
            {
                int ran = Random.Range(0, playerSpawnPointList.Count);
                Managers.Spawn.PlayerSpawn(p, playerSpawnPointList[ran].transform.position);
                playerSpawnPointList.RemoveAt(ran);
            }
            //print("Spawn Readt!!");
            NextScene(Define.GameState.GameReady);

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
            sendAllSkinInfo.nickName = p.NickName;
            result.Add(sendAllSkinInfo);
        }
        return result;
    }

    void GameExit()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            NextScene(Define.GameState.Wait);
        }
    }
}
