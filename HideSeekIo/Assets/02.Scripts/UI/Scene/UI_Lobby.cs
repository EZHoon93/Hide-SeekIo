using UnityEngine;
using UnityEngine.UI;

public class UI_Lobby : UI_Scene
{
    [SerializeField] Button _gmaeSelectButton;
    [SerializeField] Button _storeButton;
    [SerializeField] Button _optionButton;



    public override void Init()
    {
        base.Init();

        _gmaeSelectButton.onClick.AddListener(OnButtonClick_GameSelect);

        _optionButton.onClick.AddListener(OnButtonClick_Option);
        _storeButton.onClick.AddListener(OnButtonClick_Store);

    }

    void OnButtonClick_GameSelect()
    {
        Managers.UI.ShowPopupUI<UI_GameSelect>();
    }

    void OnButtonClick_Store()
    {
        Managers.UI.ShowPopupUI<UI_Store>();
    }
    void OnButtonClick_Option()
    {
        Managers.UI.ShowPopupUI<UI_Setting>();
    }
}
