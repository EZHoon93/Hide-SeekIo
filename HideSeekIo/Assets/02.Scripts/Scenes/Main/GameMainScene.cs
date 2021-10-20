using System.Collections;
using System.Collections.Generic;

using Photon.Pun;

using UnityEngine;
using System;

public class GameMainScene : GameScene
{
    public Define.MissionType[] missionArray { get; private set; } =
    {
       Define.MissionType.Key
    };

    //진행한 이벤트리스트
    public List<Define.MissionType> doMissionList { get; private set; } = new List<Define.MissionType>(4);



    //public Enum[] hiderItemArray { get; private set; } =
    //{
    //    Define.Skill.Invinc , Define.Skill.Staeth, Define.Skill.Dash,
    //};

    protected override void Init()
    {
        base.Init();
    }

    protected override void Start()
    {
        base.Start();
        mission1ok = false;
        //print("GameMainScene Start!!!");
        //PhotonGameManager.Instacne.AddListenr(Define.GameState.Wait, Clear);
    }

    public override void Clear()
    {
        doMissionList.Clear();
    }

    public override void OnUpdateTime(int remainGameTime)
    {
        //if(remainGameTime < mission1Time)
        //{
        //    if (mission1ok) return;
        //    mission1ok = true;
        //    CreateMission();    
        //}

    }

    void CreateMission()
    {
        if (PhotonNetwork.IsMasterClient == false) return;
        //var ranMissionType = Util.GetRandomEnumTypeExcept(doMissionList.ToArray());
        //object[] sendData = { ranMissionType };
        object[] sendData = { Define.MissionType.Key};
        //이벤트 생성
        PhotonNetwork.InstantiateRoomObject("Mission", Vector3.zero, Quaternion.identity , 0 , sendData);
    }
    //public Enum[] GetSelectList(Define.Team team)
    //{
    //    if(team == Define.Team.Hide)
    //    {
    //        return hiderItemArray;
    //    }

    //    return null;
    //}



}
