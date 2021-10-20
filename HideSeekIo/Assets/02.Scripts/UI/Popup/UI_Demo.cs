using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Demo : UI_Popup
{
    enum Buttons
    {
        Confirm,
    }


    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.Confirm).gameObject.BindEvent(Confirm);
    }

    void Confirm(PointerEventData pointerEventData)
    {
        Managers.UI.ClosePopupUI();
    }


}
