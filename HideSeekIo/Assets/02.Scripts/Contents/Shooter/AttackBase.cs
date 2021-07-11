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
    public Weapon weapon { get; protected set; }    //안없어지는무기
    public Weapon currentWeapon;    //임시무기


    

    private void OnEnable()
    {
        State = state.Idle;
    }

    public virtual void OnPhotonInstantiate()
    {
        _animator = GetComponentInChildren<Animator>();
        if (weapon)
        {
            PhotonNetwork.Destroy(weapon.gameObject);
        }
    }

    public virtual void SetupWeapon(Weapon newWeapon)
    {
        if(newWeapon.type == Weapon.Type.Permanent)
        {
            weapon = newWeapon;
        }
        newWeapon.transform.ResetTransform(_animator.GetComponent<CharacterAvater>().RightHandAmount);  //무기오브젝트
        newWeapon.UICanvas.transform.ResetTransform(this.transform);       //UI
    }

    //가지고있는 Permeanet아이템 사용으로 전환
    public void UsePermanent()
    {
        if (weapon == null) return;
        UseWeapon(weapon);
    }
    public void UseWeapon(Weapon newWeapon)
    {
        print("UseWeapon " + newWeapon);
        currentWeapon = newWeapon;
        currentWeapon.AttackSucessEvent -= AttackSucess;
        currentWeapon.AttackSucessEvent += AttackSucess;

        currentWeapon.AttackEndEvent -= AttackEnd;
        currentWeapon.AttackEndEvent += AttackEnd;

        if (currentWeapon.type == Weapon.Type.Disposable)
        {
            //currentWeapon.AttackSucessEvent
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
        currentWeapon = weapon; //사용할무기를 오리지널무기로 대체
    }
    protected void UpdateAttackCoolTime()
    {
        InputManager.Instacne.AttackCoolTime(weapon.InitCoolTime, weapon.ReaminCoolTime);
    }

  
}
