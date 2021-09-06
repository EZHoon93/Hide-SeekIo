using System.Collections;

using UnityEngine;
using Photon.Pun;
using PepijnWillekens.Extensions;

public class Weapon_Gun : Weapon, IZoom
{
   
    [SerializeField] float _maxDistance = 3;
    int _zoomLayer = 1 << (int)Define.Layer.Wall;
    protected override void Awake()
    {
        base.Awake();
        weaponType = WeaponType.Gun;
    }
    protected override void SetupCallBack()
    {
        inputControllerObject = this.gameObject.GetOrAddComponent<InputControllerObject>();
        inputControllerObject.inputType = InputType.Sub1;
        inputControllerObject.attackType = Define.AttackType.Joystick;
        inputControllerObject.shooterState = PlayerShooter.state.MoveAttack;
        inputControllerObject.AddUseEvent(Attack);
        inputControllerObject.AddZoomEvent(Zoom);
    }

 

    public override void Attack(Vector2 inputVector)
    {

    }
    public override void Zoom(Vector2 inputVector)
    {
        if (inputVector.sqrMagnitude == 0)
        {
            uiZoom.lineRenderer.enabled = false;
            return;
        }
        //var pos = UtillGame.ConventToVector3(inputVector);
        //uiZoom.transform.rotation = Quaternion.Euler( pos);
        var startPoint = uiZoom.lineRenderer.transform.position;
        var endPoint = GetHitPoint(uiZoom.lineRenderer.transform, inputVector);
        startPoint.y = 1.0f;
        endPoint .y= 1.0f;
        uiZoom.lineRenderer.SetPosition(0, startPoint);
        uiZoom.lineRenderer.SetPosition(1, endPoint);
        uiZoom.lineRenderer.enabled = true;
        uiZoom.gameObject.SetActive(true);
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
            print("Hit + " + hit.collider.name);
        }
        else
        {
            //hitPosition = rayTransform.transform.position + rayTransform.transform.forward * _maxDistance;
            hitPosition = rayTransform.transform.position + UtillGame.ConventToVector3( inputVector  )* _maxDistance;

        }
        return hitPosition;
    }

}
