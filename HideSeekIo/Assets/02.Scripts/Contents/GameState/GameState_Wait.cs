using System.Linq;

using Photon.Pun;


//바로 바뀌는거 방지를 위한
public class GameState_Wait : GameState_Base
{
    public override float remainTime  => 5;

    public override void OnPhotonInstantiate(PhotonMessageInfo info, float createServerTime)
    {
        Managers.ContentsClear();
        //Managers.Input.SetActiveController(false);       //조이스틱 오프 
        uI_Main.ResetTexts();
    }

 
    public override void OnUpdate(int remainTime)
    {
        CheckReadyPlayer();
    }
    public override void OnTimeEnd()
    {

    }



    //방장만 주기적으로 레디한 플레이어가 1명이상인지 체크 
    void CheckReadyPlayer()
    {
        if (PhotonNetwork.IsMasterClient == false) return;
        var joinUserCount = PhotonNetwork.CurrentRoom.Players.Values.Count(s => (bool)s.CustomProperties["jn"] == true);
        if (joinUserCount >= 1)
        {
            //1명이라도 게임시작눌렀다면
            NextScene(Define.GameState.CountDown);
        }
    }
}
