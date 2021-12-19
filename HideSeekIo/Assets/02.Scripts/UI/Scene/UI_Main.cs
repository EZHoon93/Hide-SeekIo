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
        //GameInfo,
        KillNotice,
        //StatController,
        //Mission,
    }

    enum Panels
    {
        Lobby,
        Game
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

    public UI_Main_Common common { get; set; }
    public UI_Main_Lobby lobby { get; set; }
    public UI_Main_Game game { get; set; }


    //public UI_InGameInfo InGameInfo => GetObject((int)GameObjects.GameInfo).GetComponent<UI_InGameInfo>();
    //public UI_StatController StatController => GetObject((int)GameObjects.StatController).GetComponent<UI_StatController>();
    //public UI_Mission Mission => GetObject((int)GameObjects.Mission).GetComponent<UI_Mission>();
    public TextMeshProUGUI titleText => GetText(TextMeshProUGUIs.Title);
    public TextMeshProUGUI killText => GetText(TextMeshProUGUIs.KillCount);
    public Image noticeBg => GetImage((int)Images.NoticeBg);


    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(TextMeshProUGUIs));
        Bind<Transform>(typeof(Panels));
        Bind<Image>(typeof(Images));
        ChangePanel(Define.GameScene.Lobby);
    }

    private void OnEnable()
    {
        Managers.Game.AddListenrOnGameEvent(Define.GameEvent.GameEnter, GameJoin);
        Managers.Game.AddListenrOnGameEvent(Define.GameEvent.GameExit, GameExit);

    }

    private void OnDisable()
    {
        //Managers.Game.RemoveListnerOnGameEvent(Define.GameEvent.GameEnter, GameJoin);
        //Managers.Game.RemoveListnerOnGameEvent(Define.GameEvent.GameExit, GameExit);

    }

    void GameJoin(object nullObject)
    {
        ChangePanel(Define.GameScene.Game);
    }

    void GameExit(object nullObject)
    {
        ChangePanel(Define.GameScene.Lobby);
    }
    public void ChangePanel(Define.GameScene selectGameType)
    {
        foreach (var p in Enum.GetValues(typeof(Panels)))
        {
            var go = Get<Transform>((int)p);
            var active = string.Compare(selectGameType.ToString(), p.ToString()) == 0 ? true : false;
            go.gameObject.SetActive(active);
        }
    }

    public void ResetTexts()
    {
        var array = Enum.GetValues(typeof(TextMeshProUGUIs));
        foreach (var e in array)
        {
            GetText((int)e).text = null;
        }
        //InGameInfo.ResetTextes();

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

}
