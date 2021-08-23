using UnityEngine;
using Photon.Pun;
using System.Collections;
using System;

[RequireComponent(typeof(ObtainableItem))]
public abstract class Weapon_Throw : Weapon 
{
    [SerializeField] GameObject _projectilePrefab;
    public ObtainableItem obtainableItem { get; private set; }
    public float attackRange { get; protected set; }
    public Action destroyCallBackEvent { get; set; }
    public Define.ThrowItem throwType { get; set; }
    //public override Define.ZoomType zoomType { get; set; } = Define.ZoomType.Throw;


    protected override void Awake()
    {
        base.Awake();
        weaponType = WeaponType.Throw;
        inputType = InputType.Item1;
        obtainableItem = GetComponent<ObtainableItem>();
    }

    public override void OnPreNetDestroy(PhotonView rootView)
    {
        base.OnPreNetDestroy(rootView);
        //playerController.GetAttackBase().RemoveItem(this);
    }

    
    public void Setup(string animName, float delayTime, float afaterDelayTime, float distance, float newAttackRange)
    {
        AttackAnim = animName;
        AttackDelay = delayTime;
        AfaterAttackDelay = afaterDelayTime;
        AttackDistance = distance;
        attackRange = newAttackRange;

        //UICanvas.transform.localScale = new Vector3(attackRange, attackRange, attackRange); //범위에 따른 ui변경
    }

    public override void Zoom(Vector2 inputVector)
    {
        //var state = UtillGame.ThrowZoom(inputVector, AttackDistance, playerController.GetAttackBase().CenterPivot, _zoomUI.currentZoom);
        ////UICanvas.transform.position = playerController.transform.position;
        //if (state)
        //{
        //    useState = UseState.Use;
        //}
        //if(playerController.gameObject.IsValidAI() == false) 
        //{
        //    if (inputVector.sqrMagnitude == 0)
        //    {
        //        print(this.gameObject.name);
        //        _zoomUI.gameObject.SetActive(false);
        //        useState = UseState.NoUse;
        //        return;
        //    }
        //    _zoomUI.FixedUI();
        //    _zoomUI.currentZoom.transform.position = UtillGame.GetThrowPosion(inputVector, AttackDistance, playerController.GetAttackBase().CenterPivot);
        //    _zoomUI.gameObject.SetActive(true);
        //}

    
        //useState = UseState.Use;
    }

    private void LateUpdate()
    {
        //UICanvas.transform.position = playerController.transform.position;
    }


 #region Attack
    public override void Attack(Vector2 inputVector)
    {
        if (playerController.IsMyCharacter())
        {
            //_zoomUI.gameObject.SetActive(false);
        }
        state = State.Delay;
        Vector3 endPoint = UtillGame.GetThrowPosion(inputVector, AttackDistance, playerController.transform);
        LastAttackInput = inputVector;
        photonView.RPC("AttackOnServer", RpcTarget.AllViaServer, inputVector, endPoint);
    }

    [PunRPC]
    public void AttackOnServer(Vector2 inputVector, Vector3 endPoint)
    {
        obtainableItem.removeCallBack?.Invoke();
        LastAttackInput = inputVector;
        Vector3 startPoint = playerController.GetAttackBase().CenterPivot.position;
        StartCoroutine(AttackProcessOnAllClinets(startPoint, endPoint));
    }

    IEnumerator AttackProcessOnAllClinets(Vector3 startPoint, Vector3 endPoint)
    {
        state = State.Delay;
        //AttackSucessEvent?.Invoke(this);
        yield return new WaitForSeconds(AttackDelay);   //대미지 주기전까지 시간
        var projectile = Managers.Pool.Pop(_projectilePrefab).GetComponent<ThrowProjectileObject>();
        projectile.Play(playerController.GetAttackBase(),  startPoint, endPoint);
        yield return new WaitForSeconds(AfaterAttackDelay);   //대미지 주기전까지 시간
        AttackEndEvent?.Invoke();

        state = State.End;
        Use(playerController);
    }


    #endregion

    public virtual void Use(PlayerController usePlayerController)
    {
        if(inputType ==  InputType.Item1)
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }

    public abstract Enum GetEnum();

}
