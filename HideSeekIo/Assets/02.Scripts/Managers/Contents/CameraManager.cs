using System;
using System.Collections;
using Cinemachine;
using FoW;
using UnityEngine;
using Photon.Pun;

public class CameraManager : MonoBehaviour
{


    Define.CameraState _cameraState;

    public Define.CameraState cameraState
    {
        get => _cameraState;
        set
        {
            _cameraState = value;
            CameraBase cameraBase = null;
            _observerCamera.SetActive(false);
            _myCamera.gameObject.SetActive(false);
            _autoCamera.gameObject.SetActive(false);
            switch (value)
            {
                case Define.CameraState.Auto:
                    cameraBase = _autoCamera;
                    break;
                case Define.CameraState.Observer:
                    cameraBase = _observerCamera;
                    break;
                case Define.CameraState.MyPlayer:
                    cameraBase = _myCamera;
                    break;
            }
            cameraBase.gameObject.SetActive(true);
            cameraBase.Init();
            //cameraStateChangeEvent?.Invoke(value);
        }
    }

    //public event Action<Define.CameraState> cameraStateChangeEvent;

    [Header("Layer")]
    public LayerMask _initObjectModeLayer;
    public LayerMask  _seekerLayer;
    public LayerMask _hiderLayer;
    [Header("Camera Type")]
    [SerializeField] ObserverCamera _observerCamera;
    [SerializeField] AutoCamera _autoCamera;
    [SerializeField] MyCamera _myCamera;
    [SerializeField] Camera _worldUICamera;

    [SerializeField] Transform _mapCenterView;

    public CinemachineVirtualCamera VirtualCamera { get; private set; }
    public CinemachineCameraOffset offsetCamera { get; private set; }
    public CinemachineBasicMultiChannelPerlin virtualCameraNoise { get; private set; }
    public PlayerController cameraTagerPlayer { get; set; }

    public Camera worldUICamera => _worldUICamera;
    FogOfWarLegacy _fogOfWarLegacy;
    public event Action<int> fogChangeEvent;
    public event Action<PlayerController> cameraViewChangeEvent;
    int _observerNumber = -1;  //현재 관찰하고있는 유저의 actNumber;
    IEnumerator _coroutine;

    protected void Awake()
    {
        Managers.cameraManager = this;
        if (VirtualCamera == null)
            VirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (offsetCamera == null)
            offsetCamera = VirtualCamera.GetComponent<CinemachineCameraOffset>();
        if (virtualCameraNoise == null)
            virtualCameraNoise = VirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (_fogOfWarLegacy == null)
            _fogOfWarLegacy = Camera.main.GetComponent<FogOfWarLegacy>();

        _fogOfWarLegacy.team = 0;
        _fogOfWarLegacy.enabled = true;
        //_coroutine = AutoCamera();


        InitFieldOfView();

        //Managers.Game.AddListenrOnGameEvent(Define.GameEvent.GameJoin, CallBack_GameJoin);
        //Managers.Game.AddListenrOnGameEvent(Define.GameEvent.MyPlayerActive, CallBack_MyPlayerActive);
    }

    void InitFieldOfView()
    {
        var cam = Camera.main;
        float width = Screen.width;
        float height = Screen.height;
        float designRatio = 16.0f / 9;
        float targetRatio = height / width;
        float fov = cam.fieldOfView;
        float reusltFov = targetRatio * fov / designRatio;
        cam.fieldOfView = reusltFov;
    }

    private void Start()
    {
        cameraState = Define.CameraState.Auto;
    }

    public void Clear()
    {
        VirtualCamera.Follow = Managers.Scene.currentGameScene.CameraView;  //카메라 초기화
        offsetCamera.m_Offset = new Vector3(0, 0, 0);   //오프셋 초기화
        _fogOfWarLegacy.team = 0;
    }

   


    //IEnumerator AutoCamera()
    //{
    //    yield return new WaitForSeconds(3.0f);  //지속적으로 확인 

    //    while (true)
    //    {
    //        if (Managers.Game.gameStateType == Define.GameState.Gameing )
    //        {
    //            if (cameraTagerPlayer == null && _observerController.gameObject.activeSelf == false)
    //            {
    //                FindNextPlayer();
    //            }
    //        }
    //        yield return new WaitForSeconds(5.0f);  //지속적으로 확인 
    //    }
    //}


    public void SetupFollowTarget(Transform target)
    {
        VirtualCamera.Follow = target.transform;
        offsetCamera.m_Offset = new Vector3(0, 0, 0);   //오프셋 초기화

    }

    public void SetupTargetPlayerController(PlayerController targetPlayerController)
    {
        //이전에 관찰하던 플레이어가 있다면
        if (cameraTagerPlayer)
        {
            cameraTagerPlayer.playerGrassDetect.Clear();
        }
        cameraTagerPlayer = targetPlayerController;
        _fogOfWarLegacy.team = targetPlayerController.ViewID();
        fogChangeEvent?.Invoke(targetPlayerController.ViewID());
        ChangeTeamByTargetView(targetPlayerController);
        SetupFollowTarget(targetPlayerController.transform);
    }

 

    /// <summary>
    /// 카메라가 보고 있는 대상의 팀에 따라 변경할것들 변경.
    /// --- 플레이어들 UI변경 
    /// --- 팀에따른 보이는 객체 변경
    /// </summary>
    public void ChangeTeamByTargetView(PlayerController playerController)
    {
        Camera.main.cullingMask = playerController.Team == Define.Team.Hide ? _hiderLayer : _seekerLayer;
        cameraViewChangeEvent?.Invoke(playerController);    //플레이어 UI변경 이벤트 콜백
    }


    /// <summary>
    /// 다음 유저를 찾음.
    /// </summary>
    public void FindNextPlayer()
    {
        PlayerController findcameraTagerPlayer = null;
        var livingEntities = Managers.Game.GetAllLivingEntity();
        if (livingEntities.Length <= 0) return;  //없으면 X
        Array.Sort(livingEntities, (a, b) => (a.ViewID() > b.ViewID()) ? -1 : 1);
        int i = 0;
        do
        {
            if (_observerNumber < i )
            {
                findcameraTagerPlayer = livingEntities[i].GetComponent<PlayerController>();
                if (findcameraTagerPlayer == null)
                {
                    i++;
                    continue;
                }
                if (cameraTagerPlayer != null)
                {
                    if (cameraTagerPlayer == findcameraTagerPlayer)
                    {
                        findcameraTagerPlayer = null;
                        i++;
                        continue;
                    }
                }
                _observerNumber = i;
                //SetupTargetPlayerController(findcameraTagerPlayer.transform);
                SetupTargetPlayerController(findcameraTagerPlayer);
                return;
            }
            i++;
            if (i > livingEntities.Length - 1)
            {
                i = 0;
                _observerNumber = -1;
            }


        } while (findcameraTagerPlayer == null);
    }






    #region Shake
    void ShakeCamera(float _time, float _ampltiude, float _frequency)
    {
        virtualCameraNoise.m_AmplitudeGain = _ampltiude;
        virtualCameraNoise.m_FrequencyGain = _frequency;
        Invoke("ShakeCameraOff", _time);
    }
    void ShakeCameraOff()
    {
        virtualCameraNoise.m_AmplitudeGain = 0.0f;
        virtualCameraNoise.m_FrequencyGain = 0.0f;
    }
    public void ShakeCameraByPosition(Vector3 pos, float time, float ampltiude, float frequency)
    {
        if (UtillGame.IsView(pos) == false) return;
        ShakeCamera(time, ampltiude, frequency);
    }

    #endregion

    #region Event CallBack
    //void CallBack_GameJoin(object isJoin)
    //{
    //    if ((bool)isJoin){
    //        print("isJo");
    //        cameraState = Define.CameraState.Observer;
    //    }
    //    else{
    //        print("isExit");

    //        if (Managers.Game.myPlayer == null)
    //        {
    //        print("isExit");

    //            cameraState = Define.CameraState.Auto;
    //        }
    //    }
    //}

    //void CallBack_MyPlayerActive(object active)
    //{
    //    var userController = Managers.Game.userController;
    //    //내캐릭터 바라보게
    //    if ((bool)active)
    //    {
    //        if (userController.playerController)
    //        {
    //            SetupTargetPlayerController(userController.playerController);
    //        }
    //        cameraState = Define.CameraState.MyPlayer;

    //    }
    //    //자동 관전으로 전환. => 게임을 나가면 오토상테 , 죽으면 옵저버상태.
    //    else
    //    {
    //        cameraTagerPlayer = null;
     
    //        if (userController.IsJoin)
    //        {
    //            print("Join");
    //            cameraState = Define.CameraState.Observer;
    //        }
    //        else
    //        {
    //            print("Auto");
    //            cameraState = Define.CameraState.Auto;
    //        }
    //    }
    //}




    //public void TestChange()
    //{
    //    if (offsetCamera.m_Offset == Vector3.zero)
    //    {
    //        VirtualCamera.transform.rotation = Quaternion.Euler(60, 45, 0);
    //        offsetCamera.m_Offset = new Vector3(-17, -5, 0);
    //    }
    //    else
    //    {
    //        VirtualCamera.transform.rotation = Quaternion.Euler(60, 0, 0);
    //        offsetCamera.m_Offset = Vector3.zero;
    //    }
    //}
    //public void SetupcameraTagerPlayer(Transform target)
    //{
    //    VirtualCamera.Follow = target.transform;
    //    offsetCamera.m_Offset = new Vector3(0, 0, 0);   //오프셋 초기화
    //    var targetPlayer = target.GetComponent<PlayerController>();
    //    if (targetPlayer == null) return;
    //    //이전에 관찰하던 플레이어가 있다면
    //    if (cameraTagerPlayer)
    //    {
    //        cameraTagerPlayer.playerGrassDetect.Clear();
    //    }
    //    cameraTagerPlayer = targetPlayer;
    //    _fogOfWarLegacy.team = targetPlayer.ViewID();
    //    fogChangeEvent?.Invoke(targetPlayer.ViewID());
    //    ChangeTeamByTargetView(targetPlayer);
    //}


    #endregion

}
