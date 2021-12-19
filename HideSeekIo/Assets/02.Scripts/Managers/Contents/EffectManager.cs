using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class EffectManager : MonoBehaviourPun
{
    private void Awake()
    {
        Managers.effectManager = this;
    }
    [PunRPC]
    public void EffectOnLocal(Define.EffectType effectType, Vector3 position,int isSee, float size = 1, int viewID = 0)
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
        if(viewID != 0)
        {
            go.GetComponent<FoW.FogOfWarUnit>().team = viewID;
        }
        if(size != 1)
        {
            go.transform.localScale = new Vector3(size, size, size);
        }
    }

    public void EffectToServer(Define.EffectType effectType , Vector3 position,int isSee ,float size = 1 ,int viewID = 0) 
        =>photonView.RPC("EffectOnLocal", RpcTarget.All, effectType, position, isSee, viewID);

    public void EffectAllLivingEntity(Define.EffectEventType effectEventType, Define.EffectType effectType)
    {
       
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
        foreach(var living in livingEntitieList)
        {
            EffectOnLocal(effectType, living.transform.position, isSee);
        }
    }
    
}
