using FoW;

public class Buff_OverSee : BuffBase
{
    FogOfWarUnit _fogOfWarUnit;

    protected void Awake()
    {
        _fogOfWarUnit = GetComponent<FogOfWarUnit>();
        _fogOfWarUnit.enabled = false;
    }


    public override void ProcessEnd()
    {
        _fogOfWarUnit.enabled = false;
    }

    public override void ProcessStart()
    {
        _fogOfWarUnit.team = _buffController.livingEntity.ViewID();
        _fogOfWarUnit.enabled = true;
    }

  
}
