using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;

public class UI_Main : UI_Scene
{
    enum GameObjects
    {
        Chatting,
        InGameStore,
        UserList_Lobby,
        UserList_Game,
        GameInfo,
        KillNotice
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
        Etc,
        AppExit,
        FindPlayer,
        ADMop
    }

    public enum TextMeshProUGUIs
    {
        CountDown,
        Notice,
        Title,
        DeathInfo,
        KillCount,
    }

    public enum Images
    {
        NoticeBg
    }


    public UI_InGameInfo InGameInfo => GetObject((int)GameObjects.GameInfo).GetComponent<UI_InGameInfo>();
    public Button FindButton => GetButton((int)Buttons.FindPlayer);
    public Button ADButton => GetButton((int)Buttons.ADMop);
    public TextMeshProUGUI titleText => GetText(TextMeshProUGUIs.Title);

    public TextMeshProUGUI killText => GetText(TextMeshProUGUIs.KillCount);
    public Image noticeBg => GetImage((int)Images.NoticeBg);

    public UI_Store_Character storeAvater;
    public UI_Store_Etc storeEtc;


    public override void Init()
    {
        base.Init();
        //Managers.Game.GameResetEvent += () => InGameStore.gameObject.SetActive(false);

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(TextMeshProUGUIs));
        Bind<Transform>(typeof(Panels));
        Bind<Image>(typeof(Images));


        GetButton((int)Buttons.GameExit).gameObject.BindEvent(OnGameExit_ButtonClicked);
        GetButton((int)Buttons.GameJoin).gameObject.BindEvent(OnGameJoin_ButtonClicked);
        GetButton((int)Buttons.Setting).gameObject.BindEvent(OnSetting_ButtonClicked);
        GetButton((int)Buttons.ChangeChannel).gameObject.BindEvent(OnChangeChannel_ButtonClicked);
        GetButton((int)Buttons.QuickChannel).gameObject.BindEvent(OnQuickChangeChannel_ButtonClicked);
        GetButton((int)Buttons.ChangeServer).gameObject.BindEvent(OnChangeServer_ButtonClicked);
        GetButton((int)Buttons.Avater).gameObject.BindEvent(OnAvaterStore_ButtonClicked);
        GetButton((int)Buttons.Etc).gameObject.BindEvent(OnEtcStore_ButtonClicked);
        //GetButton((int)Buttons.UserListButton).gameObject.BindEvent(OnUserListGame_ButtonClicked);
        GetButton((int)Buttons.AppExit).gameObject.BindEvent(OnAppExit_ButtonClicked);
        GetButton((int)Buttons.FindPlayer).gameObject.BindEvent(OnFindPlayer_ButtonClicked);
        GetButton((int)Buttons.ADMop).gameObject.BindEvent(OnADMop_ButtonClicked);




        //GetObject((int)GameObjects.InGameStore).SetActive(false);
        //GetObject((int)GameObjects.UserList_Game).SetActive(false);

        ChangePanel(Define.GameScene.Lobby);

        //uI_KillNoticePrefab = Managers.Resource.Load<GameO>
    }

    public void ChangePanel(Define.GameScene selectGameType)
    {
        foreach (var p in Enum.GetValues(typeof(Panels)))
        {
            var go = Get<Transform>((int)p);
            print(go.gameObject.name + "/" + p.ToString());
            var active = string.Compare(selectGameType.ToString(), p.ToString()) == 0 ? true : false;
            go.gameObject.SetActive(active);

        }
        //로비로 돌아갈시 필요버튼 켜줌
        if (selectGameType == Define.GameScene.Lobby)
        {
            FindButton.gameObject.SetActive(true);
        }
    }

    #region Texts
    public void ResetTexts()
    {
        var array = Enum.GetValues(typeof(TextMeshProUGUIs));
        foreach (var e in array)
        {
            GetText((int)e).text = null;
        }
        InGameInfo.ResetTextes();

        noticeBg.enabled = false;
    }
    public TextMeshProUGUI GetText(TextMeshProUGUIs textType)
    {
        return GetText((int)textType);
    }

    public void UpdateCountText(int newTime)
    {
        if (newTime <= 0)
        {
            GetText(TextMeshProUGUIs.CountDown).text = null;
        }
        else
        {
            GetText(TextMeshProUGUIs.CountDown).text = newTime.ToString();
        }
    }

    public void UpdateNoticeText(string content)
    {
        GetText(TextMeshProUGUIs.Notice).text = content;
    }

    public void UpdateSeekerCount(int count)
    {
        InGameInfo.UpdateSeekerText(count);
    }

    public void UpdateHiderCount(int count)
    {
        InGameInfo.UpdateHiderText(count);

    }

    public void UpdateInGameTime(int newtime)
    {
        InGameInfo.UpdateGameTimeText(newtime);
    }

    public void UpdateMyCharacterDie(string content )
    {
        var deathText =  GetText(TextMeshProUGUIs.DeathInfo);
        deathText.text = content;
        Color color = deathText.color;
        color.a = 1;
        deathText.color = color;
        deathText.DOFade(0.0f, 2.0f);
        

    }

    #endregion

    #region OnButtonCliied  PopUp Event

    void OnGameExit_ButtonClicked(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_GameExit>();
    }

    void OnGameJoin_ButtonClicked(PointerEventData data)
    {
        PhotonGameManager.Instacne.GameJoin();
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
        var outLine = data.selectedObject.GetComponent<UI_SelectButtonOutLine>();
        if (outLine)
        {
            outLine.Click();

        }
        if (storeEtc)
            storeEtc.gameObject.SetActive(false);
        if (storeAvater == null)
        {
            storeAvater =  Managers.UI.ShowPopupUI<UI_Store_Character>();

        }
        else
        {
            storeAvater.gameObject.SetActive(!storeAvater.gameObject.activeInHierarchy);

        }


    }

    public void OnEtcStore_ButtonClicked(PointerEventData data)
    {
        var outLine = data.selectedObject.GetComponent<UI_SelectButtonOutLine>();
        if (outLine)
        {
            outLine.Click();
        }
        if (storeAvater)
            storeAvater.gameObject.SetActive(false);
        if(storeEtc== null)
        {
            storeEtc = Managers.UI.ShowPopupUI<UI_Store_Etc>();
        }
        else
        {
            storeEtc.gameObject.SetActive(!storeEtc.gameObject.activeInHierarchy);
        }

    }

    public void OnWeaponStore_ButtonClicked(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_Store_Weapon>();
    }

    public void OnUserListGame_ButtonClicked(PointerEventData data)
    {
        //GetObject((int)GameObjects.UserList_Game).SetActive(true);
    }
    public void OnAppExit_ButtonClicked(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_AppExit>();
    }

    public void OnFindPlayer_ButtonClicked(PointerEventData data)
    {
        CameraManager.Instance.FindNextPlayer();
        //Managers.UI.Root.gameObject.SetActive(false);
    }
    public void OnADMop_ButtonClicked(PointerEventData data)
    {

    }
    public void Test(PointerEventData data)
    {
        PhotonGameManager.Instacne.ChangeRoomStateToServer(Define.GameState.CountDown);

    }
    public void Test1()
    {
        PhotonGameManager.Instacne.ChangeRoomStateToServer(Define.GameState.CountDown);

    }
    public void Test2()
    {
        //CameraManager.Instance.TestChange();
        Managers.UI.Root.gameObject.SetActive(false);

    }

    public void UpdateKillNotice(string killPlayer, string deathPlayer)
    {
        var killNoticeTransform = GetObject((int)GameObjects.KillNotice);

        var killNoticeUI = Managers.Resource.Instantiate("UI/SubItem/UI_KillNotice").GetComponent<UI_KillNotice>();
        killNoticeUI.transform.ResetTransform(killNoticeTransform.transform);
        killNoticeUI.Setup(killPlayer, deathPlayer);
    }
    #endregion

}
