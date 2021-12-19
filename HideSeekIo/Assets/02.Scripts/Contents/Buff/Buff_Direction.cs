using System.Collections;

using FoW;

using UnityEngine;


public class Buff_Direction : BuffBase
{
    PlayerInput _playerInput;
    public override void ProcessStart()
    {
        _playerInput = _livingEntity.GetComponent<PlayerInput>();
        if (_playerInput == null) return;
        int isOppsive = Random.Range(0, 3);
        float x = 1;
        float y = 1;
        switch (isOppsive)
        {
            case 0:
                x = 1;
                y = -1;
                break;
            case 1:
                x = -1;
                y = 1;
                break;
            case 2:
                x = -1;
                y = -1;
                break;
        }
        _playerInput.RandomVector2 = new Vector2(x, y);
    }
    public override void ProcessEnd()
    {
        _playerInput.RandomVector2 = Vector2.one;
    }
  
}
