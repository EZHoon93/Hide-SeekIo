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
                object[] aiData = { PhotonNetwork.LocalPlayer.NickName, PlayerInfo.CurrentAvater };
                var ai = PhotonNetwork.InstantiateRoomObject(photonObject.ToString(), pos, Quaternion.identity,0,aiData);
                break;
            case Define.PhotonObject.Box:
            case Define.PhotonObject.Trap:
                PhotonNetwork.Instantiate(photonObject.ToString(), pos, Quaternion.identity , 0, new object[] { itemUseViewID }); //사용한 플레이어 ViewID
                break;

        }


    }

    public void LocalSpawn(Define.LocalWorldObject localWorldObject)
    {

    }

    public void WeaponSpawn(Define.Weapon weapon , AttackBase newAttacker)
    {
        Debug.Log(weapon.ToString() + "생성");
        string path = $"Weapon/{weapon.ToString()}";
        GameObject weaponObject = null;
        weaponObject = Managers.Resource.Instantiate(path);
        newAttacker.SetupWeapon(weaponObject.GetComponent<Weapon>());
        switch (weapon)
        {
            case Define.Weapon.Grenade:
                
                break;
        }
    }
}
