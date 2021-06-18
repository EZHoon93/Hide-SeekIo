using System.Collections;

using UnityEngine;

public abstract class HiderInput : InputBase
{
    public Vector2 MoveVector { get;  protected set; } //감지된 움직인 벡터 값
    public bool IsRun { get; set; }
    public bool IsStop { get; protected set; }



    float stopTime;
    
    protected virtual void Awake()
    {
        GetComponent<LivingEntity>().onDeath += HandleDeath;
    }

    public override void OnPhotonInstantiate()
    {
        IsStop = false;
        RandomVector2 = Vector2.one;

    }
    protected virtual void HandleDeath()
    {
        
    }
    //스탑 상태에서 발생
    protected void UpdateStopState()
    {
        if (stopTime > 0)
        {
            stopTime -= Time.deltaTime;
            MoveVector = Vector2.zero;
        }
        else
        {
            IsStop = false;
        }
    }



    public override void Stop(float newTime)
    {

    }


}
