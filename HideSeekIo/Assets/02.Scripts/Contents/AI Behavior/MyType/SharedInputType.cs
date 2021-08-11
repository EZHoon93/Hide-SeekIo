
using System;

using BehaviorDesigner.Runtime;

public class SharedInputType : SharedVariable<InputType>
{
    public static implicit operator SharedInputType(InputType value) { return new SharedInputType { Value = value }; }

}
