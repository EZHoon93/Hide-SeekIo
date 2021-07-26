using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_Setting : UI_Popup
{
    [SerializeField] Transform _optionPanel;
    [SerializeField] Transform _editPanel;

    enum Buttons
    {
        Confirm,
        Cancel,
        Joystick,
        SaveEdit,
        CancelEdit,
        Reset,
        BgmOn,
        BgmOff,
        SfxOn,
        SfxOff
    }


    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.Confirm).gameObject.BindEvent(Confirm);
        GetButton((int)Buttons.Cancel).gameObject.BindEvent(Cancel);
        GetButton((int)Buttons.Reset).gameObject.BindEvent(Click_Reset);
        GetButton((int)Buttons.Joystick).gameObject.BindEvent(Click_EditJoysitck);
        GetButton((int)Buttons.SaveEdit).gameObject.BindEvent(Click_SaveEdit);
        GetButton((int)Buttons.CancelEdit).gameObject.BindEvent(Click_CancelEdit);

        GetButton((int)Buttons.BgmOn).gameObject.BindEvent(Click_OnBgm);
        GetButton((int)Buttons.BgmOff).gameObject.BindEvent(Click_OffBgm);
        GetButton((int)Buttons.SfxOn).gameObject.BindEvent(Click_OnSfx);
        GetButton((int)Buttons.SfxOff).gameObject.BindEvent(Click_OffSfx);



        SetActiveOptionMode(true);
    }
    private void OnEnable()
    {
   
        GetButton((int)Buttons.BgmOn).gameObject.SetActive(PlayerInfo.optionData.bgmValue);
        GetButton((int)Buttons.BgmOff).gameObject.SetActive(!PlayerInfo.optionData.bgmValue);
        GetButton((int)Buttons.SfxOn).gameObject.SetActive(PlayerInfo.optionData.soundValue);
        GetButton((int)Buttons.SfxOff).gameObject.SetActive(!PlayerInfo.optionData.soundValue);

     }
   

    void SetActiveOptionMode(bool active)
    {
        _optionPanel.gameObject.SetActive(active);
        _editPanel.gameObject.SetActive(!active);

    }

    void Confirm(PointerEventData pointerEventData)
    {
        Managers.UI.ClosePopupUI();
        Managers.UI.SceneUI.GetComponent<UI_Main>().ChangePanel(Define.GameScene.Lobby);
    }

    void Cancel(PointerEventData pointerEventData)
    {
        Managers.UI.ClosePopupUI();
    }

    void Click_EditJoysitck(PointerEventData pointerEventData)
    {
        SetActiveOptionMode(false);


        var inputSetting = InputManager.Instance.GetComponent<UI_InputSetting>();
        if (inputSetting)
        {
            inputSetting.SetActive(true);
        }
        Managers.UI.SceneUI.gameObject.SetActive(false);
    }

    void Click_Reset(PointerEventData pointerEventData)
    {
        var inputSetting = InputManager.Instance.GetComponent<UI_InputSetting>();
        if (inputSetting)
        {
            inputSetting.ResetSetting();
        }
    }

    void Click_CancelEdit(PointerEventData pointerEventData)
    {
        SetActiveOptionMode(true);
        var inputSetting = InputManager.Instance.GetComponent<UI_InputSetting>();
        if (inputSetting)
        {
            inputSetting.Init();
            inputSetting.SetActive(false);
        }

        Managers.UI.SceneUI.gameObject.SetActive(true);

    }
    void Click_SaveEdit(PointerEventData pointerEventData)
    {
        SetActiveOptionMode(true);
        var inputSetting = InputManager.Instance.GetComponent<UI_InputSetting>();
        if (inputSetting)
        {
            inputSetting.Save();
            inputSetting.SetActive(false);
        }

        Managers.UI.SceneUI.gameObject.SetActive(true);
    }


    void Click_OnBgm(PointerEventData pointerEventData)
    {
        GetButton((int)Buttons.BgmOn).gameObject.SetActive(false);
        GetButton((int)Buttons.BgmOff).gameObject.SetActive(true);

        PlayerInfo.optionData.bgmValue = GetButton((int)Buttons.BgmOn).gameObject.activeSelf;
        PlayerInfo.SaveOptionData();
    }

    void Click_OffBgm(PointerEventData pointerEventData)
    {
        GetButton((int)Buttons.BgmOn).gameObject.SetActive(true);
        GetButton((int)Buttons.BgmOff).gameObject.SetActive(false);
        PlayerInfo.optionData.bgmValue = GetButton((int)Buttons.BgmOn).gameObject.activeSelf;
        PlayerInfo.SaveOptionData();
    }

    void Click_OnSfx(PointerEventData pointerEventData)
    {
        GetButton((int)Buttons.SfxOn).gameObject.SetActive(false);
        GetButton((int)Buttons.SfxOff).gameObject.SetActive(true);
        PlayerInfo.optionData.soundValue = GetButton((int)Buttons.SfxOn).gameObject.activeSelf;
        PlayerInfo.SaveOptionData();
    }

    void Click_OffSfx(PointerEventData pointerEventData)
    {
        GetButton((int)Buttons.SfxOn).gameObject.SetActive(true);
        GetButton((int)Buttons.SfxOff).gameObject.SetActive(false);
        PlayerInfo.optionData.soundValue = GetButton((int)Buttons.SfxOn).gameObject.activeSelf;
        PlayerInfo.SaveOptionData();
    }
}
