using System.Collections;

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Data;
using System;
using System.Linq;

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
    Dictionary<string, SkinObject> _skinDic = new Dictionary<string, SkinObject>();
    Dictionary<Define.CharacterType, CharacterAvater> _characterDic = new Dictionary<Define.CharacterType, CharacterAvater>();
    CharacterAvater _currentCharacterAvater;
    public event Action changeViewCallBack;
    public bool isCanBuy;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Toggle>(typeof(Define.CharacterType));
    }

 
  

    void BuyCharacter(Define.CharacterType characterType)
    {

    }


    public void InitSetupUserSkin()
    {
        foreach (var chType in Enum.GetValues(typeof(Define.CharacterType)))
        {
            var characterType = (Define.CharacterType)chType;
            var characterUserHasData = PlayerInfo.GetCharacterData(characterType);
            Toggle chToggle = Get<Toggle>((int)chType);
            if (characterUserHasData == null)
            {
                chToggle.interactable = false;
                chToggle.gameObject.BindEvent((point)=> BuyCharacter(characterType));
                continue;
            }
            else
            {
                chToggle.interactable = true;
                chToggle.gameObject.BindEvent((point) => ShowCharacter(characterType));
            }
        }

        foreach (var characterUserHasData in PlayerInfo.userData.characterList)
        {
            var skinObject = GetSkinObject($"Character/{characterUserHasData.characterType.ToString()}/{characterUserHasData.GetIsUsingAvater()}");
            _characterDic.Add(characterUserHasData.characterType, skinObject.GetComponent<CharacterAvater>());
            skinObject.transform.ResetTransform(GetObject((int)GameObjects.ShowPanel).transform);

            //CreateWeapon(avater, chadaterHasData.weaponKey);
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
        changeViewCallBack?.Invoke();
        //CreateAvater(characterUserHasData.characterType, characterUserHasData.GetIsUsingAvater());
        //CreateWeapon(_currentCharacterAvater, characterUserHasData.weaponKey);
        //changeViewCallBack?.Invoke(characterUserHasData.characterType);
    }

    public SkinObject GetSkinObject(string skinKey)
    {
        if (string.IsNullOrEmpty(skinKey)) return null;
        SkinObject skinObject = null;
        bool isExist = _skinDic.TryGetValue(skinKey, out skinObject);
        if(isExist == false)
        {
           skinObject = Managers.Resource.Instantiate($"{skinKey}").GetComponent<SkinObject>();
           
            _skinDic.Add(skinKey, skinObject);
        }

        return skinObject;
    }

    public void ShowSkinObject(Define.SkinType skinType, string skinKey)
    {
        var skinObject = GetSkinObject(skinKey);
        if(skinType == Define.SkinType.Skin)
        {
            _currentCharacterAvater?.gameObject.SetActive(false);
            skinObject.transform.ResetTransform(GetObject((int)GameObjects.ShowPanel).transform);
            var newCurrentCharacterAvater = skinObject.GetComponent<CharacterAvater>();
            newCurrentCharacterAvater.CloneSkin(_currentCharacterAvater.skinDic);
            _currentCharacterAvater = newCurrentCharacterAvater;
            _characterDic[currentCharacterType] = _currentCharacterAvater;
        }
        else
        {
            _currentCharacterAvater.AddSkinObject(skinType, skinObject.gameObject);
        }
    }



    //void OnButtonClick_Left(PointerEventData data)
    //{
    //    if (currentCharacterType <= Define.CharacterType.Bear)
    //    {
    //        currentCharacterType = Define.CharacterType.Cat;
    //    }
    //    else
    //    {
    //        currentCharacterType--;
    //    }
    //    ShowCharacter(currentCharacterType);
    //}

    //void OnButtonClick_Right(PointerEventData data)
    //{
    //    if (currentCharacterType >= Define.CharacterType.Cat)
    //    {
    //        currentCharacterType = Define.CharacterType.Bear;
    //    }
    //    else
    //    {
    //        currentCharacterType++;
    //    }
    //    ShowCharacter(currentCharacterType);
    //}
}
