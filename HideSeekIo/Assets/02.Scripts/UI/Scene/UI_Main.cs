using TMPro;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Photon.Realtime;

public class UI_Main : UI_Scene
{
    enum GameObjects
    {
        Chatting,
        InGameStore,
        UserList_Lobby,
        UserList_Game
    }

    enum Panels
    {
        Lobby,
        Game
    }


    enum Buttons
    {
        GameExit,
        GameJoin,
        Setting,
        ChangeChannel,
        QuickChannel,
        ChangeServer,
        Avater,
        Weapon,
        Etc,
        UserListButton,
        AppExit
            
    }

    public enum TextMeshProUGUIs
    {
        CountDown,
        Notice,
        DeathInfo,
        KillCount,
        ZombieCount,
        HumanCount,
        InGameTime
    }


    public UI_InGameStore InGameStore => Get<GameObject>((int)GameObjects.InGameStore).GetComponent<UI_InGameStore>();

    public TextMeshProUGUI CountDown => Get<TextMeshProUGUI>((int)TextMeshProUGUIs.CountDown).GetComponent<TextMeshProUGUI>();
    public TextMeshProUGUI Notice => Get<TextMeshProUGUI>((int)TextMeshProUGUIs.Notice).GetComponent<TextMeshProUGUI>();


    public override void Init()
    {
        base.Init();
        GameManager.Instance.GameResetEvent += () => InGameStore.gameObject.SetActive(false);

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(TextMeshProUGUIs));
        Bind<Transform>(typeof(Panels));


        GetButton((int)Buttons.GameExit).gameObject.BindEvent(OnGameExit_ButtonClicked);
        GetButton((int)Buttons.GameJoin).gameObject.BindEvent(OnGameJoin_ButtonClicked);
        GetButton((int)Buttons.Setting).gameObject.BindEvent(OnSetting_ButtonClicked);
        GetButton((int)Buttons.ChangeChannel).gameObject.BindEvent(OnChangeChannel_ButtonClicked);
        GetButton((int)Buttons.QuickChannel).gameObject.BindEvent(OnQuickChangeChannel_ButtonClicked);
        GetButton((int)Buttons.ChangeServer).gameObject.BindEvent(OnChangeServer_ButtonClicked);
        GetButton((int)Buttons.Avater).gameObject.BindEvent(OnAvaterStore_ButtonClicked);
        GetButton((int)Buttons.Weapon).gameObject.BindEvent(OnWeaponStore_ButtonClicked);
        GetButton((int)Buttons.Etc).gameObject.BindEvent(OnEtcStore_ButtonClicked);
        GetButton((int)Buttons.UserListButton).gameObject.BindEvent(OnUserListGame_ButtonClicked);
        GetButton((int)Buttons.AppExit).gameObject.BindEvent(OnAppExit_ButtonClicked);


        GetObject((int)GameObjects.InGameStore).SetActive(false);
        GetObject((int)GameObjects.UserList_Game).SetActive(false);

        ChangePanel(Define.GameScene.Lobby);
    }

    public void ChangePanel(Define.GameScene selectGameType)
    {
        foreach(var p in Enum.GetValues(typeof(Panels)))
        {
            var go = Get<Transform>((int)p);
            print(go.gameObject.name + "/" + p.ToString());
            var active = string.Compare(selectGameType.ToString(), p.ToString()) == 0 ? true : false;
            go.gameObject.SetActive(active);
            
        }
    }

    public void ResetTexts()
    {
        var array = Enum.GetValues( typeof(TextMeshProUGUIs)) ;
        foreach(var e in array)
        {
            GetText((int)e).text = null;
        }

        GetText(TextMeshProUGUIs.InGameTime).text = "00:00";
        GetText(TextMeshProUGUIs.HumanCount).text = "0";
        GetText(TextMeshProUGUIs.ZombieCount).text = "0";


    }
    public TextMeshProUGUI GetText(TextMeshProUGUIs textType)
    {
        return GetText((int)textType);
    }

    #region OnButtonCliied  PopUp Event

    void OnGameExit_ButtonClicked(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_GameExit>();
    }

    void OnGameJoin_ButtonClicked(PointerEventData data)
    {
        GameManager.Instance.GameJoin();
        ChangePanel(Define.GameScene.Game);
    }

    void OnChangeChannel_ButtonClicked(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_ChangeChannel>();


    }

    void OnQuickChangeChannel_ButtonClicked(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_QuickChangeChannel>();


    }

    void OnChangeServer_ButtonClicked(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_ChangeServer>();


    }

    public void OnSetting_ButtonClicked(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_Setting>();

    }

    public void OnAvaterStore_ButtonClicked(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_AvaterStore>();

    }

    public void OnEtcStore_ButtonClicked(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_EtcStore>();

    }

    public void OnWeaponStore_ButtonClicked(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_WeaponStore>();
    }

    public void OnUserListGame_ButtonClicked(PointerEventData data)
    {
        //GetObject((int)GameObjects.UserList_Game).SetActive(true);
    }
    public void OnAppExit_ButtonClicked(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_AppExit>();
    }
    #endregion

}
