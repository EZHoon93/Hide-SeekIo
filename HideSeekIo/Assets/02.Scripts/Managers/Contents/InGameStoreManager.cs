﻿using System;
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
        //코인부족, 인벤토리 부족이면 실패
        if (playerController.Coin < coinPrice || playerController.IsBuyItem() == false)
            return Define.InGameItemUIState.Failed;
        //Hider
        if (typeof(Define.HiderStoreList) == @enum.GetType())
        {
            return BuyHiderItem_OnLocal(@enum, playerController as HiderController);
        }
        //Seekr
        else
        {
            return BuySeekerItem_OnLocal(@enum, playerController as SeekerController);
        }
    }
    public Define.InGameItemUIState BuyHiderItem_OnLocal(Enum @enum, HiderController hiderController)
    {

        var hiderItemEnum = (Define.HiderStoreList)@enum;
        Define.InGameItemUIState resultState = Define.InGameItemUIState.Failed;

        switch (hiderItemEnum)
        {
            case Define.HiderStoreList.Trap:
            case Define.HiderStoreList.Flash:
            case Define.HiderStoreList.Speed:
            case Define.HiderStoreList.Vaccine:
            case Define.HiderStoreList.Grenade:
                Managers.Spawn.InGameItemSpawn(@enum, hiderController);
                resultState = Define.InGameItemUIState.SucessRecycle;
                break;

            case Define.HiderStoreList.Shoes:
                BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.Shoes, hiderController.GetLivingEntity());
                resultState = Define.InGameItemUIState.Sucess;

                break;
            case Define.HiderStoreList.Shield:
                BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.Shield, hiderController.GetLivingEntity());
                resultState = Define.InGameItemUIState.Sucess;

                break;

                //case Define.HiderStoreList.Shoes:
                //    BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.Shoes, hiderController.GetLivingEntity());
                //    break;
                //case Define.HiderStoreList.Speed:
                //    BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.Speed, hiderController.GetLivingEntity());
                //    break;
                //case Define.HiderStoreList.Shield:
                //    BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.Shield, hiderController.GetLivingEntity());
                //    break;
                //case Define.HiderStoreList.Grenade:
                //    Managers.Spawn.WeaponSpawn(Define.Weapon.Grenade, hiderController.hiderAttack);
                //    break;
        }

        //photonView.RPC("ResultBuyInGameItem_OnAllClients", RpcTarget.All,
        //    Define.InGameItemUIState.Sucess, Define.Team.Hide, (int)hiderItemEnum, hiderController.ViewID());


        return resultState;
    }
    //seeker팀 아이템 
    public Define.InGameItemUIState BuySeekerItem_OnLocal(Enum @enum, SeekerController seekerController)
    {
        var seekerItemEnum = (Define.SeekrStoreList)@enum;
        Define.InGameItemUIState resultState = Define.InGameItemUIState.Failed;

        switch (seekerItemEnum)
        {
            case Define.SeekrStoreList.Dynamite:
            case Define.SeekrStoreList.Fire:
            case Define.SeekrStoreList.Soil:
                Managers.Spawn.InGameItemSpawn(@enum, seekerController);
                resultState = Define.InGameItemUIState.SucessRecycle;
                break;

            case Define.SeekrStoreList.Mask:
                BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.Shoes, seekerController.GetLivingEntity());
                resultState = Define.InGameItemUIState.Sucess;
                break;
            case Define.SeekrStoreList.Rader:
                break;
            case Define.SeekrStoreList.PowerUp:
                BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.Shoes, seekerController.GetLivingEntity());
                resultState = Define.InGameItemUIState.Sucess;
                break;
            case Define.SeekrStoreList.Curse1:
                Managers.Spawn.InGameItemSpawn(@enum, seekerController);
                resultState = Define.InGameItemUIState.SucessRecycle;
                break;
            case Define.SeekrStoreList.Curse2:
                Managers.Spawn.InGameItemSpawn(@enum, seekerController);
                resultState = Define.InGameItemUIState.SucessRecycle;
                break;
            case Define.SeekrStoreList.SightUp:
                BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.Shoes, seekerController.GetLivingEntity());
                resultState = Define.InGameItemUIState.Sucess;
                break;
            case Define.SeekrStoreList.Speed2:
                BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.Shoes, seekerController.GetLivingEntity());
                resultState = Define.InGameItemUIState.Sucess;
                break;
        }
        //photonView.RPC("ResultBuyInGameItem_OnAllClients", RpcTarget.All, Define.InGameItemUIState.Sucess,Define.Team.Seek, (int)seekerItemEnum, seekerController.ViewID());

        return resultState;

    }


    //서버쪽

    [PunRPC]
    public void ResultBuyInGameItem_OnAllClients(Define.InGameItemUIState inGameItemUIState,Define.Team team,  int @enum, int viewID)
    {
        var usePlayer = Managers.Game.GetLivingEntity(viewID);
        if (usePlayer == null) return;
        switch (team)
        {
            case Define.Team.Hide:
                Result_BuyHiderItemOnAllClients(Util.GetEnumByIndex<Define.HiderStoreList>(@enum), usePlayer);
                break;
            case Define.Team.Seek:
                Result_BuySeekerItemOnAllClients(Util.GetEnumByIndex<Define.SeekrStoreList>(@enum), usePlayer);
                break;

        }
        //if (typeof(Define.HiderStoreList) == @enum.GetType())
        //{
        //    Result_BuyHiderItemOnAllClients(@enum, usePlayer);
        //}
        ////Seekr
        //else
        //{
        //    Result_BuySeekerItemOnAllClients(@enum, usePlayer);

        //}
    }
    public void  Result_BuyHiderItemOnAllClients(Enum @enum, LivingEntity usePlayer)
    {

        var hiderItemEnum = (Define.HiderStoreList)@enum;

        switch (hiderItemEnum)
        {
            case Define.HiderStoreList.Trap:
                EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, usePlayer.transform.position, 1);
                break;
            //case Define.HiderStoreList.Box:
            //    EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, usePlayer.transform.position, 1);

            //    break;
            //case Define.HiderStoreList.Shoes:
            //    EffectManager.Instance.EffectOnLocal(Define.EffectType.BuffEffect, usePlayer.transform.position, 1);

            //    break;
            //case Define.HiderStoreList.Speed:
            //    EffectManager.Instance.EffectOnLocal(Define.EffectType.BuffEffect, usePlayer.transform.position, 1);

            //    break;
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
            //case Define.SeekrStoreList.ChangeWeapon:
            //    EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, usePlayer.transform.position, 0);

            //    break;
            //case Define.SeekrStoreList.DirectionCurse:
            //    EffectManager.Instance.EffectOnLocal(Define.EffectType.Curse, usePlayer.transform.position, 0);
            //    EffectManager.Instance.EffectAllLivingEntity(Define.EffectEventType.Hider, Define.EffectType.Curse);    //전체이펙트

            //    break;
            //case Define.SeekrStoreList.SightCurse:
            //    EffectManager.Instance.EffectOnLocal(Define.EffectType.Curse, usePlayer.transform.position, 0);
            //    EffectManager.Instance.EffectAllLivingEntity(Define.EffectEventType.Hider, Define.EffectType.Curse);

            //    break;
            //case Define.SeekrStoreList.Speed:
            //    EffectManager.Instance.EffectOnLocal(Define.EffectType.BuffEffect, usePlayer.transform.position, 0);
            //    break;
            //case Define.SeekrStoreList.BodyUp:
            //    EffectManager.Instance.EffectOnLocal(Define.EffectType.BuffEffect, usePlayer.transform.position, 0);
            //    break;
            //case Define.SeekrStoreList.AllumanClap:
            //    EffectManager.Instance.EffectOnLocal(Define.EffectType.Curse, usePlayer.transform.position, 0);
            //    EffectManager.Instance.EffectAllLivingEntity(Define.EffectEventType.Hider, Define.EffectType.Curse);

            //    break;
        }

    }


}
