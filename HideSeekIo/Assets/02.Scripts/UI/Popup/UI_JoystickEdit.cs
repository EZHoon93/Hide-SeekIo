using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_JoystickEdit : UI_Popup
{
    [SerializeField] Button _saveButton;
    [SerializeField] Button _cancelButton;
    [SerializeField] Button _resetButton;


    public override void Init()
    {
        base.Init();
        _saveButton.onClick.AddListener(OnClick_Save);
        _cancelButton.onClick.AddListener(OnClick_Cancel);
        _resetButton.onClick.AddListener(OnClick_Reset);
    }

    private void OnEnable()
    {
        var inputSetting = Managers.Input.GetComponent<UI_InputSetting>();
        if (inputSetting)
        {
            inputSetting.SetActive(true);
        }
    }

    void OnClick_Cancel()
    {
        var inputSetting = Managers.Input.GetComponent<UI_InputSetting>();
        if (inputSetting)
        {
            inputSetting.Save();
            inputSetting.SetActive(false);
        }
        Managers.UI.ClosePopupUI(this);
    }
    void OnClick_Reset()
    {
        var inputSetting = Managers.Input.GetComponent<UI_InputSetting>();
        if (inputSetting)
        {
            inputSetting.ResetSetting();
        }
    }

    void OnClick_Save()
    {
        var inputSetting = Managers.Input.GetComponent<UI_InputSetting>();
        if (inputSetting)
        {
            inputSetting.Save();
            inputSetting.SetActive(false);
        }
        Managers.UI.ClosePopupUI(this);
    }

}
