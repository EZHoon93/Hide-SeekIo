
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChangeNickName : UI_Popup
{
    [SerializeField] Button _confirmButton;
    [SerializeField] Button _cancelButton;
    [SerializeField] TextMeshProUGUI _nickNameText;

    int _price = 0;
    public override void Init()
    {
        base.Init();
        _confirmButton.onClick.AddListener(OnClick_ConfirmNickName);
        _cancelButton.onClick.AddListener(OnClick_Cancel);

    }


    void OnClick_ConfirmNickName()
    {
        bool isSucess = false;
        if(PlayerInfo.userData.coin  >= _price)
        {
            PlayerInfo.userData.nickName = _nickNameText.text.ToString();
            PlayerInfo.SaveUserData();
            isSucess = true;
        }
        Managers.UI.ClosePopupUI(this);
        Managers.UI.ShowPopupUI<UI_BuyResult>().SetupSucess(isSucess);
    }

    void OnClick_Cancel()
    {
        Managers.UI.ClosePopupUI(this);
    }

    public void SetupNickName(string nickName)
    {
        _nickNameText.text = nickName;
    }

}
