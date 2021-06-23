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
        PhotonManager.instacne.enterUserList += UpdatePlayer;
        PhotonManager.instacne.leftUserList += LeftrPlayer;

    }

    void LoadCurrentPlayer()
    {
        foreach(var player in PhotonNetwork.CurrentRoom.Players)
        {
            UpdatePlayer(player.Value);
        }
    }

  
    public void UpdatePlayer(Player newPlayer)
    {
        GameObject go = null;
        bool isExist = _playerDic.TryGetValue(newPlayer.ActorNumber, out go);
        if(isExist == false)
        {
            go = Instantiate(userInfoPrefab);
            go.transform.ResetTransform(_content);
            _playerDic.Add(newPlayer.ActorNumber, go.gameObject);
        }
        print("추가");
        var userLevel = newPlayer.CustomProperties["lv"];
        go.GetComponent<TMPro.TextMeshProUGUI>().text = $"{newPlayer.NickName} (LV.{userLevel})";
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
