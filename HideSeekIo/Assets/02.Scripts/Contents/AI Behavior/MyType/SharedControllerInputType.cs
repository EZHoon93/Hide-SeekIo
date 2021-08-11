
using System;

using BehaviorDesigner.Runtime;

public class SharedControllerInputType : SharedVariable<ControllerInputType>
{
    public static implicit operator SharedControllerInputType(ControllerInputType value) { return new SharedControllerInputType { Value = value }; }

}
