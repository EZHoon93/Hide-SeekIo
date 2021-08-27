using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class UI_ControllerJoystick : MonoBehaviour
{
    [SerializeField] Image _itemImage;
    public InputType inputType;

    public Vector2 InputVector2 { get; private set; }
    public UltimateJoystick _ultimateJoystick { get; private set; }
    public MyInput myInput { get; set; }

    public UI_Slider_CoolTime _UI_Slider_CoolTime { get; set; }


    private void Awake()
    {
        _ultimateJoystick = GetComponent<UltimateJoystick>();
        _ultimateJoystick.OnPointerDownCallback += Down;
        _ultimateJoystick.OnDragCallback += Drag;
        _ultimateJoystick.OnPointerUpCallback += Up;
        _ultimateJoystick.OnTapCallBack += Tap;
        _UI_Slider_CoolTime = GetComponentInChildren<UI_Slider_CoolTime>();
    }
#if UNITY_ANDROID

#endif

    void Down()
    {
        if (myInput == null) return;
        InputVector2 = GetInputVector2();
        myInput.Call(ControllerInputType.Down, InputVector2);
    }

    void Up()
    {

        if (myInput == null) return;
        if(inputType == InputType.Move)
        {
            InputVector2 = Vector2.zero;
            myInput.Call(ControllerInputType.Up, InputVector2);
        }
        else
        {
            if (InputVector2.magnitude >= _ultimateJoystick.deadZone)
            {
                myInput.Call(ControllerInputType.Up, InputVector2);
            }
        }
        InputVector2 = Vector2.zero;
    }

    void Drag()
    {
        if (myInput == null) return;
        InputVector2 = GetInputVector2();
        myInput.Call(ControllerInputType.Drag, InputVector2);

    }

    void Tap()
    {
        if (myInput == null) return;
        myInput.Call(ControllerInputType.Tap, InputVector2);

    }

    Vector2 GetInputVector2()
    {
        var inputVector2 = new Vector2(_ultimateJoystick.GetHorizontalAxis(),_ultimateJoystick.GetVerticalAxis());
        var result = UtillGame.GetInputVector2_ByCamera(inputVector2);
        return result;
    }

    public void SetActiveControllerType(Define.AttackType attackType ,Sprite sprite = null)
    {
        print("SEtaCtivoe " + this.gameObject.name);
        //if (controllerType == Define.ControllerType.Button)
        //{
        //    if (newObtainableItem)
        //    {
        //         _itemImage.sprite = newObtainableItem.itemSprite;
        //        _itemImage.transform.localPosition = Vector3.zero;
        //        _itemImage.enabled = true;
        //    }
        //    else
        //    {
        //        _itemImage.enabled = false;
        //    }
        //    _itemImage.transform.SetParent(_ultimateJoystick.joystickBase);
        //    _ultimateJoystick.joystick.gameObject.SetActive(false);
        //}
        //else
        //{
        //    if (newObtainableItem)
        //    {
        //        _itemImage.sprite = newObtainableItem.itemSprite;
        //        _ultimateJoystick.joystick.transform.localPosition = Vector3.zero;
        //        _itemImage.transform.localPosition = Vector3.zero;
        //        _itemImage.enabled = true;
        //    }
        //    else
        //    {
        //        _itemImage.enabled = false;
        //    }
        //    _ultimateJoystick.joystick.gameObject.SetActive(true);
        //    _itemImage.transform.SetParent(_ultimateJoystick.joystick);
        //}
        if(attackType == Define.AttackType.Button)
        {
            _ultimateJoystick.joystick.gameObject.SetActive(false);
        }
        else
        {
            _ultimateJoystick.joystick.gameObject.SetActive(true);
        }
     
        this.gameObject.SetActive(true);
    }

}
