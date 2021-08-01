using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Weapon_Melee2 : Weapon
{
 

    [SerializeField] Transform _modelTransform;
    [SerializeField] Transform _attackRangeUI;
    AudioClip _attackClip;

    //[SerializeField] float _angle = 120;

    int _attackLayer = 1 << (int)Define.Layer.Hider ;

    protected override void Awake()
    {
        base.Awake();
        weaponType = WeaponType.Melee;
        type = Type.Permanent;

    }
    private void Start()
    {
        AttackAnim = "Attack";
        AttackDelay= 0.5f;
        AfaterAttackDelay= 0.5f;
        AttackDistance= 2;

        _attackClip = Resources.Load<AudioClip>("Sounds/SMelee2");
    }
    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var weaponId = (string)info.photonView.InstantiationData[2];
        _weaponModel = Managers.Resource.Instantiate($"Melee2/{weaponId}").transform;   //유저 무기 아바타 생성
        _weaponModel.ResetTransform(_modelTransform);   //아바타 생성된것 자식오브젝트로 이동
        base.OnPhotonInstantiate(info);
        
        _attackRangeUI.gameObject.SetActive(false);     // 공격 UI
        hasPlayerController.GetAttackBase().SetupWeapon(this, true);
        //attackPlayer.UseWeapon(this);    //무기 사용상태로 전환

    }
    public override bool Zoom(Vector2 inputVector)
    {
        if (inputVector.sqrMagnitude == 0)
        {
            _attackRangeUI.gameObject.SetActive(false);
            return false;
        }
        _attackRangeUI.rotation = UtillGame.WorldRotationByInput(inputVector);
        _attackRangeUI.gameObject.SetActive(true);
        useState = UseState.Use;
        return true;
    }


    #region Attack

    public override void Attack(Vector2 inputVector)
    {
        state = State.Delay;
        LastAttackInput = inputVector;
        photonView.RPC("AttackOnServer", RpcTarget.AllViaServer, LastAttackInput);
    }

    [PunRPC]
    public void AttackOnServer(Vector2 inputVector)
    {
        StartCoroutine(AttackProcessOnAllClinets(inputVector));
    }
    IEnumerator AttackProcessOnAllClinets(Vector2 inputVector)
    {
        state = State.Delay;
        LastAttackInput = inputVector;
        AttackSucessEvent?.Invoke(this);
        //var angle = _attackRangeUI.rotation.eulerAngles;
        //angle.y = angle.y - 180;
        //_attackWarningRangeUI.rotation = Quaternion.Euler( angle);
        yield return new WaitForSeconds(AttackDelay);   //대미지 주기전까지 시간
        AttackEffect();
        yield return new WaitForSeconds(AfaterAttackDelay);   //대미지 주기전까지 시간
        AttackEndEvent?.Invoke();
        state = State.End;
    }

    void AttackEffect()
    {
        var attackPos = hasPlayerController.transform.position + hasPlayerController.transform.forward * AttackDistance * 0.5f;
        print(attackPos + "어택이펙트");
        EffectManager.Instance.EffectToServer(Define.EffectType.BodySlam, attackPos, 0);
        UtillGame.DamageInRange(hasPlayerController.transform, AttackDistance, 10, hasPlayerController.ViewID(), UtillLayer.seekerToHiderAttack, 110);
        Managers.Sound.Play(_attackClip, Define.Sound.Effect);
        //Collider[] colliders = new Collider[10];

        //var hitCount = Physics.OverlapSphereNonAlloc(this.transform.position, AttackDistance, colliders, _attackLayer);
        //if (hitCount > 0)
        //{
        //    print(hitCount + "힛카운트");
        //    for (int i = 0; i < hitCount; i++)
        //    {

        //        print(colliders[i].gameObject.name);
        //        if (IsTargetOnSight(colliders[i].transform))
        //        {
        //            print("시야안");
        //        }
        //        else
        //        {
        //            print("시야박");
        //        }
        //        var damageable = colliders[i].gameObject.GetComponent<IDamageable>();
        //        if (damageable != null)
        //        {
        //            damageable.OnDamage(5, 5, colliders[i].transform.position);
        //        }
        //    }

        //}
    }

    #endregion


    private bool IsTargetOnSight(Transform target)
    {
        RaycastHit hit;
        Vector3 startPoint = hasPlayerController.transform.position;
        Vector3 endPoint = target.transform.position;

        startPoint.y = 0.5f;
        endPoint.y = 0.5f;

        var direction = endPoint - startPoint;

        if (Vector3.Angle(direction, hasPlayerController.transform.forward) > 120 * 0.5f)
        {
            return false;
        }


        if (Physics.Raycast(startPoint, direction, out hit, 11, _attackLayer))
        {
            if (hit.transform == target) return true;
        }

        return false;
    }
}
