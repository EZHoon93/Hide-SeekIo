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
    public GameObject[] itemInventory = new GameObject[2];

    public abstract InputBase GetInputBase();
    public Vector2 AttackDirection { get; set; }

    Action<int> weaponChangeCallBack;

    private void OnEnable()
    {
        State = state.Idle;
    }

    protected virtual void Awake()
    {

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
            GetInputBase().baseInput.ControllerDic[ControllerInputType.Up] = (input) => UpdateThrowAttackCheck(baseWeapon, input);
            GetInputBase().baseInput.ControllerDic[ControllerInputType.Drag] = (input) => UpdateWeaponZoom(newWeapon, input);
        }
        else
        {
            var inventroyIndex = GetItemInventoryIndex();
            GetInputBase().itemsInput[inventroyIndex].ControllerDic[ControllerInputType.Drag] = (input) => UpdateWeaponZoom(newWeapon,input);
            GetInputBase().itemsInput[inventroyIndex].ControllerDic[ControllerInputType.Up] = (input) => UpdateThrowAttackCheck(newWeapon,input);
            AddItem(newWeapon.gameObject, inventroyIndex);  //인벤토리추
            newWeapon.useState = Weapon.UseState.NoUse;
        }
    }

 
    public virtual void SetupImmdediateItem(Item_Base newItem)
    {
        //AddItem(newItem);
        int inventroyIndex = GetItemInventoryIndex();
        itemInventory[inventroyIndex] = newItem.gameObject;
        GetInputBase().itemsInput[inventroyIndex].ControllerDic[ControllerInputType.Tap] = (Vector2)=> newItem.Use(GetComponent<PlayerController>());

        AddItem(newItem.gameObject,inventroyIndex);

    }

    public void ChangeWeapon(Weapon useNewWeapon)
    {
        weaponChangeCallBack?.Invoke(useNewWeapon.GetInstanceID());
        SetupAnimation(useNewWeapon);
    }

    protected void RemoveChangeEvent(Weapon usedWeapon)
    {
        weaponChangeCallBack -= usedWeapon.WeaponChange;
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

    #region Item 
    public void AddItem(GameObject newItem, int inventroyIndex)
    {
        var item = newItem.GetComponent<ObtainableItem>();
        if (item == null) return;

        itemInventory[inventroyIndex] = newItem;
        item.removeCallBack = () => RemoveItem(inventroyIndex);
        if (this.IsMyCharacter())
        {
            InputManager.Instance.itemControllerJoysticks[inventroyIndex].AddItem(item);
        }
    }

    int GetItemInventoryIndex()
    {
        for (int i = 0; i < itemInventory.Length; i++)
        {
            if (itemInventory[i] != null) continue;
            return i;
        }
        return 0;
    }

    public void RemoveItem(int useIndex)
    {
        itemInventory[useIndex] = null;
        GetInputBase().itemsInput[useIndex].ControllerDic[ControllerInputType.Down] = null;
        GetInputBase().itemsInput[useIndex].ControllerDic[ControllerInputType.Drag] = null;
        GetInputBase().itemsInput[useIndex].ControllerDic[ControllerInputType.Tap] = null;
        GetInputBase().itemsInput[useIndex].ControllerDic[ControllerInputType.Up] = null;

        if (this.IsMyCharacter())
        {
            InputManager.Instance.itemControllerJoysticks[useIndex].RemoveItem();
        }
    }

    #endregion

    public void UpdateThrowAttackCheck(Weapon weapon, Vector2 inputVector2)
    {
        if (inputVector2.sqrMagnitude == 0 || State != state.Idle) return;
        weapon.AttackCheck(inputVector2);
        currentWeapon = weapon;
        AttackDirection = inputVector2;
    }

    public void UpdateWeaponZoom(Weapon weapon , Vector2 inputVector2)
    {
        if (weapon)
        {
            weapon.Zoom(inputVector2);
        }
    }


    
    protected virtual void AttackBaseSucess(Weapon currentUseweapon)
    {
        State = state.Attack;
        _animator.SetTrigger(currentUseweapon.AttackAnim);
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
        //if (baseWeapon)
        //{
        //    if(baseWeapon.Zoom(GetInputBase().AttackVector))
        //    {
        //        //CurrentZoomWeaponIndex = baseWeapon.GetInstanceID();
        //    }
        //}
        //if(itemInventory[0] != null)
        //{
        //    if(itemInventory[0].Zoom(GetInputBase().ItemVector1))
        //    {
        //        //CurrentZoomWeaponIndex = itemInventory[0].GetInstanceID();

        //    }
        //}
        if (itemInventory[1] != null)
        {
            //if (itemInventory[1].Zoom(GetInputBase().ItemVector2))
            //{
            //    //CurrentZoomWeaponIndex = itemInventory[1].GetInstanceID();

            //}
            //itemInventory[1].GetComponent<IItem>().Zoom(Vector2.zero);
        }
    }

}
