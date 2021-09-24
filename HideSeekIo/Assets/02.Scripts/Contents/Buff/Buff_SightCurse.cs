using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_SightCurse : BuffBase
{

    public override void ProcessStart()
    {
        _buffController.livingEntity.fogController.ratio = 0.3f;
    }

    public override void ProcessEnd()
    {
        _buffController.livingEntity.fogController.ratio = 1f;

    }


}
