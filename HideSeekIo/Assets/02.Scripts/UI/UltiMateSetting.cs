using System.Collections;

using Data;

using UnityEngine;
using UnityEngine.EventSystems;

public class UltiMateSetting : MonoBehaviour, IPointerDownHandler , IDragHandler , IPointerUpHandler
{
    public float size
    {
        get
        {
            if (_type == Type.Button)
            {
                return UltimateButton.GetUltimateButton(_controllerName).buttonSize;
            }
            else
            {
                return UltimateJoystick.GetUltimateJoystick(_controllerName).joystickSize;
            }
        }
    }
    public string _controllerName;
    public Vector2 Vector2
    {
        get
        {
            if (_type == Type.Button)
            {
                return new Vector2(UltimateButton.GetUltimateButton(_controllerName).positionHorizontal, UltimateButton.GetUltimateButton(_controllerName).positionVertical);
            }
            else
            {
                return new Vector2(UltimateJoystick.GetUltimateJoystick(_controllerName).positionHorizontal,UltimateJoystick.GetUltimateJoystick(_controllerName).positionVertical);
            }
        }
        
    }
    [SerializeField] RectTransform canvasRectTrans;
    public enum Type{
        Joystick,
        Button
    }

     [SerializeField] Type _type;
    public  enum Anchor
    {
        Left,
        Right
    }
    [SerializeField] Anchor anchor;

    public enum ScalingAxis
    {
        Height,
        Width
    }
    [SerializeField] ScalingAxis scalingAxis;


    [SerializeField] float referenceSize;
    [SerializeField] float textureSize;
    [SerializeField] RectTransform joystick;
    public void Awake()
    {
        var button = GetComponent<UltimateButton>();
        canvasRectTrans = InputManager.Instance.GetComponent<RectTransform>();
        if(button)
        {
            _type = Type.Button;
            _controllerName = button.buttonName;
            scalingAxis = button.scalingAxis == UltimateButton.ScalingAxis.Height ? ScalingAxis.Height : ScalingAxis.Width;
        }
        var joystick = GetComponent<UltimateJoystick>();
        if (joystick)
        {
            _type = Type.Joystick;
            _controllerName = joystick.joystickName;
            scalingAxis = joystick.scalingAxis == UltimateJoystick.ScalingAxis.Height ? ScalingAxis.Height : ScalingAxis.Width;

        }

    }
    public void OnPointerDown(PointerEventData eventData)
    {

        if (_type == Type.Button)
        {
            anchor = UltimateButton.GetUltimateButton(_controllerName).anchor == UltimateButton.Anchor.Left ? Anchor.Left : Anchor.Right;
            scalingAxis = UltimateButton.GetUltimateButton(_controllerName).scalingAxis == UltimateButton.ScalingAxis.Height ? ScalingAxis.Height : ScalingAxis.Width;
            joystick = UltimateButton.GetUltimateButton(_controllerName).GetComponent<RectTransform>();

            referenceSize = scalingAxis == ScalingAxis.Height ? canvasRectTrans.sizeDelta.y : canvasRectTrans.sizeDelta.x;
            textureSize = referenceSize * (UltimateButton.GetUltimateButton(_controllerName).buttonSize / 10);
        }
        else
        {
            anchor = UltimateJoystick.GetUltimateJoystick(_controllerName).anchor == UltimateJoystick.Anchor.Left ? Anchor.Left : Anchor.Right;
            scalingAxis = UltimateJoystick.GetUltimateJoystick(_controllerName).scalingAxis == UltimateJoystick.ScalingAxis.Height ? ScalingAxis.Height : ScalingAxis.Width;
             joystick = UltimateJoystick.GetUltimateJoystick(_controllerName).joystick;
            referenceSize = scalingAxis == ScalingAxis.Height ? canvasRectTrans.sizeDelta.y : canvasRectTrans.sizeDelta.x;
            textureSize = referenceSize * (UltimateJoystick.GetUltimateJoystick(_controllerName).joystickSize / 10);

        }


        InputManager.Instance.GetComponent<UI_InputSetting>().SetupEdit(this);

    }
    public void OnDrag(PointerEventData eventData)
    {
        //print(eventData.position);
        this.transform.position = eventData.position;
        //if (_type == Type.Button)
        //{
        //    var vector = GetButtonPosition();
        //    UltimateButton.GetUltimateButton(_controllerName).positionHorizontal = vector.x;
        //    UltimateButton.GetUltimateButton(_controllerName).positionVertical = vector.y;
        //    UltimateButton.GetUltimateButton(_controllerName).UpdatePositioning();
        //    print(vector);

        //}
        //else
        //{
        //    var vector = GetJoystickSposition();
        //    UltimateJoystick.GetUltimateJoystick(_controllerName).positionHorizontal = vector.x;
        //    UltimateJoystick.GetUltimateJoystick(_controllerName).positionVertical = vector.y;
        //    UltimateJoystick.GetUltimateJoystick(_controllerName).UpdateJoystickPositioning();
        //}
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_type == Type.Button)
        {
            var vector = GetPoistion();
            UltimateButton.GetUltimateButton(_controllerName).positionHorizontal = vector.x;
            UltimateButton.GetUltimateButton(_controllerName).positionVertical = vector.y;
            UltimateButton.GetUltimateButton(_controllerName).UpdatePositioning();
            print(vector);

        }
        else
        {
            var vector = GetPoistion();
            UltimateJoystick.GetUltimateJoystick(_controllerName).positionHorizontal = vector.x;
            UltimateJoystick.GetUltimateJoystick(_controllerName).positionVertical = vector.y;
            UltimateJoystick.GetUltimateJoystick(_controllerName).UpdateJoystickPositioning();
            print(vector);
        }
    }
    public Vector2 GetPoistion()
    {

    

        var convertX = (joystick.transform.position.x * canvasRectTrans.sizeDelta.x / Screen.width);
        if (anchor == Anchor.Right)
        {

            //convertX = -convertX;
            convertX = convertX - (canvasRectTrans.sizeDelta.x * 0.5f);
            convertX = -convertX;

        }
        else
        {
            convertX = convertX - (canvasRectTrans.sizeDelta.x * 0.5f);

        }

        var convertY = (joystick.transform.position.y * canvasRectTrans.sizeDelta.y / Screen.height);
        convertY = convertY - (canvasRectTrans.sizeDelta.y * 0.5f);

        var joystickPosition = new Vector2(convertX, convertY);


        var tex = (50 * (2 * joystickPosition.x + canvasRectTrans.sizeDelta.x - textureSize)) / (canvasRectTrans.sizeDelta.x - textureSize);
        var tey = (50 * (2 * joystickPosition.y + canvasRectTrans.sizeDelta.y - textureSize)) / (canvasRectTrans.sizeDelta.y - textureSize);



        return new Vector2(tex, tey);
    }

    public void ChangeSize(float value)
    {
        if (_type == Type.Button)
        {
            UltimateButton.GetUltimateButton(_controllerName).buttonSize = value;
            UltimateButton.GetUltimateButton(_controllerName).UpdatePositioning();
        }
        else
        {
            UltimateJoystick.GetUltimateJoystick(_controllerName).joystickSize = value;
            UltimateJoystick.GetUltimateJoystick(_controllerName).UpdateJoystickPositioning();
        }

    }

    public void SetupByData(InputUIInfo inputUIInfo)
    {
        if (_type == Type.Button)
        {
            UltimateButton ultimateButton = UltimateButton.GetUltimateButton(inputUIInfo.joystickName);
            ultimateButton.positionHorizontal = inputUIInfo.vector2.x;
            ultimateButton.positionVertical = inputUIInfo.vector2.y;
            ultimateButton.buttonSize = inputUIInfo.size;
            ultimateButton.UpdatePositioning();
        }
        else
        {
            UltimateJoystick ultimateJoystick = UltimateJoystick.GetUltimateJoystick(inputUIInfo.joystickName);
            ultimateJoystick.positionHorizontal = inputUIInfo.vector2.x;
            ultimateJoystick.positionVertical = inputUIInfo.vector2.y;
            ultimateJoystick.joystickSize = inputUIInfo.size;
            ultimateJoystick.UpdateJoystickPositioning();

        }

    }

    private void LateUpdate()
    {
        if (_type == Type.Button)
        {
        }
        else
        {
            //UltimateJoystick.GetUltimateJoystick(_controllerName).joystick.transform.localPosition = Vector2.zero;
        }
    }

  
}
