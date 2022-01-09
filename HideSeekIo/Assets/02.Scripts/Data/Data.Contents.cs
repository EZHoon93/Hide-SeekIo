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
    //[Serializable]
    //public class TypeColor : SerializableDictionary<Define.ChattingColor, Color> { }



    #region User

    [Serializable]
    public class UserData
    {
        public string key;
        public string nickName;
        public int level;
        public int coin;
        public int gem;
        public int exp;
        public int maxExp;

        public List<AvaterSlotInfo> avaterList;

        //최초 생성
        public UserData(string _key, string _nickName)
        {
            key = _key;
            nickName = _nickName;
            level = 1;
            coin = 0;
            exp = 0;
            gem = 0;
            maxExp = 10;
            avaterList = new List<AvaterSlotInfo>()
            {
                new AvaterSlotInfo()
            };
        }

        public AvaterSlotInfo GetCurrentAvater()
        {
            var index = GetCurrentAvaterIndex();

            if(avaterList.Count > index)
            {
                return avaterList[index];
            }
            //만약 없으면..
            return null;
        }

        public int GetCurrentAvaterIndex()
        {
            return PlayerPrefs.GetInt("av");
        }

        public bool UseNewCharacterAvater(int index)
        {
            if(index > avaterList.Count)
            {
                //실패 
                return false;
            }
            else
            {
                PlayerPrefs.SetInt("av", index);

                return true;
            }
        }

    }
    [Serializable]
    public class ServerKey
    {
        public bool isUsing;
        public string avaterSeverKey;
        public string weaponSeverKey;
        public string accesoryKey;
        public ServerKey(string newServerKey, string newWeaponServerKey, bool newUsing)
        {
            avaterSeverKey = newServerKey;
            weaponSeverKey = newWeaponServerKey;
            isUsing = newUsing;
        }
    }

    [Serializable]
    public class AvaterSlotInfo
    {
        public string characterAvaterKey;
        public string weaponKey;
        public string accesoryKey;
        public bool isSelect;

        public AvaterSlotInfo()
        {
            characterAvaterKey = "Ch01";
            weaponKey = "Wm01";
            accesoryKey = null;
            isSelect = false;
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
        public float bgmValue;
        public float soundValue;
        public bool isLeftHand;
        public List<InputUIInfo> joystickSettings = new List<InputUIInfo>();
        //기본값
        public OptionData()
        {
            bgmValue = .5f;
            soundValue = .5f;
            isLeftHand = false;

            joystickSettings.Clear();
            foreach (var js in Managers.UISetting.inputUIInfos)
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