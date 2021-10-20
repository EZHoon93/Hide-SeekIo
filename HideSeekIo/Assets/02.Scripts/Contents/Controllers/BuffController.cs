
using UnityEngine;
using Photon.Pun;
using TMPro;

/// <summary>
/// 서버통신을위한 버프컨트롤러,
/// </summary>
public class BuffController : MonoBehaviourPun, IPunObservable
{
    float n_durationTime;   //지속시간
    float n_createServerTime;   //서버에서의 생성시간
    BuffBase _buffBase;
    Poolable _poolable;

    public Define.BuffType BuffType { get; private set; }
    public float ReaminTime => ((n_durationTime + n_createServerTime) - (float)PhotonNetwork.Time);
    public LivingEntity livingEntity { get; private set; }

    public bool IsNuff { get; private set; }    //너프 인지

  
    private void Awake()
    {
        _poolable = GetComponent<Poolable>();
    }

    private void Update()
    {
        if (BuffType == Define.BuffType.Null)
            return;

        var remainTime =  (n_createServerTime + n_durationTime)- (float)PhotonNetwork.Time;
        if (remainTime <= 0)
        {
            End();
        }
    }

    public void SetupLivingEntitiy(LivingEntity newLivingEntity)
    {
        livingEntity = newLivingEntity;
    }

    //재갱신 및 최초
    public void SetupInfo(Define.BuffType buffType, float createServerTime ,float durationTime)
    {
        if (buffType == Define.BuffType.Null) return;
        n_createServerTime = createServerTime;

        if (_buffBase == null)
        {
            BuffType = buffType;
            _buffBase = BuffManager.Instance.MakeBuffObject(buffType, this.transform);
            _buffBase.Setup(this);
            n_durationTime = durationTime;
            livingEntity.AddRenderer(_buffBase.renderController);
        }
        Play();
    }
    //재갱신 
    public void Renew(float createServerTime)
    {
        n_createServerTime = createServerTime;
    }

    public void Play()
    {
        var createAfaterTime =   (float)PhotonNetwork.Time - n_createServerTime;
        if(createAfaterTime < 0.3f)
        {
            PlayEffect();
        }

        _buffBase.Play();
    }
    // 버프생성시 이펙트
    void PlayEffect()
    {
        _buffBase.PlayEffect();
    }

    public void End()
    {
        BuffManager.Instance.RemoveBuffController(livingEntity, this);
        _buffBase.Push();
        livingEntity.RemoveRenderer(_buffBase.renderController);
        BuffType = Define.BuffType.Null;
        n_createServerTime = 0;
        livingEntity = null;
        _buffBase = null;
        Managers.Pool.Push(_poolable);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(BuffType);
            stream.SendNext(n_createServerTime);
            stream.SendNext(n_durationTime);
        }
        else
        {
            var r_BuffType = (Define.BuffType)stream.ReceiveNext();
            var r_createServerTime = (float)stream.ReceiveNext();
            var r_durationTime = (float)stream.ReceiveNext();

            if (BuffType == r_BuffType) return;
            SetupInfo(r_BuffType, r_createServerTime, r_durationTime);
        }

    }
}
