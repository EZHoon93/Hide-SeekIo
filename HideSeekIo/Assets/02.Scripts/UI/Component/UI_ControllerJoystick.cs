using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class UI_ControllerJoystick : MonoBehaviour
{
    [SerializeField] ObtainableItem _obtainableItem;
    public Vector2 InputVector2 { get; private set; }
    public UltimateJoystick _ultimateJoystick;
    public MyInput myInput;

    private void Awake()
    {
        _ultimateJoystick = GetComponent<UltimateJoystick>();
        _ultimateJoystick.OnPointerDownCallback += Down;
        _ultimateJoystick.OnDragCallback += Drag;
        _ultimateJoystick.OnPointerUpCallback += Up;
        _ultimateJoystick.OnTapCallBack += Tap;
        SetActiveControllerType(Define.ControllerType.Button);
    }
#if UNITY_ANDROID

#endif

    void Down()
    {
        print("Down!!");
        if (myInput == null) return;
        InputVector2 = GetInputVector2();
        myInput.ControllerDic[ControllerInputType.Drag]?.Invoke(InputVector2);
    }

    void Up()
    {
        if (myInput == null) return;
        if (InputVector2.magnitude >= _ultimateJoystick.deadZone)
        {
            myInput.ControllerDic[ControllerInputType.Up]?.Invoke(InputVector2);
        }
        InputVector2 = Vector2.zero;
    }

    void Drag()
    {
        if (myInput == null) return;
        InputVector2 = GetInputVector2();
        myInput.ControllerDic[ControllerInputType.Drag]?.Invoke(InputVector2);
    }

    void Tap()
    {
        if (myInput == null) return;
        myInput.ControllerDic[ControllerInputType.Tap]?.Invoke(Vector2.zero);
    }

    Vector2 GetInputVector2()
    {
        return new Vector2(_ultimateJoystick.GetHorizontalAxis(), _ultimateJoystick.GetVerticalAxis());
    }

    public void SetActiveControllerType(Define.ControllerType controllerType)
    {
        if (controllerType == Define.ControllerType.Button)
        {
            _ultimateJoystick.joystick.gameObject.SetActive(false);
            if (_obtainableItem)
            {
                _ultimateJoystick.joystick.GetComponent<Image>().sprite = _obtainableItem.itemSprite;
            }
        }
        else
        {
            _ultimateJoystick.joystick.gameObject.SetActive(true);

        }
    }
    ////즉시아이템 사용
    //void Click()
    //{
    //    onTapEventCallBack?.Invoke();
    //    var myPlayer = Managers.Game.myPlayer;
    //    if (myPlayer == null || _inGameItem == null) return;
    //    if (_inGameItem.useType == Define.UseType.Item)
    //    {
    //        _inGameItem.Use(myPlayer);
    //        RemoveItem();
    //        this.gameObject.SetActive(false);
    //    }
    //}
    public void AddItem(ObtainableItem newItem)
    {
        //_obtainableItem = newItem;
        //if (newItem.useType== Define.UseType.Item)
        //{
        //    _joystickOutLine.enabled = false;
        //    _itemOutLine.enabled = true;
        //    _itemImage.transform.SetParent(_itemOutLine.transform);
        //}
        //else
        //{
        //    _joystickOutLine.enabled = true;
        //    _itemOutLine.enabled = false;
        //    _itemImage.transform.SetParent(_joystickOutLine.transform);
        //}
        //_joystickOutLine.transform.localPosition = Vector3.zero;
        //_itemImage.transform.localPosition = Vector3.zero;
        //_itemImage.sprite = newItem.itemSprite;
        //_itemImage.enabled = true;
        //this.gameObject.SetActive(true);
        ////_ultimateJoystick.OnDragCallback
    }

    public void RemoveItem()
    {
        print("Remove ITMe11");
        this.gameObject.SetActive(false);
    }

}
