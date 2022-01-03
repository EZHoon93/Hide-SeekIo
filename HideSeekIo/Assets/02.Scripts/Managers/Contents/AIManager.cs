using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Data;
using System.Linq;
using System;
using Photon.Realtime;
public class AIManager : MonoBehaviour , IListener
{
    List<int> _nickNameIndexList = new List<int>(16);
    private void Awake()
    {
        Managers.aIManager = this;

        Managers.photonGameManager.AddEventListenr(Define.InGamePhotonEvent.NewMaster, NewMaster);
    }


    void NewMaster(Player newMasterPlayer)
    {

    }


    public void CreateAI(int count, ref List<SendAllSkinInfo> currentUserInfos)
    {
        var nameList = AISetting.Instance.aINames.OrderBy(s => Guid.NewGuid()).Take(count).ToArray();

        for (int i = currentUserInfos.Count; i < count; i++)  //나머지 자리 AI추가.
        {
            //SendAllSkinInfo sendAllSkinInfo = UtillGame.MakeRandomAIInfo();
            //sendAllSkinInfo.nickName = nameList[i];
            //
            //currentUserInfos.Add(sendAllSkinInfo);
        }
    }

    public void AddAIInfo(ref List<SendAllSkinInfo> sendAllSkinInfos , int maxPlayerCount)
    {
        for (int i = sendAllSkinInfos.Count; i < maxPlayerCount; i++)  //나머지 자리 AI추가.
        {
            
        }
    }
    //public void SetupRandomSkinnfo(Dictionary<int, object[]> objectDataDic, int maxPlayerCount)
    //{
    //    for(int i = objectDataDic.Count; i < maxPlayerCount; i++)
    //    {
    //        objectDataDic.Add(-i, new object[]
    //        {
    //            -i,
    //            (string)GetRandomNickName(),
    //            (int)Managers.productSetting.GetSkinIndexByType(Define.ProductType.Character) ,
    //            0,
    //            0,
    //            0
    //        });
    //    }
    //}


    public void SetupRandomSkinnfo(Dictionary<int, Dictionary<string, object>> playerDataTable, int maxPlayerCount )
    {
        for (int i = playerDataTable.Count; i < maxPlayerCount; i++)
        {
            playerDataTable.Add(-i, new Dictionary<string, object>()
            {
                //["nu"] = -i,        //넘버
                ["nn"] = GetRandomNickName(),          //닉네임
                ["te"] = Define.Team.Hide,           //팀
                ["ch"] = 0,                          //캐릭스킨
                ["we"] = 0,                        //무기스킨
                ["ac"] = 0,                        //악세스킨
            });
        }
    }
    public string GetRandomNickName()
    {
        int index;
        while (true)
        {
            index = UnityEngine.Random.Range(0, AISetting.Instance.aINames.Length);
            if(_nickNameIndexList.Contains(index) == false)
            {
                _nickNameIndexList.Add(index);
                return AISetting.Instance.aINames[index];
            }
        }
    }
    public SendAllSkinInfo GetSendAllSkinInfo()
    {
        SendAllSkinInfo resultInfo = new SendAllSkinInfo() ;
        resultInfo.avaterKey = 0;
        //resultInfo.accessoriesSkinID = ProductSetting.Instance.GetRandomSkinName(Define.ProductType.Accessories, null);
        resultInfo.nickName = GetRandomNickName();
        resultInfo.autoNumber = -_nickNameIndexList.Count;    //음수로. 
        resultInfo.team = Define.Team.Hide;
        return resultInfo;
    }



    public void OnEvent(Define.EventType eventType, Enum @enum, Component sender, params object[] param)
    {
        switch (eventType)
        {
            case Define.EventType.Photon:
                OnPhotonEvent((Define.InGamePhotonEvent)@enum, sender, param);
                break;
        }
    }

    void OnPhotonEvent(Define.InGamePhotonEvent photonEvent ,Component sender, params object[] param)
    {
        switch (photonEvent)
        {
            case Define.InGamePhotonEvent.NewMaster:
                OnMasterChange((Player)param[0]);
                break;
        }
    }

    void OnMasterChange(Player newMasterPlayer)
    {
        //게임 진행중이지않은상태라면 X
        if(newMasterPlayer.IsLocal == false || Managers.Game.gameStateType < Define.GameState.GameReady)
        {
            return;
        }

        var allLivingArray = Managers.Game.GetAllLivingEntity();
        foreach(var living in allLivingArray)
        {
            if(living.gameObject.IsValidAI() == false)
            {
                return;
            }

            living.GetComponent<PlayerInput>().ChangePlayerType(Define.PlayerType.AI);
        }
    }



}
