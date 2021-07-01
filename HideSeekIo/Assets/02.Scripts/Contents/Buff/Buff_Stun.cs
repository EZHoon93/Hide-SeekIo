using System.Collections;

using UnityEngine;

public class Buff_Stun : BuffBase
{
    InputBase _inputBase;

    public override void ProcessStart()
    {
        _inputBase = _buffController.livingEntity.GetComponent<InputBase>();
        _inputBase.Stop(2.0f);
    }
    public override void ProcessEnd()
    {
        _inputBase.RemoveStop();
    }


}
