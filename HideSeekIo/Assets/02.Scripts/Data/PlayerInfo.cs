using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using System;
using Data;
using System.Linq;
using Photon.Pun;

public static class PlayerInfo 
{
    private static readonly string jsonDataName = "userData.json";
    private static readonly string optionDataName = "optionData.json";

    public static UserData userData;
    public static OptionData optionData;
    public static Action chnageInfoEvent;
    public static Action chnageOptionInfoEvent;

    public static Define.UserDataState State { get;  set; }  //로그인 여부 

    public static ServerKey CurrentSkin => null;
    //public static ServerKey CurrentWeapon => userData.weaponList.Find(s => s.isUsing == true);
    public static string nickName => userData.nickName;
    public static int level => userData.level;
    public static int coin => userData.coin;
    public static int exp => userData.exp;
    public static int maxExp => userData.maxExp;

    public static void Login()
    {
        State = Define.UserDataState.Wait;
        if (UserDataSystem.DoseSaveGameExist(jsonDataName))
        {
            //아이디가 존재한다면
            userData = UserDataSystem.LoadData<UserData>(jsonDataName);
            //Debug.Log("로그인... 존재합니다" + userData);
            State = Define.UserDataState.Load;
            //Debug.Log("test");

        }
        else
        {
            //아이디가 존재하지않는다면
            //Debug.Log("로그인... 존재하지 않습니다..." + userData);
            State = Define.UserDataState.Null;
        }

        
    }

    public static void LoadOptionData()
    {
        if (UserDataSystem.DoseSaveGameExist(optionDataName))
        {
            optionData = UserDataSystem.LoadData<OptionData>(optionDataName);
        }
        else
        {
            optionData = new OptionData();
            SaveOptionData();
        }

        
    }
    public static void CreateFirstID(string nickName)
    {
        userData = new UserData("test", nickName);//초기 데이터 생성
        SaveUserData();
        Login();
    }

   
    public static bool SaveUserData()
    {
        try
        {
            UserDataSystem.SaveData(userData, jsonDataName);
            chnageInfoEvent?.Invoke();
            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    public static bool SaveOptionData()
    {
        try
        {
            UserDataSystem.SaveData(optionData, optionDataName);
            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    public static void ChangeUserData()
    {
        //chnageInfoEvent();
    }

    public static int CurrentAvaterUsingIndex()
    {
        //return  userData.skinList.FindIndex(s => s.isUsing == true);
        return -1;
    }


 
   
    public static CharacterUserHasData GetCharacterData(Define.CharacterType characterType)
    {
        foreach(var ch in userData.characterList)
        {
            if(ch.characterType == characterType)
            {
                return ch;
            }
        }
        return null;
    }

    public static bool CheckUserHasCharacterAvarer(Define.CharacterType characterType , string avaterSkinKey)
    {
        foreach (var ch in userData.characterList)
        {
            if (ch.characterType == characterType)
            {
                return ch.characterSkinList.Any(s => s.avaterKey == avaterSkinKey);
            }
        }
        return false;
    }

    public static bool CheckUserHasWeaponSkin(string checkWeaponKey)
    {
        foreach (var weapon in userData.weaponList)
        {
            if(string.Equals(weapon , checkWeaponKey))
            {
                return true;
            }
        }
        return false;
    }

    public static SkinHasData GetCurrentUsingCharacterAvaterSkin(Define.CharacterType characterType)
    {
        if (userData.characterList.Count == 0)
        {
            return null;
        }

        List<SkinHasData> skinHasDatas = null;
        SkinHasData result = null;
        foreach (var ch in userData.characterList)
        {
            if (ch.characterType == characterType)
            {
                skinHasDatas = ch.characterSkinList;
                result = skinHasDatas.Find(s => s.isUsing == true);
            }
        }
        if (result != null)
        {
           return result;
        }

        return skinHasDatas[0] ?? null;
    }

    public static Define.CharacterType GetCurrentUsingCharacter()
    {
        return Define.CharacterType.Cat;
    }

    //public static SendAllSkinInfo MakeAllSkinInfo()
    //{
    //    SendAllSkinInfo sendAllSkinInfo;
    //    var characterType = GetCurrentUsingCharacter();
    //    sendAllSkinInfo.autoNumber = PhotonNetwork.LocalPlayer.ActorNumber;
    //    sendAllSkinInfo.chacterType = characterType;
    //    sendAllSkinInfo.avaterSkinID = GetCurrentUsingCharacterAvaterSkin(characterType).avaterKey;
    //    return sendAllSkinInfo;
    //}
    //public static string ChangeCurrentSkin(int useSelectIndex)
    //{
    //    var currentCh = GetCurrentUsingCharacter();
    //    return GetCurrentUsingCharacterAvaterSkin(currentCh);
    //}
}
