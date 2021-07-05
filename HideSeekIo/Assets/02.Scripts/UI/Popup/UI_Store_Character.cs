﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Coffee.UIExtensions;

public class UI_Store_Character: UI_Popup
{
    [SerializeField] Transform _content;
    [SerializeField] UIParticle _uIParticle;
    GameObject currentAvaterObject;
    GameObject currentWeaponObject;
    enum Buttons
    {
        Cancel,
        Skin,
        Weapon,
        NickName,
        AddList,
        Coin1,
        Coin2
            
    }

    private void Reset()
    {
        _content = this.transform.Find("content");
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.Cancel).gameObject.BindEvent(Cancel);
        GetButton((int)Buttons.Skin).gameObject.BindEvent(Skin);
        GetButton((int)Buttons.Weapon).gameObject.BindEvent(Weapon);


    }
    private void Start()
    {
        GetAvater();
    }

    void GetAvater()
    {
        if(currentAvaterObject != null)
        {
            Managers.Resource.Destroy(currentAvaterObject);
        }
        var currentAvater = PlayerInfo.CurrentAvater;
        currentAvaterObject = Managers.Resource.Instantiate($"Avater/{currentAvater}");
        currentAvaterObject.transform.ResetTransform(_content);
        var avaterAnimator = currentAvaterObject.GetComponent<Animator>();
        avaterAnimator.runtimeAnimatorController = GameSetting.Instance.GetRuntimeAnimatorController(Define.Team.Seek);
        GetWeapon(currentAvaterObject.GetComponent<CharacterAvater>());
    }

    void GetWeapon(CharacterAvater characterAvater)
    {
        if (currentWeaponObject != null)
        {
            Managers.Resource.Destroy(currentWeaponObject);
        }
        var currentWeapon = PlayerInfo.CurrentWeapon;
        currentWeaponObject = Managers.Resource.Instantiate($"Melee2/{currentWeapon}");
        currentWeaponObject.transform.ResetTransform(characterAvater.RightHandAmount);

    }
    void Cancel(PointerEventData pointerEventData)
    {
        Managers.UI.ClosePopupUI();
    }

    void Skin(PointerEventData pointerEventData)
    {

        Managers.UI.ShowPopupUI<UI_Check_Buy>().Setup("스킨", () => {
            StoreManager.ChangeSkin();
            GetAvater();
            _uIParticle.Play();
         });
    }
    void Weapon(PointerEventData pointerEventData)
    {
        Managers.UI.ShowPopupUI<UI_Check_Buy>().Setup("스킨", () => {
            StoreManager.ChangeWeapon();
            GetWeapon(currentAvaterObject.GetComponent<CharacterAvater>());
        });
    }
    void NickName(PointerEventData pointerEventData)
    {
        
    }
    void AddList(PointerEventData pointerEventData)
    {
        
    }
}
