﻿
using Photon.Pun;
using Photon.Realtime; // 포톤 서비스 관련 라이브러리
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;
using System.Linq;
using System;
using UnityEngine;

public class PhotonGameManager : MonoBehaviourPunCallbacks, IOnEventCallback 
{


    #region SingleTon
    public static PhotonGameManager Instacne
    {
        get
        {
            if (_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                _instance = FindObjectOfType<PhotonGameManager>();
            }

            // 싱글톤 오브젝트를 반환
            return _instance;
        }
    }
    private static PhotonGameManager _instance; // 싱글톤이 할당될 static 변수
    #endregion




    #region GameState
    GameState_Base _gameState;
    Define.GameState _state; //게임 상태, 최초상태 wait
    public Define.GameState State
    {
        get => _state;
        set
        {
            _state = value;
            if (_gameState)
            {
                this.photonView.ObservedComponents.Remove(_gameState);
                Destroy(_gameState);
            }
            switch (_state)
            {
                case Define.GameState.Wait:
                    _gameState = this.gameObject.AddComponent<GameState_Wait>();
                    break;
                case Define.GameState.CountDown:
                    _gameState = this.gameObject.AddComponent<GameState_Count>();
                    break;
                case Define.GameState.GameReady:
                    _gameState = this.gameObject.AddComponent<GameState_GameReady>();
                    break;
                case Define.GameState.Gameing:
                    _gameState = this.gameObject.AddComponent<GameState_Gameing>();
                    break;
                case Define.GameState.End:
                    _gameState = this.gameObject.AddComponent<GameState_End>();
                    break;
            }

            this.photonView.ObservedComponents.Add(_gameState);
        }
    }
    #endregion

    #region 변수
    public event Action<Player> enterUserList;
    public event Action<Player> leftUserList;
    public event Action<Define.ChattingColor,string> reciveChattingEvent;
    #endregion

 


   
    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (Instacne != this)
        {
            // 자신을 파괴
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

   
    public override void OnEnable()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "jn", false } });

        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("gs"))
        {
            var CP = PhotonNetwork.CurrentRoom.CustomProperties;
            var gameState = (Define.GameState)CP["gs"];
            State = gameState;
        }
        if (propertiesThatChanged.ContainsKey("jn"))
        {
            var joinUserCount = PhotonNetwork.CurrentRoom.Players.Values.Count(s => (bool)s.CustomProperties["jn"] == true);
            if (joinUserCount <= 0)
            {
                //게임에 참여중인 유저가 한명도없다면.
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable()
                    {
                        {"gs" ,Define.GameState.Wait }
                    });
                }
            }
        }
        //유저 정보변경  level => lv 닉네임변경시도 동일레벨,lv호출
        if (propertiesThatChanged.ContainsKey("lv"))
        {

        }

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("jn"))
        {
            print("Jn변경!!!");
            if (PhotonNetwork.IsMasterClient == false) return;
            var joinUserCount = PhotonNetwork.CurrentRoom.Players.Values.Count(s => (bool)s.CustomProperties["jn"] == true);
            if (joinUserCount <= 0)
            {
                //게임에 참여중인 유저가 한명도없다면.
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable()
                {
                    {"gs" ,Define.GameState.Wait }
                });
            }
        }
    }
   
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        switch (eventCode)
        {
            case (byte)Define.PhotonOnEventCode.AbilityCode:
                ReciveAbility_GlobalCachedEvent(photonEvent.CustomData);
                break;
        }
    }

    public void SendEvent(int photonViewID, byte keyCode, bool isIA, Hashtable hashtable)
    {
        byte evCode = keyCode;
        object content = hashtable;

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.AddToRoomCache,
            Receivers = ReceiverGroup.All,
            InterestGroup = 0,
            //TargetActors = new int[] { 1 }
        };
        //AI플레이어가 한것이라면 글로벌룸으로 업데이트.
        if (isIA)
            raiseEventOptions.CachingOption = EventCaching.AddToRoomCacheGlobal;


        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }



    public void ChangeRoomStateToServer(Define.GameState gameState)
    {
        if (PhotonNetwork.IsMasterClient == false) return;
        print("ChangeRoomStateToServer" + gameState);
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable()
        {
            {"gs" , gameState }
        });
    }


    #region Chatting
    [PunRPC]
    public void SendChattingMessageOnServer(Define.ChattingColor chattingColor, string content, PhotonMessageInfo _photonMessageInfo )
    {
        var playerMessage= _photonMessageInfo.Sender.NickName + ": " + content;
        reciveChattingEvent?.Invoke( chattingColor , playerMessage);
    }

    public void SendChattingMessageOnLocal(Define.ChattingColor chattingColor, string content)
    {
        print("인사받음");
        reciveChattingEvent?.Invoke(chattingColor, content);
    }

    #endregion
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        enterUserList?.Invoke(newPlayer);
    }

 

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        leftUserList?.Invoke(otherPlayer);
    }

    public void HiderDieOnLocal(int dieViewID, int attackViewID)
    {
        photonView.RPC("HiderDieOnServer", RpcTarget.MasterClient , dieViewID,attackViewID);
    }
    [PunRPC]
    public void HiderDieOnServer(int dieViewID, int attackViewID)
    {
        if (State == Define.GameState.Gameing)
        {
            var dieEntity =  Managers.Game.GetLivingEntity(dieViewID);


            var gameingState = _gameState as GameState_Gameing;
            gameingState.UpdatePlayerCount();
        }
    }


    #region Ability Caching Event

    public void SendAbility_GlobalCachedEvent(PlayerController playerController)
    {
        //int viewId = playerController.photonView.ViewID;
        //byte keyCode = (byte)Define.EventCode.AbilityCode;
        //bool isAI = false;
        //List<int> sendData = new List<int>();   //보낼데이터
        //foreach (var v in playerController._buyAbilityList)
        //    sendData.Add((int)v);   //현재 데이터들을 갖고옴
        //sendData.Add((int)abilityType);  //새로 추가 데이터
        ////포톤으로 보낼 데이터 만든다
        //Hashtable HT = new Hashtable();
        //HT.Add("Pv", viewId);   //적용할 캐릭 뷰 아이디
        //RemoveEvent(keyCode, HT);   //현재까지의 키코드로 데이터제거 보냄
        //HT.Add("Ab", sendData.ToArray());       //int[] 형식
        //SendEvent(viewId, keyCode, isAI, HT);   //데이터 보내기
    }

    //영구적 능력치 이벤트 받았을때 (캐싱 이벤트)
    public void ReciveAbility_GlobalCachedEvent(object photonCustomData)
    {
        //var HT = (Hashtable)photonCustomData;
        //int viewID = (int)HT["Pv"];
        //int[] datas = (int[])HT["Ab"];
        //var playerController = GameManager.instance.GetLivingEntity(viewID).GetComponent<PlayerController>();
        ////해당 플레이어에게 적용
        //AbilityManager.FindAddAbilityType(playerController, datas);

    }

    #endregion
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Managers.Game.myPlayer.GetComponent<LivingEntity>().OnDamage(0, 3, UnityEngine.Vector3.zero);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Component c = _gameState;
            Destroy(c);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Managers.Spawn.PlayerSpawn(Define.Team.Hide, Vector3.zero);

        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Managers.Spawn.PlayerSpawn(Define.Team.Seek, Vector3.zero);

        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Camera.main.orthographic = true;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Camera.main.orthographic = false;

        }
    }

  
}
