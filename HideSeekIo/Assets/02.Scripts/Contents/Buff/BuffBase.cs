using System.Collections;

using UnityEngine;

public abstract class BuffBase : Poolable
{
    [SerializeField] protected ParticleSystem _particle;
    [SerializeField] protected BuffController _buffController;

    public void Setup(BuffController buffController)
    {
        _buffController = buffController;
    }

    public void Push()
    {
        ProcessEnd();
        Managers.Pool.Push(this);
    }
    public abstract void ProcessStart();

    public abstract void ProcessEnd();
  

}
