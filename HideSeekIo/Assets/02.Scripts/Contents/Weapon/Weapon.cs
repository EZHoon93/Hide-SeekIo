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
    public AttackBase attackPlayer { get; set; }

    public Action AttackSucessEvent;
    public Action AttackEndEvent;

    public UseState useState
    {
        get => _useState;
        set
        {
            _useState = value;
            if(_useState == UseState.Use)
            {
                _weaponModel.gameObject.SetActive(true);
            }
            else
            {
                switch (weaponType)
                {
                    case WeaponType.Throw:
                        _weaponModel.gameObject.SetActive(false);
                        break;
                }
            }
        }
    }

    public abstract void Zoom(Vector2 inputVector);
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
            if (attackPlayer == null) return;
            useState = n_state;
            attackPlayer.UseWeapon(this);
        }
    }

    protected virtual void Awake()
    {
        UICanvas = GetComponentInChildren<Canvas>().gameObject;
        //UICanvas.SetActive(false);
    }
    
   
    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null) return;
        useState = UseState.NoUse;  //사용하지않음으로설정
        UICanvas.SetActive(false);
        AttackSucessEvent = null;
        AttackEndEvent = null;
        var playerViewID = (int)info.photonView.InstantiationData[0];
        var livingEntity = Managers.Game.GetLivingEntity(playerViewID);
        livingEntity.fogController.AddHideRender(_weaponModel.GetComponentInChildren<Renderer>());
        attackPlayer = livingEntity.GetComponent<AttackBase>();
        attackPlayer.SetupWeapon(this);
        ReaminCoolTime = 0;
    }
    public void OnPreNetDestroy(PhotonView rootView)
    {
        if (Managers.Resource == null) return;
        UICanvas.transform.SetParent(this.transform);
        if (attackPlayer)
        {
            var livingEntity = attackPlayer.GetComponent<LivingEntity>();
            if (livingEntity == null) return;
            livingEntity.fogController.RemoveRenderer(_weaponModel.GetComponentInChildren<Renderer>());
        }
    }

    
    //public void UseWeapon(AttackBase newAttackPlayer , Action newAttackSucessEvent, Action newAttackEndEvent)
    //{
    //    attackPlayer = newAttackPlayer;
    //    AttackSucessEvent = newAttackSucessEvent;
    //    AttackEndEvent = newAttackEndEvent;
    //    _weaponModel.gameObject.SetActive(true);
    //    useState = UseState.Use;
    //    //newAttackPlayer.Attack
    //}
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

    private void Update()
    {
        if (ReaminCoolTime >= 0)
        {
            ReaminCoolTime -= Time.deltaTime;
        }
    }

   
}
