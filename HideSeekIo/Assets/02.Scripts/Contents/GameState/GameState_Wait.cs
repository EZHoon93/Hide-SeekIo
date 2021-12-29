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
        CheckAnyReadyJoinUser();
    }
    public override void OnTimeEnd()
    {

    }
    /// <summary>
    /// 1명이라도 참가한유저가있다면 true,
    /// </summary>
    void CheckAnyReadyJoinUser()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        bool isExistUser = PhotonNetwork.CurrentRoom.Players.Values.Any(s => (bool)s.CustomProperties["jn"] == true); ;
        if (isExistUser)
        {
            Managers.Spawn.GameStateSpawn(Define.GameState.CountDown);
        }
    }
   
}
