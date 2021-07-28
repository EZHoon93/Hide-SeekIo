using UnityEngine;
using Photon.Pun;
using System;

public abstract class InputBase : MonoBehaviourPun
{
    protected float _stopTime;
    protected bool _isAttack;
    
    public Vector2 MoveVector { get; protected set; }
    public Vector2 AttackVector { get; protected set; }
    public Vector2 ItemVector1 { get; set; }
    public Vector2 ItemVector2 { get; set; }
    public event Action<Vector2> AttackEventCallBack;
    public event Action<Vector2> ItemEventCallBackList1;
    public event Action<Vector2> ItemEventCallBackList2;
    public Vector2 RandomVector2 { get; set; }
    public bool IsStop { get; protected set; }
    

    protected virtual void Awake()
    {
        GetComponent<LivingEntity>().onDeath += HandleDeath;
    }
    public virtual  void OnPhotonInstantiate()
    {
         IsStop = false;
        _isAttack = false;
        _stopTime = 0;
        RandomVector2 = Vector2.one;
    }

    protected virtual void HandleDeath()
    {

    }

    protected void Call_AttackCallBackEvent(Vector2 vector2)
    {
        AttackEventCallBack?.Invoke(vector2);
    }

    protected void CallBackItem1(Vector2 vector2)
    {
        ItemEventCallBackList1?.Invoke(vector2);
    }

    protected void CallBackItem2(Vector2 vector2)
    {
        ItemEventCallBackList2?.Invoke(vector2);
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
