using System.Collections;


using UnityEngine;
using System;

public static class PlayerInfo 
{
    public static Action chnageInfoEvent;

    public static Define.UserDataState State { get; private set; }  //로그인 여부 

    public static string CurrentAvater { get; private set; }
    public static string CurrentWeapon { get; private set; }
    public static string nickName;
    public static int level { get; private set; }
    public static int coin { get; private set; }
    public static int exp;
    public static int maxExp;

    public static void Login()
    {
        State = Define.UserDataState.Load;
        nickName = "EZsss";
        coin = 999;
        level = 33;
        CurrentAvater = "Ch01";
        CurrentWeapon = "Wm01";
    }

    public static void ChangeUserData()
    {
        //chnageInfoEvent();
    }
}
