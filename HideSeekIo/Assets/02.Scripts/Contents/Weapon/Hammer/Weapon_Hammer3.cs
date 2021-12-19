using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;

public class Weapon_Hammer3 : Weapon , IParentCollider 
{
    public override WeaponType weaponType => WeaponType.Hammer;
    public override HandType handType => HandType.Right;

    [SerializeField] AudioClip _attackClip;
    [SerializeField] float _amount = 1;
    [SerializeField] Skill_Base _skill_Base;

    float _damageRange = 1.2f;
    
    [SerializeField] ParticleSystem _effect;
    [SerializeField] Transform _effetParent;
    [SerializeField] SeekerCollider _collider;

    RenderController _skinRenderController;
    protected override void SetupCallBack()
    {
        inputControllerObject = this.gameObject.GetOrAddComponent<InputControllerObject>();
        inputControllerObject.inputType = InputType.Sub1;
        inputControllerObject.attackType = Define.AttackType.Joystick;
        inputControllerObject.shooterState = PlayerShooter.state.Skill;
        inputControllerObject.AddZoomEvent(Zoom);
        inputControllerObject.AddUseEvent(Attack);

    }
   
    private void Start()
    {
        AttackAnim = "Attack";
        AttackDelay = 0.25f;
        AfaterAttackDelay = 0.25f;
        AttackDistance = 2.2f;
        inputControllerObject.InitCoolTime = 0.5f;
        _damage = 50;
    }
    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var weaponId = (string)info.photonView.InstantiationData[1];
        var weaponObject = Managers.Resource.Instantiate($"Hammer/{weaponId}");   //유저 무기 아바타 생성
        weaponObject.transform.ResetTransform(_weaponModel);   //아바타 생성된것 자식오브젝트로 이동
        AddSkinRenderController(weaponObject);
        _collider.SetActiveCollider(false);
        base.OnPhotonInstantiate(info);
        //_skill_Base.OnPhotonInstantiate(playerController);
    }

    void AddSkinRenderController(GameObject newSkinObject)
    {
        newSkinObject.gameObject.SetActive(true);
        _skinRenderController = newSkinObject.GetComponent<RenderController>();

        if (_skinRenderController)
        {
            foreach(var skin in _skinRenderController.renderers)
            {
                renderController.renderers.Add(skin);
            }
        }

    }

    public override void OnPreNetDestroy(PhotonView rootView)
    {
        if (_skinRenderController)
        {
            foreach (var skin in _skinRenderController.renderers)
            {
                renderController.renderers.Remove(skin);
            }
        }
        base.OnPreNetDestroy(rootView);
    }

    public override void Zoom(Vector2 inputVector)
    {
        if (inputVector.sqrMagnitude == 0)
        {
            _uI_ZoomBase.SetActiveZoom(false);
            return;
        }
        var endPoint = GetHitPoint(inputVector.normalized);
        var startPoint = this.transform.position + Vector3.up * 0.5f;
        _uI_ZoomBase.UpdateZoom(startPoint, endPoint);
        _uI_ZoomBase.SetActiveZoom(true);

    }
  
    #region Attack

    public override void Attack(Vector2 inputVector)
    {
        var endPoint =  GetHitPoint(inputVector.normalized);
        photonView.RPC("AttackOnServer", RpcTarget.All, endPoint);
    }

    [PunRPC]
    public void AttackOnServer(Vector3 endPoint)
    {
        attackPoint = endPoint;
        StartCoroutine(AttackProcessOnAllClinets(endPoint));
    }

    IEnumerator AttackProcessOnAllClinets(Vector3 endPoint)
    {
        attackStartCallBack?.Invoke();
        var startPoint = this.transform.position;
        //yield return new WaitForSeconds(0.15f);   //대미지 주기전까지 시간
        var distance =  Mathf.Clamp( Vector3.Distance(this.transform.position, endPoint) * 0.08f , 0.05f, 0.4f);
        playerController.playerMove.MoveToTarget(attackPoint,  distance); //거리양예비래
        var dir = UtillGame.GetDirVector3ByEndPoint(this.transform, attackPoint);
        _collider.SetActiveCollider(true);
        //Quaternion newRotation = Quaternion.LookRotation(dir);
        //playerController.playerCharacter.characterAvater.transform.rotation = newRotation;
        //_trailRenderer.enabled = true;
        //_effetParent.transform.rotation = Quaternion.LookRotation(dir);
        this.transform.rotation = Quaternion.LookRotation(dir);
        _effect.Play();
        if (playerController.IsMyCharacter())
        {
            Managers.cameraManager.ShakeCameraByPosition(attackPoint, 0.3f, 0.5f, 0.1f);
            Managers.Sound.Play(_attackClip, Define.Sound.Effect);
        }
        yield return new WaitForSeconds(0.2f);   //대미지 주기전까지 시간
                                                 //Damage(startPoint, attackPoint);
        _collider.SetActiveCollider(false);


        //yield return new WaitForSeconds(0.3f);   
        //_trailRenderer.enabled = false;
        yield return new WaitForSeconds(distance);   //움직이기 . 애니메이션의 끝나면
        attackEndCallBack?.Invoke();
    }

    void Damage(Vector3 startPoint, Vector3 destPoint)
    {
        var direction = (destPoint - startPoint).normalized;
        direction.y = 0;
        startPoint.y = 0.5f;
        destPoint.y = 0.5f;

        Quaternion quaternion = Quaternion.Euler(direction);
        Vector3 boxSize = new Vector3( 0.5f, 0.5f, 0.5f);
        var raycastHits = Physics.BoxCastAll(startPoint, boxSize, direction, this.transform.rotation, AttackDistance, 1 << (int)Define.Layer.Hider);
        foreach (var raycastHit in raycastHits)
        {
            var damageable = raycastHit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.OnDamage(playerController.ViewID(), _damage, raycastHit.collider.transform.position);
                Managers.effectManager.EffectOnLocal(Define.EffectType.Hit, raycastHit.collider.transform.position, 0);
            }
        }


    }

    private void OnDrawGizmos()
    {
        float maxDistance = 100;
        RaycastHit hit;
        // Physics.BoxCast (레이저를 발사할 위치, 사각형의 각 좌표의 절판 크기, 발사 방향, 충돌 결과, 회전 각도, 최대 거리)
        bool isHit = Physics.BoxCast(transform.position, transform.lossyScale / 2, transform.forward, out hit, transform.rotation, maxDistance);

        Gizmos.color = Color.red;
        if (isHit)
        {
            Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
            Gizmos.DrawWireCube(transform.position + transform.forward * hit.distance, transform.lossyScale);
        }
        else
        {
            Gizmos.DrawRay(transform.position, transform.forward * maxDistance);
        }
    }


    void AttackEffect(Vector3 attackPoint)
    {
        Managers.effectManager.EffectOnLocal(Define.EffectType.BodySlam, attackPoint, 0, _damageRange);
        UtillGame.DamageInRange(attackPoint, _damageRange, _damage, playerController.ViewID(), UtillLayer.seekerToHiderAttack);
        if (Managers.cameraManager.IsView(attackPoint) && playerController.IsMyCharacter())
        {
            Managers.cameraManager.ShakeCameraByPosition(attackPoint, 0.3f, 0.4f, 0.1f);
            Managers.Sound.Play(_attackClip, Define.Sound.Effect);
        }
    }
    #endregion


    //Vector3 GetSkillAttackPoint(Vector2 inputVector2)
    //{
    //    var startPoint = playerController.transform.position;
    //    var endPoint = playerController.transform.position+ new Vector3(inputVector2.x, 0, inputVector2.y) * AttackDistance;

    //    var ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(endPoint));
    //    Vector3 targetPoint = Vector3.zero;
    //    float hitDist;
    //    //Vector3 center = Vector3.zero;

    //    if (_plane.Raycast(ray, out hitDist))
    //    {
    //        targetPoint = ray.GetPoint(hitDist);
    //        //center = (startPoint + targetPoint) * 0.5f;
    //        //center.y -= _amount;
    //        RaycastHit hitInfo;
    //        if (Physics.Linecast(startPoint, targetPoint, out hitInfo, 1 << (int)Define.Layer.Ground))
    //        {
    //            targetPoint = hitInfo.point;
    //        }
    //    }
    //    else
    //    {
    //        targetPoint = startPoint;
    //    }

    //    _skillTargetPoint = Vector3.Lerp(_skillTargetPoint, targetPoint, inputVector2.magnitude * 20);

    //    return targetPoint;
    //}

    
    Vector3 GetHitPoint(Vector2 inputVector)
    {
        RaycastHit hit;
        Vector3 hitPosition;
        Vector3 start = playerController.transform.position;
        start.y = 0.5f;
        Vector3 direction = UtillGame.ConventToVector3(inputVector);
        float boxSize = 0.2f;
        if(Physics.BoxCast(start, Vector3.one*boxSize  ,direction,out hit,Quaternion.identity, AttackDistance, 1<<(int)Define.Layer.Wall))
        {
            hitPosition= hit.point;
            hitPosition.y = 0.5f;
        }
        else
        {
            hitPosition = start + direction * AttackDistance;
        }
        //if (Physics.Raycast(start, direction, out hit, AttackDistance, 1 <<(int)Define.Layer.Wall))
        //{
        //    hitPosition = hit.point;
        //    hitPosition.y = playerController.transform.position.y;
        //}
        //else
        //{
        //    hitPosition = start + direction * AttackDistance;
        //}
        hitPosition.y = 0.5f;
        return hitPosition;
    }

   
    public void ParentOnTriggerEnter(Collider other)
    {
        var livingEntity = other.GetComponent<LivingEntity>();
        if (livingEntity)
        {
            livingEntity.OnDamage(playerController.ViewID(), _damage, other.transform.position);
            Managers.effectManager.EffectOnLocal(Define.EffectType.Hit, livingEntity.transform.position, 0);
        }
    }
}


