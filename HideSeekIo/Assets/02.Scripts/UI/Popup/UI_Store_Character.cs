using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Coffee.UIExtensions;
using TMPro;
using Data;

public class UI_Store_Character: UI_Popup
{
    [SerializeField] Transform _content;
    [SerializeField] UIParticle _uIParticle;
    CharacterAvater currentAvaterObject;
    GameObject currentWeaponObject;

    int _currentAvaterIndex;
    enum Buttons
    {
        Skin,
        Weapon,
        State    ,
        Left,
        Right,
        AddSkin

    }

    enum Texts
    {
        AvaterCount,
        StateTxt

    }

    private void Reset()
    {
        _content = this.transform.Find("content");
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        GetButton((int)Buttons.Skin).gameObject.BindEvent(ChangeSkin);
        GetButton((int)Buttons.Weapon).gameObject.BindEvent(ChangeWeapon);
        GetButton((int)Buttons.Right).gameObject.BindEvent(AvaterRightButton);
        GetButton((int)Buttons.Left).gameObject.BindEvent(AvaterLeftButton);
        GetButton((int)Buttons.State).gameObject.BindEvent(UseAvater);
        GetButton((int)Buttons.AddSkin).gameObject.BindEvent(AddSkin);



    }
    private void Start()
    {
        _currentAvaterIndex = PlayerInfo.CurrentAvaterUsingIndex();
        UpdateAvater(PlayerInfo.CurrentSkin);
        PhotonGameManager.Instacne.gameJoin += Destroy;
    }
    
    void Destroy()
    {
        PhotonGameManager.Instacne.gameJoin -= Destroy;
        Managers.Resource.Destroy(this.gameObject);
    }

    void UpdateAvater(ServerKey skinSeverKey)
    {
        CreateAvaterObject(skinSeverKey.avaterSeverKey);
        UpdateStateButtonNText(skinSeverKey.isUsing);
        UpdaterAvaterCountText();
    }
    void CreateAvaterObject(string avaterID)
    {
        if (currentAvaterObject != null)
        {
            Managers.Resource.Destroy(currentAvaterObject.gameObject);
        }
        currentAvaterObject = Managers.Resource.Instantiate($"Avater/{avaterID}").GetComponent<CharacterAvater>();
        currentAvaterObject.transform.ResetTransform(_content);
        var avaterAnimator = currentAvaterObject.GetComponent<Animator>();
        avaterAnimator.runtimeAnimatorController = GameSetting.Instance.GetRuntimeAnimatorController(Define.Team.Seek);
        CreateWeaponObject(PlayerInfo.userData.skinList[_currentAvaterIndex].weaponSeverKey);

    }

    void UpdateStateButtonNText(bool isUsing)
    {
        if (isUsing)
        {
            GetButton((int)Buttons.State).interactable = false;
            GetText((int)Texts.StateTxt).text = "사용중";
        }
        else
        {
            GetButton((int)Buttons.State).interactable = true;
            GetText((int)Texts.StateTxt).text = "사용하기";
        }
    }
    void UpdaterAvaterCountText()
    {
        GetText((int)Texts.AvaterCount).text = $"{_currentAvaterIndex+1}/{ PlayerInfo.userData.skinList.Count}";
    }

    void CreateWeaponObject(string weaponID )
    {
        if (currentWeaponObject != null)
        {
            Managers.Resource.Destroy(currentWeaponObject);
        }
        currentWeaponObject = Managers.Resource.Instantiate($"Melee2/{weaponID}");
        currentWeaponObject.transform.ResetTransform(currentAvaterObject.RightHandAmount);

    }


    #region 버튼 이벤트
    void AddSkin(PointerEventData pointerEventData)
    {
        Managers.UI.ShowPopupUI<UI_Check_Buy>().Setup("스킨", () => {
            StoreManager.AddSkinList();
            _uIParticle.Play();
            UpdaterAvaterCountText();
        });
    }
    void ChangeSkin(PointerEventData pointerEventData)
    {

        Managers.UI.ShowPopupUI<UI_Check_Buy>().Setup("스킨", () => {
            var selectAvaterID = StoreManager.ChangeSkin(_currentAvaterIndex);
            CreateAvaterObject(selectAvaterID);
            _uIParticle.Play();
         });
    }
    void ChangeWeapon(PointerEventData pointerEventData)
    {
        Managers.UI.ShowPopupUI<UI_Check_Buy>().Setup("스킨", () => {
            var selectWeaponID = StoreManager.ChangeWeapon(_currentAvaterIndex);
            CreateWeaponObject(selectWeaponID);
            _uIParticle.Play();
        });
    }

    void UseAvater(PointerEventData pointerEventData)
    {
        PlayerInfo.ChangeCurrentSkin(_currentAvaterIndex);
        UpdateStateButtonNText(true);
    }


    void AvaterLeftButton(PointerEventData pointerEventData)
    {
        if(--_currentAvaterIndex < 0)
        {
            _currentAvaterIndex = PlayerInfo.userData.skinList.Count-1;
        }
        UpdateAvater(PlayerInfo.userData.skinList[_currentAvaterIndex]);

    }

    void AvaterRightButton(PointerEventData pointerEventData)
    {
        if(++_currentAvaterIndex >= PlayerInfo.userData.skinList.Count)
        {
            _currentAvaterIndex = 0;

        }
        UpdateAvater(PlayerInfo.userData.skinList[_currentAvaterIndex]) ;

    }

    #endregion


}
