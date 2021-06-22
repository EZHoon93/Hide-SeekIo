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

    public event Action<int> CoinChangeEvent;   //���κ��� �̺�Ʈ

    //bool n_sync;


    //public List<Define.AbilityType> _buyAbilityList;    //�������� ������ ������ �ɷ¸���Ʈ

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
        if (this.IsMyCharacter())    //��ĳ�� �̶��
        {
            var mainSceneUI = Managers.UI.SceneUI as UI_Main;
            mainSceneUI.InGameStore.Setup(Team);
            CoinChangeEvent = mainSceneUI.InGameStore.UpdateCoinText; ; //���� ���� �̺�Ʈ
        }
        Coin = 300;   //��� �÷��̾� ĳ������ ���� 0���ν���

        StartCoroutine(AddCoinByTime());
    }


    ////����� �׷��� ���� ����ȭ �ʿ� ����
    //protected virtual void FixedUpdate()
    //{
    //    n_sync = false;
    //}

    ////����ȭ�� �ʿ��ϸ� true => 
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

