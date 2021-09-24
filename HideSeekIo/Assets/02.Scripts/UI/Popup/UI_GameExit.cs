using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_GameExit : UI_Popup
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
        Managers.UI.ClosePopupUI();
        PhotonGameManager.Instacne.GameExit();
    }

    void Cancel(PointerEventData pointerEventData)
    {
        Managers.UI.ClosePopupUI();
    }

}
