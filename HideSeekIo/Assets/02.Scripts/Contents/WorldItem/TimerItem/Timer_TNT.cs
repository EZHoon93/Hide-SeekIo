
using FoW;
using Photon.Pun;
public class Timer_TNT : TimerItem, IPunInstantiateMagicCallback
{
    int _viewID;

    FogOfWarUnit _fogOfWarUnit;

    private void Awake()
    {
        _fogOfWarUnit = GetComponent<FogOfWarUnit>();
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null) return;
        //_viewID = (int)info.photonView.InstantiationData[1];


        //_fogOfWarUnit.team = _viewID;
    }

    public override void EndTime()
    {
        
    }

    public override void Setup(int useViewID)
    {
        _fogOfWarUnit.team = _viewID;
    }
}
