using System.Collections;
using UnityEngine;

public class LobbyScene : BaseScene
{
    public override void Clear()
    {

    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(3.0f);
        PhotonManager.instacne.JoinRoom();
    }
}
