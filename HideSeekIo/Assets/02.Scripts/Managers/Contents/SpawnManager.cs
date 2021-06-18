using System.Collections;

using UnityEngine;
using Photon.Pun;

public class SpawnManager 
{
    public void PhotonSpawn(Define.PhotonObject photonObject,Vector3 pos )
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

        }


    }

    public void LocalSpawn(Define.LocalWorldObject localWorldObject)
    {

    }
}
