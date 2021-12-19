using System.Collections;

using UnityEngine;
using FoW;


/// <summary>
/// Flash   에서 발생, 시야 저주 
/// </summary>

public class Buff_Sight : BuffBase
{

    FogOfWarUnit _fogOfWarUnit;


    public override void ProcessStart()
    {
        _fogOfWarUnit = _livingEntity.GetComponentInChildren<FogOfWarUnit>();
        _fogOfWarUnit.circleRadius = 1;
    }
    public override void ProcessEnd()
    {
        _fogOfWarUnit = _livingEntity.GetComponentInChildren<FogOfWarUnit>();
        _fogOfWarUnit.circleRadius = 5;

    }

}
