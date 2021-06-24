using System.Collections;

using UnityEngine;
using Photon.Pun;
using System;

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
    public Weapon weapon { get; protected set; }




    public virtual void OnPhotonInstantiate()
    {
        _animator = GetComponentInChildren<Animator>();
        State = state.Idle;
    }

    public void OnUpdate(Vector2 inputVector2)
    {
        if (this.photonView.IsMine == false || weapon == null) return;
        UpdateAttackCoolTime();
        UpdateAttack(inputVector2);
        weapon.Zoom(inputVector2);
    }

    public virtual void SetupWeapon(Weapon newWeapon)
    {
        if (weapon)
        {
            Managers.Resource.Destroy(weapon.gameObject);
        }
        weapon = newWeapon;
        weapon.newAttacker = this;
        weapon.transform.ResetTransform(_animator.GetBoneTransform(HumanBodyBones.RightHand));  //무기오브젝트
        weapon.UICanvas.transform.ResetTransform(this.transform);       //UI
        weapon.AttackSucessEvent = AttackSucess;
        weapon.AttackEndEvent = AttackEnd;
        SetupAnimation();
    }

    void SetupAnimation()
    {
        switch (weapon.weaponType)
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

    protected virtual void AttackSucess()
    {
        State = state.Attack;
        _animator.SetTrigger(weapon.AttackAnim);
    }
    public void UpdateAttack(Vector2 inputVector2)
    {
        if (inputVector2.sqrMagnitude == 0 || State != state.Idle) return;
        weapon.AttackCheck(inputVector2);
    }
    protected virtual void AttackEnd()
    {
        State = state.Idle;
    }
    protected void UpdateAttackCoolTime()
    {
        InputManager.Instacne.AttackCoolTime(weapon.InitCoolTime, weapon.ReaminCoolTime);
    }


}
