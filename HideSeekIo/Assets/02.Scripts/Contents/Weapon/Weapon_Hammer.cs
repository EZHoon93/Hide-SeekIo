﻿using System.Collections;

using UnityEngine;

public class Weapon_Hammer : Weapon
{
    [SerializeField] Transform _attackRangeUI;
    [SerializeField] float _distance;
    [SerializeField] float _angle = 120;

    int _attackLayer = ( 1 << ( int)Define.Layer.Hider) | ( 1<<  (int)Define.Layer.Item ) ;
    private void Start()
    {
        _attackAnimationName = "Attack";
        _attackDelayTime = 0.7f;
        _afterAttackDelayTime = 0.5f;
        _distance = 1.5f;
        _attackRangeUI.gameObject.SetActive(false);     // 공격 UI
    }
    public override void Attack(Vector2 inputVector)
    {
        var attackPos = _seekerAttack.transform.position + _seekerAttack.transform.forward * _distance;
        EffectManager.Instance.EffectToServer(Define.EffectType.BodySlam, attackPos);

        Collider[] colliders = new Collider[10] ;

        var hitCount = Physics.OverlapSphereNonAlloc(this.transform.position,_distance,colliders ,_attackLayer);
        if(hitCount > 0)
        {
            print(hitCount + "힛카운트");
            for(int i = 0; i < hitCount; i++)
            {
                
                print(colliders[i].gameObject.name);
                if( IsTargetOnSight( colliders[i].transform ))
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
                    damageable.OnDamage(5, 5, colliders[i].transform.position );
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
        _attackRangeUI.position = _seekerAttack.CenterPivot.position;
        _attackRangeUI.rotation = UtillGame.WorldRotationByInput(inputVector);
        _attackRangeUI.gameObject.SetActive(true);

    }

    private bool IsTargetOnSight(Transform target)
    {
        RaycastHit hit;
        Vector3 startPoint = _seekerAttack.transform.position;
        Vector3 endPoint = target.transform.position;

        startPoint.y = 0.5f;
        endPoint.y = 0.5f;

        var direction = endPoint - startPoint;

        if (Vector3.Angle(direction, _seekerAttack.transform.forward) > 120 * 0.5f)
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
