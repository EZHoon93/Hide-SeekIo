﻿using System;
using System.Collections;

using Cinemachine;

using FoW;

using UnityEngine;

public class CameraManager : GenricSingleton<CameraManager>
{
    
    //public event Action<int, Define.Team> _cameraViewChange;

    public CinemachineVirtualCamera VirtualCamera { get; private set; }

    public CinemachineCameraOffset offsetCamera { get; private set; }
    public CinemachineBasicMultiChannelPerlin virtualCameraNoise { get; private set; }



    PlayerController _target;

    FogOfWarLegacy _fogOfWarLegacy;
    int _observerNumber = -1;  //현재 관찰하고있는 유저의 actNumber;


    protected override void Awake()
    {
        if (VirtualCamera == null)
            VirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if(offsetCamera == null)
            offsetCamera = VirtualCamera.GetComponent<CinemachineCameraOffset>();
        if(virtualCameraNoise == null)
            virtualCameraNoise = VirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if(_fogOfWarLegacy == null)
            _fogOfWarLegacy = Camera.main.GetComponent<FogOfWarLegacy>();

        _fogOfWarLegacy.team = 0;
        _fogOfWarLegacy.enabled = true;
    }


    private void Start()
    {
        StartCoroutine(UpdateCameraIsViewTarget());
    }

    IEnumerator UpdateCameraIsViewTarget()
    {
        yield return new WaitForSeconds(1.0f);  //지속적으로 확인 

        while (true)
        {
            if (PhotonGameManager.Instacne.State == Define.GameState.GameReady || PhotonGameManager.Instacne.State == Define.GameState.Gameing)
            {
                if(_target == null)
                {
                    FindNextPlayer();
                }
            }
            yield return new WaitForSeconds(5.0f);  //지속적으로 확인 
        }
    }



    public void SetupTarget(Transform target)
    {
        VirtualCamera.Follow = target.transform;
        offsetCamera.m_Offset = new Vector3(0, 0, 0);   //오프셋 초기화

        var targetPlayer=  target.GetComponent<PlayerController>();

        if (targetPlayer == null) return;
        _target = targetPlayer;
        if (targetPlayer.Team == Define.Team.Hide)
        {
            Camera.main.cullingMask = ~( 1 << (int)Define.Layer.UI); 
            _fogOfWarLegacy.team = targetPlayer.ViewID();
        }
        else
        {
            Camera.main.cullingMask = ~(1 << (int)Define.Layer.Hider | 1 <<(int)Define.Layer.UI);

        }

        if (targetPlayer.IsMyCharacter() && targetPlayer.Team == Define.Team.Seek)
        {
            StartCoroutine(CameraOffset());
        }
    }

    public void FindNextPlayer()
    {
        print("FinxNextPlayer@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
        PlayerController findTarget = null;
        var livingEntities = Managers.Game.GetAllLivingEntity();
        print("FinxNextPlayer@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" + livingEntities.Length);

        if (livingEntities.Length <= 0) return;  //없으면 X
        int i = 0;
        do
        {
            if (_observerNumber < livingEntities[i].photonView.ViewID)
            {
                findTarget = livingEntities[i].GetComponent<PlayerController>();
                if (findTarget == null)
                {
                    continue;
                }
                _observerNumber = findTarget.photonView.ViewID;
                SetupTarget(findTarget.transform);
                return;
            }
            i++;

            if (i > livingEntities.Length - 1)
            {
                i = 0;
                _observerNumber = -1;
            }


        } while (findTarget == null);
    }

    //술래팀 카메라
    IEnumerator CameraOffset()
    {
        offsetCamera.m_Offset = new Vector3(0, 0, 6);
        yield return new WaitForSeconds(3.0f);

        while(offsetCamera.m_Offset.z  >= 0)
        {
            Vector3 offset = offsetCamera.m_Offset;
            offset.z -= Time.deltaTime * 3;
            offsetCamera.m_Offset = offset;
            yield return null;

        }

        offsetCamera.m_Offset = Vector3.zero;

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

  

}
