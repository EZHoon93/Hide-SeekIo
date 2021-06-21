using System.Collections;

using FoW;

using UnityEngine;


public class Buff_Direction : BuffBase
{
    InputBase _InputBase;

    public override void ProcessStart()
    {
        _InputBase = _buffController.livingEntity.GetComponent<InputBase>();
        if (_InputBase == null) return;

        float x = Random.Range(-1.0f, 1.0f);
        float y = Random.Range(-1.0f, 1.0f);
        _InputBase.RandomVector2 = new Vector2(x, y);
    }
    public override void ProcessEnd()
    {
        _InputBase.RandomVector2 = Vector2.one;
    }
  
}
