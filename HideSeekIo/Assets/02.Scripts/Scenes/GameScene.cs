using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Data;

using Photon.Pun;

using UnityEngine;

public abstract class GameScene : BaseScene
{
    [SerializeField] Transform _cameraView;
    protected Define.GameMode _gameMode;
    public Transform CameraView => _cameraView;
    public abstract int initReadyTime { get; }
    public abstract int initGameTime { get; }
    public abstract int totSeekerCount { get; }
    public abstract int maxPlayerCount { get; }


    [SerializeField] MainSpawnPoints _mainSpawnPoints;
    [SerializeField] ItemSpawnManager _itemSpawnManager;
    [SerializeField] SpawnPoint[] _sectors;
    [SerializeField] MapController _mapController;
    public MainSpawnPoints mainSpawnPoints => _mainSpawnPoints;
    public ItemSpawnManager itemSpawnManager => _itemSpawnManager;
    public SpawnPoint[] sectors => _sectors;
    public MapController mapController => _mapController;
    public abstract Define.GameMode gameMode { get; }



    GameMissionController _gameMissionController;
    public GameMissionController gameMissionController
    {
        get => _gameMissionController;
        set
        {
            if (_gameMissionController)
            {
                Managers.Resource.PunDestroy(_gameMissionController);
            }
            _gameMissionController = value;
            _gameMissionController.transform.ResetTransform(this.transform);
        }
    }





    protected override void Init()
    {
        base.Init();
        _cameraView = this.transform.Find("cameraView");
        Managers.UI.ShowSceneUI<UI_Main>(); //메인 UI온 
        Managers.Sound.Play("Bgm", Define.Sound.Bgm);
        mapController.InitMapMaker();   //맵 

        if (PhotonNetwork.IsMasterClient)
        {
            Managers.Spawn.GameStateSpawn(Define.GameState.Wait);
        }
    }

    protected virtual void Start()
    {
        Managers.Spawn.UserControllerSpawn();
        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["jn"])
        {
            Managers.Game.NotifyGameEvent(Define.GameEvent.GameJoin,true);
        }
        else
        {
            Managers.Game.NotifyGameEvent(Define.GameEvent.GameJoin, false);
        }
        Managers.photonGameManager.SendChattingMessageOnLocal(Define.ChattingColor.System, $"[{PhotonNetwork.CurrentRoom.Name.Substring(1)}{DynamicTextData.gamaSceneNotice}");
        //Managers.Game.AddListenrOnGameState(Define.GameState.Wait, () => Clear());
    }
    public override void Clear()
    {

    }

    public virtual void PlayerSpawn()
    {

    }


    public virtual void PlayerSpawnOnGameReady(Dictionary<int, Dictionary<string, object>> playerDataTable)
    {

    }

    public virtual void PlayerSpawnOnGameStart(Dictionary<int, Dictionary<string, object>> playerDataTable)
    {

    }




}
