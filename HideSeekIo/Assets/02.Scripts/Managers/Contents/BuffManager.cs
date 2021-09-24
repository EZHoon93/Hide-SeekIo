using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class BuffManager : GenricSingleton<BuffManager>
{

    private void AddBuffController(LivingEntity livingEntity, BuffController buffController)
    {
        livingEntity.n_buffControllerList.Add(buffController);
        livingEntity.photonView.ObservedComponents.Add(buffController);
    }

    public void RemoveBuffController(LivingEntity livingEntity, BuffController buffController)
    {
        livingEntity.photonView.ObservedComponents.Remove(buffController);
        livingEntity.n_buffControllerList.Remove(buffController);
    }

    private BuffController MakeBuffController(LivingEntity livingEntity)
    {
        var buffController = Managers.Resource.Instantiate($"Buff/BuffController").GetComponent<BuffController>();
        buffController.transform.ResetTransform(livingEntity.transform);
        buffController.gameObject.SetLayerRecursively(livingEntity.gameObject.layer);
        return buffController;
    }
    //버프 이펙트 생성
    public BuffBase MakeBuffObject(Define.BuffType buffType, Transform target)
    {
        if (buffType == Define.BuffType.Null) return null;
        var buffBase = Managers.Resource.Instantiate($"Buff/{buffType.ToString()}", target).GetComponent<BuffBase>();
        buffBase.transform.ResetTransform(target);
        buffBase.gameObject.SetLayerRecursively(target.gameObject.layer);
        return buffBase;
    }



    /// <summary>
    /// 버프 아이템 사용시, 해당 버프가 이미 있는버프인지 체크, =>있는버프 재갱신 없는버프 => 생성
    /// 
    /// </summary>
    public void CheckBuffController(LivingEntity livingEntity, Define.BuffType buffType = Define.BuffType.Null)
    {
        if (livingEntity == null) return;
        if (livingEntity.Dead) return;
        var buffControllerList = livingEntity.n_buffControllerList;
        BuffController buffController = buffControllerList.Find(s => s.BuffType == buffType);
        //float durationTime = 10;
        float createServerTime = livingEntity.photonView.IsMine ? (float)PhotonNetwork.Time : 0;
        if (buffController == null)
        {
            buffController = MakeBuffController(livingEntity);
            AddBuffController(livingEntity, buffController);
            buffController.SetupLivingEntitiy(livingEntity);
            buffController.SetupInfo(buffType, createServerTime);
        }
        else
        {
            buffController.Renew(createServerTime);
        }
    }

}
