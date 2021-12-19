using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class PlayerStat : Stat
{
    public enum StatChange
    {
        level
    }
    StatChange _statChange;

    int _level;
    int _statPoint;
    public List<int> statDataList { get; set; } = new List<int>();
    //[SerializeField] float _currentEnergy;
    //[SerializeField] float _maxEnergy;
    //[SerializeField] float _energyRegenAmount = 1;

    
    [SerializeField] int _initHp;
    [SerializeField] float _initMoveSpeed;

    //delegate void StantChangeDelegate(StatChange , int);
    public event Action<StatChange, object> statChangeListenrers;
    public event Action<int> changeShootMaxEnergyEvent;
    public event Action<float> changeShootCurrentEnergyEvent;
    public event Action<int> changeHealthEvent;

    PlayerHealth _playerHealth;
    PlayerShooter _playerShooter;
    PlayerMove _playerMove;



    #region 프로퍼티

    public int level { get => _level;
        set
        {
            _level = value;
            statChangeListenrers?.Invoke(StatChange.level, _level);
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

   


    #endregion

    private void Awake()
    {
        _playerHealth = GetComponent<PlayerHealth>();
        _playerMove = GetComponent<PlayerMove>();
        _playerShooter = GetComponent<PlayerShooter>();

        
    }
    private void OnEnable()
    {
        _statPoint = 0;
        statDataList.Clear();

        _playerHealth.maxHp = _initHp;
    }

    private void Update()
    {
        //float newEnergy = Mathf.Clamp(shootCurrentEnergy + Time.deltaTime, 0, shootMaxEnergy);
        //shootCurrentEnergy  = Mathf.Clamp(shootCurrentEnergy + Time.deltaTime * 0.3f , 0, _shootMaxEnergy);
    }
    public void OnPhotonInstantiate()
    {
        //Managers.Game.AddListenrOnGameState(Define.GameState.Gameing, GameStart_SeletRandomSkill);
    }

    public void ChangeTeam(Define.Team team)
    {

        if(team == Define.Team.Hide)
        {
        }
        else
        {

        }
    }

    //void GameStart_SeletRandomSkill()
    //{
    //    //로컬유저만 실
    //    if (this.photonView.IsMine == false) return;
    //    var statSelectArray = Managers.StatSelectManager.RandomSelectOnlySkill();    //랜덤으로 선택된 3개의 스킬목록
    //    //컨트롤 캐릭
    //    if (this.IsMyCharacter())
    //    {
    //        var uimain = Managers.UI.SceneUI as UI_Main;
    //        uimain.StatController.ShowSelectList(statSelectArray);
    //    }
    //    //AI
    //    else
    //    {
    //        var ranSelect = UnityEngine.Random.Range(0,statSelectArray.Length);
    //        var selectType =  statSelectArray[ranSelect];
    //        Managers.StatSelectManager.PostEvent_StatDataToServer(GetComponent<PlayerController>(), selectType);

    //    }
    //}


   
}


