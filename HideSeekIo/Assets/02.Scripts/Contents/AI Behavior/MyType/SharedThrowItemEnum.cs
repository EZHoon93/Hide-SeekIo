
using System;

using BehaviorDesigner.Runtime;

public class SharedThrowItemEnum : SharedVariable<Define.ThrowItem>
{
    public static implicit operator SharedThrowItemEnum(Define.ThrowItem value) { return new SharedThrowItemEnum { Value = value }; }

}
