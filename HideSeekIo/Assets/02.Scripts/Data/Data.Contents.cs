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
		public string info;
    }

    [Serializable]
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

		public List<ServerKey> skinList;
	}
	[Serializable]
	public class ServerKey
    {
		public bool isUsing;
		public string avaterSeverKey;
		public string weaponSeverKey;

		public ServerKey(string newServerKey,string newWeaponServerKey , bool newUsing)
        {
			avaterSeverKey= newServerKey;
			weaponSeverKey = newWeaponServerKey;
			isUsing = newUsing;
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

	[System.Serializable]
	public class InputUIInfo
	{
		public string joystickName;
		public Vector2 vector2;
		public float size;
	}


	#endregion

}