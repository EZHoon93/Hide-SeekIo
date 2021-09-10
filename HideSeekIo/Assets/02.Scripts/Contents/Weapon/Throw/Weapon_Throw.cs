using UnityEngine;
using Photon.Pun;
using System.Collections;
using System;

[RequireComponent(typeof(ObtainableItem))]
public abstract class Weapon_Throw : Weapon 
{
    [SerializeField] GameObject _projectilePrefab;
    public float attackRange { get; protected set; }
    public Action destroyCallBackEvent { get; set; }

    protected override void Awake()
    {
        base.Awake();
        weaponType = WeaponType.Throw;
    }
    protected override void SetupCallBack()
    {
        inputControllerObject = this.gameObject.GetOrAddComponent<InputControllerObject>();
        inputControllerObject.inputType = InputType.Sub3;
        inputControllerObject.attackType = Define.AttackType.Joystick;
        inputControllerObject.shooterState = PlayerShooter.state.MoveAttack;
        inputControllerObject.AddUseEvent(Attack);
        inputControllerObject.AddZoomEvent(Zoom);

    }
    
    public void Setup(string animName, float delayTime, float afaterDelayTime, float distance, float newAttackRange)
    {
        AttackAnim = animName;
        AttackDelay = delayTime;
        AfaterAttackDelay = afaterDelayTime;
        AttackDistance = distance;
        attackRange = newAttackRange;
        //UICanvas.transform.localScale = new Vector3(attackRange, attackRange, attackRange); //범위에 따른 ui변경
    }

    public override void Zoom(Vector2 inputVector)
    {
        var state = UtillGame.ThrowZoom(inputVector, AttackDistance, playerController.transform, uiZoom.currentZoom);
        if (state)
        {
            useState = UseState.Use;
        }
        if (playerController.gameObject.IsValidAI() == false)
        {
            if (inputVector.sqrMagnitude == 0)
            {
                uiZoom.gameObject.SetActive(false);
                useState = UseState.NoUse;
                return;
            }
            uiZoom.FixedUI();
            uiZoom.gameObject.SetActive(true);
        }
        useState = UseState.Use;
    }

   

 #region Attack
    public override void Attack(Vector2 inputVector)
    {
        Vector3 endPoint = UtillGame.GetThrowPosion(inputVector, AttackDistance, playerController.transform);
        //LastAttackInput = inputVector;
        photonView.RPC("AttackOnServer", RpcTarget.AllViaServer, inputVector, endPoint);
    }

    [PunRPC]
    public void AttackOnServer(Vector2 inputVector, Vector3 endPoint)
    {
        //LastAttackInput = inputVector;
        var direction = endPoint - playerController.transform.position;
        direction = direction.normalized;
        direction.y = playerController.transform.position.y;
        inputControllerObject.attackDirection = direction;


        StartCoroutine(AttackProcessOnAllClinets(endPoint));
    }

    IEnumerator AttackProcessOnAllClinets(Vector3 endPoint)
    {
        inputControllerObject.Call_UseSucessStart();
        yield return new WaitForSeconds(AttackDelay);   //대미지 주기전까지 시간
        var projectile = Managers.Pool.Pop(_projectilePrefab).GetComponent<ThrowProjectileObject>();
        Vector3 startPoint = playerController.playerCharacter.animator.GetBoneTransform(HumanBodyBones.RightHand).position;
        projectile.Play(playerController.playerShooter, startPoint, endPoint);
        yield return new WaitForSeconds(AfaterAttackDelay);   //대미지 주기전까지 시간
        inputControllerObject.Call_UseSucessEnd();
        Use(playerController);
    }


    #endregion

    public virtual void Use(PlayerController usePlayerController)
    {
        //if(inputType ==  InputType.Item1)
        //{
        //    if (photonView.IsMine)
        //    {
        //        PhotonNetwork.Destroy(this.gameObject);
        //    }
        //}
    }

    //public abstract Enum GetEnum();

}
