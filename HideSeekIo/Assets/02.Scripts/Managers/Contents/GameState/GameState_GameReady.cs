using TMPro;

//바로 바뀌는거 방지를 위한
public class GameState_GameReady : GameState_Base
{
    int _initGameTime = 5;
    protected override void Setup()
    {
        _initRemainTime = _initGameTime;
        uI_Main.UpdateInGameTime(Managers.Game.CurrentGameScene.InitGameTime); //플레이타임 갖고옴
    }


    //초시간이 변할때 호출
    protected override void ChangeRemainTime()
    {
        //UpdatePlayerCount();
        uI_Main.UpdateCountText(RemainTime);
    }
    //시간이 0초일 때
    protected override void EndRemainTime()
    {
        Master_ChangeState(Define.GameState.Gameing);
    }

    public void UpdatePlayerCount()
    {
        uI_Main.UpdateHiderCount(Managers.Game.GetHiderCount());
        uI_Main.UpdateHiderCount(Managers.Game.GetSeekerCount());
    }



}
