using System.Collections.Generic;

using Photon.Pun;

public class GameState_Gameing : GameState_Base , IPunObservable
{
    int n_hiderCount;
    int n_seekerCount;
    Dictionary<int, Dictionary<string, object>> _playerDataTable;

    public int seekerCount
    {
        get => n_seekerCount;
        set
        {
            n_seekerCount = value;
            //uI_Main.UpdateSeekerCount(value);
        }
    }
    public int hiderCount
    {
        get => n_hiderCount;
        set
        {
            n_hiderCount = value;
            //uI_Main.UpdateHiderCount(value);
        }
    }
    public override float remainTime => _gameScene.initGameTime;


    protected override void OnEnable()
    {
        base.OnEnable();
        GetComponent<GameStateController>().ChangeInitTime(_gameScene.initGameTime);    //게임타임 설정
    }

    public override void OnPhotonInstantiate(PhotonMessageInfo info, float createServerTime)
    {
        uI_Main.ResetTexts();   // Text 리셋 
        _playerDataTable = (Dictionary<int, Dictionary<string, object>>)info.photonView.InstantiationData[1];
        //방장만 실행. 술래생성
        _gameScene.PlayerSpawnOnGameStart(_playerDataTable);

        Invoke("OffText", 2.0f);
    }
    public override void OnUpdate(int remainTime)
    {
        Managers.Game.inGameTime = remainTime;
        UpdatePlayerCount();

    }
    public override void OnTimeEnd()
    {
        NextScene(Define.GameState.End, Define.Team.Hide);  //숨는팀 승리로 게임 종료
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


}