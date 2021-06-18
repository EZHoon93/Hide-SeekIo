using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System;

public abstract class PlayerController : MonoBehaviourPun, IPunObservable
{
    public event Action<int> CoinChangeEvent;   //코인변경 이벤트
    protected int _coin;    


    public int Coin { get => _coin; set { _coin = value; CoinChangeEvent?.Invoke(value); } }
    public int TimeCoinAmount { get; set; } = 1;
    protected virtual bool isDead { get; set; }
    //public Define.Team Team { get => GetTeam(); }
    public string NickName { get; set; }

    bool n_sync;


    //public List<Define.AbilityType> _buyAbilityList;    //상점에서 구매한 영구적 능력리스트

    private void Start()
    {

    }
    public virtual void OnPhotonInstantiate() 
    {
        if (this.IsMyCharacter())    //내캐릭 이라면
        {
            var mainSceneUI = Managers.UI.SceneUI as UI_Main;
            mainSceneUI.InGameStore.Setup(GetTeam());
            CoinChangeEvent = mainSceneUI.InGameStore.UpdateCoinText; ; //코인 변경 이벤트
        }

        Coin = 300;   //모든 플레이어 캐릭들은 코인 0으로시작

        StartCoroutine(AddCoinByTime());
    }
    //포톤뷰 그룹을 위한 동기화 필요 여부
    protected virtual void FixedUpdate()
    {
        n_sync = false;
    }

    //동기화가 필요하면 true => 
    [PunRPC]
    public void CallSync()
    {
        n_sync = true;
    }


    IEnumerator AddCoinByTime()
    {
        while (true)
        {
            Coin += TimeCoinAmount;
            yield return new WaitForSeconds(1.0f);
        }
    }

    public abstract Define.Team GetTeam();
    public abstract LivingEntity GetLivingEntity();


    #region Virtual & Abstract 



    protected virtual void HandleDeath()
    {
        if (this.IsMyCharacter())
        {
            //var inventory = UIManager.instance.GetSingleUI(UI_Single_Base.EzType.Inventory) as UI_Inventory;
            //inventory.SetActive(false, Team);
        }
    }
    #endregion



    #region InterPace

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(n_sync);
        }
        else
        {
            n_sync = (bool)stream.ReceiveNext();
        }
    }


}

    #endregion