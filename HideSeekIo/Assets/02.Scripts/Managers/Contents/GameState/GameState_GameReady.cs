using TMPro;

using UnityEngine;

//바로 바뀌는거 방지를 위한
public class GameState_GameReady : GameState_Base
{
    int _initGameTime = 5;
    protected override void Setup()
    {
        _initRemainTime = _initGameTime;
        uI_Main.UpdateInGameTime(Managers.Game.CurrentGameScene.InitGameTime); //플레이타임 갖고옴

        if(Managers.Game.myPlayer.Team == Define.Team.Hide)
        {
            var noticeContent = Util.GetColorContent(Color.blue, "숨는 팀 ");
            uI_Main.UpdateNoticeText(noticeContent);
            uI_Main.killText.text = "끝까지 살아 남으세요!!";
            uI_Main.noticeBg.enabled = true;

        }
        else
        {
            var noticeContent = Util.GetColorContent(Color.red, "술래 팀 ");
            uI_Main.UpdateNoticeText(noticeContent);
            uI_Main.killText.text = "숨는팀을 모두 잡으세요!!";
            uI_Main.noticeBg.enabled = true;
        }
    }


    //초시간이 변할때 호출
    protected override void ChangeRemainTime()
    {
        uI_Main.UpdateCountText(RemainTime);
        Managers.Sound.Play("TimeCount", Define.Sound.Effect);

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
