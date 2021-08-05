using System;
using System.Collections.Generic;

using Photon.Pun;

using UnityEngine;

public enum ControllerInputType
{
    Down,
    Drag,
    Up,
    Tap
}
public class MyInput
{
    public Dictionary<ControllerInputType, Action<Vector2>> ControllerDic { get; set; }
    = new Dictionary<ControllerInputType, Action<Vector2>>()
    {
        {ControllerInputType.Down,null },
        {ControllerInputType.Drag,null },
        {ControllerInputType.Up,null },
        {ControllerInputType.Tap,null },
    };
}

public abstract class InputBase : MonoBehaviourPun
{
    protected float _stopTime;
    protected bool _isAttack;

    public Vector2 MoveVector { get; protected set; }
    public Vector2 AttackVector { get; protected set; }
    public Vector2 ItemVector1 { get; set; }
    public Vector2 ItemVector2 { get; set; }
    public Vector2 RandomVector2 { get; set; }
    public bool IsStop { get; protected set; }


    public event Action<Vector2> AttackEventCallBack;
    public Action<Vector2> AttackZoomCallBack;

    public MyInput baseInput = new MyInput();
    public MyInput[] itemsInput = new MyInput[2] { new MyInput(), new MyInput() };


    public Action<int> UseItemCallBack;
    
 
    protected virtual void Awake()
    {
        GetComponent<LivingEntity>().onDeath += HandleDeath;

        //itemsInput[0].ControllerDic[ControllerInputType.Up] = (Vector2)=> UseItem(0);
        itemsInput[1].ControllerDic[ControllerInputType.Up] = (Vector2) => UseItem(0,Vector2);


    }
    public virtual void OnPhotonInstantiate()
    {
        IsStop = false;
        _isAttack = false;
        _stopTime = 0;
        RandomVector2 = Vector2.one;
        if (this.IsMyCharacter())
        {
            InputManager.Instance.mainJoystick.myInput = baseInput;
            //InputManager.Instance.baseAttackJoystick.onAttackEventCallBack = AttackEventCallBack;
            //InputManager.Instance.baseAttackJoystick.onAttackEventCallBack = baseInput.ControllerDic[ControllerInputType.Down];
            //InputManager.Instance
            //InputManager.Instance.baseAttackJoystick.onapEventCallBack = 

            //아이템 컨트롤러
            for (int i = 0; i < InputManager.Instance.itemControllerJoysticks.Length; i++)
            {
                InputManager.Instance.itemControllerJoysticks[i].myInput = itemsInput[i];

                //items += 
            }

        }
    }

    void UseItem(int index, Vector2  t)
    {
        UseItemCallBack?.Invoke(index);
    }

    protected virtual void HandleDeath()
    {

    }


    public void Call_AttackCallBackEvent(Vector2 vector2)
    {
        AttackEventCallBack?.Invoke(vector2);
    }

    public void CallBackItem1(Vector2 vector2)
    {
    }

    public void CallBackItem2(Vector2 vector2)
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
