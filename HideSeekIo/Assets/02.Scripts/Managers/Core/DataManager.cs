using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class InGameStore
{

}

public class DataManager
{
    public Define.GameDataState State { get; private set; }
    public Dictionary<int, Stat> StatDict { get; private set; } = new Dictionary<int, Data.Stat>();

    //Dictionary<string, InGameStat> InGameItems = new Dictionary<string, InGameStat>();
    //Dictionary<string, InGameStat> inGameSeekrItems = new Dictionary<string, InGameStat>();

    //public Dictionary<Type, Dictionary<string, InGameStat>> InGameItemDic = new Dictionary<Type, Dictionary<string, InGameStat>>();

    public  Dictionary<string, InGameStat> InGameItemDic = new Dictionary<string, InGameStat>();

    public Dictionary<string, Dictionary<string, InGameStat>> ProductDic { get; set; }
        = new Dictionary<string, Dictionary<string, InGameStat>>();


    public void Init()
    {
        StatDict = LoadJson<StatData, int, Stat>("StatData").MakeDict();

        InGameItemDic = LoadJson<InGameStatData, string, InGameStat>("InGameItem").MakeDict();


        foreach (var productType in Enum.GetValues(typeof(Define.ProductType)))
        {
            var productDataDic = LoadJson<InGameStatData, string, InGameStat>(productType.ToString()).MakeDict();
            if(ProductDic.ContainsKey(productType.ToString()) == false)
            {
                ProductDic.Add(productType.ToString(), productDataDic);
            }
        }

        //inGameSeekrItems = LoadJson<InGameStatData, string, InGameStat>("InGameSeeker").MakeDict();


        //InGameItemDic.Add( typeof(Define.HiderStoreList), InGameHiderItems);
        //InGameItemDic.Add(typeof(Define.SeekrStoreList) , inGameSeekrItems);



        State = Define.GameDataState.Load;

    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
		TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        //Debug.LogError(textAsset);
        return JsonUtility.FromJson<Loader>(textAsset.text);
	}
}
