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
    //public List<IItem> itemWeapons;
    public IItem[] itemInventory = new IItem[2];
    //public Weapon skillWeapon { get; protected set; }   //스킬무기
    //public Weapon currentWeapon { get; protected set; }   //현재 사용 무기
    public abstract InputBase GetInputBase();
    public Vector2 AttackDirection { get; set; }

    Action<int> weaponChangeCallBack;



    private void OnEnable()
    {
        State = state.Idle;
    }

    private void Start()
    {
        GetInputBase().AttackEventCallBack += UpdateBaseAttack;
        GetInputBase().ItemEventCallBackList1 += UpdateItemAttack1;

    }
    public virtual void OnPhotonInstantiate()
    {
        _animator = GetComponentInChildren<Animator>();
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
        if (IsBaseWeapon)
        {
            baseWeapon = newWeapon;
            baseWeapon.useState = Weapon.UseState.Use;
            UseWeapon(baseWeapon);
            if (this.IsMyCharacter())
            {

            }
        }
        else
        {
            var item = newWeapon.GetComponent<IItem>();
            if (item != null)
            {
                int inventroyIndex = AddItem(item);
                newWeapon.useState = Weapon.UseState.NoUse;
            }
        }
    }

    

    public virtual void SetupImmdediateItem(Item_Base newItem)
    {

    }

    //가지고있는 Permeanet아이템 사용으로 전환
    public void UsePermanent()
    {
        if (baseWeapon == null) return;
        //UseWeapon(baseWeapon);
    }

    public void ChangeWeapon(int index)
    {
        if(index == 0)
        {
            UseWeapon(baseWeapon);
        }
        else
        {
            if( itemInventory[index-1] != null)
            {
                //var weapon = itemInventory[index - 1].GetComponent<Weapon>();
                //if (weapon)
                //{
                //    UseWeapon(weapon);
                //}
            }
        }
    }
    public virtual void UseWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;
        currentWeapon.AttackSucessEvent += AttackBaseSucess;
        currentWeapon.AttackEndEvent += AttackBaseEnd;
        currentWeapon.useState = Weapon.UseState.Use;
        weaponChangeCallBack?.Invoke(newWeapon.GetInstanceID());
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
    public int AddItem(IItem newItem)
    {
        for (int i = 0; i < itemInventory.Length; i++)
        {
            if (itemInventory[i] != null) continue;
            itemInventory[i] = newItem;
            //var weapon = newItem.GetComponent<Weapon>();
            //if (weapon)
            //{
            //    SetupWeapon(weapon, false);
            //}
            if (this.IsMyCharacter())
            {
                InputManager.Instance.itemControllerJoysticks[i].AddItem(newItem);
            }
            return i ;
        }

        return 0;
    }
    public void UpdateItemAttack1(Vector2 inputVector2)
    {
        if (inputVector2.sqrMagnitude == 0 || State != state.Idle) return;
        if (itemInventory[0] == null) return;
        if( itemInventory[0].useType == Define.UseType.Weapon)
        {
            itemInventory[0].Attack(inputVector2);
            InputManager.Instance.itemControllerJoysticks[0].RemoveItem();

        }
        AttackDirection = inputVector2;
    }
    void SetupItem(IItem newItem)
    {

    }

    protected void UpdateBaseZoom(Weapon weapon, Vector2 inputVector2)
    {
        currentWeapon.Zoom(inputVector2);
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
        print(inputVector2 + " / " + index);
        //itemWeapons[index].Zoom(inputVector2);
    }

    private void LateUpdate()
    {
        if (this.IsMyCharacter() == false) return;
        if (baseWeapon)
        {
            currentWeapon.Zoom(GetInputBase().AttackVector);
        }
        if(itemInventory[0] != null)
        {
            itemInventory[0].Zoom(GetInputBase().ItemVector1);
        }
    }

}
