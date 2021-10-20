using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Weapon_SlingShot : Weapon_Bow
{
    
    private void Start()
    {
        AttackAnim = "Attack";
        AttackDelay = 0.25f;
        AfaterAttackDelay = 0.25f;
        AttackDistance = 5;
        inputControllerObject.InitCoolTime = 1;
        _damage = 10;
    }

    //public override void Attack(Vector2 inputVector)
    //{
    //    var endPoint = GetHitPoint(inputVector.normalized);
    //    photonView.RPC("AttackOnServer2", RpcTarget.All, endPoint);
    //}


    //[PunRPC]
    //public void AttackOnServer2(Vector3 endPoint)
    //{
    //    attackPoint = endPoint;
    //    _animator.SetTrigger("Attack");
    //    StartCoroutine(AttackProcessOnAllClinets(endPoint));
    //}

    //IEnumerator AttackProcessOnAllClinets(Vector3 endPoint)
    //{
    //    attackStartCallBack?.Invoke();
    //    yield return new WaitForSeconds(AttackDelay);   //대미지 주기전까지 시간
    //    var bulletProjectile = Managers.Pool.Pop(_projectilePrefab.gameObject).GetComponent<BulletProjectile>();
    //    Vector3 startPoint = playerController.playerCharacter.animator.GetBoneTransform(HumanBodyBones.RightHand).position;
    //    bulletProjectile.transform.position = startPoint;
    //    bulletProjectile.Play(startPoint, endPoint);
    //    yield return new WaitForSeconds(AfaterAttackDelay);   //대미지 주기전까지 시간
    //    attackEndCallBack?.Invoke();
    //}

}
