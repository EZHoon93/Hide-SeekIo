using System.Collections.Generic;

using Photon.Pun;

using TMPro;

using UnityEngine;

public class GameState_Gameing : GameState_Base , IPunObservable
{
    //bool _gameStart;    //게임 시작
    //readonly int _maxEventCount = 3;
    //readonly int _eventCreateTimeInterval = 30;

    //int _currentEventCount = 0;
    //float _remainEventCreateTime;

    int n_hiderCount;
    int n_seekerCount;
    //float n_endEventTime;    //최근 끝난 이벤트 타임
    GameMainScene _gameMainScene;

    public int seekerCount
    {
        get => n_seekerCount;
        set
        {
            n_seekerCount = value;
            uI_Main.UpdateSeekerCount(value);
        }
    }
    public int hiderCount
    {
        get => n_hiderCount;
        set
        {
            n_hiderCount = value;
            uI_Main.UpdateHiderCount(value);
        }
    }
    public override float remainTime => Managers.Game.CurrentGameScene.initGameTime;

    protected override void OnEnable()
    {
        base.OnEnable();
        _gameMainScene = Managers.Game.CurrentGameScene as GameMainScene;
        GetComponent<GameStateController>().ChangeInitTime(_gameMainScene.initGameTime);    //게임타임 설정
    }

    public override void OnPhotonInstantiate(PhotonMessageInfo info, float createServerTime)
    {
        uI_Main.ResetTexts();
        if (PhotonNetwork.IsMasterClient)
        {
            UpdatePlayerCount();
        }

        if (Managers.Game.myPlayer)
        {
            var team = Managers.Game.myPlayer.Team;
            if (team == Define.Team.Seek)
            {
                var noticeContent = Util.GetColorContent(Color.red, "당신은 술래입니다. ");
                uI_Main.noticeBg.enabled = true;
                uI_Main.UpdateNoticeText(noticeContent);
            }
        }
        uI_Main.titleText.text = "술래가 정해졌습니다.";
        Invoke("OffText", 2.0f);
    }
    public override void OnUpdate(int remainTime)
    {
        uI_Main.UpdateInGameTime(remainTime);   //시간
        _gameMainScene.OnUpdateTime(remainTime);
        UpdatePlayerCount();    //인원
    }
    public override void OnTimeEnd()
    {
        NextScene(Define.GameState.End, Define.Team.Hide);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hiderCount);
            stream.SendNext(seekerCount);
        }
        else
        {
            hiderCount = (int)stream.ReceiveNext();
            seekerCount = (int)stream.ReceiveNext();
        }
    }

    void OffText()
    {
        uI_Main.titleText.text = null;
        uI_Main.noticeBg.enabled = false;
        uI_Main.UpdateNoticeText(null);
    }
   
    public void UpdatePlayerCount()
    {
        hiderCount = Managers.Game.GetHiderCount();
        seekerCount = Managers.Game.GetSeekerCount();

        //print($"Hider:{hiderCount} / Seeker :{seekerCount}");
        //술래팀 승리.!! 
        if (hiderCount <= 0)
        {
            NextScene(Define.GameState.End, Define.Team.Seek);
        }
    }

    ///// <summary>
    ///// 조건1. 방장,이벤트최대3이하
    ///// 조건2. 시간 0이하, 현재미션이 없다면 생성
    ///// </summary>
    //void CheckEvent()
    //{
    //    if (_currentEventCount >= _maxEventCount || currentMission != null || !PhotonNetwork.IsMasterClient  ) return; //이미 이벤트 진행중이거나, 이벤트최대횟수,방장아니면 리턴
    //    _remainEventCreateTime = (n_endEventTime + _eventCreateTimeInterval) - (float)PhotonNetwork.Time;

    //    if(_remainEventCreateTime <= 0 ) 
    //    {
    //        var ranMissionType  = Util.GetRandomEnumTypeExcept(_existMissionList.ToArray());
    //        object[] sendData = { ranMissionType };
    //        //이벤트 생성
    //        PhotonNetwork.Instantiate("GameMission", Vector3.zero, Quaternion.identity , 0 , sendData);
    //    }

    //}

    //[PunRPC]
    //public void GameEndOnServer(Define.Team winTeam)
    //{

    //}


}