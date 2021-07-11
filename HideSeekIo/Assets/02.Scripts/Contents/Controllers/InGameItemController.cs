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
        var type = (int)data[1];
        hasPlayer = Managers.Game.GetLivingEntity(buyPlayerViewID).GetComponent<PlayerController>();
        if (hasPlayer == null)
        {
            return;
        }
        Component c = GetComponent<Item_Base>();
        if (c != null)
        {
            Destroy(c);
        }


        //타입으로 구분
        switch (type)
        {
            case 0:
                itemEnum = (Define.HiderStoreList)data[2];
                UseHideItem_OnLocal(itemEnum, hasPlayer as HiderController);
                break;
            case 1:
                itemEnum = (Define.SeekrStoreList)data[2];
                UseSeeker_OnLocal(itemEnum, hasPlayer as SeekerController);
                break;
        }

        hasPlayer.AddItem(this);    //플레이어에게 할당
    }

    public void OnPreNetDestroy(PhotonView rootView)
    {
        hasPlayer.RemoveItem(this);
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
            case Define.HiderStoreList.Stealth:
                this.gameObject.GetOrAddComponent<Item_Stealth>();
                itemType = Define.InGameItemType.Weapon;
                break;
            case Define.HiderStoreList.TNT:
                this.gameObject.GetOrAddComponent<Item_TNT>();
                itemType = Define.InGameItemType.Weapon;
                break;
            case Define.HiderStoreList.Glue:
                this.gameObject.GetOrAddComponent<Item_Glue>();
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
            case Define.SeekrStoreList.Flash:
                this.gameObject.GetOrAddComponent<Item_Flash>();
                itemType = Define.InGameItemType.Weapon;
                break;
            case Define.SeekrStoreList.Speed2:
                this.gameObject.GetOrAddComponent<Item_Speed2>();
                itemType = Define.InGameItemType.Weapon;
                break;
            
            case Define.SeekrStoreList.PosionBomb:
                itemType = Define.InGameItemType.use;
                this.gameObject.GetOrAddComponent<Item_PosionBomb>();
                break;
            case Define.SeekrStoreList.SightUp:
                this.gameObject.GetOrAddComponent<Item_SightUp>();
                itemType = Define.InGameItemType.Weapon;
                break;
            case Define.SeekrStoreList.Immune:
                this.gameObject.GetOrAddComponent<Item_Immune>();
                itemType = Define.InGameItemType.use;
                break;
        }
    }


    //일회용 아이템이면 삭제
    public void Use(PlayerController usePlayer)
    {
        GetComponent<Item_Base>().Use(usePlayer);
    }

   
}
