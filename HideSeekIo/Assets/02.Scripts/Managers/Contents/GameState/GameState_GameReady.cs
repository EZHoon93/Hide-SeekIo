using System.Collections.Generic;
using System.Linq;

using Photon.Pun;

using TMPro;

using UnityEngine;

//바로 바뀌는거 방지를 위한
public class GameState_GameReady : GameState_Base
{
    int _initGameTime = 5;
    int _selectSeekerCount = 1;
    protected override void Setup()
    {
        _initRemainTime = _initGameTime;
        uI_Main.UpdateInGameTime(Managers.Game.CurrentGameScene.InitGameTime); //플레이타임 갖고옴
        //var noticeContent = Util.GetColorContent(Color.blue, "숨는 팀 ");
        //uI_Main.noticeBg.enabled = true;
        //uI_Main.UpdateNoticeText(noticeContent);
        //uI_Main.titleText.text = "잠시 후 술래가 정해 집니다.";
        uI_Main.UpdateNoticeText("잠시 후 술래가 정해 집니다.");


    }


    //초시간이 변할때 호출
    protected override void ChangeRemainTime()
    {
        uI_Main.UpdateCountText(RemainTime);
        Managers.Sound.Play("TimeCount", Define.Sound.Effect);

    }
    //시간이 0초일 때
    protected override void EndRemainTime()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            var allHiderList = Managers.Game.GetAllHiderList().ToList();
            List<int> selectSeekerViewIDLIst = new List<int>();
            for (int i = 0; i < _selectSeekerCount; i++)
            {
                int ran = Random.Range(0, allHiderList.Count);
                selectSeekerViewIDLIst.Add(allHiderList[ran].ViewID()); //뷰아이디 등록
                allHiderList.RemoveAt(ran);
            }

            selectSeekerViewIDLIst.Remove(Managers.Game.myPlayer.ViewID()); //뷰아이디 등록
            //selectSeekerViewIDLIst.Add(Managers.Game.myPlayer.ViewID()); //뷰아이디 등록

            photonView.RPC("SelectSeeker", RpcTarget.AllViaServer, selectSeekerViewIDLIst.ToArray());
        }
    }

    [PunRPC]
    public void SelectSeeker(int[] seekerViewList)
    {

        foreach(var s in seekerViewList)
        {
            var selectplayer = Managers.Game.GetLivingEntity(s).GetComponent<PlayerController >();
            if (selectplayer.photonView.IsMine)
            {
                Managers.Spawn.WeaponSpawn(Define.Weapon.Hammer, selectplayer.playerShooter);
            }
        }
        
        if (PhotonNetwork.IsMasterClient)
        {
            Master_ChangeState(Define.GameState.Gameing);
        }
    }

    public void UpdatePlayerCount()
    {
        uI_Main.UpdateHiderCount(Managers.Game.GetHiderCount());
        uI_Main.UpdateHiderCount(Managers.Game.GetSeekerCount());
    }



}
