﻿using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Weapon_Hammer : Weapon
{

    float _damageRange = 1.2f;
    float _angle = 30;
    //int _damage = 10;
    AudioClip _attackClip;

    public override WeaponType weaponType => WeaponType.Hammer;
    public override HandType handType => HandType.Right;

    protected override void SetupCallBack()
    {
        inputControllerObject = this.gameObject.GetOrAddComponent<InputControllerObject>();
        inputControllerObject.inputType = InputType.Main;
        inputControllerObject.attackType =  Define.AttackType.Joystick;
        inputControllerObject.shooterState = PlayerShooter.state.Skill;
        inputControllerObject.AddZoomEvent(Zoom);
        inputControllerObject.AddUseEvent(Attack);
    }
    private void Start()
    {
        AttackAnim = "Attack";
        AttackDelay = 0.25f;
        AfaterAttackDelay = 0.25f;
        AttackDistance = 1;
        inputControllerObject.InitCoolTime = 1;
        _attackClip = Resources.Load<AudioClip>("Sounds/SMelee2");
         var arcZoom = _uI_ZoomBase as UI_ArcZoom;

        arcZoom?.SetupAngle(_damageRange, _angle);
    }
    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var weaponId = (string)info.photonView.InstantiationData[1];
        var weaponObject = Managers.Resource.Instantiate($"Hammer/{weaponId}");   //유저 무기 아바타 생성
        weaponObject.transform.ResetTransform(_weaponModel);   //아바타 생성된것 자식오브젝트로 이동
        renderController = weaponObject.GetComponent<RenderController>();
        base.OnPhotonInstantiate(info);
        //_zoomUI.gameObject.SetActive(false);     // 공격 UI
        //playerController.GetAttackBase().SetupWeapon(this);
        //attackPlayer.UseWeapon(this);    //무기 사용상태로 전환

    }

    public override void Zoom(Vector2 inputVector)
    {
        if (inputVector.sqrMagnitude == 0)
        {
            _uI_ZoomBase.SetActiveZoom(false);
            return;
        }
        //var endPoint = GetHitPoint(inputVector.normalized);
        //var startPoint = this.transform.position + Vector3.up * 0.5f;
        _uI_ZoomBase.UpdateZoom(Vector3.zero, inputVector);
        _uI_ZoomBase.SetActiveZoom(true);

    }
    #region Attack

    public override void Attack(Vector2 inputVector)
    {
        if (photonView.IsMine == false) return;
        var direction = playerController.playerCharacter.characterAvater.transform.forward;//아바타가보는 정면
        var attackPoint = playerController.transform.position + direction * AttackDistance;
        photonView.RPC("AttackOnServer", RpcTarget.All, attackPoint);
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
        attackStartCallBack?.Invoke();

        //inputControllerObject.attackDirection = direction;
        //inputControllerObject.Call_UseSucessStart();
        yield return new WaitForSeconds(AttackDelay);   //대미지 주기전까지 시간
        AttackEffect(attackPoint);
        yield return new WaitForSeconds(AfaterAttackDelay);   //움직이기 . 애니메이션의 끝나면
        attackEndCallBack?.Invoke();

        //inputControllerObject.Call_UseSucessEnd();
    }

    void AttackEffect(Vector3 attackPoint )
    {
        Managers.effectManager.EffectOnLocal(Define.EffectType.BodySlam, attackPoint, 0,_damageRange);
        UtillGame.DamageInRange(attackPoint, _damageRange, _damage, playerController.ViewID(), UtillLayer.seekerToHiderAttack);
        if(Managers.cameraManager.IsView(attackPoint) && playerController.IsMyCharacter())
        {
            Managers.cameraManager.ShakeCameraByPosition(attackPoint, 0.3f, 0.7f, 0.1f);
            Managers.Sound.Play(_attackClip, Define.Sound.Effect);
        }
    }

    #endregion

}
