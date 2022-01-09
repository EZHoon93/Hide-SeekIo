using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;

public class Weapon_Hammer3 : Weapon , IParentCollider 
{
    public override WeaponType weaponType => WeaponType.Hammer;
    //public override HandType handType => HandType.Right;

    [SerializeField] AudioClip _attackClip;
    [SerializeField] float _amount = 1;
    //[SerializeField] Skill_Base _skill_Base;

    float _damageRange = 1.2f;
    
    [SerializeField] ParticleSystem _effect;
    [SerializeField] Transform _effetParent;
    [SerializeField] SeekerCollider _collider;

    RenderController _skinRenderController;

    int lastAttackHitCount;


   
    private void Start()
    {
        AttackAnim = "Attack";
        AttackDelay = 0.25f;
        AfaterAttackDelay = 0.25f;
        AttackDistance = 2.2f;
        inputControllerObject.InitCoolTime = 0.5f;
        _damage = 2;
    }

    protected override void Setup(object[] infoData)
    {
        var weaponSkinKeyIndex = (int)infoData[1];
        var weaponObject = CreateWeaponSkinByIndex(weaponSkinKeyIndex , _weaponModel);
        AddSkinRenderController(weaponObject);
        _collider.SetActiveCollider(false);

        base.Setup(infoData);
    }
    GameObject CreateWeaponSkinByIndex(int skinKeyIndex , Transform parent = null)
    {
        var go = Managers.ProductSetting.CreateSkin(Define.ProductType.Hammer, 0 , _weaponModel);
        go.transform.ResetTransform();
        return go;
    }

    void AddSkinRenderController(GameObject newSkinObject)
    {
        newSkinObject.gameObject.SetActive(true);
        _skinRenderController = newSkinObject.GetComponent<RenderController>();
        _skinRenderController.OnPhotonInstantiate(playerController.playerHealth);
    }

    public override void OnPreNetDestroy(PhotonView rootView)
    {
        if (_skinRenderController)
        {
            _skinRenderController.OnPreNetDestroy();
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
        lastAttackHitCount = 0;
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
            Managers.CameraManager.ShakeCameraByPosition(attackPoint, 0.3f, 0.5f, 0.1f);
            Managers.Sound.Play(_attackClip, Define.Sound.Effect);
        }
        yield return new WaitForSeconds(0.2f);   //대미지 주기전까지 시간
                                                 //Damage(startPoint, attackPoint);
        _collider.SetActiveCollider(false);

        CheckAnyHit();      //맞추는지..
        //yield return new WaitForSeconds(0.3f);   
        //_trailRenderer.enabled = false;
        yield return new WaitForSeconds(distance);   //움직이기 . 애니메이션의 끝나면
        attackEndCallBack?.Invoke();
    }

    void CheckAnyHit()
    {
        print(lastAttackHitCount);
        if(lastAttackHitCount <= 0)
        {
            playerController.playerHealth.OnDamage(playerController.ViewID(), 1, Vector3.zero);
        }
    }

  
 
    #endregion


  

    
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
            print("222");
            lastAttackHitCount++;
            print(_damage);
            livingEntity.OnDamage(playerController.ViewID(), _damage, other.transform.position);
            Managers.effectManager.EffectOnLocal(Define.EffectType.Hit, livingEntity.transform.position, 0);
        }
    }
}


