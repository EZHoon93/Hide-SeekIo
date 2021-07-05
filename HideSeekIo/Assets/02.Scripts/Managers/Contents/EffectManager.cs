using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class EffectManager : MonoBehaviourPun
{
    #region �̱���
    // �ܺο��� �̱��� ������Ʈ�� �����ö� ����� ������Ƽ
    public static EffectManager Instance
    {
        get
        {
            // ���� �̱��� ������ ���� ������Ʈ�� �Ҵ���� �ʾҴٸ�
            if (_instance == null)
            {
                // ������ GameManager ������Ʈ�� ã�� �Ҵ�
                _instance = FindObjectOfType<EffectManager>();
            }

            // �̱��� ������Ʈ�� ��ȯ
            return _instance;
        }
    }
    private static EffectManager _instance; // �̱����� �Ҵ�� static ����
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
        //���̾� ���������� ���ӾȺ��ӿ���
    }

    public void EffectToServer(Define.EffectType effectType , Vector3 position,int isSee ) 
        =>photonView.RPC("EffectOnLocal", RpcTarget.All, effectType, position, isSee);


    
    /// <summary>
    /// ��ü ����Ʈ, 
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

        print(livingEntitieList.Length + " ���٤���     ");

        foreach(var living in livingEntitieList)
        {
            EffectOnLocal(effectType, living.transform.position, isSee);
        }
    }
    
}
