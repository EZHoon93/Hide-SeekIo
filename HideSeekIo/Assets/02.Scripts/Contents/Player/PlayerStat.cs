using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using System.Linq;
using System;

public class PlayerStat : MonoBehaviourPun
{
    public enum StatChange
    {
        level
    }
    StatChange _statChange;

    int _level;
    int _statPoint;
    List<int> _statDataList = new List<int>();
    [SerializeField] float _currentEnergy;
    [SerializeField] float _moveSpeed;
    [SerializeField] float _maxEnergy;

    //delegate void StantChangeDelegate(StatChange , int);
    public event Action<StatChange, object> statChangeListenrers;
    #region 프로퍼티

    public int level { get => _level;
        set
        {
            _level = value;
            statChangeListenrers?.Invoke(StatChange.level, _level);
        }
    }
    public float moveSpeed
    {
        get => _moveSpeed;
        set
        {
            _moveSpeed = value;
        }
    }

    public float MaxEnergy
    {
        get => _maxEnergy;
        set
        {
            _maxEnergy = value;
        }
    }
    public float CurrentEnergy 
    {
        get => _currentEnergy;
        set
        {
            _currentEnergy = value;
        }
    }


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


    #endregion

   
    
    private void OnEnable()
    {
        _statPoint = 0;
        _statDataList.Clear();
    }

    public void AddStatChangeEvent(Action<StatChange,object> notification)
    {

    }

    public void Recive_ChangeTeam()
    {
        var gameMainScene  = Managers.Game.CurrentGameScene.GetComponent<GameMainScene>();
        if (gameMainScene)
        {
            //var enumList = gameMainScene.GetSelectList(_playerHealth.Team);
            StatPoint++;
        }
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
