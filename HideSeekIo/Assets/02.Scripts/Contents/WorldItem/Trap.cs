using System.Collections;

using UnityEngine;
using Photon.Pun;
public class Trap : MonoBehaviourPun , IPunInstantiateMagicCallback
{
    [SerializeField] GameObject _modelObject;
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        

        double sendTime = info.SentServerTime;
        _modelObject.SetActive(true);
        //생성된지 1초 이하면, 이펙트발생    => 생성시 서버로 이펙트 함수 호출이아닌, 생성시 전송시간이랑 비교해서 로컬로처리 (메시지 하나라도아끼려고)
        //if(PhotonNetwork.Time <= sendTime + 1.0f)
        //{
        //    Managers.effectManager.EffectOnLocal(Define.EffectType.CloudBurst, this.transform.position,1);
        //}
    }


    public void TrapCollider(GameObject trapEnemeyObject)
    {
        //BuffManager.Instance.CheckBuffController(trapEnemeyObject.GetComponent<LivingEntity>(), Define.BuffType.B_Stun);

        Managers.effectManager.EffectOnLocal(Define.EffectType.CloudBurst, this.transform.position, 1);
        PhotonNetwork.Destroy(this.gameObject);
    }
}
