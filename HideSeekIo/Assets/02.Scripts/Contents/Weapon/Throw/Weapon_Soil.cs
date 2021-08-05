﻿using System.Collections;

using UnityEngine;

public class Weapon_Soil : Weapon_Throw
{
    public override System.Enum GetEnum() => Define.ThrowItem.Dynamite;

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
