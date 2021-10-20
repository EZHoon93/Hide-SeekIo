using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class UI_ControllerJoystick : MonoBehaviour
{
    [SerializeField] Image _itemImage;
    [SerializeField] UI_CoolTime _uiCoolTime;
    public InputType inputType;
    public Vector2 InputVector2 { get; private set; }
    public UltimateJoystick _ultimateJoystick { get; private set; }
    public ControllerInput controllerInput { get; set; }

    public UI_CoolTime UI_CoolTime => _uiCoolTime;

    float mag;




    private void Awake()
    {
        _ultimateJoystick = GetComponent<UltimateJoystick>();
        _ultimateJoystick.OnPointerDownCallback += Down;
        _ultimateJoystick.OnDragCallback += Drag;
        _ultimateJoystick.OnPointerUpCallback += Up;
        _ultimateJoystick.OnTapCallBack += Tap;
    }
#if UNITY_ANDROID

#endif

    void Down()
    {
        if (controllerInput == null) return;
        InputVector2 = GetInputVector2();
        controllerInput.Call(ControllerInputType.Down, InputVector2);
    }

    void Up()
    {

        if (controllerInput == null) return;
        if (InputVector2.magnitude > 0)
        {
            controllerInput.Call(ControllerInputType.Up, InputVector2);
        }
        InputVector2 = Vector2.zero;
    }
    void Drag()
    {
        if (controllerInput == null) return;
        var inputVector2 = GetInputVector2();

        float mag = Mathf.Clamp( inputVector2.magnitude - 0.2f , 0,1);
        InputVector2 = inputVector2.normalized * mag;
        

        controllerInput.Call(ControllerInputType.Drag, InputVector2);
    }
    void Tap()
    {
        if (controllerInput == null) return;
        controllerInput.Call(ControllerInputType.Tap, InputVector2);
    }

    Vector2 GetInputVector2()
    {
        var inputVector2 = new Vector2(_ultimateJoystick.GetHorizontalAxis(),_ultimateJoystick.GetVerticalAxis());
        var result = UtillGame.GetInputVector2_ByCamera(inputVector2);

        return result;
    }
  
    public void StartCoolTime(float coolTime)
    {
        _uiCoolTime.StartCoolTime(coolTime);
    }
    public void ResetUIController()
    {
        InputVector2 = Vector2.zero;
        _ultimateJoystick.joystick.localPosition = Vector3.zero;
        UI_CoolTime?.ResetCoolTime();
    }

    public void SetActiveControllerType(Define.AttackType attackType ,Sprite sprite = null)
    {
        if(attackType == Define.AttackType.Button)
        {
            _ultimateJoystick.joystick.gameObject.SetActive(false);
            _itemImage.transform.ResetTransform(_ultimateJoystick.joystickBase.transform);
        }
        else
        {
            _itemImage.transform.ResetTransform(_ultimateJoystick.joystick.transform);
            var joystick = _ultimateJoystick.joystick;
            joystick.gameObject.SetActive(true);
            //joystick.GetComponent<Image>().enabled = true;
        }
        if (sprite)
        {
            _itemImage.sprite = sprite;
            _itemImage.enabled = true;
        }
        else
        {
            //_itemImage.enabled = false;
        }
        this.gameObject.SetActive(true);
    }


    
}
