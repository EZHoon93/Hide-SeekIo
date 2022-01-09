using System;
using System.Collections.Generic;
using System.Linq;

using Photon.Pun;
using Photon.Realtime;

using UnityEngine;

public class GameManager : MonoBehaviourPun
{
    public UserController myUserController { get; set; }
    public PlayerController myPlayer => myUserController == null ? null : myUserController.playerController;

  

    [SerializeField] GameStateController _gameStateController;
    public GameStateController gameStateController
    {
        get => _gameStateController;

        set
        {
            if (gameStateController)
            {
                Managers.Resource.PunDestroy(gameStateController);
            }
            _gameStateController = value;
            _gameStateController.transform.ResetTransform(this.transform);
            Managers.EventManager.PostNotification(EventDefine.EventType.InGame, EventDefine.InGameEvent.ChangeState, this, gameStateType);
        }
    }


    public Define.GameState gameStateType
    {
        get
        {
            if (gameStateController == null)
            {
                return Define.GameState.Wait;
            }
            return gameStateController.gameStateType;
        }
    }


    public Define.GameMode gameMode { get; set; }



    int _inGameTime;
    public int inGameTime
    {
        get => _inGameTime;
        set
        {
            _inGameTime = value;
            //if (gameEventDic.ContainsKey(Define.GameEvent.ChangeInGameTime))
            //{
            //    gameEventDic[Define.GameEvent.ChangeInGameTime]?.Invoke(inGameTime);
            //}
        }
    }


    Dictionary<Define.GameState, Action> onChangeGameState = new Dictionary<Define.GameState, Action>();
    Dictionary<Define.InGameEventType , Action<object>> gameEventDic = new Dictionary<Define.InGameEventType, Action<object>>();
    Dictionary<int, LivingEntity> _livingEntityDic = new Dictionary<int, LivingEntity>();
    Dictionary<int, UserController> _userControllerDic = new Dictionary<int, UserController>();



    private void Awake()
    {
        Managers.Game = this;
    }
   
    public void Clear()
    {
        _livingEntityDic.Clear();
    }

    #region UserController Register & Unregister
    public void RegisterUserController(int actorNumber, UserController userController)
    {
        if (_userControllerDic.ContainsKey(actorNumber)) return;
        _userControllerDic.Add(actorNumber, userController);
    }

    public void UnRegisterUserController(int actorNumber)
    {
        if (!_userControllerDic.ContainsKey(actorNumber)) return;
        _userControllerDic.Remove(actorNumber);
    }
    public UserController GetUserController(int actorNumber)
    {
        if(actorNumber < 0)
        {
            return null;
        }

        if (_userControllerDic.ContainsKey(actorNumber))
        {
            return _userControllerDic[actorNumber];
        }
        return null;
    }

    #endregion

    #region LivingEntity Register&UnRegister, Get

    public void RegisterLivingEntity(int viewID, LivingEntity livingEntity)
    {
        if (_livingEntityDic.ContainsKey(viewID)) return;
        _livingEntityDic.Add(viewID, livingEntity);
        if (livingEntity.gameObject.IsValidAI())
        {
            //AIManager.Instance.RegisterAI(livingEntity);
        }
    }

    public void UnRegisterLivingEntity(int viewID)
    {
        if (!_livingEntityDic.ContainsKey(viewID)) return;
        _livingEntityDic.Remove(viewID);
        //AIManager.Instance.UnRegisterAI(viewID);
    }

    public PlayerController GetPlayerController(int viewID)
    {
        var livingEntity = GetLivingEntity(viewID);
        if (livingEntity)
        {
            return livingEntity.GetComponent<PlayerController>();
        }
        return null;
    }
    public LivingEntity GetLivingEntity(int viewID)
    {
        if (_livingEntityDic.ContainsKey(viewID))
        {
            return _livingEntityDic[viewID];
        }

        return null;
    }
    public LivingEntity[] GetAllLivingEntity()
    {
        return _livingEntityDic.Values.ToArray();
    }
    public LivingEntity[] GetAllSeekerList()
    {
        return _livingEntityDic.Where(s => s.Value.Team == Define.Team.Hide).Select(s => s.Value).ToArray();
    }
    public LivingEntity[] GetAllHiderList()
    {
        return _livingEntityDic.Where(s => s.Value.Team == Define.Team.Hide).Select(s => s.Value).ToArray();
    }

    //살아있는 Hider 수 
    public int GetHiderCount()
    {
        int count = _livingEntityDic.Count(s => s.Value.Team == Define.Team.Hide && s.Value.Dead == false);

        Managers.EventManager.PostNotification(EventDefine.EventType.InGame, (EventDefine.InGameEvent.GameJoin), this, true);

        //NotifyGameEvent(Define.GameEvent.ChangeHider, count);
        return count;
    }

    public int GetSeekerCount()
    {
        int count = _livingEntityDic.Count(s => s.Value.Team == Define.Team.Seek && s.Value.Dead == false);
        Managers.EventManager.PostNotification(EventDefine.EventType.InGame, (EventDefine.InGameEvent.GameJoin), this, true);

        //NotifyGameEvent(Define.GameEvent.ChangeSeeker, count);
        return count;
    }

    #endregion

    #region GameState , Event Listenr
    //public void AddListenrOnGameState(Define.GameState newGameState, Action newAction)
    //{
    //    if (onChangeGameState.ContainsKey(newGameState))
    //    {
    //        onChangeGameState[newGameState] += newAction;
    //    }
    //    else
    //    {
    //        onChangeGameState.Add(newGameState, newAction);
    //    }
    //}

    //public void RemoveListnerOnGameState(Define.GameState gameState, Action action)
    //{

    //}
    public void AddListenrOnGameEvent(Define.InGameEventType inGameEventType, Action<object> newAction)
    {
        if (gameEventDic.ContainsKey(inGameEventType))
        {
            gameEventDic[inGameEventType] += newAction;
        }
        else
        {
            gameEventDic.Add(inGameEventType, newAction);
        }
    }
    public void RemoveListnerOnGameEvent(Define.InGameEventType  inGameEventType, Action<object> action)
    {
        if (gameEventDic.ContainsKey(inGameEventType))
        {
            gameEventDic[inGameEventType] -= action;
        }
    }

    public void NotifyGameEvent(Define.InGameEventType inGameEventType, object data = null)
    {
        if (gameEventDic.ContainsKey(inGameEventType))
        {
            gameEventDic[inGameEventType]?.Invoke(data);
        }
    }


    #endregion

    #region GameJoin, GameExit

    public void GameJoin()
    {
        //NotifyGameEvent(Define.GameEvent.GameEnter);

    }

    public void GameExit()
    {
        //if (myPlayer)
        //{
        //    myPlayer.GetComponent<PlayerSetup>().RemoveUserPlayerToServer();
        //}
        //if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        //{
        //    Managers.Spawn.GameStateSpawn(Define.GameState.Wait);
        //}
    }
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            myPlayer.playerHealth.OnDamage(myPlayer.ViewID(), 25, Vector3.zero);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            //myPlayer.playerHealth.OnDamage(myPlayer.ViewID(), 25, Vector3.zero);
            PhotonNetwork.Destroy(myPlayer.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            //myPlayer.playerHealth.OnDamage(myPlayer.ViewID(), 25, Vector3.zero);
            gameStateController.gameObject.SetActive(false); 
        }

    }

}
