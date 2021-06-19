using System.Collections;

using UnityEngine;
using Photon.Pun;

public class SpawnManager 
{
    public void PhotonSpawn(Define.PhotonObject photonObject ,Vector3 pos , int itemUseViewID =0  )
    {

        switch (photonObject)
        {
            case Define.PhotonObject.UserHider:
            case Define.PhotonObject.UserSeeker:
                object[] datas = { PhotonNetwork.LocalPlayer.NickName, PlayerInfo.CurrentAvater};
                GameManager.Instance.myPlayer = PhotonNetwork.Instantiate(photonObject.ToString(), pos, Quaternion.identity,0,datas).GetComponent<PlayerController>();

                break;
            case Define.PhotonObject.AIHider:
            case Define.PhotonObject.AISeekr:
                //방장이아니면 생성X
                if (PhotonNetwork.IsMasterClient == false)
                {
                    return;
                }
                object[] aiData = { PhotonNetwork.LocalPlayer.NickName, PlayerInfo.CurrentAvater };
                var ai = PhotonNetwork.InstantiateRoomObject(photonObject.ToString(), pos, Quaternion.identity,0,aiData);

                break;
            case Define.PhotonObject.Box:
            case Define.PhotonObject.Trap:
                Debug.Log("생성..."+photonObject.ToString());
                PhotonNetwork.Instantiate(photonObject.ToString(), pos, Quaternion.identity , 0, new object[] { itemUseViewID }); //사용한 플레이어 ViewID
                Debug.Log("생성..2." + photonObject.ToString());

                break;

        }


    }

    public void LocalSpawn(Define.LocalWorldObject localWorldObject)
    {

    }
}
