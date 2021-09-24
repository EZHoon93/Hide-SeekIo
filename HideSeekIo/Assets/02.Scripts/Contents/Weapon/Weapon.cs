using System;

using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(InputControllerObject))]
[RequireComponent(typeof(Equipmentable))]
[RequireComponent(typeof(RenderController))]

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
    public enum UseState
    {
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

    public string AttackAnim { get; set; }
    public float AttackDelay { get; set; }
    public float AfaterAttackDelay { get; set; }
    public float AttackDistance { get; set; }

    public PlayerController playerController { get; private set; }
    public InputControllerObject inputControllerObject { get; set; }
    public Equipmentable equipmentable { get; set; }
    public RenderController renderController { get; protected set; }
    public UI_Zoom uiZoom { get; set; }


    /// <summary>
    /// 
    /// </summary>
    protected virtual void Awake()
    {
        equipmentable = this.gameObject.GetOrAddComponent<Equipmentable>();
        renderController = GetComponent<RenderController>();
        SetupCallBack();
        equipmentable.Setup(_weaponModel, Define.SkinType.Weapon);
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
    /// <param name="info"></param>
    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null) return;
        useState = UseState.NoUse;  //사용하지않음으로설정
        var playerViewID = (int)info.photonView.InstantiationData[0];
        playerController = Managers.Game.GetPlayerController(playerViewID);
        Managers.InputItemManager.SetupWeapon(playerController, this);
        inputControllerObject.OnPhotonInstantiate(playerController);
        equipmentable.OnPhotonInstantiate(playerController);
        renderController.OnPhotonInstantiate(playerController.playerHealth);
        CreateZoomUI(playerController);  //줌 UI생성
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rootView"></param>

    public virtual void OnPreNetDestroy(PhotonView rootView)
    {
        if (Managers.Resource == null) return;
        inputControllerObject.OnDestroyEvent();
        renderController.OnDestroyEvent();
        equipmentable.OnDestroyEvent();
        if (playerController)
        {
            playerController.playerHealth.RemoveRenderer(renderController);
          
        }
        if (uiZoom)
        {
            uiZoom.transform.SetParent(this.transform);
            uiZoom.gameObject.SetActive(false);
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>

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
