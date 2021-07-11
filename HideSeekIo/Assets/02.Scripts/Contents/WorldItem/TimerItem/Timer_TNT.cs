
using FoW;
using Photon.Pun;
public class Timer_TNT : TimerItem
{
    int _viewID;

    FogOfWarUnit _fogOfWarUnit;

    private void Awake()
    {
        _fogOfWarUnit = GetComponent<FogOfWarUnit>();
    }

    public override void EndTime()
    {
        
    }

    public override void Setup(int useViewID)
    {
        print("팀!!"+useViewID);
        _viewID = useViewID;
        _fogOfWarUnit.team = useViewID;
    }
}
