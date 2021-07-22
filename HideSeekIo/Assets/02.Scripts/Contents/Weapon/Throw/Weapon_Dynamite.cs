using System.Collections;

using UnityEngine;

public class Weapon_Dynamite : Weapon_Throw
{


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
