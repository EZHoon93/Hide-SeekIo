using System.Collections;

using UnityEngine;

public class Buff_Revive : BuffBase
{
    //HiderMove _hiderMove;
    MakeRunEffect _makeRunEffect;

    public override void ProcessStart()
    {
        //_hiderMove =  _buffController.livingEntity.GetComponent<HiderMove>();
        ////_makeRunEffect = _buffController.livingEntity.GetComponentInChildren<MakeRunEffect>();

        //if (_hiderMove == null ) return;

        ////_hiderMove.AddMoveBuffList(0.5f, true);
        ////_makeRunEffect.enabled = false;

    }

    private void Update()
    {
        //if(_hiderMove)
        //{
        //    _hiderMove.CurrentEnergy = _hiderMove.MaxEnergy;    //에너지 떨어지지않음
        //}
    }

    public override void ProcessEnd()
    {
      //  _hiderMove.AddMoveBuffList(0.5f, false);
        //_makeRunEffect.enabled = true;
    }
}
