using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class UI_ServerInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _serverInfoText;


    private void OnEnable()
    {
        string content = $"{PhotonNetwork.CloudRegion } / " +
             $"{PhotonNetwork.CurrentRoom.Name.Substring(1)} 채널 ";
        _serverInfoText.text = content;
    }
    

    //IEnumerator UpdateServerInfo()
    //{
    //    while (true)
    //    {
    //        string content = $"Ping : {PhotonNetwork.GetPing()}ms " +
    //            $"{PhotonNetwork.CloudRegion } / " +
    //            $"{PhotonNetwork.CurrentRoom.Name.Substring(1)} 채널 ";
    //        _serverInfoText.text = content;
    //        yield return new WaitForSeconds(1.0f);
    //    }
    //}


}
