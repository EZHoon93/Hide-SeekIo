using System.Collections;
using UnityEngine;
using Photon.Pun;
using System;
using System.Linq;
using FoW;

public abstract class PlayerController : MonoBehaviourPun
{
    protected int _coin;
    public int Coin { get => _coin; set { _coin = value; CoinChangeEvent?.Invoke(value); } }
    public int TimeCoinAmount { get; set; } = 1;
    public string NickName { get;  set; }

    public event Action<int> CoinChangeEvent;   //코인변경 이벤트
    public Define.Team Team => GetLivingEntity().Team;
    public abstract LivingEntity GetLivingEntity();

    public InGameItemController[] itemInventory { get; protected set; } = new InGameItemController[3];


    public virtual void OnPhotonInstantiate(PhotonView photonView)
    {
        if (this.IsMyCharacter())    //내캐릭 이라면
        {
            var mainSceneUI = Managers.UI.SceneUI as UI_Main;
            mainSceneUI.InGameStore.Setup(Team);
            CoinChangeEvent = mainSceneUI.InGameStore.UpdateCoinText; ; //코인 변경 이벤트
        }
        Coin = 300;   //모든 플레이어 캐릭들은 코인 0으로시작
        for(int i =0; i < itemInventory.Length; i++)
        {
            itemInventory[i] = null;
        }

        StartCoroutine(AddCoinByTime());
        //StartCoroutine(UpdateFog());

    }

    protected virtual void HandleDeath()
    {
    }

    IEnumerator UpdateFog()
    {
        GetComponentInChildren<FogOfWarTeam>().team = this.ViewID();
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            FoW.FogOfWarTeam.GetTeam(this.ViewID()).ManualUpdate(1);
        }
        
    }


    //아이템 살 수 있는지 체크 => 인벤토리 공간이 남는지.
    public bool IsBuyItem()
    {
        return itemInventory.Any(s => s == null);
    }
    public void AddItem(InGameItemController newItem)
    {
        for (int i = 0; i < itemInventory.Length; i++)
        {
            if (itemInventory[i] != null) continue;

            itemInventory[i] = newItem;
            if (this.IsMyCharacter())
            {
                InputManager.Instacne.AddItemByButton(i, newItem);
            }
            return;
        }
    }
    public void RemoveItem(InGameItemController useItem)
    {
        for (int i = 0; i < itemInventory.Length; i++)
        {
            if (itemInventory[i] != useItem) continue;
            
            itemInventory[i] = null;
            if (this.IsMyCharacter())
            {
                InputManager.Instacne.RemoveItemButton(i);
            }
            return;
        }
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

