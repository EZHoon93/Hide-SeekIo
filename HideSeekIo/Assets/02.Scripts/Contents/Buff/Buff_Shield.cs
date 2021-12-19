using System.Collections;

using UnityEngine;

public class Buff_Shield : BuffBase
{

    public override void ProcessStart()
    {
        _livingEntity.isShield = true;
        //_buffController.livingEntity.GetComponent<PlayerMove>().isInvinc = true;

    }

    public override void ProcessEnd()
    {
        _livingEntity.isShield = false;
        //_buffController.livingEntity.GetComponent<PlayerMove>().isInvinc = false;
    }
}
