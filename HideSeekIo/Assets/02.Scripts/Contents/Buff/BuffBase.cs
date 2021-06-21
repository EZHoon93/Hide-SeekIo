using System.Collections;

using UnityEngine;

public abstract class BuffBase : Poolable
{
    //protected LivingEntity _livingEntity;
    //protected Transform _targetTransform;
    [SerializeField] protected ParticleSystem _particle;
    [SerializeField] protected BuffController _buffController;

    public void Setup(BuffController buffController)
    {
        _buffController = buffController;
    }

    //void init()
    //{
    //    if (_particle == null)
    //        _particle = GetComponentInChildren<ParticleSystem>();
    //}
    //protected void OnEnable()
    //{
    //    _buffController = GetComponentInParent<BuffController>();
    //    if (_buffController == null) return;
    //    _particle?.Play();  //이펙트있으면 실행
    //    ProcessStart(); //효과있으면 실행
    //}

    //public override void Push()
    //{
    //    ProcessEnd();
    //    //_livingEntity = null;
    //    base.Push();
    //}

    public abstract void ProcessStart();

    public virtual void ProcessEnd()
    {
        Managers.Pool.Push(this);
    }

}
