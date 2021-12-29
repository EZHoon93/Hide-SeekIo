using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class InputBase : MonoBehaviourPun
{
    protected  Dictionary<InputType, ControllerInput> controllerInputDic { get; set; } =
    new Dictionary<InputType, ControllerInput>();




    public void SetActiveUserControllerJoystick(bool active)
    {
        if (active)
        {
            foreach (var input in controllerInputDic)
            {
                SetupControllerInputUI(input.Value.attackType, input.Value.inputType);
            }
        }
        else
        {
            foreach (var input in controllerInputDic)
            {
                RemoveControllerInputUI(input.Value.inputType);
            }
        }
     
    }




    public ControllerInput AddInputEvent(Define.AttackType attackType, ControllerInputType controllerInputType, InputType inputType, Action<Vector2> action)
    {
        ControllerInput controllerInput = null;
        bool isCache = controllerInputDic.TryGetValue(inputType, out controllerInput);
        if (isCache == false)
        {
            controllerInput = new ControllerInput(inputType, attackType);
            controllerInputDic.Add(inputType, controllerInput);
        }
        controllerInput.controllerDic[controllerInputType] += action;
        return controllerInput;
    }

    public ControllerInput GetControllerInput(InputType inputType)
    {
        if (controllerInputDic.ContainsKey(inputType) == false)
        {
            Debug.LogError("Error Not Controller Input");
            return null;
        }
        return controllerInputDic[inputType];
    }

    public UI_ControllerJoystick GetControllerJoystickUI(InputType inputType)
        => Managers.Input.GetControllerJoystick(inputType);


    public Vector2 GetVector2(InputType inputType)
    {
       return  GetControllerInput(inputType).inputVector2;
    }

    public virtual void RemoveInputEvent(InputType inputType)
    {
        if (controllerInputDic.ContainsKey(inputType))
        {
            controllerInputDic[inputType].Reset();
        }
    }


    public virtual void SetupControllerInputUI(Define.AttackType attackType, InputType inputType, Sprite sprite = null)
    {
        var controllerInput = GetControllerInput(inputType);
        var controllerjoystick = GetControllerJoystickUI(inputType);
        controllerjoystick.controllerInput = controllerInput;
        controllerjoystick.gameObject.SetActive(true);
        controllerjoystick.ResetUIController();
    }

    public virtual void RemoveControllerInputUI(InputType inputType)
    {
        var controllerjoystick = GetControllerJoystickUI(inputType);
        var controllerInput = GetControllerInput(inputType);
        
        controllerjoystick.RemoveUI(controllerInput);
    }

    public void SetupControllerInputUISprite(InputType inputType, Sprite sprite)
    {
        Managers.Input.GetControllerJoystick(inputType).SetupItemImage(sprite);
    }
}
