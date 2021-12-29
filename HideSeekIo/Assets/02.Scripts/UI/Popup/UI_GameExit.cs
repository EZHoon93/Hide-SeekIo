using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
public class UI_GameExit : UI_Popup
{
    float _lastClickTime;
    float _clickTimeBet = 0.3f;
    enum Buttons
    {
        Confirm,
        Cancel,
    }


    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.Confirm).gameObject.BindEvent(Confirm);
        GetButton((int)Buttons.Cancel).gameObject.BindEvent(Cancel);


        Managers.Game.AddListenrOnGameEvent(Define.GameEvent.GameJoin, GameJoin);
    }

    void Confirm(PointerEventData pointerEventData)
    {

        var userController = (UserController)PhotonNetwork.LocalPlayer.TagObject;
        if (userController == null)
        {
            //없다면 => 에러.. 생성
            return;
        }

        userController.GameExitToServer();        
        //Managers.UI.ClosePopupUI();
        //Managers.Game.NotifyGameEvent(Define.GameEvent.GameJoin, false);
    }

    /// <summary>
    /// 게임 나가기 콜백 성공시.
    /// </summary>
    void GameJoin(object isJoin)
    {
        if((bool)isJoin == false)
        {
            Managers.UI.ClosePopupUI();
        }
    }

    void Cancel(PointerEventData pointerEventData)
    {
        Managers.UI.ClosePopupUI();
    }

}
