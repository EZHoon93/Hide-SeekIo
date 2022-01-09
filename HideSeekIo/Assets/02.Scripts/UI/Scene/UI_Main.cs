using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_Main : UI_Scene ,IListener
{
    enum GameObjects
    {
        //Chatting,
        //GameInfo,
        KillNotice,
        //StatController,
        //Mission,
    }

    enum Panels
    {
        //Lobby,
        //Game
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

    #region Independent

    public UI_GameMenu uI_GameMenu { get; set; }
    public UI_Chatting uI_Chatting { get; set; }
    public UI_InGameInfo uI_InGameInfo { get; set; }

    #endregion

    public TextMeshProUGUI titleText => GetText(TextMeshProUGUIs.Title);
    public TextMeshProUGUI killText => GetText(TextMeshProUGUIs.KillCount);
    public Image noticeBg => GetImage((int)Images.NoticeBg);


    public override void Init()
    {
        base.Init();
        
        //Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(TextMeshProUGUIs));
        //Bind<Transform>(typeof(Panels));
        Bind<Image>(typeof(Images));
        //ChangePanel(Define.GameScene.Lobby);


        uI_Chatting =  Managers.UI.MakeIndependentUI<UI_Chatting>();
        uI_GameMenu = Managers.UI.MakeIndependentUI<UI_GameMenu>();
        uI_InGameInfo = Managers.UI.MakeIndependentUI<UI_InGameInfo>();

    }

    private void Start()
    {
    }

    private void OnEnable()
    {
        Managers.EventManager.AddListener(EventDefine.EventType.InGame, this);
    }

    private void OnDisable()
    {
        //Managers.Game.RemoveListnerOnGameEvent(Define.GameEvent.GameEnter, GameJoin);
        //Managers.Game.RemoveListnerOnGameEvent(Define.GameEvent.GameExit, GameExit);

    }

    void GameJoin(object isJoin)
    {
        if ((bool)isJoin)
        {
            ChangePanel(Define.GameScene.Game);
        }
        else
        {
            ChangePanel(Define.GameScene.Lobby);
        }
    }

  
    public void ChangePanel(Define.GameScene selectGameType)
    {
        //foreach (var p in Enum.GetValues(typeof(Panels)))
        //{
        //    var go = Get<Transform>((int)p);
        //    var active = string.Compare(selectGameType.ToString(), p.ToString()) == 0 ? true : false;
        //    go.gameObject.SetActive(active);
        //}
    }

    public void ResetTexts()
    {
        //var array = Enum.GetValues(typeof(TextMeshProUGUIs));
        //foreach (var e in array)
        //{
        //    GetText((int)e).text = null;
        //}
        ////InGameInfo.ResetTextes();

        //noticeBg.enabled = false;
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

    //public void UpdateSeekerCount(int count)
    //{
    //    InGameInfo.UpdateSeekerText(count);
    //}

    //public void UpdateHiderCount(int count)
    //{
    //    InGameInfo.UpdateHiderText(count);

    //}

    //public void UpdateInGameTime(int newtime)
    //{
    //    InGameInfo.UpdateGameTimeText(newtime);
    //}


    public void UpdateMyCharacterDie(string content)
    {
        var deathText = GetText(TextMeshProUGUIs.DeathInfo);
        deathText.text = content;
        Color color = deathText.color;
        color.a = 1;
        deathText.color = color;
        deathText.DOFade(0.0f, 2.0f);


    }



    public void UpdateKillNotice(string killPlayer, string deathPlayer)
    {
        var killNoticeTransform = GetObject((int)GameObjects.KillNotice);

        var killNoticeUI = Managers.Resource.Instantiate("UI/SubItem/UI_KillNotice").GetComponent<UI_KillNotice>();
        killNoticeUI.transform.ResetTransform(killNoticeTransform.transform);
        killNoticeUI.Setup(killPlayer, deathPlayer);
    }

    #region Event CallBack
    public void OnEvent(EventDefine.EventType eventType, System.Enum @enum, Component sender, params object[] param)
    {
        switch (eventType)
        {
            case EventDefine.EventType.InGame:
                CallBack_InGameEvent(@enum, sender, param);
                break;
            default:
                break;
        }
    }

    void CallBack_InGameEvent(System.Enum @enum, Component sender, params object[] param)
    {
        switch ((EventDefine.InGameEvent)@enum)
        {
            case EventDefine.InGameEvent.GameJoin:
                bool isJoin = (bool)param[0];
                uI_GameMenu.gameObject.SetActive(!isJoin);
                break;
        }
    }



    #endregion 
}
