using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

using UnityEngine;

public class Skill_Jump : Skill_Base
{
    public override Define.AttackType attakType => Define.AttackType.Joystick;
    [SerializeField] float _jumpDistance = 3;
    Plane _plane;
    Vector3 _targetPoint;


    protected override void SetupData()
    {
        inputControllerObject.InitCoolTime = 3;
    }

    public override void OnPhotonInstantiate(PlayerController newPlayerController)
    {
        base.OnPhotonInstantiate(newPlayerController);
        _plane = new Plane(Vector3.up  , newPlayerController.transform.position);
    }


    public override void Use(Vector2 inputVector2)
    {
        var hitPoint =  GetSkillAttackPoint(inputVector2);
        var targetPoint =  UtillGame.GetPointOnNavMeshLoop(hitPoint);
        var distance = Vector3.Distance(this.transform.position, targetPoint);
        var time = Mathf.Clamp(distance * 0.5f, 0.5f, 1.5f);

        playerController.transform.DOJump(targetPoint, distance, 1,time);
        var direction = targetPoint - playerController.transform.position;
        direction = direction.normalized;
        direction.y = this.transform.position.y;
        Quaternion newRotation = Quaternion.LookRotation(direction);
        playerController.playerCharacter.characterAvater.transform.rotation = newRotation;
        _targetPoint = targetPoint;
        StartCoroutine(ProceeJump(targetPoint,time));
    }

    IEnumerator ProceeJump(Vector3 targetPoint, float time)
    {
     

        playerController.playerCharacter.animator.CrossFade("JumpStart", 0.0f);
        playerController.playerCharacter.animator.SetTrigger("JumpStart");

        yield return new WaitForSeconds(time - 0.25f );
        //playerController.playerCharacter.animator.SetTrigger("JumpEnd");
        playerController.playerCharacter.animator.CrossFade("JumpEnd", 0.0f);
        playerController.playerCharacter.animator.SetTrigger("JumpEnd");

        yield return new WaitForSeconds(0.25f);
        EffectManager.Instance.EffectOnLocal(Define.EffectType.BodySlam, targetPoint, 0, 1);

    }


    public override void Zoom(Vector2 inputVector2)
    {
        if(inputVector2.sqrMagnitude == 0)
        {
            _uI_ZoomBase.SetActiveZoom(false);
            return;
        }
        _targetPoint = GetSkillAttackPoint(inputVector2);
        var startPoint = this.transform.position + Vector3.up * 0.5f;
        _uI_ZoomBase.UpdateZoom(startPoint, _targetPoint);
        _uI_ZoomBase.SetActiveZoom(true);

    }


    Vector3 GetSkillAttackPoint(Vector2 inputVector2)
    {
        var startPoint = this.transform.position;
        var endPoint = this.transform.position+ new Vector3(inputVector2.x, 0, inputVector2.y) * _jumpDistance;

        var ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(endPoint));
        Vector3 targetPoint = Vector3.zero;
        float hitDist;

        if (_plane.Raycast(ray, out hitDist))
        {
            targetPoint = ray.GetPoint(hitDist);
            RaycastHit hitInfo;
            if (Physics.Linecast(startPoint, targetPoint, out hitInfo, 1 << (int)Define.Layer.Ground))
            {
                targetPoint = hitInfo.point;
            }
        }
        else
        {
            targetPoint = endPoint;
        }
        return targetPoint;
    }
 
}
