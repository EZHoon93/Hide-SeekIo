using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UI_Store : UI_Popup
{
    [SerializeField] TMP_InputField _nickNameInput;
    [SerializeField] Button _editNickNameButton;

    [SerializeField] Button _productCoint1;
    [SerializeField] Button _productCoint2;
    [SerializeField] Button _productCoint3;
    [SerializeField] Button _productRemoveAD;
    [SerializeField] Button _producAddCoin;
    [SerializeField] Button _producAddExp;
    public override void Init()
    {
        base.Init();

        _editNickNameButton.onClick.AddListener(OnClick_EditNickName);
    }

    private void OnEnable()
    {
        SetupNickName();
    }
    void SetupNickName()
    {
        _nickNameInput.text = PlayerInfo.userData.nickName;
    }

    void OnClick_EditNickName()
    {
        var checkUI = Managers.UI.ShowPopupUI<UI_ChangeNickName>();
        checkUI.SetupNickName(_nickNameInput.text);
    }
}
