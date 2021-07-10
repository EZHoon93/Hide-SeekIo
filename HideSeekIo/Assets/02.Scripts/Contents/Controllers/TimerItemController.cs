
using UnityEngine;
using Photon.Pun;

public class TimerItemController : MonoBehaviourPun, IPunInstantiateMagicCallback , IOnPhotonViewPreNetDestroy
{
    TimerItem _timerItem;
    Define.TimerItem _itemEnum;
    float _durationTime;
    float _craeteTime;
    
    public float RemainTime { get; private set; }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null) return;
        _craeteTime = (float)info.SentServerTime;
        _itemEnum = (Define.TimerItem)info.photonView.InstantiationData[0];
        _durationTime = (float)info.photonView.InstantiationData[1];

         _timerItem =  Managers.Resource.Instantiate($"TimerItem/{_itemEnum.ToString()}").GetComponent<TimerItem>();
    }

    private void Update()
    {
        RemainTime  =  (float)( PhotonNetwork.Time - (_craeteTime + _durationTime));
        if(RemainTime <=  0)
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }

    public void OnPreNetDestroy(PhotonView rootView)
    {
        _timerItem.EndTime();
        Managers.Resource.Destroy(_timerItem.gameObject);
    }
}
