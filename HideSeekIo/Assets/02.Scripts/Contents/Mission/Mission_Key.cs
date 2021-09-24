using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Photon.Pun;

using UnityEngine;


/// <summary>
/// 맵에 열쇠를 찾으세요!! 
/// </summary>
public class Mission_Key : Mission_Base
{
    int _initKeyCount = 2;
    public List<Item_Key> keyList { get; set; } = new List<Item_Key>(4);
    public override float missionTime => 20;


    public override void OnStart(MissionInfo missionInfo)
    {
        _uI_Mission.Setup(missionInfo);
        Clear();
        CreateKey();
    }

    public override void OnTimeEnd()
    {
        //미션 실패 시
        _uI_Mission.End();
        Clear();
    }

    public override void OnUpdate(int remainTime)
    {
        //미션 성공시
        if( keyList.Count <= 0)
        {
            _uI_Mission.End();
            Clear();
        }
        _uI_Mission.UpdateRemainTime(Util.GetTimeFormat( remainTime));
        _uI_Mission.UpdateSueessText($"{keyList.Count}/{_initKeyCount}");
    }

    /// <summary>
    /// 키아이템있는지 체keyList
    /// </summary>
    void Clear()
    {
        if (keyList.Count > 0)
        {
            foreach (var key in keyList.ToArray())
            {
                Managers.Resource.PunDestroy(key);
            }
        }
        keyList.Clear();
    }

    void CreateKey()
    {
        if (PhotonNetwork.IsMasterClient == false) return;
        for (int i = 0; i < _initKeyCount; i++)
        {
            var spawnData = Managers.Game.CurrentGameScene.itemSpawnManager.GetSpawnPoint();
            PhotonNetwork.InstantiateRoomObject("ItemKey", spawnData.spawnPos, Quaternion.identity, 0, new object[] { spawnData.spawnIndex, -1 });
        }
    }

}
