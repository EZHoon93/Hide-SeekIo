using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Data;
using System.Linq;
using System;

public class AIManager : GenricSingleton<AIManager>
{



    private void Start()
    {

    }

    /// <summary>
    /// 현재 유
    /// </summary>
    /// <param name="count"></param>
    /// <param name="currentUserInfos"></param> 현재 참currentUserInfos유저들 스킨,등 목록담긴 리스트
    public void CreateAI(int count, ref List<SendAllSkinInfo> currentUserInfos)
    {
        var nameList = AISetting.Instance.aINames.OrderBy(s=>Guid.NewGuid()).Take(count).ToArray();

        for (int i = currentUserInfos.Count; i < count; i++)  //나머지 자리 AI추가.
        {
            SendAllSkinInfo sendAllSkinInfo = UtillGame.MakeRandomAIInfo();
            sendAllSkinInfo.nickName = nameList[i];
            currentUserInfos.Add(sendAllSkinInfo);
        }


    }
    
}
