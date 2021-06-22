using UnityEngine;
using Photon.Pun;

public abstract class Weapon_Throw : Weapon
{
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] Transform _attackRangeUI;

    public float attackRange { get; protected set; }

    protected virtual void Awake()
    {
        weaponType = WeaponType.Throw;
    }
    public void Setup(string animName, float delayTime, float afaterDelayTime, float distance, float newAttackRange)
    {
        _attackAnimationName = animName;
        _attackDelayTime = delayTime;
        _afterAttackDelayTime = afaterDelayTime;
        _distance = distance;
        attackRange = newAttackRange;
    }
    public override bool Attack(Vector2 inputVector)
    {
        AttackToServer(inputVector);
        return true;

    }
    void AttackToServer(Vector2 inputVector)
    {
        Vector3 startPoint = newAttacker.CenterPivot.position;
        Vector3 endPoint = UtillGame.GetThrowPosion(inputVector, _distance, newAttacker.transform);
        AttackProcess(startPoint, endPoint);
        //photonView.RPC("AttackProcess", RpcTarget.All, startPoint, endPoint);
    }
    [PunRPC]
    public void AttackProcess(Vector3 startPoint, Vector3 endPoint)
    {
        var projectile = Managers.Pool.Pop(_projectilePrefab).GetComponent<ThrowProjectileObject>();
        projectile.Play(startPoint, endPoint);
    }
    public override void Zoom(Vector2 inputVector)
    {
        UtillGame.ThrowZoom(inputVector, _distance, newAttacker.CenterPivot, _attackRangeUI);
    }

}
