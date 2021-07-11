using UnityEngine;
using Photon.Pun;
using System.Collections;

public abstract class Weapon_Throw : Weapon
{
    [SerializeField] GameObject _projectilePrefab;
    
    //Transform _attackRangeUI;

    public float attackRange { get; protected set; }

    protected override void Awake()
    {
        base.Awake();
        //_attackRangeUI = GetComponentInChildren<Canvas>().transform;
        weaponType = WeaponType.Throw;
    }
    public void Setup(string animName, float delayTime, float afaterDelayTime, float distance, float newAttackRange)
    {
        AttackAnim = animName;
        AttackDelay = delayTime;
        AfaterAttackDelay = afaterDelayTime;
        AttackDistance = distance;
        attackRange = newAttackRange;
    }

    public override void Zoom(Vector2 inputVector)
    {
        print(AttackDistance);
        UtillGame.ThrowZoom(inputVector, AttackDistance, newAttacker.CenterPivot, UICanvas.transform);
    }


    #region Attack
    public override void Attack(Vector2 inputVector)
    {
        state = State.Delay;
        Vector3 endPoint = UtillGame.GetThrowPosion(inputVector, AttackDistance, newAttacker.transform);
        LastAttackInput = inputVector;
        photonView.RPC("AttackOnServer", RpcTarget.AllViaServer, inputVector, endPoint);
    }

    [PunRPC]
    public void AttackOnServer(Vector2 inputVector, Vector3 endPoint)
    {
        print("어택온서버!!!!");
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
        projectile.Play(newAttacker.ViewID(),  startPoint, endPoint);
        yield return new WaitForSeconds(AfaterAttackDelay);   //대미지 주기전까지 시간
        AttackEndEvent?.Invoke();
        state = State.End;
        print("끝");
    }


    //public void AttackEffect(Vector3 startPoint, Vector3 endPoint)
    //{
    //    var projectile = Managers.Pool.Pop(_projectilePrefab).GetComponent<ThrowProjectileObject>();
    //    projectile.Play(newAttacker.ViewID() , startPoint, endPoint) ;
    //}

    #endregion
    public void UseToPlayerToServer()
    {
        photonView.RPC("UserToPlayerOnLocal", RpcTarget.All);
    }

    [PunRPC]
    public void UserToPlayerOnLocal()
    {
        newAttacker.UseWeapon(this);
    }
}
