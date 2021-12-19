using System.Collections;

using UnityEngine;

namespace Data
{
    public struct JoystickInfo
    {
        public Vector2 pos;
        public float size;
    }

    public class SendAllSkinInfo
    {
        public Define.Team team;
        public int autoNumber;
        public int avaterKey;
        //public string accessoriesSkinID;
        //public string weaponSkinID;
        public string nickName;

        
    }
    public struct SpawnData
    {
        public int spawnIndex;
        public Vector3 spawnPos;
    }
}