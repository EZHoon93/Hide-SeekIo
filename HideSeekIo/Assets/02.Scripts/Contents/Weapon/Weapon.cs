using Photon.Pun;
using UnityEngine;

public abstract class Weapon : MonoBehaviourPun , IAttack
{
    public enum WeaponType
    {
        Melee,
        Throw,
        Gun
    }
    [SerializeField]
    GameObject _uICanvas;
    
    protected string _attackAnimationName;
    protected float _attackDelayTime;   //대미지 주기까지 시간
    protected float _afterAttackDelayTime;  //다음움직임 까지 시간
    protected float _distance;


    public AttackBase newAttacker { get; set; }
    public string AttackAnim => _attackAnimationName;
    public float AttackDelay => _attackDelayTime;
    public float AfaterAttackDelay => _afterAttackDelayTime;
    public float AttackDistance => _distance;
    public WeaponType weaponType { get; protected set; }
    public Define.Weapon weaponServerKey { get; protected set; }    //웨펀의 이름, 키
    public GameObject UICanvas => _uICanvas;
    public abstract void Zoom(Vector2 inputVector);
    public abstract void Attack(Vector2 inputVector);

    private void OnDisable()
    {
        if (Managers.Resource == null) return;
        _uICanvas.transform.SetParent(this.transform);
    }




}
