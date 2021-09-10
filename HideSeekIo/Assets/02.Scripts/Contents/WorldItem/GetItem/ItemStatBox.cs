using System;
using System.Collections;
using Photon.Pun;
using Random = UnityEngine.Random;
using UnityEngine;

public class ItemStatBox : ItemBox_Base
{
    public override void Get(GameObject getObject)
    {
        var playerController = getObject.GetComponent<PlayerController>();
        if (playerController == null) return;
        playerController.playerStat.StatPoint++;
    }
}
