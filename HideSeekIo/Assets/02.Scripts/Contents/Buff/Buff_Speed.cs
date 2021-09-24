using System.Collections;

using UnityEngine;

public class Buff_Speed : BuffBase
{
    //MoveBase _applayMoveBase;   //적용하고있는 객체

    float addRatioAmount = 0.13f;
    PlayerMove _playerMove;
    public override void ProcessStart()
    {
        _playerMove = _buffController.livingEntity.GetComponent<PlayerMove>();
        _playerMove.AddMoveBuffList(addRatioAmount, true);
    }
    public override void ProcessEnd()
    {
        _playerMove.AddMoveBuffList(addRatioAmount, false);
    }

}
