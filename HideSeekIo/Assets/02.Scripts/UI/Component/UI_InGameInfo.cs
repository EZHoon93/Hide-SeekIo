using TMPro;
using UnityEngine;


public class UI_InGameInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _seekerCountText;
    [SerializeField] TextMeshProUGUI _hiderCountText;
    [SerializeField] TextMeshProUGUI _gameTimeText;


    private void Start()
    {
        ResetTextes();

        Managers.Game.AddListenrOnGameState(Define.GameState.Wait, ResetTextes);
        Managers.Game.AddListenrOnGameState(Define.GameState.Gameing, SetupHiderNSeeker);
        Managers.Game.AddListenrOnGameEvent(Define.GameEvent.ChangeHider,UpdateHiderText);
        Managers.Game.AddListenrOnGameEvent(Define.GameEvent.ChangeSeeker, UpdateSeekerText);
        Managers.Game.AddListenrOnGameEvent(Define.GameEvent.ChangeInGameTime, UpdateInGameTimeText);
        

    }

    private void OnDestroy()
    {
        //Managers.Game.changeHiderCount -= UpdateHiderText;
        //Managers.Game.changeSeekerCount -= UpdateSeekerText;
        //Managers.Game.changeInGameTime -= UpdateInGameTimeText;
    }
    private void OnDisable()
    {
    }

    public void ResetTextes()
    {
        UpdateSeekerText(0);
        UpdateHiderText(0);
        var gameScene = Managers.Scene.currentGameScene;
        if (gameScene)
        {
            UpdateInGameTimeText(gameScene.initGameTime);
        }
    }

    void SetupHiderNSeeker()
    {
        UpdateHiderText( Managers.Game.GetHiderCount());
        UpdateSeekerText(Managers.Game.GetSeekerCount());
    }

    public void UpdateSeekerText(object count)
    {
        _seekerCountText.text = count.ToString();
    }

    public void UpdateHiderText(object count)
    {
        _hiderCountText.text = count.ToString();
    }

    public void UpdateInGameTimeText(object newTime)
    {
        _gameTimeText.text = Util.GetTimeFormat((int)newTime);
    }

}
