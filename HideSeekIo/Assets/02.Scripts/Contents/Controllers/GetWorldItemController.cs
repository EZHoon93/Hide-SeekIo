using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using DG.Tweening;
using FoW;

public class GetWorldItemController : MonoBehaviourPun , IPunInstantiateMagicCallback, ICanEnterTriggerPlayer, ICanExitTriggerPlayer ,
     IPunObservable , IPunOwnershipCallbacks, IOnPhotonViewPreNetDestroy
{
    //public LivingEntity gettingLivingEntity { get; set; } //얻고있는 생명체
    public LivingEntity gettingLivingEntity;//얻고있는 생명체

    [SerializeField] Slider _getSlider;     //얻고있는 UI
    [SerializeField] Transform _modelTransform;     //모델 객체 collect Effect할
    HideInFog _hideInFog;
    [SerializeField] float _maxGetTime;    //얻기 위해필요한시간
    [SerializeField] private float _shakeScaleDuration = 1;
    [SerializeField] private float _hideScaleDuration = .25f;
    [SerializeField] public int Value = 1;

    

    float n_eneterTime;  //얻을 때 시간
    float n_getTime;    //얻었던 시간
    bool _isGet; //얻으면 

    public int controllerIndex { get; private set; }
    public int spawnIndex { get; private set; }
    public bool isReset { get; set; }
    [SerializeField] ItemBox_Base  _itemBox_Base;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(n_eneterTime);
            stream.SendNext(n_getTime);
            stream.SendNext(_isGet);

        }
        else
        {
            n_eneterTime = (float)stream.ReceiveNext();
            n_getTime = (float)stream.ReceiveNext();
            _isGet = (bool)stream.ReceiveNext();
        }
    }

    private void Awake()
    {
        _getSlider.maxValue = _maxGetTime;
        _itemBox_Base = GetComponent<ItemBox_Base>();
        _hideInFog = GetComponent<HideInFog>();
    }
    private void OnEnable()
    {
        _isGet = false;
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null) return;
        spawnIndex = (int)info.photonView.InstantiationData[0];
        controllerIndex = (int)info.photonView.InstantiationData[1];

        _itemBox_Base.OnPhotonInstantiate(info);


        isReset = false;
        Managers.Game.CurrentGameScene.itemSpawnManager.CreateCallBack(this);
        this.transform.localScale = Vector3.one;
        n_eneterTime = 0;
        gettingLivingEntity = null;
        _isGet = false;
        _getSlider.value = 0;
        n_getTime = 0;
        PhotonNetwork.AddCallbackTarget(this);
        _getSlider.gameObject.SetActive(false);
    }

    public void OnPreNetDestroy(PhotonView rootView)
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        _itemBox_Base.OnPreNetDestroy(rootView);
        if (isReset) return;
        Managers.Game.CurrentGameScene.itemSpawnManager.RemoveCallBack(this);
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

        if (_isGet == false) return;
        //시간이 지났는데도 파괴가되지않았다면.. 
        if((float)PhotonNetwork.Time - n_getTime >= 7)
        {
            if (photonView.IsMine)
            {
                n_getTime = (float)PhotonNetwork.Time;
                Managers.Resource.PunDestroy(this);
            }
        }

    }

    //시간지나 얻었을떄
    public void CollectEffect()
    {
        _isGet = true;
        this.transform.DOShakeScale(_shakeScaleDuration);
        this.transform.DOScale(Vector3.zero, _hideScaleDuration).SetDelay(_shakeScaleDuration);

        if (photonView.IsMine == false || gettingLivingEntity == null) return;
        if (gettingLivingEntity.photonView.IsMine)
        {
            _itemBox_Base.Get(gettingLivingEntity.gameObject);  //아이템 얻기 효과
            n_getTime = (float)PhotonNetwork.Time;
            if (gettingLivingEntity.IsMyCharacter())
            {
                Managers.Sound.Play("Get", Define.Sound.Effect);
            }
            //PhotonNetwork.Destroy(this.gameObject);
            Invoke("AfaterDestroy", 2.0f);  //Destroy

        }

    }

    void AfaterDestroy()
    {
        Managers.Resource.PunDestroy(this);
    }


    //누군가 얻으려고할때 => 얻으려는 로컬 오브젝트가 서버전송
    public void Enter(PlayerController enterPlayer, Collider collider)
    {
        //print("Enter Item!!");
        //this._modelTransform.DOShakeScale(_shakeScaleDuration * 0.5f);

        //if (gettingLivingEntity != null) return;   //이미 얻고있는 플레이어가 있다면 취소
        //var living = enterPlayer.playerHealth;
        //if (living.photonView.IsMine == false) return;  //얻은캐릭이 자기자신캐릭이아니라면 x
        //if (living)
        //{

        //    if (living.IsMyCharacter())
        //    {
        //        _hideInFog.enabled = false;
        //        _hideInFog.SetActiveRender(true);
        //        Managers.Sound.Play("Getting", Define.Sound.Effect);
        //    }
        //    photonView.RPC("Check_IsGetOnServer", RpcTarget.AllViaServer, living.ViewID());
        //}
    }
    //동시에 얻는걸 막기위해 서버 전송후 체크 
    [PunRPC]
    public void Check_IsGetOnServer(int getViewID , PhotonMessageInfo photonMessageInfo)
    {
        if (gettingLivingEntity != null) return;       //누군가 이미 얻었다면  return

        var newGetLivingEntity = Managers.Game.GetLivingEntity(getViewID);
        if (newGetLivingEntity == null) return; //없으면 X

        //변수 할당
        gettingLivingEntity = newGetLivingEntity;
        n_eneterTime = (float)photonMessageInfo.SentServerTime;
        if (gettingLivingEntity.IsMyCharacter())
        {
            _getSlider.gameObject.SetActive(true);
        }
        //권한 넘김
        this.photonView.TransferOwnership(photonMessageInfo.Sender.ActorNumber);
        //this.photonView.trasn
    }

    //로컬 오브젝트만 실행
    public void Exit(GameObject exitGameObject)
    {
    }


    public void Exit(PlayerController exitPlayer, Collider collider)
    {

        if (photonView.IsMine == false) return;
        var exitLivingEntity = exitPlayer.playerHealth;
        if (exitLivingEntity == null) return;
        if (gettingLivingEntity == exitLivingEntity && n_eneterTime > 0)
        {
            gettingLivingEntity = null;
            this.photonView.TransferOwnership(0);   //중립오브젝트로 전환
            _getSlider.gameObject.SetActive(false);
            _hideInFog.enabled = true;

        }
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {

    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (this == null) return;
        if (targetView.AmOwner == false)
        {
            if(this.ViewID() == targetView.ViewID)
            {
                gettingLivingEntity = null;
                n_eneterTime = 0;
            }
            
            
        }
        else
        {
        }
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
    }

   
}
