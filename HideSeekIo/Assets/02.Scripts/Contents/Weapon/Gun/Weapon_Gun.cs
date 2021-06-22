using System.Collections;

using UnityEngine;

public class Weapon_Gun : Weapon
{
    [SerializeField] ParticleSystem _muzzleFalsh;
    [SerializeField] protected GameObject _bulletPrefab;
    [SerializeField] int _maxAmmo;
    [SerializeField] int _currentAmmo;
    [SerializeField] float _maxDistance;
    [SerializeField] float _attackTimeBet;
    [SerializeField] protected Transform _fireTransform1;
    [SerializeField] protected Transform _fireTransform2;

    [SerializeField] protected Transform _lineTransform;
    float _lastFireTime;
    LineRenderer _lineRenderer;
    int _zoomLayer = 1 << (int)Define.Layer.Wall;



    protected virtual void Awake()
    {
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.enabled = false;
        weaponType = WeaponType.Gun;
    }
    protected virtual void Start()
    {
        _attackAnimationName = "Attack";
        _attackDelayTime = 0.1f;
        _afterAttackDelayTime = 0.5f;
        _distance = 1.5f;
    }
    protected virtual void OnEnable()
    {
        _currentAmmo = _maxAmmo;
    }

    public override bool Attack(Vector2 inputVector)
    {
        _muzzleFalsh.Play();
        var go1 = Managers.Pool.Pop(_bulletPrefab).GetComponent<Bullet>();
        var go2 = Managers.Pool.Pop(_bulletPrefab).GetComponent<Bullet>();

        go1.transform.position = _fireTransform1.transform.position;
        go1.Setup(GetHitPoint(_fireTransform1, inputVector));
        go2.transform.position = _fireTransform2.transform.position;
        go2.Setup(GetHitPoint(_fireTransform2, inputVector));

        return true;
    }

    public override void Zoom(Vector2 inputVector)
    {
        var pos = UtillGame.ConventToVector3(inputVector);
        if (pos.sqrMagnitude == 0)
        {
            _lineRenderer.enabled = false;
            return;
        }
        _lineRenderer.SetPosition(0, _lineTransform.position);
        _lineRenderer.SetPosition(1, GetHitPoint(_lineTransform, inputVector));
        _lineRenderer.enabled = true;


    }

    Vector3 GetHitPoint(Transform rayTransform, Vector2 inputVector)
    {
        UICanvas.transform.rotation = UtillGame.GetWorldRotation_ByInputVector(inputVector);
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
