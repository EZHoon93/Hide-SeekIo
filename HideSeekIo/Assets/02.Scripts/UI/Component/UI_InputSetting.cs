using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class UI_InputSetting : MonoBehaviour
{

    [SerializeField] Slider _sizeSlider;
    [HideInInspector]
    public UltiMateSetting _currentUltimateUI;
    public Slider sizeSlider => _sizeSlider;
     [SerializeField] UltiMateSetting[] settingList;

    private void Reset()    
    {
        settingList = GetComponentsInChildren<UltiMateSetting>();
    }
    
    public void Init()
    {   
        foreach(var settingData in PlayerInfo.optionData.joystickSettings)
        {
            foreach(var setting in settingList)
            {
                if (string.Equals(settingData.joystickName, setting._controllerName))
                {
                    setting.SetupByData(settingData);
                }
            }
        }
    }

    public void Save()
    {
        foreach (var settingData in PlayerInfo.optionData.joystickSettings)
        {
            foreach (var setting in settingList)
            {
                if (string.Equals(settingData.joystickName, setting._controllerName))
                {
                    settingData.size = setting.size;
                    settingData.vector2 = setting.Vector2;
                    setting.SetupByData(settingData);
                }
            }
        }

        PlayerInfo.SaveOptionData();

    }

    public void ResetSetting()
    {
        foreach (var settingData in UISetting.Instance.inputUIInfos)
        {
            foreach (var setting in settingList)
            {
                if (string.Equals(settingData.joystickName, setting._controllerName))
                {
                    setting.SetupByData(settingData);
                }
            }
        }

    }

    public void SetActive(bool active)
    {
        foreach (var s in settingList)
        {
            s.enabled = active;
            var joystick = s.GetComponent<UltimateJoystick>();
            if (joystick)
            {
                joystick.enabled = !active;
            }
            var button = s.GetComponent<UltimateButton>();
            if (button)
            {
                button.enabled = !active;
            }
            s.gameObject.SetActive(active);
        }
        _sizeSlider.gameObject.SetActive(active);
        var canvas = this.GetComponent<Canvas>();
        canvas.sortingOrder = active == true ? 100 : 0;
        

    }
    public void ChangeSlider()
    {
        if (_currentUltimateUI.size == _sizeSlider.value) return;
        _currentUltimateUI.ChangeSize(_sizeSlider.value);
    }

    public void SetupEdit(UltiMateSetting ultiMateSetting)
    {
        _currentUltimateUI = ultiMateSetting;
        _sizeSlider.value = _currentUltimateUI.size;
    }
}
