using System.Collections;
using System.Collections.Generic;

using Data;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Lobby : UI_Scene
{
   
    enum Buttons
    {
        AppExit,
        My,
        Store,
        Setting,
        Record,
        Server,
        GameStart,
        GameFind,
        Left,
        Right
    }

    enum TextMeshProUGUIs
    {
        ChCount
    }


    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(TextMeshProUGUIs));

        GetButton((int)Buttons.AppExit).gameObject.BindEvent(OnButtonClick_AppExit);
        GetButton((int)Buttons.My).gameObject.BindEvent(OnButtonClick_My);
        GetButton((int)Buttons.Store).gameObject.BindEvent(OnButtonClick_Store);
        GetButton((int)Buttons.Setting).gameObject.BindEvent(OnButtonClick_Setting);
        GetButton((int)Buttons.Record).gameObject.BindEvent(OnButtonClick_Record);
        GetButton((int)Buttons.Server).gameObject.BindEvent(OnButtonClick_Server);
        GetButton((int)Buttons.GameStart).gameObject.BindEvent(OnButtonClick_GameStart);
        GetButton((int)Buttons.GameFind).gameObject.BindEvent(OnButtonClick_GameFind);
    }

    #region Button Click Event

    void OnButtonClick_AppExit(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_AppExit>();
    }
    void OnButtonClick_My(PointerEventData data)
    {
        //Managers.UI.ShowPopupUI<UI_GameExit>();
    }
    void OnButtonClick_Store(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_Store>();
    }
    void OnButtonClick_Setting(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_Setting>();
    }
    void OnButtonClick_Record(PointerEventData data)
    {
        //Managers.UI.ShowPopupUI<UI_R>();
    }
    void OnButtonClick_Server(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_ChangeServer>();
    }
    void OnButtonClick_GameStart(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_QuickChangeChannel>();
    }
    void OnButtonClick_GameFind(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_ChangeChannel>();
    }

 
    #endregion
}
