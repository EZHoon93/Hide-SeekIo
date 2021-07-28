using UnityEngine;
using Photon.Pun;
using System.Collections;
using System;

public abstract class Weapon_Throw : Weapon , IItem
{
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] Sprite _sprite;
    //Transform _attackRangeUI;

    public float attackRange { get; protected set; }
    public Define.UseType useType { get; set; }
    public Action destroyCallBackEvent { get; set; }

    protected override void Awake()
    {
        base.Awake();
        //_attackRangeUI = GetComponentInChildren<Canvas>().transform;
        useType = Define.UseType.Weapon;
        weaponType = WeaponType.Throw;
        print(this.name + "@@@@@@@@@@@@@@@@@@@@@" + this.ToString());
        _sprite = Resources.Load<Sprite>( $"Sprites/InGameItem/{this.gameObject.name}" );
    }

    public Sprite GetSprite() => _sprite;
    
    public void Setup(string animName, float delayTime, float afaterDelayTime, float distance, float newAttackRange)
    {
        AttackAnim = animName;
        AttackDelay = delayTime;
        AfaterAttackDelay = afaterDelayTime;
        AttackDistance = distance;
        attackRange = newAttackRange;

        UICanvas.transform.localScale = new Vector3(attackRange, attackRange, attackRange);
    }

    public override void Zoom(Vector2 inputVector)
    {

        UtillGame.ThrowZoom(inputVector, AttackDistance, attackPlayer.CenterPivot, UICanvas.transform);
    }


    #region Attack
    public override void Attack(Vector2 inputVector)
    {
        state = State.Delay;
        Vector3 endPoint = UtillGame.GetThrowPosion(inputVector, AttackDistance, attackPlayer.transform);
        LastAttackInput = inputVector;
        photonView.RPC("AttackOnServer", RpcTarget.AllViaServer, inputVector, endPoint);
    }

    [PunRPC]
    public void AttackOnServer(Vector2 inputVector, Vector3 endPoint)
    {
        print("어택온서버!!!!");
        LastAttackInput = inputVector;
        Vector3 startPoint = attackPlayer.CenterPivot.position;
        StartCoroutine(AttackProcessOnAllClinets(startPoint, endPoint));
    }

    IEnumerator AttackProcessOnAllClinets(Vector3 startPoint, Vector3 endPoint)
    {
        state = State.Delay;
        AttackSucessEvent?.Invoke();
        yield return new WaitForSeconds(AttackDelay);   //대미지 주기전까지 시간
        var projectile = Managers.Pool.Pop(_projectilePrefab).GetComponent<ThrowProjectileObject>();
        projectile.Play(attackPlayer,  startPoint, endPoint);
        yield return new WaitForSeconds(AfaterAttackDelay);   //대미지 주기전까지 시간
        AttackEndEvent?.Invoke();
        state = State.End;
        print("끝");
    }


    //public void AttackEffect(Vector3 startPoint, Vector3 endPoint)
    //{
    //    var projectile = Managers.Pool.Pop(_projectilePrefab).GetComponent<ThrowProjectileObject>();
    //    projectile.Play(attackPlayer.ViewID() , startPoint, endPoint) ;
    //}

    #endregion
    public void UseToPlayerToServer()
    {
        photonView.RPC("UserToPlayerOnLocal", RpcTarget.All);
    }

    [PunRPC]
    public void UserToPlayerOnLocal()
    {
        //attackPlayer.UseWeapon(this);
    }

    
}
