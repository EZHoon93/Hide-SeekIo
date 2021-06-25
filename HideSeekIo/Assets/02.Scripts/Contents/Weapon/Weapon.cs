using System;

using Photon.Pun;
using UnityEngine;

public abstract class Weapon : MonoBehaviourPun , IAttack , IPunInstantiateMagicCallback , IOnPhotonViewPreNetDestroy
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
    public State state;

    [SerializeField]
    protected GameObject _uICanvas;
    
    protected string _attackAnimationName;
    protected float _attackDelayTime;   //대미지 주기까지 시간
    protected float _afterAttackDelayTime;  //다음움직임 까지 시간
    protected float _distance;
    [SerializeField]
    protected float _initCoolTime;
    protected float _remianCoolTime;

    public string AttackAnim => _attackAnimationName;
    public float AttackDelay => _attackDelayTime;
    public float AfaterAttackDelay => _afterAttackDelayTime;
    public float AttackDistance => _distance;
    public float InitCoolTime => _initCoolTime;
    public float ReaminCoolTime => _remianCoolTime;

    public Vector2  LastAttackInput { get; protected set; }     //공격 박향. 캐릭터 바라보는방향으로맞추기위해 
    public WeaponType weaponType { get; protected set; }
    public GameObject UICanvas => _uICanvas;
    public AttackBase newAttacker { get; set; }

    public Action AttackSucessEvent;
    public Action AttackEndEvent;


    public abstract void Zoom(Vector2 inputVector);
    public abstract void Attack(Vector2 inputVector);

    public void OnPreNetDestroy(PhotonView rootView)
    {
        if (Managers.Resource == null) return;
        _uICanvas.transform.SetParent(this.transform);
    }
    private void OnEnable()
    {
    }
    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null) return;
        var playerViewID = (int)info.photonView.InstantiationData[0];
        var attackBase = Managers.Game.GetLivingEntity(playerViewID).GetComponent<AttackBase>();
        attackBase.SetupWeapon(this);
        _remianCoolTime = 0;
    }

    public bool AttackCheck(Vector2 inputVector)
    {
        if(_remianCoolTime > 0)
        {
            return false;
        }
        _remianCoolTime = _initCoolTime;
        Attack(inputVector);
        return true;
    }

    private void Update()
    {
        if (_remianCoolTime >= 0)
        {
            _remianCoolTime -= Time.deltaTime;
        }
    }

}
