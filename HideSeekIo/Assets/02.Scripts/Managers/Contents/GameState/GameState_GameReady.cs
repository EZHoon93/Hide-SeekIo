using TMPro;

//바로 바뀌는거 방지를 위한
public class GameState_GameReady : GameState_Base
{
    TextMeshProUGUI _countDownText;
    int _initGameTime = 5;


    protected override void Setup()
    {
        var mainSceneUI = Managers.UI.SceneUI as UI_Main;
        _countDownText = mainSceneUI.GetText(UI_Main.TextMeshProUGUIs.Notice);
        _initRemainTime = _initGameTime;


        //ShowHumanAndZombieCount();
    }

    //현재 인간/좀비의 수를 갖고와 UI에 할당
    void ShowHumanAndZombieCount()
    {
        //GameManager.instance.HumanCount = GameManager.instance.GetAllLivingEntity().Count(s => s.Team == Define.Team.Human);
        //GameManager.instance.ZombieCount= GameManager.instance.GetAllLivingEntity().Count(s => s.Team == Define.Team.Zombie);
    }

    //초시간이 변할때 호출
    protected override void ChangeRemainTime()
    {
        _countDownText.text = RemainTime.ToString();
    }

    //시간이 0초일 때
    protected override void EndRemainTime()
    {
        _countDownText.text = null;

        Master_ChangeState(Define.GameState.Gameing);
    }

    

  

}
