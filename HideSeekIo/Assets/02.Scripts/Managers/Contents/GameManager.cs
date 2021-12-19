using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager  : MonoBehaviourPun
{
    PlayerController _myPlayer;
    public UserController userController { get; set; }
    public PlayerController myPlayer
    {
        get => _myPlayer;
        set
        {
            _myPlayer = value;
            NotifyGameEvent(Define.GameEvent.MyPlayerOn, false);
        }
    }
    //public GameScene CurrentGameScene { get; set; }

    GameStateController _gameStateController;
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
            if (onChangeGameState.ContainsKey(gameStateController.gameStateType))
            {
                onChangeGameState[gameStateController.gameStateType]?.Invoke();
            }

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
            if (gameEventDic.ContainsKey(Define.GameEvent.ChangeInGameTime))
            {
                gameEventDic[Define.GameEvent.ChangeInGameTime]?.Invoke(inGameTime);
            }
        }
    }


    Dictionary<Define.GameState, Action> onChangeGameState = new Dictionary<Define.GameState, Action>();
    Dictionary<Define.GameEvent, Action<object>> gameEventDic = new Dictionary<Define.GameEvent, Action<object>>();
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
    public void RegisterUserController(int localNumber, UserController userController)
    {
        if (_userControllerDic.ContainsKey(localNumber)) return;
        _userControllerDic.Add(localNumber, userController);
    }

    public void UnRegisterUserController(int localNumber)
    {
        if (!_userControllerDic.ContainsKey(localNumber)) return;
        _userControllerDic.Remove(localNumber);
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

    public void UnRegisterLivingEntity(int viewID )
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
        //changeSeekerCount?.Invoke(count);
        return count;
    }

    public int GetSeekerCount()
    {
        int count = _livingEntityDic.Count(s => s.Value.Team == Define.Team.Seek && s.Value.Dead == false);
        //changeSeekerCount?.Invoke(count);
        return count;
    }

    #endregion

    #region GameState , Event Listenr
    public void AddListenrOnGameState(Define.GameState newGameState, Action newAction)
    {
        if (onChangeGameState.ContainsKey(newGameState))
        {
            onChangeGameState[newGameState] += newAction;
        }
        else
        {
            onChangeGameState.Add(newGameState, newAction);
        }
    }

    public void RemoveListnerOnGameState(Define.GameState gameState , Action action)
    {

    }
    public void AddListenrOnGameEvent(Define.GameEvent newGameEvent, Action<object> newAction)
    {
        if (gameEventDic.ContainsKey(newGameEvent))
        {
            gameEventDic[newGameEvent] += newAction;
        }
        else
        {
            gameEventDic.Add(newGameEvent, newAction);
        }
    }
    public void RemoveListnerOnGameEvent(Define.GameEvent gameEvent, Action<object> action)
    {

    }

    public void NotifyGameEvent(Define.GameEvent gameEvent, object data = null)
    {
        if (gameEventDic.ContainsKey(gameEvent))
        {
            gameEventDic[gameEvent]?.Invoke(data);
        }
    }


    #endregion

    #region GameJoin, GameExit

    //public void GameJoin()
    //{
    //    NotifyGameEvent(Define.GameEvent.GameEnter);
    //}

    //public void GameExit()
    //{
    //    if (myPlayer)
    //    {
    //        myPlayer.GetComponent<PlayerSetup>().RemoveUserPlayerToServer();
    //    }
    //    //if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
    //    //{
    //    //    Managers.Spawn.GameStateSpawn(Define.GameState.Wait);
    //    //}
    //}
    #endregion

}
