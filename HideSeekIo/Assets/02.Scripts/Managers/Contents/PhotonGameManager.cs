
using Photon.Pun;
using Photon.Realtime; // 포톤 서비스 관련 라이브러리
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;
using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

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


    #region 변수
    public event Action<Player> enterUserList;
    public event Action<Player> leftUserList;
    public event Action gameStarEventPoster;
    public event Action gameJoin;
    public event Action gameExit;
    public event Action<Define.ChattingColor, string> reciveChattingEvent;
    public Dictionary<Define.GameState, Action> StateChangeEventDic = new Dictionary<Define.GameState, Action>();

    #endregion


    public bool testSeeekr { get; set; } = true;


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
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }


    public void AddListenr(Define.GameState gameState, Action newAction)
    {
        if (StateChangeEventDic.ContainsKey(gameState))
        {
            StateChangeEventDic[gameState] += newAction;
        }
        else
        {
            StateChangeEventDic.Add(gameState, newAction);
        }
    }
    public void PostStateEvent(Define.GameState gameState)
    {
        //이벤트 있으면 실행
        if (StateChangeEventDic.ContainsKey(gameState))
        {
            StateChangeEventDic[gameState]?.Invoke();
        }
    }
    public void GameJoin()
    {
        var characterType = PlayerInfo.GetCurrentUsingCharacter();
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {
            { "jn", true },
            { "ch", (int)characterType},
            { "as" ,PlayerInfo.GetCurrentUsingCharacterAvaterSkin(characterType).avaterKey }
        }); ;
        gameJoin?.Invoke();
        var uiMain = Managers.UI.SceneUI.GetComponent<UI_Main>();
        if (uiMain)
        {
            uiMain.ChangePanel(Define.GameScene.Game);
        }
    }

    public void GameExit()
    {
        if (Managers.Game.myPlayer)
        {
            PhotonNetwork.Destroy(Managers.Game.myPlayer.gameObject);
        }
        gameExit?.Invoke();
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "jn", false } });
        CameraManager.Instance.ResetCamera();
        InputManager.Instance.SetActiveController(false);
        var uiMain = Managers.UI.SceneUI.GetComponent<UI_Main>();
        if (uiMain)
        {
            uiMain.ChangePanel(Define.GameScene.Lobby);
        }
    }

    /// <summary>
    /// 팀선정후 게임 시작
    /// </summary>
    public void GameStart()
    {
       //_gast
    }

    public override void OnJoinedRoom()
    {
        //base.OnJoinedRoom();
        //PhotonNetwork.IsMessageQueueRunning = true; 
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("gs"))
        {
            var CP = PhotonNetwork.CurrentRoom.CustomProperties;
            var gameState = (Define.GameState)CP["gs"];
            //State = gameState;
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
            if (PhotonNetwork.IsMasterClient == false) return;
            var joinUserCount = PhotonNetwork.CurrentRoom.Players.Values.Count(s => (bool)s.CustomProperties["jn"] == true);
            if (joinUserCount <= 0)
            {
                //게임에 참여중인 유저가 한명도없다면. => 리셋
                ChangeRoomStateToServer(Define.GameState.Wait);
            }
        }
    }

  

    public void ChangeRoomStateToServer(Define.GameState gameState)
    {
        if (PhotonNetwork.IsMasterClient == false) return;
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable()
        {
            {"gs" , gameState }
        });
    }


    #region Chatting
    [PunRPC]
    public void SendChattingMessageOnServer(Define.ChattingColor chattingColor, string content, PhotonMessageInfo _photonMessageInfo)
    {
        var playerMessage = _photonMessageInfo.Sender.NickName + ": " + content;
        reciveChattingEvent?.Invoke(chattingColor, playerMessage);
    }

    public void SendChattingMessageOnLocal(Define.ChattingColor chattingColor, string content)
    {
        reciveChattingEvent?.Invoke(chattingColor, content);
    }

    #endregion
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print("OnPlayerEnter Room");
        enterUserList?.Invoke(newPlayer);
    }



    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print("OnPlayerLetr Room");
        leftUserList?.Invoke(otherPlayer);
    }

    public void HiderDieOnLocal(int dieViewID, int attackViewID)
    {
        photonView.RPC("DieOnServer", RpcTarget.All, dieViewID, attackViewID);
    }
    [PunRPC]
    public void DieOnServer(int dieViewID, int attackViewID)
    {
        //if (State != Define.GameState.Gameing) return;
        DelayDie(dieViewID, attackViewID);
        //Util.CallBackFunction(this, 1.0f, () => DelayDie(dieViewID, attackViewID));
    }

    void DelayDie(int dieViewID, int attackViewID)
    {
        var deathPlayer = Managers.Game.GetLivingEntity(dieViewID).GetComponent<PlayerController>();
        var killPlayer = Managers.Game.GetLivingEntity(attackViewID).GetComponent<PlayerController>();
        var uiMain = Managers.UI.SceneUI as UI_Main;
        uiMain.UpdateKillNotice("dsdowok", "playertEst");
        if (killPlayer.IsMyCharacter())
        {
            uiMain.killText.text = $"{deathPlayer.NickName} 를 잡으셨습니다.";
            Color color = uiMain.killText.color;
            color.a = 1;
            uiMain.killText.color = color;
            uiMain.killText.DOFade(0.0f, 2.0f);
        }
        //Managers.Sound.Play("Die", Define.Sound.Effect);
    }


    #region OnEvent 

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        switch (eventCode)
        {
            case (byte)Define.PhotonOnEventCode.AbilityCode:
                var HT = (Hashtable)photonEvent.CustomData;
                int viewID = (int)HT["vid"];
                var statDatas = (int[])HT["stl"];
                var playerController = Managers.Game.GetLivingEntity(viewID).GetComponent<PlayerController>();
                if (playerController)
                    Managers.StatSelectManager.OnEvent_StatDatasByServer(playerController, statDatas);
                    break;

            case (byte)Define.PhotonOnEventCode.Warning:
                print("OnEvent warning");

                //var HT = (Hashtable)photonEvent.CustomData;
                //int viewID = (int)HT["vid"];
                //bool hideActive = (bool)HT["har"];

                //var playerController = Managers.Game.GetLivingEntity(viewID).GetComponent<PlayerController>();
                //if (playerController)
                //{
                //    //playerController.WarningToServer(hideActive);
                //}

                break;
        }
    }

    void OnEvent_Stat(EventData photonEvent)
    {

    }
    void OnEvent_War(EventData photonEvent)
    {

    }

    #region SendEvent Overloading


    public void SendEvent(Define.PhotonOnEventCode photonOnEventCode ,EventCaching eventCachingCode,  object hashtable)
    {
        byte evCode = (byte)photonOnEventCode;
        object content = hashtable;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = eventCachingCode,
            Receivers = ReceiverGroup.All,
        };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
        //PhotonNetwork.RaiseEvent(0, null, new RaiseEventOptions{ TargetActors = new int[]{ 1,2 }  } , sendOptions);

    }
    public void SendEvent(Define.PhotonOnEventCode photonOnEventCode, EventCaching eventCachingCode, object[] datas)
    {
        byte evCode = (byte)photonOnEventCode;
        object[] content = datas;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = eventCachingCode,
            Receivers = ReceiverGroup.All,
        };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }

    public void SendEvent(Define.PhotonOnEventCode photonOnEventCode, EventCaching eventCachingCode, object hashtable ,int interestGroup)
    {
        byte evCode = (byte)photonOnEventCode;
        object content = hashtable;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = eventCachingCode,
            Receivers = ReceiverGroup.All,
            InterestGroup = (byte)interestGroup
        };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }



    public void SendEvent(Define.PhotonOnEventCode photonOnEventCode, EventCaching eventCachingCode, object hashtable , int[] targetActors)
    {
        byte evCode = (byte)photonOnEventCode;
        object content = hashtable;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = eventCachingCode,
            Receivers = ReceiverGroup.All,
            TargetActors = targetActors
        };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }




    public void SendEvent(Define.PhotonOnEventCode photonOnEventCode, EventCaching eventCachingCode, object hashtable,int interestGroup, int[] targetActors )
    {
        byte evCode = (byte)photonOnEventCode;
        object content = hashtable;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = eventCachingCode,
            Receivers = ReceiverGroup.All,
            InterestGroup = (byte)interestGroup,
            TargetActors = targetActors
        };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }



    #endregion

    #endregion
    public void Update()
    {
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    Managers.Game.myPlayer.GetComponent<LivingEntity>().OnDamage(0, 3, UnityEngine.Vector3.zero);
        //}

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    Component c = _gameState;
        //    Destroy(c);
        //}


        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    Managers.Game.myPlayer.ChangeTeam(Define.Team.Seek);
        //}

        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    Managers.Spawn.WeaponSpawn(Define.Weapon.Flash, Managers.Game.myPlayer.playerShooter);
        //}
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Break();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            //Debug.
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            //Managers.Game.myPlayer.playerStat.StatPoint++;
            //object datas = new Hashtable { { "oID", 1 } };
            var s = new Hashtable { { "a", 1 } ,{ "b", 1 } };
            object datas2 = new Hashtable { { "oID", 2 }, { "stl", 2 } };
            //int[] s = {  Managers.Game.myPlayer.photonView.ControllerActorNr};
            //int[] ss = { Managers.Game.myPlayer.photonView.ControllerActorNr,2 ,3};
            SendEvent(Define.PhotonOnEventCode.Warning, EventCaching.AddToRoomCacheGlobal, datas2);
        }
        //}
        if (Input.GetKeyDown(KeyCode.O))
        {
            //Managers.Game.myPlayer.playerStat.StatPoint++;
            //object datas = new Hashtable { { "oID", 1 } };
            //object datas2 = new Hashtable { { "oID", 2 } };
            //object datas2 = new Hashtable { { "oID", 2 }, { "stl", new int[] { 1, 2 } } };
            //print("제거코드");
            //var s = new Hashtable { { "a", 1 }, { "b", 1 } };

            //object datas2 = new Hashtable { { "oID", 2 }, { "stl", 1 } };

            ////int[] s = { Managers.Game.myPlayer.photonView.ControllerActorNr };
            ////int[] ss = { Managers.Game.myPlayer.photonView.ControllerActorNr, 2, 3 };

            //SendEvent(Define.PhotonOnEventCode.Warning, EventCaching.RemoveFromRoomCache, datas2);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            //Managers.Game.myPlayer.playerStat.StatPoint++;
            PhotonNetwork.IsMessageQueueRunning = true;
            print(PhotonNetwork.IsMessageQueueRunning +"변경");

        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            var myPlayer = Managers.Game.myPlayer;
            if (myPlayer == null) return;
            var ranType = GetRandomItemID(seekerItemArray);
            Managers.Spawn.InGameItemSpawn(ranType, myPlayer);
        }

    }


    Define.InGameItem[] seekerItemArray =
 {
        Define.InGameItem.Flash,Define.InGameItem.Grenade,Define.InGameItem.PoisonBomb,
         Define.InGameItem.Stone
    };

    Define.InGameItem GetRandomItemID(Define.InGameItem[] itemTypeArray)
    {
        var ran = UnityEngine.Random.Range(0, itemTypeArray.Length);
        var seletType = itemTypeArray[ran];
        return seletType;
    }

    Enum GetRandomItemEnum(Define.Team team)
    {
        switch (team)
        {
            case Define.Team.Hide:
                int hiderRandom = UnityEngine.Random.Range(0, RandomItemBox.hiderItemArray.Length);
                return RandomItemBox.hiderItemArray[hiderRandom];
            case Define.Team.Seek:
                int seekerRandom = UnityEngine.Random.Range(0, RandomItemBox.seekerItemArray.Length);
                return RandomItemBox.seekerItemArray[seekerRandom];
        }

        return Define.InGameItem.Null;
    }



}
