using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class UI_ServerInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _serverInfoText;

    private void Start()
    {
        StartCoroutine(UpdateServerInfo());
    }

    IEnumerator UpdateServerInfo()
    {
        while (true)
        {
            print("업데이트 인포");
            string content = $"Ping : {PhotonNetwork.GetPing()}ms " +
                $"{PhotonNetwork.CloudRegion } / " +
                $"{PhotonNetwork.CurrentRoom.Name} 채널 ";
            _serverInfoText.text = content;
            yield return new WaitForSeconds(1.0f);
        }
    }


}
