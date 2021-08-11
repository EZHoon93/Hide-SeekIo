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
        Dash
    }
    public state State { get; protected set; }

    [SerializeField] Transform _centerPivot;
    protected Animator _animator => character_Base.animator;
    public Character_Base character_Base { get; set; }
    protected IEnumerator _attackEnumerator;
    public Transform CenterPivot => _centerPivot;
    public Weapon baseWeapon { get; protected set; }    //안없어지는무기
    [SerializeField] GameObject[] itemInventory = new GameObject[1];

    protected InputBase _inputBase;
    public Vector2 AttackDirection { get; set; }
    public GameObject[] ItemIntentory => itemInventory;

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
        _inputBase = GetComponent<InputBase>();
        if(character_Base == null)
        {
            character_Base = GetComponentInChildren<Character_Base>();
        }
        SetupSkill(character_Base.mainSkill);  //스킬 셋
        weaponChangeCallBack = null;
        if (baseWeapon)
        {
            PhotonNetwork.Destroy(baseWeapon.gameObject);
        }
        if (this.IsMyCharacter())
        {
        }
    }

    public virtual void SetupWeapon(Weapon newWeapon )
    {
        newWeapon.transform.ResetTransform(_animator.GetComponent<Character_Base>().RightHandAmount);  //무기오브젝트
        newWeapon.AttackSucessEvent += AttackBaseSucess;
        newWeapon.AttackEndEvent += AttackBaseEnd;
        weaponChangeCallBack += newWeapon.WeaponChange;
        ObtainableItem obtainableItem = null;
        switch (newWeapon.inputType)
        {
            case InputType.Main:
                baseWeapon = newWeapon;
                ChangeWeapon(baseWeapon);
                break;
            case InputType.Item1:
                var inventroyIndex = GetItemInventoryIndex();
                obtainableItem = newWeapon.GetComponent<ObtainableItem>();
                AddItemInventory(obtainableItem, inventroyIndex);  //아이템 인벤토리추가
                newWeapon.useState = Weapon.UseState.NoUse;
                break;
            case InputType.Skill:
                break;
            case InputType.Sub:
                break;
        }

        _inputBase.AddInputEvent(newWeapon.inputType, ControllerInputType.Drag, (input) => UpdateZoom(newWeapon, input), obtainableItem);
        _inputBase.AddInputEvent(newWeapon.inputType, ControllerInputType.Up, (input) => UpdateAttackCheck(newWeapon, input), obtainableItem);
    }

    public virtual void SetupSkill(IAttack newSKill)
    {
        if(newSKill.controllerType == Define.ControllerType.Button)
        {
            _inputBase.AddInputEvent(InputType.Skill,  ControllerInputType.Tap , UseSkill);
        }
        else
        {

        }
    }

    public void UseSkill(Vector2 inputVector2)
    {
        character_Base.UseSkill(inputVector2);
    }

    public virtual void SetupImmdediateItem(Item_Base newItem)
    {
        int inventroyIndex = GetItemInventoryIndex();
        _inputBase.AddInputEvent(InputType.Item1, ControllerInputType.Tap, (vector2) => newItem.Use(GetComponent<PlayerController>() ) , newItem.obtainableItem);
        AddItemInventory(newItem.obtainableItem,inventroyIndex);
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

    #region Item Invenvtory
    public void AddItemInventory(ObtainableItem newItem, int inventroyIndex)
    {
        if (newItem == null) return;
        itemInventory[inventroyIndex] = newItem.gameObject;
        newItem.removeCallBack = () => RemoveItemInventory(inventroyIndex);
    }

    int GetItemInventoryIndex()
    {
        for (int i = 0; i < itemInventory.Length; i++)
        {
            if (itemInventory[i] == null)
            {
                return i;
            }
        }
        if(itemInventory[0] != null)
        {
            PhotonNetwork.Destroy(itemInventory[0]);
        }
        return 0;
    }

    public void RemoveItemInventory(int useIndex)
    {
        print("RemoveInventort");
        itemInventory[useIndex] = null;
        _inputBase.RemoveInputEveent(InputType.Item1);
    }

    #endregion

    public void UpdateAttackCheck(IAttack attack, Vector2 inputVector2)
    {
        if (inputVector2.sqrMagnitude == 0 || State != state.Idle) return;
        if (this.IsMyCharacter())
        {
            attack.Zoom(Vector2.zero);
        }
        attack.AttackCheck(inputVector2);
        AttackDirection = inputVector2;
    }

    public void UpdateZoom(IAttack attack , Vector2 inputVector2)
    {
        if (attack != null)
        {
            attack.Zoom(inputVector2);
        }
    }


    
    protected virtual void AttackBaseSucess(IAttack currentAttack)
    {
        State = state.Attack;
        _animator.SetTrigger(currentAttack.AttackAnim);
    }
    protected virtual void AttackBaseEnd()
    {
        State = state.Idle;
        
        
        if (baseWeapon)
        {
            baseWeapon.useState = Weapon.UseState.Use;
            if (baseWeapon.type == Weapon.Type.Disposable && photonView.IsMine)    //사용한 무기가 일회용무기였다면(수류탄) 삭제
            {
                PhotonNetwork.Destroy(baseWeapon.gameObject);
            }
        }
        
    }


    public void Dash()
    {
        StartCoroutine(DashProcess());
    }

    IEnumerator DashProcess()
    {
        //AttackDirection =  _inputBase.controllerInputDic[InputType.Move].inputVector2;
        AttackDirection = UtillGame.ConventToVector2(this.transform.forward);
        State = state.Dash;
        yield return new WaitForSeconds(0.3f) ;
        State = state.Idle;

    }

    private void Update()
    {
        if(itemInventory[0] != null)
        {
            print(itemInventory[0].gameObject.name + "   / ");

        }
    }
}
