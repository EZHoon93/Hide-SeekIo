using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_Setting : UI_Popup
{
    [SerializeField] Slider _bgmSlider;
    [SerializeField] Slider _sfxSlider;

    [SerializeField] Button _confirmButton;
    [SerializeField] Button _cancelButton;
    [SerializeField] Button _joystickEditButton;
    [SerializeField] Button _languageButton;


    public override void Init()
    {
        base.Init();

        _bgmSlider.value = PlayerInfo.optionData.bgmValue;
        _sfxSlider.value = PlayerInfo.optionData.soundValue;

        _confirmButton.onClick.AddListener(OnClick_Confirm);
        _cancelButton.onClick.AddListener(OnClick_Cancel);
        _joystickEditButton.onClick.AddListener(OnClick_JoystickEdit);
        _languageButton.onClick.AddListener(OnClick_Language);


    }

   
    public void ChangeBgmValue()
    {
        Managers.Sound.ChangeBgmValue(_bgmSlider.value);
    }

    public void ChangeSfxValue()
    {
        Managers.Sound.ChangeSfxValue(_sfxSlider.value);

    }

    /// <summary>
    /// 확인 누르면 데이터저장
    /// </summary>
    void OnClick_Confirm()
    {
        Managers.UI.ClosePopupUI();
        PlayerInfo.optionData.soundValue = _sfxSlider.value;
        PlayerInfo.optionData.bgmValue = _bgmSlider.value;
        PlayerInfo.SaveOptionData();
    }

    void OnClick_Cancel()
    {
        Managers.UI.ClosePopupUI();
    }

    void OnClick_JoystickEdit()
    {
        Managers.UI.ShowPopupUI<UI_JoystickEdit>();
    }

    void OnClick_Language()
    {

    }
}
