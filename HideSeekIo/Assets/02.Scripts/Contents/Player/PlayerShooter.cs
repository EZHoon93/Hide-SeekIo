using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
public class PlayerShooter : MonoBehaviourPun
{
    public enum state
    {
        Idle,
        NoMove,
        MoveAttack,
        Wait,
        Jump
        //Attack,
        //Throw,
        //Dash,
        //Jump
    }
    public state State { get; protected set; }
    [SerializeField] GameObject[] itemInventory = new GameObject[1];

    Character_Base _character_Base;
    PlayerInput _playerInput;
    Animator _animator => _character_Base.animator;
    public Weapon baseWeapon { get; protected set; }    //안없어지는무기
    public Vector2 AttackDirection { get; set; }
    public Vector3 AttackPoint { get; set; }
    public GameObject[] ItemIntentory => itemInventory;
    Action<int> weaponChangeCallBack;

    public IAttack currentAttack { get; protected set; }

    private void OnEnable()
    {
        State = state.Idle;
    }

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }




    public virtual void OnPhotonInstantiate()
    {
        _character_Base = GetComponent<Character_Base>();
        weaponChangeCallBack = null;
        if (baseWeapon)
        {
            PhotonNetwork.Destroy(baseWeapon.gameObject);
        }
    }

    public void ChangeOwnerShip()
    {

        if (this.IsMyCharacter())
        {
            Managers.Spawn.WeaponSpawn(Define.Weapon.Gun, this);
        }
    }


    public virtual void SetupWeapon(Weapon newWeapon)
    {
        print("Setup Weapon " + newWeapon.gameObject.name + "/"+newWeapon.inputControllerObject.inputType);
        var state = GetShooterState(newWeapon);
        newWeapon.inputControllerObject.useSucessStartCallBack += () => { _animator.SetTrigger(newWeapon.AttackAnim); State = state; };
        newWeapon.inputControllerObject.useSucessEndCallBack += AttackBaseEnd;
        weaponChangeCallBack += newWeapon.WeaponChange;
        SetupEquipmentable(newWeapon.equipmentable);
        SetupControllerObject(newWeapon.inputControllerObject);
        print("dddddddd Weapon " + newWeapon.gameObject.name + "/" + newWeapon.inputControllerObject.inputType);

        switch (newWeapon.inputControllerObject.inputType)
        {

            case InputType.Sub1:    //술래!!
                baseWeapon = newWeapon;
                print("Setup sssssssss " + newWeapon.gameObject.name + "/" + newWeapon.inputControllerObject.inputType);

                ChangeWeapon(baseWeapon);
                break;
        }

        print("dkkkkn " + newWeapon.gameObject.name + "/" + newWeapon.inputControllerObject.inputType);


    }

    public void SetupControllerObject(InputControllerObject newInputControllerObject)
    {
        if (newInputControllerObject.attackType == Define.AttackType.Button)
        {
            _playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Down, newInputControllerObject.inputType, (input) => UseInputControllerObject(input, newInputControllerObject));
        }
        else
        {
            _playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Up, newInputControllerObject.inputType, (input) => UseInputControllerObject(input, newInputControllerObject));
            _playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Drag, newInputControllerObject.inputType, (input) => ZoomInputConrollerObject(input, newInputControllerObject));

        }
    }

    public void SetupEquipmentable(Equipmentable equipmentable)
    {
        var avater = _animator.GetComponent<CharacterAvater>();
        var adaptTransform = avater.GetSkilParentTransform(equipmentable.equipSkiType);
        equipmentable.transform.ResetTransform(adaptTransform);
    }

    public void ChangeWeapon(Weapon useNewWeapon)
    {
        print(useNewWeapon + "/ ssssss/ / ");
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
            case Weapon.WeaponType.Hammer:
                _animator.SetBool("Hammer", true);

                break;
        }

    }

   

    public void UseInputControllerObject(Vector2 inputVector2, InputControllerObject inputControllerObject)
    {
        if (this.IsMyCharacter())
        {
            inputControllerObject.Zoom(Vector2.zero);
        }
        AttackDirection = inputVector2;
        inputControllerObject.Use(inputVector2);
        if (baseWeapon)
        {
            baseWeapon.useState = Weapon.UseState.Use;
        }
    }
    public void ZoomInputConrollerObject(Vector2 inputVector2, InputControllerObject inputControllerObject)
    {
        inputControllerObject.Zoom(inputVector2);
    }


    protected virtual void WeaponAttackSucess(Weapon attackWeapon)
    {
        _animator.SetTrigger(attackWeapon.AttackAnim);
        State = attackWeapon.inputControllerObject.shooterState;
    }
    protected virtual void AttackBaseEnd()
    {
        State = state.Idle;


        if (baseWeapon)
        {
            baseWeapon.useState = Weapon.UseState.Use;
            if (baseWeapon.type == Weapon.UseType.Disposable && photonView.IsMine)    //사용한 무기가 일회용무기였다면(수류탄) 삭제
            {
                PhotonNetwork.Destroy(baseWeapon.gameObject);
            }
        }
    }

    state GetShooterState(Weapon weapon)
    {
        switch (weapon.weaponType)
        {
            case Weapon.WeaponType.Hammer:
                return state.NoMove;
            case Weapon.WeaponType.Throw:
                return state.MoveAttack;
            default:
                return state.Idle;
        }
    }

    [PunRPC]

    public void Dash(Vector3 playerPos,  Vector2 inputVector2)
    {
        this.transform.position = playerPos;
        StartCoroutine(DashProcess(inputVector2));
    }

    IEnumerator DashProcess( Vector2 inputVector2)
    {
        //AttackDirection =  _inputBase.controllerInputDic[InputType.Move].inputVector2;
        //AttackDirection = UtillGame.ConventToVector2(this._character_Base.characterAvater. transform.forward);
        State = state.Jump;
        AttackDirection = inputVector2;
        _animator.SetTrigger("Dash");
        yield return new WaitForSeconds(0.3f);
        State = state.Idle;
        //GetComponent<PhotonMove>().m_StoredPosition = this.transform.position;
        //GetComponent<PhotonMove>().m_NetworkPosition = this.transform.position;
    }

}
