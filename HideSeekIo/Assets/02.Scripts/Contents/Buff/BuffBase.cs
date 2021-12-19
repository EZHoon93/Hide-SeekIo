using System.Collections;

using ExitGames.Client.Photon.StructWrapping;

using Photon.Pun;

using UnityEngine;

public abstract class BuffBase : Poolable 
{
    [SerializeField] protected ParticleSystem _buffEffect;
    [SerializeField] protected ParticleSystem _playEffect;
    [SerializeField] bool _isNuff;

    float _createTime;
    float _durationTime;
    IEnumerator _processEnumerator;
    Define.BuffType _buffType;

    protected BuffController _buffController;
    protected LivingEntity _livingEntity => _buffController.livingEntity;
    public bool isNuff => _isNuff;

    public RenderController renderController { get; private set; }
    public Define.BuffType buffType => _buffType;

    public float durationTime => _durationTime;
    public float createTime => _createTime;


    private void Awake()
    {
        renderController = GetComponent<RenderController>();
    }
    public void Setup(BuffController buffController, Define.BuffType newBuffType)
    {
        _buffController = buffController;
        _buffType = newBuffType;
    }
    public void Play(float createTime , float durationTime)
    {
        _createTime = createTime;
        _durationTime = durationTime;
        if (_buffEffect && _createTime <= PhotonNetwork.Time + 0.5f)
        {
            _buffEffect?.Play();
        }
        Util.StartCoroutine(this, ref _processEnumerator, UpdateBuff());
    }

    public override void Push()
    {
        _buffController.RemoveBuff(this);
        _durationTime = 0;
        _buffController = null;
        _createTime = 0;
        base.Push();
    }

    public void RePlay(float addDurationTime)
    {
        _durationTime += addDurationTime;
    }


    IEnumerator UpdateBuff()
    {
        ProcessStart();
        while (PhotonNetwork.Time <= _createTime + _durationTime)
        {
            yield return new WaitForSeconds(0.1f);
        }
        ProcessEnd();
        Push();
    }

    public virtual void ProecessUpdate()
    {

    }
    public abstract void ProcessStart();

    public abstract void ProcessEnd();

 
}
