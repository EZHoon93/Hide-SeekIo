using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using System.Linq;

using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class GameManager : MonoBehaviourPun, IPunObservable
{
    #region 싱글톤
    // 외부에서 싱글톤 오브젝트를 가져올때 사용할 프로퍼티
    public static GameManager Instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                _instance = FindObjectOfType<GameManager>();
            }

            // 싱글톤 오브젝트를 반환
            return _instance;
        }
    }
    private static GameManager _instance; // 싱글톤이 할당될 static 변수
    #endregion
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


    public Action<int> seekerChangeEvent;
    public Action<int> hiderChangeEvent;
    public Action<int> gameTimeChangeEvent;


    public int gameTime;    //게임 진행중 게임타임
  

    GameState_Base gameState;
    SpawnManager _spawnManager = new SpawnManager();

    public SpawnManager SpawnManager => _spawnManager;
    public PlayerController myPlayer;
    Dictionary<int, LivingEntity> _livingEntityDic = new Dictionary<int, LivingEntity>();
    Dictionary<int, PlayerController> _playerDic = new Dictionary<int, PlayerController>();

    int n_humanCount = 0;
    int n_zombieCount = 0;

    public GameScene CurrentGameScene { get; set; }
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
    private void Awake()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "Join", false } });
        if (Instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable()
            {
                {"GS" ,Define.GameState.Wait }
            });
            print("게임상태변경");
        }
        else
        {
            var CP = PhotonNetwork.CurrentRoom.CustomProperties;
            var gameState = (Define.GameState)CP["GS"];
            State = gameState;
        }
    }






    public void GameJoin()
    {
        //UIM
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "Join", true } });
    }
    public void GameExit()
    {
        if(myPlayer)
        {
            PhotonNetwork.Destroy(myPlayer.gameObject);
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "Join", false } });

    }

    #region LivingEntity

    public void RegisterLivingEntity(int viewID, LivingEntity livingEntity)
    {
        if (_livingEntityDic.ContainsKey(viewID)) return;
        _livingEntityDic.Add(viewID, livingEntity);
        JudgeCount(livingEntity);

        var player = livingEntity.GetComponent<PlayerController>();
        if (player == null) return;
        if (_playerDic.ContainsKey(viewID)) return;
        _playerDic.Add(viewID, player);
    }

    public void UnRegisterLivingEntity(int viewID, LivingEntity livingEntity)
    {
        if (!_livingEntityDic.ContainsKey(viewID)) return;
        _livingEntityDic.Remove(viewID);
        JudgeCount(livingEntity);
        var player = livingEntity.GetComponent<PlayerController>();
        if (player == null) return;
        if (_playerDic.ContainsKey(viewID) == false) return;
        _playerDic.Remove(viewID);

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

    public PlayerController[] GetPlayerArray()
    {
        return _playerDic.Values.ToArray();
    }

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


    #endregion
    [PunRPC]
    public void HumanDie(int viewID)
    {
        //var livingEntity = GetLivingEntity(viewID);
        //if (!livingEntity) return;
        //var humanController = livingEntity.GetComponent<HumanController>();
        //if (!humanController) return;
        //var dieName = humanController.NickName;
        //UIManager.instance.GetMenuText(UI_Text_Menu.Type.DeathInfo)
        //    .UpdateFadeText(dieName + "님이 잡히셨습니다", 0.01f);

        //SoundManager.instance.PlaySoundSFX(Define.SoundSFX.Die);    //사운드 재생

        //UnRegisterLivingEntity(viewID, livingEntity);   //제거..

    }

    #region Test
    public void Update()
    {



        if (Input.GetKeyDown(KeyCode.Y))
        {
            Click_Damage();
        }
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
            //myPlayer.Coin += 1000;
        }
        if (gameState == null) return;
        //gameState.UpdateStae();
    }

    [PunRPC]
    public void Test22()
    {
        print("Test22");
    }
    public void Click_Damage()
    {
        myPlayer.GetComponent<LivingEntity>().OnDamage(1, 1, Vector3.zero);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }

    public void ResetGameState()
    {
        var gameState = GetComponent<GameState_Base>();    //리셋 => Wait상태로 바꿈
        if (gameState)
        {
            gameState.ResetGameState();
        }
        else
        {
            print("없음..에러");
        }
    }

    public void TestStart()
    {
        //Master_ChangeState(Define.GameState.CountDown);
        GetComponent<GameState_Wait>().Test();    //리셋 => Wait상태로 바꿈

    }
    #endregion
}
