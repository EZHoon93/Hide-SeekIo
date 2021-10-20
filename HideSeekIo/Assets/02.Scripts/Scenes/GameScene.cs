using System.Collections;

using Photon.Pun;

using UnityEngine;

public class GameScene : BaseScene
{
    Transform _cameraView;
    protected int _initGameTime = 180;
    protected int _initReadyTime = 5;
    protected int _totSeekerCount = 2;
    public Transform CameraView => _cameraView;
    public int initGameTime => _initGameTime;
    public int initReadyTime => _initReadyTime;

    public int totSeekerCount => _totSeekerCount;


    public Transform test;
    [SerializeField] MainSpawnPoints _mainSpawnPoints;
    [SerializeField] ItemSpawnManager _itemSpawnManager;
    [SerializeField] SpawnPoint[] _sectors;

    public MainSpawnPoints mainSpawnPoints => _mainSpawnPoints;
    public ItemSpawnManager itemSpawnManager => _itemSpawnManager;
    public SpawnPoint[] sectors => _sectors;


    protected readonly int mission1Time = 150;
    protected readonly int mission2Time = 90;

    protected bool mission1ok;    //
    protected bool mission2ok;    //


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
        Managers.Game.CurrentGameScene = this;
    }
    
    protected virtual void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {

            if (Managers.Game.gameStateController != null) return;
            Managers.Spawn.GameStateSpawn(Define.GameState.Wait);
        }
        if((bool)PhotonNetwork.LocalPlayer.CustomProperties["jn"])
        {
            PhotonGameManager.Instacne.GameJoin();
        }
        else
        {
            PhotonGameManager.Instacne.GameExit();

        }
        PhotonGameManager.Instacne.SendChattingMessageOnLocal(Define.ChattingColor.System, $"[{PhotonNetwork.CurrentRoom.Name}{DynamicTextData.gamaSceneNotice}");
        PhotonGameManager.Instacne.AddListenr(Define.GameState.Wait, () => Clear());

    }
    public override void Clear()
    {
        itemSpawnManager.Clear();
    }

    public virtual void OnUpdateTime(int remainGameTime)
    {

    }
}
