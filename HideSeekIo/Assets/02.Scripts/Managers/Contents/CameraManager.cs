using System;
using System.Collections;

using Cinemachine;

using FoW;

using UnityEngine;

public class CameraManager : MonoBehaviour
{

    #region 싱글톤
    // 외부에서 싱글톤 오브젝트를 가져올때 사용할 프로퍼티
    public static CameraManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<CameraManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }
    private static CameraManager m_instance; // 싱글톤이 할당될 static 변수
    #endregion


    public CinemachineVirtualCamera TargetCamera { get; private set; }

    public PlayerController Target { get; private set; }

    public event Action<int, Define.Team> _cameraViewChange;

    public CinemachineCameraOffset cameraOffset { get; set; }
    public CinemachineBasicMultiChannelPerlin virtualCameraNoise { get; set; }


    int _observerNumber = -1;  //현재 관찰하고있는 유저의 actNumber;

    public Transform mapCenter;

    FogOfWarLegacy _fogOfWarLegacy;


    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }


        if (TargetCamera == null)
            TargetCamera = FindObjectOfType<CinemachineVirtualCamera>();

        cameraOffset = TargetCamera.GetComponent<CinemachineCameraOffset>();
        virtualCameraNoise = TargetCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        ShakeCameraOff();

        _fogOfWarLegacy = Camera.main.GetComponent<FogOfWarLegacy>();
        _fogOfWarLegacy.team = 0;
        _fogOfWarLegacy.enabled = true;
    }




    public void SetupTarget(PlayerController target)
    {
        Target = target;    //현재캐릭터로 설정
        TargetCamera.Follow = target.transform;
        cameraOffset.m_Offset = new Vector3(0, 0, 0);   //오프셋 초기화

        if(target.Team == Define.Team.Hide)
        {
            Camera.main.cullingMask = ~( 1 << (int)Define.Layer.UI); 
            _fogOfWarLegacy.team = target.ViewID();
        }
        else
        {
            Camera.main.cullingMask = ~(1 << (int)Define.Layer.Hider | 1 <<(int)Define.Layer.UI);

        }


        if (target.IsMyCharacter() && target.Team == Define.Team.Seek)
        {
            StartCoroutine(CameraOffset());
        }

    }

    IEnumerator CameraOffset()
    {
        cameraOffset.m_Offset = new Vector3(0, 0, 6);
        yield return new WaitForSeconds(3.0f);

        while(cameraOffset.m_Offset.z  >= 0)
        {
            Vector3 offset = cameraOffset.m_Offset;
            offset.z -= Time.deltaTime * 3;
            cameraOffset.m_Offset = offset;
            yield return null;

        }

        cameraOffset.m_Offset = Vector3.zero;

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
        //virtualCameraNoise.m_AmplitudeGain = 0.0f;
        //virtualCameraNoise.m_FrequencyGain = 0.0f;
    }

    //옵저버 모드 ..
    public void ChangeNextPlayer()
    {
        switch (GameManager.Instance.State)
        {
            case Define.GameState.GameReady:
            case Define.GameState.Gameing:
                FindNextPlayer();
                break;
            default:
                //ResetToWaitState();
                break;
        }
    }

    void FindNextPlayer()
    {

        PlayerController findTarget = null;
        var playerControllerArray = GameManager.Instance.GetPlayerArray();
        if (playerControllerArray.Length <= 0) return;  //없으면 X
        int i = 0;
       
    }

    //리셋
    public void ResetToWaitState()
    {
        TargetCamera.Follow = mapCenter;
        cameraOffset.m_Offset = new Vector3(0, 0, -5);
    }

}
