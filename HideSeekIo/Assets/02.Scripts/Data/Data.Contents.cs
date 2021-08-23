using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Data
{



    [Serializable]
    public class InGameStat
    {
        public string key;
        public int price;
        public int level;
    }

    [Serializable]
    //  public class InGameStatData : ILoader<string, InGameStat>
    //  {
    //public List<InGameStat> inGameitems = new List<InGameStat>();
    //      public Dictionary<string, InGameStat> MakeDict()
    //      {
    //	Dictionary<string, InGameStat> dict = new Dictionary<string, InGameStat>();

    //	foreach (InGameStat inGameStat in inGameitems)
    //		dict.Add(inGameStat.key, inGameStat);

    //	return dict;

    //      }
    //  }
    public class InGameStatData : ILoader<string, InGameStat>
    {
        public List<InGameStat> inGameitems = new List<InGameStat>();
        public Dictionary<string, InGameStat> MakeDict()
        {
            Dictionary<string, InGameStat> dict = new Dictionary<string, InGameStat>();

            foreach (InGameStat inGameStat in inGameitems)
                dict.Add(inGameStat.key, inGameStat);
            return dict;
        }
    }
    #region Stat
    [Serializable]
    public class Stat
    {
        public int level;
        public int maxHp;
        public int attack;
        public int totalExp;
    }

    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
        public List<Stat> stats = new List<Stat>();

        public Dictionary<int, Stat> MakeDict()
        {
            Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
            foreach (Stat stat in stats)
                dict.Add(stat.level, stat);
            return dict;
        }
    }
    #endregion
    [Serializable]
    public class TypeColor : SerializableDictionary<Define.ChattingColor, Color> { }



    #region User

    [Serializable]
    public class UserData
    {
        public string key;
        public string nickName;
        public int level;
        public int coin;
        public int exp;
        public int maxExp;

        public List<CharacterUserHasData> characterList;
        public List<string> weaponList;
        public List<string> accessoriesList;
        //초기 생성
        public UserData(string _key, string _nickName)
        {
            key = _key;
            nickName = _nickName;
            level = 1;
            coin = 0;
            exp = 0;
            maxExp = 10;
            weaponList = new List<string>() { "Wm01" };
            accessoriesList = new List<string>();
            characterList = new List<CharacterUserHasData>()
            {
                new CharacterUserHasData(Define.CharacterType.Bear, new SkinHasData("Bear01", true), weaponList[0] ),
                new CharacterUserHasData(Define.CharacterType.Bunny, new SkinHasData("Bunny01", true), weaponList[0]),
                new CharacterUserHasData(Define.CharacterType.Cat, new SkinHasData("Cat01", true), weaponList[0])
            };

        }
    }
    [Serializable]
    public class ServerKey
    {
        public bool isUsing;
        public string avaterSeverKey;
        public string weaponSeverKey;

        public ServerKey(string newServerKey, string newWeaponServerKey, bool newUsing)
        {
            avaterSeverKey = newServerKey;
            weaponSeverKey = newWeaponServerKey;
            isUsing = newUsing;
        }
    }

    [Serializable]
    public class CharacterUserHasData
    {
        public Define.CharacterType characterType;
        public string weaponKey;
        public string accesoryKey;
        public string etc;
        public bool isSelect;
        public List<SkinHasData> characterSkinList;

        public CharacterUserHasData(Define.CharacterType _characterType, SkinHasData _skinData, string _weaponKey)
        {
            characterType = _characterType;
            characterSkinList = new List<SkinHasData>() { _skinData };
            weaponKey = _weaponKey;
            isSelect = false;
        }

        public string GetIsUsingAvater()
        {
            return characterSkinList.Find(s => s.isUsing == true).avaterKey;
        }
    }

    [Serializable]
    public class SkinHasData
    {
        public bool isUsing;
        public string avaterKey;

        public SkinHasData(string key, bool newUsing)
        {
            isUsing = newUsing;
            avaterKey = key;
        }
    }



    [System.Serializable]
    public class OptionData
    {
        public bool bgmValue;
        public bool soundValue;
        public bool isLeftHand;
        public List<InputUIInfo> joystickSettings = new List<InputUIInfo>();
        //기본값
        public OptionData()
        {
            bgmValue = true;
            soundValue = true;
            isLeftHand = false;

            joystickSettings.Clear();
            foreach (var js in UISetting.Instance.inputUIInfos)
            {
                var inputUIInfo = new InputUIInfo();
                inputUIInfo.joystickName = js.joystickName;
                inputUIInfo.size = js.size;
                inputUIInfo.vector2 = js.vector2;
                joystickSettings.Add(inputUIInfo);
            }

        }
    }

    [Serializable]
    public class InputUIInfo
    {
        public string joystickName;
        public Vector2 vector2;
        public float size;
    }

    [Serializable]
    public class CharcterStatInfo
    {
        public Define.CharacterType characterType;
        public string info;
    }

    #endregion

}