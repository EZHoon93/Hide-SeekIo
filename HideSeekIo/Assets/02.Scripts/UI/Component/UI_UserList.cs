using System.Collections;
using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime;

using UnityEngine;
using UnityEngine.UI;

public class UI_UserList : MonoBehaviour
{
    [SerializeField] Transform _content;
    Dictionary<int, GameObject> _playerDic = new Dictionary<int, GameObject>();

    public GameObject userInfoPrefab;

    void Start()
    {
        LoadCurrentPlayer();
    }

    void LoadCurrentPlayer()
    {
        foreach(var player in PhotonNetwork.CurrentRoom.Players)
        {
            EnterPlayer(player.Value);
        }
    }

  
    public void EnterPlayer(Player newPlayer)
    {
        if(_playerDic.ContainsKey(newPlayer.ActorNumber) == false)
        {
            var go = Instantiate(userInfoPrefab);
            go.transform.SetParent(this._content);
            go.transform.position = Vector3.zero;
            go.gameObject.SetActive(true);
            var userLevel =  newPlayer.CustomProperties["lv"];
            go.GetComponent<TMPro.TextMeshProUGUI>().text = $"{newPlayer.NickName} (LV.{userLevel})";
            _playerDic.Add(newPlayer.ActorNumber, go.gameObject);
        }
    }
    public void LeftrPlayer(Player otherPlayer)
    {
        if(_playerDic.ContainsKey(otherPlayer.ActorNumber))
        {
            Destroy( _playerDic[otherPlayer.ActorNumber].gameObject);
            _playerDic.Remove(otherPlayer.ActorNumber);
        }
    }
 
}
