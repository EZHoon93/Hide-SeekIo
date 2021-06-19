using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ChangeServer : UI_Popup
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
        Managers.UI.SceneUI.GetComponent<UI_Main>().ChangePanel(Define.GameScene.Lobby);
    }

    void Cancel(PointerEventData pointerEventData)
    {
        Managers.UI.ClosePopupUI();
    }
}
