using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Store_Character : UI_Popup
{
    enum GameObjects
    {
        ProductContent,
        CharacterView,
        ToggleGroups
    }

    enum Buttons
    {
        Cancel
    }

    ToggleGroup _toggleGroup;
    UI_CharacterView _uI_CharacterView;
    Define.SkinType _currentSkinType;

    Dictionary<string, Dictionary<string, UI_Product>> productDic = new Dictionary<string, Dictionary<string, UI_Product>>();
    Dictionary<string, UI_Product> _currentShowProuctDic; //현재 보여주고있는 상품목록 리스트

    public override void Init()
    {
        base.Init();
        Bind<Toggle>(typeof(Define.SkinType));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        _uI_CharacterView = GetObject((int)GameObjects.CharacterView).GetComponent<UI_CharacterView>();
        _toggleGroup = GetObject((int)GameObjects.ToggleGroups).GetComponent<ToggleGroup>();

        Get<Toggle>((int)Define.SkinType.Skin).onValueChanged.AddListener( (b) => OnChange_ToggleValue(b,Define.SkinType.Skin));
        Get<Toggle>((int)Define.SkinType.RightHand).onValueChanged.AddListener((b) => OnChange_ToggleValue(b, Define.SkinType.RightHand));
        Get<Toggle>((int)Define.SkinType.Bag).onValueChanged.AddListener((b) => OnChange_ToggleValue(b, Define.SkinType.Bag));
        Get<Toggle>((int)Define.SkinType.Hat).onValueChanged.AddListener((b) => OnChange_ToggleValue(b, Define.SkinType.Hat));
        GetButton((int)Buttons.Cancel).gameObject.BindEvent(ClickEvent_Back);
        //_uI_CharacterView.changeViewCallBack += ChangeMyCharcterType;
    }
    private void Start()
    {
        //_uI_CharacterView.InitSetupUserSkin();
        //_uI_CharacterView.ShowCharacter(PlayerInfo.GetCurrentUsingCharacter());
    }

    void OnChange_ToggleValue(bool isOn, Define.SkinType skinType)
    {
        if (isOn == false) return;
        if (_currentSkinType == skinType) return;
        ShowProductList(skinType);
    }

    void ClickEvent_Back(PointerEventData pointerEventData)
    {
        ClosePopupUI();
    }
    void Clear()
    {
        if(_currentShowProuctDic != null)
        {
            foreach (var product in _currentShowProuctDic)
            {
                product.Value.gameObject.SetActive(false);
            }
        }
    }

    //캐릭터 뷰에서 캐릭터를 변경하였을 떄 호출
    void ChangeMyCharcterType()
    {
        if (_currentSkinType != Define.SkinType.Skin) return;
        ShowProductList(_currentSkinType);

    }

    void ShowProductList(Define.SkinType skinType)
    {
        Clear();    //리스트 제거
        _currentSkinType = skinType;
        string showSkinKey;
        Dictionary<string, UI_Product> content;

        //if (skinType == Define.SkinType.Skin)
        //{
        //    showSkinKey = _uI_CharacterView.currentCharacterType.ToString();    //스킨타입은 보고있는캐릭으로
        //}
        //else
        //{
        //    showSkinKey = skinType.ToString();
        //}

        //bool isExist = productDic.TryGetValue(showSkinKey, out content);
        //if (isExist == false)
        //{
        //    content = new Dictionary<string, UI_Product>();
        //    foreach (var productData in Managers.Data.ProductDic[showSkinKey].Values)
        //    {
        //        var productUI = Managers.Resource.Instantiate("UI/SubItem/UI_Product").GetComponent<UI_Product>();
        //        var price = productData.price;
        //        var productkey = productData.key;
        //        string folder = skinType == Define.SkinType.Skin ? "Character" : "Accessories";
        //        var sprite = Managers.Resource.Load<Sprite>($"Sprites/{folder}/{showSkinKey}/{productkey}");
        //        Action clickEventCallBack = null;
        //        //콜백함수 설정
        //        switch (skinType)
        //        {
        //            case Define.SkinType.Skin:
        //                clickEventCallBack = () => { _uI_CharacterView.ShowSkinObject(skinType, $"{folder}/{showSkinKey}/{productkey}"); };
        //                productUI.Setup(showSkinKey, price, sprite, clickEventCallBack);
        //                break;
        //            default:
        //                productUI.Setup(showSkinKey, price, sprite, clickEventCallBack);
        //                break;
        //        }
        //        productUI.transform.SetParent(GetObject((int)GameObjects.ProductContent).transform);
        //        content.Add(productkey, productUI);
        //    }
        //    productDic.Add(showSkinKey, content);
        //}
        //else
        //{
        //    foreach (var p in content.Values)
        //        p.gameObject.SetActive(true);
        //}
        

        //_currentShowProuctDic = content;
    }

    
    public override void ClosePopupUI()
    {
        base.ClosePopupUI();
    }
}
