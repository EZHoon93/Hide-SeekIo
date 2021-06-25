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
        Master_ChangeState(Define.GameState.End);
    }



    //좀비팀승리시. GameManger에서 호출
    public void ZombieTeamWin()
    {
        Master_ChangeState(Define.GameState.End);
    }


    public void UpdatePlayerCount()
    {
        hiderCount = Managers.Game.GetHiderCount();
        seekerCount = Managers.Game.GetSeekerCount();
    }


}