using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class UI_ControllerJoystick : MonoBehaviour
{
    [SerializeField] IItem _inGameItem;
    [SerializeField] Image _joystickOutLine;
    [SerializeField] Image _itemOutLine;
    [SerializeField] Image _itemImage;
    public Vector2 InputVector2 { get; private set; }

    public  Action<Vector2> onAttackEventCallBack;
    public UltimateJoystick _ultimateJoystick;
    private void Awake()
    {
        _ultimateJoystick = GetComponent<UltimateJoystick>();
        _ultimateJoystick.OnDragCallback += Down;
        _ultimateJoystick.OnPointerUpCallback += Up;
        _ultimateJoystick.OnTapCount += Click;

    }
#if UNITY_ANDROID

#endif

    void Down()
    {

        InputVector2 = new Vector2(_ultimateJoystick.GetHorizontalAxis(), _ultimateJoystick.GetVerticalAxis());

    }

    void Up()
    {
        if (InputVector2.magnitude >= _ultimateJoystick.deadZone)
        {
            onAttackEventCallBack?.Invoke(InputVector2);
        }
        InputVector2 = Vector2.zero;
    }

  
    //즉시아이템 사용
    void Click()
    {
        print("Click");
        //var myPlayer = Managers.Game.myPlayer;
        //if (myPlayer == null || _inGameItem == null) return;
        //if (_inGameItem._item_Base.useType == Item_Base.UseType.Item)
        //{
        //    _inGameItem.Use(myPlayer);
        //    this.gameObject.SetActive(false);

        //}

    }
    public void AddItem(IItem newItem)
    {
        _inGameItem = newItem;
        if (newItem.useType== Define.UseType.Item)
        {
            print("아잉템타입");
            _joystickOutLine.enabled = false;
            _itemOutLine.enabled = true;
            _itemImage.transform.SetParent(_itemOutLine.transform);
        }
        else
        {
            print("아잉템타입ㄴㄴㄴㄴㄴㄴㄴ");

            _joystickOutLine.enabled = true;
            _itemOutLine.enabled = false;
            _itemImage.transform.SetParent(_joystickOutLine.transform);
        }
        _itemImage.transform.localPosition = Vector3.zero;
        _itemImage.sprite = newItem.GetSprite();
        _itemImage.enabled = true;
        this.gameObject.SetActive(true);
        //_ultimateJoystick.OnDragCallback
    }

    public void RemoveItem()
    {
        this.gameObject.SetActive(false);
    }

}
