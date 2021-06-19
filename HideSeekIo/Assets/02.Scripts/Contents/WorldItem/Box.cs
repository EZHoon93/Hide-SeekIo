using System.Collections;

using UnityEngine;
using Photon.Pun;
public class Box : MonoBehaviourPun, IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy
{

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        double sendTime = info.SentServerTime;
        //생성된지 1초 이하면, 이펙트발생    => 생성시 서버로 이펙트 함수 호출이아닌, 생성시 전송시간이랑 비교해서 로컬로처리 (메시지 하나라도아끼려고)
        if (PhotonNetwork.Time <= sendTime + 1.0f)
        {
            EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, this.transform.position);
        }
    }

    public void OnPreNetDestroy(PhotonView rootView)
    {
        //게임 끝난상태가 아니라면 이펙트 => 로컬에서 처리
        if (GameManager.Instance.State != Define.GameState.End)
        {
            EffectManager.Instance.EffectOnLocal(Define.EffectType.CloudBurst, this.transform.position);
        }
    }
}
