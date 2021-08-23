
using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

class UI_Product: MonoBehaviour
{
    [SerializeField] Image _characterImage;
    [SerializeField] TextMeshProUGUI _priceText;

    string _key;
    Sprite _sprite;
    int _price;
    Button _button;
    event Action _clickCallBackEvent;
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ClickButton);
    }

    public void Setup(string key, int price, Sprite sprite, Action clickCallBackEvent = null)
    {
        _key = key;
        _characterImage.sprite = sprite;
        _price = price;
        _priceText.text = _price.ToString();
        _clickCallBackEvent = clickCallBackEvent;
    }

    void ClickButton()
    {
        _clickCallBackEvent?.Invoke();
    }
}
