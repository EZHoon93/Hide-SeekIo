using UnityEngine;

public class UI_PopUpCall : UI_Button
{
    [SerializeField] string _key;
    protected override void OnClickEvent()
    {
        Managers.UI.ShowPopupUI<UI_Popup>(_key);

    }

}
