
using System;

using BehaviorDesigner.Runtime;

public class SharedImmdiateItemEnum : SharedVariable<Define.InGameItem>
{
    public static implicit operator SharedImmdiateItemEnum(Define.InGameItem value) { return new SharedImmdiateItemEnum { Value = value }; }

}
