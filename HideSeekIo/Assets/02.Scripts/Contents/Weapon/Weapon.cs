using System;

using Photon.Pun;
using UnityEngine;

public abstract class Weapon : MonoBehaviourPun,  IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy, IPunObservable
{
    [SerializeField] protected Transform _weaponModel;

    public enum WeaponType
    {
        Melee,
        Hammer,
        Throw,
        Gun
    }
    public enum UseType
    {
        Permanent,//영구적
        Disposable  //일회용
    }

    public enum UseState
    {
        Use,
        NoUse
    }


    public WeaponType weaponType { get; protected set; }
    public UseType type { get; set; }
    UseState _useState;
    public UseState useState
    {
        get => _useState;
        set
        {
            var newState = value;
            if (_useState == newState) return;
            _useState = newState;
            if (_useState == UseState.Use)
            {
                //playerController.playerShooter.ChangeWeapon(this);
                _weaponModel.gameObject.SetActive(true);
            }
            else
            {
                _weaponModel.gameObject.SetActive(false);
            }
        }
    }

    public string AttackAnim { get; set; }
    public float AttackDelay { get; set; }
    public float AfaterAttackDelay { get; set; }
    public float AttackDistance { get; set; }
    public PlayerController playerController => inputControllerObject.playerController;
    public InputControllerObject inputControllerObject { get; set; }
    public Equipmentable equipmentable { get; set; }
    public UI_Zoom uiZoom { get; set; }
    public RenderController renderController { get; protected set; }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(useState);
        }
        else
        {
            var n_state = (UseState)stream.ReceiveNext();
            if (useState == n_state) return;
            if (playerController == null) return;
            useState = n_state;
        }
    }

    protected virtual void Awake()
    {
        equipmentable = this.gameObject.GetOrAddComponent<Equipmentable>();
        renderController = this.gameObject.GetOrAddComponent<RenderController>();
        equipmentable.Setup(Define.SkinType.Weapon);
        SetupCallBack();
    }
    public abstract void Attack(Vector2 inputVector);

    protected virtual void SetupCallBack()
    {
        inputControllerObject = this.gameObject.GetOrAddComponent<InputControllerObject>();
        inputControllerObject.AddUseEvent(Attack);
        inputControllerObject.AddZoomEvent(Zoom);
        inputControllerObject.InitCoolTime = 3;
    }
    //public void Setup(string animName, float delayTime, float afaterDelayTime, float distance, float newAttackRange)
    //{
    //    AttackAnim = animName;
    //    AttackDelay = delayTime;
    //    AfaterAttackDelay = afaterDelayTime;
    //    AttackDistance = distance;
    //}

    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null) return;
        useState = UseState.NoUse;  //사용하지않음으로설정
        _weaponModel.gameObject.SetActive(false);
        var playerViewID = (int)info.photonView.InstantiationData[0];
        var playerController = Managers.Game.GetPlayerController(playerViewID);
        inputControllerObject.SetupPlayerController(playerController);
        playerController.playerShooter.SetupWeapon(this);
        CreateZoomUI(playerController);  //줌 UI생성

    }
    public virtual void OnPreNetDestroy(PhotonView rootView)
    {
        if (Managers.Resource == null) return;
        if (playerController)
        {
            playerController.playerHealth.RemoveRenderer(renderController);
           Managers.Resource.Destroy(uiZoom.gameObject);
           //destroyEventCallBack?.Invoke();
        }
    }

    protected virtual void CreateZoomUI(PlayerController hasMyController)
    {
        if (playerController.IsMyCharacter() == false)
        {
            return;
        }
        if(uiZoom == null)
        {
            uiZoom = Managers.Resource.Instantiate("Contents/ZoomUI", this.transform).GetComponent<UI_Zoom>();
        }
        uiZoom.Setup(weaponType,this, hasMyController.transform);
    }



    public virtual void Zoom(Vector2 inputVector)
    {
  
    }
 
    public void WeaponChange(int instanceID)
    {
        useState = instanceID == (GetInstanceID()) ? UseState.Use : UseState.NoUse;
    }

  

}
