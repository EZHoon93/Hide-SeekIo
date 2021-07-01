using System.Collections;


using UnityEngine;
using System;
using Data;
public static class PlayerInfo 
{
    private static readonly string jsonDataName = "userData.json";
    public static UserData userData;
    public static Action chnageInfoEvent;
    public static Define.UserDataState State { get;  set; }  //로그인 여부 

    public static string CurrentAvater => userData.HasAvaters[0];
    public static string CurrentWeapon => userData.HasWeapons[0];
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

        }
        else
        {
            //아이디가 존재하지않는다면
            Debug.Log("로그인... 존재하지 않습니다..." + userData);
            State = Define.UserDataState.Null;
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
        userData.HasAvaters = new string[] { "Ch01" };
        userData.HasWeapons= new string[] { "Wm01" };
        SaveUserData();
        Login();
    }

    //static IEnumerator WaitLogin()
    //{
    //    yield return new WaitForSeconds(1.5f);
    //    //nickName = "EZsss";
    //    //coin = 999;
    //    //level = 33;
    //    //CurrentAvater = "Ch01";
    //    //CurrentWeapon = "Wm01";
    //}

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

    public static void ChangeUserData()
    {
        //chnageInfoEvent();
    }
}
