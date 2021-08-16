using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_QuickChangeChannel : UI_Popup
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
        if( Managers.Scene.CurrentScene.SceneType == Define.Scene.Loading)
        {
            print("로딩으로ㄴ");

            PhotonManager.Instance.ChangeChannel();
        }
        else
        {
            print("로딩으로");
            Managers.Scene.LoadScene(Define.Scene.Loading);
        }
    }

    void Cancel(PointerEventData pointerEventData)
    {
        Managers.UI.ClosePopupUI();
    }
}
