using Photon.Pun;
using UnityEngine;

public abstract class Weapon : MonoBehaviourPun , IAttack , IPunInstantiateMagicCallback
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


    public AttackBase newAttacker { get; set; }
    public string AttackAnim => _attackAnimationName;
    public float AttackDelay => _attackDelayTime;
    public float AfaterAttackDelay => _afterAttackDelayTime;
    public float AttackDistance => _distance;
    public Vector3 AttackDirecition { get; protected set; }     //공격 박향. 캐릭터 바라보는방향으로맞추기위해 
    public WeaponType weaponType { get; protected set; }
    public Define.Weapon weaponServerKey { get; protected set; }    //웨펀의 이름, 키
    public GameObject UICanvas => _uICanvas;
    public abstract void Zoom(Vector2 inputVector);
    public abstract bool Attack(Vector2 inputVector);


    private void OnDisable()
    {
        if (Managers.Resource == null) return;
        _uICanvas.transform.SetParent(this.transform);
    }

    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null) return;
        var playerViewID = (int)info.photonView.InstantiationData[0];
        var attackBase = GameManager.Instance.GetLivingEntity(playerViewID).GetComponent<AttackBase>();
        attackBase.SetupWeapon(this);

    }
}
