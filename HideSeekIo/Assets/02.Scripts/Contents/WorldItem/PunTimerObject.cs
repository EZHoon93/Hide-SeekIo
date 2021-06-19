using System.Collections;

using UnityEngine;
using Photon.Pun;
public class PunTimerObject : MonoBehaviourPun, IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy
{

    float _craeteServerTime;   
    bool _isEnd;  
    int _usePlayerViewID;

    public float InitRemainTime { get;  protected set; }
    public float RemainTime => ((InitRemainTime + _craeteServerTime) - (float)PhotonNetwork.Time);



    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var datas = info.photonView.InstantiationData;
        if (datas == null) return;
        _craeteServerTime =(float) info.SentServerTime;
        _usePlayerViewID = (int)datas[0];
        _isEnd = false;
    }

    public virtual void OnPreNetDestroy(PhotonView rootView)
    {

    }


    private void Update()
    {
        if (_isEnd) return;
        if (RemainTime < 0)
        {
            _isEnd = true;
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }

}
