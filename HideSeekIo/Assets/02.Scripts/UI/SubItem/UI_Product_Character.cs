using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class UI_Product_Character : MonoBehaviour
{
    Button _clickButton;
    string _key;
    [SerializeField] Image _image;

    private void Awake()
    {
        _clickButton = GetComponentInChildren<Button>();
        _clickButton.onClick.AddListener(() => ClickBuyButton());
            
    }
    public void Setup(string key, Sprite sprite)
    {
        _key = key;
        _image.sprite = sprite;
    }

    void ClickBuyButton()
    {
        if (string.IsNullOrEmpty(_key)) return;
         var popup = Managers.UI.ShowPopupUI<UI_Check_Buy>();
        popup.Setup(_key);
    }
}
