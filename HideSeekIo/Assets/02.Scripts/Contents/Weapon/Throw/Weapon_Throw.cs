using UnityEngine;
using Photon.Pun;
using System.Collections;
using System;

public abstract class Weapon_Throw : Weapon 
{
    [SerializeField] GameObject _projectilePrefab;

    float _attackRange;
    public override WeaponType weaponType => WeaponType.Throw;
    public override HandType handType => HandType.Right;

    public float attackRange 
    {
        get => _attackRange;
        protected set
        {
            _attackRange = value;
            var uiCurveZoom = _uI_ZoomBase.GetComponent<UI_CurveZoom>();
            if (uiCurveZoom)
            {
                uiCurveZoom.UpdateRange(value);
            }
        }
    }

    Plane _plane;



    protected override void SetupCallBack()
    {
        inputControllerObject = this.gameObject.GetOrAddComponent<InputControllerObject>();
        inputControllerObject.inputType = InputType.Main;
        inputControllerObject.attackType = Define.AttackType.Joystick;
        inputControllerObject.shooterState = PlayerShooter.state.MoveAttack;
        inputControllerObject.AddUseEvent(Attack);
        inputControllerObject.AddZoomEvent(Zoom);
        _plane = new Plane(Vector3.up, Vector3.zero);

    }

    public void Setup(string animName, float delayTime, float afaterDelayTime, float distance, float newAttackRange)
    {
        AttackAnim = animName;
        AttackDelay = delayTime;
        AfaterAttackDelay = afaterDelayTime;
        AttackDistance = distance;
        attackRange = newAttackRange;
    }

    public override void Zoom(Vector2 inputVector)
    {
        if (inputVector.sqrMagnitude == 0)
        {
            _uI_ZoomBase.SetActiveZoom(false);
            return;
        }
        var endPoint = UtillGame.GetCurveHitPoint(_plane, this.transform, inputVector, AttackDistance);
        var startPoint = this.transform.position + Vector3.up ;
        endPoint.y = 0;
        _uI_ZoomBase.UpdateZoom(startPoint , endPoint);
        _uI_ZoomBase.SetActiveZoom(true);
    }

  

    #region Attack
    /// <summary>
    /// 동시시작을 위해 데이터만넘김
    /// </summary>
    public override void Attack(Vector2 inputVector)
    {
        Vector3 endPoint = UtillGame.GetThrowPosion(inputVector, AttackDistance, playerController.transform);
        endPoint.y = 0;

        photonView.RPC("AttackOnServer", RpcTarget.AllViaServer, endPoint);
    }

    /// <summary>
    /// endPoint로 방향구하기 및 프로세스 시작
    /// </summary>
    /// <param name="endPoint"></param>
    [PunRPC]
    public void AttackOnServer(Vector3 endPoint)
    {
        attackPoint = endPoint;
        StartCoroutine(AttackProcessOnAllClinets(endPoint));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="endPoint"></param>
    /// <returns></returns>

    IEnumerator AttackProcessOnAllClinets(Vector3 endPoint)
    {
        attackStartCallBack?.Invoke();
        yield return new WaitForSeconds(AttackDelay);   //대미지 주기전까지 시간
        var projectile = Managers.Pool.Pop(_projectilePrefab).GetComponent<ThrowProjectileObject>();    //투척물 생성
        Vector3 startPoint = playerController.playerCharacter.animator.GetBoneTransform(HumanBodyBones.RightHand).position;
        projectile.Play(playerController, startPoint, endPoint);
        yield return new WaitForSeconds(AfaterAttackDelay);   //대미지 주기전까지 시간
        attackEndCallBack?.Invoke();
    }
    #endregion



}
