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

    public void PlayerSpawn(Define.Team team, Vector3 pos)
    {
        List<object> datas = new List<object>() { PhotonNetwork.LocalPlayer.NickName, PlayerInfo.CurrentAvater };

        switch (team)
        {
            case Define.Team.Hide:
                GameManager.Instance.myPlayer = PhotonNetwork.Instantiate("UserHider", pos, Quaternion.identity, 0, datas.ToArray()).GetComponent<PlayerController>();
                break;
            case Define.Team.Seek:
                datas.Add(PlayerInfo.CurrentWeapon);
                GameManager.Instance.myPlayer = PhotonNetwork.Instantiate("UserSeeker", pos, Quaternion.identity, 0, datas.ToArray()).GetComponent<PlayerController>();
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
    public void WeaponSpawn(Define.Weapon weapon , AttackBase newAttacker)
    {
        List<object> datas = new List<object>() { newAttacker.photonView.ViewID };
        switch (weapon)
        {
            case Define.Weapon.Melee2:
                datas.Add(PlayerInfo.CurrentWeapon);
                break;
        }
        PhotonNetwork.Instantiate(weapon.ToString(), new Vector3(0, -10, 0), Quaternion.identity, 0, datas.ToArray());
    }
}
