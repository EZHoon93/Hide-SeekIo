using FoW;

public class Buff_SightUp : BuffBase
{
    FogOfWarUnit _fogOfWarUnit;

    float resetValue;
    public override void ProcessStart()
    {
        _fogOfWarUnit = _livingEntity.GetComponentInChildren<FogOfWarUnit>();
        resetValue = _fogOfWarUnit.circleRadius;
        _fogOfWarUnit.circleRadius = 5;
    }

   
    public override void ProcessEnd()
    {
        _fogOfWarUnit = _livingEntity.GetComponentInChildren<FogOfWarUnit>();
        _fogOfWarUnit.circleRadius = resetValue;
    }
}
