using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_AppExit : UI_Popup
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
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }

    void Cancel(PointerEventData pointerEventData)
    {
        Managers.UI.ClosePopupUI();
    }

}
