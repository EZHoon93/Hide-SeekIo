using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Store : UI_Popup
{
    enum GameObjects
    {
        GridPanel,
        CharacterPanel
    }

    enum Toggles
    {
        Bear,
        Bunny,
        Cat,
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

    Dictionary<Define.ProductType, Dictionary<string, UI_Product>> productDic = new Dictionary<Define.ProductType, Dictionary<string, UI_Product>>();

    Dictionary<string, CharacterAvater> _showCharacterAvaterDic = new Dictionary<string, CharacterAvater>();
    Dictionary<string, GameObject> _showWeaponDic = new Dictionary<string, GameObject>();
    Dictionary<string, GameObject> _showBagDic = new Dictionary<string, GameObject>();
    Dictionary<string, GameObject> _showHatDic = new Dictionary<string, GameObject>();




    Define.ProductType _currentProductType;
    Define.CharacterType _currentChararcterType;
    CharacterAvater _currentShowCharacterAvater;
    GameObject _currentShowWeaponAvater;
    GameObject _currentHat;
    GameObject _currentBag;



    public override void Init()
    {
        base.Init();

        Bind<Toggle>(typeof(Toggles));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        Get<Toggle>((int)Toggles.Bear).gameObject.BindEvent(ClickEvent_Bear);
        Get<Toggle>((int)Toggles.Bunny).gameObject.BindEvent(ClickEvent_Bunny);
        Get<Toggle>((int)Toggles.Cat).gameObject.BindEvent(ClickEvent_Cat);
        Get<Toggle>((int)Toggles.Weapon).gameObject.BindEvent(ClickEvent_Weapon);
        Get<Toggle>((int)Toggles.Bag).gameObject.BindEvent(ClickEvent_Bag);
        Get<Toggle>((int)Toggles.Hat).gameObject.BindEvent(ClickEvent_Hat);

        Get<Button>((int)Buttons.Back).gameObject.BindEvent(ClickEvent_Back);


    }
    private void Start()
    {
        ShowProductList(Define.ProductType.Bear);   //곰을첫번째로 보여줌
    }

    void ClickEvent_Bear(PointerEventData pointerEventData)
    {
        _currentChararcterType = Define.CharacterType.Bear;
        _currentProductType = Define.ProductType.Bear;
        ShowProductList(_currentProductType);
    }

    void ClickEvent_Bunny(PointerEventData pointerEventData)
    {
        _currentChararcterType = Define.CharacterType.Bunny;
        _currentProductType = Define.ProductType.Bunny;
        ShowProductList(_currentProductType);
    }

    void ClickEvent_Cat(PointerEventData pointerEventData)
    {
        _currentChararcterType = Define.CharacterType.Cat;
        _currentProductType = Define.ProductType.Cat;
        ShowProductList(_currentProductType);
    }

    //void ClickEvent_Avater(PointerEventData pointerEventData)
    //{
    //    foreach(var produtType in Enum.GetValues( typeof (Define.ProductType)))
    //    {
    //        if(string.Equals(produtType.ToString(), _currentChararcterType.ToString()))
    //        {
    //            ShowProductList( (Define.ProductType)produtType );
    //        }
    //    }

    //}
    void ClickEvent_Weapon(PointerEventData pointerEventData)
    {
        print("Click Weapin");
        _currentProductType = Define.ProductType.Weapon;
        ShowProductList(_currentProductType);
    }

    void ClickEvent_Bag(PointerEventData pointerEventData)
    {
        _currentProductType = Define.ProductType.Bag;
        ShowProductList(_currentProductType);
    }

    void ClickEvent_Hat(PointerEventData pointerEventData)
    {
        _currentProductType = Define.ProductType.Hat;
        ShowProductList(_currentProductType);
    }
    void ClickEvent_Back(PointerEventData pointerEventData)
    {
        ClosePopupUI();
    }


    void Clear()
    {

        foreach (var p in _showWeaponDic)
        {
            p.Value.gameObject.SetActive(false);
        }

        foreach (var p in productDic)
        {
            foreach(var product in p.Value)
            {
                product.Value.gameObject.SetActive(false);
            }
        }


    }

    void ShowProductList(Define.ProductType productType)
    {
        Clear();    //리스트 제거
        string firstAvater = null;

        //이전 생성 데이터가없다면 생성
        //if ( productDic.ContainsKey(productType) == false)
        //{
        //    //생성
        //    Dictionary<string, UI_Product> content = new Dictionary<string, UI_Product>();
        //    foreach(var product in Managers.Data.ProductDic[productType].Values)
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
        //                productUI.Setup(product.key, price, sprite, Show_WeaponSkin);
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
        //        productUI.transform.SetParent(GetObject((int)GameObjects.GridPanel).transform);
        //        content.Add(product.key, productUI);
        //    }
        //    productDic.Add(productType, content);
        //}
        // //이전 생성한게 있다면 그걸로 보여줌
        //else
        //{
        //    foreach(var p in productDic[productType])
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

    void Show_Hat(string key)
    {
        _currentHat?.gameObject.SetActive(false);
        GameObject hat = null;
        if(_showHatDic.TryGetValue(key, out hat) == false)
        {
            hat = Managers.Resource.Instantiate($"Accessories/Hat/{key}");
            hat.transform.ResetTransform(_currentShowCharacterAvater.headTransform.transform);
            _showHatDic.Add(key, hat);
        }
        _currentHat = hat;
        _currentHat?.gameObject.SetActive(true);
    }

    void Show_Bag(string key)
    {
        _currentBag?.gameObject.SetActive(false);
        GameObject bag = null;
        if (_showBagDic.TryGetValue(key, out bag) == false)
        {
            bag = Managers.Resource.Instantiate($"Accessories/Bag/{key}");
            bag.transform.ResetTransform(_currentShowCharacterAvater.accessoriesTransform.transform);
            _showBagDic.Add(key, bag);
        }
        _currentBag = bag;
        _currentBag?.gameObject.SetActive(true);
    }
    void Show_CharacterAvater( string key)
    {
        _currentShowCharacterAvater?.gameObject.SetActive(false);
        //if (_currentShowCharacterAvater)
        //{
        //    _currentShowCharacterAvater.gameObject.SetActive(false);
        //}
        GameObject showAvater = null;
        if (_showCharacterAvaterDic.ContainsKey(key) == false)
        {
            showAvater = Managers.Resource.Instantiate($"Character/{_currentChararcterType.ToString()}/{key}");
            showAvater.transform.ResetTransform(GetObject((int)GameObjects.CharacterPanel).transform);
            _showCharacterAvaterDic.Add(key, showAvater.GetComponent<CharacterAvater>());
        }
        else
        {
            showAvater = _showCharacterAvaterDic[key].gameObject;
        }

        _currentShowCharacterAvater = showAvater.GetComponent<CharacterAvater>();
        _currentShowCharacterAvater.gameObject.SetActive(true);

        //버튼상태 체크
        //bool hasSkin  = PlayerInfo.CheckUserHasCharacterAvarer(_currentChararcterType, key);
        //GetButton((int)Buttons.Buy).interactable = !hasSkin;
    }

    void Show_WeaponSkin(string key)
    {

        if (_currentShowWeaponAvater)
        {
            _currentShowWeaponAvater.gameObject.SetActive(false);
        }
        GameObject showWeaponSkin = null;
        if (_showWeaponDic.ContainsKey(key) == false)
        {
            showWeaponSkin = Managers.Resource.Instantiate($"Melee2/{key}");
            _showWeaponDic.Add(key, showWeaponSkin);
        }
        else
        {
            showWeaponSkin = _showWeaponDic[key].gameObject;
        }

        _currentShowWeaponAvater = showWeaponSkin;
        _currentShowWeaponAvater.transform.ResetTransform(_currentShowCharacterAvater.rightHand.transform);
        _currentShowWeaponAvater.gameObject.SetActive(true);

        //bool hasSkin = PlayerInfo.CheckUserHasWeaponSkin(key);
        //GetButton((int)Buttons.Buy).interactable = !hasSkin;
    }
    public override void ClosePopupUI()
    {
        base.ClosePopupUI();
    }
}
