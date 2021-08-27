using System;

using Photon.Pun;
using UnityEngine;

public abstract class Weapon : MonoBehaviourPun,  IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy, IPunObservable
{

    public enum WeaponType
    {
        Melee,
        Hammer,
        Throw,
        Gun
    }
    public enum State
    {
        Delay,
        End
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

    [SerializeField] protected Transform _weaponModel;

    public InputType inputType { get; protected set; } 
    public WeaponType weaponType { get; protected set; }
    public State state { get; set; }
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
                playerController.playerShooter.ChangeWeapon(this);
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
  
    public Vector3 AttackPoint { get; set; }
    public Vector2 LastAttackInput { get; protected set; }     //공격 박향. 캐릭터 바라보는방향으로맞추기위해

    public PlayerController playerController { get; set; }
    public InputControllerObject inputControllerObject { get; set; }
    public Equipmentable equipmentable { get; set; }
    public UI_Zoom uiZoom { get; set; }

    //public Action AttackSucessEvent { get; set; }
    //public Action AttackEndEvent { get; set; }
    //public Action destroyEventCallBack;

    //public Define.ControllerType controllerType  { get; set;} =  Define.ControllerType.Joystick;

    public abstract void Attack(Vector2 inputVector);

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
        equipmentable.Setup(Define.SkinType.Weapon);
        SetupCallBack();
    }

    protected virtual void SetupCallBack()
    {
        inputControllerObject = this.gameObject.GetOrAddComponent<InputControllerObject>();
        //inputControllerObject = inputControllerObject ?? new InputControllerObject();
        inputControllerObject.AddUseEvent(Attack);
        inputControllerObject.AddZoomEvent(Zoom);

    }


    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null) return;
        useState = UseState.NoUse;  //사용하지않음으로설정
        _weaponModel.gameObject.SetActive(false);
        //AttackSucessEvent = null;
        //AttackEndEvent = null;
        var playerViewID = (int)info.photonView.InstantiationData[0];
        playerController = Managers.Game.GetPlayerController(playerViewID);
        //playerController.GetLivingEntity().fogController.AddHideRender(_weaponModel.GetComponentInChildren<Renderer>());
        playerController.playerShooter.SetupWeapon(this);
        //inputControllerObject.RemainCoolTime = 0;
        CreateZoomUI(playerController);  //줌 UI생성

    }
    public virtual void OnPreNetDestroy(PhotonView rootView)
    {
        if (Managers.Resource == null) return;
        if (playerController)
        {
           //playerController.GetLivingEntity().fogController.RemoveRenderer(_weaponModel.GetComponentInChildren<Renderer>());
           //destroyEventCallBack?.Invoke();
        }
    }

    protected virtual void CreateZoomUI(PlayerController hasMyController)
    {
        if (playerController.IsMyCharacter() == false)
        {
            return;
        }
        print("줌생성");
        uiZoom = Managers.Resource.Instantiate("Contents/ZoomUI", this.transform).GetComponent<UI_Zoom>();
        uiZoom.Setup(weaponType,this, hasMyController.transform);
        //inputControllerObject.zoomEventCallBack = _zom
        //destroyEventCallBack += () => Managers.Resource.Destroy(_zoomUI.gameObject);
        //destroyEventCallBack += () => inputControllerObject.RemoveEvent(ControllerInputType.Down, _zoomUI.Zoom);
    }



    public virtual void Zoom(Vector2 inputVector)
    {
        if (playerController?.IsMyCharacter() == false) return;
        //uiZoom.
        //if (inputVector.sqrMagnitude == 0)
        //{
        //    _zoomUI.gameObject.SetActive(false);
        //    return;
        //}
        //_zoomUI.gameObject.SetActive(true);
        //_zoomUI.rotation = UtillGame.WorldRotationByInput(inputVector);
        //_zoomUI.gameObject.SetActive(true);
        //useState = UseState.Use;
    }

    public virtual void Down()
    {

    }

    


    //public void AttackCheck(Vector2 inputVector)
    //{
    //    if(RemainCoolTime > 0)
    //    {
    //        return ;
    //    }
    //    RemainCoolTime = InitCoolTime;
    //    Attack(inputVector);
    //    return ;
    //}

    public void WeaponChange(int instanceID)
    {
        if(instanceID == GetInstanceID())
        {
            useState = UseState.Use;
            print("체인지..."+useState);

        }
        else
        {
            useState = UseState.NoUse;
            print("체인지..." + useState);

        }
    }

    //private void Update()
    //{
    //    if (playerController == null) return;
    //    if (RemainCoolTime >= 0)
    //    {
    //        RemainCoolTime -= Time.deltaTime;
    //        if (playerController.IsMyCharacter())
    //        {
    //            //InputManager.Instance.GetControllerJoystick(inputType)._UI_Slider_CoolTime.UpdateCoolTime(InitCoolTime, RemainCoolTime);
    //        }
    //    }

    //}

   
}
