
using Photon.Pun;
using Photon.Realtime; // 포톤 서비스 관련 라이브러리
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;
using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// InGameManager...
/// </summary>
public class PhotonGameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{

    #region 변수
    
    //public event Action<bool> onMyCharacter;  //내캐릭터 생성시 ..
    public event Action<Define.ChattingColor, string> reciveChattingEvent;
    public Dictionary<Define.InGamePhotonEvent, Action<Player>> onPlayerEventCallBackDic = new Dictionary<Define.InGamePhotonEvent, Action<Player>>();
    #endregion

    private void Awake()
    {
        Managers.photonGameManager = this;
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);

    }
    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }


    //public void AddListenr(Define.GameState gameState, Action newAction)
    //{
    //    if (StateChangeEventDic.ContainsKey(gameState))
    //    {
    //        StateChangeEventDic[gameState] += newAction;
    //    }
    //    else
    //    {
    //        StateChangeEventDic.Add(gameState, newAction);
    //    }
    //}
    //public void PostStateEvent(Define.GameState gameState)
    //{
    //    //이벤트 있으면 실행
    //    if (StateChangeEventDic.ContainsKey(gameState))
    //    {
    //        StateChangeEventDic[gameState]?.Invoke();
    //    }
    //}
    //public void GameJoin()
    //{
    //    var currentSkinInfo = PlayerInfo.userData.GetCurrentAvater();
    //    PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {
    //        { "jn", true },
    //        { "ch" ,currentSkinInfo.characterAvaterKey },   // 캐릭아바타스킨
    //        { "we" ,currentSkinInfo.weaponKey },   //무기아바타스킨
    //        { "ac" ,currentSkinInfo.accesoryKey },   //악세사리스킨

    //    }); ;
    //    gameJoin?.Invoke();
    //    var uiMain = Managers.UI.SceneUI.GetComponent<UI_Main>();
    //    if (uiMain)
    //    {
    //        uiMain.ChangePanel(Define.GameScene.Game);
    //    }
    //}

    //public void GameExit()
    //{
    //    if (myPlayer)
    //    {
    //        myPlayer.GetComponent<PlayerSetup>().RemoveUserPlayerToServer();
    //    }
    //    gameExit?.Invoke();
    //    PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "jn", false } });
    //    Managers.Input.SetActiveController(false);
    //    var uiMain = Managers.UI.SceneUI.GetComponent<UI_Main>();
    //    if (uiMain)
    //    {
    //        uiMain.ChangePanel(Define.GameScene.Lobby);
    //    }

        
    //    if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
    //    {
    //        Managers.Spawn.GameStateSpawn(Define.GameState.Wait);
    //    }
    //}

    /// <summary>
    /// 팀선정후 게임 시작
    /// </summary>
  

    /// <summary>
    /// 방장이 바뀌엇을떄
    /// </summary>
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        var allLivings =  Managers.Game.GetAllLivingEntity();
        foreach(var living in allLivings)
        {
            if (living == null) continue;   //만약 없는캐릭이라면 
            if (living.gameObject.IsValidAI())
            {
                 var playerController = living.GetComponent<PlayerController>();
                if (playerController)
                {
                    //playerController.ChangeAI();
                }
            }
        }
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
            if (targetPlayer.IsLocal)
            {
                var localUserController = (UserController)targetPlayer.TagObject;
                localUserController.GameJoinCallBackByPhotonServer((bool)changedProps["jn"]);

            }
            //if (PhotonNetwork.IsMasterClient == false) return;
            //var joinUserCount = PhotonNetwork.CurrentRoom.Players.Values.Count(s => (bool)s.CustomProperties["jn"] == true);
            //if (joinUserCount <= 0)
            //{
            //    //게임에 참여중인 유저가 한명도없다면. => 리셋
            //    //ChangeRoomStateToServer(Define.GameState.Wait);
            //    //Managers.Spawn.GameStateSpawn( Define.GameState.Wait);

            //}
        }
    }

    public void AddEventListenr(Define.InGamePhotonEvent @eventType, Action<Player> action)
    {
        if (onPlayerEventCallBackDic.ContainsKey(@eventType))
        {
            onPlayerEventCallBackDic[@eventType] += action;
        }
        else
        {
            onPlayerEventCallBackDic.Add(@eventType, action);
        }
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
        if (onPlayerEventCallBackDic.ContainsKey(Define.InGamePhotonEvent.Enter))
        {
            onPlayerEventCallBackDic[Define.InGamePhotonEvent.Enter]?.Invoke(newPlayer);
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //if (otherPlayer.TagObject != null)
        //{
        //    var userController = (UserController)otherPlayer.TagObject;
        //    if (userController)
        //    {
        //        //playerController.ChangeAI();
        //    }
        //}
        if (onPlayerEventCallBackDic.ContainsKey(Define.InGamePhotonEvent.Left))
        {
            onPlayerEventCallBackDic[Define.InGamePhotonEvent.Left]?.Invoke(otherPlayer);
        }
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
        print(deathPlayer + "/" + killPlayer + "/" + dieViewID + "/" + attackViewID);
        if (deathPlayer == null || killPlayer == null) return;
        var uiMain = Managers.UI.SceneUI as UI_Main;
        uiMain.UpdateKillNotice(killPlayer.NickName, deathPlayer.NickName);
        if (killPlayer.IsMyCharacter())
        {
            uiMain.killText.text = $"{deathPlayer.NickName} 를 잡으셨습니다.";
            Color color = uiMain.killText.color;
            color.a = 1;
            uiMain.killText.color = color;
            uiMain.killText.DOFade(0.0f, 2.0f);
        }
        Managers.Sound.Play("Die", Define.Sound.Effect);

        if (deathPlayer.IsMyCharacter())
        {
        }
        if (killPlayer.IsMyCharacter())
        {
            //Managers.Sound.Play("Die", Define.Sound.Effect);
        }
    }


    #region OnEvent 

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        switch (eventCode)
        {
            //case (byte)Define.PhotonOnEventCode.AbilityCode:
            //    var HT = (Hashtable)photonEvent.CustomData;
            //    int viewID = (int)HT["vid"];
            //    var statDatas = (int[])HT["stl"];
            //    var playerController = Managers.Game.GetLivingEntity(viewID).GetComponent<PlayerController>();
            //    if (playerController)
            //        //Managers.StatSelectManager.OnEvent_StatDatasByServer(playerController, statDatas);
            //        break;

            case (byte)Define.PhotonOnEventCode.Warning:

                break;
            case (byte)Define.PhotonOnEventCode.TeamSelect:
                var HT3 = (Hashtable)photonEvent.CustomData;
                var seekerArray = (int[] ) HT3["se"];
                foreach (var seekrer in seekerArray)
                {
                    var selectplayer = Managers.Game.GetLivingEntity(seekrer).GetComponent<PlayerController>();
                    if (selectplayer)
                    {
                        selectplayer?.ChangeTeam(Define.Team.Seek);
                    }
                }
                var hiderArray =  Managers.Game.GetAllHiderList();
                foreach(var hider in hiderArray)
                {
                    if (hider)
                    {
                        var hiderPlayer = hider.GetComponent<PlayerController>();
                        //Managers.StatSelectManager.SetupRandomWeapon(hiderPlayer);
                        hiderPlayer?.ChangeTeam(Define.Team.Hide);
                    }
                }
                //if (PhotonNetwork.IsMasterClient)
                //{
                //    Managers.Game.gameStateController.NextScene(Define.GameState.Gameing);
                //}
              

                break;
            case (byte)Define.PhotonOnEventCode.InitMapObject:
                var value = (int[])photonEvent.CustomData;
                var _gameScene = Managers.Scene.currentScene as GameScene;
                _gameScene.mapController.MapMake(value) ;


                break;
        }
    }

    void OnEvent_Stat(EventData photonEvent)
    {

    }
    void OnEvent_War(EventData photonEvent)
    {

    }



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

    



}