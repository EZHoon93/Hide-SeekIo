using System.Collections;

using UnityEngine;

public abstract class BuffBase : MonoBehaviour
{
    //protected LivingEntity _livingEntity;
    //protected Transform _targetTransform;
    [SerializeField] protected ParticleSystem _particle;
    [SerializeField] protected BuffController _buffController;

    private void Reset()
    {
        //init();
    }
    private void Awake()
    {
        //init();
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

    protected abstract void ProcessStart();

    protected abstract void ProcessEnd();

}
