using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuyResult : UI_Popup
{
    [SerializeField] GameObject _successPanel;
    [SerializeField] GameObject _failedPanel;

    [SerializeField] Button _confirmButton;
    public override void Init()
    {
        base.Init();
        _confirmButton.onClick.AddListener(OnClick_Confirm);
    }

    void OnClick_Confirm()
    {
        Managers.UI.ClosePopupUI(this);
    }

    public void SetupSucess(bool isSucess)
    {
        _successPanel.SetActive(isSucess);
        _failedPanel.SetActive(!isSucess);
    }
}
