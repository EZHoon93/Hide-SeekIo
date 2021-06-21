using Photon.Pun;
using TMPro;


public class GameState_Gameing : GameState_Base, IPunObservable
{
    int _initGameTime = 100 ;

    TextMeshProUGUI _inGametimeText;
    TextMeshProUGUI _noticeText;
    TextMeshProUGUI _countDownText;



    protected override void Setup()
    {
        var mainSceneUI = Managers.UI.SceneUI as UI_Main;
        _inGametimeText = mainSceneUI.GetText(UI_Main.TextMeshProUGUIs.InGameTime);
        _noticeText = mainSceneUI.GetText(UI_Main.TextMeshProUGUIs.Notice);
        _countDownText = mainSceneUI.GetText(UI_Main.TextMeshProUGUIs.CountDown);

        _countDownText.text = null;
        _noticeText.text = null;

        _initRemainTime = _initGameTime;
        print("셋업 노티스,카운트");

    }



    protected override void ChangeRemainTime()
    {
        string timeStr = Util.GetTimeFormat(RemainTime);
        _inGametimeText.text = timeStr;
    }

    protected override void EndRemainTime()
    {
        Master_ChangeState(Define.GameState.End);
    }

    //좀비팀승리시. GameManger에서 호출
    public void ZombieTeamWin()
    {
        Master_ChangeState(Define.GameState.End);
    }

    

}