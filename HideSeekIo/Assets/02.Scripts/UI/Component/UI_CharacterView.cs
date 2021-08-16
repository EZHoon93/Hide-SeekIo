using System.Collections;

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Data;
using System;

public class UI_CharacterView : UI_Base
{
    enum GameObjects
    {
        ShowPanel
    }
    enum Buttons
    {
        Left,
        Right
    }

    public Define.CharacterType currentCharacterType { get; private set; }
    Dictionary<Define.CharacterType, CharacterAvater> _characterDic = new Dictionary<Define.CharacterType, CharacterAvater>();
    Dictionary<string, GameObject> _showWeaponDic = new Dictionary<string, GameObject>();
    CharacterAvater _currentCharacterAvater;
    GameObject _currentWeapon;

    public event Action<Define.CharacterType> changeViewCallBack;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.Left).gameObject.BindEvent(OnButtonClick_Left);
        GetButton((int)Buttons.Right).gameObject.BindEvent(OnButtonClick_Right);

    }


    public void InitSetupUserSkin()
    {
        foreach(var chadaterHasData in PlayerInfo.userData.characterList)
        {
            var avater =  CreateAvater(chadaterHasData.characterType, chadaterHasData.GetIsUsingAvater());
            CreateWeapon(avater, chadaterHasData.weaponKey);
        }

    }

    public void ChangeProduct(Define.ProductType productType)
    {


    }

    public void ShowCharacter(Define.CharacterType characterType)
    {
        foreach(var ch in _characterDic)
        {
            bool isView = (ch.Key == characterType) ? true : false;
            ch.Value.gameObject.SetActive(isView);
            if (isView)
            {
                ch.Value.AllSkinSetup();
                _currentCharacterAvater = ch.Value;
            }
        }
        currentCharacterType = characterType;
        changeViewCallBack?.Invoke(characterType);
        //CreateAvater(characterUserHasData.characterType, characterUserHasData.GetIsUsingAvater());
        //CreateWeapon(_currentCharacterAvater, characterUserHasData.weaponKey);
        //changeViewCallBack?.Invoke(characterUserHasData.characterType);
    }
    CharacterAvater CreateAvater(Define.CharacterType characterType, string avaterKey)
    {
        //_currentCharacterAvater?.gameObject.SetActive(false);

        CharacterAvater characterAvater = null;
        if (_characterDic.TryGetValue(characterType, out characterAvater) == false)
        {

            characterAvater = Managers.Spawn.CharacterAvaterSpawn(characterType, avaterKey);
            characterAvater.transform.ResetTransform(GetObject((int)GameObjects.ShowPanel).transform);
            _characterDic.Add(characterType, characterAvater);
        }

        return characterAvater;
        //_currentCharacterAvater = characterAvater;
        //_currentCharacterAvater.gameObject.SetActive(true);
    }

    GameObject CreateWeapon(CharacterAvater characterAvater, string key)
    {
        GameObject weaponObject = null;
        if (_showWeaponDic.TryGetValue(key, out weaponObject) == false)
        {
            weaponObject = Managers.Resource.Instantiate($"Melee2/{key}");
            _showWeaponDic.Add(key, weaponObject);
        }
        characterAvater.AddSkinObject(Define.SkinType.Weapon, weaponObject);

        return weaponObject;
    }

    GameObject CreateAccesories(string key)
    {

        return null;
    }

    public void ShowCharacterSkin(Define.CharacterType characterType, string key)
    {

    }

    public void ShowProduct(Define.ProductType productType)
    {

    }

    void OnButtonClick_Left(PointerEventData data)
    {
        if (currentCharacterType <= Define.CharacterType.Bear)
        {
            currentCharacterType = Define.CharacterType.Cat;
        }
        else
        {
            currentCharacterType--;
        }
        ShowCharacter(currentCharacterType);
    }

    void OnButtonClick_Right(PointerEventData data)
    {
        if (currentCharacterType >= Define.CharacterType.Cat)
        {
            currentCharacterType = Define.CharacterType.Bear;
        }
        else
        {
            currentCharacterType++;
        }
        ShowCharacter(currentCharacterType);
    }
}
