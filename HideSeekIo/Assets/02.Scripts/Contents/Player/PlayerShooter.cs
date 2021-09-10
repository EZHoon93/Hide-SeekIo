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
    PlayerCharacter _playerCharacter;
    PlayerInput _playerInput;
    Animator _animator => _playerCharacter.animator;
    public Weapon baseWeapon { get; protected set; }    //안없어지는무기
    public Vector3 AttackDirection { get; private set; }
    Action<int> weaponChangeCallBack;
    InputControllerObject currentInputController = null;



    [Header("Transform")]
    Transform upperSpine;
    protected Vector3 upperBodyDir;
    protected bool rotate = false;

    public float baseWeaponReaminTime;
    private void OnEnable()
    {
        State = state.Idle;
    }

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerCharacter = GetComponent<PlayerCharacter>();
    }
    
    private void LateUpdate()
    {
        if (rotate)
        {
            if (AttackDirection == Vector3.zero) return;
            if (upperSpine == null)
            {
                upperSpine = _animator.GetBoneTransform(HumanBodyBones.Spine);

            }
            Vector3 spineRot = Quaternion.LookRotation(AttackDirection).eulerAngles;
            spineRot -= _playerCharacter.characterAvater.transform.eulerAngles;
            upperSpine.transform.localRotation = Quaternion.Euler(
                upperSpine.transform.localEulerAngles.x - spineRot.y,
                upperSpine.transform.localEulerAngles.y,
                upperSpine.transform.localEulerAngles.z
             );
        }
        if (this.IsMyCharacter() == false || currentInputController == null) return;
        currentInputController.Zoom(_playerInput.controllerInputDic[currentInputController.inputType].inputVector2 );

        
    }
  
    public virtual void OnPhotonInstantiate()
    {
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
            Managers.Spawn.WeaponSpawn(Define.Weapon.Stone, this);
        }
    }


    public virtual void SetupWeapon(Weapon newWeapon)
    {
        newWeapon.inputControllerObject.useSucessStartCallBack += () => WeaponAttackSucess(newWeapon);
        newWeapon.inputControllerObject.useSucessEndCallBack += AttackBaseEnd;
        weaponChangeCallBack += newWeapon.WeaponChange;
        
        SetupEquipmentable(newWeapon.equipmentable);
        SetupControllerObject(newWeapon.inputControllerObject);
        switch (newWeapon.inputControllerObject.inputType)
        {
            case InputType.Sub1:    //술래!!
                if (baseWeapon)
                {
                    if (baseWeapon.photonView.IsMine)
                        PhotonNetwork.Destroy(baseWeapon.gameObject);
                }
                if(newWeapon.weaponType== Weapon.WeaponType.Hammer)
                {
                    GetComponent<PlayerController>().ChangeTeam(Define.Team.Seek);
                }
                if (newWeapon.weaponType == Weapon.WeaponType.Gun)
                {

                }
                baseWeapon = newWeapon;
                ChangeWeapon(baseWeapon);
                break;
        }

        GetComponent<PlayerHealth>().AddRenderer(newWeapon.renderController);
    }

    public void SetupControllerObject(InputControllerObject newInputControllerObject)
    {
        if (newInputControllerObject.attackType == Define.AttackType.Button)
        {
            _playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Down, newInputControllerObject.inputType,
              (input) => { UseInputControllerObject(input, newInputControllerObject); }, newInputControllerObject.sprite);
            _playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Up, newInputControllerObject.inputType, null);
        }
        else
        {
            //_playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Down, newInputControllerObject.inputType,
            //   (input) => { currentInputController?.Zoom(Vector2.zero); currentInputController = newInputControllerObject; });
            _playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Down, newInputControllerObject.inputType,
              (input) => { ZoomInputConrollerObject(input, newInputControllerObject);  });
            _playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Up, newInputControllerObject.inputType, (input) => UseInputControllerObject(input, newInputControllerObject) , newInputControllerObject.sprite);
           
            //_playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Drag, newInputControllerObject.inputType, (input) => ZoomInputConrollerObject(input, newInputControllerObject));
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
        weaponChangeCallBack?.Invoke(useNewWeapon.GetInstanceID());
        //currentInputController = useNewWeapon.inputControllerObject;
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
        if (this.State != state.Idle) return;
        if (this.IsMyCharacter())
        {
            //내캐릭이라면 줌을꺼줌
            inputControllerObject.Zoom(Vector2.zero);
        }
        
        if(inputControllerObject.Use(inputVector2))
        {
            if (baseWeapon)
            {
                ChangeWeapon(baseWeapon);
            }
        }
    }
    public void ZoomInputConrollerObject(Vector2 inputVector2, InputControllerObject inputControllerObject)
    {
        if(currentInputController != null)
        {
            currentInputController.Zoom(Vector2.zero);
        }
        currentInputController = inputControllerObject;
    }


    protected virtual void WeaponAttackSucess(Weapon attackWeapon)
    {
        State = attackWeapon.inputControllerObject.shooterState;
        //var attackDirection = (attackWeapon.inputControllerObject.attackPoint - this.transform.position).normalized;
        //attackDirection.y = this.transform.position.y;
        AttackDirection = attackWeapon.inputControllerObject.attackDirection; ;
        //AttackDirection = UtillGame.ConventToVector3( attackWeapon.inputControllerObject.lastInputSucessVector2);
        if (attackWeapon.weaponType == Weapon.WeaponType.Hammer)
        {
            //AttackDirection = Vector3.zero;

        }
        else
        {
            rotate = true;
        }
        _animator.SetTrigger(attackWeapon.AttackAnim);
    }
    protected virtual void AttackBaseEnd()
    {
        rotate = false;
        if (baseWeapon)
        {
            if(currentInputController == null)
            {
                ChangeWeapon(baseWeapon);
            }
            //if (baseWeapon.type == Weapon.UseType.Disposable && photonView.IsMine)    //사용한 무기가 일회용무기였다면(수류탄) 삭제
            //{
            //    PhotonNetwork.Destroy(baseWeapon.gameObject);
            //}
        }
        State = state.Idle;

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

    /// <summary>
    /// 주 무기 남은 쿨타임 
    /// </summary>
    /// <returns></returns>
    public float GetBaseWeaponRemainCoolTime()
    {
        if(baseWeapon == null)
        {
            return -1;
        }
        return baseWeapon.inputControllerObject.RemainCoolTime;
    }
}
