using UnityEngine;
using System.Collections;
using System;
using Photon.Pun;
using System.Collections.Generic;

public class InputControllerObject : MonoBehaviour
{
    public Dictionary<ControllerInputType,Action<Vector2>> controllerInputTypeDic = new Dictionary<ControllerInputType, Action<Vector2>>();
    public InputType inputType { get; protected set; }



    public void AddEvent(ControllerInputType controllerInputType , Action<Vector2> newAction)
    {
        Action<Vector2> callBackAction = null;
        var isCache = controllerInputTypeDic.TryGetValue(controllerInputType, out callBackAction);
        if(isCache == false)
        {
            controllerInputTypeDic.Add(controllerInputType, callBackAction);
        }
        callBackAction += newAction;
    }

    public void RemoveEvent(ControllerInputType controllerInputType , Action<Vector2> removeAction)
    {
        if(controllerInputTypeDic.ContainsKey(controllerInputType))
        {
            controllerInputTypeDic[controllerInputType] -= removeAction;
        }
    }

}
