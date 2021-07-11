
using UnityEngine;
using System.Collections.Generic;

public class Timer_Glue : TimerItem
{
    List<MoveBase> _moveBase = new List<MoveBase>();

    public override void EndTime()
    {
       foreach(var m in _moveBase)
        {
            m.AddMoveBuffList(-0.5f, false);
        }
    }

    private void OnEnable()
    {
        _moveBase.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        var moveBase = other.GetComponent<MoveBase>();
        if (moveBase != null)
        {
            _moveBase.Add(moveBase);
            moveBase.AddMoveBuffList(-0.5f, true);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        print(other.gameObject.name + "> 나감");

        var moveBase = other.GetComponent<MoveBase>();
        if (moveBase != null)
        {
            _moveBase.Add(moveBase);
            moveBase.AddMoveBuffList(-0.5f, false);
        }
    }
}
