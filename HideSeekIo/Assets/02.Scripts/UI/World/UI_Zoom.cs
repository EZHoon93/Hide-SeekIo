using System.Collections;

using UnityEngine;

public class UI_Zoom : MonoBehaviour
{
    [SerializeField] Transform uiZoom;
    AttackBase _attackBase;
    InputBase _inputBase;
    private void Awake()
    {
        _attackBase =  this.transform.parent.GetComponentInParent<AttackBase>();
        _inputBase = this.transform.parent.GetComponentInParent<InputBase>();
        //_attackBase.weaponChangeEvent += ChangeWeapon;
    }

    void ChangeWeapon(Weapon newWeapon)
    {
        print(newWeapon.weaponType + "체인지");
        switch (newWeapon.weaponType)
        {
            case Weapon.WeaponType.Throw:
                var throwWeapon = newWeapon as Weapon_Throw;
                var size = throwWeapon.attackRange;
                print(newWeapon.weaponType + "레인지"+size);

                uiZoom.transform.localScale = new Vector3( size,size,size);
                break;
        }
        
    }

    private void Update()
    {
        UpdateZoom();
    }

    void UpdateZoom()
    {
        switch (_attackBase.currentWeapon.weaponType)
        {
            case Weapon.WeaponType.Melee:
                break;
            case Weapon.WeaponType.Throw:
                break;
            case Weapon.WeaponType.Gun:
                break;
        }
        UtillGame.ThrowZoom(_inputBase.AttackVector, _attackBase.currentWeapon.AttackDistance, this.transform, uiZoom);
    }
}
