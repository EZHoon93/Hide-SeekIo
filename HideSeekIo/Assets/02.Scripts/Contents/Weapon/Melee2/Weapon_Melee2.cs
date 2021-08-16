using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Weapon_Melee2 : Weapon
{
 

    AudioClip _attackClip;

    //[SerializeField] float _angle = 120;

    int _attackLayer = 1 << (int)Define.Layer.Hider ;

    public override Define.ZoomType zoomType { get; set; } = Define.ZoomType.Melee;

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
        InitCoolTime = 3;
        _attackClip = Resources.Load<AudioClip>("Sounds/SMelee2");
    }
    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var weaponId = (string)info.photonView.InstantiationData[1];
        inputType = InputType.Main;
        var weaponObject = Managers.Resource.Instantiate($"Melee2/{weaponId}");   //유저 무기 아바타 생성
        weaponObject.transform.ResetTransform(_weaponModel);   //아바타 생성된것 자식오브젝트로 이동
        base.OnPhotonInstantiate(info);
        _zoomUI.gameObject.SetActive(false);     // 공격 UI
        playerController.GetAttackBase().SetupWeapon(this);
        //attackPlayer.UseWeapon(this);    //무기 사용상태로 전환

    }
    public override void Zoom(Vector2 inputVector)
    {
        if(playerController.gameObject.IsValidAI()== false)
        {
            if (inputVector.sqrMagnitude == 0)
            {
                _zoomUI.gameObject.SetActive(false);
                return;
            }
            if (_zoomUI == null)
            {
                CreateZoomUI(playerController);
            }
            _zoomUI.currentZoom.transform.rotation = UtillGame.WorldRotationByInput(inputVector);
            _zoomUI.gameObject.SetActive(true);
        }
     
        useState = UseState.Use;
    }


    #region Attack

    public override void Attack(Vector2 inputVector)
    {
        _zoomUI.gameObject.SetActive(false);
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
        yield return new WaitForSeconds(AttackDelay);   //대미지 주기전까지 시간
        AttackEffect();
        yield return new WaitForSeconds(AfaterAttackDelay);   //대미지 주기전까지 시간
        AttackEndEvent?.Invoke();
        state = State.End;
    }

    void AttackEffect()
    {
        var attackPos = playerController.transform.position + playerController.transform.forward * AttackDistance * 0.5f;
        EffectManager.Instance.EffectToServer(Define.EffectType.BodySlam, attackPos, 0);
        UtillGame.DamageInRange(playerController.transform, AttackDistance, 10, playerController.ViewID(), UtillLayer.seekerToHiderAttack, 110);
        Managers.Sound.Play(_attackClip, Define.Sound.Effect);
    }

    #endregion


}
