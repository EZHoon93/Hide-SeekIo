using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Glue : Weapon_Throw
{

    protected override void Awake()
    {
        base.Awake();
        Setup("Attack", .2f, .5f, 7f, 2);
        inputControllerObject.InitCoolTime = 0;
    }
}
