using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
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
    Skill_Base skill_Base;
    public List<int> statDataList { get; set; } = new List<int>();
    [SerializeField] float _currentEnergy;
    [SerializeField] float _moveSpeed;
    [SerializeField] float _maxEnergy;
    [SerializeField] float _energyRegenAmount = 1;
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
            var prevPoint = _statPoint;
            _statPoint = value;
            if (_statPoint > 0)
            {
                if (photonView.IsMine)
                {

                    if (_statPoint == 1 || prevPoint > _statPoint)
                    {
                    }
                }
            }
        }
    }

    public float EnergyRegemAmount {
        get => _energyRegenAmount;
        set
        {
            _energyRegenAmount = value;
        }
    }

    #endregion

    private void Awake()
    {
    }

    private void OnEnable()
    {
        _statPoint = 0;
        statDataList.Clear();
    }

    private void OnDisable()
    {
        
    }

    public void OnPhotonInstantiate()
    {
        PhotonGameManager.Instacne.AddListenr(Define.GameState.Gameing, GameStart_SeletRandomSkill);
    }

    public void ChangeTeam(Define.Team team)
    {

        if(team == Define.Team.Hide)
        {
            _energyRegenAmount = 1;
        }
        else
        {
            MaxEnergy *= 1.5f;
            _energyRegenAmount = 2;

        }
    }

    void GameStart_SeletRandomSkill()
    {
        //로컬유저만 실
        if (this.photonView.IsMine == false) return;
        var statSelectArray = Managers.StatSelectManager.RandomSelectOnlySkill();    //랜덤으로 선택된 3개의 스킬목록
        //컨트롤 캐릭
        if (this.IsMyCharacter())
        {
            var uimain = Managers.UI.SceneUI as UI_Main;
            uimain.StatController.ShowSelectList(statSelectArray);
        }
        //AI
        else
        {
            print("AI데이터보냄");
            var ranSelect = UnityEngine.Random.Range(0,statSelectArray.Length);
            var selectType =  statSelectArray[ranSelect];
            Managers.StatSelectManager.PostEvent_StatDataToServer(GetComponent<PlayerController>(), selectType);

        }
    }


}
