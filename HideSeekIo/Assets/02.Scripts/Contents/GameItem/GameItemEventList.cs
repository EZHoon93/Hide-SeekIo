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

        //EffectManager.Instance.EffectOnLocal(Define.EffectType.Curse, hiderController.transform.position);

        return Define.InGameItemUIState.SucessRecycle;
    }
    //seeker팀 아이템 
    public static Define.InGameItemUIState UseSeekerItem(Enum @enum, SeekerController seekerController)
    {
        var seekrItemEnum = (Define.SeekrStoreList)@enum;

        switch (seekrItemEnum)
        {
            case Define.SeekrStoreList.DirectionCurse:
                BuffManager.Instance.BuffControllerCheckOnLocal(Define.BuffType.Direction, seekerController);
                break;
            case Define.SeekrStoreList.SightCurse:
                break;
        }

        return Define.InGameItemUIState.SucessRecycle;
    }

    

}
