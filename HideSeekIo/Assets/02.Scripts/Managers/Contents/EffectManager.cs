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
    public void EffectOnLocal(Define.EffectType effectType, Vector3 position)
    {
        var go = Managers.Resource.Instantiate($"Effect/{effectType.ToString()}");
        go.transform.position = position;
        //���̾� ���������� ���ӾȺ��ӿ���
    }

    public void EffectToServer(Define.EffectType effectType , Vector3 position ) =>photonView.RPC("EffectOnLocal", RpcTarget.All, effectType, position);


    public void BuffToServer(Define.BuffType buffType , int livingViewID)
    {
        var livingEntity = GameManager.Instance.GetLivingEntity(livingViewID);

    }
    
}
