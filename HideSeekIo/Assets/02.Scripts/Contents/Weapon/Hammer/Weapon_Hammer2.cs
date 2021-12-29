using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Weapon_Hammer2 : Weapon
{

    float _damageRange = 1.2f;
    int _damage = 10;
    AudioClip _attackClip;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float startTime;
    [SerializeField] float _maxDistance = 3;
    [SerializeField] LayerMask _zoomLayer;
    bool zoom;
    public override WeaponType weaponType => WeaponType.Hammer;
    public override HandType handType => HandType.Right;

    protected override void SetupCallBack()
    {
        inputControllerObject = this.gameObject.GetOrAddComponent<InputControllerObject>();
        inputControllerObject.inputType = InputType.Sub1;
        inputControllerObject.attackType = Define.AttackType.Button;
        inputControllerObject.shooterState = PlayerShooter.state.Skill;
        inputControllerObject.AddZoomEvent(Zoom);
        inputControllerObject.AddUseEvent(Attack);
    }

    private void Update()
    {
        
    }
    private void Start()
    {
        AttackAnim = "JumpAttack";
        AttackDelay = 1.25f;
        AfaterAttackDelay = 0.25f;
        AttackDistance = 1;
        inputControllerObject.InitCoolTime = 1;
        _attackClip = Resources.Load<AudioClip>("Sounds/SMelee2");
    }
    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var weaponId = (string)info.photonView.InstantiationData[1];
        var weaponObject = Managers.Resource.Instantiate($"Hammer/{weaponId}");   //유저 무기 아바타 생성
        weaponObject.transform.ResetTransform(_weaponModel);   //아바타 생성된것 자식오브젝트로 이동
        renderController = weaponObject.GetComponent<RenderController>();
        base.OnPhotonInstantiate(info);
        zoom = false;

        //playerController.playerInput.AddInputEvent(Define.AttackType.Button)
        //_zoomUI.gameObject.SetActive(false);     // 공격 UI
        //playerController.GetAttackBase().SetupWeapon(this);
        //attackPlayer.UseWeapon(this);    //무기 사용상태로 전환

    }
    public override void Zoom(Vector2 inputVector)
    {
        if (zoom) return;
        print("Zoom!");
        zoom = true;
        StopAllCoroutines();
        StartCoroutine(ZoomStart());
    }

    IEnumerator ZoomStart()
    {
        startTime = 0;
        lineRenderer.SetPosition(0, playerController.transform.position);
        while (true)
        {
            startTime += Time.deltaTime ;
            if (startTime >=2)
            {
                startTime = 2;
            }
            var hitPoint = GetHitPoint(playerController.playerCharacter.characterAvater.transform,startTime);
            lineRenderer.SetPosition(1, hitPoint);
            lineRenderer.enabled = true;
            yield return null;
        }
    }
    Vector3 GetHitPoint(Transform rayTransform, float maxDistance)
    {
        RaycastHit hit;
        Vector3 hitPosition;
        Vector3 start = rayTransform.transform.position;
        start.y = 0.5f;
        Vector3 direction = rayTransform.transform.forward;
        if (Physics.Raycast(start, direction, out hit, maxDistance, _zoomLayer))
        {
            hitPosition = hit.point;
            hitPosition.y = rayTransform.transform.position.y;
        }
        else
        {
            hitPosition = start + direction * maxDistance;

        }
        hitPosition.y = rayTransform.transform.position.y;
        return hitPosition;
    }

    #region Attack

    public override void Attack(Vector2 inputVector)
    {
        if (photonView.IsMine == false) return;
        StopAllCoroutines();
        zoom = false;
        lineRenderer.enabled = false;

        var direction = playerController.playerCharacter.characterAvater.transform.forward;//아바타가보는 정면
        var attackPoint = playerController.transform.position + direction * AttackDistance;
        photonView.RPC("AttackOnServer", RpcTarget.AllViaServer, attackPoint);
    }

    [PunRPC]
    public void AttackOnServer(Vector3 attackPoint)
    {
        StartCoroutine(AttackProcessOnAllClinets(attackPoint));
    }

    IEnumerator AttackProcessOnAllClinets(Vector3 attackPoint)
    {
        var direction = attackPoint - playerController.transform.position;
        direction = direction.normalized;
        direction.y = playerController.transform.position.y;
        //inputControllerObject.attackDirection = direction;
        //inputControllerObject.Call_UseSucessStart();
        yield return new WaitForSeconds(AttackDelay);   //대미지 주기전까지 시간
        AttackEffect(attackPoint);
        yield return new WaitForSeconds(AfaterAttackDelay);   //움직이기 . 애니메이션의 끝나면
        //inputControllerObject.Call_UseSucessEnd();
        print("End!!");
    }

    void AttackEffect(Vector3 attackPoint)
    {
        Managers.effectManager.EffectOnLocal(Define.EffectType.BodySlam, attackPoint, 0, _damageRange);
        UtillGame.DamageInRange(attackPoint, _damageRange, _damage, playerController.ViewID(), UtillLayer.seekerToHiderAttack);
        //if (Managers.cameraManager.IsView(attackPoint) && playerController.IsMyCharacter())
        //{
        //    Managers.cameraManager.ShakeCameraByPosition(attackPoint, 0.3f, 0.7f, 0.1f);
        //    Managers.Sound.Play(_attackClip, Define.Sound.Effect);
        //}
    }

    #endregion

}
