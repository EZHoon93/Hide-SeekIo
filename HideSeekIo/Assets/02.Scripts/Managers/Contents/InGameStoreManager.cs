using System;
using System.Collections;
using System.Linq;

using UnityEngine;
using Photon.Pun;
public class InGameStoreManager : GenricSingleton<InGameStoreManager>
{

    /// <summary>
    /// 게임 아이템 구매시 => 가격체크, 구매팀에 따른 판단
    /// </summary>
    /// <param name="enum"></param>
    /// <param name="playerController"></param>
    /// <returns></returns>
    public Define.InGameItemUIState BuyItem_OnLocal(Enum @enum, PlayerController playerController)
    {
        int coinPrice = 0;  //아이템 가격

        if (playerController.Coin < coinPrice)
            return Define.InGameItemUIState.Failed;
        
        //Hider
        if( typeof( Define.HiderStoreList )== @enum.GetType())
        {
            return BuyHiderItem_OnLocal(@enum, playerController as HiderController);
        }
        //Seekr
        else
        {
            //Instance.SeekrBuyItemOnLocal(Define.SeekrStoreList.DirectionCurse, 1);
            return BuySeekerItem_OnLocal(@enum, playerController as SeekerController);
        }
    }
    public Define.InGameItemUIState BuyHiderItem_OnLocal(Enum @enum, HiderController hiderController)
    {

        var hiderItemEnum = (Define.HiderStoreList)@enum;

        switch (hiderItemEnum)
        {
            case Define.HiderStoreList.Trap:
                GameManager.Instance.SpawnManager.WorldItemSpawn(Define.WorldItem.Trap, hiderController.transform.position );
                break;
            case Define.HiderStoreList.Box:
                GameManager.Instance.SpawnManager.WorldItemSpawn(Define.WorldItem.Box, hiderController.transform.position);
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

        photonView.RPC("ResultBuyInGameItem_OnAllClients", RpcTarget.All, Define.InGameItemUIState.SucessRecycle, hiderItemEnum, hiderController.ViewID());

        return Define.InGameItemUIState.SucessRecycle;
    }
    //seeker팀 아이템 
    public Define.InGameItemUIState BuySeekerItem_OnLocal(Enum @enum, SeekerController seekerController)
    {
        var seekerItemEnum = (Define.SeekrStoreList)@enum;

        switch (seekerItemEnum)
        {
            case Define.SeekrStoreList.ChangeWeapon:
                GameManager.Instance.SpawnManager.WeaponSpawn(Define.Weapon.Sniper, seekerController.seekerAttack);
                break;
            case Define.SeekrStoreList.DirectionCurse:
                BuffManager.Instance.HiderTeamBuffControllerToServer(Define.BuffType.Direction, seekerController.ViewID());

                break;
            case Define.SeekrStoreList.SightCurse:
                BuffManager.Instance.HiderTeamBuffControllerToServer(Define.BuffType.Sight, seekerController.ViewID());

                break;
            case Define.SeekrStoreList.Speed:
                BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.Speed, seekerController.livingEntity);
                break;
            case Define.SeekrStoreList.BodyUp:
                BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.Shield, seekerController.livingEntity);
                break;
            case Define.SeekrStoreList.AllumanClap:
                //BuffManager.Instance.HiderTeamBuffControllerOnLocal
                break;
        }
        photonView.RPC("ResultBuyInGameItem_OnAllClients", RpcTarget.All, Define.InGameItemUIState.Sucess, seekerItemEnum, seekerController.ViewID());

        return Define.InGameItemUIState.SucessRecycle;
    }


    //서버쪽

    [PunRPC]
    public void ResultBuyInGameItem_OnAllClients(Define.InGameItemUIState inGameItemUIState, Enum @enum, int viewID)
    {
        var usePlayer = GameManager.Instance.GetLivingEntity(viewID);
        if (usePlayer == null) return;

        if (typeof(Define.HiderStoreList) == @enum.GetType())
        {
            Result_BuyHiderItemOnAllClients(@enum, usePlayer);
        }
        //Seekr
        else
        {
            Result_BuySeekerItemOnAllClients(@enum, usePlayer);
        }
    }
    public void  Result_BuyHiderItemOnAllClients(Enum @enum, LivingEntity usePlayer)
    {

        var hiderItemEnum = (Define.HiderStoreList)@enum;

        switch (hiderItemEnum)
        {
            case Define.HiderStoreList.Trap:
                EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, usePlayer.transform.position, 1);
                break;
            case Define.HiderStoreList.Box:
                EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, usePlayer.transform.position, 1);

                break;
            case Define.HiderStoreList.Shoes:
                EffectManager.Instance.EffectOnLocal(Define.EffectType.BuffEffect, usePlayer.transform.position, 1);

                break;
            case Define.HiderStoreList.Speed:
                EffectManager.Instance.EffectOnLocal(Define.EffectType.BuffEffect, usePlayer.transform.position, 1);

                break;
            case Define.HiderStoreList.Shield:
                EffectManager.Instance.EffectOnLocal(Define.EffectType.BuffEffect, usePlayer.transform.position, 1);

                break;
            case Define.HiderStoreList.Grenade:
                EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, usePlayer.transform.position, 1);

                break;
        }


    }
    public void  Result_BuySeekerItemOnAllClients(Enum @enum, LivingEntity usePlayer)
    {

        var seekerItemEnum = (Define.SeekrStoreList)@enum;

        switch (seekerItemEnum)
        {
            case Define.SeekrStoreList.ChangeWeapon:
                EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, usePlayer.transform.position, 0);

                break;
            case Define.SeekrStoreList.DirectionCurse:
                EffectManager.Instance.EffectOnLocal(Define.EffectType.Curse, usePlayer.transform.position, 0);
                EffectManager.Instance.EffectAllLivingEntity(Define.EffectEventType.Hider, Define.EffectType.Curse);    //전체이펙트

                break;
            case Define.SeekrStoreList.SightCurse:
                EffectManager.Instance.EffectOnLocal(Define.EffectType.Curse, usePlayer.transform.position, 0);
                EffectManager.Instance.EffectAllLivingEntity(Define.EffectEventType.Hider, Define.EffectType.Curse);

                break;
            case Define.SeekrStoreList.Speed:
                EffectManager.Instance.EffectOnLocal(Define.EffectType.BuffEffect, usePlayer.transform.position, 0);
                break;
            case Define.SeekrStoreList.BodyUp:
                EffectManager.Instance.EffectOnLocal(Define.EffectType.BuffEffect, usePlayer.transform.position, 0);
                break;
            case Define.SeekrStoreList.AllumanClap:
                EffectManager.Instance.EffectOnLocal(Define.EffectType.Curse, usePlayer.transform.position, 0);
                EffectManager.Instance.EffectAllLivingEntity(Define.EffectEventType.Hider, Define.EffectType.Curse);

                break;
        }

    }


}
