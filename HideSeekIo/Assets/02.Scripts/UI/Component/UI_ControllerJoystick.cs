using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class UI_ControllerJoystick : MonoBehaviour
{
    [SerializeField] Image _buttonImage;
    [SerializeField] Image _itemImage;
    [SerializeField] UI_CoolTime _uiCoolTime;
    [SerializeField] InputType _inputType;
    [SerializeField] Color _precssButtonColor;

    public InputType inputType => _inputType;
    public Vector2 InputVector2 { get; private set; }
    public ControllerInput controllerInput;
    public UI_CoolTime UI_CoolTime => _uiCoolTime;
    public Image itemImage => _itemImage;

    UltimateJoystick _ultimateJoystick;
    Define.AttackType _attackType;
    Color _orignalButtonColor;

    public void Init()
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
        //Debug.Log("DownCall1");
        if (_attackType == Define.AttackType.Button)
        {
            _buttonImage.color = _precssButtonColor;
        }
    }

    void Up()
    {
 
        if (controllerInput == null) return;
        if (InputVector2.magnitude > 0)
        {
            controllerInput.Call(ControllerInputType.Up, InputVector2);
        }
        InputVector2 = Vector2.zero;
        if (_attackType == Define.AttackType.Button)
        {
            _buttonImage.color = Color.white;
        }

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
        //UI_CoolTime?.ResetCoolTime();
    }

    public void SetupItemImage(Sprite newSprite)
    {
        _itemImage.sprite = newSprite;
    }

    public void RemoveUI(ControllerInput removeControllerInput)
    {
        if(controllerInput == removeControllerInput)
        {
            controllerInput = null;
            this.gameObject.SetActive(false);
        }
    }

 
    public void SetActiveControllerType(Define.AttackType attackType ,Sprite sprite = null)
    {
        _attackType = attackType;
        if (attackType == Define.AttackType.Button)
        {
            _ultimateJoystick.joystick.gameObject.SetActive(false);
            _buttonImage?.gameObject.SetActive(true);
            _itemImage.transform.ResetTransform(_buttonImage.transform);
            _ultimateJoystick.dynamicPositioning = false;
            //_ultimateJoystick.inputTransition = false;
            //GetComponent<CanvasGroup>().alpha = 1;
        }
        else
        {
            _itemImage?.transform.ResetTransform(_ultimateJoystick.joystick.transform);
            _ultimateJoystick.joystick.gameObject.SetActive(true);
            _buttonImage?.gameObject.SetActive(false);
            _ultimateJoystick.dynamicPositioning = true;
            //_ultimateJoystick.inputTransition = true;
            //GetComponent<CanvasGroup>().alpha = _ultimateJoystick.fadeUntouched;

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
