using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using System.Linq;

using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class GameManager : GenricSingleton<GameManager>
{
    
    #region GameState
    Define.GameState _state; //게임 상태, 최초상태 wait
    public Define.GameState State
    {
        get => _state;
        set
        {
            _state = value;
            if (gameState)
            {
                this.photonView.ObservedComponents.Remove(gameState);
                Destroy(gameState);
            }
            switch (_state)
            {
                case Define.GameState.Wait:
                    gameState = this.gameObject.AddComponent<GameState_Wait>();
                    break;
                case Define.GameState.CountDown:
                    gameState = this.gameObject.AddComponent<GameState_Count>();
                    break;
                case Define.GameState.GameReady:
                    gameState = this.gameObject.AddComponent<GameState_GameReady>();
                    break;
                case Define.GameState.Gameing:
                    gameState = this.gameObject.AddComponent<GameState_Gameing>();
                    break;
                case Define.GameState.End:
                    gameState = this.gameObject.AddComponent<GameState_End>();
                    break;
            }

            this.photonView.ObservedComponents.Add(gameState);
        }
    }
    #endregion


    int n_humanCount = 0;
    int n_zombieCount = 0;


    public Action<int> seekerChangeEvent;
    public Action<int> hiderChangeEvent;
    public Action<int> gameTimeChangeEvent;
    public Action GameResetEvent;   //게임 리셋, 유저 참여가 다없을시 발동,


    public int gameTime;    //게임 진행중 게임타임
  

    GameState_Base gameState;
    SpawnManager _spawnManager = new SpawnManager();


    public SpawnManager SpawnManager => _spawnManager;

    public PlayerController myPlayer { get; set; }

    Dictionary<int, LivingEntity> _livingEntityDic = new Dictionary<int, LivingEntity>();

   
    public GameScene CurrentGameScene { get; set; }

    //public event Action GameResetEvent { }
    public int HumanCount
    {
        get => n_humanCount;
        set
        {
            if (n_humanCount == value) return;
            n_humanCount = value;
            //UIManager.instance.GetMenuText(UI_Text_Menu.Type.HumanCount).UpdateText(n_humanCount.ToString());
        }
    }
    public int ZombieCount
    {
        get => n_zombieCount;
        set
        {
            if (n_zombieCount == value) return;
            n_zombieCount = value;
            //UIManager.instance.GetMenuText(UI_Text_Menu.Type.ZombieCount).UpdateText(n_zombieCount.ToString());
        }
    }
    /// <summary>
    /// 최초 시작
    /// </summary>
    protected override void Awake()
    {
        print("GameManager Awake");
        base.Awake();
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "jn", false } });
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonGameManager.Instacne.ChangeRoomStateToServer(Define.GameState.Wait);
        }
        else
        {
            State = (Define.GameState)PhotonNetwork.CurrentRoom.CustomProperties["gs"];
        }
    }
    public void GameJoin()
    {
        //UIM
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "jn", true } });
    }
    public void GameExit()
    {
        if(myPlayer)
        {
            PhotonNetwork.Destroy(myPlayer.gameObject);
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "jn", false } });
        InputManager.Instacne.OffAllController();
        GameResetEvent?.Invoke();   
    }

    public void GameReset()
    {
        GameResetEvent?.Invoke();
    }


    #region LivingEntity Register&UnRegister, Get

    public void RegisterLivingEntity(int viewID, LivingEntity livingEntity)
    {
        print("RegisterLivingEntity" + viewID);
        if (_livingEntityDic.ContainsKey(viewID)) return;
        _livingEntityDic.Add(viewID, livingEntity);
        JudgeCount(livingEntity);

        var player = livingEntity.GetComponent<PlayerController>();
    }

    public void UnRegisterLivingEntity(int viewID, LivingEntity livingEntity)
    {
        if (!_livingEntityDic.ContainsKey(viewID)) return;
        _livingEntityDic.Remove(viewID);
        JudgeCount(livingEntity);
        var player = livingEntity.GetComponent<PlayerController>();
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
        print(_livingEntityDic.Count + "리빙리스트");
        print("올하이더 리스트" + _livingEntityDic.Where(s => s.Value.Team == Define.Team.Hide).Select(s => s.Value).ToArray().Length);
        print("올하이더 리스트" + _livingEntityDic.Where(s => s.Value.Team == Define.Team.Seek).Select(s => s.Value).ToArray().Length);

        return _livingEntityDic.Where(s => s.Value.Team == Define.Team.Hide).Select(s => s.Value).ToArray();
    }
    #endregion

    //방장만 판단 다른클라이언트는 => 값만받아옴
    public void JudgeCount(LivingEntity livingEntity)
    {
        if (_state != Define.GameState.Gameing) return;    //게임 진행시에만 판단
        switch (livingEntity.Team)
        {
            case Define.Team.Hide:
                HumanCount = _livingEntityDic.Count(s => s.Value.Team == Define.Team.Hide);
                if (HumanCount <= 0)
                {
                    //게임 끝냄
                    var gameState = GetComponent<GameState_Gameing>();
                    if (gameState)
                    {
                        gameState.ZombieTeamWin();
                    }
                }
                break;
            case Define.Team.Seek:
                ZombieCount = _livingEntityDic.Count(s => s.Value.Team == Define.Team.Seek);
                break;
        }

    }


  

    #region Test
    public void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Component c = GetComponent<GameState_Base>();
            Destroy(c);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            photonView.RPC("Test22", RpcTarget.All);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            myPlayer.Coin += 1000;
        }
        if (gameState == null) return;
        //gameState.UpdateStae();
    }

    #endregion
}
