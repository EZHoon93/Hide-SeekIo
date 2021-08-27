using UnityEngine;
using Photon.Pun;
using System.Collections;
using System;

[RequireComponent(typeof(ObtainableItem))]
public abstract class Weapon_Throw : Weapon 
{
    [SerializeField] GameObject _projectilePrefab;
    //public ObtainableItem obtainableItem { get; private set; }
    public float attackRange { get; protected set; }
    public Action destroyCallBackEvent { get; set; }
    public Define.ThrowItem throwType { get; set; }
    //public override Define.ZoomType zoomType { get; set; } = Define.ZoomType.Throw;


    protected override void Awake()
    {
        base.Awake();
        weaponType = WeaponType.Throw;
    }
    protected override void SetupCallBack()
    {
        inputControllerObject = this.gameObject.GetOrAddComponent<InputControllerObject>();
        //inputControllerObject = inputControllerObject ?? new InputControllerObject();
        inputControllerObject.inputType = InputType.Sub3;
        inputControllerObject.attackType = Define.AttackType.Joystick;
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
        state = State.Delay;
        Vector3 endPoint = UtillGame.GetThrowPosion(inputVector, AttackDistance, playerController.transform);
        LastAttackInput = inputVector;
        photonView.RPC("AttackOnServer", RpcTarget.AllViaServer, inputVector, endPoint);
    }

    [PunRPC]
    public void AttackOnServer(Vector2 inputVector, Vector3 endPoint)
    {
        LastAttackInput = inputVector;
        StartCoroutine(AttackProcessOnAllClinets(endPoint));
    }

    IEnumerator AttackProcessOnAllClinets(Vector3 endPoint)
    {
        state = State.Delay;
        //AttackSucessEvent?.Invoke();
        inputControllerObject.Call_UseSucessStart();
        yield return new WaitForSeconds(AttackDelay);   //대미지 주기전까지 시간
        var projectile = Managers.Pool.Pop(_projectilePrefab).GetComponent<ThrowProjectileObject>();
        Vector3 startPoint = playerController.character_Base.animator.GetBoneTransform(HumanBodyBones.RightHand).position;

        projectile.Play(playerController.playerShooter, startPoint, endPoint);
        yield return new WaitForSeconds(AfaterAttackDelay);   //대미지 주기전까지 시간
        inputControllerObject.Call_UseSucessEnd();
        //AttackEndEvent?.Invoke();
        state = State.End;
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
