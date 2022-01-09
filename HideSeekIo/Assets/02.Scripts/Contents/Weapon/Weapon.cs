using System;

using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(InputControllerObject))]
[RequireComponent(typeof(Equipmentable))]
[RequireComponent(typeof(RenderController))]

public abstract class Weapon : MonoBehaviourPun,  IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy, IPunObservable
{
    public enum WeaponType {
        Melee,
        Hammer,
        Throw,
        Gun,
        Bow ,
        TwoHandHammer

    }
    public enum UseState {
        Use,
        NoUse
    }

  

    public abstract WeaponType weaponType { get; }


    private UseState _useState;
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


    [SerializeField] protected Transform _weaponModel;
    [SerializeField] protected UI_ZoomBase _uI_ZoomBase;
    //public float _attackDistance;


    public string AttackAnim { get; set; }
    public float AttackDelay;
    public float AfaterAttackDelay;
    public float AttackDistance;


    public Vector3 attackPoint { get; protected set; }


    public PlayerController playerController { get; private set; }
    public InputControllerObject inputControllerObject { get; set; }
    public Equipmentable equipmentable { get; set; }
    public RenderController renderController { get; protected set; }
    public UI_ZoomBase uI_ZoomBase => _uI_ZoomBase;


    public Action attackStartCallBack;
    public Action attackEndCallBack;

    [SerializeField] protected int _damage;


    /// <summary>
    /// 
    /// </summary>
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
        equipmentable = GetComponent<Equipmentable>();
        renderController = GetComponent<RenderController>();
        inputControllerObject = GetComponent<InputControllerObject>();
        inputControllerObject.AddZoomEvent(Zoom);
        inputControllerObject.AddUseEvent(Attack);
    }

   

    /// <summary>
    /// 
    /// </summary>
    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var infoData = info.photonView.InstantiationData;
        if (infoData == null)
        {
            return;
        }

        var playerViewID = (int)info.photonView.InstantiationData[0];
        playerController = Managers.Game.GetPlayerController(playerViewID);

        if (playerController == null)
        {
            return;
        }
        // 해당 컨트롤러에게 컨트롤러권을 넘김
        if (playerController.photonView.IsMine)
        {
            this.photonView.ControllerActorNr = playerController.photonView.ControllerActorNr;
        }

        Setup(infoData);
    }

    protected virtual void Setup(object[] infoData)
    {
        inputControllerObject.OnPhotonInstantiate(playerController);
        equipmentable.OnPhotonInstantiate(playerController);
        renderController.OnPhotonInstantiate(playerController.playerHealth);
        playerController.playerShooter.SetupWeapon(this);
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void OnPreNetDestroy(PhotonView rootView)
    {
        if (Managers.Resource == null) return;
        inputControllerObject.OnDestroyEvent();
        renderController.OnPreNetDestroy();
        equipmentable.OnDestroyEvent();
        _uI_ZoomBase.SetActiveZoom(false);
        if (playerController)
        {
            playerController.playerHealth.RemoveRenderer(renderController);
        }
    }


    public virtual void Zoom(Vector2 inputVector)
    {
    }
    public abstract void Attack(Vector2 inputVector);


    public void WeaponChange(int instanceID)
    {
        useState = instanceID == (GetInstanceID()) ? UseState.Use : UseState.NoUse;
    }

   

}
