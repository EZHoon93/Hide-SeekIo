
using UnityEngine;
using Photon.Pun;
using System.Collections;
public class Weapon_Stone : Weapon, IZoom
{

    [SerializeField] float _maxDistance = 3;
    int _zoomLayer = 1 << (int)Define.Layer.Wall;
    GameObject _bullet;
    protected override void Awake()
    {
        base.Awake();
        weaponType = WeaponType.Gun;
        AttackAnim = "OneThrow";
        AttackDelay = 0.2f;
        AfaterAttackDelay = 0.2f;
        _bullet = Managers.Resource.Load<GameObject>("Prefabs/Projectile/P_Stone");
    }
    protected override void SetupCallBack()
    {
        inputControllerObject = this.gameObject.GetOrAddComponent<InputControllerObject>();
        inputControllerObject.inputType = InputType.Sub1;
        inputControllerObject.attackType = Define.AttackType.Joystick;
        inputControllerObject.shooterState = PlayerShooter.state.MoveAttack;
        inputControllerObject.InitCoolTime = 3;
        inputControllerObject.AddUseEvent(Attack);
        inputControllerObject.AddZoomEvent(Zoom);
    }



    public override void Attack(Vector2 inputVector)
    {
        var endPoint = GetHitPoint(playerController.transform, inputVector);
        photonView.RPC("AttackOnServer", RpcTarget.AllViaServer, inputVector, endPoint);

    }

    [PunRPC]
    public void AttackOnServer(Vector2 inputVector, Vector3 endPoint)
    {
        //LastAttackInput = inputVector;
        var direction = endPoint - playerController.transform.position;
        direction = direction.normalized;
        direction.y = playerController.transform.position.y;
        inputControllerObject.attackDirection = direction;
        StartCoroutine(AttackProcessOnAllClinets(endPoint));
    }

    public override void Zoom(Vector2 inputVector)
    {
        if (inputVector.sqrMagnitude == 0)
        {
            uiZoom.lineRenderer.enabled = false;
            return;
        }
        var startPoint = uiZoom.lineRenderer.transform.position + UtillGame.ConventToVector3( inputVector).normalized*0.3f;
        var endPoint = GetHitPoint(uiZoom.lineRenderer.transform, inputVector);
        startPoint.y = .5f;

        uiZoom.lineRenderer.SetPosition(0, startPoint);
        uiZoom.lineRenderer.SetPosition(1, endPoint);
        uiZoom.lineRenderer.enabled = true;
        uiZoom.gameObject.SetActive(true);
    }
    IEnumerator AttackProcessOnAllClinets(Vector3 endPoint)
    {
        inputControllerObject.Call_UseSucessStart();
        yield return new WaitForSeconds(AttackDelay);   //대미지 주기전까지 시간
        var bullet =  Managers.Pool.Pop(_bullet.gameObject).GetComponent<StoneProjectile>();
        Vector3 startPoint = playerController.playerCharacter.animator.GetBoneTransform(HumanBodyBones.RightHand).position;
        bullet.transform.position = startPoint;
        bullet.Play(endPoint);
        yield return new WaitForSeconds(AfaterAttackDelay);   //대미지 주기전까지 시간
        inputControllerObject.Call_UseSucessEnd();
    }

    Vector3 GetHitPoint(Transform rayTransform, Vector2 inputVector)
    {
        RaycastHit hit;
        Vector3 hitPosition;
        Vector3 start = rayTransform.transform.position;
        start.y = 0.5f;
        Vector3 direction = UtillGame.ConventToVector3(inputVector);
        if (Physics.Raycast(start, direction, out hit, _maxDistance, _zoomLayer))
        {
            hitPosition = hit.point;
            hitPosition.y = rayTransform.transform.position.y;
        }
        else
        {
            hitPosition = start +direction * _maxDistance;

        }
        hitPosition.y = 0.5f;
        return hitPosition;
    }

}
