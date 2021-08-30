using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Weapon_Hammer : Weapon
{


    AudioClip _attackClip;

    protected override void Awake()
    {
        base.Awake();
        weaponType = WeaponType.Hammer;
        type = UseType.Permanent;
    }

    protected override void SetupCallBack()
    {
        inputControllerObject = this.gameObject.GetOrAddComponent<InputControllerObject>();
        //inputControllerObject = inputControllerObject ?? new InputControllerObject();
        inputControllerObject.inputType = InputType.Sub1;
        inputControllerObject.attackType =  Define.AttackType.Button;
        inputControllerObject.shooterState = PlayerShooter.state.NoMove;
        inputControllerObject.AddUseEvent(Attack);
    }
    private void Start()
    {
        AttackAnim = "HammerAttack";
        AttackDelay = 1.5f;
        AfaterAttackDelay = 0.5f;
        AttackDistance = 2;
        inputControllerObject.InitCoolTime = 3;
        _attackClip = Resources.Load<AudioClip>("Sounds/SMelee2");
    }
    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var weaponId = (string)info.photonView.InstantiationData[1];
        var weaponObject = Managers.Resource.Instantiate($"Melee2/{weaponId}");   //유저 무기 아바타 생성
        weaponObject.transform.ResetTransform(_weaponModel);   //아바타 생성된것 자식오브젝트로 이동
        base.OnPhotonInstantiate(info);
        //_zoomUI.gameObject.SetActive(false);     // 공격 UI
        //playerController.GetAttackBase().SetupWeapon(this);
        //attackPlayer.UseWeapon(this);    //무기 사용상태로 전환

    }


    #region Attack

    public override void Attack(Vector2 inputVector)
    {
        //StopAllCoroutines();
        ////_zoomUI.gameObject.SetActive(false);
        //state = State.Delay;
        //LastAttackInput = inputVector;
        photonView.RPC("AttackOnServer", RpcTarget.AllViaServer, LastAttackInput);
    }

    [PunRPC]
    public void AttackOnServer(Vector2 inputVector)
    {
        StartCoroutine(AttackProcessOnAllClinets(inputVector));
    }

    //Vector3 GetJumpPont()
    //{
    //    var hitPoint = Vector3.zero;
    //    RaycastHit hit;
    //    print(time);
    //    if (Physics.Raycast(playerController.transform.position, playerController.transform.forward, out hit, 1, (int)Define.Layer.Wall))
    //    {
    //        hitPoint = hit.point;
    //    }
    //    else
    //    {
    //        hitPoint = playerController.transform.position + playerController.transform.forward * 1;
    //    }
    //    return hitPoint;
    //}
    IEnumerator AttackProcessOnAllClinets(Vector2 inputVector)
    {
        state = State.Delay;
        LastAttackInput = inputVector;
        inputControllerObject.Call_UseSucessStart();
        yield return new WaitForSeconds(0.5f);   //대미지 주기전까지 시간
        AttackEffect();
        yield return new WaitForSeconds(AfaterAttackDelay);   //움직이기 . 애니메이션의 끝나면
        inputControllerObject.Call_UseSucessEnd();
        state = State.End;
    }

    void AttackEffect()
    {
        var attackPos = playerController.transform.position + playerController.character_Base.characterAvater.transform.forward * AttackDistance * 0.5f;
        EffectManager.Instance.EffectToServer(Define.EffectType.BodySlam, attackPos, 0);
        UtillGame.DamageInRange(playerController.transform, AttackDistance, 10, playerController.ViewID(), UtillLayer.seekerToHiderAttack, 110);
        Managers.Sound.Play(_attackClip, Define.Sound.Effect);
    }

    #endregion

}
