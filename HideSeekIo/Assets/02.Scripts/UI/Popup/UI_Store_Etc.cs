using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Store_Etc : UI_Popup
{
    [SerializeField] TMP_InputField _InputField;
    enum Buttons
    {
        ChangeNick,
        Coin1,
        Coin2,
        RemoveAD,
        AddSkin
    }

    enum Texts
    {
        AvaterCount,
        StateTxt
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        GetButton((int)Buttons.ChangeNick).gameObject.BindEvent(ChangeNickName);
        GetButton((int)Buttons.Coin1).gameObject.BindEvent(Coin1);
        GetButton((int)Buttons.Coin2).gameObject.BindEvent(Coin2);
        GetButton((int)Buttons.RemoveAD).gameObject.BindEvent(RemoveAD);
        GetButton((int)Buttons.AddSkin).gameObject.BindEvent(AddSkin);

    }
    private void Start()
    {
        PhotonGameManager.Instacne.gameJoin += Destroy;
        _InputField.text = PlayerInfo.nickName;
    }

    void Destroy()
    {
        PhotonGameManager.Instacne.gameJoin -= Destroy;
        Managers.Resource.Destroy(this.gameObject);
    }

    void ChangeNickName(PointerEventData pointerEventData)
    {
        Managers.UI.ShowPopupUI<UI_Check_Buy>().Setup("스킨", () => {
        });
    }

    void Coin1(PointerEventData pointerEventData)
    {
        Managers.UI.ShowPopupUI<UI_Check_Buy>().Setup("스킨", () => {
        });
    }

    void Coin2(PointerEventData pointerEventData)
    {
        Managers.UI.ShowPopupUI<UI_Check_Buy>().Setup("스킨", () => {
        });
    }

    void RemoveAD(PointerEventData pointerEventData)
    {
        Managers.UI.ShowPopupUI<UI_Check_Buy>().Setup("스킨", () => {
        });
    }

    void AddSkin(PointerEventData pointerEventData)
    {
        Managers.UI.ShowPopupUI<UI_Check_Buy>().Setup("스킨", () => {

        });
    }
}
