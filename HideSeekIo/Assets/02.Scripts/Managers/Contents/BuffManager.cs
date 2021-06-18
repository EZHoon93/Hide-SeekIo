using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class BuffManager : MonoBehaviourPun
{
    #region 싱글톤
    // 외부에서 싱글톤 오브젝트를 가져올때 사용할 프로퍼티
    public static BuffManager Instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                _instance = FindObjectOfType<BuffManager>();
            }

            // 싱글톤 오브젝트를 반환
            return _instance;
        }
    }
    private static BuffManager _instance; // 싱글톤이 할당될 static 변수
    #endregion


    public BuffController MakeBuffController(Transform target)
    {
        var go = Managers.Resource.Instantiate($"Buff/BuffController", target).GetComponent<BuffController>();
        return go;
    }
    //버프 이펙트 생성
    public BuffBase MakeBuffObject(Define.BuffType buffType,Transform target)
    {
        if (buffType == Define.BuffType.Null) return null;
        var go = Managers.Resource.Instantiate($"Buff/{buffType.ToString()}",target).GetComponent<BuffBase>();
        return go;
    }


    /// <summary>
    /// 버프 아이템 사용시, 해당 버프가 이미 있는버프인지 체크, =>있는버프 재갱신 없는버프 => 생성
    /// 로컬 유저만 사용
    /// </summary>
    /// <param name="buffController"></param>
    /// <param name="playerController"></param>
    public void BuffControllerCheckOnLocal(Define.BuffType buffType, PlayerController playerController)
    {
        var buffControllerList = playerController.GetLivingEntity().BuffControllerList;
        BuffController buffController = buffControllerList.Find(s => s.BuffType == buffType);
        float createServerTime = (float)PhotonNetwork.Time;
        float durationTime = 10;
        if (buffController == null)
        {
            var livingEntity = playerController.GetLivingEntity();
            buffController = MakeBuffController(livingEntity.transform);
            RegisterBuffControllerOnLivingEntity(buffController,livingEntity);
        }
        buffController.Setup(buffType, createServerTime, durationTime);
    }
    //최초 버프 생성시 로컬 및 서버에 사용
    public void RegisterBuffControllerOnLivingEntity(BuffController buffController , LivingEntity livingEntity)
    {
        livingEntity.BuffControllerList.Add(buffController);
        livingEntity.photonView.ObservedComponents.Add(buffController);
    }

    
}
