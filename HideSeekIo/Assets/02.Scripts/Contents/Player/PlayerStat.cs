using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using System.Linq;

public class PlayerStat : MonoBehaviourPun
{
    int _statPoint;
    List<int> _statDataList = new List<int>();

    public int StatPoint
    {
        get => _statPoint;
        set
        {
            _statPoint = value;
            if (this.IsMyCharacter())
            {
                //UI On
                var mainUI = Managers.UI.SceneUI as UI_Main;
                mainUI.StatController.SetActive(true);
            }
            //AI
            if(photonView.IsMine && this.gameObject.IsValidAI())
            {
              
            }
        }
    }


    private void OnEnable()
    {
        _statPoint = 0;
        _statDataList.Clear();
    }

    public void UPStatPointToServer(Define.StatType newStat)
    {
        if (this.IsMyCharacter() == false) return;  //로컬 유저만 실행.
        var copyOriginalData = _statDataList.ToList();
        Hashtable prevHashtable = new Hashtable()
        {
            { "vID", this.ViewID() },
            { "st" , copyOriginalData.ToArray() },
        };
        copyOriginalData.Add((int)newStat);
        Hashtable nextHashtable = new Hashtable()
        {
            { "vID", this.ViewID() },
            { "st" , copyOriginalData.ToArray() }
        };
        PhotonGameManager.Instacne.SendEvent(Define.PhotonOnEventCode.AbilityCode, EventCaching.RemoveFromRoomCache, prevHashtable);
        PhotonGameManager.Instacne.SendEvent(Define.PhotonOnEventCode.AbilityCode, EventCaching.AddToRoomCacheGlobal, nextHashtable);
    }

    public void UpdateStatDatasByServer(int[] datas)
    {
        var copyReciveData = datas.ToList();
        foreach(var originalData in _statDataList)
        {
            var isExistData = _statDataList.Find(s => s == originalData);
            copyReciveData.Remove(isExistData);
        }
        foreach (var addData in copyReciveData)
        {
            ApplyStat(addData);    //Update
            _statDataList.Add(addData);
        }
    }

    void ApplyStat(int statIndex)
    {
        var statType = (Define.StatType)statIndex;
        switch (statType)
        {
            case Define.StatType.Speed:
                break;
            case Define.StatType.EnergyMax:
                break;
            case Define.StatType.EnergyRegen:
                break;
            case Define.StatType.CoolTime:
                break;
            case Define.StatType.Sight:
                break;
        }
    }


}
