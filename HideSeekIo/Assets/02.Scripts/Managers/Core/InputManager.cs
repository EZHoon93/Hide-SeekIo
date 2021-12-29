using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField] UI_ControllerJoystick[] _controllerJoysticks;
    public UI_ControllerJoystick GetControllerJoystick(InputType inputType)
    {
        foreach(var joystick in _controllerJoysticks)
        {
            if(joystick.inputType == inputType)
            {
                return joystick;
            }
        }
        return null;
       //return _controllerJoysticks.Single(s => s.inputType == inputType);
    }

 


    public void Init()
    {
        foreach (var c in _controllerJoysticks)
        {
            c.Init();
            c.gameObject.SetActive(false);
        }
    }

  
    public void Clear()
    {
        SetActiveController(false);
    }

    //public void SetUpControllerInput(InputType inputType , ControllerInput controllerInput)
    //{
    //    GetControllerJoystick(inputType).controllerInput = controllerInput;
    //    GetControllerJoystick(inputType).SetActiveControllerType(controllerInput.attackType);
    //}

    private void Update()
    {

#if UNITY_ANDROID
        //MoveVector = new Vector2(_moveJoystick.GetHorizontalAxis(), _moveJoystick.GetVerticalAxis());
#endif

#if UNITY_EDITOR
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        //MoveVector = new Vector2(h, v);
#endif
    }
    public void SetActiveController(bool active)
    {
        foreach (var joystick in _controllerJoysticks)
            joystick.gameObject.SetActive(active);

    }




}
