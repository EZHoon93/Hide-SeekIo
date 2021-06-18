using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame_Item : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _priceText;
    [SerializeField] Image _itemImage;
    [SerializeField] Image _coolTimeImage;

    Button _button;
    IEnumerator _coolTimeEnumerator;


    event Define.GameItemStateCallBack _itemCallBackEvent;  //콜백이벤트 리턴값은  마지막 리턴값에 결정. or 처음값?

    void Awake()
    {
        _button = GetComponentInChildren<Button>();
        _button.onClick.AddListener(() => Click());
    }
    private void OnEnable()
    {
        _coolTimeImage.enabled = false;
    }

    public void Setup(Sprite sprite, int price, Define.GameItemStateCallBack itemStateCallBack = null)
    {
        _priceText.text = price.ToString();
        _itemImage.sprite = sprite;
        _itemCallBackEvent = itemStateCallBack;
    }

    void Click()
    {
        switch (_itemCallBackEvent.Invoke())
        {
            case Define.InGameItemUIState.Sucess:
                _button.interactable = false;
                break;
            case Define.InGameItemUIState.SucessRecycle:
                Util.ImageFillAmount(_coolTimeImage, ref _coolTimeEnumerator, 1.0f);    //쿨타임
                break;
            case Define.InGameItemUIState.Failed:
                //아무것도 x
                break;
        }

    }

    
}
