using System.Collections;
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
    public Skill_Base currentSkill { get; set; }
    InputControllerObject currentInputController = null;
    Skill_Base _currentSkill;
    GameObject _inGameItem;

    [Header("Transform")]
    public Action<int> weaponChangeCallBack;
    Transform upperSpine;
    public Vector3 AttackDirection { get; private set; }
    public GameObject inGameItem => _inGameItem;
    protected Vector3 upperBodyDir;
    protected bool rotate = false;
    public float baseWeaponReaminTime;

    public ConsumItem consumItem { get; set; }

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
        //currentInputController.Zoom(_playerInput.controllerInputDic[currentInputController.inputType].inputVector2 );

        
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
        
    }



    public void ChangeWeapon(Weapon useNewWeapon, bool isBaseWeapon)
    {
        if (isBaseWeapon)
            baseWeapon = useNewWeapon;

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
        currentInputController = inputControllerObject;
        print("UseInputControllerObject");
        if (this.IsMyCharacter())
        {
            //내캐릭이라면 줌을꺼줌
            inputControllerObject.Zoom(Vector2.zero);
        }
        if(inputControllerObject.Use(inputVector2))
        {
            if (baseWeapon)
            {
                ChangeWeapon(baseWeapon,true);
            }
        }
        //if (currentInputController.consumItem)
        //{
        //    currentInputController = null;
        //}
    }
    public void ZoomInputConrollerObject(Vector2 inputVector2, InputControllerObject inputControllerObject)
    {
        if(currentInputController != null)
        {
            currentInputController.Zoom(Vector2.zero);
        }
        currentInputController = inputControllerObject;
        currentInputController.Zoom(inputVector2);
    }


    public virtual void WeaponAttackStart(Weapon attackWeapon)
    {
        State = attackWeapon.inputControllerObject.shooterState;
        AttackDirection = attackWeapon.inputControllerObject.attackDirection; ;
        if (attackWeapon.weaponType == Weapon.WeaponType.Hammer)
        {

        }
        else
        {
            rotate = true;
        }
        _animator.SetTrigger(attackWeapon.AttackAnim);
    }
    public virtual void AttackBaseEnd()
    {
        rotate = false;
        if (baseWeapon)
        {
            if(currentInputController == null)
            {
                ChangeWeapon(baseWeapon,true);
            }
        }
        State = state.Idle;

    }

    [PunRPC]

    public void Dash(Vector3 playerPos,  Vector3 inputVector2)
    {
        this.transform.position = playerPos;
        StartCoroutine(DashProcess(inputVector2));
    }

    IEnumerator DashProcess( Vector3 inputVector2)
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







    #region --
    //public virtual void SetupWeapon(Weapon newWeapon)
    //{
    //    newWeapon.inputControllerObject.useSucessStartCallBack += () => WeaponAttackSucess(newWeapon);
    //    newWeapon.inputControllerObject.useSucessEndCallBack += AttackBaseEnd;
    //    weaponChangeCallBack += newWeapon.WeaponChange;

    //    SetupEquipmentable(newWeapon.equipmentable);
    //    SetupControllerObject(newWeapon.inputControllerObject);
    //    switch (newWeapon.inputControllerObject.inputType)
    //    {
    //        case InputType.Sub1:    //술래!!
    //            if (baseWeapon)
    //            {
    //                if (baseWeapon.photonView.IsMine)
    //                    PhotonNetwork.Destroy(baseWeapon.gameObject);
    //            }
    //            if(newWeapon.weaponType== Weapon.WeaponType.Hammer)
    //            {
    //                GetComponent<PlayerController>().ChangeTeam(Define.Team.Seek);
    //            }
    //            if (newWeapon.weaponType == Weapon.WeaponType.Gun)
    //            {

    //            }
    //            baseWeapon = newWeapon;
    //            //ChangeWeapon(baseWeapon);
    //            break;
    //        case InputType.Sub3:
    //            if (_inGameItem)
    //            {
    //                if (photonView.IsMine)
    //                {
    //                    PhotonNetwork.Destroy(_inGameItem.gameObject);
    //                }
    //            }
    //            _inGameItem = newWeapon.gameObject;
    //            break;
    //    }

    //    GetComponent<PlayerHealth>().AddRenderer(newWeapon.renderController);
    //}

    //public void SetupSkill(Skill_Base newSkill)
    //{
    //    if (currentSkill)
    //    {
    //        Destroy(currentSkill);
    //    }

    //    _currentSkill = newSkill;
    //    SetupControllerObject(_currentSkill.inputControllerObject) ;
    //}
    //public void SetupImmediateGameItem(Item_Base item_Base)
    //{
    //    if (_inGameItem)
    //    {
    //        if (photonView.IsMine)
    //        {
    //            PhotonNetwork.Destroy(_inGameItem.gameObject);
    //        }
    //    }
    //    _inGameItem = item_Base.gameObject;
    //    SetupControllerObject(item_Base.inputControllerObject);
    //}

    //public void SetupControllerObject(InputControllerObject newInputControllerObject)
    //{
    //    newInputControllerObject.SetupPlayerController(GetComponent<PlayerController>());
    //    if (newInputControllerObject.attackType == Define.AttackType.Button)
    //    {
    //        _playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Down, newInputControllerObject.inputType,
    //          (input) => { UseInputControllerObject(input, newInputControllerObject); }, newInputControllerObject.sprite);
    //        _playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Up, newInputControllerObject.inputType, null);
    //    }
    //    else
    //    {
    //        _playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Down, newInputControllerObject.inputType,
    //          (input) => { ZoomInputConrollerObject(input, newInputControllerObject);  });
    //        _playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Up, newInputControllerObject.inputType, (input) => UseInputControllerObject(input, newInputControllerObject) , newInputControllerObject.sprite);
    //    }
    //}

    //public void SetupEquipmentable(Equipmentable equipmentable)
    //{
    //    var avater = _animator.GetComponent<CharacterAvater>();
    //    var adaptTransform = avater.GetSkinParentTransform(equipmentable.equipSkiType);
    //    equipmentable.transform.ResetTransform(adaptTransform);
    //}

    #endregion
}
