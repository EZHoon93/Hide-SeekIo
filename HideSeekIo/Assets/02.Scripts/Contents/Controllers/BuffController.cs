
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

/// <summary>
/// 서버통신을위한 버프컨트롤러, 버프 대상마다 1개씩 존재. 
/// 
/// </summary>
public class BuffController : MonoBehaviourPun, IPunObservable 
{
    public List<BuffBase> buffBaseList { get;  } = new List<BuffBase>();

    public LivingEntity livingEntity;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(buffBaseList.Count);
            foreach(var buff in buffBaseList)
            {
                stream.SendNext(buff.buffType);
                stream.SendNext(buff.createTime);
                stream.SendNext(buff.durationTime);

            }
        }
        else
        {
            var n_buffCount = (int)stream.ReceiveNext();
            for(int i = 0; i < n_buffCount; i++)
            {
                var n_buffType = (Define.BuffType)stream.ReceiveNext();
                var n_createTime = (float)stream.ReceiveNext();
                var n_durationTime = (float)stream.ReceiveNext();
                Managers.buffManager.CheckBuff(this, n_buffType, n_createTime, n_durationTime);
            }
        }
    }

    public void Init(LivingEntity newLivingEntity)
    {
        livingEntity = newLivingEntity;
        Clear();
    }
    void Clear()
    {
        foreach(var buff in buffBaseList.ToArray())
        {
            Managers.Resource.Destroy(buff.gameObject);
        }
    }

    public void AddBuff(BuffBase newBuffBase)
    {
        if (buffBaseList.Contains(newBuffBase) == false)
        {
            buffBaseList.Add(newBuffBase);
        }
    }

    public void RemoveBuff(BuffBase newBuffBase)
    {
        if (buffBaseList.Contains(newBuffBase))
        {
            buffBaseList.Remove(newBuffBase);
        }
    }


}
