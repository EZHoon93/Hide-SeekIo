using System.Collections;

using UnityEngine;

public abstract class BuffBase : Poolable
{
    [SerializeField] protected ParticleSystem _buffEffect;
    [SerializeField] protected ParticleSystem _playEffect;
    [SerializeField] float _durationTime;
    [SerializeField] bool _isNuff;

    protected BuffController _buffController;

    public bool isNuff => _isNuff;
    public float durationTime => _durationTime;
    public RenderController renderController { get; private set; }

    private void Awake()
    {
        renderController = GetComponent<RenderController>();
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
    public void Play()
    {
        if (_buffEffect)
        {
            _buffEffect?.Play();
        }
        ProcessStart();
    }
    public void PlayEffect()
    {
        if (_playEffect)
        {
            _playEffect.Play();
        }
    }
    public abstract void ProcessStart();

    public abstract void ProcessEnd();
  
    
}
