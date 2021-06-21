using System.Collections;

using UnityEngine;

public class Buff_Shoes : BuffBase
{
    MakeRunEffect _makeRunEffect;

    public override void ProcessStart()
    {
        _makeRunEffect = _buffController.livingEntity.GetComponentInChildren<MakeRunEffect>();

        if (_makeRunEffect)
        {
            _makeRunEffect.gameObject.SetActive(false);

        }

    }
    public override void ProcessEnd()
    {
        base.ProcessEnd();
    }


}
