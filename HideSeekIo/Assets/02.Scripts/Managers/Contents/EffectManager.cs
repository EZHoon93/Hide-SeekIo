using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class EffectManager : MonoBehaviourPun
{
    #region ??????
    // ???????? ?????? ?????????? ???????? ?????? ????????
    public static EffectManager Instance
    {
        get
        {
            // ???? ?????? ?????? ???? ?????????? ???????? ????????
            if (_instance == null)
            {
                // ?????? GameManager ?????????? ???? ????
                _instance = FindObjectOfType<EffectManager>();
            }

            // ?????? ?????????? ????
            return _instance;
        }
    }
    private static EffectManager _instance; // ???????? ?????? static ????
    #endregion

    /// <summary>
    /// 0은 포그오브워 밖에서도보이게..
    /// </summary>
    /// <param name="effectType"></param>
    /// <param name="position"></param>
    /// <param name="isSee"></param>
    /// <param name="size"></param>
    /// <param name="viewID"></param>
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
        //?????? ?????????? ??????????????
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


    
    
    /// <summary>
    /// ???? ??????, 
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

        print(livingEntitieList.Length + " ????????     ");

        foreach(var living in livingEntitieList)
        {
            EffectOnLocal(effectType, living.transform.position, isSee);
        }
    }
    
}
