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

        MakeItemList(itemPrefab, type);

        this.gameObject.SetActive(true);
    }

    void MakeItemList(UI_InGame_Item prefab, Type type)
    {
        foreach (var e in Enum.GetValues(type))
        {
            Enum newType = (Enum)e;
            if (Managers.Data.InGameItemDict.ContainsKey(newType.ToString()) == false)   //������ ���ٸ�..����x
                continue;

            print(prefab.gameObject.name);

            var go = Instantiate(prefab, _content);
            var price = Managers.Data.InGameItemDict[newType.ToString()].price;
            var sprite = Managers.Resource.Load<Sprite>($"Sprites/InGameItem/{newType.ToString()}");
            Define.GameItemStateCallBack itemEvent = () => GameItemEventList.BuyItem(newType, GameManager.Instance.myPlayer);   //��ưŬ���� ��������
            go.transform.localPosition = Vector3.zero;
            go.Setup(sprite, price, itemEvent); //�¾�,�̹���,����,�ݹ��Լ�
        }
    }
    
}
