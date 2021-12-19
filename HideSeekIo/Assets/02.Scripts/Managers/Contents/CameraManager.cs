using System;
using System.Collections;

using BehaviorDesigner.Runtime.Tasks;

using Cinemachine;

using FoW;

using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] LayerMask _initObjectModeLayer;
    [SerializeField] LayerMask  _seekerLayer;
    [SerializeField] LayerMask _hiderLayer;
    [SerializeField] ObserverController _observerController;
    [SerializeField] Transform _mapCenterView;
    [SerializeField] Camera _worldUICamera;
    public CinemachineVirtualCamera VirtualCamera { get; private set; }
    public CinemachineCameraOffset offsetCamera { get; private set; }
    public CinemachineBasicMultiChannelPerlin virtualCameraNoise { get; private set; }
    public PlayerController cameraTagerPlayer { get; set; }

    FogOfWarLegacy _fogOfWarLegacy;

    public event Action<int> fogChangeEvent;
    public event Action<PlayerController> cameraViewChangeEvent;

    int _observerNumber = -1;  //현재 관찰하고있는 유저의 actNumber;


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
        Managers.Game.AddListenrOnGameEvent(Define.GameEvent.GameEnter, (n)=>SetActiveOberserver(true));
        Managers.Game.AddListenrOnGameEvent(Define.GameEvent.GameExit, (n) => SetActiveOberserver(false));
        Managers.Game.AddListenrOnGameEvent(Define.GameEvent.MyPlayerOn, SetActiveOberserver);
    }
    private void Start()
    {
        SetupFieldOfView();
        StartCoroutine(UpdateCameraIsViewcameraTagerPlayer());
        if (Managers.Game.gameMode == Define.GameMode.Object)
        {
            Camera.main.cullingMask = _initObjectModeLayer;
            _worldUICamera.gameObject.SetActive(false);
        }
        else
        {

        }
    }

    public void Clear()
    {
        VirtualCamera.Follow = Managers.Scene.currentGameScene.CameraView;  //카메라 초기화
        offsetCamera.m_Offset = new Vector3(0, 0, 0);   //오프셋 초기화
        _fogOfWarLegacy.team = 0;
    }

    void SetActiveOberserver(object active) => _observerController.SetActive((bool)active);
 
    void SetupFieldOfView()
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



    IEnumerator UpdateCameraIsViewcameraTagerPlayer()
    {
        yield return new WaitForSeconds(3.0f);  //지속적으로 확인 

        while (true)
        {
            if (Managers.Game.gameStateType == Define.GameState.Gameing )
            {
                if (cameraTagerPlayer == null && _observerController.gameObject.activeSelf == false)
                {
                    FindNextPlayer();
                }
            }
            yield return new WaitForSeconds(5.0f);  //지속적으로 확인 
        }
    }

 


    public void SetupcameraTagerPlayer(Transform target)
    {
        VirtualCamera.Follow = target.transform;
        offsetCamera.m_Offset = new Vector3(0, 0, 0);   //오프셋 초기화
        var targetPlayer = target.GetComponent<PlayerController>();
        if (targetPlayer == null) return;
        //이전에 관찰하던 플레이어가 있다면
        if (cameraTagerPlayer)
        {
            cameraTagerPlayer.playerGrassDetect.Clear();
        }
        cameraTagerPlayer = targetPlayer;
        _fogOfWarLegacy.team = targetPlayer.ViewID();
        fogChangeEvent?.Invoke(targetPlayer.ViewID());
        ChangeTeamByTargetView(targetPlayer);
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
                SetupcameraTagerPlayer(findcameraTagerPlayer.transform);
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


    public void ShakeCameraByPosition(Vector3 pos, float time, float ampltiude, float frequency)
    {
        if (IsView(pos) == false) return;
        ShakeCamera(time, ampltiude, frequency);
    }

    public bool IsView(Vector3 pos)
    {
        var viewPos = Camera.main.WorldToViewportPoint(pos);
        if (viewPos.x > 0.0F && viewPos.x < 1.0F && viewPos.y > 0.0f && viewPos.y < 1.0f)
        {
            //움직이고 있는 상태만
            return true;
        }
        else
        {
            return false;
        }
    }
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


    public void TestChange()
    {
        if (offsetCamera.m_Offset == Vector3.zero)
        {
            VirtualCamera.transform.rotation = Quaternion.Euler(60, 45, 0);
            offsetCamera.m_Offset = new Vector3(-17, -5, 0);
        }
        else
        {
            VirtualCamera.transform.rotation = Quaternion.Euler(60, 0, 0);
            offsetCamera.m_Offset = Vector3.zero;
        }
    }

}
