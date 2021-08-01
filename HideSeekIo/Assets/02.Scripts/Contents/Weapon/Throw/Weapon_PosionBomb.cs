using System.Collections;

using UnityEngine;
using Photon.Pun;

public class Weapon_PosionBomb : Weapon_Throw
{

    protected override void Awake()
    {
        base.Awake();
        type = Type.Disposable;
    }

    private void Start()
    {
        Setup("Throw", .2f, .2f, 7.0f, 2.0f);
    }


}
