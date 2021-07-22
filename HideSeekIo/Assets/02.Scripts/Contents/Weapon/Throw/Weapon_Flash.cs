using System.Collections;

using UnityEngine;
using Photon.Pun;

public class Weapon_Flash : Weapon_Throw
{
    //private void Start()
    //{
    //    Setup("Throw", .2f, .3f, 5f , 4);
    //}

    protected override void Awake()
    {
        base.Awake();
        type = Type.Disposable;
    }


    private void Start()
    {
        Setup("Throw", .2f, .2f, 4.0f, 1);
    }


}
