﻿using System.Collections;

using BehaviorDesigner.Runtime.Tasks;

using UnityEngine;

public static class PlayerInfo 
{
    public static Action chnageInfoEvent;

    public static Define.UserDataState State { get; private set; }  //로그인 여부 

    public static string CurrentAvater { get; private set; }
    public static string nickName;
    public static int coin;

    public static void Login()
    {
        State = Define.UserDataState.Load;
        nickName = "EZs";
        coin = 999;
        CurrentAvater = "Ch01";
    }

    public static void ChangeUserData()
    {
        //chnageInfoEvent();
    }
}
