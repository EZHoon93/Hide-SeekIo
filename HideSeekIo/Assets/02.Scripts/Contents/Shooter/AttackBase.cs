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
    Define.Weapon n_currentWeapon;

    protected IEnumerator _attackEnumerator;


    public virtual void OnPhotonInstantiate()
    {
        _animator = GetComponentInChildren<Animator>();
    }
    public virtual  void SetupWeapon(Weapon newWeapon)
    {
        if (weapon)
        {
            Managers.Resource.Destroy(weapon.gameObject);
        }
        weapon = newWeapon;
        n_currentWeapon = weapon.weaponServerKey;
        weapon.newAttacker = this;
        weapon.transform.SetParent(_animator.GetBoneTransform(HumanBodyBones.RightHand));
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
        weapon.transform.localScale = Vector3.one;

        weapon.UICanvas.transform.SetParent(this.transform);
        weapon.UICanvas.transform.localPosition = Vector3.zero;
        weapon.UICanvas.transform.localRotation = Quaternion.Euler(Vector3.zero);
        weapon.UICanvas.transform.localScale = Vector3.one;

        switch (weapon.weaponType)
        {
            case Weapon.WeaponType.Gun:
                _animator.SetBool("Gun", true);
                _animator.SetBool("Melee", false);
                break;
            case Weapon.WeaponType.Melee:
                _animator.SetBool("Gun", false);
                _animator.SetBool("Melee", true);
                print(weapon.weaponType + "밀리");


                break;
            case Weapon.WeaponType.Throw:

                break;
        }
    }

}
