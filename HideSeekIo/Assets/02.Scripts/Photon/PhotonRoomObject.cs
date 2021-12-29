using System.Linq;
using Photon.Pun;


public abstract class PhotonRoomObject : MonoBehaviourPun , IPunInstantiateMagicCallback
{

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var infoData = info.photonView.InstantiationData;
        if (infoData == null)
        {
            return;
        }
        var actorNumber = (int)infoData[0];
        if (PhotonNetwork.PlayerList.Any(s => s.ActorNumber == actorNumber))
        {
            this.photonView.ControllerActorNr = actorNumber;
        }
    }

    protected abstract void Init(int actorNumber, object[] infoData);
}
