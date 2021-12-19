using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_SightCurse : BuffBase
{

    public override void ProcessStart()
    {
        _livingEntity.fogController.ratio = 0.3f;
    }

    public override void ProcessEnd()
    {
        _livingEntity.fogController.ratio = 1f;

    }


}
