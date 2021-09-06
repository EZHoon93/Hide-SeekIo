using System.Collections;

using UnityEngine;

public class Buff_Stun : BuffBase
{
    PlayerInput _playerInput;

    public override void ProcessStart()
    {
        _playerInput = _buffController.livingEntity.GetComponent<PlayerInput>();
        if (_playerInput)
        {
            _playerInput.Stop(5.0f);
        }
    }
    public override void ProcessEnd()
    {
        if (_playerInput)
        {
            _playerInput.RemoveStop();
        }
    }


}
