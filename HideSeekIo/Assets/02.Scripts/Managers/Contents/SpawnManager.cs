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


    public void PlayerSpawn(Define.Team team, Vector3 pos, bool isAI)
    {
        //var selectCharacterType = (Define.CharacterType)Util.RandomEnum<Define.CharacterType>();
        Define.CharacterType characterType;
        string avaterId = null;
        if (isAI)
        {
            characterType = Util.RandomEnum<Define.CharacterType>();    //랜덤으로 가져옴
        }
        else
        {
            characterType = PlayerInfo.GetCurrentUsingCharacter();
            avaterId = PlayerInfo.GetCurrentUsingCharacterAvaterSkin(characterType).avaterKey;
        }
        List<object> datas = new List<object>() { PhotonNetwork.LocalPlayer.NickName, avaterId , characterType , isAI };

        PlayerController createPlayer = null;
        switch (team)
        {
            case Define.Team.Hide:
                createPlayer= PhotonNetwork.Instantiate("Player", pos, Quaternion.identity, 0, datas.ToArray()).GetComponent<PlayerController>();
                break;
            case Define.Team.Seek:
                //datas.Add("Bear");
                createPlayer = PhotonNetwork.Instantiate("Player", pos, Quaternion.identity, 0, datas.ToArray()).GetComponent<PlayerController>();
                break;
        }

        if(isAI == false)
        {
            Managers.Game.myPlayer = createPlayer;
        }
    }
    public void AISpawn(Define.Team team, Vector3 pos)
    {
        //var selectCharacterType = (Define.CharacterType)Util.RandomEnum<Define.CharacterType>();

        //List<object> datas = new List<object>() { PhotonNetwork.LocalPlayer.NickName, PlayerInfo.CurrentSkin.avaterSeverKey , selectCharacterType };

        //switch (team)
        //{
        //    case Define.Team.Seek:
        //        PhotonNetwork.InstantiateRoomObject("UserSeeker", pos, Quaternion.identity, 0, datas.ToArray());
        //        break;
        //    case Define.Team.Hide:
        //        datas.Add(PlayerInfo.CurrentSkin.weaponSeverKey);
        //        PhotonNetwork.InstantiateRoomObject("UserHider", pos, Quaternion.identity, 0, datas.ToArray());
        //        break;
        //}

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

                datas.Add($"Gun/{weapon.ToString()}");
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
