using System.Collections;

using UnityEngine;
using Photon.Pun;

public class Weapon_Flash : Weapon_Throw
{
    protected override void Awake()
    {
        base.Awake();
        Setup("Throw", .2f, .5f,7f  , 2);
        inputControllerObject.InitCoolTime = 5;
    }
    
}
