using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

public class UI_LobbyExit : UI_Popup
{
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



    }

    void Confirm(PointerEventData pointerEventData)
    {
        Managers.Scene.LeaveGameRoom();
    }

    void Cancel(PointerEventData pointerEventData)
    {
        Managers.UI.ClosePopupUI();
    }

}
