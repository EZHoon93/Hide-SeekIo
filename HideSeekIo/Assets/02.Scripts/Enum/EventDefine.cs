using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDefine 
{
    public enum EventType
    {
        InGame,
        Photon,
        ChangeScene,
        LoadData
    }
    
    public enum ChangeScene
    {
        Lobby,
        Main
    }

    public enum PhotonEvnet
    {
        NewMaster
    }

    public enum InGameEvent
    {
        ChangeSeeker,
        ChangeHider,
        ChangeInGameTime,
        ChangeReadyTime,
        GameJoin,
        MyPlayerActive,
        ChangeState,
    }

 
}
