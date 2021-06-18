using System.Collections;

using UnityEngine;
using Photon.Pun;
public class SeekerAttack : MonoBehaviourPun
{
    #region 임시데이터
   
    #endregion
    public enum state
    {
        Idle,
        Attack,
        Skill
    }

    state _state;

    [SerializeField] Transform _centerPivot;
    SeekerInput _seekerInput;
    Transform upperSpine;
    Animator _animator;
    [SerializeField] Weapon _weapon;
    [SerializeField] SeekrRader _seekrRader;

    int attackTargetLayer = (1 << (int)Define.Layer.Wall) | (1 << (int)Define.Layer.Item);
    private readonly float initSkillCoolTime = 10;
    private readonly int initAttackDamage = 1;


    public Transform CenterPivot => _centerPivot;
    public state State { get => _state; set { _state = value; } }
    public int AttackDamage { get; set; }


    public Vector3 upperBodyDir;
    public Vector3 tempDireciton;

    public Vector2 lastAttack;

    private void Awake()
    {
        _seekerInput = GetComponent<SeekerInput>();
    }
    public void OnPhotonInstantiate()
    {
        _animator = GetComponentInChildren<Animator>();
        upperSpine = _animator.GetBoneTransform(HumanBodyBones.Spine);

        SetupAttack(_weapon);
        AttackDamage = initAttackDamage;
        _state = state.Idle;

        if (this.IsMyCharacter())
        {

        }
    }


    public void SetupAttack(Weapon newWeapon)
    {
        _weapon = newWeapon;
        _weapon.transform.SetParent(_animator.GetBoneTransform(HumanBodyBones.RightHand));
        _weapon.transform.localPosition = Vector3.zero;
        _weapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
        _weapon.transform.localScale = Vector3.one;
        _weapon.SeutpSeekr(this);

    }


    public void Update()
    {
        UpdateZoom();
        UpdateAttack();

        CenterPivot.rotation = UtillGame.WorldRotationByInput(_seekerInput.AttackVector);

    }

    public void UpdateAttack()
    {
        if (_seekerInput.LastAttackVector != Vector2.zero)
        {
            if (State != state.Idle) return;
            State = state.Attack;
            lastAttack = _seekerInput.LastAttackVector;
            photonView.RPC("AttackToServer", RpcTarget.All, _seekerInput.LastAttackVector);
        }
    }

  
    void UpdateZoom()
    {
        _weapon.Zoom(_seekerInput.AttackVector);
    }

    #region Attack
    [PunRPC]
    public void AttackToServer(Vector2 targetPoint)
    {
        StartCoroutine(ProcessAttackOnClients(targetPoint));
    }

    IEnumerator ProcessAttackOnClients(Vector3 targetPoint)
    {
        State = state.Attack;
        upperBodyDir = UtillGame.ConventToVector3(targetPoint);
        upperBodyDir.y = 0;
        _animator.SetTrigger(_weapon.AttackAnim);
        yield return new WaitForSeconds(_weapon.AttackDelay);   //대미지 주기전까지 시간
        _weapon.Attack(targetPoint);    //대미지 줌
        yield return new WaitForSeconds(_weapon.AfaterAttackDelay); //끝날때까지 못움직임
        State = state.Idle;
    }

    #endregion



    private void LateUpdate()
    {
        //LateUpdateShooter();

    }

    public void LateUpdateShooter()
    {

        if (State == state.Attack)
        {
            if (upperBodyDir == Vector3.zero) return;
            Vector3 spineRot = Quaternion.LookRotation(upperBodyDir).eulerAngles;
            spineRot -= transform.eulerAngles;

            upperSpine.transform.localRotation = Quaternion.Euler(
                upperSpine.transform.localEulerAngles.x,
                upperSpine.transform.localEulerAngles.y - spineRot.y,
                upperSpine.transform.localEulerAngles.z
                );

        }

    }

}
