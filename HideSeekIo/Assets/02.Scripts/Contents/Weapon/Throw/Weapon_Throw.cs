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
    }

    public override void OnPreNetDestroy(PhotonView rootView)
    {
        base.OnPreNetDestroy(rootView);
        hasPlayerController.GetAttackBase().RemoveItem(this);
    }

    public Sprite GetSprite() => _sprite;
    
    public void Setup(string animName, float delayTime, float afaterDelayTime, float distance, float newAttackRange)
    {
        AttackAnim = animName;
        AttackDelay = delayTime;
        AfaterAttackDelay = afaterDelayTime;
        AttackDistance = distance;
        attackRange = newAttackRange;

        UICanvas.transform.localScale = new Vector3(attackRange, attackRange, attackRange); //범위에 따른 ui변경
    }

    public override bool Zoom(Vector2 inputVector)
    {
        var state = UtillGame.ThrowZoom(inputVector, AttackDistance, hasPlayerController.GetAttackBase().CenterPivot, UICanvas.transform);
        if (state)
        {
            useState = UseState.Use;
        }
        return state;

    }


    #region Attack
    public override void Attack(Vector2 inputVector)
    {
        state = State.Delay;
        UICanvas.transform.gameObject.SetActive(false);
        Vector3 endPoint = UtillGame.GetThrowPosion(inputVector, AttackDistance, hasPlayerController.transform);
        LastAttackInput = inputVector;
        photonView.RPC("AttackOnServer", RpcTarget.AllViaServer, inputVector, endPoint);
    }

    [PunRPC]
    public void AttackOnServer(Vector2 inputVector, Vector3 endPoint)
    {
        LastAttackInput = inputVector;
        Vector3 startPoint = hasPlayerController.GetAttackBase().CenterPivot.position;
        StartCoroutine(AttackProcessOnAllClinets(startPoint, endPoint));
    }

    IEnumerator AttackProcessOnAllClinets(Vector3 startPoint, Vector3 endPoint)
    {
        state = State.Delay;
        AttackSucessEvent?.Invoke(this);
        yield return new WaitForSeconds(AttackDelay);   //대미지 주기전까지 시간
        var projectile = Managers.Pool.Pop(_projectilePrefab).GetComponent<ThrowProjectileObject>();
        projectile.Play(hasPlayerController.GetAttackBase(),  startPoint, endPoint);
        yield return new WaitForSeconds(AfaterAttackDelay);   //대미지 주기전까지 시간
        AttackEndEvent?.Invoke();

        state = State.End;
        Use(hasPlayerController);
    }


    #endregion

    public virtual void Use(PlayerController usePlayerController)
    {
        if(type == Type.Disposable)
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }

}
