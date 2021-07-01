using UnityEngine;
using Photon.Pun;

public abstract class InputBase : MonoBehaviourPun
{
    protected float _stopTime;
    protected bool _isAttack;
    protected Vector2 _attackVector;
    protected Vector2 _lastAttackVector;

    public Vector2 MoveVector { get; protected set; }
    public Vector2 AttackVector => _attackVector;
    public Vector2 LastAttackVector => _lastAttackVector;
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
