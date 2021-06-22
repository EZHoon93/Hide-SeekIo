using System.Collections;

using Photon.Pun;

using UnityEngine;

public class Weapon_Melee2 : Weapon
{
 

    [SerializeField] Transform _modelTransform;
    [SerializeField] Transform _attackRangeUI;
    [SerializeField] float _angle = 120;

    int _attackLayer = (1 << (int)Define.Layer.Hider) | (1 << (int)Define.Layer.Item);

    protected void Awake()
    {
        weaponType = WeaponType.Melee;
        weaponServerKey = Define.Weapon.Melee2;
    }
    private void Start()
    {
        _attackAnimationName = "Attack";
        _attackDelayTime = 0.7f;
        _afterAttackDelayTime = 0.5f;
        _distance = 1.5f;
    }

    private void OnEnable()
    {
        _attackRangeUI.gameObject.SetActive(false);     // 공격 UI
    }

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);
        var weaponId = (string)info.photonView.InstantiationData[1];
        var weaponSkin = Managers.Resource.Instantiate($"Melee2/{weaponId}");
        weaponSkin.transform.ResetTransform(_modelTransform);
    }
    public override bool Attack(Vector2 inputVector)
    {
        AttackDirecition = UtillGame.ConventToVector3(inputVector);
        print(AttackDirecition + "방향");
        state = State.Delay;
        photonView.RPC("AttackToServer", RpcTarget.AllViaServer, AttackDirecition);
        return true;
        //var attackPos = newAttacker.transform.position + newAttacker.transform.forward * _distance;
        //EffectManager.Instance.EffectToServer(Define.EffectType.BodySlam, attackPos, 0);

        //Collider[] colliders = new Collider[10];

        //var hitCount = Physics.OverlapSphereNonAlloc(this.transform.position, _distance, colliders, _attackLayer);
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
        //state = State.Delay;
        //return true;
    }

    [PunRPC]
    public void AttackToServer(Vector3 targetPoint)
    {
        StartCoroutine(AttackProcessOnAllClinets(targetPoint));
    }

    IEnumerator AttackProcessOnAllClinets(Vector3 targetPoint)
    {
        state = State.Delay;
        AttackDirecition = targetPoint;
        yield return new WaitForSeconds(AttackDelay);   //대미지 주기전까지 시간
        AttackEffect();
        yield return new WaitForSeconds(AfaterAttackDelay);   //대미지 주기전까지 시간

        state = State.End;
    }

    void AttackEffect()
    {
        var attackPos = newAttacker.transform.position + newAttacker.transform.forward * _distance;
        print(attackPos + "어택이펙트");
        EffectManager.Instance.EffectToServer(Define.EffectType.BodySlam, attackPos, 0);

        Collider[] colliders = new Collider[10];

        var hitCount = Physics.OverlapSphereNonAlloc(this.transform.position, _distance, colliders, _attackLayer);
        if (hitCount > 0)
        {
            print(hitCount + "힛카운트");
            for (int i = 0; i < hitCount; i++)
            {

                print(colliders[i].gameObject.name);
                if (IsTargetOnSight(colliders[i].transform))
                {
                    print("시야안");
                }
                else
                {
                    print("시야박");
                }
                var damageable = colliders[i].gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.OnDamage(5, 5, colliders[i].transform.position);
                }
            }

        }
    }

    public override void Zoom(Vector2 inputVector)
    {
        if (inputVector.sqrMagnitude == 0)
        {
            _attackRangeUI.gameObject.SetActive(false);
            return;
        }
        //_attackRangeUI.position = newAttacker.CenterPivot.position;
        _attackRangeUI.rotation = UtillGame.WorldRotationByInput(inputVector);
        _attackRangeUI.gameObject.SetActive(true);

    }

    private bool IsTargetOnSight(Transform target)
    {
        RaycastHit hit;
        Vector3 startPoint = newAttacker.transform.position;
        Vector3 endPoint = target.transform.position;

        startPoint.y = 0.5f;
        endPoint.y = 0.5f;

        var direction = endPoint - startPoint;

        if (Vector3.Angle(direction, newAttacker.transform.forward) > 120 * 0.5f)
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
