
using UnityEngine;
using Photon.Pun;
using System.Linq;

/// <summary>
/// 서버통신을위한 버프컨트롤러,
/// </summary>
public class BuffController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] float _durationTime;
    [SerializeField] float _createServerTime;
    [SerializeField] BuffBase _buffBase;

    public Define.BuffType BuffType { get; private set; }
    public float ReaminTime => ((_durationTime + _createServerTime) - (float)PhotonNetwork.Time);
    public LivingEntity livingEntity { get; private set; }
    public float DurationTime => _durationTime;

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
            if (n_BuffType == Define.BuffType.Null)
                return;
            Setup(n_BuffType, n_createServerTime, n_durationTime);
        }

    }

    //재갱신 및 최초
    public void Setup(Define.BuffType buffType, float createServerTime, float durationTime)
    {
        //if (_createServerTime == createServerTime) return;
      
        if (_buffBase == null )
        {
            _buffBase = BuffManager.Instance.MakeBuffObject(buffType, this.transform);
        }
        BuffType = buffType;
        _createServerTime = createServerTime;
        _durationTime = durationTime;
    }


    private void Update()
    {
        if (_buffBase == null)
            return;

        var remainTime = (float)PhotonNetwork.Time - (_createServerTime + _durationTime);
        if ((float)PhotonNetwork.Time > _createServerTime + _durationTime)
        {
            Destroy(this.gameObject);
        }


    }




}
