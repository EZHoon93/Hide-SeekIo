using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Buff_Stealth : BuffBase 
{
    float _timeBet = 0.1f;
    float _currentRemainTime;
    List<LivingEntity> _seeList = new List<LivingEntity>(16);
    public LayerMask layerMask;
    Vector3 start;
    Vector3 dir;
    Vector3 hitPoint;
    [SerializeField] ColliderChildren _colliderObject;

    private void Awake()
    {
        _colliderObject.triggerStayCallBack += OnListen_TriggerStayCallBack;
    }

    public override void ProcessStart()
    {
        _colliderObject.gameObject.SetActive(true);
        ChangeLayer(_buffController.livingEntity.Team);
        _buffController.livingEntity.fogController.ChangeTransParentBySkill(true);
    }
    public override void ProcessEnd()
    {
        _colliderObject.gameObject.SetActive(false);
        _buffController.livingEntity.fogController.ChangeTransParentBySkill(false);
    }

    public void OnListen_TriggerStayCallBack(Collider other)
    {
        if (!UpdateCheckTime()) return;
        //트리거 검출 줄이기
        if (CheckInRange(other.transform))
        {
            var buffPalyer = _buffController.livingEntity.GetComponent<PlayerController>();
            if (buffPalyer)
            {
                buffPalyer.playerUI.ChangeWarining(true);
            }

            var otherPlayer = other.GetComponent<PlayerController>();
            if (otherPlayer)
            {
                otherPlayer.playerUI.ChangeWarining(true);
            }
            _buffController.End();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="team"> 버프사용하는 유저의소 </param>
    void ChangeLayer(Define.Team team)
    {
        if(team == Define.Team.Hide)
        {
            _colliderObject.gameObject.layer = (int)Define.Layer.HiderCollider;
        }
        else
        {
            _colliderObject.gameObject.layer = (int)Define.Layer.SeekerCollider;
        }
    }

    bool UpdateCheckTime()
    {
        if (_currentRemainTime < 0)
        {
            _currentRemainTime = _timeBet;
            return true;
        }
        else
        {
            _currentRemainTime -= Time.deltaTime;
            return false;
        }
    }

    /// <summary>
    /// 범위안에 있으면 호출, 벽 사이엤으면 호출 X
    /// </summary>
    bool CheckInRange(Transform target)
    {
        var s = target.GetComponent<PlayerController>();
        RaycastHit raycastHit;
        var startPoint = _buffController.livingEntity.transform.position;
        var direction =  target.transform.position - _buffController.livingEntity.transform.position;
        startPoint.y = 0.2f;
        direction.y = 0.2f;
        start = startPoint;
        dir = direction;
        var maxDistance = Vector3.Distance(startPoint, target.transform.position);
        if (Physics.Raycast(startPoint,direction, out raycastHit ,maxDistance, layerMask))
        {
            //direction= raycastHit.collider.transform.position;
            //end.y = 0.5f;
            hitPoint = raycastHit.point;
            hitPoint.y = 0.2f;
            return false;
        }
        hitPoint = startPoint + maxDistance * direction;
        hitPoint.y = 0.2f;

        return true;
    }
    //private void Update()
    //{
    //    //print(Vector3.Distance(start, dir));
    //    Debug.DrawRay(start, dir, Color.red);

    //}
    //private void OnDrawGizmos()
    //{
    //    Debug.DrawLine(start, hitPoint, Color.blue);

    //}
    //public void Enter(PlayerController enterPlayer, Collider collider)
    //{
    //    print("Enter!!");
    //    if (!CheckInRange(enterPlayer.transform)) return;
    //    if (enterPlayer.IsMyCharacter())
    //    {
    //        enterPlayer.playerUI.ChangeWarining(true);
    //    }
    //    if (_buffController.livingEntity.IsMyCharacter())
    //    {
    //        var playercontroller = _buffController.livingEntity.GetComponent<PlayerController>();
    //        if (playercontroller)
    //        {
    //            playercontroller.playerUI.ChangeWarining(true);
    //        }
    //    }
    //}



    //public void Exit(PlayerController exitPlayer, Collider collider)
    //{
    //    print("Exit!!");
    //    //if (!CheckInRange(exitPlayer.transform)) return;
    //    if (exitPlayer.IsMyCharacter())
    //    {
    //        exitPlayer.playerUI.ChangeWarining(false);
    //    }
    //    if (_buffController.livingEntity.IsMyCharacter())
    //    {
    //        var playercontroller = _buffController.livingEntity.GetComponent<PlayerController>();
    //        if (playercontroller)
    //        {
    //            playercontroller.playerUI.ChangeWarining(false);
    //        }
    //    }
    //}
}
