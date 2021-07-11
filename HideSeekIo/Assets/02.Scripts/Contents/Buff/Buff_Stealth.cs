using System.Collections;

using UnityEngine;

public class Buff_Stealth : BuffBase
{
    LivingEntity livingEntity;

    float addRatioAmount = 0.2f;
    public override void ProcessStart()
    {
        _buffController.livingEntity.fogController.ChangeTransParent(true);
    }
    public override void ProcessEnd()
    {
        _buffController.livingEntity.fogController.ChangeTransParent(false);
    }

}
