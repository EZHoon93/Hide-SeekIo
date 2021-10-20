using System.Collections;
using UnityEngine;
using Photon.Pun;
using System;

public class PlayerShooter : MonoBehaviourPun
{
    public enum state
    {
        Idle,
        MoveAttack,
        Skill
    }
    public state State { get; protected set; }
    PlayerCharacter _playerCharacter;
    PlayerInput _playerInput;
    PlayerStat _playerStat;
    InputControllerObject currentInputController ;

    //Skill_Base _currentSkill;
    [SerializeField] Transform upperSpine;
    [SerializeField] Transform _weaponPivot;
    [SerializeField] Transform _leftHand;
    [SerializeField] Transform _rightHand;


    Animator _animator => _playerCharacter.animator;
    public Weapon baseWeapon { get; protected set; }    //안없어지는무기
    public Skill_Base currentSkill { get; set; }

    public Vector3 AttackDirection { get; private set; }
    public ConsumItem consumItem { get; set; }


    public Action<int> weaponChangeCallBack;
    protected Vector3 upperBodyDir;
    public float baseWeaponReaminTime;

    [SerializeField] float _currentEnergy;
    public float currentEnergy
    {
        get => _currentEnergy;
        set
        {
            _currentEnergy = value;
            onChangeCurrEnergyListeners?.Invoke(value);
        }
    }

    [SerializeField] int _maxEnergy;
    public int maxEnergy
    {
        get => _maxEnergy;
        set
        {
            _maxEnergy = value;
            onChangeMaxEnergyListeners?.Invoke(value);
        }
    }

    float _energyRegenAmount;

    public event Action<int> onChangeMaxEnergyListeners;
    public event Action<float> onChangeCurrEnergyListeners;

    private void OnEnable()
    {
        State = state.Idle;
    }

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerCharacter = GetComponent<PlayerCharacter>();
        _playerStat = GetComponent<PlayerStat>();
    }
    
    private void LateUpdate()
    {
        if (State == state.MoveAttack)
        {
            if (AttackDirection == Vector3.zero) return;
            if (upperSpine == null)
            {
                upperSpine = _animator.GetBoneTransform(HumanBodyBones.Spine);
            }

            Vector3 spineRot = Quaternion.LookRotation(AttackDirection).eulerAngles;
            spineRot -= _playerCharacter.characterAvater.transform.eulerAngles;
            float addAngle = 0;
            if(baseWeapon.weaponType == Weapon.WeaponType.Bow)
            {
                //addAngle = 45;
            }

            upperSpine.transform.localRotation = Quaternion.Euler(
                upperSpine.transform.localEulerAngles.x - spineRot.y + addAngle,
                upperSpine.transform.localEulerAngles.y,
                upperSpine.transform.localEulerAngles.z
             );
            //Vector3 spineRot = Quaternion.LookRotation(AttackDirection).eulerAngles;

            //upperSpine.transform.rotation = Quaternion.Euler(
            //    spineRot.y,
            //    0,
            //    0
            // );
            //print(spineRot.x);
        }

        if (photonView.IsMine == false) return;
        RecoveryCurrentEnergy();

        if (this.IsMyCharacter() == false ) return;
        UpdateZoom();
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

    void RecoveryCurrentEnergy()
    {
        currentEnergy = Mathf.Clamp(currentEnergy + Time.deltaTime * _energyRegenAmount , 0, maxEnergy);
    }

    void GameStart_SeletRandomSkill()
    {
        //로컬유저만 실
        if (this.photonView.IsMine == false) return;
        var weaponSelectArray = Managers.StatSelectManager.RandomSelectOnlyWeapon();    //랜덤으로 선택된 3개의 스킬목록
        foreach (var s in weaponSelectArray)
            print(s.ToString());
        //컨트롤 캐릭
        if (this.IsMyCharacter())
        {
            var uimain = Managers.UI.SceneUI as UI_Main;
            uimain.StatController.ShowWeaponList(weaponSelectArray);
        }
        //AI
        else
        {
            //print("AI데이터보냄");
            //var ranSelect = UnityEngine.Random.Range(0, statSelectArray.Length);
            //var selectType = statSelectArray[ranSelect];
            //Managers.StatSelectManager.PostEvent_StatDataToServer(GetComponent<PlayerController>(), selectType);

        }
    }




    public void ChangeWeapon(Weapon useNewWeapon)
    {
        weaponChangeCallBack?.Invoke(useNewWeapon.GetInstanceID());
        //currentInputController = useNewWeapon.inputControllerObject;
        maxEnergy = useNewWeapon.maxEnergy;
        _energyRegenAmount = useNewWeapon.energyRegenAmount;
        SetupAnimation(useNewWeapon);
    }


    protected void SetupAnimation(Weapon newWeapon)
    {
        switch (newWeapon.weaponType)
        {
            case Weapon.WeaponType.Gun:
                //_animator.SetBool("Gun", true);
                //_animator.SetBool("Melee", false);
                _animator.CrossFade("Gun", 0.1f);

                break;
            case Weapon.WeaponType.Melee:
                //_animator.SetBool("Gun", false);
                //_animator.SetBool("Melee", true);

                break;
            case Weapon.WeaponType.Throw:
                //_animator.SetBool("Melee", false);
                _animator.CrossFade("Throw", 0.1f);
                //_animator.cor
                break;
            case Weapon.WeaponType.Hammer:
                //_animator.SetBool("Hammer", true);
                _animator.CrossFade("Hammer", 0.1f);
                break;
            case Weapon.WeaponType.Bow:
                //_animator.SetBool("Hammer", true);
                _animator.CrossFade("Bow", 0.1f);
                break;
        }

    }

    #region Setup 
    public void SetupWeapon(Weapon newWeapon)
    {
        //var playerShooter = playerController.playerShooter;
        newWeapon.transform.ResetTransform(this.transform);
        newWeapon.attackStartCallBack += () => WeaponAttackStart(newWeapon);
        newWeapon.attackEndCallBack += AttackBaseEnd;
        weaponChangeCallBack += newWeapon.WeaponChange;
        if (baseWeapon)
        {
            Managers.Resource.PunDestroy(baseWeapon);
        }
        baseWeapon = newWeapon;
        ChangeWeapon(newWeapon);
        //currentEnergy = maxEnergy;
        var uiZoom = this.IsMyCharacter() ? true : false;
        baseWeapon.uI_ZoomBase.gameObject.SetActive(uiZoom);
        //switch (newWeapon.inputControllerObject.inputType)
        //{
        //    case InputType.Main:    //술래!!
            
        //        if (newWeapon.weaponType == Weapon.WeaponType.Gun)
        //        {

        //        }
        //        break;
        //    case InputType.Sub3:

        //        break;
        //}
    }

    public void SetupInputControllerObject(InputControllerObject newInputControllerObject)
    {
        var inputType = newInputControllerObject.inputType;
        var attackType = newInputControllerObject.attackType;
        var sprite = newInputControllerObject.sprite;
        if (newInputControllerObject.attackType == Define.AttackType.Button)
        {
            //playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Up, newInputControllerObject.inputType, null);
            //playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Down, newInputControllerObject.inputType,
            //  (input) => { playerShooter.UseInputControllerObject(input, newInputControllerObject); }, newInputControllerObject.sprite);

            //playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Down, newInputControllerObject.inputType,
            //(input) => { playerShooter.ZoomInputConrollerObject(input, newInputControllerObject); });
            //playerInput.AddInputEvent(newInputControllerObject.attackType, ControllerInputType.Up, newInputControllerObject.inputType,
            //  (input) => { playerShooter.UseInputControllerObject(input, newInputControllerObject); }, newInputControllerObject.sprite);
        }
        else
        {
            _playerInput.AddInputEvent(attackType, ControllerInputType.Down, inputType,(input) => { ChangeInputConrollerObject(input, newInputControllerObject); });
            _playerInput.AddInputEvent(attackType, ControllerInputType.Up, inputType, (input) => UseInputControllerObject(input, newInputControllerObject));
        }

        _playerInput.SetupControllerInputUI(attackType, inputType, sprite);
    }

    #endregion
    public void UseInputControllerObject(Vector2 inputVector2, InputControllerObject inputControllerObject)
    {
        if (this.enabled == false) return;
        if (this.State != state.Idle || currentEnergy < 1) return;
        currentInputController = inputControllerObject;
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
    public void ChangeInputConrollerObject(Vector2 inputVector2, InputControllerObject inputControllerObject)
    {
        if(currentInputController != null)
        {
            currentInputController.Zoom(Vector2.zero);
        }
        currentInputController = inputControllerObject;
    }

    void UpdateZoom()
    {
        if (currentInputController == null) return;
        var currInputType = currentInputController.inputType;
        var inputVector2 = _playerInput.controllerInputDic[currInputType].inputVector2;
        currentInputController.Zoom(inputVector2);
    }


    public virtual void WeaponAttackStart(Weapon attackWeapon)
    {
        currentEnergy -= 1;
        State = attackWeapon.inputControllerObject.shooterState;
        AttackDirection = UtillGame.GetDirVector3ByEndPoint(this.transform, attackWeapon.attackPoint);
        if (attackWeapon.weaponType == Weapon.WeaponType.Hammer)
        {

        }
        else
        {
            State = state.MoveAttack;
        }
        _animator.SetTrigger(attackWeapon.AttackAnim);
    }
    public virtual void AttackBaseEnd()
    {
        if (baseWeapon)
        {
            if(currentInputController == null)
            {
                ChangeWeapon(baseWeapon);
            }
        }
        State = state.Idle;

    }

  

    /// <summary>
    /// 주 무기 남은 쿨타임 
    /// </summary>
    public float GetBaseWeaponRemainCoolTime()
    {
        if(baseWeapon == null)
        {
            return -1;
        }
        return baseWeapon.inputControllerObject.RemainCoolTime;
    }

    // 애니메이터의 IK 갱신
    public void OnAnimatorIK(int layerIndex)
    {
        //if (baseWeapon == null) return;
        //// 총의 기준점 gunPivot을 3D 모델의 오른쪽 팔꿈치 위치로 이동
        //_weaponPivot.position = _animator.GetIKHintPosition(AvatarIKHint.RightElbow);

        //// IK를 사용하여 왼손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다
        //_animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        //_animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        //_animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHand.position);
        //_animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHand.rotation);

        //// IK를 사용하여 오른손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다
        //_animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        //_animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        //_animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHand.position);
        //_animator.SetIKRotation(AvatarIKGoal.RightHand, _rightHand.rotation);
    }

}
