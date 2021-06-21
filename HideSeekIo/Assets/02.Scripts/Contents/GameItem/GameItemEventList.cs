using System;
using System.Collections;
using System.Linq;

using UnityEngine;

public static class GameItemEventList
{

    /// <summary>
    /// 게임 아이템 구매시 => 가격체크, 구매팀에 따른 판단
    /// </summary>
    /// <param name="enum"></param>
    /// <param name="playerController"></param>
    /// <returns></returns>
    public static Define.InGameItemUIState BuyItem(Enum @enum, PlayerController playerController)
    {
        int coinPrice = 0;  //아이템 가격

        if (playerController.Coin < coinPrice)
            return Define.InGameItemUIState.Failed;
        
        //Hider
        if( typeof( Define.HiderStoreList )== @enum.GetType())
        {
            return UseHiderItem(@enum, playerController as HiderController);
        }
        //Seekr
        else
        {
            return UseSeekerItem(@enum, playerController as SeekerController);
        }
    }

    //Hider팀 아이템 
    public static Define.InGameItemUIState UseHiderItem(Enum @enum, HiderController hiderController)
    {

        var seekrItemEnum = (Define.HiderStoreList)@enum;

        switch (seekrItemEnum)
        {
            case Define.HiderStoreList.Trap:
                GameManager.Instance.SpawnManager.PhotonSpawn(Define.PhotonObject.Trap, hiderController.transform.position );
                break;
            case Define.HiderStoreList.Box:
                GameManager.Instance.SpawnManager.PhotonSpawn(Define.PhotonObject.Box, hiderController.transform.position);
                break;
            case Define.HiderStoreList.Shoes:
                BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.Shoes, hiderController.livingEntity);
                break;
            case Define.HiderStoreList.Speed:
                BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.Speed, hiderController.livingEntity);
                break;
            case Define.HiderStoreList.Shield:
                BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.Shield, hiderController.livingEntity);
                break;
            case Define.HiderStoreList.Grenade:
                GameManager.Instance.SpawnManager.WeaponSpawn(Define.Weapon.Grenade, hiderController.hiderAttack);
                break;
        }

        return Define.InGameItemUIState.SucessRecycle;
    }
    //seeker팀 아이템 
    public static Define.InGameItemUIState UseSeekerItem(Enum @enum, SeekerController seekerController)
    {
        var seekrItemEnum = (Define.SeekrStoreList)@enum;

        switch (seekrItemEnum)
        {
            case Define.SeekrStoreList.ChangeWeapon:
                GameManager.Instance.SpawnManager.WeaponSpawn(Define.Weapon.Sniper, seekerController.seekerAttack);
                break;
            case Define.SeekrStoreList.DirectionCurse:
                BuffManager.Instance.HiderTeamBuffControllerToServer(Define.BuffType.Direction, seekerController.ViewID());

                break;
            case Define.SeekrStoreList.SightCurse:
                break;
        }

        return Define.InGameItemUIState.SucessRecycle;
    }

    

}
