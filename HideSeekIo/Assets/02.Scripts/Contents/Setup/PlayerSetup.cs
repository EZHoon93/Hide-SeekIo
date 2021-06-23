﻿using System.Collections;

using UnityEngine;
using Photon.Pun;
using System;

public class PlayerSetup : MonoBehaviourPun, IPunInstantiateMagicCallback , IOnPhotonInstantiate
{
    public event Action OnPhotonInstantiateEvent;
    
    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (photonView.InstantiationData == null) return;   //데이터가없으면 null
        
        var nickName = (string)info.photonView.InstantiationData[0]; //닉네임
        var avaterId = (string)info.photonView.InstantiationData[1]; //캐릭터

        var playerController = this.GetComponent<PlayerController>();
        //-데이터받아옴//
        var avater = Managers.Resource.Instantiate($"Avater/{avaterId}",this.transform);
        avater.transform.ResetTransform();
        avater.GetOrAddComponent<Animator>().runtimeAnimatorController = GameSetting.Instance.GetRuntimeAnimatorController(playerController.Team);

        //유저자신의 캐릭이라면.
        if (this.IsMyCharacter())
        {
            CameraManager.Instance.SetupTarget(playerController.transform);
        }


        LayerChange(avater);
        playerController.OnPhotonInstantiate();
        OnPhotonInstantiateEvent?.Invoke();
        //ch?.Invoke()
    }

    protected virtual void LayerChange(GameObject gameObject )
    {

    }


    
}
