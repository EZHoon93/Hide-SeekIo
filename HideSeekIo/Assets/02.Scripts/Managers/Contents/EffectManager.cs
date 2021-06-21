using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class EffectManager : MonoBehaviourPun
{
    #region 싱글톤
    // 외부에서 싱글톤 오브젝트를 가져올때 사용할 프로퍼티
    public static EffectManager Instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                _instance = FindObjectOfType<EffectManager>();
            }

            // 싱글톤 오브젝트를 반환
            return _instance;
        }
    }
    private static EffectManager _instance; // 싱글톤이 할당될 static 변수
    #endregion

    [PunRPC]
    public void EffectOnLocal(Define.EffectType effectType, Vector3 position,int isSee)
    {
        var go = Managers.Resource.Instantiate($"Effect/{effectType.ToString()}");
        go.transform.position = position;
        if(isSee == 0)
        {
            go.SetLayerRecursively((int)Define.Layer.Seeker);
        }
        else
        {
            go.SetLayerRecursively((int)Define.Layer.Hider);
        }
        //레이어 각팀에따라 보임안보임여부
    }

    public void EffectToServer(Define.EffectType effectType , Vector3 position,int isSee ) 
        =>photonView.RPC("EffectOnLocal", RpcTarget.All, effectType, position, isSee);


    public void BuffToServer(Define.BuffType buffType , int livingViewID)
    {
        var livingEntity = GameManager.Instance.GetLivingEntity(livingViewID);

    }
    
}
