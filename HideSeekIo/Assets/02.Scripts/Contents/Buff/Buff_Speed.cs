using System.Collections;

using UnityEngine;

public class Buff_Speed : BuffBase
{
    MoveBase _applayMoveBase;   //적용하고있는 객체

    float addRatioAmount = 0.2f;
    public override void ProcessStart()
    {
        _applayMoveBase = _buffController.livingEntity.GetComponent<MoveBase>();
        _applayMoveBase.AddMoveBuffList(addRatioAmount, true);
    }
    public override void ProcessEnd()
    {
        _applayMoveBase.AddMoveBuffList(addRatioAmount, false);

    }

}
