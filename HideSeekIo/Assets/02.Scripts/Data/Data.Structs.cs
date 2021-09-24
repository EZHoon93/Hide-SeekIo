using System.Collections;

using UnityEngine;

namespace Data
{
    public struct JoystickInfo
    {
        public Vector2 pos;
        public float size;
    }

    public struct SendAllSkinInfo
    {
        public int autoNumber;
        public Define.CharacterType chacterType;
        public string avaterSkinID;
        public string nickName;
    }
    public struct SpawnData
    {
        public int spawnIndex;
        public Vector3 spawnPos;
    }
}