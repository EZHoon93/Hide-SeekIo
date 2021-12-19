using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : ItemBox_Base
{

    public override void Get(GameObject getObject)
    {
        var playerController =  getObject.GetComponent<PlayerController>();
        if (playerController)
        {

        }
    }

}
