using System.Collections;
using UnityEngine;

public class LobbyScene : MonoBehaviour
{


    private IEnumerator Start()
    {
        yield return new WaitForSeconds(3.0f);
        PhotonManager.instacne.JoinRoom();
    }
}
