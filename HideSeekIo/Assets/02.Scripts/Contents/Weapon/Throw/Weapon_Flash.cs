using System.Collections;

using UnityEngine;
using Photon.Pun;

public class Weapon_Flash : Weapon_Throw
{
    //private void Start()
    //{
    //    Setup("Throw", .2f, .3f, 5f , 4);
    //}
    public override System.Enum GetEnum() => Define.ThrowItem.Flash;

    protected override void Awake()
    {
        base.Awake();
        type = Type.Disposable;
        throwType = Define.ThrowItem.Flash;
        inputType = InputType.Sub;

    }


    private void Start()
    {
        inputType = InputType.Sub;
        Setup("Throw", .2f, .2f, 7.0f, 2);
        InitCoolTime = 5;
    }


}
