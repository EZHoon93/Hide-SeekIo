using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class UI_InGameStore : MonoBehaviour
{
    [SerializeField] Transform _content;    //UI�������� ��ġ�Ұ�
    [SerializeField] TextMeshProUGUI _coinText;

    public void UpdateCoinText(int newValue)
    {
        _coinText.text = newValue.ToString();
    }


    public void Setup(Define.Team team)
    {
        UpdateCoinText(0);
        var itemPrefab = Managers.Resource.Load<UI_InGame_Item>("Prefabs/UI/SubItem/UI_InGame_Item");
        Type type = null;
        switch (team)
        {
            case Define.Team.Hide:
                print("Hider Setup InGameStore");
                type = typeof(Define.HiderStoreList);
                break;
            case Define.Team.Seek:
                print("Seeker Setup InGameStore");
                type = typeof(Define.SeekrStoreList);

                break;
        }

        MakeSeekrItemList(itemPrefab, type);

        this.gameObject.SetActive(true);
    }

    void MakeSeekrItemList(UI_InGame_Item prefab, Type type)
    {
        foreach (var e in Enum.GetValues(type))
        {
            Enum itemEnum = (Enum)e;
            string itemName = Enum.GetName(type, itemEnum);
            if (Managers.Data.InGameItemDic[type].ContainsKey(itemName) ==false) continue;
            var go = Instantiate(prefab, _content);
            var price = Managers.Data.InGameItemDic[type][itemName].price;
            var sprite = Managers.Resource.Load<Sprite>($"Sprites/InGameItem/{type.Name}/{itemName}");
            Define.GameItemStateCallBack itemEvent = () => GameItemEventList.BuyItem(itemEnum, GameManager.Instance.myPlayer);   //��ưŬ���� ��������
            go.transform.localPosition = Vector3.zero;
            go.Setup(sprite, price, itemEvent); //�¾�,�̹���,����,�ݹ��Լ�
        }
    }

    
}
