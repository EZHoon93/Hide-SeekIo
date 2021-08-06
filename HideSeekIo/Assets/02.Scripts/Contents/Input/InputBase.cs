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
    public Vector2 RandomVector2 { get; set; }
    public bool IsRun { get; set; }
    public bool IsStop { get; protected set; }
    public MyInput skillInput = new MyInput();
    public MyInput subInput = new MyInput();
    public MyInput mainInput = new MyInput();
    public MyInput[] itemsInput = new MyInput[1] { new MyInput()};


    public Action<int> UseItemCallBack;
    
 
    protected virtual void Awake()
    {
        GetComponent<LivingEntity>().onDeath += HandleDeath;

        itemsInput[0].ControllerDic[ControllerInputType.Up] = (Vector2)=> UseItem(0,Vector2);
        //itemsInput[1].ControllerDic[ControllerInputType.Up] = (Vector2) => UseItem(0,Vector2);


    }
    public virtual void OnPhotonInstantiate()
    {
        IsStop = false;
        _isAttack = false;
        _stopTime = 0;
        RandomVector2 = Vector2.one;
    }
    private void Start()
    {
        IsStop = false;
        _isAttack = false;
        _stopTime = 0;
        RandomVector2 = Vector2.one;
    }
    void UseItem(int index, Vector2  t)
    {
        UseItemCallBack?.Invoke(index);
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
