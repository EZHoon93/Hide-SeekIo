
using UnityEngine;
using Photon.Pun;
using TMPro;

/// <summary>
/// 서버통신을위한 버프컨트롤러,
/// </summary>
public class BuffController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] float _durationTime;
    [SerializeField] float _createServerTime;
    [SerializeField] BuffBase _buffBase;
    Poolable _poolable;

    public Define.BuffType BuffType { get; private set; }
    public float ReaminTime => ((_durationTime + _createServerTime) - (float)PhotonNetwork.Time);
    public LivingEntity livingEntity { get; private set; }
    public float DurationTime => _durationTime;

    public bool IsNuff { get; private set; }    //너프 인지

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(BuffType);
            stream.SendNext(_createServerTime);
            stream.SendNext(_durationTime);
        }
        else
        {
            var n_BuffType = (Define.BuffType)stream.ReceiveNext();
            var n_createServerTime = (float)stream.ReceiveNext();
            var n_durationTime = (float)stream.ReceiveNext();

            if (n_createServerTime == _createServerTime) return;

            var newLivingEntity = this.transform.parent.GetComponent<LivingEntity>();
            Setup(n_BuffType, newLivingEntity, n_createServerTime, n_durationTime);
        }

    }
    private void Awake()
    {
        _poolable = GetComponent<Poolable>();
    }
    //재갱신 및 최초
    public void Setup(Define.BuffType buffType, LivingEntity newLivingEntity, float createServerTime, float durationTime)
    {
        livingEntity = newLivingEntity;
        _createServerTime = createServerTime;
        _durationTime = durationTime;


        if (_buffBase == null)
        {
            BuffType = buffType;
            _buffBase = BuffManager.Instance.MakeBuffObject(buffType, this.transform);
            _buffBase.Setup(this);
            SetupIsPostiveBuff(BuffType);
            foreach(var render in _buffBase.renderers)
            {
                livingEntity.fogController.AddHideRender(render);
            }
        }
       
        _buffBase.ProcessStart();
        //LocalEffect();
    }

    //버퍼시 이펙트 로컬에서 발생 => 메시지수아끼기위해
    //void LocalEffect()
    //{
    //    if (this.CheckCreateTime(_createServerTime) == false) return;
    //    switch (BuffType)
    //    {
    //        case Define.BuffType.Shoes:
    //        case Define.BuffType.Shield:
    //        case Define.BuffType.Speed:
    //            EffectManager.Instance.EffectOnLocal(Define.EffectType.BuffEffect, this.transform.position, 1);
    //            break;

    //    }

    //}


    private void Update()
    {
        if (_buffBase == null)
            return;

        var remainTime = (float)PhotonNetwork.Time - (_createServerTime + _durationTime);

        if ((float)PhotonNetwork.Time > _createServerTime + _durationTime)
        {
            End();
        }
    }

    public void End()
    {
        _buffBase.Push();
        _createServerTime = 0;
        BuffType = Define.BuffType.Null;
        foreach (var render in _buffBase.renderers)
        {
            livingEntity.fogController.RemoveRenderer(render);
        }
        livingEntity.RemoveBuffController(this);
        livingEntity = null;
        _buffBase = null;

        Managers.Pool.Push(_poolable);
    }


    //긍정버프인지, 너프인지,
    void SetupIsPostiveBuff(Define.BuffType buffType)
    {
        switch (buffType)
        {
            case Define.BuffType.Direction:
            case Define.BuffType.Sight:
            case Define.BuffType.Stun:
                IsNuff = true;
                break;
            default:
                IsNuff = false;
                break;

        }
    }

}
