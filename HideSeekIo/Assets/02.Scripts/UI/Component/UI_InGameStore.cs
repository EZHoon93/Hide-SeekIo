using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_InGameStore : MonoBehaviour
{
    [SerializeField] Transform _content;    //UI???????? ????????
    [SerializeField] TextMeshProUGUI _coinText;
    [SerializeField] ScrollRect _scrollRect;
    [SerializeField] Scrollbar _scrollbar;


    private void Start()
    {
        Invoke("ChangeBarSize", 1.0f);
    }
    void ChangeBarSize()
    {
        _scrollbar.value = 1f;
        _scrollbar.size = 0.1f;
    }

    public void OnValueChanged()
    {
        _scrollbar.size = 0.1f;
    }

    public void UpdateCoinText(int newValue)
    {
        _coinText.text = newValue.ToString();
    }

    void Clear()
    {
        var existItems = _content.transform.GetComponentsInChildren<UI_InGame_Item>();
        foreach (var e in existItems)
        {
            Managers.Resource.Destroy(e.gameObject);
        }
    }
    public void Setup(Define.Team team)
    {
        UpdateCoinText(0);
        Clear();
        var itemPrefab = Managers.Resource.Load<UI_InGame_Item>("Prefabs/UI/SubItem/UI_InGame_Item");
        Type type = null;
        //switch (team)
        //{
        //    case Define.Team.Hide:
        //        type = typeof(Define.HiderStoreList);
        //        break;
        //    case Define.Team.Seek:
        //        type = typeof(Define.SeekrStoreList);
        //        break;
        //}
        MakeSeekrItemList(itemPrefab, type);
        this.gameObject.SetActive(true);
    }


    void MakeSeekrItemList(UI_InGame_Item prefab, Type type)
    {
        foreach (var e in Enum.GetValues(type))
        {

            Enum itemEnum = (Enum)e;
            string itemName = Enum.GetName(type, itemEnum);
            if (Managers.Data.InGameItemDic.ContainsKey(itemName) == false) continue;
            var go = Instantiate(prefab, _content);
            var price = Managers.Data.InGameItemDic[itemName].price;
            var sprite = Managers.Resource.Load<Sprite>($"Sprites/InGameItem/{itemName}");
            //Define.GameItemStateCallBack itemEvent = () => InGameStoreManager.Instance.BuyItem_OnLocal(itemEnum, Managers.Game.myPlayer);   //?????????? ????????
            var info = Lean.Localization.LeanLocalization.GetTranslationText(itemName);
            go.transform.localPosition = Vector3.zero;

            //go.Setup(sprite, price,info, itemEvent); //????,??????,????,????????
        }
    }


    //void MakeSeekrItemList(UI_InGame_Item prefab, Type type)
    //{
    //    foreach (var e in Enum.GetValues(type))
    //    {
    //        Enum itemEnum = (Enum)e;
    //        string itemName = Enum.GetName(type, itemEnum);
    //        if (Managers.Data.InGameItemDic[type].ContainsKey(itemName) ==false) continue;
    //        var go = Instantiate(prefab, _content);
    //        var price = Managers.Data.InGameItemDic[type][itemName].price;
    //        var sprite = Managers.Resource.Load<Sprite>($"Sprites/InGameItem/{itemName}");
    //        Define.GameItemStateCallBack itemEvent = () => InGameStoreManager.Instance.BuyItem_OnLocal(itemEnum, Managers.Game.myPlayer);   //?????????? ????????
    //        go.transform.localPosition = Vector3.zero;
    //        go.Setup(sprite, price, itemEvent); //????,??????,????,????????
    //    }
    //}


}
