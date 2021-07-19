using System.Collections;

using UnityEngine;
using Photon.Pun;
using System.Linq.Expressions;
using System.Collections.Generic;

public class SpawnManager 
{
    public void WorldItemSpawn(Define.WorldItem photonObject ,Vector3 pos , int itemUseViewID =0  )
    {
        switch (photonObject)
        {
            case Define.WorldItem.Box:
            case Define.WorldItem.Trap:
                break;

        }
        PhotonNetwork.Instantiate(photonObject.ToString(), pos, Quaternion.identity, 0, new object[] { itemUseViewID }); //사용한 플레이어 ViewID
    }

   
    //public void InGameItemSpawn( System.Enum @enum , PlayerController buyPlayer)
    //{
    //    int sendEnumValue = -1;
    //    if( @enum.GetType() == typeof( Define.HiderStoreList))
    //    {
    //        sendEnumValue = 0;
    //    }
    //    else
    //    {
    //        sendEnumValue = 1;
    //    }


    //    PhotonNetwork.Instantiate("InGameItem", Vector3.up*50, Quaternion.identity, 0, new object[] { buyPlayer.ViewID() ,sendEnumValue, @enum }); //사용한 플레이어 ViewID
    //}



    public void PlayerSpawn(Define.Team team, Vector3 pos)
    {
        List<object> datas = new List<object>() { PhotonNetwork.LocalPlayer.NickName, PlayerInfo.CurrentAvater };

        switch (team)
        {
            case Define.Team.Hide:
                Managers.Game.myPlayer = PhotonNetwork.Instantiate("UserHider", pos, Quaternion.identity, 0, datas.ToArray()).GetComponent<PlayerController>();
                break;
            case Define.Team.Seek:
                datas.Add(PlayerInfo.CurrentWeapon);
                Managers.Game.myPlayer = PhotonNetwork.Instantiate("UserSeeker", pos, Quaternion.identity, 0, datas.ToArray()).GetComponent<PlayerController>();
                break;
        }
    }
    public void AISpawn(Define.Team team, Vector3 pos)
    {
        List<object> datas = new List<object>() { PhotonNetwork.LocalPlayer.NickName, PlayerInfo.CurrentAvater };

        switch (team)
        {
            case Define.Team.Seek:
                PhotonNetwork.InstantiateRoomObject("AISeeker", pos, Quaternion.identity, 0, datas.ToArray());
                break;
            case Define.Team.Hide:
                datas.Add(PlayerInfo.CurrentWeapon);
                PhotonNetwork.InstantiateRoomObject("AIHider", pos, Quaternion.identity, 0, datas.ToArray());
                break;
        }

    }
    public GameObject WeaponSpawn(Define.Weapon weapon , AttackBase newAttacker)
    {
        List<object> datas = new List<object>() { newAttacker.photonView.ViewID };
        string weaponID = null;
        switch (weapon)
        {
            case Define.Weapon.Melee2:
                weaponID = "Melee2";
                datas.Add(PlayerInfo.CurrentWeapon);
                break;
            case Define.Weapon.Sniper:

                datas.Add($"Gun/{weapon.ToString()}");
                break;
            default:
                weaponID = $"Throw/{weapon.ToString()}";
                break;
        }
        Debug.LogError("생성 " + weaponID);
        return PhotonNetwork.Instantiate( weaponID , new Vector3(0, -10, 0), Quaternion.identity, 0, datas.ToArray());
    }

    //public void TimerItemSpawn(Define.TimerItem timerItem , int useViewID)
    //{

    //    //PhotonNetwork.InstantiateRoomObject("TimerItem/TNT", this.transform.position, Quaternion.identity)

    //}

    //public void MeleeWeaponSpawn(int viewID)
    //{
    //    PhotonNetwork.Instantiate("Melee2", Vector3.zero, Quaternion.identity);
    //}

    //public void ThrowWeaponSpawn(int viewID)
    //{

    //}
}
