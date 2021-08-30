using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Data;

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
        string prefabID = $"Melee2/{meleeKey}";
        return Managers.Resource.Instantiate(prefabID);
    }


    public void PlayerSpawn(SendAllSkinInfo sendAllSkinInfo, Vector3 pos, bool isAI)
    {
        //var selectCharacterType = (Define.CharacterType)Util.RandomEnum<Define.CharacterType>();
        Define.CharacterType characterType;
        string avaterId = null;
        //SendAllSkinInfo sendAllSkinInfo;
        //if (isAI)
        //{
        //    characterType = Util.RandomEnum<Define.CharacterType>();    //랜덤으로 가져옴
        //}
        //else
        //{
        //    sendAllSkinInfo = PlayerInfo.MakeAllSkinInfo();
        //    //characterType = PlayerInfo.GetCurrentUsingCharacter();
        //    //avaterId = PlayerInfo.GetCurrentUsingCharacterAvaterSkin(characterType).avaterKey;
        //}

        List<object> datas = new List<object>() { PhotonNetwork.LocalPlayer.NickName,sendAllSkinInfo.autoNumber, sendAllSkinInfo.chacterType,sendAllSkinInfo.avaterSkinID  , isAI };
        PhotonNetwork.InstantiateRoomObject("Player", pos, Quaternion.identity, 0, datas.ToArray()).GetComponent<PlayerController>();
    }

    public GameObject ItemSpawn(System.Enum inGameItem, PlayerController playerController)
    {
        List<object> datas = new List<object>() { playerController.photonView.ViewID, false };
        string prefabID = $"{inGameItem.GetType().Name}/{inGameItem.ToString()}";
        //string prefabID = $"ThrowItem/{inGameItem.ToString()}";

        if (playerController.gameObject.IsValidAI())
        {
            return PhotonNetwork.InstantiateRoomObject(prefabID, new Vector3(0, -10, 0), Quaternion.identity, 0, datas.ToArray());
        }
        else
        {
            Debug.LogError("생성 " + prefabID);

            return PhotonNetwork.Instantiate(prefabID, new Vector3(0, -10, 0), Quaternion.identity, 0, datas.ToArray());
        }
    }
    public GameObject WeaponSpawn(Define.Weapon weapon , PlayerShooter playerShooter)
    {
        List<object> datas = new List<object>() { playerShooter.photonView.ViewID };
        string weaponID = null;
        switch (weapon)
        {
            case Define.Weapon.Melee2:
                weaponID = "Melee2";
                datas.Add("Wm01");
                break;
            case Define.Weapon.Sniper:
            case Define.Weapon.Gun:
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

   
}
