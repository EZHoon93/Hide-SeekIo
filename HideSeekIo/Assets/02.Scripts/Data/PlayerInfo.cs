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
    public static Action<OptionData> chnageOptionInfoEvent;
    public static Define.UserDataState State { get;  set; }  //로그인 여부 
    public static Define.UserDataState optionState { get; set; }//옵션데이터여부

    public static ServerKey CurrentSkin => null;
    public static string nickName => userData.nickName;
    public static int level => userData.level;
    public static int coin => userData.coin;
    public static int exp => userData.exp;
    public static int maxExp => userData.maxExp;
    public static int gem => userData.gem;


    public static AvaterSlotInfo currentAvater => userData.GetCurrentAvater();

    public static AvaterSlotInfo[] hasAvaterList => userData.avaterList.ToArray();

    public static void Login()
    {
        State = Define.UserDataState.Wait;
        if (UserDataSystem.DoseSaveGameExist(jsonDataName))
        {
            //아이디가 존재한다면
            userData = UserDataSystem.LoadData<UserData>(jsonDataName);
            //Debug.Log("로그인... 존재합니다" + userData);
            State = Define.UserDataState.Load;
        }
        else
        {
            //아이디가 존재하지않는다면
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

        Managers.Sound.ChangeSfxValue(optionData.soundValue);
        Managers.Sound.ChangeBgmValue(optionData.bgmValue);
        optionState = Define.UserDataState.Load;
        chnageOptionInfoEvent?.Invoke(optionData);
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

   


 
   
}
