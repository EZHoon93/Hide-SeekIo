using System.Collections;

using UnityEngine;
using Photon.Pun;
using System;

public class InGameItemController : MonoBehaviourPun , IPunInstantiateMagicCallback , IOnPhotonViewPreNetDestroy
{
    public Enum itemEnum { get; private set; }
    public Define.InGameItemType itemType;

    PlayerController hasPlayer; //아이템을 가지고있는 플레이어 

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var data = info.photonView.InstantiationData;
        if (data == null) return;

        int buyPlayerViewID = (int)data[0];
        itemEnum = (System.Enum)data[1];
        hasPlayer = Managers.Game.GetLivingEntity(buyPlayerViewID).GetComponent<PlayerController>();
        if(hasPlayer == null)
        {
            return;
        }

        AddComponenyByItemEnum(itemEnum, hasPlayer);    //해당 컴포넌트 추가.. 


        hasPlayer.AddItem(this);    //플레이어에게 할당
    }

    public void OnPreNetDestroy(PhotonView rootView)
    {
        hasPlayer.RemoveItem(this);
    }

    public void AddComponenyByItemEnum(Enum @enum, PlayerController playerController)
    {

        Component c = GetComponent<Item_Base>();
        if (c != null)
        {
            Destroy(c);
        }

        if (typeof(Define.HiderStoreList) == @enum.GetType())
        {
            UseHideItem_OnLocal(@enum, playerController as HiderController);
            return;
        }
        //Seekr
        else
        {
            UseSeeker_OnLocal(@enum, playerController as SeekerController);
            return;
        }
    }
    public void UseHideItem_OnLocal(Enum @enum, HiderController hiderController)
    {

        var hiderItemEnum = (Define.HiderStoreList)@enum;


        switch (hiderItemEnum)
        {
            case Define.HiderStoreList.Trap:
                this.gameObject.GetOrAddComponent<Item_Trap>();
                itemType = Define.InGameItemType.use;
                break;
            case Define.HiderStoreList.Flash:
                this.gameObject.GetOrAddComponent<Item_Flash>();
                itemType = Define.InGameItemType.Weapon;

                break;
            case Define.HiderStoreList.Grenade:
                this.gameObject.GetOrAddComponent<Item_Grenade>();
                itemType = Define.InGameItemType.Weapon;
                break;
            case Define.HiderStoreList.Speed:
                this.gameObject.GetOrAddComponent<Item_Speed>();
                itemType = Define.InGameItemType.use;
                break;
            case Define.HiderStoreList.Vaccine:
                this.gameObject.GetOrAddComponent<Item_Vaccine>();
                itemType = Define.InGameItemType.use;
                break;
            case Define.HiderStoreList.Shield:
                BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.Shield, hiderController.GetLivingEntity());
                break;
            case Define.HiderStoreList.Shoes:
                BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.Shield, hiderController.GetLivingEntity());
                break;
      
        }
    }
    public void UseSeeker_OnLocal(Enum @enum, SeekerController seekerController)
    {
        var seekerItemEnum = (Define.SeekrStoreList)@enum;

        switch (seekerItemEnum)
        {
            case Define.SeekrStoreList.Dynamite:
                itemType = Define.InGameItemType.Weapon;
                this.gameObject.GetOrAddComponent<Item_Dynamite>();

                break;
            case Define.SeekrStoreList.Fire:
                itemType = Define.InGameItemType.Weapon;
                this.gameObject.GetOrAddComponent<Item_Fire>();

                break;
            case Define.SeekrStoreList.Soil:
                itemType = Define.InGameItemType.Weapon;
                this.gameObject.GetOrAddComponent<Item_Soil>();
                break;
        }
    }


    //일회용 아이템이면 삭제
    public void Use(PlayerController usePlayer)
    {
        GetComponent<Item_Base>().Use(usePlayer);
    }

   
}
