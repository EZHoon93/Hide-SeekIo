
using System.Collections.Generic;
using System;
using UnityEngine;
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


public class ControllerInput
{
    //public UI_ControllerJoystick uI_ControllerJoystick { get; set; }
    public Define.AttackType attackType { get; set; }
    public InputType inputType{ get; set; }
    public Dictionary<ControllerInputType, Action<Vector2>> controllerDic { get; set; }
    = new Dictionary<ControllerInputType, Action<Vector2>>()
    {
        {ControllerInputType.Down,null },
        {ControllerInputType.Drag,null },
        {ControllerInputType.Up,null },
        {ControllerInputType.Tap,null },
    };

    public ControllerInput(InputType initInputType , Define.AttackType initAttackType)
    {
        attackType = initAttackType;
        inputType = initInputType;
    }

    public void Call(ControllerInputType controllerInputType, Vector2 vector2)
    {
        controllerDic[controllerInputType]?.Invoke(vector2);
        inputVector2 = vector2;
        if(controllerInputType == ControllerInputType.Up)
        {
            inputVector2 = Vector2.zero;
        }
    }

    public void Reset() 
    {
        inputVector2 = Vector2.zero; ;
        remainCoolTime = 0;
        controllerDic[ControllerInputType.Down] = null;
        controllerDic[ControllerInputType.Drag] = null;
        controllerDic[ControllerInputType.Up] = null;
        controllerDic[ControllerInputType.Tap] = null;
    }   

    public void Use()
    {

    }
    public void removeEvent()
    {

    }

    public Vector2 inputVector2 { get;  set; }
    public float coolTime { get; set; }
    public float remainCoolTime { get; set; }
}
