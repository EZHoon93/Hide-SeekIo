using System.Collections;

using UnityEngine;
using Photon.Pun;
public class Weapon_Gun : Weapon
{
    //[SerializeField] ParticleSystem _muzzleFalsh;
    //[SerializeField] protected GameObject _bulletPrefab;
    //[SerializeField] int _maxAmmo;
    //[SerializeField] int _currentAmmo;
    [SerializeField] float _maxDistance;
    //[SerializeField] float _attackTimeBet;
    //[SerializeField] protected Transform _fireTransform;

    //[SerializeField] protected Transform _lineTransform;
    //float _lastFireTime;
    //LineRenderer _lineRenderer;
    int _zoomLayer = 1 << (int)Define.Layer.Wall;

    ////public override Define.ZoomType zoomType { get; set; } = Define.ZoomType.Gun;


    ////protected virtual void Awake()
    ////{
    ////    _lineRenderer = GetComponentInChildren<LineRenderer>();
    ////    _lineRenderer.positionCount = 2;
    ////    _lineRenderer.enabled = false;
    ////    weaponType = WeaponType.Gun;
    ////}/
    //protected virtual void Start()
    //{
    //    //AnimationN = "Attack";
    //    //_attackDelayTime = 0.1f;
    //    //_afterAttackDelayTime = 0.5f;
    //    //_distance = 1.5f;
    //}
    //protected virtual void OnEnable()
    //{
    //    _currentAmmo = _maxAmmo;
    //}
    protected override void Awake()
    {
        base.Awake();
        weaponType = WeaponType.Gun;
    }
    protected override void SetupCallBack()
    {
        inputControllerObject = this.gameObject.GetOrAddComponent<InputControllerObject>();
        //inputControllerObject = inputControllerObject ?? new InputControllerObject();
        inputControllerObject.inputType = InputType.Sub1;
        inputControllerObject.attackType = Define.AttackType.Joystick;
        inputControllerObject.shooterState = PlayerShooter.state.MoveAttack;
        inputControllerObject.AddUseEvent(Attack);
        inputControllerObject.AddZoomEvent(Zoom);

    }

    public void Setup(string animName, float delayTime, float afaterDelayTime, float distance, float newAttackRange)
    {
        AttackAnim = animName;
        AttackDelay = delayTime;
        AfaterAttackDelay = afaterDelayTime;
        AttackDistance = distance;
        //UICanvas.transform.localScale = new Vector3(attackRange, attackRange, attackRange); //범위에 따른 ui변경
    }

    public override void Attack(Vector2 inputVector)
    {

    }

    public override void Zoom(Vector2 inputVector)
    {
        var pos = UtillGame.ConventToVector3(inputVector);
        if (pos.sqrMagnitude == 0)
        {
            uiZoom.lineRenderer.enabled = false;
        }
        uiZoom.lineRenderer.SetPosition(0, uiZoom.lineRenderer.transform.position);
        uiZoom.lineRenderer.SetPosition(1, GetHitPoint(uiZoom.lineRenderer.transform, inputVector));
        uiZoom.lineRenderer.enabled = true;
    }
    //public override void Attack(Vector2 inputVector)
    //{
    //    state = State.Delay;
    //    LastAttackInput = inputVector;
    //    //Vector3 startPoint = _fireTransform.transform.position;
    //    Vector3 endPoint = GetHitPoint(_fireTransform, inputVector);

    //    photonView.RPC("AttackOnServer", RpcTarget.AllViaServer, inputVector, endPoint);
    //}
    //[PunRPC]
    //public void AttackOnServer(Vector2 inputVector, Vector3 endPoint)
    //{
    //    state = State.Delay;
    //    LastAttackInput = inputVector;
    //    _muzzleFalsh.Play();
    //    StartCoroutine(AttackProcessOnAllClinets(endPoint));
    //}
    //IEnumerator AttackProcessOnAllClinets(Vector3 endPoint)
    //{
    //    state = State.Delay;
    //    //AttackSucessEvent?.Invoke();
    //    yield return new WaitForSeconds(AttackDelay);   //대미지 주기전까지 시간
    //    var go1 = Managers.Pool.Pop(_bulletPrefab).GetComponent<Bullet>();
    //    go1.transform.position = _fireTransform.transform.position;
    //    go1.Setup(endPoint);
    //    yield return new WaitForSeconds(AfaterAttackDelay);   //대미지 주기전까지 시간
    //    AttackEndEvent?.Invoke();
    //    state = State.End;
    //}
    //public override void Zoom(Vector2 inputVector)
    //{
    //    var pos = UtillGame.ConventToVector3(inputVector);
    //    if (pos.sqrMagnitude == 0)
    //    {
    //        _lineRenderer.enabled = false;
    //    }
    //    _lineRenderer.SetPosition(0, _lineTransform.position);
    //    _lineRenderer.SetPosition(1, GetHitPoint(_lineTransform, inputVector));
    //    _lineRenderer.enabled = true;

    //}

    Vector3 GetHitPoint(Transform rayTransform, Vector2 inputVector)
    {
        //UICanvas.transform.rotation = UtillGame.GetWorldRotation_ByInputVector(inputVector);
        RaycastHit hit;
        Vector3 hitPosition;
        if (Physics.Raycast(rayTransform.transform.position, rayTransform.transform.forward, out hit, _maxDistance, _zoomLayer))
        {
            hitPosition = hit.point;
            hitPosition.y = rayTransform.transform.position.y;
        }
        else
        {
            hitPosition = rayTransform.transform.position + rayTransform.transform.forward * _maxDistance;
        }
        return hitPosition;
    }

}
