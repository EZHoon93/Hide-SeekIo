using TMPro;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


public class UI_Main : UI_Scene
{
    enum GameObjects
    {
        Chatting,
        InGameStore,
        UserList
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
        Server
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


    public UI_UserList uI_UserList => Get<GameObject>((int)GameObjects.UserList).GetComponent<UI_UserList>();
    public UI_InGameStore InGameStore => Get<GameObject>((int)GameObjects.InGameStore).GetComponent<UI_InGameStore>();

    public TextMeshProUGUI CountDown => Get<TextMeshProUGUI>((int)TextMeshProUGUIs.CountDown).GetComponent<TextMeshProUGUI>();
    public TextMeshProUGUI Notice => Get<TextMeshProUGUI>((int)TextMeshProUGUIs.Notice).GetComponent<TextMeshProUGUI>();


    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(TextMeshProUGUIs));
        Bind<Transform>(typeof(Panels));


        
        Get<Button>((int)Buttons.GameJoin).gameObject.BindEvent((PointerEventData) => { ChangePanel(Define.GameScene.Game); });
        Get<Button>((int)Buttons.GameExit).gameObject.BindEvent((PointerEventData) => { Managers.UI.ShowPopupUI<UI_GameExit>(); });

        GetObject((int)GameObjects.InGameStore).SetActive(false);

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

    public TextMeshProUGUI GetText(TextMeshProUGUIs textType)
    {
        return GetText((int)textType);
    }


}
