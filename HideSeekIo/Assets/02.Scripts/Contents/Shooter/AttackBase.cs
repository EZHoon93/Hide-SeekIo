using System.Collections;

using UnityEngine;
using Photon.Pun;
using System;


public abstract class AttackBase : MonoBehaviourPun 
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
    public Weapon currentWeapon { get; protected set; } //현재사용무기
    public IItem[] itemInventory = new IItem[2];
    public abstract InputBase GetInputBase();
    public Vector2 AttackDirection { get; set; }

    Action<int> weaponChangeCallBack;

    //int _currentWeaponIndex;
    //public int CurrentZoomWeaponIndex
    //{
    //    get => _currentWeaponIndex;
    //    set
    //    {
    //        var newIndex = value;
    //        if(newIndex != _currentWeaponIndex)
    //        {
    //            weaponChangeCallBack?.Invoke(newIndex);
    //            _currentWeaponIndex = newIndex;
    //            //ChangeWeapon(_currentWeaponIndex);
    //        }
    //    }
    //}



    private void OnEnable()
    {
        State = state.Idle;
    }

    private void Start()
    {
        GetInputBase().AttackEventCallBack += UpdateBaseAttack;
        GetInputBase().ItemEventCallBackList1 += UpdateItemAttack1;
        GetInputBase().ItemEventCallBackList2 += UpdateItemAttack2;
    }


    public virtual void OnPhotonInstantiate()
    {
        _animator = GetComponentInChildren<Animator>();
        weaponChangeCallBack = null;
        if (baseWeapon)
        {
            PhotonNetwork.Destroy(baseWeapon.gameObject);
        }
        if (this.IsMyCharacter())
        {
        }
    }

    public virtual void SetupWeapon(Weapon newWeapon, bool IsBaseWeapon )
    {
        newWeapon.transform.ResetTransform(_animator.GetComponent<CharacterAvater>().RightHandAmount);  //무기오브젝트
        newWeapon.UICanvas.transform.ResetTransform(this.transform);       //UI
        newWeapon.AttackSucessEvent += AttackBaseSucess;
        newWeapon.AttackEndEvent += AttackBaseEnd;
        weaponChangeCallBack += newWeapon.WeaponChange;
        if (IsBaseWeapon)
        {
            baseWeapon = newWeapon;
            ChangeWeapon(baseWeapon);
            //if (this.IsMyCharacter())
            //{
            //    InputManager.Instance.baseAttackJoystick._ultimateJoystick.OnPointerDownCallback += () => ChangeWeapon(baseWeapon);
            //}
        }
        else
        {
            var item = newWeapon.GetComponent<IItem>();
            if (item != null)
            {
                newWeapon.AttackSucessEvent += () => { weaponChangeCallBack -= newWeapon.WeaponChange; };

                var index = AddItem(item);
                //if (this.IsMyCharacter())
                //{

                //    InputManager.Instance.itemControllerJoysticks[index]._ultimateJoystick.OnPointerDownCallback += () => ChangeWeapon(newWeapon);
                //    newWeapon.AttackSucessEvent  += () => {
                //        InputManager.Instance.itemControllerJoysticks[index]._ultimateJoystick.OnPointerDownCallback -= () => ChangeWeapon(newWeapon);
                //    };
                //}
                newWeapon.useState = Weapon.UseState.NoUse;
            }
        }
    }

 
    public virtual void SetupImmdediateItem(Item_Base newItem)
    {
        AddItem(newItem);
    }

    public void ChangeWeapon(Weapon useNewWeapon)
    {
        weaponChangeCallBack?.Invoke(useNewWeapon.GetInstanceID());
        SetupAnimation(useNewWeapon);
    }
    protected void SetupAnimation(Weapon newWeapon)
    {
        switch (newWeapon.weaponType)
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
                _animator.SetBool("Melee", false);

                break;
        }

    }
    public int AddItem(IItem newItem)
    {
        for (int i = 0; i < itemInventory.Length; i++)
        {
            if (itemInventory[i] != null) continue;
            itemInventory[i] = newItem;
            if (this.IsMyCharacter())
            {
                InputManager.Instance.itemControllerJoysticks[i].AddItem(newItem);
            }
            return i ;
        }

        return 0;
    }
    public void RemoveItem(IItem useItem)
    {
        for (int i = 0; i < itemInventory.Length; i++)
        {
            if (itemInventory[i] == null) continue;
            
            if(itemInventory[i] == useItem)
            {
                itemInventory[i] = null;
                if (this.IsMyCharacter())
                {
                    InputManager.Instance.itemControllerJoysticks[i].RemoveItem();
                }
            }
        }
    }

    public void UpdateBaseAttack(Vector2 inputVector2)
    {
        if (inputVector2.sqrMagnitude == 0 || State != state.Idle) return;
        baseWeapon.AttackCheck(inputVector2);
        AttackDirection = inputVector2;
    }
    public void UpdateItemAttack1(Vector2 inputVector2)
    {
        if (inputVector2.sqrMagnitude == 0 || State != state.Idle) return;
        if (itemInventory[0] == null) return;
        if( itemInventory[0].useType == Define.UseType.Weapon)
        {
            itemInventory[0].Attack(inputVector2);
            RemoveItem(itemInventory[0]);

        }
        AttackDirection = inputVector2;
    }

    public void UpdateItemAttack2(Vector2 inputVector2)
    {
        if (inputVector2.sqrMagnitude == 0 || State != state.Idle) return;
        if (itemInventory[1] == null) return;
        if (itemInventory[1].useType == Define.UseType.Weapon)
        {
            itemInventory[1].Attack(inputVector2);
            RemoveItem(itemInventory[1]);
        }
        AttackDirection = inputVector2;
    }

    
    protected virtual void AttackBaseSucess()
    {
        State = state.Attack;
        _animator.SetTrigger(baseWeapon.AttackAnim);
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
            baseWeapon.useState = Weapon.UseState.Use;
        }
        
    }
    protected void UpdateAttackCoolTime()
    {
        //InputManager.Instance.AttackCoolTime(currentWeapon.InitCoolTime, currentWeapon.ReaminCoolTime);
    }
    private void LateUpdate()
    {
        if (this.IsMyCharacter() == false) return;
        if (baseWeapon)
        {
            if(baseWeapon.Zoom(GetInputBase().AttackVector))
            {
                //CurrentZoomWeaponIndex = baseWeapon.GetInstanceID();
            }
        }
        if(itemInventory[0] != null)
        {
            if(itemInventory[0].Zoom(GetInputBase().ItemVector1))
            {
                //CurrentZoomWeaponIndex = itemInventory[0].GetInstanceID();

            }
        }
        if (itemInventory[1] != null)
        {
            if(itemInventory[1].Zoom(GetInputBase().ItemVector2))
            {
                //CurrentZoomWeaponIndex = itemInventory[1].GetInstanceID();

            }
        }
    }

}
