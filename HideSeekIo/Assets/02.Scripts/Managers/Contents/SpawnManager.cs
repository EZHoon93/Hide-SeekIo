using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Data;

public class SpawnManager 
{
    //public void WorldItemSpawn(Define.WorldItem photonObject ,Vector3 pos , int itemUseViewID =0  )
    //{
    //    switch (photonObject)
    //    {
    //        case Define.WorldItem.Box:
    //        case Define.WorldItem.Trap:
    //            break;

    //    }
    //    PhotonNetwork.Instantiate(photonObject.ToString(), pos, Quaternion.identity, 0, new object[] { itemUseViewID }); //사용한 플레이어 ViewID
    //} 

    public GameObject InGameItemSpawn(Define.InGameItem inGameItem, PlayerController playerController)
    {
        object[] datas = new object[] { playerController.ViewID()};
        //즉시사용
        var go= PhotonNetwork.Instantiate($"InGameItem/{inGameItem.ToString()}", new Vector3(0, -10, 0), Quaternion.identity ,0, datas);

        return go;
    }

    public GameObject CharacterSpawn(Define.CharacterType characterType)
    {
        //string prefabID = $"Character/{characterType.ToString()}";
        string prefabID = $"Character/{Define.CharacterType.Cat}";
        //List<object> datas = new List<object>() {PlayerInfo.CurrentSkin.avaterSeverKey };
        return Managers.Resource.Instantiate(prefabID); 
            
    }
    public CharacterAvater CharacterAvaterSpawn(Define.CharacterType characterType, string key)
    {
        string prefabID = $"Character/{characterType.ToString()}/{key}";        
        var characterAvaterGo = Managers.Resource.Instantiate(prefabID).GetComponent<CharacterAvater>();
        return characterAvaterGo ?? null;
    }
    public GameObject WeaponSkinSpawn(string meleeKey)
    {
        string prefabID = $"Hammer/{meleeKey}";
        return Managers.Resource.Instantiate(prefabID);
    }


    public void PlayerSpawn(SendAllSkinInfo sendAllSkinInfo, Vector3 pos )
    {
        List<object> datas = new List<object>() { sendAllSkinInfo.nickName,sendAllSkinInfo.autoNumber, sendAllSkinInfo.chacterType,sendAllSkinInfo.avaterSkinID  };
        PhotonNetwork.InstantiateRoomObject("Player", pos, Quaternion.identity, 0, datas.ToArray()).GetComponent<PlayerController>();
    }

    public GameObject WeaponSpawn(Define.Weapon weapon , PlayerShooter playerShooter)
    {
        List<object> datas = new List<object>() { playerShooter.photonView.ViewID }; 
        string weaponID = null;
        switch (weapon)
        {
            case Define.Weapon.Hammer:
                weaponID = "Hammer";
                datas.Add("Wm01");
                break;
            case Define.Weapon.Sniper:
            case Define.Weapon.Stone:
                weaponID = $"Gun/{weapon.ToString()}";
                break;
            default:
                weaponID = $"ThrowItem/{weapon.ToString()}";
                break;
        }

        if (playerShooter.gameObject.IsValidAI())
        {
            return PhotonNetwork.InstantiateRoomObject(weaponID, new Vector3(0, -10, 0), Quaternion.identity, 0, datas.ToArray());
        }
        else
        {
            return PhotonNetwork.Instantiate(weaponID, new Vector3(0, -10, 0), Quaternion.identity, 0, datas.ToArray());
        }

    }


    public GameObject GameStateSpawn(Define.GameState gameState ,object addData =null)
    {
        //List<object> datas = new List<object>() { gameState , addData };
        object[] datas = new object[] { gameState, addData };
        var go = PhotonNetwork.InstantiateRoomObject($"GameState", Vector3.down, Quaternion.identity,0,datas);

        return go;
    }
}
