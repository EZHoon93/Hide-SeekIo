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
        go.transform.localPosition = Vector3.zero;
        go.gameObject.SetLayerRecursively(target.gameObject.layer);
        return go;
    }
    //버프 이펙트 생성
    public BuffBase MakeBuffObject(Define.BuffType buffType,Transform target)
    {
        if (buffType == Define.BuffType.Null) return null;
        var go = Managers.Resource.Instantiate($"Buff/{buffType.ToString()}",target).GetComponent<BuffBase>();
        go.transform.localPosition = Vector3.zero;
        go.gameObject.SetLayerRecursively(target.gameObject.layer);
        return go;
    }


    /// <summary>
    /// 버프 아이템 사용시, 해당 버프가 이미 있는버프인지 체크, =>있는버프 재갱신 없는버프 => 생성
    /// 로컬 유저만 사용
    /// </summary>
    /// <param name="buffController"></param>
    /// <param name="playerController"></param>
    public void BuffControllerCheckOnLocal(Define.BuffType buffType, LivingEntity livingEntity)
    {
        var buffControllerList = livingEntity.BuffControllerList;
        BuffController buffController = buffControllerList.Find(s => s.BuffType == buffType);
        float createServerTime = (float)PhotonNetwork.Time;
        float durationTime = 10;
        if (buffController == null)
        {
            buffController = MakeBuffController(livingEntity.transform);
            RegisterBuffControllerOnLivingEntity(buffController,livingEntity);
        }
        buffController.Setup(buffType, livingEntity, createServerTime, durationTime);
    }
    //최초 버프 생성시 로컬 및 서버에 사용
    public void RegisterBuffControllerOnLivingEntity(BuffController buffController , LivingEntity livingEntity)
    {
        livingEntity.BuffControllerList.Add(buffController);
        livingEntity.photonView.ObservedComponents.Add(buffController);
    }

    public void UnRegisterBuffControllerOnLivingEntity(BuffController buffController, LivingEntity livingEntity)
    {
        livingEntity.BuffControllerList.Remove(buffController);
        livingEntity.photonView.ObservedComponents.Remove(buffController);
    }

    //Hider 팀 전체에게 버프 적용
    public void HiderTeamBuffControllerToServer(Define.BuffType buffType, int useSeekrViewID)
    {
        photonView.RPC("HiderTeamBuffControllerOnLocal", RpcTarget.All, buffType, useSeekrViewID);
    }

    [PunRPC]
    public void HiderTeamBuffControllerOnLocal(Define.BuffType buffType, int useSeekrViewID)
    {
        //아이템 사용한술래에게 사용 이펙트
        var useSeekrPlayer = GameManager.Instance.GetLivingEntity(useSeekrViewID);  //아이템을 사용한 술래
        if (useSeekrPlayer)
        {
            EffectManager.Instance.EffectOnLocal(Define.EffectType.Curse, useSeekrPlayer.transform.position,0);
        }
     
        //방장은 AI에게도 버퍼
        if (PhotonNetwork.IsMasterClient)
        {
            var allLivingEntity = GameManager.Instance.GetAllLivingEntity();
            foreach(var livingEntity in allLivingEntity)
            {
                if(livingEntity.CompareTag("AI")  && livingEntity.gameObject.layer == (int)Define.Layer.Hider)
                {
                    BuffControllerCheckOnLocal(buffType, livingEntity);
                }
            }
        }
        //로컬조종중인 캐릭에게 버퍼
        var myPlayer = GameManager.Instance.myPlayer;
        if (myPlayer)
        {
            if (myPlayer.Team == Define.Team.Seek) return;   //술래팀은 적용X
            BuffControllerCheckOnLocal(buffType, myPlayer.livingEntity);
            EffectManager.Instance.EffectOnLocal(Define.EffectType.Curse, myPlayer.transform.position, 1);

        }
    }


}
