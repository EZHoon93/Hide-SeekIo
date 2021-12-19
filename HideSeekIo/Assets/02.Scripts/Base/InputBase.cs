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
        foreach (var input in controllerInputDic)
        {
            Managers.Input.SetUpControllerInput(input);
            Managers.Input.GetControllerJoystick(input.Key).gameObject.SetActive(active);
        }
    }




    public virtual void AddInputEvent(Define.AttackType attackType, ControllerInputType controllerInputType, InputType inputType, Action<Vector2> action, Sprite sprite = null)
    {
        ControllerInput addInput = null;
        bool isCache = controllerInputDic.TryGetValue(inputType, out addInput);
        if (isCache == false)
        {
            addInput = new ControllerInput(inputType);
            controllerInputDic.Add(inputType, addInput);
        }
        addInput.controllerDic[controllerInputType] += action;
        addInput.attackType = attackType;
    }

    public ControllerInput GetControllerInput(InputType inputType)
    {
        return controllerInputDic[inputType];
    }

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
        if (this.IsMyCharacter())
        {
            Managers.Input.GetControllerJoystick(inputType).gameObject.SetActive(false);
        }
    }


    public void SetupControllerInputUI(Define.AttackType attackType, InputType inputType, Sprite sprite)
    {
        if (this.IsMyCharacter())
        {
            controllerInputDic[inputType].uI_ControllerJoystick = Managers.Input.GetControllerJoystick(inputType);
            Managers.Input.GetControllerJoystick(inputType).SetActiveControllerType(attackType, null);
            Managers.Input.GetControllerJoystick(inputType).ResetUIController();
        }
    }
}
