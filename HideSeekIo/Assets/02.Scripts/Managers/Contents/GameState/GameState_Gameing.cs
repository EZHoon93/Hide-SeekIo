using Photon.Pun;

using TMPro;

using UnityEngine;

public class GameState_Gameing : GameState_Base
{
    int n_hiderCount;
    int n_seekerCount;
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

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
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
    protected override void Setup()
    {
        _initRemainTime = Managers.Game.CurrentGameScene.InitGameTime;      //플레이타임 설정 
        uI_Main.ResetTexts();
        uI_Main.UpdateInGameTime(Managers.Game.CurrentGameScene.InitGameTime);

        if (PhotonNetwork.IsMasterClient)
        {
            UpdatePlayerCount();

        }

    }
    protected override void ChangeRemainTime()
    {
        uI_Main.UpdateInGameTime(RemainTime);

    }
    protected override void EndRemainTime()
    {
        //숨는팀 승리 !!
        photonView.RPC("GameEndOnServer", RpcTarget.All, Define.Team.Hide);
    }
    public void UpdatePlayerCount()
    {
        hiderCount = Managers.Game.GetHiderCount();
        seekerCount = Managers.Game.GetSeekerCount();
        //술래팀 승리.!! 
        if (hiderCount <= 0)
        {
            photonView.RPC("GameEndOnServer", RpcTarget.All, Define.Team.Seek);
        }
    }

    [PunRPC]
    public void GameEndOnServer(Define.Team winTeam)
    {
        string noticeContent = null;
        if (winTeam == Define.Team.Hide)
        {
            noticeContent = Util.GetColorContent(Color.blue, "숨는 팀승리!! ");
        }
        else
        {
            noticeContent = Util.GetColorContent(Color.red, "술래 팀승리!! ");
        }
        uI_Main.UpdateNoticeText(noticeContent);
        uI_Main.noticeBg.enabled = true;
        uI_Main.titleText.text = "잠시 후 다른 게임으로 입장합니다!!";

        if (PhotonNetwork.IsMasterClient)
        {
            Master_ChangeState(Define.GameState.End);
        }

    }
}