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


    
    /// <summary>
    /// 전체 이펙트, 
    /// </summary>
    /// <param name="effectEventType"></param>
    /// <param name="effectType"></param>
    public void EffectAllLivingEntity(Define.EffectEventType effectEventType, Define.EffectType effectType)
    {
        //List<LivingEntity> livingEntitieList = null;
       
        LivingEntity[] livingEntitieList = null;
        int isSee = 0;
        switch (effectEventType)
        {
            case Define.EffectEventType.All:
                livingEntitieList = Managers.Game.GetAllLivingEntity();
                break;
            case Define.EffectEventType.Hider:
                livingEntitieList = Managers.Game.GetAllHiderList();
                isSee = 1;
                break;
            case Define.EffectEventType.Seeker:
                livingEntitieList = Managers.Game.GetAllSeekerList();
                isSee = 0;
                break;
        }

        print(livingEntitieList.Length + " 올잎ㄱㅌ     ");

        foreach(var living in livingEntitieList)
        {
            EffectOnLocal(effectType, living.transform.position, isSee);
        }
    }
    
}
