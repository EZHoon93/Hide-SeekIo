using System.Linq;

using Photon.Pun;


//바로 바뀌는거 방지를 위한
public class GameState_Wait : GameState_Base
{
    int _initSceneWaitTime = 2;  //다음씬으로 넘어가기위한 대기시간

    protected override void Setup()
    {

        _initRemainTime = _initSceneWaitTime;
        CameraManager.Instance.SetupTarget(Managers.Game.CurrentGameScene.CameraView);  //카메라 초기화
        InputManager.Instacne.OffAllController();       //조이스틱 오프 
        uI_Main.ResetTexts();
    }
    protected override void ChangeRemainTime()
    {
        CheckReadyPlayer();
    }

    //방장만 주기적으로 레디한 플레이어가 1명이상인지 체크 
    void CheckReadyPlayer()
    {
        if (PhotonNetwork.IsMasterClient == false) return;
        var joinUserCount = PhotonNetwork.CurrentRoom.Players.Values.Count(s => (bool)s.CustomProperties["jn"] == true);
        if (joinUserCount >= 1)
        {
            //1명이라도 게임시작눌렀다면
            Master_ChangeState(Define.GameState.CountDown);
        }
    }

    //아무것도안함.
    protected override void EndRemainTime()
    {

    }

}
