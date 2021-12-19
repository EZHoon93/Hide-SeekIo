using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class UI_FindCamera : UI_Button
{
    void Start()
    {
        Managers.Game.AddListenrOnGameState(Define.GameState.Wait, () => SetActive(false));
        Managers.Game.AddListenrOnGameState(Define.GameState.CountDown, () => SetActive(false));
        //Managers.Game.AddListenrOnGameState(Define.GameState.Gameing, Check);
        //Managers.photonGameManager.onMyCharacter += Check;
        //if (Managers.Game.gameStateController == null) return;
        switch (Managers.Game.gameStateType)
        {
            case Define.GameState.Wait:
            case Define.GameState.CountDown:
            case Define.GameState.GameReady:
                this.gameObject.SetActive(false);
                break;
            case Define.GameState.Gameing:
            case Define.GameState.End:
                this.gameObject.SetActive(true);
                break;
        }
    }

    void SetActive(bool active)
    {
        this.gameObject.SetActive(false);
    }

    void Check(bool isActive)
    {
        this.gameObject.SetActive(!isActive);
    }

    protected override void OnClickEvent()
    {
        Managers.cameraManager.FindNextPlayer();
    }
}
