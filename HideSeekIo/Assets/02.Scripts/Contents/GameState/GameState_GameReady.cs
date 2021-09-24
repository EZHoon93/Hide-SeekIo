using System.Collections.Generic;
using System.Linq;

using Photon.Pun;

using TMPro;

using UnityEngine;

//바로 바뀌는거 방지를 위한
public class GameState_GameReady : GameState_Base
{

    public override float remainTime => Managers.Game.CurrentGameScene.initReadyTime;

    /// <summary>
    /// GameReady,Start는 게임씬 데이터 이용
    /// /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        var _gameMainScene = Managers.Game.CurrentGameScene as GameMainScene;
        GetComponent<GameStateController>().ChangeInitTime(_gameMainScene.initReadyTime);
    }
    public override void OnPhotonInstantiate(PhotonMessageInfo info, float createServerTime)
    {
        uI_Main.UpdateInGameTime(Managers.Game.CurrentGameScene.initGameTime); //플레이타임 갖고옴 v
        var noticeContent = Util.GetColorContent(Color.blue, "숨는 팀 ");
        uI_Main.noticeBg.enabled = true;
        uI_Main.UpdateNoticeText(noticeContent);
        uI_Main.titleText.text = "잠시 후 술래가 정해 집니다.";
        uI_Main.UpdateNoticeText("잠시 후 술래가 정해 집니다.");

    }
    public override void OnUpdate(int remainTime)
    {
        uI_Main.UpdateCountText(remainTime);
        Managers.Sound.Play("TimeCount", Define.Sound.Effect);
    }
    public override void OnTimeEnd()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            var allHiderList = Managers.Game.GetAllHiderList().ToList();
            List<int> selectSeekerViewIDLIst = new List<int>(10);
            //for (int i = 0; i < _selectSeekerCount; i++)
            //{
            //    int ran = Random.Range(0, allHiderList.Count);
            //    selectSeekerViewIDLIst.Add(allHiderList[ran].ViewID()); //뷰아이디 등록
            //    allHiderList.RemoveAt(ran);
            //}

            //selectSeekerViewIDLIst.Remove(Managers.Game.myPlayer.ViewID()); //뷰아이디 등록
            if (PhotonGameManager.Instacne.testSeeekr)
            {
                selectSeekerViewIDLIst.Add(Managers.Game.myPlayer.ViewID()); //뷰아이디 등록
                allHiderList.Remove(Managers.Game.myPlayer.playerHealth);
            }

            else
            {
                selectSeekerViewIDLIst.Remove(Managers.Game.myPlayer.ViewID()); //뷰아이디 등록
                allHiderList.Add(Managers.Game.myPlayer.playerHealth);
                var ai = allHiderList[allHiderList.Count - 1].ViewID();
                selectSeekerViewIDLIst.Add(ai); //뷰아이디 등록
            }
            List<int> sendData = new List<int>();
            foreach (var s in allHiderList)
            {
                sendData.Add(s.ViewID());
            }
            photonView.RPC("SelectSeeker", RpcTarget.AllViaServer, selectSeekerViewIDLIst.ToArray(), sendData.ToArray());
        }
    }

    //시간이 0초일 때
    //protected override void EndRemainTime()
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        var allHiderList = Managers.Game.GetAllHiderList().ToList();
    //        List<int> selectSeekerViewIDLIst = new List<int>(10);
    //        //for (int i = 0; i < _selectSeekerCount; i++)
    //        //{
    //        //    int ran = Random.Range(0, allHiderList.Count);
    //        //    selectSeekerViewIDLIst.Add(allHiderList[ran].ViewID()); //뷰아이디 등록
    //        //    allHiderList.RemoveAt(ran);
    //        //}

    //        //selectSeekerViewIDLIst.Remove(Managers.Game.myPlayer.ViewID()); //뷰아이디 등록
    //        selectSeekerViewIDLIst.Add(Managers.Game.myPlayer.ViewID()); //뷰아이디 등록
    //        allHiderList.Remove(Managers.Game.myPlayer.playerHealth);
    //        List<int> sendData = new List<int>();
    //        foreach (var s in allHiderList)
    //        {
    //            sendData.Add(s.ViewID());
    //        }
    //        photonView.RPC("SelectSeeker", RpcTarget.AllViaServer, selectSeekerViewIDLIst.ToArray(), sendData.ToArray());
    //    }
    //}

    [PunRPC]
    public void SelectSeeker(int[] seekerArray, int[] hiderArray)
    {
        foreach (var s in seekerArray)
        {
            var selectplayer = Managers.Game.GetLivingEntity(s).GetComponent<PlayerController>();
            if (selectplayer.photonView.IsMine)
            {
                Managers.Spawn.WeaponSpawn(Define.Weapon.Hammer, selectplayer.playerShooter);
                var skillObject = Managers.Resource.Instantiate($"Skill/{Define.Skill.Dash}").GetComponent<Skill_Base>(); //지원론
                skillObject.OnPhotonInstantiate(selectplayer);
            }
        }
        foreach (var s in hiderArray)
        {
            var selectplayer = Managers.Game.GetLivingEntity(s).GetComponent<PlayerController>();
            if (selectplayer.photonView.IsMine)
            {
                var skillObject = Managers.Resource.Instantiate($"Skill/{Define.Skill.Dash}").GetComponent<Skill_Base>(); //지원론
                skillObject.OnPhotonInstantiate(selectplayer);
                //Managers.Spawn.WeaponSpawn(Define.Weapon.Stone, selectplayer.playerShooter);
            }
        }
        if (PhotonNetwork.IsMasterClient)
        {
            NextScene(Define.GameState.Gameing);
        }
    }

    //public void UpdatePlayerCount()
    //{
    //    uI_Main.UpdateHiderCount(Managers.Game.GetHiderCount());
    //    uI_Main.UpdateHiderCount(Managers.Game.GetSeekerCount());
    //}




}
