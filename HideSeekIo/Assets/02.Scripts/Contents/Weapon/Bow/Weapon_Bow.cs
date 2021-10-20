using System.Collections;

using UnityEngine;
using Photon.Pun;

public class Weapon_Bow : Weapon
{

    public override WeaponType weaponType => WeaponType.Bow;
    public override HandType handType => HandType.Left;

    [SerializeField] Transform _fireParentPivot;
    [SerializeField] Transform _fireTransform;
    [SerializeField] AudioClip _attackClip;

    [SerializeField] protected float _amount = 1;
    [SerializeField] protected BulletProjectile _projectilePrefab;

    Animator _animator;
    protected override void SetupCallBack()
    {
        _animator = GetComponent<Animator>();
        inputControllerObject = this.gameObject.GetOrAddComponent<InputControllerObject>();
        inputControllerObject.inputType = InputType.Main;
        inputControllerObject.attackType = Define.AttackType.Joystick;
        inputControllerObject.shooterState = PlayerShooter.state.MoveAttack;
        inputControllerObject.AddZoomEvent(Zoom);
        inputControllerObject.AddUseEvent(Attack);

    }



    public override void Zoom(Vector2 inputVector)
    {
        if (inputVector.sqrMagnitude == 0)
        {
            _uI_ZoomBase.SetActiveZoom(false);
            return;
        }
        var endPoint = GetHitPoint(inputVector.normalized);
        var startPoint = _fireTransform.transform.position;
        _uI_ZoomBase.UpdateZoom(startPoint, endPoint);
        _uI_ZoomBase.SetActiveZoom(true);
    }

    #region Attack

    public override void Attack(Vector2 inputVector)
    {
        var endPoint = GetHitPoint(inputVector.normalized);
        var startPoint = _fireTransform.transform.position;
        photonView.RPC("AttackOnServer", RpcTarget.All,  startPoint,endPoint);
    }

    [PunRPC]
    public void AttackOnServer(Vector3 startPoint, Vector3 endPoint)
    {
        attackPoint = endPoint;
        StartCoroutine(AttackProcessOnAllClinets(startPoint, endPoint));
    }

    IEnumerator AttackProcessOnAllClinets(Vector3 startPoint ,Vector3  endPoint)
    {
        attackStartCallBack?.Invoke();
        _animator.SetTrigger("Attack");
        yield return new WaitForSeconds(AttackDelay);   //대미지 주기전까지 시간
        var bulletProjectile = Managers.Pool.Pop(_projectilePrefab.gameObject).GetComponent<BulletProjectile>();
        bulletProjectile.Play(playerController.ViewID(), _fireTransform.transform.position, endPoint);
        yield return new WaitForSeconds(AfaterAttackDelay);   //대미지 주기전까지 시간
        attackEndCallBack?.Invoke();
    }

    #endregion
    protected Vector3 GetHitPoint(Vector2 inputVector)
    {
        RaycastHit hit;
        Vector3 hitPosition;
        _fireParentPivot.transform.localRotation = Quaternion.LookRotation(UtillGame.ConventToVector3(inputVector));
        Vector3 start = _fireTransform.transform.position;
        Vector3 direction = _fireTransform.transform.forward;

        if (Physics.Raycast(start, direction, out hit, AttackDistance, 1 << (int)Define.Layer.Wall))
        {
            hitPosition = hit.point;
        }
        else
        {
            hitPosition = start + direction * AttackDistance;
        }
        hitPosition.y = _fireTransform.transform.position.y;

        return hitPosition;
    }
}
