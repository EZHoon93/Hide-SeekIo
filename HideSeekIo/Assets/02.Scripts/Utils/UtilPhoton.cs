
using System.Collections.Generic;
using System.Linq;

using Photon.Pun;
using Photon.Realtime;
public static class UtilPhoton 
{
    public static List<Player> GetJoinPlayerList()
    {
        return PhotonNetwork.CurrentRoom.Players.Values.Where(s => (bool)s.CustomProperties["jn"] == true).ToList();
    }

}
