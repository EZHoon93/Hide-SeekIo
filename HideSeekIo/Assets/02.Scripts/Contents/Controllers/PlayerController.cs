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

    public event Action<int> CoinChangeEvent;   //���κ��� �̺�Ʈ
    public Define.Team Team => GetLivingEntity().Team;
    public abstract LivingEntity GetLivingEntity();


    public virtual void OnPhotonInstantiate() 
    {
        if (this.IsMyCharacter())    //��ĳ�� �̶��
        {
            var mainSceneUI = Managers.UI.SceneUI as UI_Main;
            mainSceneUI.InGameStore.Setup(Team);
            CoinChangeEvent = mainSceneUI.InGameStore.UpdateCoinText; ; //���� ���� �̺�Ʈ
        }
        Coin = 300;   //��� �÷��̾� ĳ������ ���� 0���ν���
        print("OnPhotonInstantiate PlayerController     ;");
        StartCoroutine(AddCoinByTime());
    }

    protected virtual void HandleDeath()
    {

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

