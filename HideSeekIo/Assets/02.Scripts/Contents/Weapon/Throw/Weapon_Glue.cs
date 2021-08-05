using System.Collections;

using UnityEngine;

public class Weapon_Glue : Weapon_Throw
{
    public override System.Enum GetEnum() => Define.ThrowItem.Glue;

    protected override void Awake()
    {
        base.Awake();
        type = Type.Disposable;
        throwType = Define.ThrowItem.Glue;

    }

    private void Start()
    {
        Setup("Throw", .2f, .2f, 7.0f, 1);
    }

}
