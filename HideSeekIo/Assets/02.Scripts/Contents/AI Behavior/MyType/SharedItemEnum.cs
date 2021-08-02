
using BehaviorDesigner.Runtime;

public class SharedItemEnum : SharedVariable<Define.InGameItem>
{
    public static implicit operator SharedItemEnum(Define.InGameItem value) { return new SharedItemEnum { Value = value }; }

}
