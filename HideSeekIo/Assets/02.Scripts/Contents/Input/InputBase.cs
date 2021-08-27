﻿using System;
using System.Collections.Generic;

using Photon.Pun;

using UnityEngine;

//public enum ControllerInputType
//{
//    Down,
//    Drag,
//    Up,
//    Tap
//}
//public enum InputType
//{
//    Move,
//    Main,
//    Sub1,
//    Sub2,
//    Sub3
//}

//public class MyInput
//{
//    public Dictionary<ControllerInputType, Action<Vector2>> controllerDic { get; set; }
//    = new Dictionary<ControllerInputType, Action<Vector2>>()
//    {
//        {ControllerInputType.Down,null },
//        {ControllerInputType.Drag,null },
//        {ControllerInputType.Up,null },
//        {ControllerInputType.Tap,null },
//    };

//    public void Call(ControllerInputType controllerInputType, Vector2 vector2)
//    {
//        controllerDic[controllerInputType]?.Invoke(vector2);
//        inputVector2 = vector2;
//    }

//    public Vector2 inputVector2 { get; private set; }
//    public float coolTime { get; set; }
//}

public abstract class InputBase : MonoBehaviourPun
{
  
    protected float _stopTime;
    protected bool _isAttack;
    public Vector2 RandomVector2 { get; set; }
    //public bool IsRun { get; set; }
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




    
 
    protected virtual void Awake()
    {
        GetComponent<LivingEntity>().onDeath += HandleDeath;
    }

    public virtual void OnPhotonInstantiate()
    {
        IsStop = false;
        _isAttack = false;
        _stopTime = 0;
        RandomVector2 = Vector2.one;
    }

    public virtual void AddInputEvent(ControllerInputType controllerInputType,InputControllerObject inputControllerObject , System.Action<Vector2> action, ObtainableItem newItemObject = null)
    {
        MyInput addInput = null;
        bool isCache =  controllerInputDic.TryGetValue(inputControllerObject.inputType, out addInput);
        if (isCache == false)
        {
            controllerInputDic.Add(inputControllerObject.inputType, addInput);
        }
        addInput.controllerDic[controllerInputType] = action;
        if (this.IsMyCharacter())
        {
            //var contollreUI = InputManager.Instance.GetControllerJoystick(inputType);
            //if (controllerInputType == ControllerInputType.Tap || controllerInputType == ControllerInputType.Down)
            //{
            //    contollreUI.SetActiveControllerType(Define.ControllerType.Button, newItemObject);
            //}
            //if (controllerInputType == ControllerInputType.Drag)
            //{
            //    contollreUI.SetActiveControllerType(Define.ControllerType.Joystick, newItemObject);
            //}

            //InputManager.Instance.GetControllerJoystick(inputControllerObject.inputType).SetActiveControllerType(inputControllerObject);
        }
    }


    //public virtual void AddInputEvent(InputControllerObject inputControllerObject, Action <Vector2> actionEventCallBack)
    //{
    //    //MyInput myInput = controllerInputDic[inputControllerObject.inputType];
    //    //foreach(var c in inputControllerObject.controllerInputTypeDic)
    //    //{
    //    //    myInput.controllerDic[c.Key] = c.Value;
    //    //}
    //    //if (this.IsMyCharacter())
    //    //{
    //    //    InputManager.Instance.GetControllerJoystick(inputControllerObject.inputType).SetActiveControllerType(inputControllerObject);
    //    //}
    //}

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
