using System.Collections;

using UnityEngine;
using Photon.Pun;
using System;
using UnityEditor;
using System.Collections.Generic;

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
    public Weapon[] itemWeapons = new Weapon[2];
    //public Weapon skillWeapon { get; protected set; }   //스킬무기
    //public Weapon currentWeapon { get; protected set; }   //현재 사용 무기

    public Vector2 AttackDirection { get; set; }



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

        if(newWeapon.type == Weapon.Type.Permanent)
        {
            UseWeapon(newWeapon);
        }
        else
        {
            
        }
    }

    //가지고있는 Permeanet아이템 사용으로 전환
    public void UsePermanent()
    {
        if (baseWeapon == null) return;
        UseWeapon(baseWeapon);
    }
    public virtual void UseWeapon(Weapon newWeapon)
    {
        baseWeapon = newWeapon;
        baseWeapon.AttackSucessEvent += AttackBaseSucess;
        baseWeapon.AttackEndEvent += AttackBaseEnd;
        baseWeapon.useState = Weapon.UseState.Use;
        SetupAnimation();
    }

    protected void SetupAnimation()
    {
        switch (baseWeapon.weaponType)
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

    protected void UpdateBaseZoom(Vector2 inputVector2)
    {
        print(baseWeapon.gameObject.name + inputVector2 );
        baseWeapon.Zoom(inputVector2);
    }
    protected virtual void AttackBaseSucess()
    {
        State = state.Attack;
        _animator.SetTrigger(baseWeapon.AttackAnim);
    }
    public void UpdateBaseAttack(Vector2 inputVector2)
    {
        if (inputVector2.sqrMagnitude == 0 || State != state.Idle) return;
        baseWeapon.AttackCheck(inputVector2);
        AttackDirection = inputVector2;
    }
    protected virtual void AttackBaseEnd()
    {
        State = state.Idle;
        if (baseWeapon.type == Weapon.Type.Disposable && photonView.IsMine)    //사용한 무기가 일회용무기였다면(수류탄) 삭제
        {
            PhotonNetwork.Destroy(baseWeapon.gameObject);
        }
        if (baseWeapon)
        {
            UseWeapon(baseWeapon);//사용할무기를 오리지널무기로 대체
        }
        
    }
    protected void UpdateAttackCoolTime()
    {
        //InputManager.Instance.AttackCoolTime(currentWeapon.InitCoolTime, currentWeapon.ReaminCoolTime);
    }

    
    public  void UpdateItemZoom(int index, Vector2 inputVector2)
    {
        itemWeapons[index].Zoom(inputVector2);
    }


}
