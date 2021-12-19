using System;

using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(InputControllerObject))]
[RequireComponent(typeof(Equipmentable))]
[RequireComponent(typeof(RenderController))]

public abstract class Weapon : MonoBehaviourPun,  IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy, IPunObservable
{
    [SerializeField] protected Transform _weaponModel;
    [SerializeField] Transform _leftHand;
    [SerializeField] Transform _rightHand;
    [SerializeField] protected UI_ZoomBase _uI_ZoomBase;
    [SerializeField] int _maxEnergy;
    [SerializeField] float _energyRegenAmount;
    [SerializeField] float _attackDistance;
    public enum WeaponType
    {
        Melee,
        Hammer,
        Throw,
        Gun,
        Bow
    }
    public enum UseState
    {
        Use,
        NoUse
    }

    public enum HandType
    {
        Left,
        Right
    }

    public abstract WeaponType weaponType { get; }
    public abstract HandType handType { get; }


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

    public string AttackAnim { get; set; }
    public float AttackDelay { get; set; }
    public float AfaterAttackDelay { get; set; }
    public float AttackDistance { get; set; }

    public int maxEnergy => _maxEnergy;

    public float energyRegenAmount => _energyRegenAmount;
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
    protected virtual void Awake()
    {
        equipmentable = GetComponent<Equipmentable>();
        renderController = GetComponent<RenderController>();
        SetupCallBack();
        equipmentable.Setup(_weaponModel, Define.SkinType.RightHand);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputVector"></param>
    public abstract void Attack(Vector2 inputVector);

    /// <summary>
    /// 
    /// </summary>
    protected virtual void SetupCallBack()
    {
        inputControllerObject = this.gameObject.GetOrAddComponent<InputControllerObject>();
        inputControllerObject.AddUseEvent(Attack);
        inputControllerObject.AddZoomEvent(Zoom);
        inputControllerObject.InitCoolTime = 3;
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null) return;
        var playerViewID = (int)info.photonView.InstantiationData[0];
        playerController = Managers.Game.GetPlayerController(playerViewID);
        if (playerController == null) return;
        
        if (playerController.photonView.IsMine)
        {
            this.photonView.TransferOwnership(playerController.photonView.CreatorActorNr);
        }

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
        renderController.OnDestroyEvent();
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
 
    public void WeaponChange(int instanceID)
    {
        useState = instanceID == (GetInstanceID()) ? UseState.Use : UseState.NoUse;
    }

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

}
