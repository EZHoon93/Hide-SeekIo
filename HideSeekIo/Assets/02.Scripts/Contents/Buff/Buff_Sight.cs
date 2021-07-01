using System.Collections;

using UnityEngine;
using FoW;

public class Buff_Sight : BuffBase
{

    FogOfWarUnit _fogOfWarUnit;


    public override void ProcessStart()
    {
        _fogOfWarUnit = _buffController.livingEntity.GetComponentInChildren<FogOfWarUnit>();
        _fogOfWarUnit.circleRadius = 1;
    }
    public override void ProcessEnd()
    {
        _fogOfWarUnit = _buffController.livingEntity.GetComponentInChildren<FogOfWarUnit>();
        _fogOfWarUnit.circleRadius = 5;

    }

}
