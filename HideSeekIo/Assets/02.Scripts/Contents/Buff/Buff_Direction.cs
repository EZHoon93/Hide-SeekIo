using System.Collections;

using FoW;

using UnityEngine;


public class Buff_Direction : BuffBase
{
    //InputBase _InputBase;

    public override void ProcessStart()
    {
        //_InputBase = _buffController.livingEntity.GetComponent<InputBase>();
        //if (_InputBase == null) return;
        //int isOppsive = Random.Range(0, 3);
        //float x = 1;
        //float y = 1;
        //switch (isOppsive)
        //{
        //    case 0:
        //        x = 1;
        //        y = -1;
        //        break;
        //    case 1:
        //        x = -1;
        //        y = 1;
        //        break;
        //    case 2:
        //        x = -1;
        //        y = -1;
        //        break;
        //} 
        //_InputBase.RandomVector2 = new Vector2(x, y);
    }
    public override void ProcessEnd()
    {
        //_InputBase.RandomVector2 = Vector2.one;
    }
  
}
