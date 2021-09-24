using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSettingInfo : SerializableDictionary<Define.MissionType, MissionInfo> { }

[Serializable]
public class MissionInfo 
{
    public Define.MissionType missionType;
    [SerializeField] Sprite missionSprite;
    [SerializeField] string titleScirbe;

}
