using BehaviorDesigner.Runtime;

public class SharedGetItem : SharedVariable<GetWorldItemController>
{
    public static implicit operator SharedGetItem(GetWorldItemController value) { return new SharedGetItem { Value = value }; }

}
