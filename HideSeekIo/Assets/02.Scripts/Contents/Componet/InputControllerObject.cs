using UnityEngine;
using System.Collections;
using System;
using Photon.Pun;
using System.Collections.Generic;

public class InputControllerObject : MonoBehaviour
{
    //Dictionary<ControllerInputType, Action<Vector2>> controllerInputTypeDic { get; set; } = new Dictionary<ControllerInputType, Action<Vector2>>();
    public InputType inputType { get; set; }
    public Define.AttackType attackType;
    public float InitCoolTime { get; set; }
    public float RemainCoolTime { get; set; }
    public event Action<Vector2> useEventCallBack;
    public event Action<Vector2> zoomEventCallBack;
    public event Action useSucessStartCallBack;
    public event Action useSucessEndCallBack;


    

    //public void AddEvent(ControllerInputType controllerInputType , Action<Vector2> newAction)
    //{
    //    Action<Vector2> callBackAction = null;
    //    var isCache = controllerInputTypeDic.TryGetValue(controllerInputType, out callBackAction);
    //    if(isCache == false)
    //    {
    //        controllerInputTypeDic.Add(controllerInputType, callBackAction);
    //    }
    //    callBackAction += newAction;
    //}

    public void AddUseEvent(Action<Vector2> action)
    {
        useEventCallBack = action;
       
    }

    public void AddZoomEvent(Action<Vector2> action)
    {
        zoomEventCallBack = action;
    }

    //public void RemoveEvent(ControllerInputType controllerInputType , Action<Vector2> removeAction)
    //{
    //    if(controllerInputTypeDic.ContainsKey(controllerInputType))
    //    {
    //        controllerInputTypeDic[controllerInputType] -= removeAction;
    //    }
    //}

    IEnumerator UpdateRemainCoolTime()
    {
        while(RemainCoolTime > 0)
        {
            yield return null;
            RemainCoolTime -= Time.deltaTime;
        }
    }


    public void Use(Vector2 inputVector2)
    {
        if (RemainCoolTime > 0) return;
        RemainCoolTime = InitCoolTime;
        
        useEventCallBack?.Invoke(inputVector2);
    }

    public void Call_UseSucessStart()
    {
        useSucessStartCallBack?.Invoke();
    }
    public void Call_UseSucessEnd()
    {
        useSucessEndCallBack?.Invoke();
    }

    public void Zoom(Vector2 inputVector2)
    {
        zoomEventCallBack?.Invoke(inputVector2);
    }

    private void Update()
    {
        if(RemainCoolTime > 0)
        {
            RemainCoolTime -= Time.deltaTime;
        }
    }
}
