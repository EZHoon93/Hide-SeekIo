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
	[System.Serializable]
	public class TypeColor : SerializableDictionary<Define.ChattingColor, Color> { }
}