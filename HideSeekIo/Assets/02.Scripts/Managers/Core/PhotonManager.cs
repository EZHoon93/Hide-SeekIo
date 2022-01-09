
using Photon.Pun;
using Photon.Realtime; // 포톤 서비스 관련 라이브러리
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System;

public class PhotonManager : MonoBehaviourPunCallbacks 
{

    readonly string _gameVersion = "3.0.0";
    public Define.ServerState State { get; private set; }



    public bool isScret { get; set; } = false;
    public string roomName { get; set; } = null;
    public Define.GameMode gameMode { get; set; } = Define.GameMode.Item;

 

    public void Init()
    {

    }
    public void Clear()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }
        //foreach(var photonObject in myPhtonViewDic.Values.ToArray())
        //{
        //    if (photonObject)
        //    {
        //        Managers.Resource.PunDestroy(photonObject);
        //    }
        //}
    }
    
    //포톤 서버 연결
    public void Connect()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.GameVersion = _gameVersion;
        State = Define.ServerState.Connecting;  //연결중
        PhotonNetwork.ConnectUsingSettings();
    }


    /// 포톤 서버 접속시 자동 실행
    public override void OnConnectedToMaster()
    {
        print("연결성공마스터");

        //마스터 서버에 접속중이라면
        if (PhotonNetwork.IsConnected)
        {
            // 룸 접속 실행
            PhotonNetwork.ConnectToRegion("kr");

            State = Define.ServerState.Connect;
            
        }
        else
        {
            State = Define.ServerState.DisConnect;
            //접속 실패시 접솔 실패 UI
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    //정보 업데이트. 닉네임,레벨,참여여부등
    public void PhotonLogin()
    {
        PhotonNetwork.LocalPlayer.NickName = PlayerInfo.nickName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {
            { "jn", false } 
        });

    }

    public override void OnConnected()
    {
        print("연결성공");
    }
   

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //print("마스터 바뀜!!");
    }

    public void SetupRoomInfo(Define.GameMode newGameMode, string newRoomName = null, bool newIsScret = false)
    {
        gameMode = newGameMode;
        roomName = newRoomName;
        isScret = newIsScret;
    }

    //UI에서 다른채널 찾기 및 미입력시 빠른채널찾기
    public void ChangeChannel(string newRoomName= null , bool newIsScret =false)
    {
        roomName = newRoomName;
        isScret = newIsScret;
        PhotonNetwork.LeaveRoom();
    }

    //룸나갈떄 호출
    public override void OnLeftRoom()
    {
        //print("OnLeftRoo");
        Managers.Scene.LoadScene(Define.Scene.Lobby);
    }



    #region EventCallBack

    #endregion
}
