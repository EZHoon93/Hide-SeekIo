using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Data;
using ExitGames.Client.Photon;

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

    public GameObject AccessoriesSpawn(string prefabID)
    {
        if (string.IsNullOrEmpty(prefabID))
        {
            
            return null;
        }
        return Managers.Resource.Instantiate($"Accessories/{prefabID}");
    }
    public GameObject AvaterSpawn(string prefabID)
    {
        if (string.IsNullOrEmpty(prefabID))
        {
            return null;
        }
        return Managers.Resource.Instantiate($"Character/{prefabID}");
    }

    public GameObject GetSkinByIndex(Define.ProductType productType , int selectIndex, Transform parent = null)
    {
        var skinList = Managers.ProductSetting.GetSkins(productType);
        if (selectIndex < 0 || selectIndex > skinList.Length)
        {
            selectIndex = 0;
        }
        var prefab = skinList[selectIndex];
        var go = Managers.Pool.Pop(prefab, parent).gameObject;
        go.transform.ResetTransform();
        return go;
    }


    public void UserControllerSpawn()
    {
        List<object> datas = new List<object>()
        {
            PhotonNetwork.LocalPlayer.ActorNumber,
            PlayerInfo.userData.nickName,
            //PlayerInfo.userData.GetCurrentAvater().characterAvaterKey,
            //PlayerInfo.userData.GetCurrentAvater().accesoryKey,
            //PlayerInfo.userData.GetCurrentAvater().weaponKey,
        };
        PhotonNetwork.Instantiate("User", Vector3.down, Quaternion.identity, 0, datas.ToArray());
    }
    public void PlayerSpawn2(SendAllSkinInfo sendAllSkinInfo, Vector3 pos )
    {
        //List<object> datas = new List<object>() { sendAllSkinInfo.nickName,sendAllSkinInfo.autoNumber, sendAllSkinInfo.avaterSkinID  };
        List<object> datas = new List<object>()
        {
            sendAllSkinInfo.autoNumber,
            sendAllSkinInfo.team,
            sendAllSkinInfo.nickName,
            0,

            0,
            0

        };
        PhotonNetwork.InstantiateRoomObject("Player", pos, Quaternion.identity, 0, datas.ToArray()).GetComponent<PlayerController>();
    }


    public PlayerController PlayerSpawn( int playerNum, Dictionary<string,object> sendData , Vector3 pos)
    {
        //List<object> datas = new List<object>() { sendAllSkinInfo.nickName,sendAllSkinInfo.autoNumber, sendAllSkinInfo.avaterSkinID  };
        return PhotonNetwork.InstantiateRoomObject("Player", pos, Quaternion.identity, 0, new object[] { playerNum, sendData}).GetComponent<PlayerController>();
    }
    public void MyPlayerSpawn(Define.Team team,  Vector3 pos)
    {
        List<object> datas = new List<object>()
        {
            PhotonNetwork.LocalPlayer.ActorNumber,
            team,
            PlayerInfo.nickName,
            PlayerInfo.currentAvater.characterAvaterKey,
            PlayerInfo.currentAvater.weaponKey,
            PlayerInfo.currentAvater.accesoryKey,
        };

        PhotonNetwork.InstantiateRoomObject("Player", pos, Quaternion.identity, 0, datas.ToArray()).GetComponent<PlayerController>();
    }
    public GameObject WeaponSpawn(Define.Weapon weapon , PlayerController playerController)
    {
        List<object> datas = new List<object>() { playerController.photonView.ViewID }; 
        string weaponID = null;
        switch (weapon)
        {
            case Define.Weapon.Hammer:
                weaponID = "Hammer";
                datas.Add(0);
                break;
            //case Define.Weapon.Sniper:
            //case Define.Weapon.Stone:
            //    weaponID = $"Gun/{weapon.ToString()}";
            //    break;
            case Define.Weapon.SleepGun:
            case Define.Weapon.SlingShot:
                weaponID = $"Weapon/Straight/{weapon.ToString()}";
                break;
            default:
                //weaponID = $"ThrowItem/{weapon.ToString()}";
                weaponID = $"Weapon/ThrowItem/{weapon.ToString()}";
                break;
        }
        return PhotonNetwork.InstantiateRoomObject(weaponID, new Vector3(0, -10, 0), Quaternion.identity, 0, datas.ToArray());
    }


    public GameObject GameStateSpawn(Define.GameState gameState ,object addData =null)
    {
        //List<object> datas = new List<object>() { gameState , addData };
        object[] datas = new object[] { gameState, addData };
        var go = PhotonNetwork.InstantiateRoomObject($"GameState", Vector3.down, Quaternion.identity,0,datas);

        return go;
    }
}
