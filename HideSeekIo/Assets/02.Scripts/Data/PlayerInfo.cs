using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using System;
using Data;
public static class PlayerInfo 
{
    private static readonly string jsonDataName = "userData.json";
    private static readonly string optionDataName = "optionData.json";

    public static UserData userData;
    public static OptionData optionData;
    public static Action chnageInfoEvent;
    public static Action chnageOptionInfoEvent;

    public static Define.UserDataState State { get;  set; }  //로그인 여부 

    public static ServerKey CurrentSkin=> userData.skinList.Find(s => s.isUsing == true);
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
            Debug.Log("로그인... 존재합니다" + userData);
            State = Define.UserDataState.Load;

            Debug.Log("test");

        }
        else
        {
            //아이디가 존재하지않는다면
            Debug.Log("로그인... 존재하지 않습니다..." + userData);
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
        userData = new UserData();//초기 데이터 생성
        userData.nickName = nickName;
        userData.level = 1;
        userData.coin = 1;
        userData.exp = 0;
        userData.maxExp = 10;
        userData.key = "222";
        userData.skinList = new List<ServerKey>()
        {
            new ServerKey("Ch01","Wm01", true)
        };

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
        return  userData.skinList.FindIndex(s => s.isUsing == true);
    }


    public static void ChangeCurrentSkin(int useSelectIndex)
    {
        foreach(var skin in userData.skinList)
        {
            skin.isUsing = false;
        }

        userData.skinList[useSelectIndex].isUsing = true;
        SaveUserData();
    }
   

    

}
