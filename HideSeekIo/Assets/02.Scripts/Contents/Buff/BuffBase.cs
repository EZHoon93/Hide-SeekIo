using System.Collections;

using UnityEngine;

public abstract class BuffBase : Poolable
{
    [SerializeField] protected ParticleSystem _particle;
    [SerializeField] protected BuffController _buffController;
    [SerializeField] float _durationTime;

    public float durationTime => _durationTime;
    public Renderer[] renderers  { get; private set; }

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }
    public void Setup(BuffController buffController)
    {
        _buffController = buffController;
    }

    public override void Push()
    {
        ProcessEnd();
        Managers.Pool.Push(this);
    }
    public abstract void ProcessStart();

    public abstract void ProcessEnd();
  

}
