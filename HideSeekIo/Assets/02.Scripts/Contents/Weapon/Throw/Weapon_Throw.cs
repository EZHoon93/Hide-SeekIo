using UnityEngine;
using Photon.Pun;
using System.Collections;
using System;

public abstract class Weapon_Throw : Weapon 
{
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float amount;
    [SerializeField] float plane;
    [SerializeField] float speed;

    public override WeaponType weaponType => WeaponType.Throw;
    public float attackRange { get; protected set; }

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);
        if (uiZoom)
        {
            uiZoom.transform.localScale = new Vector3(attackRange, attackRange, attackRange); //범위에 따른 ui변경
            lineRenderer.enabled = false;
            lineRenderer.positionCount = 25;
        }
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
    }

    public override void Zoom(Vector2 inputVector)
    {
        bool state = false;
        if (uiZoom)
        {
             state = UtillGame.ThrowZoom(inputVector, AttackDistance, playerController.transform, uiZoom.currentZoom, speed);
            //////
            ///
            //var endPoint = UtillGame.GetThrowPosion(inputVector, AttackDistance, playerController.transform);
            //GetHitZoom(playerController.transform, endPoint);
        }
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

    public  void GetHitZoom(Transform attackStart, Vector3 endPoint)
    {
        Plane playerPlane = new Plane(Vector3.up, plane);

        var ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(endPoint));
        Vector3 targetPoint = Vector3.zero;
        float hitDist;
        Vector3 center = Vector3.zero;
        
        if (playerPlane.Raycast(ray, out hitDist))
        {
            targetPoint = ray.GetPoint(hitDist);
            center = (attackStart.transform.position + targetPoint) * 0.5f;
            center.y -= amount;
            RaycastHit hitInfo;
            if (Physics.Linecast(attackStart.position, targetPoint, out hitInfo, 1 << (int)Define.Layer.Ground))
            {
                targetPoint = hitInfo.point;
            }
        }
        else
        {
            targetPoint = attackStart.transform.position;
        }
        //targetPoint.y = 0;
        //playerUI.UpdateDamageUI(targetPoint, 5);   //타겟 위치 UI 표시
        Vector3 RelCenter = attackStart.position - center;
        Vector3 aimRelCenter = targetPoint - center;
        Vector3 theArc;
        for (float index = 0.0f, interval = -0.0417f; interval < 1.0f;)
        {
            theArc = Vector3.Slerp(RelCenter, aimRelCenter, interval += 0.0417f);
            lineRenderer.SetPosition((int)index++, theArc + center);
        }
        print(targetPoint);
        var s = targetPoint;
        s.y = 0;
        lineRenderer.SetPosition(24, s);


        lineRenderer.enabled = true;
    }

    #region Attack
    /// <summary>
    /// 동시시작을 위해 데이터만넘김
    /// </summary>
    public override void Attack(Vector2 inputVector)
    {
        Vector3 endPoint = UtillGame.GetThrowPosion(inputVector, AttackDistance, playerController.transform);
        photonView.RPC("AttackOnServer", RpcTarget.AllViaServer, endPoint);
    }

    /// <summary>
    /// endPoint로 방향구하기 및 프로세스 시작
    /// </summary>
    /// <param name="endPoint"></param>
    [PunRPC]
    public void AttackOnServer(Vector3 endPoint)
    {
        inputControllerObject.attackDirection = UtillGame.GetDirVector3ByEndPoint(playerController.transform, endPoint).normalized; ;
        StartCoroutine(AttackProcessOnAllClinets(endPoint));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="endPoint"></param>
    /// <returns></returns>

    IEnumerator AttackProcessOnAllClinets(Vector3 endPoint)
    {
        inputControllerObject.Call_UseSucessStart();
        yield return new WaitForSeconds(AttackDelay);   //대미지 주기전까지 시간
        var projectile = Managers.Pool.Pop(_projectilePrefab).GetComponent<ThrowProjectileObject>();    //투척물 생성
        Vector3 startPoint = playerController.playerCharacter.animator.GetBoneTransform(HumanBodyBones.RightHand).position;
        projectile.Play(playerController, startPoint, endPoint);
        yield return new WaitForSeconds(AfaterAttackDelay);   //대미지 주기전까지 시간
        inputControllerObject.Call_UseSucessEnd();

    }
    #endregion




}
