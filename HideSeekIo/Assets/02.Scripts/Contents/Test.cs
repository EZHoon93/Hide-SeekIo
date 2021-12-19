using System.Collections;

using Photon.Pun;

using UnityEngine;

public class Test : MonoBehaviourPunCallbacks
{
    readonly string _gameVersion = "3.0.0";

    [SerializeField] GameObject stone;
    // Use this for initialization
    [SerializeField] Transform test;
    [SerializeField] Transform end;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = _gameVersion;
        PhotonNetwork.ConnectUsingSettings();
        print("dd");

    }

    public override void OnConnectedToMaster()
    {
        print("dd2");

        base.OnConnectedToMaster();
    }
    public override void OnConnected()
    {
        print("dd3");
    }
    //void Start()
    //{
    //    Managers.Spawn.WorldItemSpawn(Define.WorldItem.Box, Vector3.zero);
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.A))
    //    {
    //        print("발사");
    //        var t =  Managers.Pool.Pop(stone).GetComponent<ThrowProjectileObject>();
    //        t.transform.position = test.position;
    //    }
    //}
}
