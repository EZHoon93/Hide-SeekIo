using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_ChangeChannel : UI_Popup
{
    [SerializeField] TMP_InputField _roomName_InputField;
    [SerializeField] Toggle _scretToggle;
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
        Managers.PhotonManager.ChangeChannel(_roomName_InputField.text.ToString(), _scretToggle);
    }

    void Cancel(PointerEventData pointerEventData)
    {
        Managers.UI.ClosePopupUI();
    }
}
