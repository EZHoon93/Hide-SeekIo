using UnityEngine;
using System.Collections;
using System;
using Photon.Pun;
using System.Collections.Generic;

public class InputControllerObject : MonoBehaviourPun
{
    public InputType inputType { get; set; }
    public Define.AttackType attackType;
    public PlayerShooter.state shooterState{ get; set; }
    public float InitCoolTime { get; set; }
    public float RemainCoolTime { get; set; }
    public event Action<Vector2> useEventCallBack;
    public event Action<Vector2> zoomEventCallBack;
    public event Action useSucessStartCallBack;
    public event Action useSucessEndCallBack;

    public Vector2 lastInputSucessVector2 { get; set; }

    public Vector3 attackPoint { get; set; }



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
        print("AddUseEnvet ");
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

    /// <summary>
    /// 아이템,스킬,무기등 사용시 
    /// </summary>
    /// <param name="inputVector2"></param>

    public bool Use(Vector2 inputVector2)
    {
        if (RemainCoolTime > 0) return false;
        RemainCoolTime = InitCoolTime;
        lastInputSucessVector2 = inputVector2;
        if (this.IsMyCharacter())
        {
            InputManager.Instance.GetControllerJoystick(inputType).StartCoolTime(RemainCoolTime);
        }
        useEventCallBack?.Invoke(inputVector2);
        return true;
        
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
