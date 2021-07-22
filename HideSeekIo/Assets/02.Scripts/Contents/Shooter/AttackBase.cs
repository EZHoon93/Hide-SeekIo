using System.Collections;

using UnityEngine;
using Photon.Pun;
using System;
using UnityEditor;

public class AttackBase : MonoBehaviourPun 
{
    public enum state
    {
        Idle,
        Attack,
    }
    public state State { get; protected set; }

    [SerializeField] Transform _centerPivot;
    protected Animator _animator;
    protected IEnumerator _attackEnumerator;
    public Transform CenterPivot => _centerPivot;
    public Weapon baseWeapon { get; protected set; }    //안없어지는무기
    public Weapon currentWeapon { get; protected set; }   //현재 사용 무기


    

    private void OnEnable()
    {
        State = state.Idle;
    }

    public virtual void OnPhotonInstantiate()
    {
        _animator = GetComponentInChildren<Animator>();
        if (baseWeapon)
        {
            PhotonNetwork.Destroy(baseWeapon.gameObject);
        }
    }

    public virtual void SetupWeapon(Weapon newWeapon)
    {
        newWeapon.transform.ResetTransform(_animator.GetComponent<CharacterAvater>().RightHandAmount);  //무기오브젝트
        newWeapon.UICanvas.transform.ResetTransform(this.transform);       //UI

        if(currentWeapon == null)
        {
            UseWeapon(newWeapon);
        }
    }

    //가지고있는 Permeanet아이템 사용으로 전환
    public void UsePermanent()
    {
        if (baseWeapon == null) return;
        UseWeapon(baseWeapon);
    }
    public void UseWeapon(Weapon newWeapon)
    {
        if(currentWeapon != null)
        {
            currentWeapon.useState = Weapon.UseState.NoUse;
        }
        currentWeapon = newWeapon;
        currentWeapon.AttackSucessEvent -= AttackSucess;
        currentWeapon.AttackSucessEvent += AttackSucess;
        currentWeapon.AttackEndEvent -= AttackEnd;
        currentWeapon.AttackEndEvent += AttackEnd;
        currentWeapon.useState = Weapon.UseState.Use;
        if (currentWeapon.type == Weapon.Type.Permanent)
        {
            baseWeapon = currentWeapon;
        }
        SetupAnimation();
    }

    protected void SetupAnimation()
    {
        switch (currentWeapon.weaponType)
        {
            case Weapon.WeaponType.Gun:
                _animator.SetBool("Gun", true);
                _animator.SetBool("Melee", false);
                break;
            case Weapon.WeaponType.Melee:
                _animator.SetBool("Gun", false);
                _animator.SetBool("Melee", true);
                break;
            case Weapon.WeaponType.Throw:

                break;
        }

    }

    protected void UpdateZoom(Vector2 inputVector2)
    {
        currentWeapon.Zoom(inputVector2);
    }

    protected virtual void AttackSucess()
    {
        State = state.Attack;
        _animator.SetTrigger(currentWeapon.AttackAnim);

    }
    public void UpdateAttack(Vector2 inputVector2)
    {
        if (inputVector2.sqrMagnitude == 0 || State != state.Idle) return;
        currentWeapon.AttackCheck(inputVector2);
    }
    protected virtual void AttackEnd()
    {
        State = state.Idle;
        if (currentWeapon.type == Weapon.Type.Disposable && photonView.IsMine)    //사용한 무기가 일회용무기였다면(수류탄) 삭제
        {
            PhotonNetwork.Destroy(currentWeapon.gameObject);
        }
        if (baseWeapon)
        {
            UseWeapon(baseWeapon);//사용할무기를 오리지널무기로 대체
        }
        
    }
    protected void UpdateAttackCoolTime()
    {
        InputManager.Instacne.AttackCoolTime(currentWeapon.InitCoolTime, currentWeapon.ReaminCoolTime);
    }

  
}
