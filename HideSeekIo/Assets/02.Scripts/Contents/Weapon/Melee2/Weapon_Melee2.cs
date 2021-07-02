﻿using System.Collections;

using ExitGames.Client.Photon.StructWrapping;

using Photon.Pun;
using UnityEngine;

public class Weapon_Melee2 : Weapon
{
 

    [SerializeField] Transform _modelTransform;
    [SerializeField] Transform _attackRangeUI;

    //[SerializeField] float _angle = 120;

    int _attackLayer = (1 << (int)Define.Layer.Hider) | (1 << (int)Define.Layer.Item);

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
        AttackDistance= 1.5f;
    }
    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);
        var weaponId = (string)info.photonView.InstantiationData[1];
        var weaponSkin = Managers.Resource.Instantiate($"Melee2/{weaponId}");   //유저 무기 아바타 생성
        weaponSkin.transform.ResetTransform(_modelTransform);   //아바타 생성된것 자식오브젝트로 이동
        _attackRangeUI.gameObject.SetActive(false);     // 공격 UI
        newAttacker.UseWeapon(this);    //무기 사용상태로 전환

    }
    public override void Zoom(Vector2 inputVector)
    {
        if (inputVector.sqrMagnitude == 0)
        {
            _attackRangeUI.gameObject.SetActive(false);
            return;
        }
        _attackRangeUI.rotation = UtillGame.WorldRotationByInput(inputVector);
        _attackRangeUI.gameObject.SetActive(true);
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
        AttackSucessEvent?.Invoke();
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
        var attackPos = newAttacker.transform.position + newAttacker.transform.forward * AttackDistance;
        print(attackPos + "어택이펙트");
        EffectManager.Instance.EffectToServer(Define.EffectType.BodySlam, attackPos, 0);

        Collider[] colliders = new Collider[10];

        var hitCount = Physics.OverlapSphereNonAlloc(this.transform.position, AttackDistance, colliders, _attackLayer);
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
