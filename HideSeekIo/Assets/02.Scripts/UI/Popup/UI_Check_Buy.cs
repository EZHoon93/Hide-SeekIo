using System;

using TMPro;

using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_Check_Buy : UI_Popup
{
    Action _confirmEvent;
    enum Buttons
    {
        Confirm,
        Cancel,
    }

    enum Texts
    {
        Content,
        Price
    }


    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));

        GetButton((int)Buttons.Confirm).gameObject.BindEvent(Confirm);
        GetButton((int)Buttons.Cancel).gameObject.BindEvent(Cancel);



    }

    void Confirm(PointerEventData pointerEventData)
    {
        _confirmEvent?.Invoke();
        Managers.UI.ClosePopupUI();
        
    }

    void Cancel(PointerEventData pointerEventData)
    {
        Managers.UI.ClosePopupUI();
    }

    public void Setup(string content, Action confirmEvent)
    {
        _confirmEvent = confirmEvent;
    }
}
