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

        Get<Button>((int)Buttons.Confirm).gameObject.BindEvent((PointerEventData) =>
        {
            Managers.UI.ClosePopupUI();
            Managers.UI.SceneUI.GetComponent<UI_Main>().ChangePanel(Define.GameScene.Lobby);
        });
        Get<Button>((int)Buttons.Cancel).gameObject.BindEvent((PointerEventData) => { Managers.UI.ClosePopupUI(); });



    }

}
