
using UnityEngine;
using Photon.Pun;

public class TimerItemController : MonoBehaviourPun, IPunInstantiateMagicCallback , IOnPhotonViewPreNetDestroy
{
    TimerItem _timerItem;
    float _craeteTime;
    bool _isEnd;

    public LivingEntity usePlayer { get; private set; }
    //public int UserViewID { get; private set; }
    public float DurationTime { get; private set; }
    public float RemainTime { get; private set; }

    private void Awake()
    {
        _timerItem = GetComponent<TimerItem>();
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null) return;
        _craeteTime = (float)info.SentServerTime;
        DurationTime = (float)info.photonView.InstantiationData[1];
        var  userViewID = (int)info.photonView.InstantiationData[0];
        var userEnttity = Managers.Game.GetLivingEntity(userViewID);
        if (userEnttity)
        {
            usePlayer = userEnttity;
        }

        _isEnd = false;
        _timerItem.OnPhotonInstantiate(info);
    }

    private void Update()
    {
        if (_isEnd) return;
        RemainTime =  _craeteTime + DurationTime    - (float)PhotonNetwork.Time ;
        if(RemainTime <=  0)
        {
            _isEnd = true;

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
