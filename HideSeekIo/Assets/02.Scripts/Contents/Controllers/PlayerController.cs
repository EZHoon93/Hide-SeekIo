using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System;

public abstract class PlayerController : MonoBehaviourPun
{
     protected int _coin;    


    public int Coin { get => _coin; set { _coin = value; CoinChangeEvent?.Invoke(value); } }
    public int TimeCoinAmount { get; set; } = 1;
    public string NickName { get; protected set; }

    public LivingEntity livingEntity  { get; private set; }
    public Define.Team Team => livingEntity.Team;

    public event Action<int> CoinChangeEvent;   //코인변경 이벤트

    //bool n_sync;


    //public List<Define.AbilityType> _buyAbilityList;    //상점에서 구매한 영구적 능력리스트

    protected virtual void Awake()
    {
        livingEntity = GetComponent<LivingEntity>();
        livingEntity.onDeath += HandleDeath;
    }
    protected virtual void HandleDeath()
    {
    }

    public virtual void OnPhotonInstantiate() 
    {
        if (this.IsMyCharacter())    //내캐릭 이라면
        {
            var mainSceneUI = Managers.UI.SceneUI as UI_Main;
            mainSceneUI.InGameStore.Setup(Team);
            CoinChangeEvent = mainSceneUI.InGameStore.UpdateCoinText; ; //코인 변경 이벤트
        }
        Coin = 300;   //모든 플레이어 캐릭들은 코인 0으로시작

        StartCoroutine(AddCoinByTime());
    }


    ////포톤뷰 그룹을 위한 동기화 필요 여부
    //protected virtual void FixedUpdate()
    //{
    //    n_sync = false;
    //}

    ////동기화가 필요하면 true => 
    //[PunRPC]
    //public void CallSync()
    //{
    //    n_sync = true;
    //}


    IEnumerator AddCoinByTime()
    {
        while (true)
        {
            Coin += TimeCoinAmount;
            yield return new WaitForSeconds(1.0f);
        }
    }


}

