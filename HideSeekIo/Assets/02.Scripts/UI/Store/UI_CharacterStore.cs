using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterStore : MonoBehaviour
{
    [SerializeField] Transform _characterPanel;
    [SerializeField] Button _stateButton;
    [SerializeField] TextMeshProUGUI _avaterCountText;
    [SerializeField] TextMeshProUGUI _nickNameText;


    CharacterAvater _currentCharacterAvater;
    List<CharacterAvater> _avaterList = new List<CharacterAvater>(16);
    int _currentIndex;

    private void Awake()
    {
        _stateButton.onClick.AddListener(OnClick_StateButton);
        PlayerInfo.chnageInfoEvent += SetupUserNickName;
    }
    private void Start()
    {
        foreach(var hasAvaterData in PlayerInfo.userData.avaterList)
        {
            var avater = CreateAvater(hasAvaterData.characterAvaterKey);
            CreateWeapon(hasAvaterData.weaponKey, avater);
            CreateAccessories(hasAvaterData.accesoryKey, avater);
        }

        ShowAvater(PlayerInfo.userData.GetCurrentAvaterIndex());
        SetupUserNickName();
    }

    void SetupUserNickName()
    {
        _nickNameText.text = PlayerInfo.userData.nickName;
    }

    #region 오브젝 생성
    CharacterAvater CreateAvater(string prefabID)
    {
        CharacterAvater avaterObject = null;
        avaterObject = Managers.Spawn.AvaterSpawn(prefabID).GetComponent<CharacterAvater>();
        if (avaterObject == null)
        {
            return null;
        }
        avaterObject.transform.ResetTransform(_characterPanel);
        avaterObject.gameObject.SetActive(false);
        _avaterList.Add(avaterObject);
        return avaterObject;
    }

    void CreateWeapon(string prefabId, CharacterAvater characterAvater)
    {
        var weapon = Managers.Spawn.WeaponSkinSpawn(prefabId);
        if (weapon == null) return;
        characterAvater.SetupWeapon(weapon);
        
    }

    void CreateAccessories(string prefabId, CharacterAvater  characterAvater)
    {
        var accesories = Managers.Spawn.AccessoriesSpawn(prefabId);
        if (accesories == null) return;
        characterAvater.SetupAccessories(accesories);
    }

    #endregion

    void ShowAvater(int index)
    {
        _currentIndex = index;
        _currentCharacterAvater?.gameObject.SetActive(false);
        _currentCharacterAvater = _avaterList[index];
        _currentCharacterAvater?.gameObject.SetActive(true);
        _avaterCountText.text = $"{index+1}/{_avaterList.Count}";
        CheckState();
    }

    void CheckState()
    {
        var useAvaterIndex = PlayerInfo.userData.GetCurrentAvaterIndex();
        var stateText = _stateButton.GetComponentInChildren<TextMeshProUGUI>();
        if(useAvaterIndex == _currentIndex)
        {
            _stateButton.interactable = false;
            stateText.text = "사용 중";
        }
        else
        {
            _stateButton.interactable = true;
            stateText.text = "사용 가능";
        }
    }


    #region 버튼 이벤트
    public void LeftClick()
    {
        var newIndex = _currentIndex - 1 >= 0 ? _currentIndex - 1 : _avaterList.Count - 1;
        ShowAvater(newIndex);
    }

    public void RightClick()
    {
        var newIndex = _currentIndex + 1 >= _avaterList.Count ? 0: _currentIndex + 1;
        ShowAvater(newIndex);
    }

    public void OnClick_CharacterSkin()
    {
        var currentSkin = _currentCharacterAvater;
        var newSkinID = Managers.ProductSetting.GetRandomSkinName(Define.ProductType.Character, _currentCharacterAvater.gameObject.name);
        var newSkin = Managers.Spawn.AvaterSpawn(newSkinID).GetComponent<CharacterAvater>();

        
        if (currentSkin)
        {
            newSkin.SetupWeapon(currentSkin.weaponObject);
            newSkin.SetupAccessories(currentSkin.accessoriesObject);
            Managers.Resource.Destroy(currentSkin.gameObject);
        }
        newSkin.transform.ResetTransform(_characterPanel);
        newSkin.gameObject.SetActive(true);

        _avaterList[_currentIndex] = newSkin;
        _currentCharacterAvater = newSkin;

        PlayerInfo.userData.avaterList[_currentIndex].characterAvaterKey = newSkin.gameObject.name;
        PlayerInfo.SaveUserData();

    }

    public void OnClick_WeaponSkin()
    {
        var currentSkin = _currentCharacterAvater.weaponObject;
        var newSkinID = Managers.ProductSetting.GetRandomSkinName(Define.ProductType.Hammer, _currentCharacterAvater.weaponObject.name);
        var newSkin = Managers.Spawn.WeaponSkinSpawn(newSkinID);

        if (currentSkin)
        {
            _currentCharacterAvater.SetupWeapon(newSkin.gameObject);
            Managers.Resource.Destroy(currentSkin.gameObject);
        }
    }

    public void OnClick_Accsseories()
    {

    }
    public void OnClick_AddSlot()
    {
        _avaterList.Add(null);


        _avaterCountText.text = $"{_currentIndex + 1}/{_avaterList.Count}";

        PlayerInfo.userData.avaterList.Add(null);
        PlayerInfo.SaveUserData();

    }

    public void OnClick_StateButton()
    {
        var sucess = PlayerInfo.userData.UseNewCharacterAvater(_currentIndex);
        if (sucess)
        {
            CheckState();
        }
        else
        {

        }
    }

    #endregion
}
