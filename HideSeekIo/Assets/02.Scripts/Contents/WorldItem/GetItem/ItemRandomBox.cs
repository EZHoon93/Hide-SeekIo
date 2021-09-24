using System.Collections;
using System.Collections.Generic;

using Photon.Pun;

using UnityEngine;

public class ItemRandomBox : ItemBox_Base
{
    Define.InGameItem[] hiderItemArray =
    {
         Define.InGameItem.Flash,Define.InGameItem.Grenade,Define.InGameItem.PoisonBomb,
       Define.InGameItem.Stone

    };
    Define.InGameItem[] seekerItemArray =
    {
        Define.InGameItem.Flash,Define.InGameItem.Grenade,Define.InGameItem.PoisonBomb,
       Define.InGameItem.Stone
    };


    public override void Get(GameObject getObject)
    {
        var playerController = getObject.GetComponent<PlayerController>();
        if (playerController == null) return;

        if (playerController.photonView.IsMine == false) return;

        Define.InGameItem[] itemTypeArray;
        if (playerController.Team == Define.Team.Hide)
        {
            itemTypeArray = hiderItemArray;
        }
        else
        {
            itemTypeArray = seekerItemArray;
        }

        if (playerController.photonView.IsMine)
        {
            var ranType = GetRandomItemID(itemTypeArray);
            Managers.Spawn.InGameItemSpawn(ranType, playerController);
        }
    }

    Define.InGameItem GetRandomItemID(Define.InGameItem[] itemTypeArray)
    {
        var ran = Random.Range(0, itemTypeArray.Length);
        var seletType = itemTypeArray[ran];
        return seletType;
    }
}
