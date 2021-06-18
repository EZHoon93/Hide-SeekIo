using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Define.GameDataState State { get; private set; }
    public Dictionary<int, Data.Stat> StatDict { get; private set; } = new Dictionary<int, Data.Stat>();

    public Dictionary<string, Data.InGameStat> InGameItemDict { get; private set; } = new Dictionary<string, Data.InGameStat>();

    public void Init()
    {
        StatDict = LoadJson<Data.StatData, int, Data.Stat>("StatData").MakeDict();

        InGameItemDict = LoadJson<Data.InGameStatData, string, Data.InGameStat>("InGameStatData").MakeDict();

        Debug.Log(InGameItemDict.Count);
        Debug.Log(StatDict.Count);

        State = Define.GameDataState.Load;

    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
		TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        Debug.LogError(textAsset);
        return JsonUtility.FromJson<Loader>(textAsset.text);
	}
}
