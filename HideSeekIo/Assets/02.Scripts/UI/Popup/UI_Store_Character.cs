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
        CharacterView
    }

    enum Toggles
    {
        Skin,
        Weapon,
        Hat,
        Bag
    }

    enum Buttons
    {
        Buy,
        Back
    }

    [SerializeField] ToggleGroup _toggleGroup;

    Dictionary<Define.SkinType, Dictionary<string, UI_Product>> productDic = new Dictionary<Define.SkinType, Dictionary<string, UI_Product>>();
    Dictionary<Define.CharacterType, Dictionary<string, UI_Product>> characterSkinDic = new Dictionary<Define.CharacterType, Dictionary<string, UI_Product>>();
    UI_CharacterView _uI_CharacterView;
    Define.SkinType _currentSkinType;

    Dictionary<string, UI_Product> _currentShowProuctDic; //현재 보여주고있는 상품목록 리스트

    public override void Init()
    {
        base.Init();
        Bind<Toggle>(typeof(Toggles));
        Bind<GameObject>(typeof(GameObjects));
        Get<Toggle>((int)Toggles.Skin).gameObject.BindEvent(ClickEvent_Skin);
        Get<Toggle>((int)Toggles.Weapon).gameObject.BindEvent(ClickEvent_Weapon);
        Get<Toggle>((int)Toggles.Bag).gameObject.BindEvent(ClickEvent_Bag);
        Get<Toggle>((int)Toggles.Hat).gameObject.BindEvent(ClickEvent_Hat);


        _uI_CharacterView = GetObject((int)GameObjects.CharacterView).GetComponent<UI_CharacterView>();
        _uI_CharacterView.changeViewCallBack += ChangeMyCharcterType;
    }
    private void Start()
    {
        _uI_CharacterView.InitSetupUserSkin();
        ShowProductList(Define.SkinType.Skin);   //스킨
        _uI_CharacterView.ShowCharacter(Define.CharacterType.Bear);
    }

    void ClickEvent_Skin(PointerEventData pointerEventData)
    {
        _currentSkinType = Define.SkinType.Skin;
        ShowProductList(_currentSkinType);
    }
    
    void ClickEvent_Weapon(PointerEventData pointerEventData)
    {
        _currentSkinType = Define.SkinType.Skin;
    }

    void ClickEvent_Bag(PointerEventData pointerEventData)
    {
        _currentSkinType = Define.SkinType.Skin;
    }

    void ClickEvent_Hat(PointerEventData pointerEventData)
    {
        _currentSkinType = Define.SkinType.Skin;
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
    void ChangeMyCharcterType(Define.CharacterType characterType)
    {
        if (_currentSkinType != Define.SkinType.Skin) return;
        print("체인지 캐릭터타입");
        ShowSkinProduct(characterType);
    }
    void ShowNoSkin(Define.SkinType skinType)
    {

    }

    Dictionary<string,UI_Product> ShowSkinProduct(Define.CharacterType characterType)
    {
        Clear();
        Dictionary<string, UI_Product> productDic;
        bool isExist = characterSkinDic.TryGetValue(characterType, out productDic);
        if(isExist == false)
        {
            productDic = new Dictionary<string, UI_Product>();
            var productPrefab = Managers.Resource.Load<UI_Product>("Prefabs/UI/SubItem/UI_Product");
            foreach(var productData in Managers.Data.ProductDic[characterType.ToString()].Values)
            {
                var productUIOjbect = Instantiate(productPrefab);
                var sprite = Managers.Resource.Load<Sprite>($"Sprites/Character/{characterType.ToString()}/{productData.key}");
                Action clickEvent = () => { _uI_CharacterView.ShowCharacterSkin(characterType, productData.key); };
                productUIOjbect.Setup(productData.key, productData.price,sprite, clickEvent);
                productUIOjbect.transform.SetParent(GetObject((int)GameObjects.ProductContent).transform);
                
                productDic.Add(productData.key, productUIOjbect);
            }
            characterSkinDic.Add(characterType, productDic);
        }

        foreach(var productObject in productDic.Values)
        {
            productObject.gameObject.SetActive(true);
        }
        _currentShowProuctDic = productDic;

        return productDic;
    }
    void ShowProductList(Define.SkinType skinType)
    {
        Clear();    //리스트 제거

        if (skinType == Define.SkinType.Skin)
        {
            _currentShowProuctDic = ShowSkinProduct(_uI_CharacterView.currentCharacterType);
        }
        else
        {

        }


        ////이전 생성 데이터가없다면 생성
        //if (productDic.ContainsKey(productType) == false)
        //{
        //    //생성
        //    Dictionary<string, UI_Product> content = new Dictionary<string, UI_Product>();
        //    foreach (var product in Managers.Data.ProductDic[productType].Values)
        //    {
        //        var productUI = Managers.Resource.Instantiate("UI/SubItem/UI_Product").GetComponent<UI_Product>();
        //        var price = product.price;
        //        var sprite = Managers.Resource.Load<Sprite>($"Sprites/Product/{productType.ToString()}/{product.key}");
        //        if (string.IsNullOrEmpty(firstAvater))
        //        {
        //            firstAvater = product.key;
        //        }
        //        //콜백함수 설정
        //        switch (productType)
        //        {
        //            case Define.ProductType.Weapon:
        //                productUI.Setup(product.key, price, sprite, _uI_CharacterView);
        //                break;
        //            case Define.ProductType.Bag:
        //                productUI.Setup(product.key, price, sprite, Show_Bag);
        //                break;
        //            case Define.ProductType.Hat:
        //                productUI.Setup(product.key, price, sprite, Show_Hat);
        //                break;
        //            default:
        //                productUI.Setup(product.key, price, sprite, Show_CharacterAvater);

        //                break;
        //        }
        //        productUI.transform.SetParent(GetObject((int)GameObjects.ProductContent).transform);
        //        content.Add(product.key, productUI);
        //    }
        //    productDic.Add(productType, content);
        //}
        ////이전 생성한게 있다면 그걸로 보여줌
        //else
        //{
        //    foreach (var p in productDic[productType])
        //    {
        //        if (string.IsNullOrEmpty(firstAvater))
        //        {
        //            firstAvater = p.Key;
        //        }
        //        p.Value.gameObject.SetActive(true);
        //    }
        //}

        ////인게임으로 보여줌
        //switch (productType)
        //{
        //    case Define.ProductType.Weapon:
        //        Show_WeaponSkin(firstAvater);
        //        break;
        //    case Define.ProductType.Bag:
        //        Show_Bag(firstAvater);
        //        break;
        //    case Define.ProductType.Hat:
        //        Show_Hat(firstAvater);
        //        break;
        //    default:
        //        Show_CharacterAvater(firstAvater);
        //        break;
        //}

    }

    //void Show_Hat(string key)
    //{
    //    _currentHat?.gameObject.SetActive(false);
    //    GameObject hat = null;
    //    if (_showHatDic.TryGetValue(key, out hat) == false)
    //    {
    //        hat = Managers.Resource.Instantiate($"Accessories/Hat/{key}");
    //        hat.transform.ResetTransform(_currentShowCharacterAvater.headTransform.transform);
    //        _showHatDic.Add(key, hat);
    //    }
    //    _currentHat = hat;
    //    _currentHat?.gameObject.SetActive(true);
    //}

    //void Show_Bag(string key)
    //{
    //    _currentBag?.gameObject.SetActive(false);
    //    GameObject bag = null;
    //    if (_showBagDic.TryGetValue(key, out bag) == false)
    //    {
    //        bag = Managers.Resource.Instantiate($"Accessories/Bag/{key}");
    //        bag.transform.ResetTransform(_currentShowCharacterAvater.accessoriesTransform.transform);
    //        _showBagDic.Add(key, bag);
    //    }
    //    _currentBag = bag;
    //    _currentBag?.gameObject.SetActive(true);
    //}
    //void Show_CharacterAvater(string key)
    //{
    //    _currentShowCharacterAvater?.gameObject.SetActive(false);
    //    //if (_currentShowCharacterAvater)
    //    //{
    //    //    _currentShowCharacterAvater.gameObject.SetActive(false);
    //    //}
    //    GameObject showAvater = null;
    //    if (_showCharacterAvaterDic.ContainsKey(key) == false)
    //    {
    //        showAvater = Managers.Resource.Instantiate($"Character/{_currentChararcterType.ToString()}/{key}");
    //        //showAvater.transform.ResetTransform(GetObject((int)GameObjects.CharacterPanel).transform);
    //        _showCharacterAvaterDic.Add(key, showAvater.GetComponent<CharacterAvater>());
    //    }
    //    else
    //    {
    //        showAvater = _showCharacterAvaterDic[key].gameObject;
    //    }

    //    _currentShowCharacterAvater = showAvater.GetComponent<CharacterAvater>();
    //    _currentShowCharacterAvater.gameObject.SetActive(true);

    //    //버튼상태 체크
    //    //bool hasSkin  = PlayerInfo.CheckUserHasCharacterAvarer(_currentChararcterType, key);
    //    //GetButton((int)Buttons.Buy).interactable = !hasSkin;
    //}

    //void Show_WeaponSkin(string key)
    //{

    //    if (_currentShowWeaponAvater)
    //    {
    //        _currentShowWeaponAvater.gameObject.SetActive(false);
    //    }
    //    GameObject showWeaponSkin = null;
    //    if (_showWeaponDic.ContainsKey(key) == false)
    //    {
    //        showWeaponSkin = Managers.Resource.Instantiate($"Melee2/{key}");
    //        _showWeaponDic.Add(key, showWeaponSkin);
    //    }
    //    else
    //    {
    //        showWeaponSkin = _showWeaponDic[key].gameObject;
    //    }

    //    _currentShowWeaponAvater = showWeaponSkin;
    //    _currentShowWeaponAvater.transform.ResetTransform(_currentShowCharacterAvater.weaponTransform.transform);
    //    _currentShowWeaponAvater.gameObject.SetActive(true);

    //    //bool hasSkin = PlayerInfo.CheckUserHasWeaponSkin(key);
    //    //GetButton((int)Buttons.Buy).interactable = !hasSkin;
    //}
    public override void ClosePopupUI()
    {
        base.ClosePopupUI();
    }
}
