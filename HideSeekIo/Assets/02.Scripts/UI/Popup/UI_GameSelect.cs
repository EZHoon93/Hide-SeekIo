using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UI_GameSelect : UI_Popup
{
    [SerializeField] Toggle _itemToggle;
    [SerializeField] Toggle _scretToggle;
    [SerializeField] TMP_InputField _roomInput;
    
    public override void Init()
    {
        base.Init();
    }
    public void OnButtonClick_Confirm()
    {
        var roomName = _roomInput.text.ToString();
        var isScret = _scretToggle.isOn;
        var itemMode = _itemToggle.isOn ? Define.GameMode.Item : Define.GameMode.Object;
        Managers.photonManager.SetupRoomInfo(itemMode, roomName, isScret);

        Managers.Scene.LoadScene(Define.Scene.Loading);

    }

    public void OnButtonClick_Cancel()
    {
        ClosePopupUI();
    }
}
