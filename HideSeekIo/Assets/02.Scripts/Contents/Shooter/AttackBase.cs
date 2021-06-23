using System.Collections;

using UnityEngine;
using Photon.Pun;
using System;
public class AttackBase : MonoBehaviourPun
{
    [SerializeField] Transform _centerPivot;
    protected Animator _animator;
    public Transform CenterPivot => _centerPivot;
    public Weapon weapon { get; protected set; }
    //Define.Weapon n_currentWeapon;

    protected IEnumerator _attackEnumerator;


    public virtual void OnPhotonInstantiate()
    {
        _animator = GetComponentInChildren<Animator>();
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
        _animator.SetTrigger(weapon.AttackAnim);

    }

    protected virtual void AttackEnd()
    {

    }
    protected void UpdateAttackCoolTime()
    {
        InputManager.Instacne.AttackCoolTime(weapon.InitCoolTime, weapon.ReaminCoolTime);
    }

    
}
