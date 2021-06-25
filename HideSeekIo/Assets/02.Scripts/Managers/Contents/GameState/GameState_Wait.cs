using Photon.Pun;
using UnityEngine;
using TMPro;

//바로 바뀌는거 방지를 위한
public class GameState_Wait : GameState_Base
{
    int _initSceneWaitTime = 2;  //다음씬으로 넘어가기위한 대기시간
    
    protected override void Setup()
    {
        _initRemainTime = _initSceneWaitTime;
        CameraManager.Instance.SetupTarget(Managers.Game.CurrentGameScene.CameraView);  //카메라 초기화
        InputManager.Instacne.OffAllController();       //조이스틱 오프 

    }
    protected override void ChangeRemainTime()
    {

        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2 && RemainTime <= 0)
        {
            Master_ChangeState(Define.GameState.CountDown);
        }
    }

    //아무것도안함.
    protected override void EndRemainTime()
    {

    }

    //테스트
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Master_ChangeState(Define.GameState.CountDown);
        }
    }

    public void Test()
    {
        Master_ChangeState(Define.GameState.CountDown);
    }

}
