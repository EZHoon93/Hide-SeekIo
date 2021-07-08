using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using DG.Tweening;

public class GetWorldItemController : MonoBehaviourPun , IPunInstantiateMagicCallback, IEnterTrigger, IExitTrigger ,
     IPunObservable , IPunOwnershipCallbacks
{
    [SerializeField] LivingEntity _gettingLivingEntity; //얻고있는 생명체
    [SerializeField] Slider _getSlider;     //얻고있는 UI
    [SerializeField] Transform _modelTransform;     //모델 객체 collect Effect할

    [SerializeField] float _maxGetTime;    //얻기 위해필요한시간
    [SerializeField] private float _shakeScaleDuration = 1;
    [SerializeField] private float _hideScaleDuration = .25f;
    [SerializeField] public int Value = 1;

    float n_eneterTime;  //얻을 때 시간
    bool _isGet; //얻으면 
    IGetWorldItem getWorldItem;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(n_eneterTime);
        }
        else
        {
            n_eneterTime = (float)stream.ReceiveNext();
        }
    }

    private void Awake()
    {
        _getSlider.maxValue = _maxGetTime;
        getWorldItem = GetComponent<IGetWorldItem>();
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        this.transform.localScale = Vector3.one;
        n_eneterTime = 0;
        _gettingLivingEntity = null;
        _isGet = false;
        _getSlider.value = 0;
        PhotonNetwork.AddCallbackTarget(this);
    }


    private void Update()
    {

        if(n_eneterTime != 0)
        {
            var currentGetTime =  (float)(PhotonNetwork.Time - n_eneterTime);
            _getSlider.value = currentGetTime;
            if (currentGetTime >= _maxGetTime)
            {
                //효과
                if (_isGet == false)
                {
                    CollectEffect();
                }
            }
        }
        else
        {
            _getSlider.value = 0;

        }

    }

    //시간지나 얻었을떄
    public void CollectEffect()
    {
        print("Collect Effect!! ");
        _isGet = true;
        this.transform.DOShakeScale(_shakeScaleDuration);
        this.transform.DOScale(Vector3.zero, _hideScaleDuration).SetDelay(_shakeScaleDuration);

        getWorldItem.Get(_gettingLivingEntity.gameObject);  //아이템 얻기 효과
        Invoke("AfaterDestroy", 2.0f);  //Destroy
    }

    void AfaterDestroy()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }


    //누군가 얻으려고할때 => 얻으려는 로컬 오브젝트가 서버전송
    public void Enter(GameObject Gettingobject)
    {
        this._modelTransform.DOShakeScale(_shakeScaleDuration*0.5f);

        if (_gettingLivingEntity != null) return;   //이미 얻고있는 플레이어가 있다면 취소
        var living =  Gettingobject.GetComponent<LivingEntity>();
        if (living.photonView.IsMine == false) return;  //얻은캐릭이 자기자신캐릭이아니라면 x
        if (living)
        {
            photonView.RPC("Check_IsGetOnServer", RpcTarget.AllViaServer, living.ViewID());
        }
    }

    //동시에 얻는걸 막기위해 서버 전송후 체크 
    [PunRPC]
    public void Check_IsGetOnServer(int getViewID , PhotonMessageInfo photonMessageInfo)
    {
        if (_gettingLivingEntity != null) return;       //누군가 이미 얻었다면  return

        var newGetLivingEntity = Managers.Game.GetLivingEntity(getViewID);
        if (newGetLivingEntity == null) return; //없으면 X

        //변수 할당
        _gettingLivingEntity = newGetLivingEntity;
        n_eneterTime = (float)photonMessageInfo.SentServerTime;
        //권한 넘김
        this.photonView.TransferOwnership(photonMessageInfo.Sender.ActorNumber);

    }

    //로컬 오브젝트만 실행
    public void Exit(GameObject exitGameObject)
    {
        if (photonView.IsMine == false) return;
        var exitLivingEntity = exitGameObject.GetComponent<LivingEntity>();
        if(_gettingLivingEntity == exitLivingEntity)
        {
            _gettingLivingEntity = null;
            this.photonView.TransferOwnership(0);   //중립오브젝트로 전환
        }
    }
  

  

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView.AmOwner == false)
        {
            _gettingLivingEntity = null;
            n_eneterTime = 0;
        }
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
    }
}
