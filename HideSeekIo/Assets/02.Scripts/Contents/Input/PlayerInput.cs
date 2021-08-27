using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
public enum ControllerInputType
{
    Down,
    Drag,
    Up,
    Tap
}
public enum InputType
{
    Move,
    Main,
    Sub1,
    Sub2,
    Sub3
}

public class MyInput
{
    public Dictionary<ControllerInputType, Action<Vector2>> controllerDic { get; set; }
    = new Dictionary<ControllerInputType, Action<Vector2>>()
    {
        {ControllerInputType.Down,null },
        {ControllerInputType.Drag,null },
        {ControllerInputType.Up,null },
        {ControllerInputType.Tap,null },
    };

    public void Call(ControllerInputType controllerInputType, Vector2 vector2)
    {
        controllerDic[controllerInputType]?.Invoke(vector2);
        inputVector2 = vector2;
    }

    public Vector2 inputVector2 { get; private set; }
    public float coolTime { get; set; }
}


public class PlayerInput : MonoBehaviourPun
{
    protected float _stopTime;
    protected bool _isAttack;
    public Vector2 RandomVector2 { get; set; }
    public bool IsStop { get; protected set; }

    public Dictionary<InputType, MyInput> controllerInputDic { get; set; } =
    new Dictionary<InputType, MyInput>()
    {
        {InputType.Move ,  new MyInput() },
        {InputType.Main ,  new MyInput() },
        {InputType.Sub1 ,   new MyInput() },
        {InputType.Sub2 , new MyInput() },
        {InputType.Sub3 , new MyInput() },
    };



    public virtual void OnPhotonInstantiate()
    {
        IsStop = false;
        _isAttack = false;
        _stopTime = 0;
        RandomVector2 = Vector2.one;
        if (this.IsMyCharacter())
        {
            foreach (var input in controllerInputDic)
            {
                InputManager.Instance.GetControllerJoystick(input.Key).myInput = input.Value;
            }
            InputManager.Instance.SetActiveController(true);
        }
    }

    public virtual void AddInputEvent(Define.AttackType attackType, ControllerInputType controllerInputType, InputType inputType , System.Action<Vector2> action)
    {
        MyInput addInput = null;
        print(inputType + " /" + controllerInputType + "/");
        bool isCache = controllerInputDic.TryGetValue(inputType, out addInput);
        if (isCache == false)
        {
            controllerInputDic.Add(inputType, addInput);
        }
        addInput.controllerDic[controllerInputType] = action;
        if (this.IsMyCharacter())
        {
            InputManager.Instance.GetControllerJoystick(inputType).SetActiveControllerType(attackType);
        }
    }



    public virtual void RemoveInputEvent(InputType inputType)
    {
        if (controllerInputDic.ContainsKey(inputType))
        {
            controllerInputDic[inputType].controllerDic[ControllerInputType.Down] = null;
            controllerInputDic[inputType].controllerDic[ControllerInputType.Tap] = null;
            controllerInputDic[inputType].controllerDic[ControllerInputType.Up] = null;
            controllerInputDic[inputType].controllerDic[ControllerInputType.Drag] = null;
        }
        if (this.IsMyCharacter())
        {
            //if(inputType == InputType.Skill1)
            //{
            //    InputManager.Instance.GetControllerJoystick(inputType).gameObject.SetActive(false);
            //}
        }
    }

    protected virtual void HandleDeath()
    {

    }


    public virtual void Stop(float newTime)
    {
        _stopTime = newTime;
        IsStop = true;
    }

    public void RemoveStop()
    {
        IsStop = false;
        _stopTime = 0;
    }

}
