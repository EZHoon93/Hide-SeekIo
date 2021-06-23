using UnityEngine;
using Photon.Pun;
using System.Collections;

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

    public override void Zoom(Vector2 inputVector)
    {
        UtillGame.ThrowZoom(inputVector, _distance, newAttacker.CenterPivot, _attackRangeUI);
    }


    #region Attack
    public override void Attack(Vector2 inputVector)
    {
        state = State.Delay;
        Vector3 endPoint = UtillGame.GetThrowPosion(inputVector, _distance, newAttacker.transform);
        LastAttackInput = inputVector;
        photonView.RPC("AttackOnServer", RpcTarget.AllViaServer, inputVector, endPoint);
    }

    [PunRPC]
    public void AttackOnServer(Vector2 inputVector, Vector3 endPoint)
    {
        LastAttackInput = inputVector;
        Vector3 startPoint = newAttacker.CenterPivot.position;
        StartCoroutine(AttackProcessOnAllClinets(startPoint, endPoint));
    }

    IEnumerator AttackProcessOnAllClinets(Vector3 startPoint, Vector3 endPoint)
    {
        state = State.Delay;
        AttackSucessEvent?.Invoke();
        yield return new WaitForSeconds(AttackDelay);   //대미지 주기전까지 시간
        var projectile = Managers.Pool.Pop(_projectilePrefab).GetComponent<ThrowProjectileObject>();
        projectile.Play(startPoint, endPoint);
        yield return new WaitForSeconds(AfaterAttackDelay);   //대미지 주기전까지 시간
        AttackEndEvent?.Invoke();
        state = State.End;
        print("끝");
    }


    public void AttackEffect(Vector3 startPoint, Vector3 endPoint)
    {
        var projectile = Managers.Pool.Pop(_projectilePrefab).GetComponent<ThrowProjectileObject>();
        projectile.Play(startPoint, endPoint);
    }

    #endregion

}
