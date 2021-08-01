using System;

using Photon.Pun;
using UnityEngine;

public abstract class Weapon : MonoBehaviourPun , IAttack , IPunInstantiateMagicCallback , IOnPhotonViewPreNetDestroy, IPunObservable
{
    public enum WeaponType
    {
        Melee,
        Throw,
        Gun
    }
    public enum State
    {
        Delay,
        End
    }
    public enum Type
    {
        Permanent   ,//영구적
        Disposable  //일회용
    }

    public enum UseState
    {
        Use,    
        NoUse
    }

    [SerializeField] protected Transform _weaponModel;

    public string AttackAnim { get; set; }
    public float AttackDelay { get; set; }
    public float AfaterAttackDelay { get; set; }
    public float AttackDistance { get; set; }
    public float InitCoolTime { get; set; }
    public float ReaminCoolTime { get; set; }
    public Vector2  LastAttackInput { get; protected set; }     //공격 박향. 캐릭터 바라보는방향으로맞추기위해 
    public WeaponType weaponType { get; protected set; }
    public State state { get;  set; }
    UseState _useState;
    public Type type { get; set; }
    public GameObject UICanvas { get; set; }
    public PlayerController hasPlayerController { get; set; }

    public Action<Weapon> AttackSucessEvent;
    public Action AttackEndEvent;

    public UseState useState
    {
        get => _useState;
        set
        {
            var newState = value;
            if (_useState == newState) return;
            _useState = newState;
            if(_useState == UseState.Use)
            {
                hasPlayerController.GetAttackBase().ChangeWeapon(this);
                _weaponModel.gameObject.SetActive(true);
            }
            else
            {
                _weaponModel.gameObject.SetActive(false);
            }
        }
    }

    public abstract bool Zoom(Vector2 inputVector);
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
            if (hasPlayerController == null) return;
            useState = n_state;
        }
    }
    protected virtual void Awake()
    {
        UICanvas = GetComponentInChildren<Canvas>().gameObject;
    }
    
   
    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null) return;
        UICanvas.SetActive(false);
        _useState = UseState.NoUse;
        _weaponModel.gameObject.SetActive(false);
        AttackSucessEvent = null;
        AttackEndEvent = null;
        var playerViewID = (int)info.photonView.InstantiationData[0];
        var isBaseWeapon = (bool)info.photonView.InstantiationData[1];  //영구적무기 여부 false면 일회용아이템
        hasPlayerController = Managers.Game.GetPlayerController(playerViewID);
        hasPlayerController.GetLivingEntity().fogController.AddHideRender(_weaponModel.GetComponentInChildren<Renderer>());
        hasPlayerController.GetAttackBase().SetupWeapon(this, isBaseWeapon);
        //useState = UseState.NoUse;  //사용하지않음으로설정
        ReaminCoolTime = 0;
    }
    public virtual void OnPreNetDestroy(PhotonView rootView)
    {
        if (Managers.Resource == null) return;
        UICanvas.transform.SetParent(this.transform);
        if (hasPlayerController)
        {
           hasPlayerController.GetLivingEntity().fogController.RemoveRenderer(_weaponModel.GetComponentInChildren<Renderer>());
        }
    }

    public bool AttackCheck(Vector2 inputVector)
    {
        if(ReaminCoolTime > 0)
        {
            return false;
        }
        ReaminCoolTime = InitCoolTime;
        Attack(inputVector);
        return true;
    }

    public void WeaponChange(int instanceID)
    {
        if(instanceID == GetInstanceID())
        {
            useState = UseState.Use;
        }
        else
        {
            useState = UseState.NoUse;
        }
    }

    private void Update()
    {
        if (ReaminCoolTime >= 0)
        {
            ReaminCoolTime -= Time.deltaTime;
        }
    }

   
}
