using System.Collections;

using ExitGames.Client.Photon.StructWrapping;

using Photon.Pun;
using UnityEngine;

public class Weapon_Melee2 : Weapon
{
 

    [SerializeField] Transform _modelTransform;
    [SerializeField] Transform _attackRangeUI;
    [SerializeField] Transform _attackWarningRangeUI;   //공격시 범위표시

    //[SerializeField] float _angle = 120;

    int _attackLayer = (1 << (int)Define.Layer.Hider) | (1 << (int)Define.Layer.Item);

    protected void Awake()
    {
        weaponType = WeaponType.Melee;
    }
    private void Start()
    {
        _attackAnimationName = "Attack";
        _attackDelayTime = 0.7f;
        _afterAttackDelayTime = 0.5f;
        _distance = 1.5f;
    }
    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);
        var weaponId = (string)info.photonView.InstantiationData[1];
        var weaponSkin = Managers.Resource.Instantiate($"Melee2/{weaponId}");
        weaponSkin.transform.ResetTransform(_modelTransform);
        _attackRangeUI.gameObject.SetActive(false);     // 공격 UI
        _attackWarningRangeUI.gameObject.SetActive(false);

    }
    public override void Zoom(Vector2 inputVector)
    {
        if (inputVector.sqrMagnitude == 0)
        {
            //_attackRangeUI.gameObject.SetActive(false);
            return;
        }
        _attackRangeUI.rotation = UtillGame.WorldRotationByInput(inputVector);

        _attackWarningRangeUI.rotation = UtillGame.WorldRotationByInput(inputVector);

        _attackRangeUI.gameObject.SetActive(true);
        print(UtillGame.WorldRotationByInput(inputVector) + "22줌인풋");

    }


    #region Attack

    public override void Attack(Vector2 inputVector)
    {
        state = State.Delay;
        LastAttackInput = inputVector;
        print(UtillGame.WorldRotationByInput(inputVector) + "어택인풋");

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
        AttackSucessEvent?.Invoke();
        _attackRangeUI.rotation = UtillGame.WorldRotationByInput(inputVector);
        print(UtillGame.WorldRotationByInput(inputVector) + "어택33인풋");

        _attackRangeUI.gameObject.SetActive(true);

        _attackWarningRangeUI.rotation = UtillGame.WorldRotationByInput(inputVector);
        _attackWarningRangeUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(AttackDelay);   //대미지 주기전까지 시간
        AttackEffect();
        _attackWarningRangeUI.gameObject.SetActive(false);

        yield return new WaitForSeconds(AfaterAttackDelay);   //대미지 주기전까지 시간
        AttackEndEvent?.Invoke();
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

    #endregion

  
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
