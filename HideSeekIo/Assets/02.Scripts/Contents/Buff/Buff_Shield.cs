using System.Collections;

using UnityEngine;

public class Buff_Shield : BuffBase
{

    public override void ProcessStart()
    {
        _buffController.livingEntity.isShield = true;
        _buffController.livingEntity.GetComponent<PlayerMove>().isInvinc = true;

    }

    public override void ProcessEnd()
    {
        _buffController.livingEntity.isShield = false;
        _buffController.livingEntity.GetComponent<PlayerMove>().isInvinc = false;
    }
}
