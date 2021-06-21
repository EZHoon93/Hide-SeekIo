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
    [SerializeField] protected Transform _fireTransform;
    [SerializeField] protected Transform _lineTransform;
    float _lastFireTime;
    LineRenderer _lineRenderer;
    int _zoomLayer = (int)Define.Layer.Wall;



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

    public override void Attack(Vector2 inputVector)
    {
        var pos = UtillGame.ConventToVector3(inputVector);
        var quaternion = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
        var newDirection = quaternion * pos;
        Quaternion newRotation = Quaternion.LookRotation(pos);
        UICanvas.transform.rotation = newRotation;

        RaycastHit hit;
        Vector3 hitPosition;
        _lineRenderer.SetPosition(0, _fireTransform.position);
        hitPosition = _fireTransform.transform.position + _fireTransform.transform.forward * _maxDistance;

        _muzzleFalsh.Play();
        var go = Managers.Pool.Pop(_bulletPrefab).GetComponent<Bullet>();
        go.transform.position = _fireTransform.transform.position;
        go.Setup(hitPosition);
        

    }

    public override void Zoom(Vector2 inputVector)
    {
        var pos = UtillGame.ConventToVector3(inputVector);
        if (pos.sqrMagnitude == 0)
        {
            _lineRenderer.enabled = false;
            return;
        }
        var quaternion = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
        var newDirection = quaternion * pos;
        Quaternion newRotation = Quaternion.LookRotation(pos);
        //_fireTransform.transform.rotation = Quaternion.Slerp(this.transform.localRotation, newRotation, 33 * Time.deltaTime);    //즉시변환
        UICanvas.transform.rotation = newRotation;


        _lineRenderer.SetPosition(0, _lineTransform.position);
        RaycastHit hit;
        Vector3 hitPosition;
        _lineRenderer.SetPosition(0, _lineTransform.position);

        if (Physics.Raycast(_lineTransform.transform.position, _lineTransform.transform.forward, out hit, _maxDistance, _zoomLayer))
        {
            hitPosition = hit.point;
            hitPosition.y = _lineTransform.transform.position.y;
        }
        else
        {
            hitPosition = _lineTransform.transform.position + _lineTransform.transform.forward * _maxDistance;
        }
        hitPosition = _lineTransform.transform.position + _lineTransform.transform.forward * _maxDistance;

        _lineRenderer.SetPosition(1, hitPosition);
        _lineRenderer.enabled = true;


    }


}
