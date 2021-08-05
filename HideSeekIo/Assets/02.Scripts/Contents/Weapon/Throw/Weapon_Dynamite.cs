using System.Collections;

using UnityEngine;

public class Weapon_Dynamite : Weapon_Throw
{
    public override System.Enum GetEnum() => Define.ThrowItem.Dynamite;

    protected override void Awake()
    {
        base.Awake();
        type = Type.Disposable;
        throwType = Define.ThrowItem.Dynamite;

    }

    private void Start()
    {
        Setup("Throw", .2f, .2f, 7.0f, 1.5f);
    }
}
